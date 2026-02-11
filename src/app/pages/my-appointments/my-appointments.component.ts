import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AppointmentService } from '../../services/appointment.service';
import { Appointment } from '../../models/appointment.model';

@Component({
  selector: 'app-my-appointments',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="container">
      <div class="page-header">
        <h1>My Appointments</h1>
        <p>View and manage your booked appointments</p>
      </div>

      @if (loading()) {
        <div class="loading-spinner">
          <div class="spinner"></div>
          <p>Loading your appointments...</p>
        </div>
      } @else if (error()) {
        <div class="error-message">
          <span class="error-icon">⚠️</span>
          <p>{{ error() }}</p>
        </div>
      } @else if (appointments().length === 0) {
        <div class="card empty-state">
          <div class="empty-icon">📅</div>
          <h3>No Appointments Yet</h3>
          <p>You haven't booked any appointments. Browse available slots to get started!</p>
          <a href="/available-slots" class="btn btn-primary">Browse Available Slots</a>
        </div>
      } @else {
        <div class="appointments-grid">
          @for (appointment of appointments(); track appointment.id) {
            <div class="appointment-card">
              <div class="appointment-header">
                <h3>{{ appointment.clientName }}</h3>
                <span [class]="'badge badge-' + getStatusClass(appointment.status)">
                  {{ appointment.status}}
                </span>
              </div>
              
              <div class="appointment-details">
                <div class="detail-row">
                  <span class="label">📧 Email:</span>
                  <span class="value">{{ appointment.clientEmail }}</span>
                </div>
                <div class="detail-row">
                  <span class="label">📞 Phone:</span>
                  <span class="value">{{ appointment.clientPhone }}</span>
                </div>
                <div class="detail-row">
                  <span class="label">📅 Date & Time:</span>
                  <span class="value">{{ appointment.timeSlot ? formatDateTime(appointment.timeSlot.startTime) : 'N/A' }}</span>
                </div>
                <div class="detail-row">
                  <span class="label">⏱️ Duration:</span>
                  <span class="value">{{ appointment.timeSlot ? (formatTime(appointment.timeSlot.startTime) + ' - ' + formatTime(appointment.timeSlot.endTime)) : 'N/A' }}</span>
                </div>
                @if (appointment.notes) {
                  <div class="detail-row notes">
                    <span class="label">📝 Notes:</span>
                    <span class="value">{{ appointment.notes }}</span>
                  </div>
                }
              </div>

              <div class="appointment-actions">
                @if (appointment.status === 'Pending' || appointment.status === 'Confirmed') {
                  <button 
                    (click)="cancelAppointment(appointment.id)" 
                    class="btn btn-danger btn-sm"
                    [disabled]="cancelling()">
                    {{ cancelling() ? 'Cancelling...' : 'Cancel Appointment' }}
                  </button>
                }
              </div>
            </div>
          }
        </div>
      }
    </div>
  `,
  styles: [`
    .appointments-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
      gap: 1.5rem;
    }

    .appointment-card {
      background: white;
      border-radius: 12px;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
      padding: 1.5rem;
      transition: transform 0.2s ease, box-shadow 0.2s ease;
    }

    .appointment-card:hover {
      transform: translateY(-4px);
      box-shadow: 0 4px 16px rgba(0, 0, 0, 0.15);
    }

    .appointment-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 1rem;
      padding-bottom: 1rem;
      border-bottom: 2px solid #f5f7fa;
    }

    .appointment-header h3 {
      margin: 0;
      color: #2d3748;
    }

    .appointment-details {
      margin-bottom: 1.5rem;
    }

    .detail-row {
      display: flex;
      margin-bottom: 0.75rem;
      gap: 0.5rem;
    }

    .detail-row .label {
      font-weight: 600;
      color: #4a5568;
      min-width: 120px;
    }

    .detail-row .value {
      color: #2d3748;
      flex: 1;
    }

    .detail-row.notes {
      flex-direction: column;
    }

    .detail-row.notes .value {
      background: #f5f7fa;
      padding: 0.75rem;
      border-radius: 8px;
      margin-top: 0.5rem;
    }

    .appointment-actions {
      display: flex;
      gap: 1rem;
      padding-top: 1rem;
      border-top: 2px solid #f5f7fa;
    }

    .empty-state {
      text-align: center;
      padding: 3rem;
    }

    .empty-icon {
      font-size: 4rem;
      margin-bottom: 1rem;
    }

    .empty-state h3 {
      color: #2d3748;
      margin-bottom: 0.5rem;
    }

    .empty-state p {
      color: #718096;
      margin-bottom: 2rem;
    }

    @media (max-width: 768px) {
      .appointments-grid {
        grid-template-columns: 1fr;
      }

      .detail-row {
        flex-direction: column;
        gap: 0.25rem;
      }

      .detail-row .label {
        min-width: auto;
      }
    }
  `]
})
export class MyAppointmentsComponent implements OnInit {
  appointments = signal<Appointment[]>([]);
  loading = signal(false);
  error = signal('');
  cancelling = signal(false);

  constructor(private appointmentService: AppointmentService) {}

  ngOnInit(): void {
    this.loadAppointments();
  }

  loadAppointments(): void {
    this.loading.set(true);
    this.error.set('');

    this.appointmentService.getAllAppointments().subscribe({
      next: (response: any) => {
        if (response.success && response.data) {
          this.appointments.set(response.data);
        }
        this.loading.set(false);
      },
      error: (err: any) => {
        this.error.set('Failed to load appointments. Please try again.');
        this.loading.set(false);
        console.error('Error loading appointments:', err);
      }
    });
  }

  cancelAppointment(id: number): void {
    if (!confirm('Are you sure you want to cancel this appointment?')) {
      return;
    }

    this.cancelling.set(true);
    this.appointmentService.cancelAppointment(id).subscribe({
      next: (response: any) => {
        if (response.success) {
          this.appointments.set(
            this.appointments().filter(apt => apt.id !== id)
          );
        }
        this.cancelling.set(false);
      },
      error: (err: any) => {
        alert('Failed to cancel appointment. Please try again.');
        this.cancelling.set(false);
        console.error('Error cancelling appointment:', err);
      }
    });
  }

  getStatusClass(status: string): string {
    const statusMap: { [key: string]: string } = {
      'Pending': 'pending',
      'Confirmed': 'confirmed',
      'Cancelled': 'cancelled'
    };
    return statusMap[status] || 'pending';
  }

  formatDateTime(dateTime: string | Date): string {
    const date = new Date(dateTime);
    return date.toLocaleString('en-US', {
      weekday: 'long',
      year: 'numeric',
      month: 'long',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  formatTime(dateTime: string | Date): string {
    const date = new Date(dateTime);
    return date.toLocaleTimeString('en-US', {
      hour: '2-digit',
      minute: '2-digit'
    });
  }
}

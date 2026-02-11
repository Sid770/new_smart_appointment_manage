import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { AppointmentService } from '../../services/appointment.service';
import { TimeSlotService } from '../../services/time-slot.service';
import { TimeSlot } from '../../models/time-slot.model';

@Component({
  selector: 'app-book-appointment',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  template: `
    <div class="container">
      <div class="booking-wrapper">
        <div class="page-header">
          <h1>Book Your Appointment</h1>
          <p>Fill in your details to complete the booking</p>
        </div>

        @if (selectedSlot()) {
          <div class="slot-info-card">
            <h3>Selected Time Slot</h3>
            <div class="slot-details">
              <div class="detail-item">
                <span class="label">Provider:</span>
                <span class="value">{{ selectedSlot()!.serviceProvider }}</span>
              </div>
              <div class="detail-item">
                <span class="label">Date & Time:</span>
                <span class="value">{{ formatDateTime(selectedSlot()!.startTime) }}</span>
              </div>
              <div class="detail-item">
                <span class="label">Duration:</span>
                <span class="value">{{ formatTime(selectedSlot()!.startTime) }} - {{ formatTime(selectedSlot()!.endTime) }}</span>
              </div>
            </div>
          </div>
        }

        <div class="form-card">
          <form [formGroup]="bookingForm" (ngSubmit)="onSubmit()">
            <div class="form-group">
              <label for="clientName">Full Name *</label>
              <input 
                type="text" 
                id="clientName" 
                formControlName="clientName"
                class="form-control"
                placeholder="Enter your full name">
              @if (bookingForm.get('clientName')?.invalid && bookingForm.get('clientName')?.touched) {
                <div class="error-text">Name is required</div>
              }
            </div>

            <div class="form-group">
              <label for="clientEmail">Email Address *</label>
              <input 
                type="email" 
                id="clientEmail" 
                formControlName="clientEmail"
                class="form-control"
                placeholder="your.email@example.com">
              @if (bookingForm.get('clientEmail')?.invalid && bookingForm.get('clientEmail')?.touched) {
                <div class="error-text">Valid email is required</div>
              }
            </div>

            <div class="form-group">
              <label for="clientPhone">Phone Number *</label>
              <input 
                type="tel" 
                id="clientPhone" 
                formControlName="clientPhone"
                class="form-control"
                placeholder="+1 (555) 123-4567">
              @if (bookingForm.get('clientPhone')?.invalid && bookingForm.get('clientPhone')?.touched) {
                <div class="error-text">Phone number is required</div>
              }
            </div>

            <div class="form-group">
              <label for="serviceType">Service Type *</label>
              <select 
                id="serviceType" 
                formControlName="serviceType"
                class="form-control">
                <option value="">Select a service</option>
                <option value="Consultation">Consultation</option>
                <option value="Follow-up">Follow-up</option>
                <option value="General Checkup">General Checkup</option>
                <option value="Specialist Visit">Specialist Visit</option>
              </select>
              @if (bookingForm.get('serviceType')?.invalid && bookingForm.get('serviceType')?.touched) {
                <div class="error-text">Service type is required</div>
              }
            </div>

            <div class="form-group">
              <label for="notes">Additional Notes</label>
              <textarea 
                id="notes" 
                formControlName="notes"
                class="form-control"
                rows="4"
                placeholder="Any special requirements or information..."></textarea>
            </div>

            @if (errorMessage()) {
              <div class="alert alert-error">
                <span class="alert-icon">⚠️</span>
                {{ errorMessage() }}
              </div>
            }

            @if (successMessage()) {
              <div class="alert alert-success">
                <span class="alert-icon">✓</span>
                {{ successMessage() }}
              </div>
            }

            <div class="form-actions">
              <button 
                type="button" 
                (click)="goBack()" 
                class="btn btn-secondary"
                [disabled]="submitting()">
                Cancel
              </button>
              <button 
                type="submit" 
                class="btn btn-primary"
                [disabled]="bookingForm.invalid || submitting()">
                @if (submitting()) {
                  <span class="btn-spinner"></span>
                  Booking...
                } @else {
                  Confirm Booking
                }
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .container {
      min-height: 100vh;
      background: #f7fafc;
      padding: 2rem;
    }

    .booking-wrapper {
      max-width: 700px;
      margin: 0 auto;
    }

    .page-header {
      text-align: center;
      margin-bottom: 2rem;
    }

    .page-header h1 {
      font-size: 2.5rem;
      color: #2d3748;
      margin-bottom: 0.5rem;
    }

    .page-header p {
      color: #718096;
      font-size: 1.1rem;
    }

    .slot-info-card {
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      color: white;
      border-radius: 12px;
      padding: 1.5rem;
      margin-bottom: 1.5rem;
      box-shadow: 0 4px 20px rgba(102, 126, 234, 0.3);
    }

    .slot-info-card h3 {
      margin-bottom: 1rem;
      font-size: 1.3rem;
    }

    .slot-details {
      display: flex;
      flex-direction: column;
      gap: 0.75rem;
    }

    .detail-item {
      display: flex;
      justify-content: space-between;
      padding: 0.5rem 0;
      border-bottom: 1px solid rgba(255,255,255,0.2);
    }

    .detail-item:last-child {
      border-bottom: none;
    }

    .label {
      font-weight: 600;
      opacity: 0.9;
    }

    .value {
      font-weight: 500;
    }

    .form-card {
      background: white;
      border-radius: 12px;
      padding: 2rem;
      box-shadow: 0 2px 10px rgba(0,0,0,0.05);
    }

    .form-group {
      margin-bottom: 1.5rem;
    }

    .form-group label {
      display: block;
      font-weight: 600;
      color: #2d3748;
      margin-bottom: 0.5rem;
    }

    .form-control {
      width: 100%;
      padding: 0.75rem;
      border: 2px solid #e2e8f0;
      border-radius: 8px;
      font-size: 1rem;
      transition: all 0.3s ease;
      font-family: inherit;
    }

    .form-control:focus {
      outline: none;
      border-color: #667eea;
      box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
    }

    .form-control:disabled {
      background-color: #f7fafc;
      cursor: not-allowed;
    }

    .error-text {
      color: #e53e3e;
      font-size: 0.875rem;
      margin-top: 0.25rem;
    }

    .alert {
      padding: 1rem;
      border-radius: 8px;
      margin-bottom: 1rem;
      display: flex;
      align-items: center;
      gap: 0.75rem;
    }

    .alert-error {
      background: #fed7d7;
      color: #c53030;
    }

    .alert-success {
      background: #c6f6d5;
      color: #22543d;
    }

    .alert-icon {
      font-size: 1.5rem;
    }

    .form-actions {
      display: flex;
      gap: 1rem;
      justify-content: flex-end;
      margin-top: 2rem;
    }

    .btn {
      padding: 0.75rem 2rem;
      border: none;
      border-radius: 8px;
      font-weight: 600;
      cursor: pointer;
      transition: all 0.3s ease;
      font-size: 1rem;
      display: inline-flex;
      align-items: center;
      gap: 0.5rem;
    }

    .btn:disabled {
      opacity: 0.6;
      cursor: not-allowed;
    }

    .btn-primary {
      background: #667eea;
      color: white;
    }

    .btn-primary:hover:not(:disabled) {
      background: #5568d3;
      transform: translateY(-2px);
      box-shadow: 0 4px 12px rgba(102, 126, 234, 0.4);
    }

    .btn-secondary {
      background: #e2e8f0;
      color: #2d3748;
    }

    .btn-secondary:hover:not(:disabled) {
      background: #cbd5e0;
    }

    .btn-spinner {
      width: 16px;
      height: 16px;
      border: 2px solid rgba(255,255,255,0.3);
      border-top-color: white;
      border-radius: 50%;
      animation: spin 0.8s linear infinite;
    }

    @keyframes spin {
      to { transform: rotate(360deg); }
    }

    @media (max-width: 768px) {
      .container {
        padding: 1rem;
      }

      .form-actions {
        flex-direction: column;
      }

      .btn {
        width: 100%;
        justify-content: center;
      }
    }
  `]
})
export class BookAppointmentComponent implements OnInit {
  bookingForm: FormGroup;
  selectedSlot = signal<TimeSlot | null>(null);
  submitting = signal(false);
  errorMessage = signal<string | null>(null);
  successMessage = signal<string | null>(null);
  slotId: number = 0;

  constructor(
    private fb: FormBuilder,
    private appointmentService: AppointmentService,
    private timeSlotService: TimeSlotService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.bookingForm = this.fb.group({
      clientName: ['', Validators.required],
      clientEmail: ['', [Validators.required, Validators.email]],
      clientPhone: ['', Validators.required],
      serviceType: ['', Validators.required],
      notes: ['']
    });
  }

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      this.slotId = +params['slotId'];
      if (this.slotId) {
        this.loadSlotDetails();
      } else {
        this.errorMessage.set('No time slot selected');
      }
    });
  }

  loadSlotDetails(): void {
    this.timeSlotService.getTimeSlotById(this.slotId).subscribe({
      next: (response) => {
        this.selectedSlot.set(response.data);
      },
      error: (err) => {
        console.error('Error loading slot:', err);
        this.errorMessage.set('Failed to load slot details');
      }
    });
  }

  onSubmit(): void {
    if (this.bookingForm.valid && !this.submitting()) {
      this.submitting.set(true);
      this.errorMessage.set(null);
      this.successMessage.set(null);

      const appointmentData = {
        timeSlotId: this.slotId,
        ...this.bookingForm.value
      };

      this.appointmentService.createAppointment(appointmentData).subscribe({
        next: (response) => {
          this.successMessage.set('Appointment booked successfully!');
          setTimeout(() => {
            this.router.navigate(['/my-appointments']);
          }, 2000);
        },
        error: (err) => {
          console.error('Error booking appointment:', err);
          this.errorMessage.set(
            err.error?.message || 'Failed to book appointment. The slot may no longer be available.'
          );
          this.submitting.set(false);
        }
      });
    }
  }

  goBack(): void {
    this.router.navigate(['/available-slots']);
  }

  formatDateTime(dateTime: Date | string): string {
    const date = new Date(dateTime);
    return date.toLocaleDateString('en-US', { 
      weekday: 'long',
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    });
  }

  formatTime(dateTime: Date | string): string {
    const date = new Date(dateTime);
    return date.toLocaleTimeString('en-US', { 
      hour: '2-digit', 
      minute: '2-digit',
      hour12: true 
    });
  }
}

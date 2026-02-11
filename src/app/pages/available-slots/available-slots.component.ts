import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { TimeSlotService } from '../../services/time-slot.service';
import { TimeSlot } from '../../models/time-slot.model';

@Component({
  selector: 'app-available-slots',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="container">
      <div class="page-header">
        <h1>Available Time Slots</h1>
        <p>Select a time slot to book your appointment</p>
      </div>

      <div class="filters-card">
        <div class="filters">
          <div class="form-group">
            <label>Date</label>
            <input 
              type="date" 
              [(ngModel)]="selectedDate"
              (change)="loadAvailableSlots()"
              class="form-control">
          </div>
          <div class="form-group">
            <label>Service Provider</label>
            <select 
              [(ngModel)]="selectedProvider"
              (change)="loadAvailableSlots()"
              class="form-control">
              <option value="">All Providers</option>
              <option value="Dr. Smith">Dr. Smith</option>
              <option value="Dr. Johnson">Dr. Johnson</option>
            </select>
          </div>
        </div>
      </div>

      @if (loading()) {
        <div class="loading-spinner">
          <div class="spinner"></div>
          <p>Loading available slots...</p>
        </div>
      } @else if (error()) {
        <div class="error-message">
          <span class="error-icon">⚠️</span>
          <p>{{ error() }}</p>
          <button (click)="loadAvailableSlots()" class="btn btn-primary">Retry</button>
        </div>
      } @else if (timeSlots().length === 0) {
        <div class="empty-state">
          <span class="empty-icon">📅</span>
          <h3>No Available Slots</h3>
          <p>There are no available time slots matching your criteria.</p>
        </div>
      } @else {
        <div class="slots-grid">
          @for (slot of timeSlots(); track slot.id) {
            <div class="slot-card" (click)="selectSlot(slot)">
              <div class="slot-header">
                <span class="slot-provider">{{ slot.serviceProvider }}</span>
                <span class="badge badge-success">Available</span>
              </div>
              <div class="slot-time">
                <span class="time-icon">🕐</span>
                <div>
                  <div class="time">{{ formatTime(slot.startTime) }} - {{ formatTime(slot.endTime) }}</div>
                  <div class="date">{{ formatDate(slot.startTime) }}</div>
                </div>
              </div>
              <button class="btn btn-primary btn-block">Book This Slot</button>
            </div>
          }
        </div>
      }
    </div>
  `,
  styles: [`
    .container {
      max-width: 1200px;
      margin: 0 auto;
      padding: 2rem;
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

    .filters-card {
      background: white;
      border-radius: 12px;
      padding: 2rem;
      box-shadow: 0 2px 10px rgba(0,0,0,0.05);
      margin-bottom: 2rem;
    }

    .filters {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
      gap: 1.5rem;
    }

    .form-group {
      display: flex;
      flex-direction: column;
    }

    .form-group label {
      font-weight: 600;
      color: #2d3748;
      margin-bottom: 0.5rem;
    }

    .form-control {
      padding: 0.75rem;
      border: 2px solid #e2e8f0;
      border-radius: 8px;
      font-size: 1rem;
      transition: all 0.3s ease;
    }

    .form-control:focus {
      outline: none;
      border-color: #667eea;
      box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
    }

    .slots-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
      gap: 1.5rem;
    }

    .slot-card {
      background: white;
      border-radius: 12px;
      padding: 1.5rem;
      box-shadow: 0 2px 10px rgba(0,0,0,0.05);
      transition: all 0.3s ease;
      cursor: pointer;
      border: 2px solid transparent;
    }

    .slot-card:hover {
      transform: translateY(-5px);
      box-shadow: 0 8px 30px rgba(0,0,0,0.12);
      border-color: #667eea;
    }

    .slot-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 1rem;
    }

    .slot-provider {
      font-weight: 600;
      color: #2d3748;
      font-size: 1.1rem;
    }

    .badge {
      padding: 0.25rem 0.75rem;
      border-radius: 20px;
      font-size: 0.85rem;
      font-weight: 600;
    }

    .badge-success {
      background: #c6f6d5;
      color: #22543d;
    }

    .slot-time {
      display: flex;
      align-items: center;
      gap: 1rem;
      padding: 1rem;
      background: #f7fafc;
      border-radius: 8px;
      margin-bottom: 1rem;
    }

    .time-icon {
      font-size: 2rem;
    }

    .time {
      font-size: 1.2rem;
      font-weight: 600;
      color: #2d3748;
    }

    .date {
      font-size: 0.9rem;
      color: #718096;
      margin-top: 0.25rem;
    }

    .btn {
      padding: 0.75rem 1.5rem;
      border: none;
      border-radius: 8px;
      font-weight: 600;
      cursor: pointer;
      transition: all 0.3s ease;
      font-size: 1rem;
    }

    .btn-primary {
      background: #667eea;
      color: white;
    }

    .btn-primary:hover {
      background: #5568d3;
      transform: translateY(-2px);
    }

    .btn-block {
      width: 100%;
    }

    .loading-spinner {
      text-align: center;
      padding: 4rem;
    }

    .spinner {
      width: 50px;
      height: 50px;
      border: 4px solid #e2e8f0;
      border-top-color: #667eea;
      border-radius: 50%;
      animation: spin 1s linear infinite;
      margin: 0 auto 1rem;
    }

    @keyframes spin {
      to { transform: rotate(360deg); }
    }

    .error-message, .empty-state {
      text-align: center;
      padding: 4rem 2rem;
      background: white;
      border-radius: 12px;
    }

    .error-icon, .empty-icon {
      font-size: 4rem;
      display: block;
      margin-bottom: 1rem;
    }

    .empty-state h3 {
      color: #2d3748;
      margin-bottom: 0.5rem;
    }

    .empty-state p {
      color: #718096;
    }

    @media (max-width: 768px) {
      .container {
        padding: 1rem;
      }
      
      .slots-grid {
        grid-template-columns: 1fr;
      }
    }
  `]
})
export class AvailableSlotsComponent implements OnInit {
  timeSlots = signal<TimeSlot[]>([]);
  loading = signal(false);
  error = signal<string | null>(null);
  selectedDate: string = '';
  selectedProvider: string = '';

  constructor(
    private timeSlotService: TimeSlotService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadAvailableSlots();
  }

  loadAvailableSlots(): void {
    this.loading.set(true);
    this.error.set(null);

    const filter: any = {};
    if (this.selectedDate) {
      filter.date = this.selectedDate;
    }
    if (this.selectedProvider) {
      filter.serviceProvider = this.selectedProvider;
    }

    this.timeSlotService.getAvailableTimeSlots(filter).subscribe({
      next: (response) => {
        this.timeSlots.set(response.data || []);
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Error loading time slots:', err);
        this.error.set('Failed to load time slots. Please try again.');
        this.loading.set(false);
      }
    });
  }

  selectSlot(slot: TimeSlot): void {
    this.router.navigate(['/book-appointment'], { 
      queryParams: { slotId: slot.id }
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

  formatDate(dateTime: Date | string): string {
    const date = new Date(dateTime);
    return date.toLocaleDateString('en-US', { 
      weekday: 'long',
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    });
  }
}

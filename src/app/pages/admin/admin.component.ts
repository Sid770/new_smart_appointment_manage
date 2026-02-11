import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators, FormsModule } from '@angular/forms';
import { TimeSlotService } from '../../services/time-slot.service';
import { TimeSlot } from '../../models/time-slot.model';
import { ApiResponse } from '../../models/api-response.model';

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  template: `
    <div class="admin-container">
      <!-- Header -->
      <div class="page-header">
        <div>
          <h1 class="page-title">Admin Panel</h1>
          <p class="page-subtitle">Manage time slots and availability</p>
        </div>
        <button class="btn btn-primary" (click)="showAddSlotForm = !showAddSlotForm">
          <svg width="20" height="20" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4"/>
          </svg>
          Add Time Slot
        </button>
      </div>

      <!-- Add Slot Form -->
      <div class="card" *ngIf="showAddSlotForm">
        <div class="card-header">
          <h3>Create New Time Slot</h3>
        </div>
        <form [formGroup]="slotForm" (ngSubmit)="createTimeSlot()">
          <div class="form-row">
            <div class="form-group">
              <label for="slotDate">Date</label>
              <input 
                type="date" 
                id="slotDate"
                formControlName="slotDate"
                class="form-control"
                [min]="today"
              />
              <span class="error-message" *ngIf="slotForm.get('slotDate')?.invalid && slotForm.get('slotDate')?.touched">
                Date is required
              </span>
            </div>

            <div class="form-group">
              <label for="startTime">Start Time</label>
              <input 
                type="time" 
                id="startTime"
                formControlName="startTime"
                class="form-control"
              />
              <span class="error-message" *ngIf="slotForm.get('startTime')?.invalid && slotForm.get('startTime')?.touched">
                Start time is required
              </span>
            </div>

            <div class="form-group">
              <label for="endTime">End Time</label>
              <input 
                type="time" 
                id="endTime"
                formControlName="endTime"
                class="form-control"
              />
              <span class="error-message" *ngIf="slotForm.get('endTime')?.invalid && slotForm.get('endTime')?.touched">
                End time is required
              </span>
            </div>
          </div>

          <div class="form-actions">
            <button type="button" class="btn btn-secondary" (click)="cancelAddSlot()">
              Cancel
            </button>
            <button type="submit" class="btn btn-primary" [disabled]="slotForm.invalid || isSubmitting">
              {{ isSubmitting ? 'Creating...' : 'Create Slot' }}
            </button>
          </div>
        </form>
      </div>

      <!-- Loading State -->
      <div class="loading-container" *ngIf="isLoading">
        <div class="spinner"></div>
        <p>Loading time slots...</p>
      </div>

      <!-- Error Message -->
      <div class="alert alert-error" *ngIf="errorMessage">
        <svg width="20" height="20" fill="currentColor" viewBox="0 0 20 20">
          <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clip-rule="evenodd"/>
        </svg>
        <span>{{ errorMessage }}</span>
        <button (click)="errorMessage = ''" class="alert-close">&times;</button>
      </div>

      <!-- Success Message -->
      <div class="alert alert-success" *ngIf="successMessage">
        <svg width="20" height="20" fill="currentColor" viewBox="0 0 20 20">
          <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd"/>
        </svg>
        <span>{{ successMessage }}</span>
        <button (click)="successMessage = ''" class="alert-close">&times;</button>
      </div>

      <!-- Time Slots Table -->
      <div class="card" *ngIf="!isLoading">
        <div class="card-header">
          <h3>All Time Slots ({{ timeSlots.length }})</h3>
          <div class="filter-group">
            <label>
              <input type="radio" name="filter" value="all" [(ngModel)]="filterStatus" (change)="applyFilter()">
              All ({{ timeSlots.length }})
            </label>
            <label>
              <input type="radio" name="filter" value="available" [(ngModel)]="filterStatus" (change)="applyFilter()">
              Available ({{ getAvailableCount() }})
            </label>
            <label>
              <input type="radio" name="filter" value="booked" [(ngModel)]="filterStatus" (change)="applyFilter()">
              Booked ({{ getBookedCount() }})
            </label>
          </div>
        </div>

        <div class="table-responsive">
          <table class="table">
            <thead>
              <tr>
                <th>ID</th>
                <th>Date</th>
                <th>Start Time</th>
                <th>End Time</th>
                <th>Status</th>
                <th>Booked By</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr *ngFor="let slot of filteredTimeSlots">
                <td>{{ slot.id }}</td>
                <td>{{ formatDate(slot.startTime) }}</td>
                <td>{{ formatTime(slot.startTime) }}</td>
                <td>{{ formatTime(slot.endTime) }}</td>
                <td>
                  <span class="badge" [class.badge-success]="slot.isAvailable" [class.badge-danger]="!slot.isAvailable">
                    {{ slot.isAvailable ? 'Available' : 'Booked' }}
                  </span>
                </td>
                <td>{{ slot.serviceProvider }}</td>
                <td>
                  <div class="action-buttons">
                    <button 
                      class="btn-icon btn-icon-danger" 
                      (click)="deleteTimeSlot(slot.id)"
                      [disabled]="!slot.isAvailable"
                      title="Delete slot"
                    >
                      <svg width="16" height="16" fill="currentColor" viewBox="0 0 20 20">
                        <path fill-rule="evenodd" d="M9 2a1 1 0 00-.894.553L7.382 4H4a1 1 0 000 2v10a2 2 0 002 2h8a2 2 0 002-2V6a1 1 0 100-2h-3.382l-.724-1.447A1 1 0 0011 2H9zM7 8a1 1 0 012 0v6a1 1 0 11-2 0V8zm5-1a1 1 0 00-1 1v6a1 1 0 102 0V8a1 1 0 00-1-1z" clip-rule="evenodd"/>
                      </svg>
                    </button>
                    <button 
                      *ngIf="!slot.isAvailable"
                      class="btn-icon btn-icon-warning" 
                      (click)="makeAvailable(slot.id)"
                      title="Make available"
                    >
                      <svg width="16" height="16" fill="currentColor" viewBox="0 0 20 20">
                        <path fill-rule="evenodd" d="M4 2a1 1 0 011 1v2.101a7.002 7.002 0 0111.601 2.566 1 1 0 11-1.885.666A5.002 5.002 0 005.999 7H9a1 1 0 010 2H4a1 1 0 01-1-1V3a1 1 0 011-1zm.008 9.057a1 1 0 011.276.61A5.002 5.002 0 0014.001 13H11a1 1 0 110-2h5a1 1 0 011 1v5a1 1 0 11-2 0v-2.101a7.002 7.002 0 01-11.601-2.566 1 1 0 01.61-1.276z" clip-rule="evenodd"/>
                      </svg>
                    </button>
                  </div>
                </td>
              </tr>
              <tr *ngIf="filteredTimeSlots.length === 0">
                <td colspan="7" class="text-center">
                  <div class="empty-state">
                    <svg width="48" height="48" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z"/>
                    </svg>
                    <p>No time slots found</p>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

      <!-- Stats Cards -->
      <div class="stats-grid">
        <div class="stat-card">
          <div class="stat-icon stat-icon-blue">
            <svg width="24" height="24" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z"/>
            </svg>
          </div>
          <div class="stat-content">
            <h4>Total Slots</h4>
            <p class="stat-value">{{ timeSlots.length }}</p>
          </div>
        </div>

        <div class="stat-card">
          <div class="stat-icon stat-icon-green">
            <svg width="24" height="24" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"/>
            </svg>
          </div>
          <div class="stat-content">
            <h4>Available</h4>
            <p class="stat-value">{{ getAvailableCount() }}</p>
          </div>
        </div>

        <div class="stat-card">
          <div class="stat-icon stat-icon-red">
            <svg width="24" height="24" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z"/>
            </svg>
          </div>
          <div class="stat-content">
            <h4>Booked</h4>
            <p class="stat-value">{{ getBookedCount() }}</p>
          </div>
        </div>

        <div class="stat-card">
          <div class="stat-icon stat-icon-purple">
            <svg width="24" height="24" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z"/>
            </svg>
          </div>
          <div class="stat-content">
            <h4>Utilization</h4>
            <p class="stat-value">{{ getUtilization() }}%</p>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .admin-container {
      padding: 2rem;
      max-width: 1400px;
      margin: 0 auto;
    }

    .page-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 2rem;
      flex-wrap: wrap;
      gap: 1rem;
    }

    .form-row {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
      gap: 1.5rem;
      margin-bottom: 1.5rem;
    }

    .filter-group {
      display: flex;
      gap: 1.5rem;
      align-items: center;
    }

    .filter-group label {
      display: flex;
      align-items: center;
      gap: 0.5rem;
      cursor: pointer;
      font-size: 0.9rem;
      color: #64748b;
    }

    .filter-group input[type="radio"] {
      cursor: pointer;
    }

    .table-responsive {
      overflow-x: auto;
    }

    .table {
      width: 100%;
      border-collapse: collapse;
    }

    .table thead th {
      background: #f8fafc;
      padding: 1rem;
      text-align: left;
      font-weight: 600;
      color: #475569;
      border-bottom: 2px solid #e2e8f0;
      font-size: 0.875rem;
      text-transform: uppercase;
      letter-spacing: 0.05em;
    }

    .table tbody td {
      padding: 1rem;
      border-bottom: 1px solid #e2e8f0;
      color: #1e293b;
    }

    .table tbody tr:hover {
      background: #f8fafc;
    }

    .action-buttons {
      display: flex;
      gap: 0.5rem;
    }

    .btn-icon {
      padding: 0.5rem;
      border: none;
      border-radius: 0.375rem;
      cursor: pointer;
      transition: all 0.2s;
      background: transparent;
    }

    .btn-icon:hover:not(:disabled) {
      transform: scale(1.1);
    }

    .btn-icon:disabled {
      opacity: 0.5;
      cursor: not-allowed;
    }

    .btn-icon-danger {
      color: #ef4444;
    }

    .btn-icon-danger:hover:not(:disabled) {
      background: #fee2e2;
    }

    .btn-icon-warning {
      color: #f59e0b;
    }

    .btn-icon-warning:hover:not(:disabled) {
      background: #fef3c7;
    }

    .text-center {
      text-align: center;
    }

    .empty-state {
      padding: 3rem;
      text-align: center;
      color: #94a3b8;
    }

    .empty-state svg {
      margin: 0 auto 1rem;
      opacity: 0.5;
    }

    .stats-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
      gap: 1.5rem;
      margin-top: 2rem;
    }

    .stat-card {
      background: white;
      padding: 1.5rem;
      border-radius: 0.75rem;
      box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
      display: flex;
      align-items: center;
      gap: 1rem;
    }

    .stat-icon {
      width: 48px;
      height: 48px;
      border-radius: 0.75rem;
      display: flex;
      align-items: center;
      justify-content: center;
      flex-shrink: 0;
    }

    .stat-icon-blue {
      background: #dbeafe;
      color: #3b82f6;
    }

    .stat-icon-green {
      background: #d1fae5;
      color: #10b981;
    }

    .stat-icon-red {
      background: #fee2e2;
      color: #ef4444;
    }

    .stat-icon-purple {
      background: #e9d5ff;
      color: #a855f7;
    }

    .stat-content h4 {
      font-size: 0.875rem;
      color: #64748b;
      margin: 0 0 0.25rem 0;
      font-weight: 500;
    }

    .stat-value {
      font-size: 1.875rem;
      font-weight: 700;
      color: #1e293b;
      margin: 0;
    }

    @media (max-width: 768px) {
      .admin-container {
        padding: 1rem;
      }

      .page-header {
        flex-direction: column;
        align-items: flex-start;
      }

      .form-row {
        grid-template-columns: 1fr;
      }

      .filter-group {
        flex-direction: column;
        align-items: flex-start;
        gap: 0.75rem;
      }

      .table {
        font-size: 0.875rem;
      }

      .table thead th,
      .table tbody td {
        padding: 0.75rem 0.5rem;
      }
    }
  `]
})
export class AdminComponent implements OnInit {
  timeSlots: TimeSlot[] = [];
  filteredTimeSlots: TimeSlot[] = [];
  slotForm: FormGroup;
  showAddSlotForm = false;
  isLoading = false;
  isSubmitting = false;
  errorMessage = '';
  successMessage = '';
  today: string;
  filterStatus: 'all' | 'available' | 'booked' = 'all';

  constructor(
    private fb: FormBuilder,
    private timeSlotService: TimeSlotService
  ) {
    const now = new Date();
    this.today = now.toISOString().split('T')[0];

    this.slotForm = this.fb.group({
      slotDate: ['', Validators.required],
      startTime: ['', Validators.required],
      endTime: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    this.loadTimeSlots();
  }

  loadTimeSlots(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.timeSlotService.getAllTimeSlots().subscribe({
      next: (response: ApiResponse<TimeSlot[]>) => {
        this.timeSlots = response.data || [];
        this.applyFilter();
        this.isLoading = false;
      },
      error: (error) => {
        this.errorMessage = 'Failed to load time slots. Please try again.';
        this.isLoading = false;
        console.error('Error loading time slots:', error);
      }
    });
  }

  createTimeSlot(): void {
    if (this.slotForm.invalid) {
      Object.keys(this.slotForm.controls).forEach(key => {
        this.slotForm.get(key)?.markAsTouched();
      });
      return;
    }

    this.isSubmitting = true;
    this.errorMessage = '';
    this.successMessage = '';

    const formValue = this.slotForm.value;
    
    // Combine date and time for startTime and endTime
    const date = new Date(formValue.slotDate);
    const [startHour, startMinute] = formValue.startTime.split(':');
    const [endHour, endMinute] = formValue.endTime.split(':');
    
    const startDateTime = new Date(date);
    startDateTime.setHours(parseInt(startHour), parseInt(startMinute), 0, 0);
    
    const endDateTime = new Date(date);
    endDateTime.setHours(parseInt(endHour), parseInt(endMinute), 0, 0);
    
    const slotData = {
      startTime: startDateTime.toISOString(),
      endTime: endDateTime.toISOString(),
      serviceProvider: 'Default Provider' // TODO: Add provider selection to form
    };

    this.timeSlotService.createTimeSlot(slotData).subscribe({
      next: (response: ApiResponse<TimeSlot>) => {
        this.successMessage = 'Time slot created successfully!';
        this.slotForm.reset();
        this.showAddSlotForm = false;
        this.loadTimeSlots();
        this.isSubmitting = false;
        setTimeout(() => this.successMessage = '', 3000);
      },
      error: (error) => {
        this.errorMessage = error.error?.message || 'Failed to create time slot. Please check the time range.';
        this.isSubmitting = false;
        console.error('Error creating time slot:', error);
      }
    });
  }

  deleteTimeSlot(id: number): void {
    if (!confirm('Are you sure you want to delete this time slot?')) {
      return;
    }

    this.timeSlotService.deleteTimeSlot(id).subscribe({
      next: (response: ApiResponse<boolean>) => {
        this.successMessage = 'Time slot deleted successfully!';
        this.loadTimeSlots();
        setTimeout(() => this.successMessage = '', 3000);
      },
      error: (error) => {
        this.errorMessage = error.error?.message || 'Failed to delete time slot.';
        console.error('Error deleting time slot:', error);
      }
    });
  }

  makeAvailable(id: number): void {
    if (!confirm('Are you sure you want to make this slot available again? This will cancel the existing appointment.')) {
      return;
    }

    // Call API to make slot available (you'll need to add this endpoint)
    this.timeSlotService.makeSlotAvailable(id).subscribe({
      next: () => {
        this.successMessage = 'Time slot is now available!';
        this.loadTimeSlots();
        setTimeout(() => this.successMessage = '', 3000);
      },
      error: (error) => {
        this.errorMessage = 'Failed to update time slot status.';
        console.error('Error making slot available:', error);
      }
    });
  }

  cancelAddSlot(): void {
    this.showAddSlotForm = false;
    this.slotForm.reset();
    this.errorMessage = '';
  }

  applyFilter(): void {
    if (this.filterStatus === 'all') {
      this.filteredTimeSlots = [...this.timeSlots];
    } else if (this.filterStatus === 'available') {
      this.filteredTimeSlots = this.timeSlots.filter(slot => slot.isAvailable);
    } else {
      this.filteredTimeSlots = this.timeSlots.filter(slot => !slot.isAvailable);
    }
  }

  getAvailableCount(): number {
    return this.timeSlots.filter(slot => slot.isAvailable).length;
  }

  getBookedCount(): number {
    return this.timeSlots.filter(slot => !slot.isAvailable).length;
  }

  getUtilization(): number {
    if (this.timeSlots.length === 0) return 0;
    return Math.round((this.getBookedCount() / this.timeSlots.length) * 100);
  }

  formatDate(dateString: string | Date): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', { 
      weekday: 'short', 
      year: 'numeric', 
      month: 'short', 
      day: 'numeric' 
    });
  }

  formatTime(dateString: string | Date): string {
    const date = new Date(dateString);
    return date.toLocaleTimeString('en-US', { 
      hour: '2-digit', 
      minute: '2-digit',
      hour12: false
    });
  }
}

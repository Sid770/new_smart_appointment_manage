import { TimeSlot } from './time-slot.model';

export interface Appointment {
  id: number;
  timeSlotId: number;
  clientName: string;
  clientEmail: string;
  clientPhone: string;
  serviceType: string;
  notes: string;
  status: AppointmentStatus;
  bookedAt: Date | string;
  timeSlot?: TimeSlot;
}

export interface CreateAppointment {
  timeSlotId: number;
  clientName: string;
  clientEmail: string;
  clientPhone: string;
  serviceType: string;
  notes: string;
}

export enum AppointmentStatus {
  Pending = 'Pending',
  Confirmed = 'Confirmed',
  Cancelled = 'Cancelled',
  Completed = 'Completed'
}

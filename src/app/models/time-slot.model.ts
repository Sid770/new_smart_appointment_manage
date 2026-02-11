export interface TimeSlot {
  id: number;
  startTime: Date | string;
  endTime: Date | string;
  serviceProvider: string;
  isAvailable: boolean;
}

export interface CreateTimeSlot {
  startTime: Date | string;
  endTime: Date | string;
  serviceProvider: string;
}

export interface TimeSlotFilter {
  date?: Date | string;
  serviceProvider?: string;
  isAvailable?: boolean;
}

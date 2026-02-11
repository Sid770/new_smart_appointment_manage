import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { TimeSlot, CreateTimeSlot, TimeSlotFilter } from '../models/time-slot.model';
import { ApiResponse } from '../models/api-response.model';

@Injectable({
  providedIn: 'root'
})
export class TimeSlotService {
  private readonly baseUrl = `${environment.apiUrl}/timeslots`;

  constructor(private http: HttpClient) {}

  getAllTimeSlots(): Observable<ApiResponse<TimeSlot[]>> {
    return this.http.get<ApiResponse<TimeSlot[]>>(this.baseUrl);
  }

  getAvailableTimeSlots(filter?: TimeSlotFilter): Observable<ApiResponse<TimeSlot[]>> {
    let params = new HttpParams();
    
    if (filter?.date) {
      params = params.set('date', filter.date.toString());
    }
    if (filter?.serviceProvider) {
      params = params.set('serviceProvider', filter.serviceProvider);
    }
    if (filter?.isAvailable !== undefined) {
      params = params.set('isAvailable', filter.isAvailable.toString());
    }
    
    return this.http.get<ApiResponse<TimeSlot[]>>(`${this.baseUrl}/available`, { params });
  }

  getTimeSlotById(id: number): Observable<ApiResponse<TimeSlot>> {
    return this.http.get<ApiResponse<TimeSlot>>(`${this.baseUrl}/${id}`);
  }

  createTimeSlot(timeSlot: CreateTimeSlot): Observable<ApiResponse<TimeSlot>> {
    return this.http.post<ApiResponse<TimeSlot>>(this.baseUrl, timeSlot);
  }

  updateTimeSlot(id: number, timeSlot: CreateTimeSlot): Observable<ApiResponse<TimeSlot>> {
    return this.http.put<ApiResponse<TimeSlot>>(`${this.baseUrl}/${id}`, timeSlot);
  }

  deleteTimeSlot(id: number): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(`${this.baseUrl}/${id}`);
  }

  makeSlotAvailable(id: number): Observable<ApiResponse<boolean>> {
    return this.http.put<ApiResponse<boolean>>(`${this.baseUrl}/${id}/make-available`, {});
  }
}

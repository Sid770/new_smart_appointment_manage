import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Appointment, CreateAppointment, AppointmentStatus } from '../models/appointment.model';
import { ApiResponse } from '../models/api-response.model';

@Injectable({
  providedIn: 'root'
})
export class AppointmentService {
  private readonly baseUrl = `${environment.apiUrl}/appointments`;

  constructor(private http: HttpClient) {}

  getAllAppointments(): Observable<ApiResponse<Appointment[]>> {
    return this.http.get<ApiResponse<Appointment[]>>(this.baseUrl);
  }

  getAppointmentById(id: number): Observable<ApiResponse<Appointment>> {
    return this.http.get<ApiResponse<Appointment>>(`${this.baseUrl}/${id}`);
  }

  getAppointmentsByStatus(status: AppointmentStatus): Observable<ApiResponse<Appointment[]>> {
    return this.http.get<ApiResponse<Appointment[]>>(`${this.baseUrl}/status/${status}`);
  }

  createAppointment(appointment: CreateAppointment): Observable<ApiResponse<Appointment>> {
    return this.http.post<ApiResponse<Appointment>>(this.baseUrl, appointment);
  }

  updateAppointmentStatus(id: number, status: AppointmentStatus): Observable<ApiResponse<Appointment>> {
    return this.http.patch<ApiResponse<Appointment>>(
      `${this.baseUrl}/${id}/status`, 
      { status }
    );
  }

  cancelAppointment(id: number): Observable<ApiResponse<boolean>> {
    return this.http.post<ApiResponse<boolean>>(`${this.baseUrl}/${id}/cancel`, {});
  }
}

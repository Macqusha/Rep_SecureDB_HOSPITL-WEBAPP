import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Time } from '@angular/common';

@Component({
  selector: 'app-doctor',
  templateUrl: './doctor.component.html'
})
export class DoctorComponent {
  public appointments: DoctorAppointmentView[];
  public patients: DoctorPatientView[];

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    http.get<DoctorAppointmentView[]>(baseUrl + 'api/Doctor/Appointment'+'?DoctorID=101').subscribe(result => {
      this.appointments = result;
    },
      error => console.error(error));
        
    http.get<DoctorPatientView[]>(baseUrl + 'api/Doctor/Patient' + '?DoctorID=101').subscribe(result => {
      this.patients = result;
    },
      error => console.error(error));  
  }
}

interface DoctorAppointmentView {
  apptime: Date;
  cabinet: number;
  name: string;
  phone: number;
  bd: Date;
  passportserial: number;
  passportnumber: number;
  room: number;
  patientid: number;
}

interface DoctorPatientView {  
  room: number;
  places: number;
  name: string;
  phone: number;
  bd: string;
  passportserial: number;
  passportnumber: number;
  arrival: string;
  depature: string;
  patientid: number;
}

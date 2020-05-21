import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Time } from '@angular/common';

@Component({
  selector: 'app-patient',
  templateUrl: './patient.component.html'
})
export class PatientComponent {
  public diagnoses: PatientDiagnosisView[];
  public rooms: PatientRoomView[];
  public appointments: PatientAppointmentView[];
  public doctors: PatientDoctorView[];


  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    http.get<PatientDiagnosisView[]>(baseUrl + 'api/Patient/Diagnosis'+'?PatientID=1004').subscribe(result => {
      this.diagnoses = result;
    },
      error => console.error(error));

    http.get<PatientRoomView[]>(baseUrl + 'api/Patient/Room' + '?PatientID=1004').subscribe(result => {
      this.rooms = result;
    },
      error => console.error(error));

    http.get<PatientAppointmentView[]>(baseUrl + 'api/Patient/Appointment' + '?PatientID=1004').subscribe(result => {
      this.appointments = result;
    },
      error => console.error(error));

    http.get<PatientDoctorView[]>(baseUrl + 'api/Patient/Doctor').subscribe(result => {
      this.doctors = result;
    },
      error => console.error(error));
  }
}

interface PatientDiagnosisView {
  Name: string;
  Treatment: string;
}

interface PatientRoomView {
  Number: number;
  Places: number;
  Doctor: string;
  WorkStart: Time;
  WorkEnd: Time;
  Position: string;
}

interface PatientAppointmentView {
  AppTime: Date;
  Name: string;
  Cabinet: number;
  DoctorID: number;
}

interface PatientDoctorView {
  Position: string;
  Name: string;
  WorkStart: Time;
  WorkEnd: Time;
  Hired: Date;
}

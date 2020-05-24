import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Time } from '@angular/common';
import { DialogService } from 'primeng/dynamicdialog';
import { MakeAppointmentComponent } from './makeAppointment/makeAppointment.component';

@Component({
  selector: 'app-patient',
  templateUrl: './patient.component.html',
  providers: [DialogService]
})
export class PatientComponent {
  public diagnoses: PatientDiagnosisView[];
  public rooms: PatientRoomView[];
  public appointments: PatientAppointmentView[];
  public doctors: PatientDoctorView[];

  makeAppointment(id, name, start, end) {
    const ref = this.dialogService.open(MakeAppointmentComponent, {
      header: 'Запись на прием к доктору ' + name,
      width: '70%',
      data: {
        doctorID: id,
        workStart: start,
        workEnd: end,
      },
    })
    ref.onDestroy.subscribe(() => {
      this.myLoadingFunction()
    });
  }
  
  cancelAppointment(key) {
    this.http.delete(this.baseUrl + 'api/Patient/DelAppointment' + '?Key=' + key).subscribe(result => {
      this.http.get<PatientAppointmentView[]>(this.baseUrl + 'api/Patient/Appointment' + '?PatientID=1004').subscribe(result => {
        this.appointments = result;
      },
        error => console.error(error));
    },
      error => console.error(error));
  }

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string,
    public dialogService: DialogService,) {
    this.myLoadingFunction();
  }

  myLoadingFunction() {
    this.http.get<PatientDiagnosisView[]>(this.baseUrl + 'api/Patient/Diagnosis' + '?PatientID=1004').subscribe(result => {
      this.diagnoses = result;
    },
      error => console.error(error));

    this.http.get<PatientRoomView[]>(this.baseUrl + 'api/Patient/Room' + '?PatientID=1004').subscribe(result => {
      this.rooms = result;
    },
      error => console.error(error));

    this.http.get<PatientAppointmentView[]>(this.baseUrl + 'api/Patient/Appointment' + '?PatientID=1004').subscribe(result => {
      this.appointments = result;
    },
      error => console.error(error));

    this.http.get<PatientDoctorView[]>(this.baseUrl + 'api/Patient/Doctor').subscribe(result => {
      this.doctors = result;
    },
      error => console.error(error));
  }
}

interface PatientDiagnosisView {
  Name: string;
  Treatment: string;
  Code: string;
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
  Key: number;
  AppTime: Date;
  Name: string;
  Cabinet: number;
  DoctorID: number;
}

interface PatientDoctorView {
  id: number;
  Position: string;
  Name: string;
  WorkStart: Time;
  WorkEnd: Time;
  Hired: Date;
}

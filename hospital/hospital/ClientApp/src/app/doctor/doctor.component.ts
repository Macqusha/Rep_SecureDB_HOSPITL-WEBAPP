import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Time } from '@angular/common';
import { DialogService } from 'primeng/dynamicdialog';
import { MedRecordComponent } from './medRecord/medRecord.component';


@Component({
  selector: 'app-doctor',
  templateUrl: './doctor.component.html',
  providers: [DialogService]
})
export class DoctorComponent {
  currentUser: UserData;
  public appointments: DoctorAppointmentView[];
  public patients: DoctorPatientView[];

  medRecordClick(id, name, room) {
    const ref = this.dialogService.open(MedRecordComponent, {
      header: 'Медицинская карта пациента ' + name,
      width: '70%',
      data: {
        patientID: id,
        room: room,
      },
    })
    ref.onDestroy.subscribe(() => {
      this.myLoadingFunction()
    });
  }

  cancelAppointment(key) {
    this.http.delete(this.baseUrl + 'api/Patient/DelAppointment' + '?Key=' + key).subscribe(result => {
      this.http.get<DoctorAppointmentView[]>(this.baseUrl + 'api/Doctor/Appointment' + '?DoctorID=' + this.currentUser.id).subscribe(result => {
        this.appointments = result;
      },
        error => console.error(error));
    },
      error => console.error(error));
  }

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string,
    public dialogService: DialogService,
  ) {
    this.myLoadingFunction();
  }

  myLoadingFunction() {
    this.currentUser = JSON.parse(localStorage.getItem('currentUser'));

    this.http.get<DoctorAppointmentView[]>(this.baseUrl + 'api/Doctor/Appointment' + '?DoctorID=' + this.currentUser.id).subscribe(result => {
      this.appointments = result;
    },
      error => console.error(error));

    this.http.get<DoctorPatientView[]>(this.baseUrl + 'api/Doctor/Patient' + '?DoctorID=' + this.currentUser.id).subscribe(result => {
      this.patients = result;
    },
      error => console.error(error));
  }
}

interface DoctorAppointmentView {
  key: number;
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

interface UserData {
  id: number | undefined;
  name: string | undefined;
  role: string | undefined;
}

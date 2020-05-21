import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Time } from '@angular/common';
import { concat } from 'rxjs';

@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html'
})
export class AdminComponent {
  public doctors: AdminDoctorView[];
  public patients: AdminPatientView[];
  public cabinets: AdminCabinetView[];
  public rooms: AdminRoomView[];
  public diseases: AdminDiseaseView[];
  public positions: AdminPositionView[];
  public auths: AdminAuthView[];
  public audits: AdminAuditView[];


  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    http.get<AdminDoctorView[]>(baseUrl + 'api/Admin/Doctor' + '?AdminID=1').subscribe(result => {
      this.doctors = result;
    },
      error => console.error(error));

    http.get<AdminPatientView[]>(baseUrl + 'api/Admin/Patient' + '?AdminID=1').subscribe(result => {
      this.patients = result;
    },
      error => console.error(error));

    http.get<AdminCabinetView[]>(baseUrl + 'api/Admin/Cabinet' + '?AdminID=1').subscribe(result => {
      this.cabinets = result;
    },
      error => console.error(error));

    http.get<AdminRoomView[]>(baseUrl + 'api/Admin/Room' + '?AdminID=1').subscribe(result => {
      this.rooms = result;
    },
      error => console.error(error));

    http.get<AdminDiseaseView[]>(baseUrl + 'api/Admin/Disease' + '?AdminID=1').subscribe(result => {
      this.diseases = result;
    },
      error => console.error(error));

    http.get<AdminPositionView[]>(baseUrl + 'api/Admin/Position' + '?AdminID=1').subscribe(result => {
      this.positions = result;
    },
      error => console.error(error));

    http.get<AdminAuthView[]>(baseUrl + 'api/Admin/Auth' + '?AdminID=1').subscribe(result => {
      this.auths = result;
    },
      error => console.error(error));

    http.get<AdminAuditView[]>(baseUrl + 'api/Admin/Audit' + '?AdminID=1').subscribe(result => {
      this.audits = result;
    },
      error => console.error(error));
  }
}

interface AdminDoctorView {
  doctorid: number;
  name: string
  phone: number;
  address: string;
  bd: Date;
  workstart: Time;
  workend: Time;
  position: string;
}

interface AdminPatientView {
  patientid: number;
  name: string
  passportnumber: number;  
  passportserial: number;
}

interface AdminCabinetView {
  number: number;
}

interface AdminRoomView {
  number: number;
  fixeddoctor: number;
  name: string;
  places: number;
  free: number;
}

interface AdminDiseaseView {
  code: string;
  name: string;
  treatment: string;
}

interface AdminPositionView {
  key: number;
  name: string;
  salary: number;
}

interface AdminAuthView {
  login: string;
  passwordhash: string;
  token: string;
  id: number;
}

interface AdminAuditView {
  id: number;
  login: string;
  action: string;
  acttime: Date;
}

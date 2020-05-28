import { Component, Inject, ViewChild } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Time } from '@angular/common';
import { concat } from 'rxjs';

@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html'
})
export class AdminComponent {
  currentUser: UserData;
  public doctors: AdminDoctorView[];
  public patients: AdminPatientView[];
  public cabinets: AdminCabinetView[];
  public rooms: AdminRoomView[];
  public diseases: AdminDiseaseView[];
  public positions: AdminPositionView[];
  public auths: AdminAuthView[];
  public audits: AdminAuditView[];
  cabinet: number | undefined;
  roomNum: number | undefined;
  roomPlaces: number | undefined;
  diseaseCode: string | undefined;
  diseaseName: string | undefined;
  diseaseTreat: string | undefined;
  posName: string | undefined;
  posSalary: number | undefined;
  docName: string | undefined;
  docPhone: number | undefined;
  docAddress: string | undefined;
  docBD: Date | undefined;
  docWorkStart: Time | undefined;
  docWorkEnd: Time | undefined;

  addCab() {
    if (this.cabinet > 0 && this.cabinet <= 299 && !this.cabinets.find((v) => v.number === this.cabinet)) {
      this.http.post<[]>(this.baseUrl + 'api/Admin/AddCabinet' + '?Cabinet=' + this.cabinet, {}).subscribe(result => {
        this.http.get<AdminCabinetView[]>(this.baseUrl + 'api/Admin/Cabinet' + '?AdminID=' + this.currentUser.id).subscribe(result => {
          this.cabinets = result;
        },
          error => console.error(error));

        this.cabinet = undefined;

      },
        error => console.error(error));
    } else {
      alert('Невозможно добавить кабинет с таким номером.');
      this.cabinet = undefined;
    }
  }

  delCab(cabNum) {
    this.http.delete<[]>(this.baseUrl + 'api/Admin/DeleteCabinet' + '?Cabinet=' + cabNum, {}).subscribe(result => {
      this.http.get<AdminCabinetView[]>(this.baseUrl + 'api/Admin/Cabinet' + '?AdminID=' + this.currentUser.id).subscribe(result => {
        this.cabinets = result;
      },
        error => console.error(error));
    },
      error => console.error(error));
  }

  @ViewChild('selecteddoc', { static: false })
  private selecteddoc;
  addRoom() {
    if (this.roomNum >= 300 && this.roomNum <= 500 && !this.rooms.find((v) => v.number === this.roomNum) && this.roomPlaces > 0
      && this.selecteddoc.nativeElement.value >= 100) {
      this.http.post<[]>(this.baseUrl + 'api/Admin/AddRoom' + '?Number=' + this.roomNum + '&Places=' + this.roomPlaces + '&Doctor=' +
        this.selecteddoc.nativeElement.value, {}).subscribe(result => {

          this.http.get<AdminRoomView[]>(this.baseUrl + 'api/Admin/Room' + '?AdminID=' + this.currentUser.id).subscribe(result => {
            this.rooms = result;
          },
            error => console.error(error));

          this.roomNum = undefined;
          this.roomPlaces = undefined;
        },
          error => console.error(error));
    } else {
      alert('Данные введены неверно. Палаты нумеруются числами от 300 до 500 без повторений. ');
      this.roomNum = undefined;
      this.roomPlaces = undefined;
    }


  }

  delRoom(roomNum) {
    this.http.delete<[]>(this.baseUrl + 'api/Admin/DeleteRoom' + '?Number=' + roomNum, {}).subscribe(result => {
      this.http.get<AdminRoomView[]>(this.baseUrl + 'api/Admin/Room' + '?AdminID=' + this.currentUser.id).subscribe(result => {
        this.rooms = result;
      },
        error => console.error(error));
    },
      error => console.error(error));
  }

  addDisease() {
    if (this.diseaseCode != undefined && this.diseaseName != undefined && this.diseaseTreat != undefined
      && !this.diseases.find((v) => v.code === this.diseaseCode) && !this.diseases.find((v) => v.name === this.diseaseName)) {
      this.http.post<[]>(this.baseUrl + 'api/Admin/AddDisease' + '?Code=' + this.diseaseCode + "&Name=" + this.diseaseName
        + "&Treat=" + this.diseaseTreat, {}).subscribe(result => {

          this.http.get<AdminDiseaseView[]>(this.baseUrl + 'api/Admin/Disease' + '?AdminID=' + this.currentUser.id).subscribe(result => {
            this.diseases = result;
          },
            error => console.error(error));

          this.diseaseCode = undefined;
          this.diseaseName = undefined;
          this.diseaseTreat = undefined;

        },
          error => console.error(error));
    } else {
      alert('Данные пусты или дублируют имеющиеся в базе.');
    }
  }

  changeSal(key, salary) {
    if (salary > 0) {
      this.http.post<[]>(this.baseUrl + 'api/Admin/ChangeSalary' + '?Key=' + key + "&Salary=" + salary, {}).subscribe(result => {

        this.http.get<AdminPositionView[]>(this.baseUrl + 'api/Admin/Position' + '?AdminID=' + this.currentUser.id).subscribe(result => {
          this.positions = result;
        },
          error => console.error(error));

      },
        error => console.error(error));
    } else {
      alert('Введите новое значение заработной платы.');
    }
  }

  addPosition() {
    if (this.posName != undefined && this.posName != "" && this.posSalary > 0
      && !this.positions.find((p) => p.name === this.posName)) {
      this.http.post<[]>(this.baseUrl + 'api/Admin/AddPosition' + '?Name=' + this.posName + "&Salary=" + this.posSalary, {}).subscribe(result => {

        this.http.get<AdminPositionView[]>(this.baseUrl + 'api/Admin/Position' + '?AdminID=' + this.currentUser.id).subscribe(result => {
          this.positions = result;
        },
          error => console.error(error));

        this.posName = undefined;
        this.posSalary = undefined;

      },
        error => console.error(error));
    } else {
      alert('Данные пусты или дублируют имеющиеся в базе.');
    }
  }

  changePass(id, pass) {
    if (pass.length > 0) {
      this.http.post<[]>(this.baseUrl + 'api/Admin/ChangePass' + '?ID=' + id + "&Pass=" + pass, {}).subscribe(result => {

        this.http.get<AdminAuthView[]>(this.baseUrl + 'api/Admin/Auth' + '?AdminID=' + this.currentUser.id).subscribe(result => {
          this.auths = result;
        },
          error => console.error(error));

      },
        error => console.error(error));
    } else {
      alert('Некорректный пароль.');
    }
  }

  @ViewChild('selectedPos', { static: false })
  private selectedPos;
  registerDoctor() {
    //Находим ID будущего врача
    let maxID = 0;
    this.doctors.forEach((v) => { if (v.doctorid > maxID) maxID = v.doctorid });
    maxID++;

    if (maxID > 99 && maxID < 1000 && !(this.docName == undefined) && this.docPhone > 0
      && !(this.docAddress == undefined) && !(this.docBD == undefined)
      && !(this.docWorkStart == undefined) && !(this.docWorkEnd == undefined) && this.selectedPos.nativeElement.value > 0) {

      this.http.post<[]>(this.baseUrl + 'api/Admin/RegisterDoctor' + '?Name=' + this.docName + "&Phone=" + this.docPhone
        + "&Position=" + this.selectedPos.nativeElement.value + "&ID=" + maxID + "&Address=" + this.docAddress
        + "&Birthday=" + this.docBD + "&Start=" + this.docWorkStart + "&End=" + this.docWorkEnd, {}).subscribe(result => {

          alert('Доктор зарегистрирован. Не забудьте сменить ему пароль.');

          this.http.get<AdminDoctorView[]>(this.baseUrl + 'api/Admin/Doctor' + '?AdminID=' + this.currentUser.id).subscribe(result => {
            this.doctors = result;
          },
            error => console.error(error));


          this.http.get<AdminAuthView[]>(this.baseUrl + 'api/Admin/Auth' + '?AdminID=' + this.currentUser.id).subscribe(result => {
            this.auths = result;
          },
            error => console.error(error));

        },
          error => console.error(error));
    }
    else {
      alert('Некорректные данные. Доктор не зарегистрирован.');
    }
  }

  deleteDoctor(id) {
    if (id >= 100 && id <= 999) {
      let tt = "";
      this.rooms.filter((r) => r.fixeddoctor == id).forEach(el => { tt += el.number + " "; this.delRoom(el.number); });

      this.http.delete<[]>(this.baseUrl + 'api/Admin/DeleteDoctor' + "?ID=" + id, {}).subscribe(result => {

        alert('Доктор удален. Отменены его приемы, выписаны пациенты и удалены палаты номер ' + tt);

        this.http.get<AdminDoctorView[]>(this.baseUrl + 'api/Admin/Doctor' + '?AdminID=' + this.currentUser.id).subscribe(result => {
          this.doctors = result;
        },
          error => console.error(error));
      },
        error => console.error(error));
    }
    else {
      alert('Некорректный ID');
    }
  }

  deletePatient(id) {
    if (id >= 1000) {

      this.http.delete<[]>(this.baseUrl + 'api/Admin/DeletePatient' + "?ID=" + id, {}).subscribe(result => {

        alert('Пациент выписан и удален. Медицинская карта удалена, приемы отменены.');

        this.http.get<AdminPatientView[]>(this.baseUrl + 'api/Admin/Patient' + '?AdminID=' + this.currentUser.id).subscribe(result => {
          this.patients = result;
        },
          error => console.error(error));

      },
        error => console.error(error));
    }
    else {
      alert('Некорректный ID');
    }
  }

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) {
    this.currentUser = JSON.parse(localStorage.getItem('currentUser'));

    http.get<AdminDoctorView[]>(baseUrl + 'api/Admin/Doctor' + '?AdminID=' + this.currentUser.id).subscribe(result => {
      this.doctors = result;
    },
      error => console.error(error));

    http.get<AdminPatientView[]>(baseUrl + 'api/Admin/Patient' + '?AdminID=' + this.currentUser.id).subscribe(result => {
      this.patients = result;
    },
      error => console.error(error));

    http.get<AdminCabinetView[]>(baseUrl + 'api/Admin/Cabinet' + '?AdminID=' + this.currentUser.id).subscribe(result => {
      this.cabinets = result;
    },
      error => console.error(error));

    http.get<AdminRoomView[]>(baseUrl + 'api/Admin/Room' + '?AdminID=' + this.currentUser.id).subscribe(result => {
      this.rooms = result;
    },
      error => console.error(error));

    http.get<AdminDiseaseView[]>(baseUrl + 'api/Admin/Disease' + '?AdminID=' + this.currentUser.id).subscribe(result => {
      this.diseases = result;
    },
      error => console.error(error));

    http.get<AdminPositionView[]>(baseUrl + 'api/Admin/Position' + '?AdminID=' + this.currentUser.id).subscribe(result => {
      this.positions = result;
    },
      error => console.error(error));

    http.get<AdminAuthView[]>(baseUrl + 'api/Admin/Auth' + '?AdminID=' + this.currentUser.id).subscribe(result => {
      this.auths = result;
    },
      error => console.error(error));

    http.get<AdminAuditView[]>(baseUrl + 'api/Admin/Audit' + '?AdminID=' + this.currentUser.id).subscribe(result => {
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
  newSalary: number | undefined;
  salary: number;
}

interface AdminAuthView {
  login: string;
  passwordhash: string;
  id: number;
  newPassword: string;
}

interface AdminAuditView {
  id: number;
  login: string;
  action: string;
  acttime: Date;
}

interface UserData {
  id: number | undefined;
  name: string | undefined;
  role: string | undefined;
}

import { Component, Inject, ViewChild } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Time } from '@angular/common';
import { concat } from 'rxjs';

@Component({
  selector: 'app-guest',
  templateUrl: './guest.component.html'
})
export class GuestComponent {
  login: string | undefined;
  password: string | undefined;
  id: number | undefined;

  rPass: string | undefined;
  rName: string | undefined;
  rBd: string | undefined;
  rPasSerial: number | undefined;
  rPasNum: number | undefined;
  rPhone: number | undefined;
  rLogin: string;

  getAuth() {
    this.http.post<number>(this.baseUrl + 'api/Guest/GetAuth' + '?Login=' + this.login + '&Password=' + this.password, {}).subscribe(result => {
      this.id = result;
    },
      error => console.error(error));
  }

  getReg() {
    this.http.post<stringresult>(this.baseUrl + 'api/Guest/GetReg' + '?Password=' + this.rPass + '&Name=' + this.rName + '&Bd=' + this.rBd
      + '&PasSerial=' + this.rPasSerial + '&PasNum=' + this.rPasNum + '&Phone=' + this.rPhone, {}).subscribe(result => {
        this.rLogin = result.login;
        if (this.rLogin == "error") {
          alert("Ошибка регистрации.");
        }
        else {
          alert("Регистрация успешно произведена. Ваш логин: " + this.rLogin);
        }
      },
        error => console.error(error)
      );
  }

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) {

  }
}


export interface stringresult {
  login: string;
}

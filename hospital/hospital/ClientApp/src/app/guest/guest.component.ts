import { Component, Inject, ViewChild } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Time } from '@angular/common';
import { concat } from 'rxjs';

@Component({
  selector: 'app-guest',
  templateUrl: './guest.component.html'
})
export class GuestComponent {
  auths: UserData[];
  //Поля входа
  login: string | undefined;
  password: string | undefined;
  //Поля регистрации
  rPass: string | undefined;
  rName: string | undefined;
  rBd: string | undefined;
  rPasSerial: number | undefined;
  rPasNum: number | undefined;
  rPhone: number | undefined;
  rLogin: string;

  getAuth() {
    this.http.post<Array<UserData>>(this.baseUrl + 'api/Guest/GetAuth' + '?Login=' + this.login + '&Password=' + this.password, {}).subscribe(result => {
      this.auths = result;

      if (this.auths[0].id > 0) {
        localStorage.setItem('currentUser', JSON.stringify(this.auths[0]));
        window.location.reload();
      }
      else {
        alert('Ошибка авторизации. Неверный логин или пароль.')
      }

    },
      error => { console.error(error) });
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
interface UserData {
  id: number | undefined;
  name: string | undefined;
  role: string | undefined;
}

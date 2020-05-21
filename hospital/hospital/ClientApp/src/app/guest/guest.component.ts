import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-guest',
  templateUrl: './guest.component.html'
})
export class GuestComponent {
  public guests: GuestView[];

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    http.get<GuestView[]>(baseUrl + 'api/MyGuest/GuestViews').subscribe(result => {
      this.guests = result;
    },
      error => console.error(error));
  }
}

interface GuestView {
  col1: string;
  col2: number;
}

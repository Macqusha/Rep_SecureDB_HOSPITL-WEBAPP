import { Component, Inject } from '@angular/core';
import { HttpClient } from 'selenium-webdriver/http';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent {
  currentUser: user;

  constructor() {
    this.currentUser = JSON.parse(localStorage.getItem('currentUser'));
  }
}

interface user {
  id: number | undefined;
  name: string | undefined;
  role: string | undefined;
}

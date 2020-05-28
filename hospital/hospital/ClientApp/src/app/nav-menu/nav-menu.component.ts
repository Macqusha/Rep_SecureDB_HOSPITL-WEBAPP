import { Component } from '@angular/core';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent {
  currentUser: user;
  isExpanded = false;

  constructor() {
    this.currentUser = JSON.parse(localStorage.getItem('currentUser'));
  }

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }

  logout() {
    localStorage.removeItem('currentUser');
    window.location.reload();
  }
}

interface user {
  id: number | undefined;
  name: string | undefined;
  role: string | undefined;
}

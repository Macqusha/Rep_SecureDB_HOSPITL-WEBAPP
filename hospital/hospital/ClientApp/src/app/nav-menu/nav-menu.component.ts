import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent {
  currentUser: user;
  isExpanded = false;

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) {
    this.currentUser = JSON.parse(localStorage.getItem('currentUser'));
  }

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }

  logout() {
    var tmpUser = JSON.parse(localStorage.getItem('currentUser'));
    localStorage.removeItem('currentUser');
    window.location.reload();
    this.http.post<null>(this.baseUrl + 'api/Guest/LogoutAudit' + '?ID=' + tmpUser.id, {}).subscribe(result => {
    },
      error => { console.error(error) });
  }

}

interface user {
  id: number | undefined;
  name: string | undefined;
  role: string | undefined;
}

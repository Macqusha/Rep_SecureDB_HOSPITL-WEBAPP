import { Component, Inject, OnInit, ViewChild, ElementRef } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Time } from '@angular/common';
import { DynamicDialogConfig } from 'primeng/dynamicdialog';


@Component({
  selector: 'app-makeAppointment',
  templateUrl: './makeAppointment.component.html'
})
export class MakeAppointmentComponent implements OnInit {
  public dates: FreeDatesView[];
  public curDoctor: number = this.config.data.DoctorID;
  currentUser: UserData;

  @ViewChild('selectedparams', { static: false })
  private selectedparams;
  addAppointment() {
    console.error(this.config.data.doctorID);
    this.http.post(this.baseUrl + 'api/Patient/AddAppointment' + '?DoctorID=' + this.config.data.doctorID + "&PatientID=" + this.currentUser.id
      + '&' + this.selectedparams.nativeElement.value, {}).subscribe(result => {

        this.http.get<FreeDatesView[]>(this.baseUrl + 'api/Patient/GetFreeDates' + '?DoctorID=' + this.currentUser.id + '&Start=' + this.config.data.workStart +
          '&End=' + this.config.data.workEnd).subscribe(result => {
            this.dates = result;
          },
            error => console.error(error));

        alert('Вы успешно записаны на прием.');

      },
        error => console.error(error));
  }

  constructor(
    private http: HttpClient,
    @Inject('BASE_URL') private baseUrl: string,
    private config: DynamicDialogConfig,
  ) {
    this.currentUser = JSON.parse(localStorage.getItem('currentUser'));
  }
  ngOnInit(): void {
    this.currentUser = JSON.parse(localStorage.getItem('currentUser'));

    this.http.get<FreeDatesView[]>(this.baseUrl + 'api/Patient/GetFreeDates' + '?DoctorID=' + this.currentUser.id + '&Start=' + this.config.data.workStart +
      '&End=' + this.config.data.workEnd).subscribe(result => {
        this.dates = result;
      },
        error => console.error(error));
  }
}

interface FreeDatesView {
  date: string;
  cabinet: number;
}

interface UserData {
  id: number | undefined;
  name: string | undefined;
  role: string | undefined;
}

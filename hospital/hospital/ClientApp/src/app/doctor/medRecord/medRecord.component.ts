import { Component, Inject, OnInit, ViewChild, ElementRef } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Time } from '@angular/common';
import { DynamicDialogConfig } from 'primeng/dynamicdialog';


@Component({
  selector: 'app-medRecord',
  templateUrl: './medRecord.component.html'
})
export class MedRecordComponent implements OnInit {
  public records: MedicalRecordView[] = [];
  public diseases: MedRecordDiseaseView[];
  public rooms: MedRecordFreeRoomsView[];
  public curRoom: number | undefined = this.config.data.room;
  currentUser: UserData;

  @ViewChild('selectedcode', { static: false })
  private selectedcode;
  addDiagnosis() {
    this.http.post<MedRecordDiseaseView[]>(this.baseUrl + 'api/Doctor/AddDisease' + '?PatientID=' + this.config.data.patientID
      + '&Code=' + this.selectedcode.nativeElement.value, {}).subscribe(result => {
        this.http.get<MedicalRecordView[]>(
          this.baseUrl + 'api/Patient/Diagnosis' + '?PatientID=' + this.config.data.patientID).subscribe(result => {
            this.records = result;
          },
            error => console.error(error));
      },
        error => console.error(error));
  }

  @ViewChild('selectedroom', { static: false })
  private selectedroom;
  addRoom() {
    this.http.post<MedRecordDiseaseView[]>(this.baseUrl + 'api/Doctor/AddRoom' + '?PatientID=' + this.config.data.patientID
      + '&Room=' + this.selectedroom.nativeElement.value, {}).subscribe(result => {
        this.curRoom = this.selectedroom.nativeElement.value;
      },
        error => console.error(error));
  }
  removeRoom() {
    this.http.post<MedRecordDiseaseView[]>(this.baseUrl + 'api/Doctor/RemoveRoom' + '?PatientID=' + this.config.data.patientID, {}).subscribe(result => {
      this.curRoom = undefined;

      this.http.get<MedRecordFreeRoomsView[]>(this.baseUrl + 'api/Doctor/GetFreeRooms' + '?DoctorID=' + this.currentUser.id).subscribe(result => {
        this.rooms = result;
      },
        error => console.error(error));

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

    this.http.get<MedicalRecordView[]>(
      this.baseUrl + 'api/Patient/Diagnosis' + '?PatientID=' + this.config.data.patientID).subscribe(result => {
        this.records = result;
      },
        error => console.error(error));

    this.http.get<MedRecordDiseaseView[]>(this.baseUrl + 'api/Admin/Disease' + '?AdminID=' + this.currentUser.id).subscribe(result => {
      this.diseases = result;
    },
      error => console.error(error));

    this.http.get<MedRecordFreeRoomsView[]>(this.baseUrl + 'api/Doctor/GetFreeRooms' + '?DoctorID=' + this.currentUser.id).subscribe(result => {
      this.rooms = result;
    },
      error => console.error(error));
  }
}


interface MedicalRecordView {
  code: string;
  name: string;
  treatment: string;
}

interface MedRecordDiseaseView {
  code: string;
  name: string;
  treatment: string;
}

interface MedRecordFreeRoomsView {
  number: number;
  places: number;
  free: number;
}

interface UserData {
  id: number | undefined;
  name: string | undefined;
  role: string | undefined;
}

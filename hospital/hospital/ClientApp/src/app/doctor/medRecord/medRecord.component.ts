import { Component, Inject, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Time } from '@angular/common';
import { DynamicDialogConfig } from 'primeng/dynamicdialog';


@Component({
  selector: 'app-medRecord',
  templateUrl: './medRecord.component.html'
})
export class MedRecordComponent implements OnInit {
  public records: MedicalRecordView[] = [];


  func() {
    ;
  }

  constructor(
    private http: HttpClient, 
    @Inject('BASE_URL') private baseUrl: string, 
    private config: DynamicDialogConfig,
    ) {
    
  }
  ngOnInit(): void {
    this.http.get<MedicalRecordView[]>(
      this.baseUrl + 'api/Patient/Diagnosis'+'?PatientID=' + this.config.data.patientID).subscribe(result => {
      this.records = result;
    },
      error => console.error(error));
  }
}


interface MedicalRecordView {
  code: string;
  name: string;
  treatment: string;
}

import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { PatientComponent } from './patient/patient.component';
import { DoctorComponent } from './doctor/doctor.component';
import { MedRecordComponent } from './doctor/medRecord/medRecord.component';
import { AdminComponent } from './admin/admin.component';
import { DialogModule } from 'primeng/dialog';
import { DynamicDialogModule } from 'primeng/dynamicdialog'
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';


@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    PatientComponent,
    DoctorComponent,
    AdminComponent,
    MedRecordComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    DialogModule,
    BrowserAnimationsModule,
    DynamicDialogModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'patient', component: PatientComponent },
      { path: 'doctor', component: DoctorComponent },
      { path: 'admin', component: AdminComponent },
    ])
  ],
  providers: [],
  bootstrap: [AppComponent],
  entryComponents: [
    MedRecordComponent
  ],
})
export class AppModule { }

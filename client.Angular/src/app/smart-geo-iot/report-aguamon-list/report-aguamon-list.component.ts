import { DeviceRegistration } from './../device';
import { SmartGeoIotService } from './../smartgeoiot.service';
import { Report } from '../report';
import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Observable } from 'rxjs';
import { GrudList } from '../../common/grud-list';
import { MessageService } from '../../common/message.service';
import { MatDialog, MatBottomSheet } from '@angular/material';
import { Utils } from '../utils';
import { ProjectEnum } from '../project';

@Component({
  selector: 'app-report-aguamon-list',
  templateUrl: './report-aguamon-list.component.html',
  styleUrls: ['./report-aguamon-list.component.css']
})
export class ReportAguamonListComponent extends GrudList<Report>  implements OnInit {
  form: FormGroup;
  // displayedColumns: string[] = ['data', 'nivel', 'luz', 'temperatura', 'umidade', 'oxigenio', 'ph', 'condutividade'];
  // displayedColumns: string[] = ['data', 'temperatura', 'ph', 'fluor', 'cloro', 'turbidez', 'rele1', 'rele2', 'rele3', 'rele4', 'rele5'];
  displayedColumns: string[] = [];
  isExporting = false;
  listDevicesFilter: DeviceRegistration[] = [];
  currentProject = ProjectEnum.AM_1leg;

  constructor(
    private sgiService: SmartGeoIotService,
    public messageService: MessageService,
    public dialog: MatDialog) {
    super(false);

    this._pageSize = 25;
    this.form = new FormGroup({
      'startPeriod': new FormControl(null),
      'endPeriod': new FormControl(null),
      'deviceFilter': new FormControl('', Validators.required)
    });

    this.form.get('deviceFilter').valueChanges.subscribe(val => {
      if (this.currentProject === ProjectEnum.AM_1leg) {
        this.displayedColumns = ['data', 'hour', 'temperatura', 'ph', 'fluor', 'cloro', 'turbidez', 'rele1', 'rele2', 'rele3', 'rele4', 'rele5'];
      } else {
        this.displayedColumns = ['data', 'hour', 'nivel', 'luz', 'temperatura', 'umidade', 'oxigenio', 'ph', 'condutividade'];
      }
    });
  }
  ngOnInit() { }

  get deviceFilter() {
    return this.form.get('deviceFilter').value;
  }

  cleanFilterDate() {
    this.form.get('startPeriod').setValue(null);
    this.form.get('endPeriod').setValue(null);
  }

  printReport() {
    window.print();
  }

  getResults(): Observable<Report[]> {
    this.messageService.blockUI();
    let startPeriod: Date = null;
    let endPeriod: Date = null;

    if (this.form.get('startPeriod').value != null) {
      startPeriod = this.form.get('startPeriod').value;
    }

    if (this.form.get('endPeriod').value != null) {
      endPeriod = this.form.get('endPeriod').value;
    }
    return this.sgiService.getReports(this.deviceFilter, startPeriod != null ? startPeriod.toJSON() : <string>null, endPeriod != null ? endPeriod.toJSON() : <string>null, this._skip, (this._skip + this._pageSize), false, 0, c => { this._totalCount = c; });
  }

  searchReports(): void {
    if (!this.form.valid) {
      return;
    }
    this.newSearch();
  }

  exportToExcel(): void {
    if (!this.form.valid) {
      return;
    }

    this.messageService.blockUI();
    let startPeriod: Date = null;
    let endPeriod: Date = null;

    if (this.form.get('startPeriod').value != null) {
      startPeriod = this.form.get('startPeriod').value;
    }

    if (this.form.get('endPeriod').value != null) {
      endPeriod = this.form.get('endPeriod').value;
    }

    this.isExporting = true;
    this.sgiService.exportReportsToExcel(this.deviceFilter, startPeriod != null ? startPeriod.toJSON() : <string>null, endPeriod != null ? endPeriod.toJSON() : <string>null, 2000).subscribe(data => {
      Utils.fileDownload(data, 'relatório.xlsx');
      this.isExporting = false;
    });
  }

  get initialDateFilter() {
    return this.form.get('startPeriod').value;
  }

  get finalDateFilter() {
    return this.form.get('endPeriod').value;
  }

}

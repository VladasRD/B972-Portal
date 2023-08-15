import { DeviceRegistration } from './../../Device';
import { SmartGeoIotService } from './../../smartgeoiot.service';
import { Report } from '../../report';
import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Observable } from 'rxjs';
import { GrudList } from '../../../common/grud-list';
import { MessageService } from '../../../common/message.service';
import { MatDialog, MatBottomSheet } from '@angular/material';
import { Utils } from '../../utils';
import { ProjectEnum } from '../../project';

@Component({
  selector: 'app-report-b980-list',
  templateUrl: './report-b980-list.component.html',
  styleUrls: ['./report-b980-list.component.css']
})
export class ReportB980ListComponent extends GrudList<Report> implements OnInit {
  form: FormGroup;
  displayedColumns: string[] = ['data', 'hour', 'ed1', 'ed2', 'ed3', 'ed4', 'sd1', 'sd2', 'ea10', 'sa3'];
  isExporting = false;
  listDevicesFilter: DeviceRegistration[] = [];
  currentProject = ProjectEnum.B980;

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

  getTitleName(fieldName: string): string {
    if (this.results === null || this.results.length === 0){
      return fieldName;
    }

    return this.results.map(t => t[fieldName.toLowerCase()])[0].toString();
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
    return this.sgiService.getReports(this.deviceFilter, startPeriod != null ? startPeriod.toJSON() : <string>null, endPeriod != null ? endPeriod.toJSON() : <string>null, this._skip, (this._skip + this._pageSize), false, null, c => { this._totalCount = c; });
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

    const _fileName = `relatório-dispositivo-${this.deviceFilter}.xlsx`;
    this.sgiService.exportReportsToExcel(this.deviceFilter, startPeriod != null ? startPeriod.toJSON() : <string>null, endPeriod != null ? endPeriod.toJSON() : <string>null, 2000, null, null).subscribe(data => {
      Utils.fileDownload(data, _fileName);
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

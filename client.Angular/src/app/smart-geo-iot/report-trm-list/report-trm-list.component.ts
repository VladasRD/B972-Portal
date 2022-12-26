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
  selector: 'app-report-trm-list',
  templateUrl: './report-trm-list.component.html',
  styleUrls: ['./report-trm-list.component.css']
})
export class ReportTrmListComponent extends GrudList<Report> implements OnInit {
  form: FormGroup;
  displayedColumns: string[] = ['data', 'hour', 'consumo', 'consumoDia', 'consumoSemana', 'consumoMes', 'media', 'modo', 'estado', 'valvula'];
  isExporting = false;
  listDevicesFilter: DeviceRegistration[] = [];
  currentProject = ProjectEnum.B972_P;
  typeReportChecked = 1;

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

  buildColumns() {
    if (this.typeReportChecked === 1) {
      // hora
      this.displayedColumns = ['data', 'hour', 'media'];
    }
    if (this.typeReportChecked === 2) {
      // dia
      this.displayedColumns = ['data', 'consumoDia'];
    }
    if (this.typeReportChecked === 3) {
      // semana
      this.displayedColumns = ['data', 'consumoSemana'];
    }
    if (this.typeReportChecked === 4) {
      // mês
      this.displayedColumns = ['data', 'consumoMes'];
    }
  }

  changeTypeReport(event) {
    this.typeReportChecked = event;

    this.results = [];
    this._totalCount = 0;
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
    return this.sgiService.getReports(this.deviceFilter, startPeriod != null ? startPeriod.toJSON() : <string>null, endPeriod != null ? endPeriod.toJSON() : <string>null, this._skip, (this._skip + this._pageSize), false, this.typeReportChecked, c => { this._totalCount = c; });
  }

  searchReports(): void {
    if (!this.form.valid) {
      return;
    }
    this.newSearch();
    this.buildColumns();
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
    this.sgiService.exportReportsToExcel(this.deviceFilter, startPeriod != null ? startPeriod.toJSON() : <string>null, endPeriod != null ? endPeriod.toJSON() : <string>null, 2000, null, this.typeReportChecked).subscribe(data => {
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

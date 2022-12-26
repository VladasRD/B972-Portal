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
import { MCond } from '../MCond';
import { ValueTransformer } from '@angular/compiler/src/util';

@Component({
  selector: 'app-report-mcond-list',
  templateUrl: './report-mcond-list.component.html',
  styleUrls: ['./report-mcond-list.component.css']
})
export class ReportMcondListComponent extends GrudList<MCond> implements OnInit {
  form: FormGroup;
  // displayedColumns: string[] = ['date', 'hour'];
  displayedColumns: string[] = [];
  isExporting = false;
  listDevicesFilter: DeviceRegistration[] = [];
  currentProject = ProjectEnum.B987;
  // typeReportChecked = 1;

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

  buildColumns(data: MCond) {
    if (data == null) {
      return;
    }
    this.displayedColumns = [];
    this.displayedColumns.push('date');
    this.displayedColumns.push('hour');

    if (data.packSup !== null) {
      // verifica pacote portaria
      this.displayedColumns.push('supLevel');
      this.displayedColumns.push('supAlertMax');
      this.displayedColumns.push('supAlertMin');
    }
    if (data.packInf !== null) {
      // verifica pacote portaria
      this.displayedColumns.push('infLevel');
      this.displayedColumns.push('infAlarmLevelMax');
      this.displayedColumns.push('infAlarmLevelMin');
    }
    if (data.packPort !== null) {
      // verifica pacote superior
      this.displayedColumns.push('portFireAlarm');
      this.displayedColumns.push('portIvaAlarm');
    }
  }

  getResults(): Observable<MCond[]> {
    this.messageService.blockUI();
    let startPeriod: Date = null;
    let endPeriod: Date = null;

    if (this.form.get('startPeriod').value != null) {
      startPeriod = this.form.get('startPeriod').value;
    }

    if (this.form.get('endPeriod').value != null) {
      endPeriod = this.form.get('endPeriod').value;
    }
    var _results = this.sgiService.getReportMCond(this.deviceFilter, startPeriod != null ? startPeriod.toJSON() : <string>null, endPeriod != null ? endPeriod.toJSON() : <string>null, this._skip, (this._skip + this._pageSize), c => { this._totalCount = c; })
    ;

    _results.subscribe(data => {
      console.log('data: ');
      console.log(data);
      if (data.length > 0) {
        this.buildColumns(data[0]);
      }
    });


    return _results;
  }

  searchReports(): void {
    if (!this.form.valid) {
      return;
    }
    this.newSearch();
    // this.buildColumns();
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
    this.sgiService.exportReportMCondToExcel(this.deviceFilter, startPeriod != null ? startPeriod.toJSON() : <string>null, endPeriod != null ? endPeriod.toJSON() : <string>null, 5000).subscribe(data => {
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

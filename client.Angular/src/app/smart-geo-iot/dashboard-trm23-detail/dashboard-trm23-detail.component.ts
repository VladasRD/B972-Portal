import { Dashboard } from './../dashboard';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material';
import { SmartGeoIotService } from './../smartgeoiot.service';
import { MessageService } from '../../common/message.service';
import { String } from 'typescript-string-operations';
import { Bits } from '../Bits';
import { GenericYesNoDialogComponent } from '../../common/generic-yes-no-dialog/generic-yes-no-dialog.component';
import { Client } from '../client';

@Component({
  selector: 'app-dashboard-trm23-detail',
  templateUrl: './dashboard-trm23-detail.component.html',
  styleUrls: ['./dashboard-trm23-detail.component.css']
})
export class DashboardTrm23DetailComponent implements OnInit {
  private _deviceId: string;
  dashboard: Dashboard;
  clientByDevice: Client;
  form: FormGroup;
  changeDevice = false;
  initialSetTimeout = 0;
  showDashboard = true;
  seqNumber = 0;
  navigation = null;

  constructor(
    private route: ActivatedRoute,
    private sgiService: SmartGeoIotService,
    private router: Router,
    private messageService: MessageService,
    public dialog: MatDialog
  ) {
    this.dashboard = new Dashboard();
    this.clientByDevice = new Client();
    this.dashboard.bits = new Bits();
  }

  ngOnInit() {
    this.form = new FormGroup({
      'dateFilter': new FormControl(null)
    });

    this.form.get('dateFilter').valueChanges.subscribe(val => {
      this.finishSetTimeout();
      this.seqNumber = 0;
      this.updateData();
    });

    this.updateData();
    this.initializeSetTimeout();
    this.getClientByDevice();
  }

  ngOnDestroy() {
    this.finishSetTimeout();
  }

  finishSetTimeout() {
    this.initialSetTimeout = 1;
  }

  startSetTimeout() {
    if (this.initialSetTimeout === 1) {
      this.initialSetTimeout = 0;
      this.initializeSetTimeout();
    }
  }

  back() {
    this.seqNumber = (this.seqNumber - 1);
    this.navigation = 'back';
    this.finishSetTimeout();
    this.getDashboard();
  }

  next() {
    this.seqNumber = (this.seqNumber + 1);
    this.navigation = 'next';
    this.finishSetTimeout();
    this.getDashboard();
  }

  initializeSetTimeout() {
    (async () => {
      while (this.initialSetTimeout === 0) {
        await new Promise(resolve => setTimeout(resolve, 10000));
        this.updateData();
      }
    })();
  }

  updateData() {
    this.getDashboard();
  }

  cleanUpdateData() {
    this.seqNumber = 0;
    this.navigation = null;
    this.form.get('dateFilter').setValue(null);
    this.startSetTimeout();
    this.getDashboard();
  }

  changeStatus() {
    this.changeDevice = !this.changeDevice;
  }

  get dateFilter() {
    return this.form.get('dateFilter').value;
  }

  get pageTitle(): string {
    return String.Format('Dados do dispositivo {0} (TRM)', this.dashboard.name);
  }

  get getTemperature(): number {
    return Number(this.dashboard.temperature);
  }

  private getDashboard(): void {

    let _dateFilter: Date = null;
    if (this.dateFilter !== null && this.dateFilter !== undefined) {
      _dateFilter = this.dateFilter;
    }

    this._deviceId = this.route.snapshot.paramMap.get('id');
    this.sgiService.getDashboard(this._deviceId, _dateFilter != null ? _dateFilter.toJSON() : <string>null, this.seqNumber, this.navigation).subscribe(d => {
      this.dashboard = Object.assign(new Dashboard(), d);

      if (d === null) {
        this.finishSetTimeout();
        this.showDashboard = false;
      } else {
        // this.startSetTimeout();
        this.showDashboard = true;
      }

      if (this.seqNumber === 0) {
        this.seqNumber = this.dashboard.seqNumber;
      }

      this.form.patchValue(this.dashboard);
      this.form.updateValueAndValidity();
    });
  }

  private getClientByDevice(): void {
    this.sgiService.getClientByDevice(this._deviceId).subscribe(d => {
      this.clientByDevice = Object.assign(new Client(), d);
    });
  }

}

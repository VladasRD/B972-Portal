import { Dashboard } from './../dashboard';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material';
import { SmartGeoIotService } from './../smartgeoiot.service';
import { MessageService } from '../../common/message.service';
import { FormUtil } from '../../common/form-util';
import { String } from 'typescript-string-operations';
import { DocumentType } from '../documentType';
import { Bits } from '../Bits';
import { ProjectEnum } from '../project';

@Component({
  selector: 'app-dashboard-detail',
  templateUrl: './dashboard-detail.component.html',
  styleUrls: ['./dashboard-detail.component.css']
})
export class DashboardDetailComponent implements OnInit {
  private _deviceId: string;
  dashboard: Dashboard;
  form: FormGroup;

  constructor(
    private route: ActivatedRoute,
    private sgiService: SmartGeoIotService,
    private router: Router,
    private messageService: MessageService,
    public dialog: MatDialog
  ) {
    this.dashboard = new Dashboard();
    this.dashboard.bits = new Bits();
  }

  ngOnInit() {
    this.form = new FormGroup({
    });

    this.getDashboard();
  }

  updateData() {
    this.getDashboard();
  }

  get pageTitle(): string {
    return String.Format('Dados do dispositivo {0} (Smart Geo IoT)', this.dashboard.name);
  }

  get getTemperature(): number {
    return Number(this.dashboard.temperature);
  }

  private getDashboard(): void {
    this._deviceId = this.route.snapshot.paramMap.get('id');
    this.sgiService.getDashboard(this._deviceId, null, 0, null, 0, ProjectEnum.Hidroleg).subscribe(d => {
      this.dashboard = Object.assign(new Dashboard(), d);
      this.form.patchValue(this.dashboard);
      this.form.updateValueAndValidity();
    });
  }

}

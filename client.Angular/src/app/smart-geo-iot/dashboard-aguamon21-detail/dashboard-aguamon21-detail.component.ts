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

@Component({
  selector: 'app-dashboard-aguamon21-detail',
  templateUrl: './dashboard-aguamon21-detail.component.html',
  styleUrls: ['./dashboard-aguamon21-detail.component.css']
})
export class DashboardAguamon21DetailComponent implements OnInit {
  private _deviceId: string;
  dashboard: Dashboard;
  form: FormGroup;
  changeDevice = false;
  initialSetTimeout = 0;

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
    this.initializeSetTimeout();
  }

  ngOnDestroy() {
    this.initialSetTimeout = 1;
  }

  initializeSetTimeout() {
    (async () => {
      while (this.initialSetTimeout === 0) {
        await new Promise(resolve => setTimeout(resolve, 10000));
        this.getDashboard();
      }
    })();
  }

  updateData() {
    this.getDashboard();
  }

  changeStatus() {
    this.changeDevice = !this.changeDevice;
  }

  get pageTitle(): string {
    return String.Format('Dados do dispositivo {0} (AguaMon-2)', this.dashboard.name);
  }

  get getTemperature(): number {
    return Number(this.dashboard.temperature);
  }

  private getDashboard(): void {
    this._deviceId = this.route.snapshot.paramMap.get('id');
    this.sgiService.getDashboard(this._deviceId).subscribe(d => {
      this.dashboard = Object.assign(new Dashboard(), d);

      this.form.patchValue(this.dashboard);
      this.form.updateValueAndValidity();
    });
  }

}

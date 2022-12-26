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
import { Client, ClientDevice } from '../client';
import { environment } from '../../../environments/environment';
import { ProjectEnum } from '../project';
import { AuthService } from '../../common/auth.service';

@Component({
  selector: 'app-dashboard-clampon-detail',
  templateUrl: './dashboard-clampon-detail.component.html',
  styleUrls: ['./dashboard-clampon-detail.component.css']
})
export class DashboardClamponDetailComponent implements OnInit {
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
    public dialog: MatDialog,
    private authService: AuthService
  ) {
    this.dashboard = new Dashboard();
    this.clientByDevice = new Client();
    this.dashboard.bits = new Bits();
  }

  ngOnInit() {
    this.form = new FormGroup({
      'tipoEnvio': new FormControl(''),
      'numeroEnvios': new FormControl(''),
      'tempoTransmissao': new FormControl(''),
      'dateFilter': new FormControl(null),
      'serialNumber': new FormControl({value: '', disabled: true}),
      'model': new FormControl({value: '', disabled: true})
    });

    this.form.get('dateFilter').valueChanges.subscribe(val => {
      this.finishSetTimeout();
      this.seqNumber = 0;
      this.updateData();
    });

    this.getDashboard();
    this.initializeSetTimeout();
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
    this.seqNumber = (this.seqNumber);
    this.navigation = 'back';
    this.finishSetTimeout();
    this.getDashboard();
  }

  next() {
    this.seqNumber = (this.seqNumber);
    this.navigation = 'next';
    this.finishSetTimeout();
    this.getDashboard();
  }

  initializeSetTimeout() {
    (async () => {
      while (this.initialSetTimeout === 0) {
        await new Promise(resolve => setTimeout(resolve, environment.TIMEOUT_REQUEST_DASHBOARD));
        
        this.seqNumber = (this.seqNumber);
        this.navigation = 'next';
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

  get pageTitle(): string {
    return String.Format('Dados do dispositivo {0}', this.dashboard.name);
  }

  get getTemperature(): number {
    return Number(this.dashboard.temperature);
  }

  get userName(): string {
    let userName = this.authService.signedUser.userClaims.find(c => c.claimType === 'given_name');
    if (userName) {
      return userName.claimValue;
    }
    return '';
  }

  sendChanges() {
    if (this.form.get('numeroEnvios').value > 140) {
      this.form.get('numeroEnvios').setValue(140);
    }

    this.sgiService.sendChangesDevice(this._deviceId, this.numeroEnvios, this.tempoTransmissao, this.tipoEnvio, 0)
      .subscribe(() => {
        this.router.navigate(['./radiodados/dashboard']);
        this.messageService.add('Alteração no dispositivo enviada.');
      },
        err => {
          this.messageService.addError(err.message + ' (alterando informações do dispositivo)');
        });
  }

  openConfirmDeleteDialog(): void {
    const dialogRef = this.dialog.open(GenericYesNoDialogComponent, {
      width: '80%',
      data: { title: 'Alterar informações do dispositivo', message: 'Tem certeza que deseja enviar as alterações do dispositivo?', isWarn: true }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result === true) {
        this.sendChanges();
      }
    });
  }

  private get numeroEnvios() {
    return this.form.get('numeroEnvios').value;
  }

  private get tempoTransmissao() {
    return this.form.get('tempoTransmissao').value;
  }

  get dateFilter() {
    return this.form.get('dateFilter').value;
  }

  private get tipoEnvio() {
    return this.form.get('tipoEnvio').value;
  }

  get deviceId() {
    return this._deviceId;
  }

  private getDashboard(): void {

    let _dateFilter: Date = null;
    if (this.dateFilter !== null && this.dateFilter !== undefined) {
      _dateFilter = this.dateFilter;
    }

    this._deviceId = this.route.snapshot.paramMap.get('id');
    this.sgiService.getDashboard(this._deviceId, _dateFilter != null ? _dateFilter.toJSON() : <string>null, this.seqNumber, this.navigation, 0, ProjectEnum.B972).subscribe(d => {
      this.dashboard = Object.assign(new Dashboard(), d);

      if (d === null) {
        this.finishSetTimeout();
        this.showDashboard = false;
      } else {
        this.showDashboard = true;
      }

      this.form.get('serialNumber').setValue(this.dashboard.serialNumber);
      this.form.get('model').setValue(this.dashboard.model);

      this.seqNumber = this.dashboard.time;
      this.form.patchValue(this.dashboard);
      this.form.updateValueAndValidity();
    });
  }

  cleanPartial() {
    let totalPartial = Number(this.dashboard.total.replace(',', '.'));
    // if (totalPartial === 0) {
    //   totalPartial = Number(this.dashboard.total.replace(',', '.'));
    // }
    this.sgiService.cleanPartialbyDevice(this._deviceId, totalPartial)
      .subscribe(() => {
        // this.router.navigate(['./radiodados/dashboard']);
        this.messageService.add('Contador zerado.');
        this.cleanUpdateData();
      },
        err => {
          this.messageService.addError(err.message + ' (zerando contador)');
        });
  }

  get getNameLogoClient(): string {
    // if (this.clientByDevice === null || this.clientByDevice === undefined) {
    //   return 'logo';
    // }
    // if (this.clientByDevice.name === null || this.clientByDevice.name === undefined) {
    //   return 'logo';
    // }
    // if (this.clientByDevice.name.toLowerCase().includes('nivetec')) {
    //   return 'logo-nivetec';
    // }
    // return 'logo';
    return 'logo-nivetec';
  }

}

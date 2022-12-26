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
import { ChangeDeviceDialogComponent } from '../change-device-dialog/change-device-dialog.component';
import { environment } from '../../../environments/environment';
import { ProjectEnum } from '../project';
import { AuthService } from '../../common/auth.service';
import { AppUser } from '../../common/appUser';

@Component({
  selector: 'app-dashboard-aguamon81-detail',
  templateUrl: './dashboard-aguamon81-detail.component.html',
  styleUrls: ['./dashboard-aguamon81-detail.component.css']
})
export class DashboardAguamon81DetailComponent implements OnInit, OnDestroy {
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
      'tensaoMinima': new FormControl({value: null, disabled: true}),
      'serialNumber': new FormControl({value: '', disabled: true}),
      'model': new FormControl({value: '', disabled: true})
    });

    this.form.get('dateFilter').valueChanges.subscribe(val => {
      this.finishSetTimeout();
      this.seqNumber = 0;
      this.updateData();
    });

    this.updateData();
    this.initializeSetTimeout();
  }

  ngOnDestroy() {
    this.initialSetTimeout = 1;
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

  get userName(): string {
    let userName = this.authService.signedUser.userClaims.find(c => c.claimType === 'given_name');
    if (userName) {
      return userName.claimValue;
    }
    return '';
  }

  back() {
    this.seqNumber = (this.seqNumber);
    this.dashboard.seqNumberb = (this.dashboard.seqNumberb);
    this.navigation = 'back';
    this.finishSetTimeout();
    this.getDashboard();
  }

  next() {
    this.seqNumber = (this.seqNumber);
    this.dashboard.seqNumberb = (this.dashboard.seqNumberb);
    this.navigation = 'next';
    this.finishSetTimeout();
    this.getDashboard();
  }

  initializeSetTimeout() {
    // (async () => {
    //   while (this.initialSetTimeout === 0) {
    //     await new Promise(resolve => setTimeout(resolve, environment.TIMEOUT_REQUEST_DASHBOARD));
    //     this.getDashboard();
    //   }
    // })();
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
    return String.Format('Dados do dispositivo {0} (Nivetec Analítica PQA)', this.dashboard.name);
  }

  get getTemperature(): number {
    return Number(this.dashboard.temperature);
  }

  get deviceId() {
    return this._deviceId;
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

  // sendChanges() {
  //   if (this.form.get('numeroEnvios').value > 140) {
  //     this.form.get('numeroEnvios').setValue(140);
  //   }

  //   this.sgiService.sendChangesDevice(this._deviceId, this.numeroEnvios, this.tempoTransmissao, this.tipoEnvio, 0)
  //     .subscribe(() => {
  //       this.router.navigate(['./radiodados/dashboard']);
  //       this.messageService.add('Alteração no dispositivo enviada.');
  //     },
  //       err => {
  //         this.messageService.addError(err.message + ' (alterando informações do dispositivo)');
  //       });
  // }
  sendChanges(data: any) {
    this.messageService.blockUI();
    if (data.numeroEnvios > 140) {
      data.numeroEnvios = 140;
    }
    this.sgiService.sendChangesDevice(this._deviceId, data.numeroEnvios, data.tempoTransmissao, data.tipoEnvio, data.tensaoMinima)
      .subscribe(() => {
        // this.router.navigate(['./radiodados/dashboard']);
        this.getDashboard();
        this.messageService.add('Alteração no dispositivo enviada.');
      },
        err => {
          this.messageService.addError(err.message + ' (alterando informações do dispositivo)');
        });
  }
  
  openConfirmSendChangesDialog(): void {
    const dialogRef = this.dialog.open(ChangeDeviceDialogComponent, {
      width: '80%',
      data: {
        title: 'Alterar informações do dispositivo',
        message: 'Tem certeza que deseja enviar as alterações do dispositivo?',
        isWarn: false,
        numeroEnvios: this.numeroEnvios,
        tempoTransmissao: this.tempoTransmissao,
        tipoEnvio: this.tipoEnvio,
        tensaoMinima: this.tensaoMinima,
        deviceId: this._deviceId
      }
    });

    dialogRef.afterClosed().subscribe(r => {
      if (!r) {
        return;
      }
      if (this.isAnyChange(r.data) === true) {
        this.sendChanges(r.data);
      }
    });
  }

  // openConfirmDeleteDialog(): void {
  //   const dialogRef = this.dialog.open(GenericYesNoDialogComponent, {
  //     width: '80%',
  //     data: { title: 'Alterar informações do dispositivo', message: 'Tem certeza que deseja enviar as alterações do dispositivo?', isWarn: true }
  //   });

  //   dialogRef.afterClosed().subscribe(result => {
  //     if (result === true) {
  //       this.sendChanges();
  //     }
  //   });
  // }

  private get numeroEnvios() {
    return this.form.get('numeroEnvios').value;
  }

  private get tempoTransmissao() {
    return this.form.get('tempoTransmissao').value;
  }

  private get tipoEnvio() {
    return this.form.get('tipoEnvio').value;
  }

  get dateFilter() {
    return this.form.get('dateFilter').value;
  }

  private get tensaoMinima() {
    return this.form.get('tensaoMinima').value;
  }

  private getDashboard(): void {

    let _dateFilter: Date = null;
    if (this.dateFilter !== null && this.dateFilter !== undefined) {
      _dateFilter = this.dateFilter;
    }

    this._deviceId = this.route.snapshot.paramMap.get('id');
    this.sgiService.getDashboard(this._deviceId, _dateFilter != null ? _dateFilter.toJSON() : <string>null, this.seqNumber, this.navigation, this.dashboard.seqNumberb, ProjectEnum.B981).subscribe(d => {
      this.dashboard = Object.assign(new Dashboard(), d);

      this.form.get('numeroEnvios').setValue(d.downloadLink.numeroEnvios);
      this.form.get('tempoTransmissao').setValue(d.downloadLink.tempoTransmissao);
      this.form.get('tipoEnvio').setValue(d.downloadLink.tipoEnvio ? 'true' : 'false');
      this.form.get('tensaoMinima').setValue(d.downloadLink.tensaoMinima);

      if (d === null) {
        this.finishSetTimeout();
        this.showDashboard = false;
      } else {
        // this.startSetTimeout();
        this.showDashboard = true;
      }

      // if (this.seqNumber === 0) {
      //   this.seqNumber = this.dashboard.seqNumber;
      // }
      this.seqNumber = this.dashboard.time;

      this.form.get('serialNumber').setValue(this.dashboard.serialNumber);
      this.form.get('model').setValue(this.dashboard.model);

      this.form.patchValue(this.dashboard);
      this.form.updateValueAndValidity();
    });
  }

  isAnyChange(data: any) {
    if (data.numeroEnvios !== this.numeroEnvios) {
      return true;
    }
    if (data.tempoTransmissao !== this.tempoTransmissao) {
      return true;
    }
    if (data.tipoEnvio !== this.tipoEnvio) {
      return true;
    }
    if (data.tensaoMinima !== this.tensaoMinima) {
      return true;
    }
    return false;
  }

  sendMaps() {
    this.router.navigate([`./radiodados/maps/${this._deviceId}`]);
    // window.open(this.dashboard.linkMaps, '_blank');
  }

}

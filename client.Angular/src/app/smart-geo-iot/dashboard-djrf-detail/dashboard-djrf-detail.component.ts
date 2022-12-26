import { ChangeDeviceDialogComponent } from './../change-device-dialog/change-device-dialog.component';
import { Dashboard } from './../dashboard';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormControl, FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material';
import { SmartGeoIotService } from './../smartgeoiot.service';
import { MessageService } from '../../common/message.service';
import { String } from 'typescript-string-operations';
import { Bits } from '../Bits';
import { environment } from '../../../environments/environment';
import { ProjectEnum } from '../project';

@Component({
  selector: 'app-dashboard-djrf-detail',
  templateUrl: './dashboard-djrf-detail.component.html',
  styleUrls: ['./dashboard-djrf-detail.component.css']
})
export class DashboardDjrfDetailComponent implements OnInit, OnDestroy {
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
      'tipoEnvio': new FormControl({value: null, disabled: true}),
      'numeroEnvios': new FormControl({value: null, disabled: true}),
      'tempoTransmissao': new FormControl({value: null, disabled: true}),
      'tensaoMinima': new FormControl({value: null, disabled: true})
    });

    this.getDashboard();
    this.initializeSetTimeout();
  }

  ngOnDestroy() {
    this.initialSetTimeout = 1;
  }

  updateData() {
    this.getDashboard();
  }

  changeStatus() {
    this.changeDevice = !this.changeDevice;
  }

  initializeSetTimeout() {
    (async () => {
      while (this.initialSetTimeout === 0) {
        await new Promise(resolve => setTimeout(resolve, environment.TIMEOUT_REQUEST_DASHBOARD));
        this.getDashboard();
      }
    })();
  }

  get pageTitle(): string {
    return String.Format('Dados do dispositivo {0} (DJRF)', this.dashboard.name);
  }

  get getTemperature(): number {
    return Number(this.dashboard.temperature);
  }

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

  private get numeroEnvios() {
    return this.form.get('numeroEnvios').value;
  }

  private get tempoTransmissao() {
    return this.form.get('tempoTransmissao').value;
  }

  private get tipoEnvio() {
    return this.form.get('tipoEnvio').value;
  }

  private get tensaoMinima() {
    return this.form.get('tensaoMinima').value;
  }

  private getDashboard(): void {
    this._deviceId = this.route.snapshot.paramMap.get('id');
    this.sgiService.getDashboard(this._deviceId, null, 0, null, 0, ProjectEnum.DJRFleg).subscribe(d => {
      this.dashboard = Object.assign(new Dashboard(), d);

      this.form.get('numeroEnvios').setValue(d.envio);
      this.form.get('tempoTransmissao').setValue(d.periodoTransmissao);
      this.form.get('tipoEnvio').setValue(d.bits.baseTempoUpLink ? 'true' : 'false');
      this.form.get('tensaoMinima').setValue(d.tensaoMinima);

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

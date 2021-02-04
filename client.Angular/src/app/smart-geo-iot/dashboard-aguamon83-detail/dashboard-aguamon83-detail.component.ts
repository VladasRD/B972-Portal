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
  selector: 'app-dashboard-aguamon83-detail',
  templateUrl: './dashboard-aguamon83-detail.component.html',
  styleUrls: ['./dashboard-aguamon83-detail.component.css']
})
export class DashboardAguamon83DetailComponent implements OnInit {
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
      'tipoEnvio': new FormControl(''),
      'numeroEnvios': new FormControl(''),
      'tempoTransmissao': new FormControl('')
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
    return String.Format('Dados do dispositivo {0} (AguaMon)', this.dashboard.name);
  }

  get getTemperature(): number {
    return Number(this.dashboard.temperature);
  }

  sendChanges() {
    if (this.form.get('numeroEnvios').value > 140) {
      this.form.get('numeroEnvios').setValue(140);
    }

    this.sgiService.sendChangesDevice(this._deviceId, this.numeroEnvios, this.tempoTransmissao, this.tipoEnvio, 0)
      .subscribe(() => {
        this.router.navigate(['./sgi/dashboard']);
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

  private get tipoEnvio() {
    return this.form.get('tipoEnvio').value;
  }

  private getDashboard(): void {
    this._deviceId = this.route.snapshot.paramMap.get('id');
    this.sgiService.getDashboard(this._deviceId).subscribe(d => {
      this.dashboard = Object.assign(new Dashboard(), d);

      // this.form.get('numeroEnvios').setValue(d.envio);
      // this.form.get('tempoTransmissao').setValue(d.periodoTransmissao);
      // this.form.get('tipoEnvio').setValue(d.bits.baseTempoUpLink ? 'true' : 'false');

      this.form.patchValue(this.dashboard);
      this.form.updateValueAndValidity();
    });
  }

}

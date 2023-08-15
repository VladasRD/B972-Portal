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
import { environment } from '../../../environments/environment';
import { ProjectEnum } from '../project';
import { AuthService } from '../../common/auth.service';
import { FormUtil } from '../../common/form-util';
import { ChangeNameDialogComponent } from '../../common/change-name-dialog/change-name-dialog.component';

@Component({
  selector: 'app-dashboard-b980-detail',
  templateUrl: './dashboard-b980-detail.component.html',
  styleUrls: ['./dashboard-b980-detail.component.css']
})
export class DashboardB980DetailComponent implements OnInit {
  public _deviceId: string;
  dashboard: Dashboard;
  clientByDevice: Client;
  form: FormGroup;
  changeDevice = false;
  initialSetTimeout = 0;
  showDashboard = true;
  seqNumber = 0;
  navigation = null;

  edit_ed1 = false;
  edit_ed2 = false;
  edit_ed3 = false;
  edit_ed4 = false;
  edit = false;

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
      'model': new FormControl({value: '', disabled: true}),
      'notes': new FormControl(''),
      'ed1': new FormControl({value: '', disabled: true}),
      'ed2': new FormControl({value: '', disabled: true}),
      'ed3': new FormControl({value: '', disabled: true}),
      'ed4': new FormControl({value: '', disabled: true}),
      'sd1': new FormControl({value: '', disabled: true}),
      'sd2': new FormControl({value: '', disabled: true}),
      'ea10': new FormControl({value: '', disabled: true})
    });

    this.form.get('dateFilter').valueChanges.subscribe(val => {
      this.finishSetTimeout();
      this.seqNumber = 0;
      this.updateData();
    });

    this.getDashboard();
    this.initializeSetTimeout();
  }

  get isMobile(): boolean {
    return FormUtil.isMobile();
  }

  get latitude() {
    return this.dashboard ? this.dashboard.latitude : 0;
  }

  get longitude() {
    return this.dashboard ? this.dashboard.longitude : 0;
  }

  get radius() {
    return this.dashboard ? Number(this.dashboard.radius) : 0;
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
        
        // this.seqNumber = (this.seqNumber);
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
    // this.getDashboard();
  }

  changeStatus() {
    this.changeDevice = !this.changeDevice;
  }

  get pageTitle(): string {
    return String.Format('Nível – {0}', this.dashboard.name);
  }

  get getTemperature(): number {
    return Number(this.dashboard.temperature);
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

  get dateFilter() {
    return this.form.get('dateFilter').value;
  }

  private get tipoEnvio() {
    return this.form.get('tipoEnvio').value;
  }

  private get notes() {
    return this.form.get('notes').value;
  }

  private getDashboard(): void {

    let _dateFilter: Date = null;
    if (this.dateFilter !== null && this.dateFilter !== undefined) {
      _dateFilter = this.dateFilter;
    }

    this._deviceId = this.route.snapshot.paramMap.get('id');
    this.sgiService.getDashboard(this._deviceId, _dateFilter != null ? _dateFilter.toJSON() : <string>null, this.seqNumber, this.navigation, 0, ProjectEnum.B980).subscribe(d => {
      this.dashboard = Object.assign(new Dashboard(), d);

      if (d === null) {
        this.finishSetTimeout();
        this.showDashboard = false;
      } else {
        this.showDashboard = true;
      }

      this.form.get('serialNumber').setValue(this.dashboard.serialNumber);
      this.form.get('model').setValue(this.dashboard.model);
      this.form.get('notes').setValue(this.dashboard.notes);

      this.seqNumber = this.dashboard.time;
      this.form.patchValue(this.dashboard);
      this.form.updateValueAndValidity();
    });
  }

  cleanPartial() {
    let totalPartial = Number(this.dashboard.totalizacao.replace(',', '.'));
    
    this.sgiService.cleanPartialbyDevice(this._deviceId, totalPartial)
      .subscribe(() => {
        this.messageService.add('Contador zerado.');
        this.cleanUpdateData();
      },
        err => {
          this.messageService.addError(err.message + ' (zerando contador)');
        });
  }

  get userName(): string {
    let userName = this.authService.signedUser.userClaims.find(c => c.claimType === 'given_name');
    if (userName) {
      return userName.claimValue;
    }
    return '';
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
    return 'logo';
  }

  get deviceId() {
    return this._deviceId;
  }

  saveNotes() {
    this.sgiService.sendChangeNotes(this.deviceId, this.notes)
      .subscribe(() => {
        this.messageService.add('Alteração das anoteações efetuada com sucesso.');
      },
        err => {
          this.messageService.addError(err.message + ' (alterando anotações.)');
        });
  }

  editValueFields(field: string, value: string) {

    this.sgiService.changeFieldsTRM11(this.deviceId, field, value)
      .subscribe(() => {
        this.updateData()
        this.messageService.add('Alteração do Serial Number efetuada com sucesso.');
      },
        err => {
          this.messageService.addError(err.message + ' (alterando Serial Number.)');
        });
  }

  changeNameField(field: string) {
    const dialogRef = this.dialog.open(ChangeNameDialogComponent, {
      width: '80%',
      data: {
        title: `Alteração de nome do campo`,
        message: `Tem certeza que deseja alterar o nome do campo ${field}?`,
        isWarn: false,
        subMessage: "Confirma a alteração?"
      }
    });

    dialogRef.afterClosed().subscribe(result => {

      if (!result || !result.result) {
        return;
      }
      if (result.textMessage === null || result.textMessage === undefined || result.textMessage === '') {
        this.messageService.addError('Por favor, informe o motivo da alteração do nome do campo.');
        return;
      }
      if (result && result.result) {
        this.editValueFields(field, result.textMessage);
      }

    });
  }

}

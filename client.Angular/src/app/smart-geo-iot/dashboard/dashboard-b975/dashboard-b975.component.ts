import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../../common/auth.service';
import { FormGroup } from '@angular/forms';
import { Observable, of } from 'rxjs';
import { GrudList } from '../../../common/grud-list';
import { SmartGeoIotService } from './../../../smart-geo-iot/smartgeoiot.service';
import { B975DevicesDashboardViewModels, DeviceRegistration } from '../../Device';
import { EstadoDJ } from '../../B975';
import { environment } from './../../../../environments/environment';
import { GenericYesNoDialogComponent } from '../../../common/generic-yes-no-dialog/generic-yes-no-dialog.component';
import { MatBottomSheet, MatDialog } from '@angular/material';
import { MessageService } from '../../../common/message.service';
import { ServiceDeskRecord } from '../../service-desk';
import { ServicedeskHistoryBottomsheetComponent } from '../../servicedesk-history-bottomsheet/servicedesk-history-bottomsheet.component';

@Component({
  selector: 'app-dashboard-b975',
  templateUrl: './dashboard-b975.component.html',
  styleUrls: ['./dashboard-b975.component.css']
})
export class DashboardB975Component extends GrudList<Object> implements OnInit {

  form: FormGroup;
  filtrarBloqueios: boolean = false;
  initialSetTimeout = 0;
  displayedColumns: string[] = ['nickName', 'name', 'statusDJ', 'locationCity', 'date', 'chamado'];

  constructor(
    public authService: AuthService,
    private sgiService: SmartGeoIotService,
    public dialog: MatDialog,
    private messageService: MessageService,
    private bottomSheet: MatBottomSheet
    ) {
      super();
      this.form =  new FormGroup({
      });
    }

  public get isSigninMessageVisible() {
    return !this.isLoading && !this.authService.hasSignedBefore && !this.authService.isUserSignedIn;
  }

  public get isLoading() {
    return this.authService.isSigning;
  }

  ngOnInit() {
    this.initializeSetTimeout();
  }

  ngOnDestroy() {
    this.finishSetTimeout();
  }

  finishSetTimeout() {
    this.initialSetTimeout = 1;
  }

  initializeSetTimeout() {
    (async () => {
      while (this.initialSetTimeout === 0) {
        await new Promise(resolve => setTimeout(resolve, environment.TIMEOUT_REQUEST_DASHBOARD));
        
        // this.seqNumber = (this.seqNumber);
        // this.navigation = 'next';
        this.newSearch();
      }
    })();
  }

  startSetTimeout() {
    if (this.initialSetTimeout === 1) {
      this.initialSetTimeout = 0;
      this.initializeSetTimeout();
    }
  }

  getResults(): Observable<B975DevicesDashboardViewModels[]> {
    if (!this.authService.isUserSignedIn) {
      return of (new Document[0]);
    }

    return this.sgiService.getDevicesB975FromDashboard(0, 0,
        this.searchFilter$.getValue(), this.filtrarBloqueios, c => { this._totalCount = c; });
  }

  serviceDeskConfirmDialog(deviceId: string, event: MouseEvent): void {
    event.stopPropagation();

    const dialogRef = this.dialog.open(GenericYesNoDialogComponent, {
      width: '80%',
      data: {
        title: 'Informação do chamado',
        message: 'Por favor, informe o texto para informar no chamado.',
        isWarn: true,
        hasTextMessage: true,
        textMessage: ''
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (!result || !result.result) {
        return;
      }
      if (result.textMessage === null || result.textMessage === undefined || result.textMessage === '') {
        this.messageService.addError('Por favor, informe o texto para informar no chamado.');
        return;
      }
      if (result && result.result) {
        this.serviceDeskSendMessage(deviceId, result.textMessage);
      }
    });
  }

  serviceDeskSendMessage(deviceId: string, reason: string) {
    this.messageService.blockUI();
    this.sgiService.serviceDeskSendMessage(deviceId, reason, 0, null)
      .subscribe(() => {
        this.messageService.add('Informações enviadas para o chamado.');
        this.newSearch();
      },
        err => {
          this.messageService.addError(err.message + ' (adicionando evento ao chamado)');
        });
  }

  serviceDeskConfirmCloseDialog(deviceId: string, event: MouseEvent): void {
    event.stopPropagation();

    const dialogRef = this.dialog.open(GenericYesNoDialogComponent, {
      width: '80%',
      data: {
        title: 'Fechamento do chamado',
        message: 'Por favor, confirma o fechamento do chamado?',
        isWarn: true,
        hasTextMessage: true,
        textMessage: ''
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (!result || !result.result) {
        return;
      }
      if (result.textMessage === null || result.textMessage === undefined || result.textMessage === '') {
        this.messageService.addError('Por favor, informe o texto para informar no chamado.');
        return;
      }
      if (result && result.result) {
        this.serviceDeskClose(deviceId, result.textMessage);
      }
    });
  }
  serviceDeskClose(deviceId: string, reason: string) {
    this.messageService.blockUI();
    this.sgiService.serviceDeskClose(deviceId, reason)
      .subscribe(() => {
        this.messageService.add('Chamado fechado com sucesso.');
        this.newSearch();
      },
        err => {
          this.messageService.addError(err.message + ' (fechando chamado)');
        });
  }

  openHistoryBottomSheet(deviceId: string, event: MouseEvent) {
    event.cancelBubble = true;
    event.preventDefault();
    event.stopPropagation();

    this.sgiService.getHistoryServiceDesk(deviceId, 0, 0, '', c => { this._totalCount = c; }).subscribe(result => {
        this.bottomSheet.open(ServicedeskHistoryBottomsheetComponent, { data: { records: result } });
      });

    return false;
  }


  getClassDiffTime(dateItem: Date) {
    dateItem = new Date(dateItem);
    const currentDate = new Date();
    let time = currentDate.getTime() - dateItem.getTime();
    time = (time/1000)/60; // minutes
    let _return = 'bg-operacional';

    if (time < 35) {
      return 'bg-operacional';
    }
    if (time < 120) {
      return 'bg-carencia';
    }
    if (time < 1440) {
      return 'bg-dormencia';
    }
    if (time >= 1440) {
      return 'bg-aguardando';
    }
    return _return;
  }

  filterStatusBloqueio(): void {
    this.filtrarBloqueios = !this.filtrarBloqueios;
    
    console.log('filterStatusBloqueio...', this.filtrarBloqueios)

    this.newSearch();
  }

  isJammer(value: string): boolean {
      return (value === 'Em Ciclos' || value === 'Em bloqueio');
  }

  bgStatusDJ(value: string): string {
    if (value === 'Aguardando') {
        return EstadoDJ.enumBackground[0];    
    }
    if (value === 'Operacional') {
        return EstadoDJ.enumBackground[1];  
    }
    if (value === 'Em carência') {
        return EstadoDJ.enumBackground[2];  
    }
    if (value === 'Em Ciclos') {
        return EstadoDJ.enumBackground[3];  
    }
    if (value === 'Em bloqueio') {
        return EstadoDJ.enumBackground[4];  
    }
    return EstadoDJ.enumBackground[5];
  }

  textSatusDJ(value: string): string {
    if (value === 'Aguardando') {
        return EstadoDJ.enumText[0];    
    }
    if (value === 'Operacional') {
        return EstadoDJ.enumText[1];  
    }
    if (value === 'Em carência') {
        return EstadoDJ.enumText[2];  
    }
    if (value === 'Em Ciclos') {
        return EstadoDJ.enumText[3];  
    }
    if (value === 'Em bloqueio') {
        return EstadoDJ.enumText[4];  
    }
    return EstadoDJ.enumText[5];
  }

}


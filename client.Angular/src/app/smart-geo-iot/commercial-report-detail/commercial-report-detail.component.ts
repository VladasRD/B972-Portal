import { GenericYesNoDialogComponent } from './../../common/generic-yes-no-dialog/generic-yes-no-dialog.component';
import { FormUtil } from './../../common/form-util';
import { Utils } from './../utils';
import { MatDialog } from '@angular/material';
import { MessageService } from './../../common/message.service';
import { SmartGeoIotService } from './../smartgeoiot.service';
import { ActivatedRoute, Router } from '@angular/router';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { Outgoing } from './../outgoing';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-commercial-report-detail',
  templateUrl: './commercial-report-detail.component.html',
  styleUrls: ['./commercial-report-detail.component.css']
})
export class CommercialReportDetailComponent implements OnInit {
  private _outgoingUId: string;
  outgoing: Outgoing;
  form: FormGroup;
  listYearsFilter: number[] = [];
  listMonthsFilter: any[] = [];
  currentYear: number = new Date().getFullYear();
  currentMonth: number = new Date().getMonth();

  constructor(
    private route: ActivatedRoute,
    private sgiService: SmartGeoIotService,
    private router: Router,
    private messageService: MessageService,
    public dialog: MatDialog,
    private formBuilder: FormBuilder
  ) {
    this.outgoing = new Outgoing();
  }

  displayedColumns: string[] = ['month', 'year', 'developmentValue', 'operationsWNDValue', 'dataCenterValue', 'operationValue', 'description'];
  ngOnInit() {
    this.form = new FormGroup({
      'year': new FormControl(this.isNewOutgoing === true ? this.currentYear : this.outgoing.year, [Validators.required]),
      'month': new FormControl(this.isNewOutgoing === true ? this.currentMonth : this.outgoing.month, [Validators.required]),
      'description': new FormControl(''),
      'developmentValue': new FormControl(0),
      'operationsWNDValue': new FormControl(0),
      'dataCenterValue': new FormControl(0),
      'operationValue': new FormControl(0)
    });
    this.fillYearsList();
    this.fillMonthsList();
    this.getOutgoing();
  }

  get pageTitle(): string {
    if (this.outgoing == null) {
      return '';
    }

    if (this.isNewOutgoing) {
      return 'Novo pacote';
    }
    return `Configuração comercial de ${Utils.enumMonths[this.outgoing.month]} de ${this.outgoing.year}`;
  }

  get isNewOutgoing(): boolean {
    return this._outgoingUId === 'new';
  }

  private getOutgoing(): void {
    this._outgoingUId = this.route.snapshot.paramMap.get('id');
    if (this.isNewOutgoing) {
      return;
    }

    this.sgiService.getOutgoing(this._outgoingUId).subscribe(outgoing => {
      this.outgoing = Object.assign(new Outgoing(), outgoing);
      this.form.patchValue(this.outgoing);
      this.form.updateValueAndValidity();
    });
  }

  saveOutgoing() {

    if (this.form.invalid) {
      return;
    }

    // updates the model
    FormUtil.updateModel(this.form, this.outgoing);

    this.sgiService.saveOutgoing(this.outgoing)
      .subscribe(() => {
        this.router.navigate(['./radiodados/relatorio-comercial']);
        this.messageService.add('Configuração salva.');
      },
        err => {
          this.messageService.addError(err.message + ' (salvando configuração)');
        });
  }

  private deleteOutgoing() {
    this.sgiService.deleteOutgoing(this.outgoing.outgoingUId)
      .subscribe(() => {
        this.messageService.add('Configuração removida.');
        this.router.navigate(['./radiodados/relatorio-comercial']);
      },
        err => {
          this.messageService.addError(err.message + ' (removendo configuração)');
        });
  }

  openConfirmDeleteDialog(): void {
    const dialogRef = this.dialog.open(GenericYesNoDialogComponent, {
      width: '80%',
      data: { title: 'Remover configuração', message: 'Tem certeza que deseja remover essa configuração?', isWarn: true }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result === true) {
        this.deleteOutgoing();
      }
    });
  }

  private fillYearsList(): void {
    for (let y = 2019; y <= this.currentYear; y++) {
      this.listYearsFilter.push(y);
    }


    this.form.get('year').setValue(this.isNewOutgoing ? this.currentYear : this.outgoing.year);
  }

  private fillMonthsList(): void {
    for (let m = 0; m <= 12; m++) {
      this.listMonthsFilter.push({ number: m, description: Utils.enumMonths[m] });
    }

    this.form.get('month').setValue(this.isNewOutgoing ? this.currentMonth : this.outgoing.month);
  }

  get yearFilter() {
    return this.form.get('year').value;
  }

  get monthFilter() {
    return this.form.get('month').value;
  }

}

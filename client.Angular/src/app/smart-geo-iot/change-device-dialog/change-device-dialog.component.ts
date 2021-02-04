import { FormGroup, FormControl } from '@angular/forms';
import { SmartGeoIotService } from './../smartgeoiot.service';
import { Component, OnInit, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material';
import { UserPickerDialogComponent } from '../../security/user-picker-dialog/user-picker-dialog.component';

@Component({
  selector: 'app-change-device-dialog',
  templateUrl: './change-device-dialog.component.html',
  styleUrls: ['./change-device-dialog.component.css']
})
export class ChangeDeviceDialogComponent implements OnInit {

  form: FormGroup;
  numeroEnvios: number;
  tempoTransmissao: string;
  tipoEnvio: boolean;
  tensaoMinima: string;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    public dialog: MatDialog,
    private sgiService: SmartGeoIotService
  ) {
    this.numeroEnvios = this.data.numeroEnvios;
    this.tempoTransmissao = this.data.tempoTransmissao;
    this.tipoEnvio = this.data.tipoEnvio ? true : false;
    this.tensaoMinima = this.data.tensaoMinima;
  }

  ngOnInit() {
    this.form = new FormGroup({
      'tipoEnvio': new FormControl(this.data.tipoEnvio),
      'numeroEnvios': new FormControl(this.data.numeroEnvios),
      'tempoTransmissao': new FormControl(this.data.tempoTransmissao),
      'tensaoMinima': new FormControl(this.data.tensaoMinima)
    });
  }

  get closeReturn() {
    return { data:
      {
        tipoEnvio: this._tipoEnvio,
        numeroEnvios: this._numeroEnvios,
        tempoTransmissao: this._tempoTransmissao,
        tensaoMinima: this._tensaoMinima
      }
    };
  }

  private get _numeroEnvios() {
    return this.form.get('numeroEnvios').value;
  }

  private get _tempoTransmissao() {
    return this.form.get('tempoTransmissao').value;
  }

  private get _tipoEnvio() {
    return this.form.get('tipoEnvio').value;
  }

  private get _tensaoMinima() {
    return this.form.get('tensaoMinima').value;
  }

}
import { Firmware } from './../firmware';
import { DeviceRegistration, Device } from './../device';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormControl, FormGroup, Validators, FormBuilder } from '@angular/forms';
import { MatDialog } from '@angular/material';
import { SmartGeoIotService } from './../smartgeoiot.service';
import { MessageService } from '../../common/message.service';
import { GenericYesNoDialogComponent } from '../../common/generic-yes-no-dialog/generic-yes-no-dialog.component';
import { String } from 'typescript-string-operations';
import { FormUtil } from '../../common/form-util';

@Component({
  selector: 'app-firmware-detail',
  templateUrl: './firmware-detail.component.html',
  styleUrls: ['./firmware-detail.component.css']
})
export class FirmwareDetailComponent implements OnInit {
  private _deviceId: string;
  firmware: Firmware;
  form: FormGroup;

  constructor(
    private route: ActivatedRoute,
    private sgiService: SmartGeoIotService,
    private router: Router,
    private messageService: MessageService,
    public dialog: MatDialog,
    private formBuilder: FormBuilder
  ) {
    this.firmware = new Firmware();
  }

  ngOnInit() {
    this.form = new FormGroup({
      'dataCompilacao': new FormControl({ value: null, disabled: true }),
      'serieDispositivo': new FormControl({ value: null, disabled: true }),
      'hardware': new FormControl({ value: null, disabled: true }),
      'firmware': new FormControl({ value: null, disabled: true }),
      'proc': new FormControl({ value: null, disabled: true }),
      'placa': new FormControl({ value: null, disabled: true }),
      'vPlaca': new FormControl({ value: null, disabled: true }),
      'nAplic': new FormControl({ value: null, disabled: true }),
      'id': new FormControl({ value: null, disabled: true })
    });

    this.getDeviceRegistration();
  }

  get pageTitle(): string {
    return String.Format('Firmware do dispositivo {0}', this._deviceId);
  }

  private getDeviceRegistration(): void {
    this._deviceId = this.route.snapshot.paramMap.get('id');

    this.sgiService.getFirmware(this._deviceId).subscribe(firmware => {
      this.firmware = Object.assign(new Firmware(), firmware);
      this.form.patchValue(this.firmware);
      this.form.updateValueAndValidity();
    });
  }

}

import { Component, Input, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { MessageService } from '../../common/message.service';
import { SmartGeoIotService } from '../../smart-geo-iot/smartgeoiot.service';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-serial-model',
  templateUrl: './serial-model.component.html',
  styleUrls: ['./serial-model.component.css']
})
export class SerialModelComponent implements OnInit {

  form: FormGroup;
  edit_serialNumber = false;
  edit_model = false;
  @Input() deviceId: string;

  constructor(
    private sgiService: SmartGeoIotService,
    private messageService: MessageService
    ) {
    
  }

  ngOnInit() {
    this.form = new FormGroup({
      'serialNumber': new FormControl({value: '', disabled: true}),
      'model': new FormControl({value: '', disabled: true})
    });

    this.getDeviceRegistration();
  }

  private getDeviceRegistration(): void {
    this.sgiService.getDeviceRegistrationByDeviceID(this.deviceId).subscribe(deviceRegistration => {
      this.form.get('serialNumber').setValue(deviceRegistration.serialNumber);
      this.form.get('model').setValue(deviceRegistration.model);
    });
  }

  change_edit_serialNumber(type: number) {
    this.edit_serialNumber = !this.edit_serialNumber;

    if (this.edit_serialNumber) {
      this.form.controls['serialNumber'].enable();
      this.form.updateValueAndValidity();
    } else {
      this.form.controls['serialNumber'].disable();
      this.form.updateValueAndValidity();
    }

    if (type === 2) {
      // salvar informação
      this.sendSerialNumber(this.form.get('serialNumber').value);
    }
  }

  change_edit_model(type: number) {
    this.edit_model = !this.edit_model;

    if (this.edit_model) {
      this.form.controls['model'].enable();
      this.form.updateValueAndValidity();
    } else {
      this.form.controls['model'].disable();
      this.form.updateValueAndValidity();
    }

    if (type === 2) {
      // salvar informação
      this.sendModel(this.form.get('model').value);
    }
  }

  sendSerialNumber(data: string) {

    this.sgiService.sendChangeSerialNumber(this.deviceId, data)
      .subscribe(() => {
        this.messageService.add('Alteração do Serial Number efetuada com sucesso.');
      },
        err => {
          this.messageService.addError(err.message + ' (alterando Serial Number.)');
        });
  }

  sendModel(data: string) {
    this.sgiService.sendChangeModel(this.deviceId, data)
      .subscribe(() => {
        this.messageService.add('Alteração do Serial Number efetuada com sucesso.');
      },
        err => {
          this.messageService.addError(err.message + ' (alterando Serial Number.)');
        });
  }

  goToMaps() {
    const baseUrl = `${environment.CLIENT_URL}/radiodados/maps/${this.deviceId}`;
    window.open(baseUrl, '_blank');
  }

  goToInfos() {
    const baseUrl = `${environment.CLIENT_URL}/radiodados/dados-firmware/${this.deviceId}`;
    window.open(baseUrl, '_blank');
  }



}

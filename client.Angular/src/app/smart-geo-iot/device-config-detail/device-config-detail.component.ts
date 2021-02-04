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
  selector: 'app-device-config-detail',
  templateUrl: './device-config-detail.component.html',
  styleUrls: ['./device-config-detail.component.css']
})
export class DeviceConfigDetailComponent implements OnInit {
  private _deviceCustomUId: string;
  deviceRegistration: DeviceRegistration;
  form: FormGroup;
  listDevicesFilter: Device[] = [];

  constructor(
    private route: ActivatedRoute,
    private sgiService: SmartGeoIotService,
    private router: Router,
    private messageService: MessageService,
    public dialog: MatDialog,
    private formBuilder: FormBuilder
  ) {
    this.deviceRegistration = new DeviceRegistration();
  }

  displayedColumns: string[] = [ 'name', 'nickName' ];

  ngOnInit() {
    this.form = new FormGroup({
      'name': new FormControl(null, [Validators.required, Validators.maxLength(500)]),
      'nickName': new FormControl(null, [Validators.required]),
      'deviceId': new FormControl('', Validators.required),
      'packageUId': new FormControl('', Validators.required),
      'projectUId': new FormControl('', Validators.required)
    });

    this.getDeviceRegistration();
    this.fillDevicesList();
  }

  get pageTitle(): string {
    if (this.deviceRegistration == null) {
      return '';
    }

    if (this.isNewDeviceRegistration) {
      return 'Novo dispositivo';
    }
    return String.Format('Dispositivo {0}', this.deviceRegistration.name);
  }

  get isNewDeviceRegistration(): boolean {
    return this._deviceCustomUId === 'new';
  }

  private fillDevicesList(): void {
    this.sgiService.getDevices(0, 0, '', 'only_device').subscribe(devices => {
      if (!devices) {
        return;
      }
      this.listDevicesFilter = devices;
    });
  }

  private getDeviceRegistration(): void {
    this._deviceCustomUId = this.route.snapshot.paramMap.get('id');
    if (this.isNewDeviceRegistration) {
      return;
    }

    this.sgiService.getDeviceRegistration(this._deviceCustomUId).subscribe(deviceRegistration => {
      this.deviceRegistration = Object.assign(new DeviceRegistration(), deviceRegistration);
      this.form.patchValue(this.deviceRegistration);
      this.form.updateValueAndValidity();
    });
  }

  saveDeviceRegistration() {
    if (this.form.invalid) {
      return;
    }

    // updates the model
    FormUtil.updateModel(this.form, this.deviceRegistration);

    // updates the model
    this.deviceRegistration.deviceId = this.form.get('deviceId').value;
    this.deviceRegistration.packageUId = this.form.get('packageUId').value;
    this.deviceRegistration.projectUId = this.form.get('projectUId').value;

    this.sgiService.saveDeviceRegistration(this.deviceRegistration)
      .subscribe(() => {
        this.router.navigate(['./sgi/dispositivos']);
        this.messageService.add('Dispositivo salvo.');
      },
        err => {
          this.messageService.addError(err.message + ' (salvando dispositivo)');
        });
  }

  private deleteDeviceRegistration() {
    this.sgiService.deleteDeviceRegistration(this.deviceRegistration.deviceCustomUId)
      .subscribe(() => {
        this.messageService.add('Dispositivo removido.');
        this.router.navigate(['./sgi/dispositivos']);
      },
        err => {
          this.messageService.addError(err.message + ' (removendo dispositivo)');
        });
  }

  openConfirmDeleteDialog(): void {
    const dialogRef = this.dialog.open(GenericYesNoDialogComponent, {
      width: '80%',
      data: { title: 'Remover dispositivo', message: 'Tem certeza que deseja remover esse dispositivo?', isWarn: true }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result === true) {
        this.deleteDeviceRegistration();
      }
    });
  }

}

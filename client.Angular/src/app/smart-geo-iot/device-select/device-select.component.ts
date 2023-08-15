import { DeviceRegistration } from './../device';
import { Component, Input, OnInit } from '@angular/core';
import { SmartGeoIotService } from './../smartgeoiot.service';
import { FormControl } from '@angular/forms';

@Component({
  selector: 'app-device-select',
  templateUrl: './device-select.component.html',
  styleUrls: ['./device-select.component.css']
})
export class DeviceSelectComponent implements OnInit {
  listDevicesFilter: DeviceRegistration[] = [];

  @Input() control: FormControl;
  @Input() nullable = false;
  @Input() appearance = 'outline';
  @Input() floatLabel = 'float';
  @Input() hasPlaceHolder = true;
  @Input() project = null;
  @Input() fxFlex = '100%';

  constructor(private sgiService: SmartGeoIotService) {
  }

  ngOnInit() {
    this.fillDevicesList();
  }

  private fillDevicesList(): void {
    this.sgiService.getDevicesOfClient(this.project).subscribe(devices => {
      if (!devices) {
        return;
      }
      this.listDevicesFilter = devices;
    });
  }

}

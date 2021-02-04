import { DeviceRegistration } from './../device';
import { Component, OnInit } from '@angular/core';
import { GrudList } from '../../common/grud-list';
import { FormGroup } from '@angular/forms';
import { SmartGeoIotService } from '../smartgeoiot.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-device-config-list',
  templateUrl: './device-config-list.component.html',
  styleUrls: ['./device-config-list.component.css']
})
export class DeviceConfigListComponent extends GrudList<Object> implements OnInit {
  form: FormGroup;
  displayedColumns: string[] = ['name', 'description', 'device', 'package', 'project'];

  constructor(private sgiService: SmartGeoIotService) {
    super();
    this.form =  new FormGroup({

    });
  }

  ngOnInit() {
  }

  getResults(): Observable<DeviceRegistration[]> {
    return this.sgiService.getDevicesRegistrations(this._skip, this._pageSize,
      this.searchFilter$.getValue(), c => { this._totalCount = c; });
  }

}

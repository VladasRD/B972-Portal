import { DeviceRegistration } from './../Device';
import { SmartGeoIotService } from './../smartgeoiot.service';
import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../common/auth.service';
import { FormGroup, FormControl } from '@angular/forms';
import { Observable } from 'rxjs';
import { GrudList } from '../../common/grud-list';

@Component({
  selector: 'app-firmware-list',
  templateUrl: './firmware-list.component.html',
  styleUrls: ['./firmware-list.component.css']
})
export class FirmwareListComponent extends GrudList<Object> implements OnInit {

  form: FormGroup;
  constructor(
    public authService: AuthService,
    private sgiService: SmartGeoIotService
  ) {
    super();
    this.form = new FormGroup({
      'clientFilter': new FormControl(null)
    });

    this.form.get('clientFilter').valueChanges.subscribe(val => {
      this.newSearch();
    });
  }

  ngOnInit() {
  }

  get clientFilterControl() {
    return this.form.get('clientFilter');
  }

  get clientFilter() {
    if (this.clientFilterControl.value) {
      return this.clientFilterControl.value.clientUId;
    }
    return null;
  }

  getResults(): Observable<DeviceRegistration[]> {
    return this.sgiService.getDevicesFromFirmware(this._skip, this._pageSize,
      this.searchFilter$.getValue(), this.clientFilter, c => { this._totalCount = c; });
  }

}

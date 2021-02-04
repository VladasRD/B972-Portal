import { Component, OnInit } from '@angular/core';
import { AuthService } from '../common/auth.service';
import { Device, DeviceRegistration } from './../smart-geo-iot/device';
import { FormGroup } from '@angular/forms';
import { Observable } from 'rxjs';
import { GrudList } from '../common/grud-list';
import { SmartGeoIotService } from './../smart-geo-iot/smartgeoiot.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent extends GrudList<Object> implements OnInit {

  form: FormGroup;
  constructor(
    public authService: AuthService,
    private sgiService: SmartGeoIotService
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
  }

  getResults(): Observable<DeviceRegistration[]> {
    return this.sgiService.getDevicesFromDashboard(this._skip, this._pageSize,
      this.searchFilter$.getValue(), c => { this._totalCount = c; });
  }

}

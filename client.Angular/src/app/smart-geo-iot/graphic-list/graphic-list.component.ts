import { DeviceRegistration } from './../Device';
import { Component, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { Observable } from 'rxjs';
import { GrudList } from '../../common/grud-list';
import { SmartGeoIotService } from './../smartgeoiot.service';

@Component({
  selector: 'app-graphic-list',
  templateUrl: './graphic-list.component.html',
  styleUrls: ['./graphic-list.component.css']
})
export class GraphicListComponent extends GrudList<Object> implements OnInit {

  form: FormGroup;

  constructor(private sgiService: SmartGeoIotService) {
    super();
    this.form =  new FormGroup({
    });
  }

  ngOnInit() {
  }

  getResults(): Observable<DeviceRegistration[]> {
    return this.sgiService.getDevicesFromDashboard(this._skip, this._pageSize,
      this.searchFilter$.getValue(), c => { this._totalCount = c; });
  }

}

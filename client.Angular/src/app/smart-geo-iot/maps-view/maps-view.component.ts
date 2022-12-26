import { SmartGeoIotService } from './../smartgeoiot.service';
import { DeviceLocation } from './../Device';
import { ActivatedRoute } from '@angular/router';
import { Component, OnInit } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';
import { Location } from '@angular/common';

@Component({
  selector: 'app-maps-view',
  templateUrl: './maps-view.component.html',
  styleUrls: ['./maps-view.component.css']
})
export class MapsViewComponent implements OnInit {

  public _deviceId: string;
  deviceLoc: DeviceLocation;
  url: any;

  constructor(
    private route: ActivatedRoute,
    private sgiService: SmartGeoIotService,
    private location: Location,
    private sanitizer: DomSanitizer
    ) {
      this.deviceLoc = new DeviceLocation();
      this.url = 'https://www.rafaelestevao.com.br/show-maps/?lat=0&long=0&radius=0&z=0';
  }

  ngOnInit() {
    this.getDeviceLocation();
  }

  back(event) {
    event.stopPropagation();
    this.location.back();
  }

  close() {
    window.close();
  }

  private getDeviceLocation(): void {
    this._deviceId = this.route.snapshot.paramMap.get('id');

    this.sgiService.getDeviceLocation(this._deviceId).subscribe(deviceLoc => {
      this.deviceLoc = Object.assign(new DeviceLocation(), deviceLoc);

      let zoom = 13;
      if (Number(this.deviceLoc.radius) > 15000) {
        zoom = 11;
      } else if (Number(this.deviceLoc.radius) > 10000) {
        zoom = 12;
      }

      this.url = this.sanitizer.bypassSecurityTrustResourceUrl(`https://www.rafaelestevao.com.br/show-maps?lat=${this.deviceLoc.latitude}&long=${this.deviceLoc.longitude}&radius=${this.deviceLoc.radius}&z=${zoom}`);
    });
  }

}

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
  public _latitude: number;
  public _longitude: number;
  public _radius: number;
  deviceLoc: DeviceLocation;
  url: any;

  constructor(
    private route: ActivatedRoute,
    private sgiService: SmartGeoIotService,
    private location: Location,
    private sanitizer: DomSanitizer
    ) {
      this.deviceLoc = new DeviceLocation();
      this.url = 'https://rdportal.com.br/maps/maps.html?lat=0&long=0&radius=0&z=0';
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
    this._latitude = Number(this.route.snapshot.paramMap.get('latitude'));
    this._longitude = Number(this.route.snapshot.paramMap.get('longitude'));
    this._radius = Number(this.route.snapshot.paramMap.get('radius'));

    if (this._latitude && this._longitude && this._radius) {
      // aqui vai para o maps com os dados de localização que recebeu da tela
      let zoom = 13;
      if (this._radius > 15000) {
        zoom = 11;
      } else if (this._radius > 10000) {
        zoom = 12;
      }
      this.url = this.sanitizer.bypassSecurityTrustResourceUrl(`https://rdportal.com.br/maps/maps.html?lat=${this._latitude}&long=${this._longitude}&radius=${this._radius}&z=${zoom}`);
    } else {
      // aqui vai para o maps consultando a última localização do dispositivo
      this.sgiService.getDeviceLocation(this._deviceId).subscribe(deviceLoc => {
        this.deviceLoc = Object.assign(new DeviceLocation(), deviceLoc);
  
        let zoom = 13;
        if (Number(this.deviceLoc.radius) > 15000) {
          zoom = 11;
        } else if (Number(this.deviceLoc.radius) > 10000) {
          zoom = 12;
        }
  
        this.url = this.sanitizer.bypassSecurityTrustResourceUrl(`https://rdportal.com.br/maps/maps.html?lat=${this.deviceLoc.latitude}&long=${this.deviceLoc.longitude}&radius=${this.deviceLoc.radius}&z=${zoom}`);
      });
    }


  }

}

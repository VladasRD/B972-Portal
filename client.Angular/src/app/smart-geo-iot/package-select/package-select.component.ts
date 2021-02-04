import { Component, Input, OnInit } from '@angular/core';
import { SmartGeoIotService } from './../smartgeoiot.service';
import { FormControl } from '@angular/forms';
import { Package } from '../package';

@Component({
  selector: 'app-package-select',
  templateUrl: './package-select.component.html',
  styleUrls: ['./package-select.component.css']
})
export class PackageSelectComponent implements OnInit {
  listPackageFilter: Package[] = [];

  @Input() control: FormControl;
  @Input() nullable = false;
  @Input() appearance = 'outline';
  @Input() floatLabel = 'float';
  @Input() hasPlaceHolder = true;

  constructor(private sgiService: SmartGeoIotService) {
  }

  ngOnInit() {
    this.fillPackagesList();
  }

  private fillPackagesList(): void {
    this.sgiService.getPackages().subscribe(packages => {
      if (!packages) {
        return;
      }
      this.listPackageFilter = packages;
    });
  }

}

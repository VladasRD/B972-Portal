import { Package } from './../package';
import { Component, OnInit } from '@angular/core';
import { GrudList } from '../../common/grud-list';
import { FormGroup } from '@angular/forms';
import { SmartGeoIotService } from '../smartgeoiot.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-package-list',
  templateUrl: './package-list.component.html',
  styleUrls: ['./package-list.component.css']
})
export class PackageListComponent extends GrudList<Object> implements OnInit {
  form: FormGroup;
  displayedColumns: string[] = ['name', 'description', 'type'];

  constructor(private sgiService: SmartGeoIotService) {
    super();
    this.form =  new FormGroup({

    });
  }

  ngOnInit() {
  }

  getResults(): Observable<Package[]> {
    return this.sgiService.getPackages(this._skip, this._pageSize,
      this.searchFilter$.getValue(), c => { this._totalCount = c; });
  }

}

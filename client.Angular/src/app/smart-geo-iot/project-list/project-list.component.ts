import { SmartGeoIotService } from '../../smart-geo-iot/smartgeoiot.service';
import { Project } from '../../smart-geo-iot/project';
import { Component, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { Observable } from 'rxjs';
import { GrudList } from '../../common/grud-list';

@Component({
  selector: 'app-project-list',
  templateUrl: './project-list.component.html',
  styleUrls: ['./project-list.component.css']
})
export class ProjectListComponent extends GrudList<Object> implements OnInit {

  form: FormGroup;
  displayedColumns: string[] = ['code', 'name', 'description'];

  constructor(private sgiService: SmartGeoIotService) {
    super();
    this.form =  new FormGroup({

    });
  }

  ngOnInit() {
  }

  getResults(): Observable<Project[]> {
    return this.sgiService.getProjects(this._skip, this._pageSize,
      this.searchFilter$.getValue(), c => { this._totalCount = c; });
  }

}

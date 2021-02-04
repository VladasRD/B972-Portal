import { Router } from '@angular/router';
import { Project } from './../project';
import { Component, OnInit } from '@angular/core';
import { GrudList } from '../../common/grud-list';
import { SmartGeoIotService } from './../../smart-geo-iot/smartgeoiot.service';
import { FormGroup } from '@angular/forms';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-report',
  templateUrl: './report.component.html',
  styleUrls: ['./report.component.css']
})
export class ReportComponent extends GrudList<Object> implements OnInit {

  form: FormGroup;
  isVisible = false;
  constructor(
    private sgiService: SmartGeoIotService,
    private router: Router
    ) {
    super();
    this.form = new FormGroup({
    });
  }

  ngOnInit() {
  }

  getResults(): Observable<Project[]> {
    const projects = this.sgiService.getMeProjects();
    projects.subscribe(s => {
      const quantity = s.reduce((acum, curr) => {
        return acum + 1;
      }, 0);
      if (quantity === 1) {
        this.router.navigate([`./sgi/relatorio/${s[0].getUrlReportLink}`]);
      } else {
        this.isVisible = true;
      }
    });
    return projects;
  }

}

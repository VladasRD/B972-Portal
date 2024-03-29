import { Router } from '@angular/router';
import { Project, ProjectEnum } from './../../project';
import { Component, OnInit } from '@angular/core';
import { GrudList } from '../../../common/grud-list';
import { SmartGeoIotService } from './../../../smart-geo-iot/smartgeoiot.service';
import { FormGroup } from '@angular/forms';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-dashboard-projects',
  templateUrl: './dashboard-projects.component.html',
  styleUrls: ['./dashboard-projects.component.css']
})
export class DashboardProjectsComponent extends GrudList<Object> implements OnInit {

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
        let linkRedirect = `./radiodados/relatorio/${s[0].getUrlReportLink}` ;

        if (s[0].code == ProjectEnum.B975) {
          linkRedirect = `/${s[0].getURLDashboardLink}`;
        }

        this.router.navigate([`${linkRedirect}`]);
      } else {
        this.isVisible = true;
      }
    });
    return projects;
  }

}
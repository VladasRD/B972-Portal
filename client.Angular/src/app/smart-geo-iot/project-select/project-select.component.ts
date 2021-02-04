import { Project } from './../project';
import { Component, Input, OnInit } from '@angular/core';
import { SmartGeoIotService } from './../smartgeoiot.service';
import { FormControl } from '@angular/forms';

@Component({
  selector: 'app-project-select',
  templateUrl: './project-select.component.html',
  styleUrls: ['./project-select.component.css']
})
export class ProjectSelectComponent implements OnInit {
  listProjectFilter: Project[] = [];

  @Input() control: FormControl;
  @Input() nullable = false;
  @Input() appearance = 'outline';
  @Input() floatLabel = 'float';
  @Input() hasPlaceHolder = true;

  constructor(private sgiService: SmartGeoIotService) {
  }

  ngOnInit() {
    this.fillProjectsList();
  }

  private fillProjectsList(): void {
    this.sgiService.getProjects().subscribe(projects => {
      if (!projects) {
        return;
      }
      this.listProjectFilter = projects;
    });
  }

}
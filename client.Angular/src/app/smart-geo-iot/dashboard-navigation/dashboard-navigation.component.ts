import { Component, OnInit } from '@angular/core';
import { FormUtil } from '../../common/form-util';

@Component({
  selector: 'app-dashboard-navigation',
  templateUrl: './dashboard-navigation.component.html',
  styleUrls: ['./dashboard-navigation.component.css']
})
export class DashboardNavigationComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

  get isMobile(): boolean {
    return FormUtil.isMobile();
  }

}

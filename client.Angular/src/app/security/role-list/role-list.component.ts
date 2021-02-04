import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';

import { GrudList } from '../../common/grud-list';
import { SecurityService } from '../security.service';
import { AppRole } from '../../common/appUser';

@Component({
  selector: 'app-role-list',
  templateUrl: './role-list.component.html',
  styleUrls: ['./role-list.component.css']
})
export class RoleListComponent extends GrudList<AppRole> implements OnInit {

  constructor(private securityService: SecurityService) {
    super();
  }

  ngOnInit() {
  }

  getResults(): Observable<AppRole[]> {
    return this.securityService.getRoles(this.searchFilter$.getValue(), this._skip,
      this._pageSize, c => { this._totalCount = c; });
  }

}

import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { Observable } from 'rxjs';

import { GrudList } from '../../common/grud-list';
import { SecurityService } from '../security.service';
import { AppUser } from '../../common/appUser';

@Component({
  selector: 'app-user-list',
  templateUrl: './user-list.component.html',
  styleUrls: ['./user-list.component.css']
})
export class UserListComponent extends GrudList<AppUser> implements OnInit {

  form: FormGroup;

  private get lockedOutFilter() {
    const status = this.form.get('statusFilter').value;
    return status === 'all' ? null : status === 'blocked' ? true : false;
  }

  constructor(private securityService: SecurityService) {
    super();
    this.form =  new FormGroup({
      'statusFilter': new FormControl('all')
    });
    this.form.get('statusFilter').valueChanges.subscribe(val => { this.newSearch(); });
  }

  ngOnInit() {
  }

  getResults(): Observable<AppUser[]> {
    return this.securityService.getUsers(this.searchFilter$.getValue(), this.lockedOutFilter, this._skip,
      this._pageSize, c => { this._totalCount = c; });
  }

}

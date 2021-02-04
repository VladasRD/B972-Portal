import { Component, OnInit, Inject } from '@angular/core';
import { SecurityService } from '../security.service';
import { MAT_DIALOG_DATA, MatDialogRef, MatCheckboxChange, MatDialogConfig } from '@angular/material';
import { I18n } from '@ngx-translate/i18n-polyfill';
import { Router } from '@angular/router';
import { AppUser } from '../../common/appUser';
import { Observable } from 'rxjs';
import { GrudList } from '../../common/grud-list';

@Component({
  selector: 'app-user-picker-dialog',
  templateUrl: './user-picker-dialog.component.html',
  styleUrls: ['./user-picker-dialog.component.css']
})
export class UserPickerDialogComponent extends GrudList<AppUser> implements OnInit {

  displayedColumns: string[];

  private selections: AppUser[];

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    private dialogRef: MatDialogRef<UserPickerDialogComponent>,
    private securityService: SecurityService, private i18n: I18n, private router: Router) {
    super();
    this._pageSize = 7;
    this.selections = [];
    if (data.multiSelect) {
      this.displayedColumns = [ 'thumb', 'name', 'check' ];
    } else {
      this.displayedColumns = [ 'thumb', 'name' ];
    }
  }

  static config(multiSelect: boolean, atRoles: string[] = null, withClaims: string[] = null): MatDialogConfig {
    return { width: '640px', height: '560px', data: { multiSelect: multiSelect, atRoles: atRoles, withClaims: withClaims } };
  }

  ngOnInit() {
  }

  getResults(): Observable<AppUser[]> {
    return this.securityService.getUsers(this.searchFilter$.getValue(), null, this._skip, this._pageSize,
    c => { this._totalCount = c; }, this.data.atRoles, this.data.withClaims);
  }

  selectUser(user: AppUser) {
    const selectedUser = this.selections.find(u => u.id === user.id);
    if (!selectedUser) {
      this.selections.push(user);
    } else {
      const idx = this.selections.indexOf(selectedUser);
      if (idx >= 0) {
        this.selections.splice(idx, 1);
      }
    }

    if (!this.data.multiSelect) {
        this.addSelections();
    }
  }

  isSelected(id: string) {
    return this.selections.find(u => u.id === id);
  }

  get isAddButtonVisible() {
    return this.data.multiSelect && this.selections.length > 0;
  }

  addSelections() {
    this.dialogRef.close(this.selections);
  }

}

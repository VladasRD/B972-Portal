import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialog, MatTabChangeEvent } from '@angular/material';

import { I18n } from '@ngx-translate/i18n-polyfill';

import { SecurityService } from '../security.service';

import { String } from 'typescript-string-operations';
import { MessageService } from '../../common/message.service';
import { FormUtil } from '../../common/form-util';
import { GenericYesNoDialogComponent } from '../../common/generic-yes-no-dialog/generic-yes-no-dialog.component';
import { AppRole, AppUser } from '../../common/appUser';
import { UserPickerDialogComponent } from '../user-picker-dialog/user-picker-dialog.component';


@Component({
  selector: 'app-role-detail',
  templateUrl: './role-detail.component.html',
  styleUrls: ['./role-detail.component.css']
})
export class RoleDetailComponent implements OnInit {

  private _roleId: string;
  role: AppRole;

  form: FormGroup;

  members: AppUser[] = [];

  constructor(
    private route: ActivatedRoute,
    private securityService: SecurityService,
    private i18n: I18n,
    private router: Router,
    private messageService: MessageService,
    public dialog: MatDialog
  ) {
    this.role = new AppRole();
  }

  ngOnInit() {
    this.form = new FormGroup({
      'name': new FormControl(null, Validators.required),
      'description': new FormControl(null, Validators.required),
      'isSystemRole': new FormControl(null)
    });
    this.getRole();
  }

  get pageTitle(): string {
    if (this._roleId === 'new') {
      return this.i18n('New Group');
    }
    if (this.role == null) {
      return '';
    }
    return this.role.name;
  }

  private getRole(): void {
    this._roleId = this.route.snapshot.paramMap.get('id');
    if (this.isNewRole) {
      return;
    }

    this.securityService.getRole(this._roleId).subscribe(role => {
      this.role = role;
      this.form.patchValue(this.role);
      this.form.updateValueAndValidity();
    });

  }

  get isNewRole(): boolean {
    return this._roleId === 'new';
  }

  saveRole(formValue: any) {

    if (this.form.invalid) {
      return;
    }

    // updates the model
    FormUtil.updateModel(this.form, this.role);
    this.securityService.saveRole(this.role)
      .subscribe(role => {
        this.router.navigate(['./security/roles']);
        this.messageService.add(this.i18n('Group saved.'));
      },
      err => {
        this.messageService.addError(err.message + ' (saving role)');
      });
  }

  openConfirmDeleteDialog(): void {
    const dialogRef = this.dialog.open(GenericYesNoDialogComponent, {
      width: '80%',
      data: { title: this.i18n('Remove group'), message: this.i18n('Are you sure you want to remove this group?'), isWarn: true}
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result === true) {
        this.deleteRole();
      }
    });
  }

  private deleteRole() {
    this.securityService.deleteRole(this.role.id)
    .subscribe(() => {
      this.messageService.add(this.i18n('Group deleted.'));
      this.router.navigate(['./security/roles']);
    },
    err => {
      this.messageService.addError(err.message + ' (deleting role)');
    });
  }

  addMember() {
    const dialogRef = this.dialog.open(UserPickerDialogComponent, UserPickerDialogComponent.config(false));
    dialogRef.afterClosed().subscribe(
      users => {
        if (!users) {
          return;
        }
        users.forEach(u => {
          const alreadyTherUser = this.members.find(ux => ux.id === u.id);
          if (!alreadyTherUser) {
            this.securityService.addUserToRole(this._roleId, u.id).subscribe(r => { this.members.push(u); });
            this.messageService.add(this.i18n('User added to role.'));
          }
        });
      });
  }

  removeMember(member: AppUser) {
    this.securityService.removeUserFromRole(this._roleId, member.id).subscribe(r => {
      const idx = this.members.indexOf(member);
      this.members.splice(idx, 1);
      this.messageService.add(this.i18n('User removed from role.'));
    });
  }

  tabChange(event: MatTabChangeEvent) {
    if (event.index === 1 && this.members.length === 0) {
      this.getMembers();
    }
  }

  private getMembers() {
    this.securityService.getUsersAtRole(this._roleId).subscribe(
      members => {
        this.members = members;
      }
    );
  }

  get canEditMembers() {
    return this.isNewRole;
  }

  get isSystemRole(): boolean {
    return this.role.isSystemRole;
  }

  changeSystemRole() {
    this.role.isSystemRole = !this.role.isSystemRole;
  }

}

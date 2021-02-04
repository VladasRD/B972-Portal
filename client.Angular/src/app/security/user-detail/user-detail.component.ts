import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material';

import { I18n } from '@ngx-translate/i18n-polyfill';

import { AppUser, AppRole } from '../../common/appUser';
import { SecurityService } from '../security.service';

import { MessageService } from '../../common/message.service';

import { FormUtil } from '../../common/form-util';
import { GenericYesNoDialogComponent } from '../../common/generic-yes-no-dialog/generic-yes-no-dialog.component';



@Component({
  selector: 'app-user-detail',
  templateUrl: './user-detail.component.html',
  styleUrls: ['./user-detail.component.css']
})
export class UserDetailComponent implements OnInit {

  private _userId: string;
  user: AppUser;

  userRoles: AppRole[];
  otherRoles: AppRole[];

  isGroupPanelVisible = false;

  form: FormGroup;

  constructor(
    private route: ActivatedRoute,
    private securityService: SecurityService,
    private i18n: I18n,
    private router: Router,
    private messageService: MessageService,
    public dialog: MatDialog
  ) {
    this.user = new AppUser();
  }

  ngOnInit() {

    this.form = new FormGroup({
      'email': new FormControl(null, [Validators.required, Validators.email]),
      'givenName': new FormControl(null, Validators.required),
      'phoneNumber': new FormControl(null),
      'loginNT': new FormControl(null),
      'cleanPassword': new FormControl(null, Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#\$%\^&\*])(?=.{6,})/))
    });

    this.getUser();

    if (!this.isNewUser) {
      this.form.controls.email.disable();
    }
  }

  get pageTitle(): string {

    if (this.user == null) {
      return '';
    }

    if (this.isNewUser) {
      return this.i18n('New User');
    }

    return FormUtil.i18nFormat(this.i18n('User {{0}}'), this.user.userName);

  }

  autoFillGivenName() {

    const givenName = this.form.controls.givenName.value;
    if (givenName != null && givenName !== '') {
      return;
    }

    let email = this.form.get('email').value;
    if (email == null) {
      return;
    }
    const idx = email.indexOf('@');
    if (idx >= 0) {
      email = email.substring(0, idx);
    }

    const names = email.split('.');
    for (let i = 0; i < names.length; i++) {
      names[i] = names[i].charAt(0).toUpperCase() + names[i].substring(1);
    }
    this.form.get('givenName').setValue(names.join(' '));
  }

  addRoleToUser(role: AppRole) {
    this.userRoles.push(role);
    this.otherRoles.splice(this.otherRoles.indexOf(role), 1);
    this.user.userRoles.push({ roleId: role.id });
  }

  removeRoleFromUser(role: AppRole) {
    this.otherRoles.push(role);
    this.userRoles.splice(this.userRoles.indexOf(role), 1);
    this.user.userRoles.splice(this.user.userRoles.findIndex(r => r.roleId === role.id), 1);
  }

  private buildRoleList() {
    this.securityService.getRoles().subscribe(roles => {
      this.userRoles = roles.filter(r => this.user.userRoles.some(ur => ur.roleId === r.id));
      this.otherRoles = roles.filter(r => !this.user.userRoles.some(ur => ur.roleId === r.id));
    });
  }

  private getUser(): void {
    this._userId = this.route.snapshot.paramMap.get('id');
    if (this.isNewUser) {
      this.buildRoleList();
      return;
    }

    this.securityService.getUser(this._userId).subscribe(user => {
      this.user =  Object.assign(new AppUser(), user);
      this.form.patchValue(this.user);
      this.form.controls['givenName'].setValue(this.user.givenName);
      this.form.updateValueAndValidity();
      this.buildRoleList();
    });
  }

  get isNewUser(): boolean {
    return this._userId === 'new';
  }

  saveUser(formValue: any) {

    if (this.form.invalid) {
      return;
    }

    // updates the model
    FormUtil.updateModel(this.form, this.user);
    this.user.userName = this.user.email;
    this.user.givenName = formValue.givenName;

    this.securityService.saveUser(this.user)
      .subscribe(user => {
        this.router.navigate(['./security/users']);
        this.messageService.add(this.i18n('User saved.'));
      },
      err => {
        this.messageService.addError(err.message + ' (saving user)');
      });
  }

  openConfirmDeleteDialog(): void {
    const dialogRef = this.dialog.open(GenericYesNoDialogComponent, {
      width: '80%',
      data: { title: this.i18n('Remove user'), message: this.i18n('Are you sure you want to remove this user?'), isWarn: true }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result === true) {
        this.deleteUser();
      }
    });
  }

  private deleteUser() {
    this.securityService.deleteUser(this.user.id)
      .subscribe(() => {
        this.messageService.add(this.i18n('User deleted.'));
        this.router.navigate(['./security/users']);
      },
      err => {
        this.messageService.addError(err.message + ' (deleting user)');
      });
  }

  unlockUser() {
    this.securityService.unlockUser(this.user.id)
    .subscribe(data => {
      this.messageService.add(this.i18n('User unlocked.'));
      this.user.lockoutEnd = null;
    },
    err => {
      this.messageService.addError(err.message + ' (unlocking user)');
    });
  }

  lockUser() {
    this.securityService.lockUser(this.user.id)
    .subscribe(data => {
      this.messageService.add(this.i18n('User locked.'));
      this.user.lockoutEnd = new Date(9999, 11, 31, 10);
    },
    err => {
      this.messageService.addError(err.message + ' (locking user)');
    });
  }

}




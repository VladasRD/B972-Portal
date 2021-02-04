import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormControl, FormGroup, FormArray, Validators } from '@angular/forms';
import { I18n } from '@ngx-translate/i18n-polyfill';

import { SecurityService } from '../security.service';
import { MessageService } from '../../common/message.service';
import { FlatTreeControl } from '@angular/cdk/tree';

@Component({
  selector: 'app-change-password',
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.css']
})
export class ChangePasswordComponent implements OnInit {

  form: FormGroup;
  passwordChanged = false;

  constructor(
    private securityService: SecurityService,
    private router: Router,
    private i18n: I18n,
    private messageService: MessageService) {
    this.form = new FormGroup({
      'newPassword': new FormControl(null,
        [Validators.required, Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#\$%\^&\*])(?=.{6,})/)]),
      'confirmPassword': new FormControl(null,
        [Validators.required, Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#\$%\^&\*])(?=.{6,})/)]),
      'oldPassword': new FormControl(null, Validators.required),
    });
  }

  ngOnInit() {}

  changePassword(formValue: any) {

    if (this.form.invalid) {
      return;
    }

    if (formValue.newPassword !== formValue.confirmPassword) {
      this.messageService.addError('New password and Confirm password should be the same.');
      return;
    }

    this.securityService.changeUserPassword(formValue.oldPassword, formValue.newPassword)
    .subscribe(
      data => {
        this.messageService.add(this.i18n('Password changed.'));
        this.passwordChanged = true;
      },
      err => { this.messageService.addError(err.message); }
    );
  }


}

import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';

import { UserListComponent } from './user-list/user-list.component';
import { UserDetailComponent } from './user-detail/user-detail.component';

import { RoleListComponent } from './role-list/role-list.component';
import { RoleDetailComponent } from './role-detail/role-detail.component';


import { SharedModule } from '../common/shared.module';
import { ClaimsFormComponent } from './claims-form/claims-form.component';
import { LogListComponent } from './log-list/log-list.component';
import { LogDetailComponent } from './log-detail/log-detail.component';
import { ChangePasswordComponent } from './change-password/change-password.component';
import { UserPickerDialogComponent } from './user-picker-dialog/user-picker-dialog.component';
import { UserPickerComponent } from './user-picker/user-picker.component';


@NgModule({
  imports: [
    RouterModule.forChild([
      { path: 'security/users', component: UserListComponent, data: { shouldReuse: true } },
      { path: 'security/users/:id', component: UserDetailComponent },
      { path: 'security/roles', component: RoleListComponent, data: { shouldReuse: true } },
      { path: 'security/roles/:id', component: RoleDetailComponent },
      { path: 'security/logs', component: LogListComponent, data: { shouldReuse: true } },
      { path: 'security/logs/:id', component: LogDetailComponent },
      { path: 'security/change-password', component: ChangePasswordComponent }
    ]),
    SharedModule
  ],
  exports: [ RouterModule, UserPickerComponent ],
  declarations: [
    UserListComponent, UserDetailComponent,
    RoleListComponent, RoleDetailComponent,
    ClaimsFormComponent,
    LogListComponent,
    LogDetailComponent,
    ChangePasswordComponent,
    UserPickerDialogComponent,
    UserPickerComponent
  ],
  entryComponents: [ UserPickerDialogComponent ],
  providers: []
})

export class SecurityModule {}

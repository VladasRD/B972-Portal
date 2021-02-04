import { Injectable } from '@angular/core';
import { I18n } from '@ngx-translate/i18n-polyfill';


import { ModuleMenus } from '../common/moduleMenus';
import { MenuService } from '../common/menu.service';
import { AuthService } from '../common//auth.service';
import { AppLink } from '../common/appLink';
import { RoleClaim } from '../common/roleClaim';



@Injectable({
  providedIn: 'root'
})
export class SecurityMenus extends ModuleMenus {

  constructor(private i18n: I18n, authService: AuthService, menuService: MenuService) {
      super(authService, menuService);
      this.moduleName = this.i18n('SECURITY');
      this.moduleIcon = 'lock';
      this.registerLinksAndClaims();
  }

  get moduleLinks(): AppLink[] {
    return [
      { place: 'SECURITY', route: 'security/users', name: this.i18n('Manage users'), requiredRoles: ['USER.READ', 'USER.WRITE']},
      { place: 'SECURITY', route: 'security/roles', name: this.i18n('Manage groups'), requiredRoles: ['ROLE.READ', 'ROLE.WRITE']},
      { place: 'SECURITY', route: 'security/logs', name: this.i18n('Logs'), requiredRoles: ['LOG.READ']},
    ];
  }

  get moduleClaims(): RoleClaim[] {
    return [
      {
        claimValue: 'USER.READ', claimName: this.i18n('Users reader'),
        claimDescription: this.i18n('Can view users.')
      },
      {
        claimValue: 'USER.WRITE', claimName: this.i18n('Users writer'),
        claimDescription: this.i18n('Can create, edit and remove users.')
      },
      {
        claimValue: 'ROLE.READ', claimName: this.i18n('Roles reader'),
        claimDescription: this.i18n('Can view roles.')
      },
      {
        claimValue: 'ROLE.WRITE', claimName: this.i18n('Roles writer'),
        claimDescription: this.i18n('Can create, edit and remove roles.')
      },
      {
        claimValue: 'LOG.READ', claimName: this.i18n('Log reader'),
        claimDescription: this.i18n('Can view all system logs.')
      }
    ];
  }

}

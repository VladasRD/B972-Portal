import { Injectable } from '@angular/core';
import { I18n } from '@ngx-translate/i18n-polyfill';


import { ModuleMenus } from '../common/moduleMenus';
import { MenuService } from '../common/menu.service';
import { AuthService } from '../common/auth.service';
import { AppLink } from '../common/appLink';
import { RoleClaim } from '../common/roleClaim';
import { CMSService } from './cms.service';



@Injectable({
  providedIn: 'root'
})
export class CMSMenus extends ModuleMenus {

  constructor(private i18n: I18n, authService: AuthService, menuService: MenuService, private cmsService: CMSService) {
      super(authService, menuService);
      this.moduleName = this.i18n('CMS');
      this.moduleIcon = 'picture_in_picture';
      this.registerLinksAndClaims();
  }

  get moduleLinks(): AppLink[] {
    return [
      { place: 'TOP', route: 'cms/crosslinks', name: this.i18n('Crosslinks'), requiredRoles: ['CROSSLINK.WRITE']},
    ];
  }

  get moduleClaims(): RoleClaim[] {
    return [
      {
        claimValue: 'CONTENT.READ', claimName: this.i18n('Content reader'),
        claimDescription: this.i18n('Can view any content.')
      },
      {
        claimValue: 'CONTENT.WRITE', claimName: this.i18n('Content writer'),
        claimDescription: this.i18n('Can create, edit and remove any content.')
      },
      {
        claimValue: 'CMS_FILE.UPLOAD', claimName: this.i18n('File uploader'),
        claimDescription: this.i18n('Can upload any file to the Media Gallery.')
      },
      {
        claimValue: 'CMS_FILE.DELETE', claimName: this.i18n('File remover'),
        claimDescription: this.i18n('Can remove any file from the Media Gallery.')
      },
      {
        claimValue: 'CROSSLINK.WRITE', claimName: this.i18n('Crosslinks editor'),
        claimDescription: this.i18n('Can place contents at any cross-link area.')
      }
    ];
  }

}

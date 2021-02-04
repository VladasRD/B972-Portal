import { Injectable, Injector, LOCALE_ID } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import { OAuthService } from 'angular-oauth2-oidc';


import { CookieService } from 'ngx-cookie-service';

import { MenuService } from '../common/menu.service';
import { Router } from '@angular/router';
import { MessageService } from '../common/message.service';

import { I18n } from '@ngx-translate/i18n-polyfill';

import { JwksValidationHandler } from 'angular-oauth2-oidc';
import { authConfig } from '../auth.config';

import { AppUser, AppRole } from '../common/appUser';


import { BaseService } from '../common/baseService';
import { ModuleMenus } from './moduleMenus';

@Injectable({
  providedIn: 'root'
})
export class AuthService extends BaseService {


  private _isSigning = false;

  private _signedUser: AppUser;
  private _signedUserRoles: string[];

  private _claimModules = new Array<ModuleMenus>();

  public afterSignin: any;

  constructor(
    private oauthService: OAuthService,
    protected http: HttpClient,
    protected messageService: MessageService,
    private i18n: I18n,
    public menuService: MenuService,
    protected router: Router,
    private injector: Injector,
    private cookieService: CookieService) {
      super(messageService, router, http);
  }

  private _signedUserChecked = false;

  public init() {
    this.oauthService.configure(authConfig);
    this.oauthService.tokenValidationHandler = new JwksValidationHandler();
    this.oauthService.setupAutomaticSilentRefresh();
  }

  public get isUserSignedIn() {
    return this._signedUser != null;
  }

  public get hasSignedBefore() {
    return this.cookieService.get('.box.signedBefore') === 'true';
  }

  /**
   * Gets the current signed user.
   */
  public get signedUser(): AppUser {
    if (!this._signedUserChecked) {
        this.refreshSignedUser();
    }
    return this._signedUser;
  }

  /**
   * Gets the current user roles.
   */
  public get signedUserRoles(): string[] {
    if (!this._signedUserChecked) {
      this.refreshSignedUser();
    }
    return this._signedUserRoles;
  }

  /**
   * Gets the current user roles.
   */
  public signedUserIsInRole(role: string): boolean {
    return this._signedUserRoles.some(r => r === role);
  }

  /**
   * Returns TRUE if is in the middle of a sign in process.
   */
  get isSigning() {
    return this._isSigning;
  }

  /**
   * Starts a sign in process.
   */
  public signIn() {
    this._isSigning = true;

    if (!this.oauthService.discoveryDocumentLoaded) {
      this.messageService.addError(this.i18n('Could not connect to identity server'));
      this.oauthService.loadDiscoveryDocument();
      this._isSigning = false;
      return;
    }

    this.oauthService.initImplicitFlow();

  }

  public loadDocumentAndSignIn() {

    // sets aspnet culture cookie to preserve culture at the IdentityServer
    const localeId = this.injector.get(LOCALE_ID);
    this.cookieService.set( '.AspNetCore.Culture', 'c=' + localeId + '|uic=' + localeId);

    this._isSigning = true;

    this.oauthService.loadDiscoveryDocumentAndTryLogin()
      .then(_ => {
        console.log('identity server discovery ok');
        const token = this.oauthService.getAccessToken();
        if (token == null) {
          if (this.hasSignedBefore) {
            this.oauthService.initImplicitFlow();
          } else {
            this._isSigning = false;
          }
          return;
        }
        this._isSigning = false;
        this.cookieService.set('.box.signedBefore', 'true', 360);
        this.refreshSignedUser();
       })
      .catch(e => {
        this._isSigning = false;
        console.error('error discovering identity server:' + e);
      });
  }

  /**
   * Sign out the current user.
   */
  public signOut() {
    this.cookieService.delete('.box.signedBefore');
    this.oauthService.logOut();
  }

  /**
   * Set the current user roles and navigaton links.
   */
  public async refreshSignedUser() {

    this._signedUserChecked = true;

    if (!this.oauthService.hasValidIdToken()) {
      this._signedUser = null;
      return;
    }

    const claims = this.oauthService.getIdentityClaims();
    if (claims == null) {
      this._signedUser = null;
      return;
    }
    this._signedUser = AppUser.fromClaims(claims);
    let roles = claims['role'];
    if (!roles) {
      roles = [];
    }
    if (!(roles instanceof Array)) {
      roles = [roles];
    }
    this._signedUserRoles = roles;

    await this.menuService.getCMSLinks(this._signedUserRoles);
    this.menuService.defineCurrentUserLinks(this._signedUserRoles);

  }

  registerClaimModule(module: ModuleMenus) {
    this._claimModules.push(module);
  }

  getClaimModules(): Array<ModuleMenus> {
    return this._claimModules;
  }

  silentRefresh() {
    this.oauthService.oidc = true;
    this.oauthService
      .silentRefresh();
      // .then(info =>
      //   console.debug('refresh ok', info)
      // )
      // .catch(err =>
      //   console.error('refresh error', err)
      // );
  }

}

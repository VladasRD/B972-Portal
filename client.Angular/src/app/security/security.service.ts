import { Injectable, Injector, LOCALE_ID } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import { OAuthService } from 'angular-oauth2-oidc';
import { environment } from '../../environments/environment';
import { Observable, of } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';

import { CookieService } from 'ngx-cookie-service';

import { MenuService } from '../common/menu.service';
import { Router } from '@angular/router';
import { MessageService } from '../common/message.service';

import { I18n } from '@ngx-translate/i18n-polyfill';

import { AppUser, AppRole } from '../common/appUser';


import { BaseService } from '../common/baseService';


@Injectable({
  providedIn: 'root'
})
export class SecurityService extends BaseService {

  constructor(
    private oauthService: OAuthService,
    protected http: HttpClient,
    protected messageService: MessageService,
    private i18n: I18n,
    public linksService: MenuService,
    protected router: Router,
    private injector: Injector,
    private cookieService: CookieService) {
      super(messageService, router, http);
  }

  /**
   * Gets aplication users.
   */
  getUsers(filter: string, lockedOut: Boolean = null, skip = 0, top = 0,
    out: (totalCount: number) => void = null, atRoles: string[] = null, withClaims: string[] = null): Observable<AppUser[]> {
    let url = environment.API_SERVER_URL + '/users/?skip=' + skip + '&top=' + top + '&filter=' + filter + '&lockedOut=' + lockedOut;

    if (atRoles) {
      atRoles.forEach(r => {
        url = url + '&atRoles=' + r;
      });
    }

    if (withClaims) {
      withClaims.forEach(c => {
        url = url + '&withClaims=' + c;
      });
    }

    return this.getEntities<AppUser>(() => new AppUser(), url, out);
  }


  /**
   * Gets a given user.
   * @param id The user id
   */
  getUser(id): Observable<AppUser> {
    this.messageService.isLoadingData = true;
    return this.http.get<AppUser>(environment.API_SERVER_URL + '/users/' + id)
      .pipe(
        tap(() => { this.messageService.isLoadingData = false; }),
        map(u => Object.assign(new AppUser(), u)),
        catchError(this.handleErrorAndContinue('getting user', new AppUser()))
      );
  }

  /**
   * Gets users of a given role.
   */
  getUsersAtRole(role: string, skip = 0, top = 0, out: (totalCount: number) => void = null): Observable<AppUser[]> {
    const url = environment.API_SERVER_URL + '/roles/' + role + '/members?skip=' + skip + '&top=' + top;
    return this.getEntities<AppUser>(() => new AppUser(), url, out);
  }

  /**
   * Add user to a given role.
   * @param role The role
   * @param userId The user id
   */
  addUserToRole(role: string, userId: string) {
    const url = environment.API_SERVER_URL + '/roles/' + role + '/members/' + userId;
    this.messageService.isLoadingData = true;
    return this.http.put<AppUser>(url, null, this.httpOptions).pipe(
      tap(() => { this.messageService.isLoadingData = false; }),
      catchError(this.handleError())
    );
  }

  /**
   * Remove user from a given role.
   * @param role The role
   * @param userId The user id
   */
  removeUserFromRole(role: string, userId: string) {
    const url = environment.API_SERVER_URL + '/roles/' + role + '/members/' + userId;
    this.messageService.isLoadingData = true;
    return this.http.delete(url, this.httpOptions).pipe(
      tap(() => { this.messageService.isLoadingData = false; }),
      catchError(this.handleError())
    );
  }

  /**
   * Updates or create an user.
   * @param user The user
   */
  saveUser(user: AppUser): Observable<AppUser> {

    const data = JSON.stringify(user);

    let action = null;
    if (user.id) {
      action = this.http.put<AppUser>(environment.API_SERVER_URL + '/users/' + user.id, data, this.httpOptions);
    } else {
      action = this.http.post<AppUser>(environment.API_SERVER_URL + '/users/', data, this.httpOptions);
    }
    this.messageService.isLoadingData = true;
    return action.pipe(
      tap(() => { this.messageService.isLoadingData = false; }),
      catchError(this.handleError())
    );
  }

  deleteUser(id): Observable<string> {
    this.messageService.isLoadingData = true;
    return this.http.delete(environment.API_SERVER_URL + '/users/' + id, this.httpOptions).pipe(
      tap(() => { this.messageService.isLoadingData = false; }),
      catchError(this.handleError(null))
    );
  }

  /**
 * Gets application roles.
 */
  getRoles(filter: string = '', skip = 0, top = 0, out: (totalCount: number) => void = null): Observable<AppRole[]> {
    const url = environment.API_SERVER_URL + '/roles/?skip=' + skip + '&top=' + top + '&filter=' + filter;
    return this.getEntities<AppRole>(() => new AppRole(), url, out);
  }

  /**
 * Gets system roles.
 */
getSystemRoles(filter: string = '', skip = 0, top = 0, out: (totalCount: number) => void = null): Observable<AppRole[]> {
  const url = environment.API_SERVER_URL + '/roles/system/?skip=' + skip + '&top=' + top + '&filter=' + filter;
  return this.getEntities<AppRole>(() => new AppRole(), url, out);
}

  /**
 * Gets a given role.
 * @param id The role id
 */
  getRole(id): Observable<AppRole> {
    this.messageService.isLoadingData = true;
    return this.http.get<AppRole>(environment.API_SERVER_URL + '/roles/' + id)
      .pipe(
        tap(() => { this.messageService.isLoadingData = false; }),
        map(u => Object.assign(new AppRole(), u)),
        catchError(this.handleErrorAndContinue('getting group', new AppRole()))
      );
  }

  /**
 * Updates or create an role.
 * @param role The role
 */
  saveRole(role: AppRole): Observable<AppRole> {
    const data = JSON.stringify(role);

    let action = null;
    if (role.id) {
      action = this.http.put<AppRole>(environment.API_SERVER_URL + '/roles/' + role.id, data, this.httpOptions);
    } else {
      action = this.http.post<AppRole>(environment.API_SERVER_URL + '/roles/', data, this.httpOptions);
    }
    this.messageService.isLoadingData = true;
    return action.pipe(
      tap(() => { this.messageService.isLoadingData = false; }),
      catchError(this.handleError())
    );
  }

  deleteRole(id): Observable<string> {
    this.messageService.isLoadingData = true;
    return this.http.delete(environment.API_SERVER_URL + '/roles/' + id, this.httpOptions).pipe(
      tap(() => { this.messageService.isLoadingData = false; }),
      catchError(this.handleError(null))
    );
  }

  unlockUser(id: string): Observable<Object> {
    this.messageService.isLoadingData = true;
    const action = this.http.put(environment.API_SERVER_URL + '/users/_unlock/' + id , null, this.httpOptions);
    return action.pipe(
      tap(() => { this.messageService.isLoadingData = false; }),
      catchError(this.handleError())
    );
  }

  lockUser(id: string): Observable<Object> {
    this.messageService.isLoadingData = true;
    const action = this.http.put(environment.API_SERVER_URL + '/users/_lock/' + id , null, this.httpOptions);
    return action.pipe(
      tap(() => { this.messageService.isLoadingData = false; }),
      catchError(this.handleError())
    );
  }

  changeUserPassword(oldPassword: string, newPassword: string): Observable<Object> {

    const change = { oldPassword: oldPassword, newPassword: newPassword };
    const data = JSON.stringify(change);

    this.messageService.isLoadingData = true;
    const action = this.http.put(environment.API_SERVER_URL + '/profile/password/', data, this.httpOptions);
    return action.pipe(
      tap(() => { this.messageService.isLoadingData = false; }),
      catchError(this.handleError())
    );
  }

}


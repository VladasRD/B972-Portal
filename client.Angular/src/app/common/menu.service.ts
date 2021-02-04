import { Injectable } from '@angular/core';
import { AppLink } from './appLink';
import { ContentKind } from '../cms/content-kind';
import { Observable } from 'rxjs';
import { BaseService } from './baseService';
import { HttpClient, HttpHeaders, HttpResponse } from '@angular/common/http';
import { MessageService } from './message.service';
import { Router } from '@angular/router';
import { environment } from '../../environments/environment';
import { tap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class MenuService extends BaseService {

  private _userLinks: Map<string, AppLink[]>;
  private _allLinks: Array<AppLink>;

  private _cmsLinksUpdated = false;
  private _kinds: ContentKind[];

  constructor(
    protected http: HttpClient,
    protected messageService: MessageService,
    protected router: Router) {
    super(messageService, router, http);

    this._allLinks = new Array<AppLink>();
    this._userLinks = new Map<string, AppLink[]>();
    this._kinds = [];

  }

  public registerLink(link: AppLink) {
    this._allLinks.push(link);
  }

  /**
   * Gets all user links for a given place.
   * @param place the place where the links belong
   */
  public getUserLinksFor(place: string): AppLink[] {
    const links = this._userLinks.get(place);
    if (links == null) {
      return new Array<AppLink>();
    }
    return links;
  }

  /**
   * Defines avaialble user links based on user role claims.
   * @param roles The current user role claims
   */
  public defineCurrentUserLinks(roles: string[]) {

    this._userLinks = new Map<string, AppLink[]>();
    for (const l of this._allLinks) {
      if (l.requiredRoles.length === 0) {
        this.addUserLink(l);
        continue;
      }

      for (const c of roles) {
        if (this.isRoleRequiredForUserLink(l, c)) {
          this.addUserLink(l);
          break;
        }
      }
    }

  }

  /**
   * Adds a link at a given place at the user links collection.
   */
  private addUserLink(link: AppLink) {
    const links = this._userLinks.get(link.place);
    if (links != null) {
      links.push(link);
      return;
    }
    this._userLinks.set(link.place, [ link ]);
  }

  public isRoleRequiredForUserLink(link: AppLink, role: string): boolean {
    if (link.requiredRoles.length === 0) {
        return true;
    }
    for (const c of link.requiredRoles) {
      if (role === c) {
        return true;
      }
    }
    return false;
  }

  async getCMSLinks(roles: string[]) {
    if (this._cmsLinksUpdated) {
      return;
    }

    const contentWriteRole = roles.filter(r => r === 'CONTENT.WRITE');
    if  (!contentWriteRole || contentWriteRole.length === 0) {
      return;
    }

    await this.getEntities<ContentKind>(() => new ContentKind(), environment.API_SERVER_URL + '/cmskinds').toPromise().then(
      ks => {
        this._kinds = ks;
        ks.forEach(
          k => {
            const route = 'cms/contents/' + k.kind.toLocaleLowerCase();
            const oldLink = this._allLinks.filter(l => l.route === route);
            if (oldLink.length === 0) {
              this.registerLink({ place: 'TOP-CONTENT', route: 'cms/contents/' + k.kind.toLocaleLowerCase(), name: k.friendlyPluralName,
                requiredRoles: ['CONTENT.READ', 'CONTENT.WRITE']});
            }
          }
        );
        this._cmsLinksUpdated = true;
      }
    );
  }

  get cmsKinds() {
    return this._kinds;
  }

  get kindsLoaded() {
    return this._cmsLinksUpdated;
  }

}

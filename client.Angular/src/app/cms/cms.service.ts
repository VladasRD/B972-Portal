import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { Observable, of } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';
import { Router } from '@angular/router';
import { MessageService } from '../common/message.service';
import { HttpClient, HttpHeaders, HttpResponse } from '@angular/common/http';

import { BaseService } from '../common/baseService';
import { File } from 'box-material';
import { ContentHead } from './content-head';
import { PluginsService } from '../common/plugins.service';
import { ContentKind } from './content-kind';
import { MenuService } from '../common/menu.service';
import { CrossLinkArea } from './crosslink';


@Injectable({
  providedIn: 'root'
})
export class CMSService extends BaseService {

  constructor(
    protected http: HttpClient,
    protected messageService: MessageService,
    protected router: Router,
    protected menuService: MenuService,
    protected pluginsService: PluginsService) {
    super(messageService, router, http);
  }

  /**
   * Gets cms contents.
   * @param filter any text to filter the contents
   * @param skip used for pagination
   * @param top  used for pagination
   * @param the contents's location
   * @param kind contents's kind
   * @param out used for pagination
   * @param onlyPublished true to filter only published contents
   * @param orderBy the order of the content will be retrived
   * @param area used to get content from a crosslink area
   */
  getContents(filter: string = '',
    skip = 0,
    top = 0,
    location = '',
    kind: string,
    out: (totalCount: number) => void = null,
    onlyPublished = false,
    orderBy = 'Date',
    area = ''): Observable<ContentHead[]> {
    const url = environment.API_SERVER_URL + '/cmscontents/?filter=' + filter + '&location=' + location +
     '&kind=' + kind + '&skip=' + skip + '&top=' + top + '&onlyPublished=' + onlyPublished + '&orderBy=' + orderBy + '&area=' + area;
    return this.getEntities<ContentHead>(() => new ContentHead(), url, out).pipe(
      map(cs => {
        cs.forEach(c => {
          this.setDates(c);
          c.setTagsBgCss();
        });
        return cs;
      })
    );
  }

  /**
   * Gets a given content.
   * @param contentUId the content UId
   */
  getContent(contentUId: string): Observable<ContentHead> {
    this.messageService.isLoadingData = true;
    return this.http.get<ContentHead>(environment.API_SERVER_URL + '/cmscontents/' + contentUId)
      .pipe(
        tap(() => { this.messageService.isLoadingData = false; }),
        map(c => {
          c = Object.assign(new ContentHead(), c);
          this.setDates(c);
          c.setTagsBgCss();
          return c;
         }),
        catchError(this.handleErrorAndContinue('getting content', new ContentHead()))
      );
  }

  /**
   * Updates or create a content.
   * @param content The content
   */
  saveContent(content: ContentHead): Observable<ContentHead> {

    const data = JSON.stringify(content);

    let action = null;
    if (content.contentUId) {
      action = this.http.put<ContentHead>(environment.API_SERVER_URL + '/cmscontents/' + content.contentUId, data, this.httpOptions);
    } else {
      action = this.http.post<ContentHead>(environment.API_SERVER_URL + '/cmscontents/', data, this.httpOptions);
    }
    this.messageService.isLoadingData = true;
    return action.pipe(
      tap(() => { this.messageService.isLoadingData = false; }),
      catchError(this.handleError())
    );
  }

  /**
   * Loads the template form for a given kind.
   * @param templateName the template kind
   */
  loadTemplateComponentFactory(templateName: string): Promise<any> {
    return this.pluginsService.loadPlugin('./assets/plugins/capture-templates.umd.min.js', 'CaptureTemplatesModule').then(
      plugin => {
        const factories = plugin.componentFactories.filter(f => f.selector === 'lib-' + templateName + '-template');
        if (factories.length > 0) {
          return factories[0];
        } else {
          return null;
        }
      }
    );
  }

  deleteContent(id: string) {
    this.messageService.isLoadingData = true;
    return this.http.delete(environment.API_SERVER_URL + '/cmscontents/' + id, this.httpOptions).pipe(
      tap(() => { this.messageService.isLoadingData = false; }),
      catchError(this.handleError(null))
    );
  }

  private setDates(c: any): any {
    c.contentDate = new Date(c.contentDate + 'Z');
    c.createDate =  new Date(c.createDate + 'Z');
    if (c.publishAfter) {
      c.publishAfter = new Date(c.publishAfter + 'Z');
    }
    if (c.publishUntil) {
      c.publishUntil = new Date(c.publishUntil + 'Z');
    }
  }

  getKind(kind: string): ContentKind {
    return this.menuService.cmsKinds.filter(k => k.kind.toLocaleLowerCase() === kind.toLocaleLowerCase())[0];
  }

  get kindsLoaded() {
    return this.menuService.kindsLoaded;
  }

  getKindLocations(kind: string): Observable<string[]> {
    return this.http.get<string[]>(environment.API_SERVER_URL + '/cmskinds/' + kind)
    .pipe(
      tap(() => { this.messageService.isLoadingData = false; }),
      map(k => {
        const kindL = Object.assign(new ContentKind(), k);
        return kindL.locations;
      }),
      catchError(this.handleErrorAndContinue('getting kinds', [], true))
    );
  }

  getCrosslinks(): Observable<CrossLinkArea[]> {
    return this.http.get<CrossLinkArea[]>(environment.API_SERVER_URL + '/cmscrosslinks/')
    .pipe(
      tap(() => { this.messageService.isLoadingData = false; }),
      map(ls => {
        const la = [];
        ls.forEach(l => { la.push(Object.assign(new CrossLinkArea(), l)); });
        return la;
      }),
      catchError(this.handleErrorAndContinue('getting crosslinks', [], true))
    );
  }

  /**
   * Uploads one or more files to the cms.
   * @param folder the folder (virtual) the files will be uploaded
   * @param filesToUpload files to be uploaded
   */
  uploadFiles(folder: string, filesToUpload: File[]): Observable<File[]> {
    const data = new FormData();
    for (const f of filesToUpload) {
      data.append('files', f._data);
    }
    return this.http.post<File[]>(environment.API_SERVER_URL + '/CMSFiles/' + folder, data).pipe(
      tap(fs => {
        if (fs && fs.length > 0) {
          this.messageService.add('file(s) uploaded');
        }
      })
    );
  }

  getFiles(
    filter: string = '',
    skip = 0,
    top = 0,
    folder: string,
    unused: boolean,
    out: (totalCount: number) => void = null): Observable<File[]> {
    const url = environment.API_SERVER_URL + '/cmsfiles/?filter=' + filter + '&folder=' + folder +
    '&skip=' + skip + '&top=' + top + '&unused=' + unused;
    return this.getEntities<File>(() => new File(environment), url, out).pipe(
      tap(fs => fs.forEach(f => f.sent = true))
    );
  }

  deleteFile(folder: string, id: string) {
    this.messageService.isLoadingData = true;
    return this.http.delete(environment.API_SERVER_URL + '/cmsfiles/' + folder + '/' + id, this.httpOptions).pipe(
      tap(() => { this.messageService.isLoadingData = false; }),
      catchError(this.handleError(null))
    );
  }

  deleteCrosslink(area: string, id: string) {
    this.messageService.isLoadingData = true;
    return this.http.delete(environment.API_SERVER_URL + '/cmscrosslinks/' + area + '/' + id, this.httpOptions).pipe(
      tap(() => { this.messageService.isLoadingData = false; }),
      catchError(this.handleError(null))
    );
  }

  moveCrosslink(area: string, id: string, direction: number) {

    const data = JSON.stringify(direction);

    this.messageService.isLoadingData = true;
    return this.http.put<number>(environment.API_SERVER_URL + '/cmscrosslinks/' + area + '/' + id + '/changeOrder', data, this.httpOptions).pipe(
      tap(() => { this.messageService.isLoadingData = false; }),
      catchError(this.handleError())
    );
  }

  addCrosslink(area: string, id: string) {
    return this.moveCrosslink(area, id, 0);
  }

}

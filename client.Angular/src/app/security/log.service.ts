import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { Observable, of } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';
import { Router } from '@angular/router';
import { MessageService } from '../common/message.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import { BaseService } from '../common/baseService';
import { FormUtil } from '../common/form-util';
import { AppLog } from './appLog';

@Injectable({
  providedIn: 'root'
})
export class LogService extends BaseService {

  constructor(protected http: HttpClient, protected messageService: MessageService, protected router: Router) {
    super(messageService, router, http);
  }

  /**
   * Gets aplication users.
   */
  getLogs(filter: string, from: Date, to: Date, skip = 0, top = 0, out: (totalCount: number) => void = null): Observable<AppLog[]> {
    const fromStr = FormUtil.toIndependentDate(from);
    const toStr = FormUtil.toIndependentDate(to);
    const url = environment.API_SERVER_URL + '/logs/?skip=' + skip + '&top=' + top + '&filter=' + filter +
    '&fromDate=' + fromStr + '&toDate=' + toStr;
    return this.getEntities<AppLog>(() => new AppLog(), url, out).pipe(
      map(ls => {
        ls.forEach(l => { l.when = new Date(l.when + 'Z'); });
        return ls;
      }));
  }


   /**
   * Gets a given log entry.
   * @param id The user id
   */
  getLog(id): Observable<AppLog> {
    this.messageService.isLoadingData = true;
    return this.http.get<AppLog>(environment.API_SERVER_URL + '/logs/' + id)
      .pipe(
        tap(() => { this.messageService.isLoadingData = false;}),
        map(l => {
          l = Object.assign(new AppLog(), l);
          l.when = new Date(l.when + 'Z');
          return l;
        }),
        catchError(this.handleErrorAndContinue('getting log', new AppLog()))
      );
  }
}

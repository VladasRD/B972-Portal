import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { Observable } from 'rxjs';

import { GrudList } from '../../common/grud-list';
import { LogService } from '../log.service';
import { AppLog } from '../appLog';

@Component({
  selector: 'app-log-list',
  templateUrl: './log-list.component.html',
  styleUrls: ['./log-list.component.css']
})
export class LogListComponent extends GrudList<Object> implements OnInit {

  form: FormGroup;

  constructor(private logService: LogService) {
    super();
  }

  displayedColumns: string[] = [ 'when', 'signedUser', 'actionDescription', 'userIp' ];

  ngOnInit() {
    this.form =  new FormGroup({
      'from': new FormControl(),
      'to': new FormControl(),
    });
    this.form.get('from').valueChanges.subscribe(val => { this.newSearch(); });
    this.form.get('to').valueChanges.subscribe(val => { this.newSearch(); });
  }

  clearFilter() {
    this.form.get('from').setValue(null);
    this.form.get('to').setValue(null);
  }

  getResults(): Observable<AppLog[]> {
    return this.logService.getLogs(this.searchFilter$.getValue(), this.form.get('from').value, this.form.get('to').value,
      this._skip, this._pageSize, c => { this._totalCount = c; });
  }

}

import { Component, OnInit, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material';
import { ContentHead } from '../content-head';

import { CMSService } from '../cms.service';
import { GrudList } from '../../common/grud-list';
import { I18n } from '@ngx-translate/i18n-polyfill';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-content-picker-dialog',
  templateUrl: './content-picker-dialog.component.html',
  styleUrls: ['./content-picker-dialog.component.css']
})
export class ContentPickerDialogComponent extends GrudList<ContentHead> implements OnInit {

  displayedColumns: string[] = [ 'thumb', 'name', 'link' ];

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    private dialogRef: MatDialogRef<ContentPickerDialogComponent>,
    private cmsService: CMSService, private i18n: I18n, private router: Router) {
      super();
      this._pageSize = 7;
  }

  ngOnInit() {}

  getResults(): Observable<ContentHead[]> {
    return this.cmsService.getContents(this.searchFilter$.getValue(), this._skip, this._pageSize, '',
    '', c => { this._totalCount = c; }, false, 'Name');
  }

  pick(content: ContentHead) {
    this.dialogRef.close(content.contentUId);
  }

  view(content: ContentHead, event: MouseEvent) {
    event.stopPropagation();
    this.router.navigate(['/cms/contents/' + content.kind + '/' + content.contentUId]);
  }

}

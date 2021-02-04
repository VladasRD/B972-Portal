import { Component, OnInit, Input, ViewChild, ElementRef } from '@angular/core';
import { Observable } from 'rxjs';
import { GrudList } from '../../common/grud-list';
import { CMSService } from '../cms.service';
import { FormGroup, FormControl } from '@angular/forms';
import { debounceTime, distinctUntilChanged, tap, switchMap } from 'rxjs/operators';
import { I18n } from '@ngx-translate/i18n-polyfill';
import { IMediaGallery, File } from 'box-material';
import { environment } from '../../../environments/environment';
import { MessageService } from '../../common/message.service';
import { AuthService } from '../../common/auth.service';
import { MatDialog } from '@angular/material';
import { GenericYesNoDialogComponent } from '../../common/generic-yes-no-dialog/generic-yes-no-dialog.component';

@Component({
  selector: 'app-files-list',
  templateUrl: './files-list.component.html',
  styleUrls: ['./files-list.component.css']
})
export class FilesListComponent extends GrudList<File> implements OnInit, IMediaGallery {

  private _isOpened = false;
  private _callBackComponent: any;
  private _singleFile = true;

  private _canAdd = true;

  @ViewChild('fileInput') fileInput;

  isAtUploadMode = false;

  get isOpened() {
    return this._isOpened;
  }

  folder = '';
  unused = false;
  filesAddedCount = 0;

  form: FormGroup;

  constructor(
    private cmsService: CMSService,
    private authService: AuthService,
    private messageService: MessageService,
    private i18n: I18n,
    public dialog: MatDialog) {
    super(false);
    this.form =  new FormGroup({
      // 'folder': new FormControl()
    });
  }

  ngOnInit() {
  }

  open(folder: string, callBackComponent: any, singleFile: boolean, allowAdd = true) {

    this.results = [];
    this.folder = folder;
    this._callBackComponent = callBackComponent;
    this._singleFile = singleFile;
    this._canAdd = allowAdd && this.authService.signedUserIsInRole('CMS_FILE.UPLOAD');
    this._pageSize = this.calcPageSize();
    this.isAtUploadMode = false;

    this.updateResults(
      this.searchFilter$.pipe(
          debounceTime(250),
          distinctUntilChanged(),
          tap(t => {
            this.loading = true;
            this._skip = 0;
          }),
          switchMap(term => this.getResults())
      ));
      this._isOpened = true;
  }

  close() {
    this._isOpened = false;
  }

  select (file: File) {
    if (!this._callBackComponent) {
      return;
    }
    this._callBackComponent.addFile(file);
    if (this._singleFile) {
      this.close();
    } else {
      this.filesAddedCount++;
    }
  }

  private calcPageSize(): number {
    const width = document.documentElement.clientWidth - 160;
    const height = document.documentElement.clientHeight - 220;
    const tilePerLine = Math.floor((width + 24) / 174);
    const lines = Math.floor(height / 174);
    return tilePerLine * lines;
  }

  getResults(): Observable<File[]> {
    let correctedPageSize = this._pageSize;
    if (this.skip === 0 && this._canAdd) {
      correctedPageSize = correctedPageSize - 1;
    }
    return this.cmsService.getFiles(this.searchFilter$.getValue(), this._skip, correctedPageSize, this.folder,
      this.unused, c => { this._totalCount = c; });
  }

  get isAddButtonVisible() {
    if (!this._canAdd) {
      return false;
    }
    return  this._skip === 0;
  }

  showUploadDialog() {
    this.fileInput.nativeElement.click();
  }

  uploadFiles(filesToUpload: any[]) {

    // no files to upload, get out of here
    if (filesToUpload.length === 0 ) {
      return;
    }

    // sets upload mode
    if (!this.isAtUploadMode) {
      this.results.splice(0, this.results.length);
    }
    this.isAtUploadMode = true;

    // create the files
    for (const f of filesToUpload) {
      const toSend = new File(environment);
      toSend.fileName = f.name;
      toSend.folder = this.folder;
      toSend.size = f.size;
      toSend._data = f;
      this.results.push(toSend);
    }

    // upload them
    for (const f of this.results) {
      if (!f.sent && !f.isSending) {
        f.isSending = true;
        this.cmsService.uploadFiles(this.folder, [f])
        .subscribe(filesSent => {
          f.fileUId = filesSent[0].fileUId;
          f.isSending = false;
          f.sent = true;
        },
        err => {
          this.messageService.addError(err.message + ' (uploading file)');
          f.isSending = false;
        });
      }
    }

    // clean the files, so the change element can be trigger for the same file
    this.fileInput.nativeElement.value = '';
  }

  goToSearchMode() {
    this.isAtUploadMode = false;
    this.newSearch();
  }

  showConfirmDeleteDialog(file: File, event: MouseEvent): void {
    event.stopPropagation();
    const dialogRef = this.dialog.open(GenericYesNoDialogComponent, {
      width: '80%',
      data: { title: this.i18n('Remove file'),
      message: this.i18n('Remove \'' + file.fileName + '\' will affect any content that uses this media. Are you sure you want to remove this media?'), isWarn: true }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result === true) {
        this.deleteFile(file);
      }
    });
  }

  deleteFile(file: File) {
    this.cmsService.deleteFile(file.folder, file.fileUId).subscribe(
      o => {
        const idx = this.results.indexOf(file);
        this.results.splice(idx, 1);
      },
      err => {
        this.messageService.addError(err.message + ' (removing file)');
      }
    );
  }

  preview(file: File, event: MouseEvent): void {
    event.stopPropagation();
    window.open(file.url, '_blank');
  }

}

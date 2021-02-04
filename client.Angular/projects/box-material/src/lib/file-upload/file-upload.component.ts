import { Component, OnInit, ViewChild, Input, forwardRef, Injector, HostBinding, HostListener, Inject } from '@angular/core';
import { NG_VALUE_ACCESSOR, ControlValueAccessor, NgControl } from '@angular/forms';
import { MatFormFieldControl } from '@angular/material';
import { Subject } from 'rxjs';

import { File } from '../file';
import { IUploadService } from '../i-upload.service';
import { IMediaGallery } from '../i-media-gallery';

@Component({
  selector: 'lib-file-upload',
  templateUrl: './file-upload.component.html',
  styleUrls: ['./file-upload.component.css'],
  providers: [{
    provide: NG_VALUE_ACCESSOR,
    useExisting: forwardRef(() => FileUploadComponent),
    multi: true},
 {
   provide: MatFormFieldControl,
   useExisting: FileUploadComponent
 }]
})

/**
 * Upload Recative Form component.
 */
export class FileUploadComponent implements OnInit {

  private _autoUpload: boolean;
  private _singleFile: boolean;
  private _placeholder: string;

  _files: File[];

  id: string;

  ngControl: NgControl;
  focused: boolean;

  required: boolean;
  disabled: boolean;
  errorState = false;
  controlType = 'box-file-upload';
  autofilled?: boolean;

  @ViewChild('fileInput') fileInput;
  @ViewChild('fileList') fileList;

  @Input() folder: string;
  @Input() mode: string;
  @Input() uploadService: IUploadService;
  @Input() mediaGallery: IMediaGallery;
  @Input() mediaGalleryPlaceHolder: string;

  isDragging: boolean;

  stateChanges = new Subject<void>();

  constructor(public injector: Injector, @Inject('env') private environment) {
      this._files = [];
      this.autoUpload = true;
  }

  ngOnInit() {
    this.ngControl = this.injector.get(NgControl);
    if (this.ngControl != null) { this.ngControl.valueAccessor = this; }
  }

  /**
   * Gets an array of files or a single file if singleFile = true.
   **/
  get value() {
    if (this.singleFile) {
      if (this._files && this._files.length > 0) {
        return this._files[0];
      } else {
        return null;
      }
    }
    return this._files;
  }

  /**
   * Sets an array of files or a single file if singleFile = true
   */
  set value(value) {
    let valueArray = [];
    if (this.singleFile) {
      valueArray.push(value);
    } else {
      valueArray = value as any[];
    }
    for (let i = 0; i < valueArray.length; i++) {
      valueArray[i] = Object.assign(new File(this.environment), valueArray[i]);
      valueArray[i].sent = true;
    }
    this._files = valueArray;
    this.propagateChange(this.value);
  }

  @Input()
  get autoUpload() {
    return this._autoUpload;
  }
  set autoUpload(value) {
    const b = value.toString().toLowerCase().trim();
    if (b === 'false') {
      this._autoUpload = false;
    } else {
      this._autoUpload = true;
    }
  }

  @Input()
  get singleFile() {
    return this._singleFile;
  }
  set singleFile(value) {
    const b = value.toString().toLowerCase().trim();
    if (b === 'false') {
      this._singleFile = false;
    } else {
      this._singleFile = true;
    }
  }

  get isMediaGalleryLinkVisible() {
    if (!this.mediaGallery) {
      return false;
    }
    return true;
  }

  /**
   * Add files to be uploaded by the control.
   * @param filesToUpload files to be added
   */
  addFilesToUpload(filesToUpload: any[]): void {
    for (const f of filesToUpload) {
      const toSend = new File(this.environment);
      toSend.fileName = f.name;
      toSend.folder = this.folder;
      toSend.size = f.size;
      toSend._data = f;
      this._files.push(toSend);
    }
    this.propagateChange(this.value);

    if (this.autoUpload) {
      this.upload();
    }
    // clean the files, so the change element can be trigger for the same file
    this.fileInput.nativeElement.value = '';
  }

  /**
   * Removes a file from the control.
   * @param file file to be removed
   */
  removeFile(file: File): void {
    this._files.splice(this._files.indexOf(file), 1);
    this.propagateChange(this.value);
  }

  /**
   * Adds a file that has alreadu been sent to the control.
   * @param file the file
   */
  addFile(file: File): void {
    if (this.singleFile && this._files.length > 0) {
      this.removeFile(this._files[0]);
    }
    file.sent = true;
    this._files.push(file);
    this.propagateChange(this.value);
  }

  openMediaGallery(event: MouseEvent): void {
    if (!this.mediaGallery) {
      return;
    }
    this.mediaGallery.open(this.folder, this, this.singleFile, false);
    event.stopPropagation();
  }

  /**
   * Uploads all files using an upload service that implements IUploadService.
   */
  upload(): void {

    if (!this.uploadService) {
      return;
    }

    for (const f of this._files) {
      if (!f.sent && !f.isSending) {
        f.isSending = true;
        this.uploadService.uploadFiles(this.folder, [f])
        .subscribe(filesSent => {
          f.fileUId = filesSent[0].fileUId;
          f.isSending = false;
          f.sent = true;
          // this.messageService.add(this.i18n('File uploaded.'));
          this.propagateChange(this.value);
        },
        err => {
          // this.messageService.addError(err.message + ' (uploading file)');
          f.isSending = false;
        });
      }
    }
  }

  onDrop(event) {
    event.preventDefault();
    event.stopPropagation();
    this.isDragging = false;
    const files = event.dataTransfer.files;
    if (!files || files.length === 0) {
      return;
    }
    if (files.length > 1 && this.singleFile) {
      this.addFilesToUpload( [ files[0] ] );
      return;
    }
    this.addFilesToUpload(files);
  }

  onDragover(event) {
    this.isDragging = true;
    event.preventDefault();
  }

  onDragleave(event) {
    this.isDragging = false;
    event.preventDefault();
  }


  // Material Form method and props
  // https://itnext.io/creating-a-custom-form-field-control-compatible-with-reactive-forms-and-angular-material-cf195905b451
  // ----------------------------------------------------------------
  @Input()
  get placeholder() {
    return this._placeholder;
  }
  set placeholder(plh) {
    this._placeholder = plh;
    this.stateChanges.next();
  }

  get empty() {
    return this._files.length === 0;
 }

  @HostBinding('class.floating')
  get shouldLabelFloat() {
    return this.focused || !this.empty;
  }

  writeValue(obj: any): void {
    if (obj) {
      this.value = obj;
    }
  }

  propagateChange = (_: any) => {};

  registerOnChange(fn: any): void {
    this.propagateChange = fn;
  }

  registerOnTouched(fn: any): void {}
  setDisabledState?(isDisabled: boolean): void {}

  setDescribedByIds(ids: string[]): void {
    // throw new Error("Method not implemented.");
  }
  onContainerClick(event: MouseEvent): void {
    if (this.singleFile && !this.empty) {
      return;
    }
    this.fileInput.nativeElement.click();
  }

  // -----------------------------------------



}

import { Input, ViewChild } from '@angular/core';
import { FormGroup, FormControl, AbstractControl, Validators, NgForm } from '@angular/forms';
import { IUploadService, IMediaGallery } from 'box-material';


export class BaseTemplate {

    form: FormGroup;

    @Input() head: any;
    @Input() content: any;

    cmsService: IUploadService;
    mediaGallery: IMediaGallery;

    constructor() {}

    createForm() {
      this.form = new FormGroup({
        'name': new FormControl(this.head.name, [Validators.required]),
        'abstract': new FormControl(this.head.abstract),
        'externalLinkUrl': new FormControl(this.head.externalLinkUrl),

        'text1': new FormControl(this.head.customInfo ? this.head.customInfo.text1 : ''),
        'text2': new FormControl(this.head.customInfo ? this.head.customInfo.text2 : ''),
        'text3': new FormControl(this.head.customInfo ? this.head.customInfo.text3 : ''),
        'text4': new FormControl(this.head.customInfo ? this.head.customInfo.text4 : ''),

        'number1': new FormControl(this.head.customInfo ? this.head.customInfo.number1 : ''),
        'number2': new FormControl(this.head.customInfo ? this.head.customInfo.number2 : ''),
        'number3': new FormControl(this.head.customInfo ? this.head.customInfo.number3 : ''),
        'number4': new FormControl(this.head.customInfo ? this.head.customInfo.number4 : ''),

        'date1': new FormControl(this.head.customInfo ? this.head.customInfo.date1 : ''),
        'date2': new FormControl(this.head.customInfo ? this.head.customInfo.date2 : ''),
        'date3': new FormControl(this.head.customInfo ? this.head.customInfo.date3 : ''),
        'date4': new FormControl(this.head.customInfo ? this.head.customInfo.date4 : '')

      });
    }

    addField(name: string, control: AbstractControl = null) {
      if (control === null) {
        control = new FormControl(this.content[name]);
      }
      this.form.addControl(name, control);
    }

    applyValues(): boolean {

      Object.keys(this.form.controls).forEach(key => {
        this.form.get(key).markAsTouched();
      });

      if (this.form.invalid) {
        return false;
      }

      this.updateHead();
      this.updateContent();

      return true;
    }

    protected updateHead() {

      if (!this.head.customInfo) {
        this.head.customInfo = new Object();
      }

      this.head.name = this.form.get('name').value;
      this.head.abstract = this.form.get('abstract').value;
      this.head.externalLinkUrl = this.form.get('externalLinkUrl').value;

      this.head.customInfo.text1 = this.form.get('text1').value;
      this.head.customInfo.text2 = this.form.get('text2').value;
      this.head.customInfo.text3 = this.form.get('text3').value;
      this.head.customInfo.text4 = this.form.get('text4').value;

      this.head.customInfo.number1 = this.form.get('number1').value;
      this.head.customInfo.number2 = this.form.get('number2').value;
      this.head.customInfo.number3 = this.form.get('number3').value;
      this.head.customInfo.number4 = this.form.get('number4').value;

      this.head.customInfo.date1 = this.form.get('date1').value;
      this.head.customInfo.date2 = this.form.get('date2').value;
      this.head.customInfo.date3 = this.form.get('date3').value;
      this.head.customInfo.date4 = this.form.get('date4').value;

      this.setThumbUrlUsingField();
    }

    protected updateContent() {
      for (const key in this.form.controls) {
        if (!this.isHeadField(key)) {
          const formValue = this.form.get(key);
          this.content[key] = formValue ? formValue.value : null;
        }
      }
    }

    protected isHeadField(name: string) {
      return name === 'name' || name === 'abstract' || name === 'thumbFilePath' || name === 'externalLinkUrl' ||
      name === 'text1' || name === 'text2' || name === 'text3' || name === 'text4' ||
      name === 'number1' || name === 'number2' || name === 'number3' || name === 'number4' ||
      name === 'date1' || name === 'date2' || name === 'date3' || name === 'date4';
    }

    protected setThumbUrl(file: any) {
      this.head.thumbFilePath = '/files/' + file.folder + '/' + file.fileUId + '/?asThumb=true';
    }

    protected setThumbUrlUsingField(fieldName = 'mainBanner') {
      const files = this.form.get(fieldName);
      if (!files) {
        return;
      }
      if (files.value) {
        this.setThumbUrl(files.value);
      }
    }

}

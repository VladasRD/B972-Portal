import { Component, OnInit, ElementRef, ViewChild, Input, Output, EventEmitter, OnChanges, forwardRef, Inject } from '@angular/core';
import Quill from 'quill';
import { NG_VALUE_ACCESSOR, ControlValueAccessor } from '@angular/forms';

import { File } from '../file';
import { IUploadService } from '../i-upload.service';
import { IMediaGallery } from '../i-media-gallery';


@Component({
  selector: 'lib-quill-editor',
  templateUrl: './quill-editor.component.html',
  styleUrls: ['./quill-editor.component.css'],
  providers: [{
    provide: NG_VALUE_ACCESSOR,
    useExisting: forwardRef(() => QuillEditorComponent),
    multi: true
 }]
})
export class QuillEditorComponent implements OnInit, OnChanges, ControlValueAccessor {

  @ViewChild('container', { read: ElementRef }) containerRef: ElementRef;
  @ViewChild('htmlView') htmlView;

  @Input() value: any;
  @Input() label: string;
  @Input() mediaGallery: IMediaGallery;
  @Input() imageFolder: string;

  @Output() changed: EventEmitter<any> = new EventEmitter();

  quill: any = Quill;
  editor: any;
  container: any;

  editorCssClass = 'show-editor';

  constructor(
    public elementRef: ElementRef,
     @Inject('env') private environment) {
  }

  ngOnInit() {

    this.container = this.containerRef.nativeElement;

    const el = this.container.querySelector('#editor');
    this.editor = new Quill(el, {
      modules: { toolbar: '#toolbar-container' },
      theme: 'snow'
    });

    const toolbar = this.editor.getModule('toolbar');

    // image custom handler
    toolbar.addHandler('image', () => {
        if (!this.imageFolder) {
          this.imageFolder = 'images';
        }
        this.mediaGallery.open(this.imageFolder, this, true, true);
    });

    // html custom handler
    toolbar.addHandler('code-block', () => { this.editHtml(); });

    this.editor.on('editor-change', (eventName, ...args) => {
      // this.onChange(this.editor.getContents());
      this.onChange(this.htmlContents);
    });
  }

  get htmlContents() {
    return this.editor.root.innerHTML;
  }
  set htmlContents(value: string) {
    this.editor.root.innerHTML = value;
  }

  ngOnChanges(): void {
    if (this.editor) {
      this.editor.setContents(this.value);
    }
  }

  onChange = (delta: any) => {};

  onTouched = () => {};

  writeValue(value: any): void {
    // this.editor.setContents(delta);
    this.htmlContents = value;
  }

  registerOnChange(fn: (v: any) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
   this.onTouched = fn;
  }

  addFile(file: File): void {
    const range = this.editor.getSelection(true);
    this.editor.enable(true);
    this.editor.editor.insertEmbed(range.index, 'image', file.url);
    // this.quill.setSelection(range.index + 1, Quill.sources.SILENT);
    this.editor.setSelection(range.index + 1);
  }

  editHtml() {
    const html = this.htmlView.nativeElement;
    
    if (html.style.display === 'none') {
      html.value = this.editor.root.innerHTML;
      html.style.display = 'block';
      this.editorCssClass = 'hide-editor';
    } else {
      this.editor.root.innerHTML = html.value;
      html.style.display = 'none';
      this.editorCssClass = 'show-editor';
    }
  }

}


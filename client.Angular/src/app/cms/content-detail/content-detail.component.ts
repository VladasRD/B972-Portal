import { Component, OnInit, AfterViewInit, ViewChild, ViewContainerRef, ElementRef } from '@angular/core';
import { COMMA, ENTER } from '@angular/cdk/keycodes';
import { CMSService } from '../cms.service';
import { ActivatedRoute, Router } from '@angular/router';
import { I18n } from '@ngx-translate/i18n-polyfill';
import { MessageService } from '../../common/message.service';
import { MatDialog, MatChipInputEvent, MatAutocompleteSelectedEvent, MatAutocomplete, MatButtonToggleChange } from '@angular/material';
import { GenericYesNoDialogComponent } from '../../common/generic-yes-no-dialog/generic-yes-no-dialog.component';
import { ContentHead, ContentData, ContentTag } from '../content-head';
import { ICaptureTemplate } from '../i-capture-template';
import { ContentKind } from '../content-kind';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { FilesListComponent } from '../files-list/files-list.component';
import { Observable } from 'rxjs';
import { startWith, map } from 'rxjs/operators';
import { CrossLinkArea, CrossLink } from '../crosslink';


@Component({
  selector: 'app-content-detail',
  templateUrl: './content-detail.component.html',
  styleUrls: ['./content-detail.component.css']
})
export class ContentDetailComponent implements OnInit, AfterViewInit {

  form: FormGroup;
  kind: ContentKind;

  private _kind: string;
  private _contentUId: string;
  private _locationIdx: string;
  private _selectedPanel: string;
  private _crosslinkAreas: CrossLinkArea[];

  private _contentLoaded: boolean;

  @ViewChild('kindCaptureForm', {read: ViewContainerRef}) vc: ViewContainerRef;
  @ViewChild('fileList') fileList: FilesListComponent;

  @ViewChild('tagInput') tagInput: ElementRef<HTMLInputElement>;
  @ViewChild('tagsAutoComplete') tagsAutoComplete: MatAutocomplete;

  HEAD: ContentHead;
  CONTENT: any;

  filteredTags: Observable<string[]>;
  tagsCtrl: FormControl;
  tagSeparatorKeysCodes: number[] = [ENTER, COMMA];

  contentForm: ICaptureTemplate;

  constructor(
    public cmsService: CMSService,
    private route: ActivatedRoute,
    private i18n: I18n,
    private router: Router,
    private messageService: MessageService,
    public dialog: MatDialog) {

      this._selectedPanel = null;

      this.contentForm = null;
      this.HEAD = new ContentHead();
      this.CONTENT = { };
      this._crosslinkAreas = [];

      this.form =  new FormGroup({
        'changeLocation' : new FormControl(),
        'displayOrder' : new FormControl(1, [Validators.required, Validators.pattern(/^-?(0|[1-9]\d*)?$/)]),
        'contentDate' : new FormControl(),
        'publishAfter' : new FormControl(),
        'publishUntil' : new FormControl(),
        'tags': new FormControl()
      });

      this.tagsCtrl = this.form.get('tags') as FormControl;

      this.filteredTags = this.tagsCtrl.valueChanges.pipe(
        startWith(null),
        map((tag: string | null) => tag ? this._filter(tag) : this.allTags.slice()));
  }

  get crosslinksAreas() {
    return this._crosslinkAreas;
  }

  get selectedPanel() {
    return this._selectedPanel;
  }

  get locations() {
    if (!this.kind) {
      return [];
    }
    return this.kind.locations;
  }

  get currentLocation() {
    return this.form.get('changeLocation').value;
  }

  ngOnInit() {
    this._contentUId = this.route.snapshot.paramMap.get('id');
    this._kind = this.route.snapshot.paramMap.get('kind');
    this._locationIdx = this.route.snapshot.paramMap.get('locationIdx');
  }


  private getContent() {

    if (!this.cmsService.kindsLoaded) {
      setTimeout(() => { this.getContent(); }, 100);
      return;
    }

    this.kind = this.cmsService.getKind(this._kind);

    if (this.isNewContent) {
      this.HEAD = new ContentHead();
      this.HEAD.kind = this._kind;
      this.HEAD.location = this.kind.locations[this._locationIdx];
      this.HEAD.displayOrder = 1;
      this.HEAD.contentDate = new Date();
      this.bindHeadToForm();
      this.loadFormTemplate();
      this._contentLoaded = true;
      return;
    }

    this.cmsService.getContent(this._contentUId).subscribe(
      c => {
        this.HEAD = c;
        this.CONTENT = JSON.parse(c.data.json);
        if (this.CONTENT === null) {
          this.CONTENT = {};
        }

        this.bindHeadToForm();
        this.loadFormTemplate();
        this._contentLoaded = true;
      }
    );
  }

  private bindHeadToForm() {
    this.form.get('changeLocation').setValue(this.HEAD.location);
    this.form.get('displayOrder').setValue(this.HEAD.displayOrder);
    this.form.get('contentDate').setValue(this.HEAD.contentDate);
    this.form.get('publishAfter').setValue(this.HEAD.publishAfter);
    this.form.get('publishUntil').setValue(this.HEAD.publishUntil);
  }

  ngAfterViewInit() {
      this.getContent();
  }

  private loadFormTemplate() {
    this.cmsService.loadTemplateComponentFactory(this._kind).then(
      factory => {
        if (factory) {
          this.contentForm = this.vc.createComponent(factory).instance as ICaptureTemplate;
          this.contentForm.cmsService = this.cmsService;
          this.contentForm.mediaGallery = this.fileList;
          this.contentForm.head = this.HEAD;
          this.contentForm.content = this.CONTENT;
        } else {
          this.messageService.addError('Error loading ' + this._kind + ' capture template.');
        }
      }
    );
  }

  get isNewContent(): boolean {
    return this._contentUId === 'new';
  }

  get pageTitle(): string {
    if (this.isNewContent) {
      return this.i18n('New Content');
    }
    return this.HEAD.name;
  }

  get backLink(): string {
    return '/cms/contents/' + this.HEAD.kind;
  }

  onSubmit() {

    if (this.form.invalid) {
      this.messageService.addError(this.i18n('Check content\'s priority and publications options.'));
      return;
    }

    const formOk = this.contentForm.applyValues();
    if (!formOk) {
      return;
    }

    this.HEAD.data = new ContentData();
    this.HEAD.data.json = JSON.stringify(this.CONTENT);

    this.HEAD.location = this.form.get('changeLocation').value;
    this.HEAD.displayOrder = this.form.get('displayOrder').value;
    this.HEAD.contentDate = this.form.get('contentDate').value;
    this.HEAD.publishAfter = this.form.get('publishAfter').value;
    this.HEAD.publishUntil = this.form.get('publishUntil').value;

    this.cmsService.saveContent(this.HEAD).subscribe(
      content => {
        this.router.navigate(['./cms/contents/' + this.HEAD.kind]);
        this.messageService.add(this.i18n('Content saved.'));
      },
      err => {
        this.messageService.addError(err.message + ' (saving content)');
      }
    );
  }

  showPanel(panelName: string) {
    this._selectedPanel = panelName;
    if (panelName === 'crosslinks') {
      this.loadCrossLinks();
    }
    // window.document.getElementById('right-panel').style.right = '0px';
    // if (document.documentElement.clientWidth < 600) {
    //   window.document.getElementById('right-panel').style.left = '0px';
    // }
  }

  closePanel() {
    // window.document.getElementById('right-panel').style.right = '-300px';
    // window.document.getElementById('right-panel').style.left = 'auto';
    this._selectedPanel = null;
  }

  loadCrossLinks() {
    if (this._crosslinkAreas.length !== 0) {
      return;
    }
    this.cmsService.getCrosslinks().subscribe(
      ls => { this._crosslinkAreas = ls; },
      err => {
        this.messageService.addError(err.message + ' (loading crosslinks)');
      }
    );
  }

  openConfirmDeleteDialog(): void {
    const dialogRef = this.dialog.open(GenericYesNoDialogComponent, {
      width: '80%',
      data: { title: this.i18n('Remove content'), message: this.i18n('Are you sure you want to remove this content?'), isWarn: true }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result === true) {
        this.deleteContent();
      }
    });
  }

  private deleteContent() {
    this.cmsService.deleteContent(this.HEAD.contentUId)
      .subscribe(() => {
        this.messageService.add(this.i18n('Content deleted.'));
        this.router.navigate([this.backLink]);
      },
      err => {
        this.messageService.addError(err.message + ' (deleting content)');
      });
  }

  addTag(event: MatChipInputEvent): void {
    if (!this.tagsAutoComplete.isOpen) {
      const input = event.input;
      const value = event.value;

      // Add a tag
      if ((value || '').trim()) {
        const tag = new ContentTag();
        tag.tag = value.trim();
        tag.contentUId = this.HEAD.contentUId;
        tag.setTagBgCss();
        this.HEAD.tags.push(tag);
      }

      // Reset the input value
      if (input) {
        input.value = '';
      }

      this.tagsCtrl.setValue(null);
    }
  }

  removeTag(tag: ContentTag): void {
    const index = this.HEAD.tags.indexOf(tag);

    if (index >= 0) {
      this.HEAD.tags.splice(index, 1);
    }
  }

  isTagSelected(event: MatAutocompleteSelectedEvent): void {
    const tag = new ContentTag();
    tag.tag = event.option.viewValue;
    tag.contentUId = this.HEAD.contentUId;
    tag.setTagBgCss();
    this.HEAD.tags.push(tag);
    this.tagInput.nativeElement.value = '';
    this.tagsCtrl.setValue(null);
  }

  private get allTags() {
    if (!this.kind) {
      return [];
    }
    return this.kind.tags;
  }

  private _filter(value: string): string[] {
    const filterValue = value.toLowerCase();
    return this.kind.tags.filter(t => t.toLowerCase().indexOf(filterValue) === 0);
  }

  get contentTags(): ContentTag[] {
    if (!this.HEAD) {
      return [];
    }
    return this.HEAD.tags;
  }

  addRemoveCrosslink(event: MatButtonToggleChange) {
    if (this.HEAD.crossLinks == null) {
      this.HEAD.crossLinks = [];
    }
    const area = event.value;
    const oldCrosslink = this.HEAD.crossLinks.find(c => c.pageArea === area);
    if (oldCrosslink) {
      const idx = this.HEAD.crossLinks.indexOf(oldCrosslink);
      this.HEAD.crossLinks.splice(idx, 1);
    } else {
      const crosslink = new CrossLink();
      crosslink.pageArea = area;
      crosslink.contentUId = this.HEAD.contentUId;
      this.HEAD.crossLinks.push(crosslink);
    }
  }

  isAtCrosslink(area: string): boolean {
    if (this.HEAD.crossLinks == null) {
      return false;
    }
    return this.HEAD.crossLinks.filter(c => c.pageArea === area).length > 0;
  }

  get isPublished(): boolean {
    if (!this._contentLoaded) {
      return true;
    }
    return this.HEAD.isPublished;
  }

  unpublish() {
    this.HEAD.publishAfter = null;
    this.HEAD.publishUntil = null;
    this.bindHeadToForm();
  }

  saveAndPublish() {
    const now = new Date();
    if (!this.HEAD.publishAfter || this.HEAD.publishAfter > now) {
      now.setMinutes(0);
      this.form.get('publishAfter').setValue(now);
    }
    this.onSubmit();
  }


}

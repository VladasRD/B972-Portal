import { Observable } from 'rxjs';
import { CMSService } from './../cms.service';
import { ContentHead } from '../content-head';
import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { FormGroup, FormControl } from '@angular/forms';
import { GrudList } from '../../common/grud-list';

import { ActivatedRoute } from '@angular/router';
import { ContentKind } from '../content-kind';
import { I18n } from '@ngx-translate/i18n-polyfill';


@Component({
  selector: 'app-content-list',
  templateUrl: './content-list.component.html',
  styleUrls: ['./content-list.component.css']
})
export class ContentListComponent extends GrudList<ContentHead> implements OnInit {

  private _resetingForm = false;
  private _kind: string;

  form: FormGroup;

  kind: ContentKind;

  orderByLabel: string;
  orderByValue: string;

  @ViewChild('searchBox') searchBox: ElementRef;

  constructor(private cmsService: CMSService, private i18n: I18n, private route: ActivatedRoute) {
    super();

    this._kind = this.route.snapshot.params.kind;

    this.form =  new FormGroup({
      'onlyPublishFilter': new FormControl('all'),
      'location': new FormControl(),
      'location-dropdown': new FormControl()
    });

    this.form.get('onlyPublishFilter').valueChanges.subscribe(val => { this.refresh(); });
    this.form.get('location').valueChanges.subscribe(val => { this.refresh(); });
    this.form.get('location-dropdown').valueChanges.subscribe(val => { this.form.get('location').setValue(val); });

    this.kind = new ContentKind();
    this.kind.locations = [];
    this.kind.friendlyPluralName = '';

    this.resetForm();

    // if the route has changed (changed the kind), refresh the page
    this.route.params.subscribe(params => { this.refreshOnRouteChange(params.kind); });
  }

  orderBy(event: MouseEvent, order: string) {
    this.orderByLabel = event.srcElement['innerText'];
    this.orderByValue = order;
    this.refresh();
  }

  get onlyPublished() {
    return this.form.get('onlyPublishFilter').value === 'published';
  }

  refreshOnRouteChange(newKind: string) {
    if (newKind === this._kind) {
      return;
    }
    this._kind = newKind;
    this.resetForm();
    this.newSearch();
  }

  refresh() {
    if (this._resetingForm) {
      return;
    }
    this.newSearch();
  }

  resetForm() {
    this._resetingForm = true;
    this.form.get('onlyPublishFilter').setValue('all');
    if (!this.cmsService.kindsLoaded) {
      setTimeout(() => { this.resetForm(); }, 100);
      return;
    }
    this.kind = this.cmsService.getKind(this._kind);
    if (this.kind.locations.length > 0 ) {
      this.form.get('location-dropdown').setValue(this.kind.locations[0]);
    }
    if (this.searchBox) {
      this.searchBox.nativeElement.value = '';
    }

    this.orderByLabel = this.i18n('orderby_date');
    this.orderByValue = 'Date';

    this._resetingForm = false;
    this.updateLocations();
  }

  get selectedLocation() {
    return this.form.get('location').value;
  }
  get selectedLocationIdx() {
    if (this.kind === null) {
      return 0;
    }
    return this.kind.locations.indexOf(this.selectedLocation);
  }

  ngOnInit() {
  }

  getResults(): Observable<ContentHead[]> {
    return this.cmsService.getContents(this.searchFilter$.getValue(), this._skip, this._pageSize, this.selectedLocation,
    this._kind, c => { this._totalCount = c; }, this.onlyPublished, this.orderByValue);
  }

  updateLocations() {
    this.cmsService.getKindLocations(this._kind)
    .subscribe(
      ls => {
        this.kind.locations = ls;
      }
    );
  }

  get isLocationDropDownVisible() {
    let size = 0;
    for (let i = 0; i < this.kind.locations.length; i++) {
      size = size + this.kind.locations[i].length;
    }
    return size > 120;
  }

}

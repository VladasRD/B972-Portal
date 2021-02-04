import { Component, OnInit, Input } from '@angular/core';
import { FormGroup, FormControl } from '@angular/forms';
import { BaseTemplate } from './base-template';

@Component({
  selector: 'lib-page-template',
  templateUrl: './page-template.component.html' ,
  styles: []
})
export class PageTemplateComponent extends BaseTemplate implements OnInit {

  constructor() {
    super();
  }

  ngOnInit() {
    this.createForm();
    this.addField('field1');
    this.addField('field2');
    this.addField('mainBanner');
    this.addField('images');
    this.addField('htmlContent');
  }


}

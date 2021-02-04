import { Component, OnInit } from '@angular/core';
import { CMSService } from '../cms.service';
import { I18n } from '@ngx-translate/i18n-polyfill';
import { ActivatedRoute } from '@angular/router';
import { CrossLinkArea } from '../crosslink';
import { MessageService } from '../../common/message.service';

@Component({
  selector: 'app-crosslink-list',
  templateUrl: './crosslink-list.component.html',
  styleUrls: ['./crosslink-list.component.css']
})
export class CrosslinkListComponent implements OnInit {

  public crosslinkAreas: CrossLinkArea[];

  constructor(
    private cmsService: CMSService,
    private i18n: I18n,
    private messageService: MessageService,
    private route: ActivatedRoute) {
    this.loadCrossLinks();
  }

  ngOnInit() {
  }

  loadCrossLinks() {
    this.cmsService.getCrosslinks().subscribe(
      ls => { this.crosslinkAreas = ls; },
      err => {
        this.messageService.addError(err.message + ' (loading crosslinks)');
      }
    );
  }

}

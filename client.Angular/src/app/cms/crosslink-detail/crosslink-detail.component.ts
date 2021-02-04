import { Component, OnInit, Input } from '@angular/core';
import { CrossLink, CrossLinkArea } from '../crosslink';
import { CMSService } from '../cms.service';
import { ContentHead } from '../content-head';
import { MessageService } from '../../common/message.service';
import { ContentPickerDialogComponent } from '../content-picker-dialog/content-picker-dialog.component';
import { MatDialog } from '@angular/material';

@Component({
  selector: 'app-crosslink-detail',
  templateUrl: './crosslink-detail.component.html',
  styleUrls: ['./crosslink-detail.component.css']
})
export class CrosslinkDetailComponent implements OnInit {

  constructor(private cmsService: CMSService, protected messageService: MessageService, private dialog: MatDialog) { }

  @Input() area: CrossLinkArea;

  contents: ContentHead[];

  ngOnInit() {
    this.loadCrosslinks();
  }

  loadCrosslinks() {
    this.cmsService.getContents(null, 0, 0, null, null, null, false, 'CrossLinkDisplayOrder', this.area.area)
    .subscribe(cs => { this.contents = cs; },
    err => {
      this.messageService.addError(err.message + ' (loading crosslinks)');
    });
  }

  remove(head: ContentHead, event: MouseEvent) {
    event.stopPropagation();
    this.cmsService.deleteCrosslink(this.area.area, head.contentUId).subscribe(
      o => {
        const idx = this.contents.indexOf(head);
        this.contents.splice(idx, 1);
      },
      err => {
        this.messageService.addError(err.message + ' (removing crosslink)');
      }
    );
  }

  move(head: ContentHead, direction: number, event: MouseEvent) {
    event.stopPropagation();
    this.cmsService.moveCrosslink(this.area.area, head.contentUId, direction).subscribe(
      o => { this.loadCrosslinks(); },
      err => {
        this.messageService.addError(err.message + ' (moving crosslink)');
      }
    );
  }

  canMoveUp(head: ContentHead) {
    return this.contents.indexOf(head) > 0;
  }

  canMoveDown(head: ContentHead) {
    return this.contents.indexOf(head) < this.contents.length - 1;
  }

  pickContent() {
    const dialogRef = this.dialog.open(ContentPickerDialogComponent, { width: '640px', height: '520px'});
    dialogRef.afterClosed().subscribe(
      contentUId => {
        if (contentUId) {
          this.cmsService.addCrosslink(this.area.area, contentUId).subscribe(
            o => { this.loadCrosslinks(); },
            err => {
              this.messageService.addError(err.message + ' (adding crosslink)');
            }
          );
        }
      });
  }

}

import { Component, OnInit, Inject } from '@angular/core';
import {MAT_DIALOG_DATA} from '@angular/material';

@Component({
  selector: 'app-generic-yes-no-dialog',
  templateUrl: './generic-yes-no-dialog.component.html',
  styleUrls: ['./generic-yes-no-dialog.component.css']
})
export class GenericYesNoDialogComponent implements OnInit {

  constructor(@Inject(MAT_DIALOG_DATA) public data: any) { }

  public get isWarn() {
    return this.data.isWarn;
  }

  ngOnInit() {
  }

  get closeReturn() {
    if (!this.data.hasTextMessage) {
      return true;
    } else {
      return { result: true, textMessage: this.data.textMessage};
    }
  }

}

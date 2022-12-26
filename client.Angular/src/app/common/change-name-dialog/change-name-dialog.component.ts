import { Component, OnInit, Inject } from '@angular/core';
import {MAT_DIALOG_DATA} from '@angular/material';

@Component({
  selector: 'app-change-name-dialog',
  templateUrl: './change-name-dialog.component.html',
  styleUrls: ['./change-name-dialog.component.css']
})
export class ChangeNameDialogComponent implements OnInit {

  constructor(@Inject(MAT_DIALOG_DATA) public data: any) { }

  public get isWarn() {
    return this.data.isWarn;
  }

  ngOnInit() {
  }

  get closeReturn() {
    return { result: true, textMessage: this.data.textMessage};
  }

}

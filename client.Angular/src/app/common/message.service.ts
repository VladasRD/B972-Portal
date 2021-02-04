import { Injectable } from '@angular/core';
import {MatSnackBar} from '@angular/material';

@Injectable({
  providedIn: 'root',
})
export class MessageService {

  messages: string[] = [];

  private _isLoadingData = false;
  private _blockUI = false;

  constructor(public snackBar: MatSnackBar) {}

  get isUIBlocked() {
    return this._blockUI === true;
  }
  blockUI(): void {
    // this._isLoadingData = value;
    // thi sto fix: ExpressionChangedAfterItHasBeenCheckedError
    Promise.resolve(null).then(() => this._blockUI = true);
  }

  get isLoadingData() {
    return this._isLoadingData;
  }
  set isLoadingData(value: boolean) {
    // this._isLoadingData = value;
    // thi sto fix: ExpressionChangedAfterItHasBeenCheckedError
    Promise.resolve(null).then(() => {
      this._isLoadingData = value;
      if (this._isLoadingData === false) { this._blockUI = false; }
    });
  }

  add(message: string) {
    this.messages.push(message);
    this.snackBar.open(message, 'ok', { duration: 4000, panelClass: ['normalMessage'] });
  }

  addError(message: string) {
    this.messages.push(message);
    this.snackBar.open(message, 'ok', { panelClass: ['errorMessage'] }, );
  }

  clear() {
    this.messages = [];
  }
}

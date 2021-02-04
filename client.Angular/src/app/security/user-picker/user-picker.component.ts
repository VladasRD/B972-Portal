import { Component, OnInit, Injector, Input, HostBinding, forwardRef } from '@angular/core';
import { AppUser } from '../../common/appUser';
import { NgControl, NG_VALUE_ACCESSOR } from '@angular/forms';
import { Subject } from 'rxjs';
import { MatFormFieldControl, MatDialog } from '@angular/material';
import { UserPickerDialogComponent } from '../user-picker-dialog/user-picker-dialog.component';

@Component({
  selector: 'app-user-picker',
  templateUrl: './user-picker.component.html',
  styleUrls: ['./user-picker.component.css'],
  providers: [{
    provide: NG_VALUE_ACCESSOR,
    useExisting: forwardRef(() => UserPickerComponent),
    multi: true},
 {
   provide: MatFormFieldControl,
   useExisting: UserPickerComponent
 }]
})
export class UserPickerComponent implements OnInit {

  private _placeholder: string;
  _users: AppUser[];

  ngControl: NgControl;
  focused: boolean;

  required: boolean;
  disabled: boolean;
  errorState = false;
  controlType = 'box-user-picker';
  autofilled?: boolean;
  stateChanges = new Subject<void>();

  @Input() singleUser = true;
  @Input() singleUserPicker = true;
  @Input() atRoles: string[] = null;
  @Input() withClaims: string[] = null;

  constructor(public injector: Injector, private dialog: MatDialog) {
    this._users = [];
  }

  ngOnInit() {
    this.ngControl = this.injector.get(NgControl);
    if (this.ngControl != null) { this.ngControl.valueAccessor = this; }
  }

   /**
   * Gets an array of users or a single file if singleUser = true.
   **/
  get value() {
    if (this.singleUser) {
      if (this._users && this._users.length > 0) {
        return this._users[0];
      } else {
        return null;
      }
    }
    return this._users;
  }

   /**
   * Sets an array of users or a single user if singleUser = true.
   */
  set value(value) {
    let valueArray = [];
    if (this.singleUser) {
      valueArray.push(value);
    } else {
      valueArray = value as any[];
    }
    for (let i = 0; i < valueArray.length; i++) {
      valueArray[i] = Object.assign(new AppUser(), valueArray[i]);
      valueArray[i].sent = true;
    }
    this._users = valueArray;
    this.propagateChange(this.value);
  }

  removeUser(user: AppUser, event: MouseEvent) {
    if (event) {
      event.stopPropagation();
    }
    const idx = this._users.indexOf(user);
    if (idx < 0) {
      return;
    }
    this._users.splice(idx, 1);
    this.propagateChange(this.value);
  }

  pickUser(event: MouseEvent = null) {
    if (event) {
      event.stopPropagation();
    }
    if (this.singleUser && !this.empty) {
      return;
    }
    const dialogRef = this.dialog.open(UserPickerDialogComponent, UserPickerDialogComponent.config(!this.singleUserPicker, this.atRoles, this.withClaims));
    dialogRef.afterClosed().subscribe(
      users => {
        if (!users) {
          return;
        }
        users.forEach(u => {
          const alreadyTherUser = this._users.find(ux => ux.id === u.id);
          if (!alreadyTherUser) {
            this._users.push(u);
          }
        });
      });
      this.propagateChange(this.value);
  }

  get canAddUser(): boolean {
    if (!this.empty && this.singleUser) {
      return false;
    }
    return true;
  }

  // Material Form method and props
  // https://itnext.io/creating-a-custom-form-field-control-compatible-with-reactive-forms-and-angular-material-cf195905b451
  // ----------------------------------------------------------------
  @Input()
  get placeholder() {
    return this._placeholder;
  }
  set placeholder(plh) {
    this._placeholder = plh;
    this.stateChanges.next();
  }

  get empty() {
    return this._users.length === 0;
 }

  @HostBinding('class.floating')
  get shouldLabelFloat() {
    return this.focused || !this.empty;
  }

  writeValue(obj: any): void {
    if (obj) {
      this.value = obj;
    }
  }

  propagateChange = (_: any) => {};

  registerOnChange(fn: any): void {
    this.propagateChange = fn;
  }

  registerOnTouched(fn: any): void {}
  setDisabledState?(isDisabled: boolean): void {}

  setDescribedByIds(ids: string[]): void {
    // throw new Error("Method not implemented.");
  }

  onContainerClick(event: MouseEvent): void {
    if (this.empty) {
      this.pickUser();
    }
  }

  // -----------------------------------------


}

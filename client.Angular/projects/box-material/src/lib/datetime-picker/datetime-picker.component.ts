import { Component, OnInit, Input, forwardRef, Injector, HostBinding, ElementRef } from '@angular/core';
import { MatFormFieldControl } from '@angular/material';
import { NG_VALUE_ACCESSOR, NgControl } from '@angular/forms';
import { Subject } from 'rxjs';
import { FocusMonitor } from '@angular/cdk/a11y';

@Component({
  selector: 'lib-datetime-picker',
  templateUrl: './datetime-picker.component.html',
  styleUrls: ['./datetime-picker.component.css'],
  providers: [{
    provide: NG_VALUE_ACCESSOR,
    useExisting: forwardRef(() => DatetimePickerComponent),
    multi: true},
 {
   provide: MatFormFieldControl,
   useExisting: DatetimePickerComponent
 }]
})
export class DatetimePickerComponent implements OnInit {

  private _placeholder: string;

  private _date: Date;

  ngControl: NgControl;
  focused: boolean;

  required: boolean;
  disabled: boolean;
  errorState = false;
  controlType = 'box-datetime-picker';
  autofilled?: boolean;

  stateChanges = new Subject<void>();

  hours: string[];
  minutes: string[];

  constructor(public injector: Injector, private fm: FocusMonitor, private elRef: ElementRef<HTMLElement>) {
    this.createHours();
    this.createMinutes();
    fm.monitor(elRef.nativeElement, true).subscribe(origin => {
      this.focused = !!origin;
      this.stateChanges.next();
    });
  }

  ngOnInit() {
    this.ngControl = this.injector.get(NgControl);
    if (this.ngControl != null) { this.ngControl.valueAccessor = this; }
  }

  private createHours(): void {
    this.hours = [];
    for (let i = 0; i < 24; i++) {
      let h = i.toString();
      if (i < 10) {
        h = '0' + h;
      }
      this.hours.push(h);
    }
  }

  private createMinutes(): void {
    this.minutes = [];
    for (let i = 0; i < 60; i++) {
      let m = i.toString();
      if (i < 10) {
        m = '0' + m;
      }
      this.minutes.push(m);
    }
  }

  get value() {
    return this._date;
  }
  set value(value) {
    this._date = value;
    this.propagateChange(this.value);
  }

  get hour() {
    if (!this._date) {
      return '00';
    }
    const h = this._date.getHours();
    if (h < 10) {
      return '0' + h;
    }
    return h.toString();
  }
  set hour(value: string) {
    if (!this._date) {
      return;
    }
    this._date.setHours(parseInt(value, 10));
    this.propagateChange(this.value);
  }

  get minute() {
    if (!this._date) {
      return '00';
    }
    const m = this._date.getMinutes();
    if (m < 10) {
      return '0' + m;
    }
    return m.toString();
  }
  set minute(value: string) {
    if (!this._date) {
      return;
    }
    this._date.setMinutes(parseInt(value, 10));
    this.propagateChange(this.value);
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
    return !this._date;
 }

  @HostBinding('class.floating')
  get shouldLabelFloat() {
    return this.focused || !this.empty;
  }

  writeValue(obj: any): void {
    this.value = obj;
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
    // if (this.singleFile && !this.empty) {
    //   return;
    // }
    // this.fileInput.nativeElement.click();
  }

  // -----------------------------------------

}

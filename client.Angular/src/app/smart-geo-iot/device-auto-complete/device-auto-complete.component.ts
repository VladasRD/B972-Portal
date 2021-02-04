import { Component, OnInit, Input } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { Observable, BehaviorSubject, EMPTY } from 'rxjs';
import { startWith, map, debounceTime, distinctUntilChanged, tap, switchMap } from 'rxjs/operators';
import { SmartGeoIotService } from './../smartgeoiot.service';
import { Device } from '../device';

@Component({
  selector: 'app-device-auto-complete',
  templateUrl: './device-auto-complete.component.html',
  styleUrls: ['./device-auto-complete.component.css']
})
export class DeviceAutoCompleteComponent implements OnInit {

  protected searchFilter$ = new BehaviorSubject<string>('');

  @Input()
  control: FormControl;

  @Input()
  label: string;

  constructor(private sgiService: SmartGeoIotService) {
  }
  filteredOptions: Device[];

  ngOnInit() {
    this.searchFilter$.pipe(
      debounceTime(250),
      distinctUntilChanged(),
      switchMap(term => this.getDevices(term))
    ).subscribe(devices => {
      this.filteredOptions = devices;
    });

    this.control.valueChanges.subscribe(
      v => {
        this.searchFilter$.next(v);
      }
    );
  }

  private getDevices(filter: string): Observable<Device[]> {
    if (filter === '') {
      return EMPTY;
    }
    return this.sgiService.getDevices(0, 30, filter, 'only_device');
  }

  displayFn(device?: Device): string {
    return device ? device.id + ' - ' + device.name : '';
  }

}

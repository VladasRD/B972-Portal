import { SmartGeoIotService } from './../smartgeoiot.service';
import { Component, OnInit, Input } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { Observable, BehaviorSubject, EMPTY } from 'rxjs';
import { startWith, map, debounceTime, distinctUntilChanged, tap, switchMap } from 'rxjs/operators';
import { Client } from '../client';

@Component({
  selector: 'app-client-auto-complete',
  templateUrl: './client-auto-complete.component.html',
  styleUrls: ['./client-auto-complete.component.css']
})
export class ClientAutoCompleteComponent implements OnInit {

  protected searchFilter$ = new BehaviorSubject<string>('');

  @Input()
  control: FormControl;

  @Input()
  label: string;

  constructor(private smartGeoIotService: SmartGeoIotService) {
  }

  filteredOptions: Client[];

  ngOnInit() {

    this.searchFilter$.pipe(
      debounceTime(250),
      distinctUntilChanged(),
      switchMap(term => this.getClients(term))
    ).subscribe(clients => {
      this.filteredOptions = clients;
    });

    if (this.control) {
      this.control.valueChanges.subscribe(
        v => {
          this.searchFilter$.next(v);
        }
      );
    }

  }

  private getClients(filter: string): Observable<Client[]> {
    if (filter === '') {
      return EMPTY;
    }
    return this.smartGeoIotService.getClients(filter, null, null, 0, 9);
  }

  displayFn(clients?: Client): string {
    return clients ? clients.name : '';
  }


}

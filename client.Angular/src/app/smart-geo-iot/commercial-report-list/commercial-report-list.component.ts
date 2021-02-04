import { Router } from '@angular/router';
import { Utils } from './../utils';
import { Outgoing } from './../outgoing';
import { Component, OnInit } from '@angular/core';
import { GrudList } from '../../common/grud-list';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { SmartGeoIotService } from '../smartgeoiot.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-commercial-report-list',
  templateUrl: './commercial-report-list.component.html',
  styleUrls: ['./commercial-report-list.component.css']
})
export class CommercialReportListComponent extends GrudList<Object> implements OnInit {
  form: FormGroup;
  listYearsFilter: number[] = [];
  listMonthsFilter: any[] = [];
  currentYear: number = new Date().getFullYear();
  displayedColumns: string[] = ['montYear', 'description', 'clientsLicenses', 'total', 'showReport'];

  constructor(private sgiService: SmartGeoIotService, private router: Router) {
    super();
    this.form =  new FormGroup({
      'yearFilter': new FormControl(this.currentYear),
      'monthFilter': new FormControl(0)
    });
    this.fillYearsList();
    this.fillMonthsList();

    this.form.get('yearFilter').valueChanges.subscribe(val => {
      this.newSearch();
    });

    this.form.get('monthFilter').valueChanges.subscribe(val => {
      this.newSearch();
    });
  }

  ngOnInit() {
  }

  getResults(): Observable<Outgoing[]> {
    return this.sgiService.getOutgoings(this._skip, this._pageSize,
      this.searchFilter$.getValue(), this.monthFilter, this.yearFilter, c => { this._totalCount = c; });
  }

  private fillYearsList(): void {
    for (let y = 2019; y <= this.currentYear; y++) {
      this.listYearsFilter.push(y);
    }
  }

  private fillMonthsList(): void {
    for (let m = 0; m <= 12; m++) {
      this.listMonthsFilter.push({ number: m, description: Utils.enumMonths[m] });
    }
  }

  get yearFilter() {
    return this.form.get('yearFilter').value;
  }

  get monthFilter() {
    return this.form.get('monthFilter').value;
  }

  showReport(outgoingUId: string, event) {
    event.stopPropagation();
    this.router.navigate([`./sgi/relatorio-comercial-show/${outgoingUId}`]);
  }

}

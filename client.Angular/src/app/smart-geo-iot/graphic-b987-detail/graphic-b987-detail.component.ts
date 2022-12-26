import { ChartConfig } from './../../common/chartConfig';
import { ChartDataSets, ChartOptions } from 'chart.js';
import { FormGroup, FormControl } from '@angular/forms';
import { MatDialog, MatSelectChange } from '@angular/material';
import { MessageService } from './../../common/message.service';
import { SmartGeoIotService } from './../smartgeoiot.service';
import { ActivatedRoute, Router } from '@angular/router';
import { Component, OnInit } from '@angular/core';
import { String } from 'typescript-string-operations';
import { Color, Label } from 'ng2-charts';
import { MCond, MCondGraphicFilterTypeEnum } from '../MCond';

@Component({
  selector: 'app-graphic-b987-detail',
  templateUrl: './graphic-b987-detail.component.html',
  styleUrls: ['./graphic-b987-detail.component.css']
})
export class GraphicB987DetailComponent implements OnInit {

  private _deviceId: string;
  showGraphic: boolean;
  form: FormGroup;
  chartConfig = new ChartConfig();
  reports: MCond[] = [];

  public lineChartData: ChartDataSets[] = [];
  public lineChartLabels: Label[] = [];
  public lineChartLegend = true;
  public lineChartType = 'line';
  public lineChartPlugins = [];

  public lineChartOptions: ChartOptions = {
    responsive: true
  };

  public lineChartColors: Color[] = [
    {
      borderColor: 'black',
      backgroundColor: 'rgba(255,0,0,0.3)',
    },
  ];

  constructor(
    private route: ActivatedRoute,
    private sgiService: SmartGeoIotService,
    private router: Router,
    private messageService: MessageService,
    public dialog: MatDialog
  ) {
    this.showGraphic = false;
    this._deviceId = this.route.snapshot.paramMap.get('id');

    this.form = new FormGroup({
      'startPeriod': new FormControl(null),
      'endPeriod': new FormControl(new Date()),
      'filter': new FormControl('')
    });
  }

  ngOnInit() {
  }

  get pageTitle(): string {
    return String.Format('Gráfico do dispositivo {0}', this._deviceId);
  }

  printGraphic() {
    window.print();
  }

  get canViewGraphic(): boolean {
    return this.initialDateFilter != null && this.finalDateFilter != null;
  }

  get isShowGraphic(): boolean {
    return this.showGraphic && this.initialDateFilter != null && this.finalDateFilter != null;
  }

  get initialDateFilter() {
    return this.form.get('startPeriod').value;
  }

  get finalDateFilter() {
    return this.form.get('endPeriod').value;
  }

  get filter() {
    return this.form.get('filter').value;
  }

  selectedFilter(event: MatSelectChange) {
    this.lineChartData = [{ data: [], label: event.source.triggerValue }];
  }

  public getDataGraphic(): void {
    this.messageService.blockUI();
    let startPeriod: Date = null;
    let endPeriod: Date = null;

    if (this.form.get('startPeriod').value != null) {
      startPeriod = this.form.get('startPeriod').value;
    }

    if (this.form.get('endPeriod').value != null) {
      endPeriod = this.form.get('endPeriod').value;
    }

    this.sgiService.getDataMCondGraphic(
      this._deviceId, startPeriod != null ? startPeriod.toJSON() : <string>null, endPeriod != null ? endPeriod.toJSON() : <string>null, 0, 5000).subscribe(d => {

        if (d === null) {
          return;
        }
        this.reports = [];

        d.forEach(r => {
          if (r[this.filter] !== null) {
            this.reports.push(r);
          }
        });

        if (this.reports !== null && this.reports.length > 0 && this.reports !== undefined) {
          this.showGraphic = true;
        }

        this.lineChartData[0].data = this.reports.map(t => Number(t[MCondGraphicFilterTypeEnum.enumNameColumnGraphic[this.filter]]));
        this.lineChartLabels = [];

        console.log('this.lineChartData[0].data: ');
        console.log(this.lineChartData[0].data);

        this.reports.forEach((r) => {
          const day = new Date(r.date);

          const _day = day.getDate() < 10 ? `0${day.getDate()}` : day.getDate();
          const _month = day.getMonth() + 1 < 10 ? `0${day.getMonth() + 1}` : day.getMonth() + 1;
          const _hour = day.getHours() < 10 ? `0${day.getHours()}` : day.getHours();
          const _minute = day.getMinutes();
          const label = `${_day}/${_month} ${_hour}:${_minute}`;
          
          this.lineChartLabels.push(label);
        });

        this.lineChartColors = this.chartConfig.lineChartColors;
      });
  }

}

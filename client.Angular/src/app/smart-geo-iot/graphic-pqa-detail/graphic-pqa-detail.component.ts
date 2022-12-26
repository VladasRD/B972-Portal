import { Report } from './../Report';
import { ChartConfig } from './../../common/chartConfig';
import { ChartDataSets, ChartOptions } from 'chart.js';
import { FormGroup, FormControl } from '@angular/forms';
import { Dashboard } from './../dashboard';
import { MatDialog } from '@angular/material';
import { MessageService } from './../../common/message.service';
import { SmartGeoIotService } from './../smartgeoiot.service';
import { ActivatedRoute, Router } from '@angular/router';
import { Component, OnInit } from '@angular/core';
import { String } from 'typescript-string-operations';
import { Color, Label } from 'ng2-charts';

@Component({
  selector: 'app-graphic-pqa-detail',
  templateUrl: './graphic-pqa-detail.component.html',
  styleUrls: ['./graphic-pqa-detail.component.css']
})
export class GraphicPqaDetailComponent implements OnInit {

  private _deviceId: string;
  showGraphic: boolean;
  form: FormGroup;
  chartConfig = new ChartConfig();
  reports: Report[] = [];

  public lineChartData: ChartDataSets[] = [{ data: [], label: 'Temperatura' }];
  public lineChartLabels: Label[] = [];
  public lineChartLegend = true;
  public lineChartType = 'line';
  public lineChartPlugins = [];

  public lineChartOptions: ChartOptions = {
    responsive: true
  };
  // public lineChartOptions: ChartOptions = this.chartConfig.lineChartOptions;
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
      'endPeriod': new FormControl(null),
      'filter': new FormControl('temperature')
    });
  }

  ngOnInit() {
  }

  get pageTitle(): string {
    return String.Format('Gráfico do dispositivo {0} (Nivetec Analítica PQA)', this._deviceId);
  }

  printGraphic() {
    window.print();
  }

  get filter() {
    return this.form.get('filter').value;
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

  get nameFilter() {
    if(this.filter === 'ph') {
      return 'Ph';
    }
    if(this.filter === 'fluor') {
      return 'Flúor';
    }
    if(this.filter === 'cloro') {
      return 'Cloro';
    }
    if(this.filter === 'turbidez') {
      return 'Turbidez';
    }
    return 'Temperatura';
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

    this.sgiService.getDataGraphic(
      this._deviceId, startPeriod != null ? startPeriod.toJSON() : <string>null, endPeriod != null ? endPeriod.toJSON() : <string>null, 0, 25).subscribe(d => {

        if (d === null) {
          return;
        }
        this.reports = [];

        d.forEach(r => {
          if (r[this.filter] !== null) {
            this.reports.push(r);
          }
        });

        if (this.reports !== null && this.reports.length > 0) {
          this.showGraphic = true;
        }

        // this.lineChartData[0] = this.chartConfig.chartDataSetsOptions;
        this.lineChartData[0].label = this.nameFilter;
        this.lineChartData[0].data = this.reports.map(t => Number(t[this.filter].replace(',', '.')));
        
        this.lineChartLabels = [];

        this.reports.forEach((r) => {
          const day = new Date(r.date);
          // const _day = day.getUTCDate() < 10 ? `0${day.getUTCDate()}` : day.getUTCDate();
          // const _month = day.getUTCMonth() + 1 < 10 ? `0${day.getUTCMonth() + 1}` : day.getUTCMonth() + 1;
          // const label = `${_day}/${_month}`;

          const _day = day.getDate() < 10 ? `0${day.getDate()}` : day.getDate();
          const _month = day.getMonth() + 1 < 10 ? `0${day.getMonth() + 1}` : day.getMonth() + 1;
          const _hour = day.getHours() < 10 ? `0${day.getHours()}` : day.getHours();
          const _minute = day.getMinutes();
          const label = `${_day}/${_month} ${_hour}:${_minute}`;

          this.lineChartLabels.push(label);
        });


        // this.lineChartLabels = [ '176.4',
        //  '176.0'
        //   , '175.5'
        //   , '175.0'
        //   ,'174.6'
        //   ,'174.1'
        //   ,'173.2'
        //   ,'172.7'
        //   ,'172.2'
        //   ,'171.8'
        //   ,'171.3'
        //   ,'170.8'
        //   ,'170.8'
        //   ,'170.3'
        //   ];

        this.lineChartColors = this.chartConfig.lineChartColors;
      });
  }

}

import { ChartDataSets, ChartOptions } from 'chart.js';
import { Color } from 'ng2-charts';

const canvas = document.createElement('canvas');
const ctx = canvas.getContext('2d');
const gradient = ctx.createLinearGradient(0, 0, 0, 400);
gradient.addColorStop(0, 'rgba(0, 113, 206, 0.5)');
gradient.addColorStop(1, 'rgba(255,255,255,0)');

export class ChartConfig {
  public lineChartOptions: ChartOptions = {
    responsive: true,
    maintainAspectRatio: false,
    tooltips: {
      callbacks: {
          title: function(tooltipItem) {
            const label = tooltipItem[0].xLabel;
            return label[0] + ', ' + label[1];
          },
          label: function(tooltipItem, data) {
            const value = tooltipItem.value;
            return data.datasets[tooltipItem.datasetIndex].label + ': ' + value;
          }
      }
    },
    scales: {
      xAxes: [{
        gridLines: {
          color: '#d3e6f7',
          zeroLineColor: 'transparent',
          borderDash: [15, 5]
        },
        ticks: {
          fontColor: '#b5b5b5',
          fontSize: 10,
        }
      }],
      yAxes: [{
        gridLines: {
          color: 'rgba(255, 255, 255)',
          zeroLineColor: 'transparent',
        },
        ticks: {
          display: false,
          beginAtZero: true,
          fontColor: '#b5b5b5',
        }
      }]
    }
  };
  public lineChartColors: Color[] = [
    {
      borderColor: '#0071ce'
    },
  ];
  public chartDataSetsOptions: ChartDataSets = {
    backgroundColor: gradient,
    label: 'Temperatura',
    showLine: true,
    lineTension: 0.1,
    borderWidth: 0.8,
    pointBackgroundColor: '#0071ce',
    pointHitRadius: 30
  };

}

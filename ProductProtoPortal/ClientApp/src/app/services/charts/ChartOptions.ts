import {
  ApexAxisChartSeries,
  ApexTitleSubtitle,
  ApexDataLabels,
  ApexFill,
  ApexMarkers,
  ApexYAxis,
  ApexXAxis,
  ApexTooltip,
  ApexStroke,
  ApexPlotOptions,
  ApexChart, ApexTheme, ApexLegend
} from "ng-apexcharts";
import {F} from "@angular/cdk/keycodes";
import {DateFormat} from "../../model/DateLocale";

export type ApexChartOption = {
  series: any[];
  chart: ApexChart;
  xaxis: ApexXAxis;
  dataLabels: ApexDataLabels;
  markers: ApexMarkers ;
  title: ApexTitleSubtitle;
  fill: ApexFill ;
  yaxis: ApexYAxis ;
  tooltip: ApexTooltip ;
  stroke: ApexStroke ;
  grid: any; //ApexGrid;
  colors: any;
  toolbar: any;
  theme: ApexTheme,
  plot: ApexPlotOptions,
  legend:ApexLegend
};

export class ChartOptions {


  dataLabelTopOption: ApexPlotOptions = {
    bar: {
      horizontal: false,
      dataLabels: {
        position: 'top',
        hideOverflowingLabels: false
      }
    }
  }
  stackLabelTopTotal: ApexDataLabels = {
    enabled: true,
    offsetY: -30,
    style: {
      fontSize: '14px',
      colors: ["#414141"]
    }, background: {
      enabled: true,
      foreColor: '#fff',
      borderRadius: 2,
      padding: 4,
      opacity: 0.9,
      borderWidth: 1,
      borderColor: '#fff'
    },

    formatter: function (_val, opt) {
      let sum = 0;
      let lastValuedIndex = 0
      opt.config.series.forEach((x: any, i: number) => {
        if (x.data.length > 0) {
          sum += x.data[opt.dataPointIndex];
          if (x.data[opt.dataPointIndex] > 0) {
            lastValuedIndex = i;
          }
        }

      });
      if (opt.seriesIndex == lastValuedIndex && sum > 0) {
        return Math.floor(sum);
      }
      return '';
    },
  }
  DarkTheme: ApexTheme = {
    mode: "dark",

  }
  DisableLabels: ApexDataLabels = {
    enabled: false
  }
  DefaultTitleStyle:any={

      fontSize: '16px',
      fontWeight: 'bold'

  }
  getOrderStatisticChartOptions(): ApexChartOption {
    return {
      legend:{},
      chart: {
        type: 'bar', height: 400, width: '100%',
      },
      theme: {
        mode: 'dark'
      },
      xaxis: {
        categories: []
      },
      series: [
        {name: 'Всего', data: [], color: '#c2185b'},
        {name: 'Выполнено', data: [], color: '#008100'}
      ],
      dataLabels: this.stackLabelTopTotal,
      markers: {},
      fill: {},
      yaxis: {},
      tooltip: {},
      stroke: {},
      grid: {},
      colors: {},
      toolbar: {},
      title: {
        text: 'Норматив\\выполнено',
        style:this.DefaultTitleStyle
      },
      plot: this.dataLabelTopOption
    }
  }

  getOrderByPostsChartOptions(): ApexChartOption {
    return {
      legend:{},
      chart: {
        type: 'bar', height: 400, width: '100%', stackType: "normal", stacked: true
      },
      theme: {
        mode: 'dark'
      },
      xaxis: {
        categories: []
      },
      series: [
        {name: 'Прогноз', data: [], color: '#15858c'},
        {name: 'Вх. буфер', data: [], color: '#8c59a9'},
        {name: 'Ожидание', data: [], color: '#e0be1c'},
        {name: 'Выполнение', data: [], color: '#00a200'},
        {name: 'Исх. буфер', data: [], color: '#1d428f'},
        {name: 'Завершено', data: [], color: '#d4ff52'},
      ],
      dataLabels: this.stackLabelTopTotal,
      markers: {},
      fill: {},
      yaxis: {},
      tooltip: {},
      stroke: {},
      grid: {},
      colors: {},
      toolbar: {},
      title: {
        text: 'Заказ по участкам',
        style:this.DefaultTitleStyle
      },
      plot: this.dataLabelTopOption
    }
  }

  getPostStatByStatusChartOptions(): ApexChartOption {
    return {
      legend:{},
      chart: {
        type: 'bar', stackType: "normal", stacked: true, height: 600, width: '100%',
      },
      theme: {
        mode: 'dark'
      },
      xaxis: {
        categories: []
      },
      series: [
        {name: 'Прогноз', data: [], color: '#15858c'},
        {name: 'Вх. буфер', data: [], color: '#8c59a9'},
        {name: 'Ожидание', data: [], color: '#e0be1c'},
        {name: 'Выполнение', data: [], color: '#00a200'},
        {name: 'Исх. буфер', data: [], color: '#1d428f'},
      ],
      dataLabels: this.stackLabelTopTotal,
      markers: {},
      fill: {},
      yaxis: {},
      tooltip: {},
      stroke: {},
      grid: {},
      colors: {},
      toolbar: {},
      title: {
        text: 'Норматив по участкам со статусами',
        style:this.DefaultTitleStyle
      },
      plot: this.dataLabelTopOption
    }
  }

  getPostByOrdersChartOptions(): ApexChartOption {
    return {
      legend:{},
      chart: {
        type: 'bar', stackType: "normal", stacked: true, height: 600, width: '100%',
      },
      theme: {
        mode: 'dark'
      },
      xaxis: {
        categories: [],
        group:{
          groups:[]
        }
      },
      series: [],
      dataLabels: this.stackLabelTopTotal,
      markers: {},
      fill: {},
      yaxis: {},
      tooltip: {
        x: {
          formatter(val: number, opts?: any): string {
            let del = opts.series[opts.seriesIndex].length / opts.w.config.xaxis.group.groups.length;
            let post = opts.w.config.xaxis.group.groups[Math.floor(opts.dataPointIndex / del)].title;
            return `${post}: ${val}`;
          }
        }
      },
      stroke: {},
      grid: {},
      colors: {},
      toolbar: {},
      title: {
        text: 'Норматив по заказам и по статусам',
        style:this.DefaultTitleStyle
      },
      plot: this.dataLabelTopOption
    }
  }

  getPostByOrdersTotalChartOptions(): ApexChartOption {
    return {
      legend:{},
      chart: {
        type: 'bar', stackType: "normal", stacked: true, height: 600, width: '100%',
      },
      theme: {
        mode: 'dark'
      },
      xaxis: {
        categories: [],
      },
      series: [],
      dataLabels: this.stackLabelTopTotal,
      markers: {},
      fill: {},
      yaxis: {},
      tooltip: {

      },
      stroke: {},
      grid: {},
      colors: {},
      toolbar: {},
      title: {
        text: 'Норматив по заказам общая',
        style:this.DefaultTitleStyle
      },
      plot: this.dataLabelTopOption
    }
  }

  getRetroPostByOrder(post:string):ApexChartOption{
    return {
      legend:{},
      chart: {
        type: 'line', stacked: false, height: 200, width: '100%',
        group:'retroPostByOrder',id:post
      },
      theme: {
        mode: 'dark'
      },
      xaxis: {
        categories: [],
        type:"datetime"
      },
      series: [],
      dataLabels: this.stackLabelTopTotal,
      markers: {

      },
      fill: {},
      yaxis: {
        tickAmount:2
      },
      tooltip: {
        followCursor: false,

        x: {
          show: false
        },
        marker: {
          show: false
        },


      }
      ,
      stroke: {
        curve: "straight"
      },
      grid: {
        clipMarkers: false
      },
      colors: {},
      toolbar: {},
      title: {
        text: post,
        style:this.DefaultTitleStyle
      },
      plot: this.dataLabelTopOption
    }
  }

  getRetroPostByStatus(post:string):ApexChartOption{
    return {
      legend:{},
      chart: {
        type: 'line', stacked: false, height: 200, width: '100%',
        group:'retroPostByOrder',id:post
      },
      theme: {
        mode: 'dark'
      },

      xaxis: {
        categories: [],
        type:"datetime",

      },
      series: [],
      dataLabels: this.stackLabelTopTotal,
      markers: {

      },
      fill: {},
      yaxis: {
        tickAmount:2
      },
      tooltip: {
        followCursor: false,

        x: {
          show: false
        },
        marker: {
          show: false
        },


      }
      ,
      stroke: {
        curve: "straight"
      },
      grid: {
        clipMarkers: false
      },
      colors: {},
      toolbar: {},
      title: {
        text: post,
        style:this.DefaultTitleStyle
      },
      plot: this.dataLabelTopOption
    }
  }

  getRetroPostTotal():ApexChartOption{
    return {
      legend:{},
      chart: {
        type: 'line', height: 500, width: '100%',
        toolbar: {
          show: false
        }
      },
      theme: {
        mode: 'dark'
      },
      xaxis: {
        categories: [],
        type: "datetime",
        title: {
          text: "Month"
        }
      },
      series: [

      ],
      dataLabels: {
        enabled: false
      },
      markers: {},
      fill: {},
      yaxis: {},
      tooltip: {

      },
      stroke: {
        curve: "straight"
      },
      grid: {},
      colors: {},
      toolbar: {},
      title: {
        text: 'Нагрузка общая',
        style:this.DefaultTitleStyle
      },
      plot: {}
    }
  }

  getOrderTimeLineChart(order:number):ApexChartOption{
    return {
      chart: {
        type: 'rangeBar', height: 800, width: '100%', group:'timeline', id:'main'
      },
      theme: {
        mode: 'dark'
      },
      xaxis: {
        categories: [],
        type:"datetime",
        labels:{
          formatter(value: string, timestamp?: number, opts?: any): string | string[] {
            if(timestamp) {

              return DateFormat.toString(new Date(Number(value)));
            }
            else {return '';}
          }
        }
      },
      series: [],
      legend:{
        position:'top',
        horizontalAlign:"left",
        offsetX:20,
        fontSize:'14px',
        fontWeight:'bold',
        itemMargin:{
          vertical:5,
          horizontal: 10
        },

      },
      dataLabels: this.DisableLabels,
      markers: { },
      fill: { },
      yaxis: { },
      tooltip: {
        onDatasetHover: {
          highlightDataSeries: true,
        },
        custom: function (opts):string {
          console.log(opts);
          console.log(opts.w.config.series[opts.seriesIndex].data);
          const from = DateFormat.toString(new Date(opts.y1));
          const to = DateFormat.toString(new Date(opts.y2));
          const delta = Math.floor(opts.w.config.series[opts.seriesIndex].data[opts.dataPointIndex].dMin)
          //const values = opts.ctx.rangeBar.getTooltipValues(opts);

          const totalFrom = DateFormat.toString(new Date(opts.w.config.series[opts.seriesIndex].data[opts.dataPointIndex].totalPeriod?.start));
          const totalTo= DateFormat.toString(new Date(opts.w.config.series[opts.seriesIndex].data[opts.dataPointIndex].totalPeriod?.end));
          const deltaMin = Math.floor(opts.w.config.series[opts.seriesIndex].data[opts.dataPointIndex].totalPeriod?.dMins);
          return (
            '<div class="apexcharts-tooltip-rangebar">' +
            '<div> <span class="series-name" style="color: ' +
            //values.color +
            '">' +
            opts.w.config.series[opts.seriesIndex].name + ":" +
            "</span></div>" +
            '<div> <span class="category" style="color: #ffffff">' +
            opts.w.config.series[opts.seriesIndex].data[opts.dataPointIndex].x + ' (' +
            `<span style="color: #ffffff"'>${opts.w.config.series[opts.seriesIndex].data[opts.dataPointIndex].comment}</span>` +
            ')'+
            //values.ylabel + '( ' + (opts.comment?opts.comment:'') + ')'+
            ' </span> <span class="value start-value">' +
            from +
            '</span> <span class="separator">-</span> <span class="value end-value">' +
            to + ` (Δ: ${delta} минут)` +
            "</span></div>" +
            '<div>' +
              '<span class="category" style="color: #ffffff">Общий:' +
            ' </span> <span class="value start-value">' +
            totalFrom +
            '</span> <span class="separator">-</span> <span class="value end-value">' +
            totalTo + ` (Δ: ${deltaMin} минут)` +
            '</span>'+
            '</div>'+
            "</div>"
          );}
      }
      ,
      stroke: {
        colors:[
          '#494949'
        ],
        width: 2,
        show:true,

      },
      grid: {

      },
      colors: {},
      toolbar: {},
      title: {
          text:`Заказ ${order}`,
          style:this.DefaultTitleStyle
      },
      plot: {
        bar:{

          horizontal:true,
          rangeBarOverlap:true,
          barHeight: "100%",
        }
      }
    }
  }

  getIssueTimeLineChart():ApexChartOption{
    return {
      legend:{},
      chart: {
        type: 'rangeBar',  height: 200, width: '100%', group:'timeline',id:'issues'
      },
      theme: {
        mode: 'dark'
      },
      xaxis: {
        categories: [],
        type:'datetime',
        labels:{
          formatter(value: string, timestamp?: number, opts?: any): string | string[] {
            if(timestamp) {

              return DateFormat.toString(new Date(Number(value)));
            }
            else {return '';}
          }
        }
      },
      series: [],
      dataLabels: {},
      markers: {},
      fill: {},
      yaxis: {},
      tooltip: {
        onDatasetHover: {
          highlightDataSeries: true,
        },
        fixed:{
          enabled:true,
          position: 'topCenter'
        },
        custom: function (opts):string {
          console.log(opts);
          console.log(opts.w.config.series[opts.seriesIndex].data);
          const from = DateFormat.toString(new Date(opts.y1));
          const to = DateFormat.toString(new Date(opts.y2));
          const delta = Math.floor(opts.w.config.series[opts.seriesIndex].data[opts.dataPointIndex].dMin)
          //const values = opts.ctx.rangeBar.getTooltipValues(opts);

          const totalFrom = DateFormat.toString(new Date(opts.w.config.series[opts.seriesIndex].data[opts.dataPointIndex].totalPeriod?.start));
          const totalTo= DateFormat.toString(new Date(opts.w.config.series[opts.seriesIndex].data[opts.dataPointIndex].totalPeriod?.end));
          const deltaMin = Math.floor(opts.w.config.series[opts.seriesIndex].data[opts.dataPointIndex].totalPeriod?.dMins);
          return (
            '<div class="apexcharts-tooltip-rangebar" >' +
            '<div> <span class="series-name" style="color: ' +
            //values.color +
            '">' +
            opts.w.config.series[opts.seriesIndex].name + ":" +
            "</span></div>" +
            '<div> <span class="category">' +
            opts.w.config.series[opts.seriesIndex].data[opts.dataPointIndex].x + ' (' +
            opts.w.config.series[opts.seriesIndex].data[opts.dataPointIndex].comment + ')'+
            //values.ylabel + '( ' + (opts.comment?opts.comment:'') + ')'+
            ' </span> <span class="value start-value">' +
            from +
            '</span> <span class="separator">-</span> <span class="value end-value">' +
            to + ` (Δ мин: ${delta})` +
            "</span></div>" +
            '<div>' +
            '<span class="category">Общий:' +
            ' </span> <span class="value start-value">' +
            totalFrom +
            '</span> <span class="separator">-</span> <span class="value end-value">' +
            totalTo + ` (Δ мин: ${deltaMin})` +
            '</span>'+
            '</div>'+
            "</div>"
          );}

      },
      stroke: {},
      grid: {},
      colors: {},
      toolbar: {},
      title: {

      },
      plot: {
        bar:{
          horizontal:true,
          rangeBarOverlap:true,
          barHeight: "80%",
        }
      }
    }
  }
}

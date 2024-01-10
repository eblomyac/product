import { Component, OnInit } from '@angular/core';
import {DataService} from "../../../services/data.service";
import {ApexAxisChartSeries, ApexChart, ApexXAxis} from "ng-apexcharts";
import {ApexChartOption, ChartOptions} from "../../../services/charts/ChartOptions";


@Component({
  selector: 'app-retro-post-statistic',
  templateUrl: './retro-post-statistic.component.html',
  styleUrls: ['./retro-post-statistic.component.css']
})
export class RetroPostStatisticComponent implements OnInit {

  from:Date;
  to:Date=new Date();
  chartOptions:ChartOptions = new ChartOptions();

  totalChartOptions = this.chartOptions.getRetroPostTotal();

  chartsByOrder:Array<{post:string,chart:ApexChartOption}>=[];
  chartsByStatus:Array<{post:string,chart:ApexChartOption}>=[];

  constructor(private data:DataService) {
    this.from=new Date();
    this.from.setDate(this.to.getDate()-7);
  }
  getByStatusChart(post:string):ApexChartOption{
    let item =  this.chartsByStatus.find(x=>x.post == post);
    if(item==null){
      item = {post:post, chart:this.chartOptions.getRetroPostByStatus(post)};
      this.chartsByStatus.push(item);
    }
    return item.chart;
  }

  getByOrderChart(post:string):ApexChartOption{
      let item =  this.chartsByOrder.find(x=>x.post == post);
      if(item==null){
        item = {post:post, chart:this.chartOptions.getRetroPostByOrder(post)};
        this.chartsByOrder.push(item);
      }
      return item.chart;
  }


  stat:any=null;

  load(){
    this.data.Statistic.PostRetroStatistic(this.from,this.to).subscribe(x=>{
      if(x!=null){
        this.stat= x;
        this.makeData();
      }
    });
  }
  ngOnInit(): void {
    this.load();
  }
  makeData(){

    this.totalChartOptions.series=[];
    this.stat.posts.forEach((x:any)=>{
      let chart =this.getByOrderChart(x);
      chart.series=[];
      chart.xaxis.categories=this.stat.stampPoints.map((t:any)=>new Date(t).getTime());
      let postWorks = this.stat.dataByOrder.filter((d:any)=>d.postId==x);
      this.stat.orders.forEach((o:any)=>{
        chart.series.push({name:o,data:postWorks.filter((pw:any)=>pw.orderNumber==o).map((pw:any)=>pw.totalCost)})
      })

    });
    this.stat.posts.forEach((x:any)=>{
      let chart = this.getByStatusChart(x);
      chart.series=[];
      chart.xaxis.categories = this.stat.stampPoints.map((t:any)=>new Date(t).getTime());


      chart.series.push({name:'Прогноз',color: '#15858c', data:this.stat.dataByStatus.filter((d:any)=>d.post==x).map((d:any)=>d.predict)});
      chart.series.push({name:'Входящий',color: '#8c59a9', data:this.stat.dataByStatus.filter((d:any)=>d.post==x).map((d:any)=>d.income)});
      chart.series.push({name:'Ожидание', color: '#e0be1c', data:this.stat.dataByStatus.filter((d:any)=>d.post==x).map((d:any)=>d.waiting)});
      chart.series.push({name:'Выполнение', color: '#00a200', data:this.stat.dataByStatus.filter((d:any)=>d.post==x).map((d:any)=>d.running)});
      chart.series.push({name:'Исходящий', color: '#1d428f', data:this.stat.dataByStatus.filter((d:any)=>d.post==x).map((d:any)=>d.sended)});

      this.totalChartOptions.xaxis.categories = this.stat.stampPoints.map((t:any)=>new Date(t).getTime());
      this.totalChartOptions.series.push({
        name:x, data:this.stat.dataByStatus.filter((d:any)=>d.post==x).map((d:any)=>d.total)
      })



    });



  }

}

import { Component, OnInit } from '@angular/core';
import {DataService} from "../../../services/data.service";
import {
  ApexAxisChartSeries,
  ApexChart,
  ApexDataLabels,
  ApexFill,
  ApexLegend,
  ApexMarkers, ApexPlotOptions,
  ApexTheme,
  ApexTitleSubtitle
} from "ng-apexcharts";
import {ApexChartOption, ChartOptions} from "../../../services/charts/ChartOptions";


@Component({
  selector: 'app-order-statistic',
  templateUrl: './order-statistic.component.html',
  styleUrls: ['./order-statistic.component.css']
})
export class OrderStatisticComponent implements OnInit {
  orders:number[]=[];
  isLoading = false;
  stat:any;
  chartOptions:ChartOptions=new ChartOptions();
  orderChartOptions:ApexChartOption = this.chartOptions.getOrderStatisticChartOptions();
  postsChartOptions:ApexChartOption = this.chartOptions.getOrderByPostsChartOptions();






  constructor(private data:DataService) {


  }

  ngOnInit(): void {
    this.loadOrders();
  }
  loadOrders(){
    this.isLoading = true;
    this.data.Statistic.ActualOrders().subscribe(x=>{
      if(x!=null){
        this.isLoading =false;
        this.orders = x;

      }
    });
  }
  loadOrderStatistic(orderId:number){
    this.isLoading=true;
    this.stat = null;
    this.data.Statistic.OrderStatistic(orderId).subscribe(x=>{
      if(x!=null){
        this.isLoading=false;
        this.stat=x;
        this.makeOrderTotalStat();
      }
    })
  }
  makeOrderTotalStat() {

    this.orderChartOptions.series[0].data=this.stat.PostStatus.map((x:any)=>x.TotalCost);
    this.orderChartOptions.series[1].data=this.stat.PostStatus.map((x:any)=>x.CompletedCost);
    this.orderChartOptions.xaxis.categories =this.stat.PostStatus.map((x:any)=>x.Name);

    this.postsChartOptions.series[0].data = this.stat.PostStatus.map((x:any)=>x.Unstarted);
    this.postsChartOptions.series[1].data = this.stat.PostStatus.map((x:any)=>x.Income);
    this.postsChartOptions.series[2].data = this.stat.PostStatus.map((x:any)=>x.Waiting);
    this.postsChartOptions.series[3].data = this.stat.PostStatus.map((x:any)=>x.Running);
    this.postsChartOptions.series[4].data = this.stat.PostStatus.map((x:any)=>x.Sended);
    this.postsChartOptions.series[5].data = this.stat.PostStatus.map((x:any)=>x.Ended);

    this.postsChartOptions.xaxis.categories=this.stat.PostStatus.map((x:any)=>x.Name);
  }


}

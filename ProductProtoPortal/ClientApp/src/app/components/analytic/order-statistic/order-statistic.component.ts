import {Component, OnInit, ViewChild} from '@angular/core';
import {DataService} from "../../../services/data.service";
import {
  ApexAxisChartSeries,
  ApexChart,
  ApexDataLabels,
  ApexFill,
  ApexLegend,
  ApexMarkers, ApexPlotOptions,
  ApexTheme,
  ApexTitleSubtitle, ChartComponent
} from "ng-apexcharts";
import {ApexChartOption, ChartOptions} from "../../../services/charts/ChartOptions";
import {ThemeService} from "../../../services/ThemeService";


@Component({
  selector: 'app-order-statistic',
  templateUrl: './order-statistic.component.html',
  styleUrls: ['./order-statistic.component.css']
})
export class OrderStatisticComponent implements OnInit {
  orders:number[]=[];
  filteredOrders:number[]=[];
  isLoading = false;
  stat:any;
  articleStat:any;
  orderFilter=0;
  chartOptions:ChartOptions=new ChartOptions(this.themeService);
  orderChartOptions:ApexChartOption = this.chartOptions.getOrderStatisticChartOptions();
  postsChartOptions:ApexChartOption = this.chartOptions.getOrderByPostsChartOptions();

  hideEnded=false;

  @ViewChild('orderChart',{static:false})orderChart:ChartComponent|null=null;
  @ViewChild('postChart',{static:false})postChart:ChartComponent|null=null;

  currentOrder=0;
  articleFilter='';

  articleFilterUpdate(){

    if(this.articleFilter.length == 0){
      if(this.hideEnded){
        this.articleStat = this.stat.ArticleStat.filter((x:any)=>x.IsEnded==false);
      }else{
        this.articleStat = this.stat.ArticleStat;
      }
      this.makeOrderTotalStatParam(this.stat);
    }else{
      if(this.hideEnded){
        this.articleStat = this.stat.ArticleStat.filter((x:any)=>x.Article.includes(this.articleFilter) && x.IsEnded==false);
      }else{
        this.articleStat = this.stat.ArticleStat.filter((x:any)=>x.Article.includes(this.articleFilter));
      }

    }
  }

  filterUpdate(){
    if(this.orderFilter==0 || this.orderFilter == null){
      this.filteredOrders = this.orders;
    }else{
      this.filteredOrders = this.orders.filter(x=>x.toString().includes(this.orderFilter.toString()));
    }
  }


  constructor(private data:DataService, private themeService:ThemeService) {

  }
  redrawCharts(){
    let articles=[];
    if(this.hideEnded){
      articles = this.stat.ArticleStat.filter((x:any)=>x.Article.includes(this.articleFilter) && x.IsEnded==false).map((x:any)=>x.Article);
    }else{
      articles = this.stat.ArticleStat.filter((x:any)=>x.Article.includes(this.articleFilter)).map((x:any)=>x.Article);
    }

    this.loadOrderStatistic(this.currentOrder, articles);
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
        this.filterUpdate();

      }
    });
  }
  loadOrderStatistic(orderId:number, articles:Array<string>=[]){
    this.isLoading=true;

    this.currentOrder = orderId;
    if(articles.length==0) {
      this.stat = null;
      this.data.Statistic.OrderStatistic(orderId).subscribe(x => {
        if (x != null) {
          this.isLoading = false;
          this.stat = x;
          this.makeOrderTotalStatParam(this.stat);
          this.articleFilterUpdate();
        }
      });
    }else{
      this.data.Statistic.OrderStatisticArticleFiltered(orderId, articles).subscribe(x=>{
        if (x != null) {
          this.isLoading = false;
          this.makeOrderTotalStatParam(x);
        }
      });
    }

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
    this.orderChart?.updateSeries(this.orderChartOptions.series);
    this.postChart?.updateSeries(this.postsChartOptions.series);
  }
  makeOrderTotalStatParam(s:any) {

    this.orderChartOptions.series[0].data=s.PostStatus.map((x:any)=>x.TotalCost);
    this.orderChartOptions.series[1].data=s.PostStatus.map((x:any)=>x.CompletedCost);
    this.orderChartOptions.xaxis.categories =s.PostStatus.map((x:any)=>x.Name);

    this.postsChartOptions.series[0].data = s.PostStatus.map((x:any)=>x.Unstarted);
    this.postsChartOptions.series[1].data =s.PostStatus.map((x:any)=>x.Income);
    this.postsChartOptions.series[2].data = s.PostStatus.map((x:any)=>x.Waiting);
    this.postsChartOptions.series[3].data = s.PostStatus.map((x:any)=>x.Running);
    this.postsChartOptions.series[4].data = s.PostStatus.map((x:any)=>x.Sended);
    this.postsChartOptions.series[5].data = s.PostStatus.map((x:any)=>x.Ended);

    this.postsChartOptions.xaxis.categories=s.PostStatus.map((x:any)=>x.Name);
    this.orderChart?.updateSeries(this.orderChartOptions.series);
    this.postChart?.updateSeries(this.postsChartOptions.series);
  }


}

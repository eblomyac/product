import {Component, OnInit, ViewChild} from '@angular/core';
import {DataService} from "../../../services/data.service";

import {ApexChartOption, ChartOptions} from "../../../services/charts/ChartOptions";
import {DatePipe} from "@angular/common";



@Component({
  selector: 'app-retro-post-statistic',
  templateUrl: './retro-post-statistic.component.html',
  styleUrls: ['./retro-post-statistic.component.css']
})
export class RetroPostStatisticComponent implements OnInit {

  from:Date;
  to:Date=new Date();
  chartOptions:ChartOptions = new ChartOptions();
  isChange3d=false;

  totalChartOptions = this.chartOptions.getRetroPostTotal();

  chartsByOrder:Array<{post:string,chart:ApexChartOption}>=[];
  chartsByStatus:Array<{post:string,chart:ApexChartOption}>=[];

  bar3dChart:any=null;

  selectedPostsBar3d:Array<string> = [];
  selectedStatusBar3d:Array<string>=['Прогноз',"Вх. буфер", "Ожидание","Выполнение","Исх. буфер"];
  posts:Array<string>=[];
  echartsInstance:any|undefined=null;
  statuses:Array<string>=['Прогноз',"Вх. буфер", "Ожидание","Выполнение","Исх. буфер"];


  constructor(private data:DataService, private datePipe:DatePipe) {
    this.from=new Date();
    this.from.setDate(this.to.getDate()-7);
    this.bar3dChart = {
      darkMode:true,
      color:['#15858c','#8c59a9','#e0be1c','#00a200','#1d428f'],
      textStyle:{
        color:'#ffffff'
      },

      grid3D: {
        boxWidth:130,
        boxHeight:100,
        boxDepth:130,
        viewControl: {
          // autoRotate: true
        },
        light: {
          main: {
            shadow: false,
            quality: 'ultra',
            intensity: 1.5
          }
        },
        axisPointer:{
          lineStyle:{
            color:'#ffffff'
          },
          label:{
            textStyle:{
              color:'#ffffff'
            }
          }
        },
      },
      tooltip: {
        formatter:this.tooltip
      },
      xAxis3D: {
        type: 'category',
        name: 'Дата',
        nameTextStyle:{
          color:'#ffffff',

        },
        nameGap:25,
        axisLabel:{
          textStyle:{
            color:'#ffffff'
          }
        }
      },
      yAxis3D: {
        type: 'category',
        name: 'Участок',
        nameTextStyle:{
          color:'#ffffff',

        },
        nameGap:30,
        axisLabel:{
          textStyle:{
            color:'#ffffff'
          }
        }
      },
      zAxis3D: {
        type: 'value',
        name:'Норматив',
        nameTextStyle:{
          color:'#ffffff'
        },
        nameGap:25,
        axisLabel:{
          textStyle:{
            color:'#ffffff'
          }
        }
      },
      series: [
        {

        },
      ],
    }}
  tooltip(a:any):string{
    console.log(a)
    let s = "Пост: " + a.value[1];
    s+="<br>Дата: "+ a.value[0];
    s+="<br>Статус: " +a.seriesName;
    s+="<br>Норматив: "+a.value[2];
    return s;

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
  onChartInit(ec:any) {
    this.echartsInstance = ec;
  }
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

  push3dDataBar(array:Array<any>,name:string){
    if(this.isStatus3dSelected(name)) {
      console.log(name);

      this.bar3dChart.series.push(
        {
          type: 'bar3D',
          stack: 'stack',
          shading: 'lambert',
          name: name,
          emphasis: {label: {show: false, textStyle: {color: '#ffffff'}}},
          data: array
        }
      )
    }
  }
  makeData3d(){
    this.isChange3d=true;
    console.log(this.selectedStatusBar3d);
    this.bar3dChart.series=[];
    this.totalChartOptions.series=[];


    //series = status
    //data [x,y,z] = x:stamp , y: post , z:value of status



    let predictData:Array<any> = [];
    let incomeData:Array<any> = [];
    let waitingData:Array<any> = [];
    let runData:Array<any> = [];
    let sendData: Array<any> = [];
    this.stat.dataByStatus.forEach((x:any)=>{
      if(this.selectedPostsBar3d.indexOf(x.post)>=0) {
        predictData.push([this.datePipe.transform(x.stamp, 'dd.MM HH:mm'), x.shortName, x.predict]);
        incomeData.push([this.datePipe.transform(x.stamp, 'dd.MM HH:mm'), x.shortName, x.income])
        waitingData.push([this.datePipe.transform(x.stamp, 'dd.MM HH:mm'), x.shortName, x.waiting]);
        runData.push([this.datePipe.transform(x.stamp, 'dd.MM HH:mm'), x.shortName, x.running]);
        sendData.push([this.datePipe.transform(x.stamp, 'dd.MM HH:mm'), x.shortName, x.sended]);
      }
    });

    this.push3dDataBar(predictData,"Прогноз");
    this.push3dDataBar(incomeData,"Вх. буфер");
    this.push3dDataBar(waitingData,"Ожидание");
    this.push3dDataBar(runData,"Выполнение");
    this.push3dDataBar(sendData,"Исх. буфер");


    //end 3d
    if(this.echartsInstance){
      console.log(this.echartsInstance);
      this.echartsInstance.setOption(this.bar3dChart, true,true );
      this.echartsInstance.resize();
    }else{
      console.log('null inst');
    }
    this.isChange3d=false;
  }
  selectPost3d(s:string){
    let index =  this.selectedPostsBar3d.indexOf(s);
    if(index>-1){
      this.selectedPostsBar3d.splice(index,1);
    }else{
      this.selectedPostsBar3d.push(s);
    }
    this.makeData3d();
  }
  selectStatus3d(s:string){
    let index =  this.selectedStatusBar3d.indexOf(s);
    if(index>-1){
      this.selectedStatusBar3d.splice(index,1);
    }else{
      this.selectedStatusBar3d.push(s);
    }
    this.makeData3d();
  }
  isPost3dSelected(s:string):boolean{
    return this.selectedPostsBar3d.indexOf(s)>-1;
  }
  isStatus3dSelected(s:string):boolean{
    return this.selectedStatusBar3d.indexOf(s)>-1;
  }

  makeData(){


    this.selectedPostsBar3d = this.stat.posts.map((x:any)=>x);

    this.posts = this.stat.posts.map((x:any)=>x);
    this.makeData3d();

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

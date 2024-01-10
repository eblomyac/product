import {Component, OnInit, ViewChild} from '@angular/core';
import {DataService} from "../../../services/data.service";

import {ApexChartOption, ChartOptions} from "../../../services/charts/ChartOptions";

@Component({
  selector: 'app-post-statistic',
  templateUrl: './post-statistic.component.html',
  styleUrls: ['./post-statistic.component.css']
})
export class PostStatisticComponent implements OnInit {

  stat: any | null = null;
  isLoading = false;
  ChartOptions: ChartOptions = new ChartOptions();
  byPostStatus: ApexChartOption = this.ChartOptions.getPostStatByStatusChartOptions();
  byOrderStatus: ApexChartOption = this.ChartOptions.getPostByOrdersChartOptions();
  byOrderTotal: ApexChartOption = this.ChartOptions.getPostByOrdersTotalChartOptions();

  constructor(private data: DataService) {
  }


  ngOnInit(): void {
    this.loadStat();
  }

  makeChartData() {
    // console.log(this.stat);


    let orderCat: string[] = [];
    let groupsXAxis: { title: string; cols: number; }[] = [];

    this.stat.Posts.forEach((x: any, i: number) => {
      groupsXAxis.push({title: x.Name, cols: this.stat.StatusNames.length});
      orderCat.push(...this.stat.StatusNames);
    });

    this.byPostStatus.xaxis.categories = this.stat.Posts.map((x: any) => x.Name);
    this.byOrderStatus.xaxis.categories = orderCat;//this.stat.Posts.map((x:any)=>x.Name);
    this.byOrderTotal.xaxis.categories = this.stat.Posts.map((x: any) => x.Name);
    if (this.byOrderStatus.xaxis.group) {
      this.byOrderStatus.xaxis.group.groups = groupsXAxis;
    }
    for (let index = 0; index < this.stat.StatusNames.length; index++) {
      this.byPostStatus.series[index].data = this.stat.Posts.map((x: any) => x.Statuses[index].Cost);
    }
    let ser: Array<{ data: any[], group: string, name: string }> = [];
    let serTotal: Array<{ data: any[], name: string }> = [];
    this.stat.Posts.forEach((post: any) => {
      post.OrderLoads.forEach((order: any) => {
        let sum = 0;
        order.Statuses.forEach((status: any) => {
            let n = order.OrderNumber.toString();
            sum += status.Cost;
            let record = ser.find(z => z.name == n && z.group == '');
            if (record == null) {
              ser.push({data: [status.Cost], group: '', name: n});
            } else {
              record.data.push(status.Cost);
            }


          })
        let recordTotal = serTotal.find(z=>z.name == order.OrderNumber);
        if(recordTotal==null){
          serTotal.push({data:[Math.floor(sum)], name:order.OrderNumber});
        }else{
          recordTotal.data.push(Math.floor(sum));
        }
      })
    });
    //console.log(ser);

    this.byOrderStatus.series = ser;
    this.byOrderTotal.series = serTotal;

  }

  loadStat() {
    this.isLoading = true;
    this.stat = null;
    this.data.Statistic.PostStatistic().subscribe(x => {
      if (x != null) {
        this.isLoading = false;
        this.stat = x;
        this.makeChartData();
      }
    });
  }

}

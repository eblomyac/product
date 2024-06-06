import {Component, OnInit} from '@angular/core';
import {DataService} from "../../../services/data.service";
import {ApexChartOption, ChartOptions} from "../../../services/charts/ChartOptions";


@Component({
  selector: 'app-order-time-line',
  templateUrl: './order-time-line.component.html',
  styleUrls: ['./order-time-line.component.css']
})
export class OrderTimeLineComponent implements OnInit {

  orderNumber = 0;
  isLoading = false;
  stat: any = null;
  chartOptions: ChartOptions = new ChartOptions();
  timeLineChart: ApexChartOption = this.chartOptions.getOrderTimeLineChart(0);
  issueLineChart: ApexChartOption = this.chartOptions.getIssueTimeLineChart();

  selectedArticles:string[]=[];
  articles:string[]=[];



  getStatusColor(status: number): string {
    switch (status) {
      case 10:
        return '#8c59a9';
      case 20:
        return '#e0be1c';
      case 30:
        return '#00a200';
      case 40:
        return '#1d428f';

    }
    return 'rgba(66,66,66,0)';
  }

  constructor(private data: DataService) {
  }

  ngOnInit(): void {
  }

  load() {
    this.isLoading = true;
    this.data.Statistic.OrderTimeLine(this.orderNumber).subscribe(x => {
      if (x != null) {
        this.isLoading = false;
        this.stat = x;
        this.fillArticles();
        this.makeData();

      }
    });
  }
  fillArticles(){
    this.articles = [];
    this.selectedArticles = [];
    this.stat.Articles.forEach((article:any)=>{
      if(this.articles.findIndex(z=>z == article.Name)==-1) {
        this.articles.push(article.Name);
      }
    });
  }
  isArticleSelected(a:string):boolean{
    return this.selectedArticles.findIndex(z=>z == a)>-1;
  }
  selectArticle(a:string){
    let i = this.selectedArticles.findIndex(z=>z == a);
    if( i==-1) {
      this.selectedArticles.push(a);
    }else{
      this.selectedArticles.splice(i,1);
    }
    this.makeData();
  }

  makeData() {
    this.timeLineChart = this.chartOptions.getOrderTimeLineChart(this.stat.OrderNumber);


    this.stat.Articles.forEach((article: any) => {
      let name = article.Name;
      let data: any[] = [];

      if(this.isArticleSelected(name)) {
        article.Data.forEach((d: any) => {

          if (d.TotalPeriod) {
            data.push(
              {
                fill: {
                  type: "gradient",
                  gradient: {
                    shade: "light",
                    type: "vertical",
                    shadeIntensity: 0.25,
                    gradientToColors: undefined,
                    inverseColors: true,
                    opacityFrom: 1,
                    opacityTo: 1,
                    stops: [50, 0, 100, 100]
                  }
                },
                x: d.Post,
                y: [new Date(d.TotalPeriod.Start).getTime(), new Date(d.TotalPeriod.End).getTime()]
              }
            )
          }

          d.Values.forEach((value: any) => {

            data.push(
              {
                fillColor: this.getStatusColor(value.Status),
                x: d.Post,
                y: [new Date(value.Start).getTime(), new Date(value.End).getTime()],
                dMin: value.DeltaMinutes,
                dHours: value.DeltaHours,
                comment: value.Comment,
                totalPeriod: {
                  start: d.TotalPeriod?.Start,
                  end: d.TotalPeriod?.End,
                  dHours: d.TotalPeriod?.DeltaHours,
                  dMins: d.TotalPeriod?.DeltaMinutes
                },
                fill: {
                  type: "gradient",
                  gradient: {
                    shade: "light",
                    type: "vertical",
                    shadeIntensity: 0.25,
                    gradientToColors: undefined,
                    inverseColors: true,
                    opacityFrom: 0,
                    opacityTo: ((value.Status > 9 && value.Status < 41) ? 1 : 0),
                    stops: [50, 0, 100, 100]
                  }
                },

              }
            )
          })
          d.Issues.forEach((issue: any) => {
            data.push(
              {
                x: d.Post,
                fillColor: 'rgba(105,0,0,0.6)',
                y: [new Date(issue.Start).getTime(), new Date(issue.End).getTime()],
                dMin: issue.DeltaMinutes,
                dHours: issue.DeltaHours,
                comment: issue.Type + ' ' + issue.Description,
                totalPeriod: {
                  start: d.TotalPeriod?.Start,
                  end: d.TotalPeriod?.End,
                  dHours: d.TotalPeriod?.DeltaHours,
                  dMins: d.TotalPeriod?.DeltaMinutes
                }
              }
            )
          })
        })

        this.timeLineChart.series.push(
          {name: name, data: data}
        )

      }
    })
   // console.log(this.timeLineChart.series);

  }

}

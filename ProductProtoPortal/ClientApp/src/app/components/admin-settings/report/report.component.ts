import { Component } from '@angular/core';
import {DataService} from "../../../services/data.service";
import { DatePipe } from '@angular/common';
@Component({
  selector: 'app-report',
  templateUrl: './report.component.html',
  styleUrl: './report.component.css'
})
export class ReportComponent {

  date=new Date();
  isLoading=false;
  datepipe: DatePipe = new DatePipe('en-US')
  infoMessage="";
  reportType: 'production' | 'article' | 'additional'|'costreport'='production';

  fromDate= new Date();
  toDate= new Date();
  minDate:Date=new Date();

  moveDay=true;

  constructor(private data:DataService) {
    this.fromDate.setDate(this.fromDate.getDate()-7);
    this.minDate=(new Date(2024,5,15));
    if (this.fromDate<this.minDate){
      this.fromDate = this.minDate;
    }
  }
  downloadExcel(){
    window.open('/download/fullreportonlinedata.xlsx',"_blank")
  }
  downloadCOstReport(){
    this.data.Statistic.CostReport().subscribe(x=>{
      if(x){
        window.open(x,'_blank');
      }
    })
  }
  makeDailyReport(){

    this.isLoading=true;
    let formattedDate = this.datepipe.transform(this.date, 'dd-MM-yyyy')!;
    this.data.Statistic.DateReport(formattedDate).subscribe(x=>{
      this.isLoading=false;
    }, e=>{
      this.isLoading=false;
    })
  }
  makeReport(){
    this.isLoading=true;
    let fromDate = this.datepipe.transform(this.fromDate, 'dd-MM-yyyy')!;
    let toDate = this.datepipe.transform(this.toDate, 'dd-MM-yyyy')!;

    let req;
    switch (this.reportType){
      case "additional":
        req =  this.data.Statistic.AdditionalCostReport(fromDate,toDate, this.moveDay)
        break;
      case "article":
        req =  this.data.Statistic.ArticlePeriodReport(fromDate,toDate, this.moveDay)
        break;
      case "production":
        req =  this.data.Statistic.PeriodReport(fromDate,toDate, this.moveDay)
        break;
      case "costreport":
        this.data.Statistic.CostPeriodReport(fromDate, toDate).subscribe(x=>{
          if(x){
            this.isLoading = false;
            window.open(x,'_blank');
          }
        });
        break;
    }
    if(req) {
      req.subscribe(x => {
        this.infoMessage = "Отчет будет доставлен на почту в ближайшее время"
        setTimeout(x => {
          this.isLoading = false;
          this.infoMessage = '';
        }, 3000)
      });
    }

  }


  orderMaconomyClose(){
    this.isLoading=true;
    this.data.Work.MaconomySyncClose().subscribe(x=>{
      if(x) this.isLoading=false;
    }, error => {this.isLoading=false;console.log(error)});
  }
}

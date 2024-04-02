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
  constructor(private data:DataService) {
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

  orderMaconomyClose(){
    this.isLoading=true;
    this.data.Work.MaconomySyncClose().subscribe(x=>{
      if(x) this.isLoading=false;
    }, error => {this.isLoading=false;console.log(error)});
  }
}

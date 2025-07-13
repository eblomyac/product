import { Component } from '@angular/core';

@Component({
  selector: 'app-calendar',
  templateUrl: './calendar.component.html',
  styleUrl: './calendar.component.css'
})
export class CalendarComponent {

  workers:string[]=[];
  selectedDate=new Date();
  constructor() {
    this.selectedDate = new Date(this.selectedDate.getFullYear(), this.selectedDate.getMonth());
  }
  public changeMonth(val:number){
    if(this.selectedDate.getMonth()==0 && val<0){
      this.selectedDate = new Date(this.selectedDate.getFullYear()-1,11);
    }else if(this.selectedDate.getMonth()==11 && val>0){
      this.selectedDate = new Date(this.selectedDate.getFullYear()+1,0);
    }else{
      this.selectedDate = new Date(this.selectedDate.getFullYear(),this.selectedDate.getMonth()+val);
    }
  }
  public getWorkers(){

  }
}

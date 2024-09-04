import {Component, Inject, OnInit} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialogRef} from "@angular/material/dialog";
import {DailySource} from "../../model/DailySource";


@Component({
  selector: 'app-number-dialog',
  templateUrl: './daily-source-dialog.component.html',
  styleUrls: ['./daily-source-dialog.component.css']
})
export class DailySourceDialogComponent implements OnInit {


  dates:Array<string> = [];
  dval:any={};
  constructor(private dialogRef:MatDialogRef<DailySourceDialogComponent>,@Inject(MAT_DIALOG_DATA)
  public data: {productionLines:DailySource[]}) {
    this.dates=this.Dates();
    this.dates.forEach(z=>{
      this.dval[z]=this.data.productionLines.filter(x=>this.pad(x.day,2) + '.' + this.pad(x.month,2)+'.'+x.year == z);
    });
    console.log(this.dval)
  }

  ngOnInit(): void {
  }
  pad(num:number,places:number){
   return  String(num).padStart(places, '0')
  }
  Dates(){
    let result:Array<string>=[];
    this.data.productionLines.forEach(x=>{
      let s = this.pad(x.day,2) + '.' + this.pad(x.month,2)+'.'+x.year;
      let existDate = result.filter(z=>z==s);
      if(existDate.length==0){
        result.push(s);
      }
    })
    return result;
  }
  ok(){
    this.dialogRef.close(this.data.productionLines)
  }
  cancel(){
    this.dialogRef.close(null)
  }
}

import {Component, Inject} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialogRef} from "@angular/material/dialog";
import {ProductCalendarRecord} from "../../model/Hr";

@Component({
  selector: 'app-hr-action-dialog',
  templateUrl: './hr-action-dialog.component.html',
  styleUrl: './hr-action-dialog.component.css'
})
export class HrActionDialogComponent {
  constructor(private dialogRef:MatDialogRef<HrActionDialogComponent>,@Inject(MAT_DIALOG_DATA)
  public data: {calendarRecords:Array<{data:ProductCalendarRecord,meta:{selected:boolean}}>}) { }


  setVal(v:number|string) {
    let val = Number(v);
    this.data.calendarRecords.forEach(calendarRecord => {
      calendarRecord.data.planningHours =val;
    })
    this.close();
  }
  setVacation(){
    this.data.calendarRecords.forEach(calendarRecord => {
      calendarRecord.data.planningHours =0;
    })
    this.close();
  }
  double(){
    this.data.calendarRecords.forEach(calendarRecord => {
      calendarRecord.data.planningHours *=2;
    })
    this.close();
  }
  div2(){
    this.data.calendarRecords.forEach(calendarRecord => {
      calendarRecord.data.planningHours /=2;
    })
    this.close();
  }

  close(){
    this.dialogRef.close();
  }


}

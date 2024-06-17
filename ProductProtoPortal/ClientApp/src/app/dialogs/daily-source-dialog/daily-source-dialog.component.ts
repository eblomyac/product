import {Component, Inject, OnInit} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialogRef} from "@angular/material/dialog";
import {DailySource} from "../../model/DailySource";


@Component({
  selector: 'app-number-dialog',
  templateUrl: './daily-source-dialog.component.html',
  styleUrls: ['./daily-source-dialog.component.css']
})
export class DailySourceDialogComponent implements OnInit {

  constructor(private dialogRef:MatDialogRef<DailySourceDialogComponent>,@Inject(MAT_DIALOG_DATA)
  public data: {productionLines:DailySource[]}) {

  }

  ngOnInit(): void {
  }

  ok(){
    this.dialogRef.close(this.data.productionLines)
  }
  cancel(){
    this.dialogRef.close(null)
  }
}

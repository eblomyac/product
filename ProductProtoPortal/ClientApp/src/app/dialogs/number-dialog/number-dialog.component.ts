import {Component, Inject, OnInit} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialogRef} from "@angular/material/dialog";


@Component({
  selector: 'app-number-dialog',
  templateUrl: './number-dialog.component.html',
  styleUrls: ['./number-dialog.component.css']
})
export class NumberDialogComponent implements OnInit {

  value=0;
  minValue:number|null=null;

  constructor(private dialogRef:MatDialogRef<NumberDialogComponent>,@Inject(MAT_DIALOG_DATA)
  public data: {caption:string,currentValue:number, minValue:number|null}) {
    this.value=data.currentValue;
    this.minValue = data.minValue;
  }

  ngOnInit(): void {
  }

  ok(){
    this.dialogRef.close(this.value);
  }
  cancel(){
    this.dialogRef.close(null)
  }
}

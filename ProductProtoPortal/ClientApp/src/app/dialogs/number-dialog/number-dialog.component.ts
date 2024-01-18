import {Component, Inject, OnInit} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialogRef} from "@angular/material/dialog";


@Component({
  selector: 'app-number-dialog',
  templateUrl: './number-dialog.component.html',
  styleUrls: ['./number-dialog.component.css']
})
export class NumberDialogComponent implements OnInit {

  value=0;

  constructor(private dialogRef:MatDialogRef<NumberDialogComponent>,@Inject(MAT_DIALOG_DATA)
  public data: {caption:string,currentValue:number}) {
    this.value=data.currentValue;
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

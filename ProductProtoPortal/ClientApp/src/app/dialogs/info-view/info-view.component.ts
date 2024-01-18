import {Component, Inject, OnInit} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialogRef} from "@angular/material/dialog";


@Component({
  selector: 'app-info-view',
  templateUrl: './info-view.component.html',
  styleUrls: ['./info-view.component.css']
})
export class InfoViewComponent implements OnInit {

  constructor(private dialogRef:MatDialogRef<InfoViewComponent>,@Inject(MAT_DIALOG_DATA)
  public data: {caption:string,infos:string[]}) { }

  ngOnInit(): void {
  }

}

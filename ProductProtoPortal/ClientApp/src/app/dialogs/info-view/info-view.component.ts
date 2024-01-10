import {Component, Inject, OnInit} from '@angular/core';
import {MAT_LEGACY_DIALOG_DATA as MAT_DIALOG_DATA, MatLegacyDialogRef as MatDialogRef} from "@angular/material/legacy-dialog";

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

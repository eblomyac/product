import {Component, Inject} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialogRef} from "@angular/material/dialog";
import {DataService} from "../../services/data.service";

@Component({
  selector: 'app-picture-view',
  templateUrl: './picture-view.component.html',
  styleUrl: './picture-view.component.css'
})
export class PictureViewComponent {
  constructor(private dialogRef:MatDialogRef<PictureViewComponent>,@Inject(MAT_DIALOG_DATA)
  public data: {img:string})
  {
  }
}

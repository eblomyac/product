import {Component, Inject, OnInit} from '@angular/core';

import {DataService} from "../../services/data.service";
import {Issue, IssueTemplate} from "../../model/Issue";
import {MAT_DIALOG_DATA, MatDialogRef} from "@angular/material/dialog";

@Component({
  selector: 'app-issue-create-dialog',
  templateUrl: './issue-create-dialog.component.html',
  styleUrls: ['./issue-create-dialog.component.css']
})
export class IssueCreateDialogComponent implements OnInit {


  issue:Issue;
  templates:IssueTemplate[]=[];

  constructor(private dataService:DataService,private dialogRef:MatDialogRef<IssueCreateDialogComponent>,@Inject(MAT_DIALOG_DATA)
  public data: {}) {
   this.issue = {id:0, resolved: new Date(), created :new Date(), description:'', templateId:0, workId:0 };
   this.dataService.Issue.ListTemplates().subscribe(x=>{
     if(x!=null){
       this.templates = x;
     }
   });
  }


  ngOnInit(): void {
  }
  cancel(){
    this.dialogRef.close(null);}
  ok(){
    this.dialogRef.close(this.issue);
  }

}

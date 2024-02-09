import {Component, Inject, OnInit} from '@angular/core';

import {DataService} from "../../services/data.service";
import {Issue, IssueTemplate} from "../../model/Issue";
import {MAT_DIALOG_DATA, MatDialogRef} from "@angular/material/dialog";
import {Work} from "../../model/Work";

@Component({
  selector: 'app-issue-create-dialog',
  templateUrl: './issue-create-dialog.component.html',
  styleUrls: ['./issue-create-dialog.component.css']
})
export class IssueCreateDialogComponent implements OnInit {


  issue:Issue;
  templates:IssueTemplate[]=[];
  linkWork:Work|null=null;
  isReturn = false;

  constructor(private dataService:DataService,private dialogRef:MatDialogRef<IssueCreateDialogComponent>,@Inject(MAT_DIALOG_DATA)
  public data: {forWork:Work}) {
    this.linkWork = data.forWork;
   this.issue = {id:0, resolved: new Date(), created :new Date(), description:'', templateId:0, workId:0, returnBackPostId:'', returnedFromPostId:'' };
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
    if(this.isReturn && this.linkWork){
      this.issue.returnBackPostId = this.linkWork?.structure.movedFrom;
    }
    this.dialogRef.close(this.issue);
  }

}

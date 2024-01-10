import { Component, OnInit } from '@angular/core';
import {DataService} from "../../../services/data.service";
import {IssueTemplate} from "../../../model/Issue";

@Component({
  selector: 'app-issue-settings',
  templateUrl: './issue-settings.component.html',
  styleUrls: ['./issue-settings.component.css']
})
export class IssueSettingsComponent implements OnInit {

  isLoading=false;
  list:IssueTemplate[]=[];
  constructor(private data:DataService) { }

  ngOnInit(): void {
    this.loadIssues();
  }

  loadIssues(){
    this.isLoading=true;
    this.data.Issue.ListTemplates().subscribe(x=>{

      if(x!=null){
        this.list = x;
        this.isLoading=false;
      }

    })
  }
  addIssue(name:string){
    let exist = this.list.find(x=>x.name == name);
    if(!exist){
      this.list.push({name:name,id:0});
    }

  }
  removeIssue(i:number){
    this.list.splice(i,1);
  }
  save(){
    this.isLoading=true;
    this.data.Issue.UpdateTemplates(this.list).subscribe(x=>{

      if(x!=null){
        this.list = x;
        this.isLoading=false;
      }

    })
  }
}

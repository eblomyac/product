import {Component, Input, OnInit} from '@angular/core';
import {IWork, Work} from "../../../model/Work";

import {Router} from "@angular/router";
import {MatDialog} from "@angular/material/dialog";
import {DataService} from "../../../services/data.service";
import {DialogHandlerService} from "../../../services/dialog-handler.service";
import {DailySourceDialogComponent} from "../../../dialogs/daily-source-dialog/daily-source-dialog.component";
import {AdditionalCostDialogComponent} from "../../../dialogs/additional-cost-dialog/additional-cost-dialog.component";

@Component({
  selector: 'app-work-compact-view',
  templateUrl: './work-compact-view.component.html',
  styleUrls: ['./work-compact-view.component.css']
})
export class WorkCompactViewComponent implements OnInit {

  @Input("Work")work:Work|null=null;

  async additionalCost(){
    let sourceDialog = await DialogHandlerService.Singleton.ask(AdditionalCostDialogComponent, {
      data:{
        work:this.work,
        dataService:this.dataService
      },
    })
  }
  priorityColor():string{
    let r = this.work?.structure.priority;
    if(r){
      r+=40
      if(r<1){
        r=0;
      }
      if(r>254){
        r=255;
      }
      return "rgba("+r+", 10, 10, 0.6)";
    }else{
      return "rgba(0,0,0,0)";
    }

  }
  view(){
    this.router.navigate(['/card'], {queryParams:{article:this.work?.structure.article}})
  }

  constructor(public router:Router, private matDialog:MatDialog, private dataService:DataService) { }


  ngOnInit(): void {
  }






}

import {Component, Input, OnInit} from '@angular/core';
import {IWork, Work} from "../../../model/Work";

import {Router} from "@angular/router";

@Component({
  selector: 'app-work-compact-view',
  templateUrl: './work-compact-view.component.html',
  styleUrls: ['./work-compact-view.component.css']
})
export class WorkCompactViewComponent implements OnInit {

  @Input("Work")work:Work|null=null;

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

  constructor(public router:Router) { }


  ngOnInit(): void {
  }






}

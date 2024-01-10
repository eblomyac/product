import {Component, Input, OnInit} from '@angular/core';
import {IWork, Work} from "../../../model/Work";
import {MatLegacyDialog as MatDialog} from "@angular/material/legacy-dialog";
import {InfoViewComponent} from "../../../dialogs/info-view/info-view.component";
import {Observable} from "rxjs";
import {Router} from "@angular/router";

@Component({
  selector: 'app-work-compact-view',
  templateUrl: './work-compact-view.component.html',
  styleUrls: ['./work-compact-view.component.css']
})
export class WorkCompactViewComponent implements OnInit {

  @Input("Work")work:Work|null=null;

  view(){
    this.router.navigate(['/card'], {queryParams:{article:this.work?.structure.article}})
  }

  constructor(public router:Router) { }


  ngOnInit(): void {
  }






}

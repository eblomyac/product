import {Component, Inject, Injector, OnDestroy, OnInit} from '@angular/core';

import {ActivatedRoute} from "@angular/router";
import {Subscription} from "rxjs";

import {DataService} from "../../../services/data.service";
import {TechCard} from "../../../model/TechCard";
import {MAT_DIALOG_DATA, MatDialogRef} from "@angular/material/dialog";

@Component({
  selector: 'app-card-view',
  templateUrl: './card-view.component.html',
  styleUrls: ['./card-view.component.css']
})
export class CardViewComponent implements OnInit, OnDestroy {

  routeSub:Subscription|null;
  card:TechCard|null=null;

  set Article(value:string){
    this.partName = value;
    this.load();
  }
  partName:string='';
  public dialogRef:MatDialogRef<any>|null = null;
  private  dialogData;

  constructor(private activatedRoute:ActivatedRoute,private injector: Injector, private data:DataService) {

    let dialog = this.injector.get(MatDialogRef, null);
    if(dialog){
      this.dialogRef = dialog;
      this.dialogData = this.injector.get(MAT_DIALOG_DATA, null);
      if(this.dialogData && this.dialogData.partName){
        this.Article = this.dialogData.partName;
      }
      this.routeSub = new Subscription();
    }else{
      this.routeSub = activatedRoute.queryParams.subscribe(x=>{
        this.Article = decodeURIComponent(x['article']);
      });
    }



  }

  ngOnInit(): void {
  }
  ngOnDestroy() {
    if(this.routeSub){
      this.routeSub.unsubscribe();
    }

  }

  load(){
      //console.log(this.partName);
    this.data.TechCard.Card(this.partName).subscribe(x=>{
      this.card = x;
    })
  }

}

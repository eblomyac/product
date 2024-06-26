import {Component, Inject} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialogRef} from "@angular/material/dialog";
import {DailySource} from "../../model/DailySource";
import {Work} from "../../model/Work";
import {DataService} from "../../services/data.service";
import {AdditionalCostTemplate} from "../../model/AdditionalCost";

@Component({
  selector: 'app-additional-cost-dialog',
  templateUrl: './additional-cost-dialog.component.html',
  styleUrl: './additional-cost-dialog.component.css'
})
export class AdditionalCostDialogComponent {

  templates:AdditionalCostTemplate[]=[];
  loadTemplates(){
    this.data.dataService.AdditionalCostData.TemplatesList(false).subscribe(x=>{
      if(x!=null){
        this.templates = x;
      }
    })
  }
  constructor(private dialogRef:MatDialogRef<AdditionalCostDialogComponent>,@Inject(MAT_DIALOG_DATA)
  public data: {work:Work, dataService:DataService}) {
    this.loadTemplates();
  }
  ok(){

  }
  create(templateId:number,desc:string,cost:number){
    let ac = {comment:'',cost:cost,additionalCostTemplateId:templateId,description:desc, id:0, workId:this.data.work.structure.id};
    this.data.dataService.AdditionalCostData.CreateWorkAddCost(ac).subscribe(x=>{
      if(x!=null){
        this.data.work.structure.additionalCosts.push(x);
      }
    });
  }

  protected readonly Number = Number;
}

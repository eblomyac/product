import {Component, Inject} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialogRef} from "@angular/material/dialog";
import {DailySource} from "../../model/DailySource";
import {Work} from "../../model/Work";
import {DataService} from "../../services/data.service";
import {AdditionalCost, AdditionalCostTemplate} from "../../model/AdditionalCost";

@Component({
  selector: 'app-additional-cost-dialog',
  templateUrl: './additional-cost-dialog.component.html',
  styleUrl: './additional-cost-dialog.component.css'
})
export class AdditionalCostDialogComponent {

  workProdLine:string='';
  workCosts:AdditionalCost[]=[];
  isLoading = false;
  templates:AdditionalCostTemplate[]=[];
  loadTemplates(){
    if(this.data.work){
      this.data.dataService.AdditionalCostData.TemplatesListItem().subscribe(x=>{
        if(x!=null){
          this.templates = x;
        }
      })
    }else{
      this.data.dataService.AdditionalCostData.TemplatesListPost().subscribe(x=>{
        if(x!=null){
          this.templates = x;
        }
      })
    }

  }
  constructor(private dialogRef:MatDialogRef<AdditionalCostDialogComponent>,@Inject(MAT_DIALOG_DATA)
  public data: {work:Work, dataService:DataService, postId:string}) {
    this.loadTemplates();
  }
  ok(){
      this.dialogRef.close(null);
  }
  create(templateId:number,desc:string,cost:number){
    this.isLoading=true;
    let ac = {comment:'',cost:cost,additionalCostTemplateId:templateId,description:desc, id:0, workId:this.data.work.structure.id};
    this.data.dataService.AdditionalCostData.CreateWorkAddCost(ac).subscribe(x=>{
      if(x!=null){
        this.isLoading=false;
        this.data.work.structure.additionalCosts.push(x);
      }
    });
  }
  addCost(template:AdditionalCostTemplate, desc:string,cost:number){
    this.workCosts.push({ cost:cost, additionalCostTemplateId: template.id, id:0, additionalCostTemplate: template, workId:0, description:desc, comment:''})

  }
  createPostWork(){
    this.isLoading=true;
    this.data.dataService.AdditionalCostData.CreatePostAddCost(this.data.postId, this.workProdLine, this.workCosts).subscribe(x=>{
      if(x){
        this.isLoading=false;
        //console.log(x);
        this.dialogRef.close(x);
      }
    })
  }

  protected readonly Number = Number;
}

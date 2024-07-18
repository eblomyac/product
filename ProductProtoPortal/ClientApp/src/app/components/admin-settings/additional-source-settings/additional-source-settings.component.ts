import { Component } from '@angular/core';
import {DataService} from "../../../services/data.service";
import {AdditionalCostTemplate} from "../../../model/AdditionalCost";

@Component({
  selector: 'app-additional-source-settings',
  templateUrl: './additional-source-settings.component.html',
  styleUrl: './additional-source-settings.component.css'
})
export class AdditionalSourceSettingsComponent {

  templates:AdditionalCostTemplate[] = [];
  constructor(private dataService:DataService) {
    this.loadTemplates();
  }
  loadTemplates(){
    this.dataService.AdditionalCostData.TemplatesList().subscribe(x=>{
      if(x!=null){
        this.templates = x;
      }
    });
  }
  createTemplate(name:string){
    let a = {name:name,canPost:false,canItem:false, id:0};
    this.templates.push(a);
  }
  saveTemplates(){
    this.dataService.AdditionalCostData.SaveTemplateList(this.templates).subscribe(x=>{
      if(x!=null){
        this.loadTemplates();
      }

    })
  }
}

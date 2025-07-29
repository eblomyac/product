import {Component, Inject} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialogRef} from "@angular/material/dialog";
import {OTKCheck, OTKCheckLine} from "../../model/OTKCheck";
import {DataService} from "../../services/data.service";
import {OTKWorker} from "../../model/OTKAvailableOperation";

@Component({
  selector: 'app-otk-check',
  templateUrl: './otk-check.component.html',
  styleUrl: './otk-check.component.css'
})
export class OtkCheckComponent {


  isLoading = false;
  workers:OTKWorker[] = [];
  allSelected = false;
  allValues=false;
  constructor(public dialogRef:MatDialogRef<OtkCheckComponent>,@Inject(MAT_DIALOG_DATA)
  public data: {template:OTKCheck,dataService:DataService, onlyView:boolean}) {
    this.data.dataService.OTK.WorkerList().subscribe(x=>{
      this.workers = x;
    })
  }
  saveCheck(){
    this.isLoading=true;
    this.data.dataService.OTK.SaveCheck(this.data.template).subscribe(x=>{
      if(x){
        this.dialogRef.close(x);
      }

    });
  }
  setTargetVal(line:OTKCheckLine,v:string){
    line.targetValue = v;

  }
  setVal(line:OTKCheckLine, v:string){
    line.measuredValue = v;

    this.valueInputed(line,v);
  }
  valueSelected(line:OTKCheckLine,value:string){
    this.allSelected = this.data.template.lines.length == this.data.template.lines.filter(x=>x.value.length>1).length;
  }
  valueInputed(line:OTKCheckLine,value:string){
    if(value == line.targetValue){
      line.value='Брака нет';
      this.valueSelected(line,'Брака нет')
    }else{
      line.value='Брак есть';
      this.valueSelected(line,'Брак есть')
    }
    this.allValues = this.data.template.lines.filter(x=>x.targetValue.length>0).length == this.data.template.lines.filter(x=>x.measuredValue.length>0).length;
  }

}

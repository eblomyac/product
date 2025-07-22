import {Component, EventEmitter, Input, Output} from '@angular/core';
import {ProductWorker} from "../../../model/Hr";
import {DataService} from "../../../services/data.service";

@Component({
  selector: 'app-hr-settings',
  templateUrl: './hr-settings.component.html',
  styleUrl: './hr-settings.component.css'
})
export class HrSettingsComponent {
  @Input() Workers: ProductWorker[]=[];

  @Output() Saving = new EventEmitter<boolean>();


  constructor(private dataService: DataService) {

  }

  Save(){
    this.Saving.emit(true);
    this.dataService.HR.SaveWorkerList(this.Workers).subscribe(r=>{
      if(r){
        this.Workers = r;
        console.log(this.Workers);
      }
    }, err => {}, ()=>this.Saving.emit(false));
  }
  addWorker(name:string, title:string){
    let ew = this.Workers.find(z=>z.name.toLowerCase() == name.toLowerCase());
    if(ew == null){
      ew = { name:name , id:this.Workers.length+1, targets:[], title:title, isActive:true};
      this.Workers.push(ew );
    }
  }

}

import { Component } from '@angular/core';
import {DataService} from "../../../services/data.service";
import {OTKCheck} from "../../../model/OTKCheck";
import {DialogHandlerService} from "../../../services/dialog-handler.service";
import {OtkCheckComponent} from "../../../dialogs/otk-check/otk-check.component";

@Component({
  selector: 'app-otk-view',
  templateUrl: './otk-view.component.html',
  styleUrl: './otk-view.component.css'
})
export class OtkViewComponent {
  filter:any={
    from:null,
    to:null,
    orderNumber:null,
    article:null,
    worker:null,
    result:null,
    offset :0
  }
  workers:any[]=[];
  checks:OTKCheck[]=[];

  isLoadingData:boolean=false;
  constructor(public dataService:DataService) {
    dataService.OTK.WorkerList().subscribe(workerList=>{
      this.workers=workerList;
    })
    this.load();
  }
  load(){
    this.isLoadingData=true;
    this.checks=[];
    this.dataService.OTK.OtkChecks(this.filter).subscribe(x=>{

      this.checks=x;
    }, error => {}, ()=>{this.isLoadingData=false});
  }
  async view(c:OTKCheck){
    let otk = await DialogHandlerService.Singleton.ask(OtkCheckComponent, {data: {template:c,dataService:this.dataService, onlyView:true}, height:'95%', width:'95%', disableClose:true});
  }
  clear(){
    this.checks=[];

    this.filter = {
      from:null,
      to:null,
      orderNumber:null,
      articleId:null,
      otkWorker:null,
      checkResult:null,
      offset :0
    }
    this.load();
  }
}

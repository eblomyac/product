import {Component, OnInit} from '@angular/core';
import {DataService} from "../../../services/data.service";
import {OTKAvailableOperation, OTKTargetValue, OTKWorker} from "../../../model/OTKAvailableOperation";
import {MatMenu, MatMenuTrigger} from "@angular/material/menu";

@Component({
  selector: 'app-otk-operations',
  standalone: false,
  templateUrl: './otk-operations.component.html',
  styleUrl: './otk-operations.component.css'
})
export class OtkOperationsComponent implements OnInit {

  loading: boolean = false;
  workers:OTKWorker[]=[];
  operations: OTKAvailableOperation[] = [];
  targetValues: OTKTargetValue[] =[];
  newOperation: OTKAvailableOperation = {id:0, shortName:'',fullName:'', productLine: '', productLines:[], targetValue:'', values:[],availableValues:'', availableTargetValues:'',targetValues:[]};
  constructor(private dataService:DataService){

  }
  ngOnInit() {
    this.loadOperations();
    this.loadWorkers();
    this.loadTargetValues();
  }

  addWorker(e:any){
    let existWorker = this.workers.find(x=>x.name == e.value);
    if(!existWorker){
      this.workers.push({id:0, name:e.value});
    }
   console.log(e);
    e.input.value = '';
  }
  loadTargetValues(){
    this.loading=true;
    this.targetValues=[];
    this.dataService.OTK.TargetValues().subscribe(x=>{
      this.loading =false;
      this.targetValues=x;
    });
  }

  loadOperations(){
    this.loading = true;
    this.dataService.OTK.OperationsList().subscribe(x=>{
      this.loading = false;
      this.operations = x;
    });
  }
  loadWorkers(){
    this.loading = true;
    this.dataService.OTK.WorkerList().subscribe(x=>{
      this.loading = false;
      this.workers = x;
    })
  }
  removeWorker(w:OTKWorker){
    this.workers.splice(this.workers.indexOf(w), 1);
  }
  saveWorkers(){
    this.loading = true;
    this.dataService.OTK.SaveWorkers(this.workers).subscribe(x=>{
      this.loading = false;
      this.workers = x;
    })
  }
  remove(op:OTKAvailableOperation){
    this.operations.splice(this.operations.indexOf(op), 1);
  }
  saveOperations(){
    this.loading = true;
    this.operations.forEach(z=>{
      z.availableValues = z.values.join(';');
      z.availableTargetValues = z.targetValues.join(';');
    })
    this.dataService.OTK.SaveOperations(this.operations).subscribe(x=>{
      this.operations = x;
      this.loading=false;
    })
    this.saveWorkers();
  }

  add(){

    this.newOperation.productLine = this.newOperation.productLines.join(', ');
    this.operations.push(this.newOperation);
    this.newOperation = {id:0, shortName:'',fullName:'', productLine: '', productLines:[], targetValue:'', values:[], availableValues:'', availableTargetValues:'', targetValues:[]};
  }
  addValue(operation:OTKAvailableOperation, val:string, menu:MatMenuTrigger, inputText:any){
    if(operation.values == null){
      operation.values = [];
    }
    let exist = operation.values.find(z=>z == val);
    if(!exist){
      operation.values.push(val);
    }
   // menu.closeMenu();
    inputText.value = '';

  }
  addTargetValue(operation:OTKAvailableOperation, val:string, menu:MatMenuTrigger, inputText:any){
    if(operation.targetValues == null){
      operation.targetValues = [];
    }
    let exist = operation.targetValues.find(z=>z == val);
    if(!exist){
      operation.targetValues.push(val);
    }
    // menu.closeMenu();
    inputText.value = '';
  }
  removeValue(operation:OTKAvailableOperation, index:number){
        operation.values.splice(index,1);
  }
  removeTargetValue(operation:OTKAvailableOperation, index:number){
    operation.targetValues.splice(index,1);
  }



}

import {Component, EventEmitter, Input, OnChanges, Output, SimpleChanges} from '@angular/core';
import {ProductTarget, ProductWorker} from "../../../model/Hr";
import {IPost} from "../../../model/Post";
import {DataService} from "../../../services/data.service";

@Component({
  selector: 'app-product-workers',
  templateUrl: './product-workers.component.html',
  styleUrl: './product-workers.component.css'
})
export class ProductWorkersComponent {

  @Input() Workers:ProductWorker[]=[];
  @Input() Posts:string[] = [];
  @Input() PostCrpData:{[post:string]:{
    [crpPost:string]:{sub:Array<{crpPost:string, desc:string}>, desc:string}
  }}={};
  @Output() Saving = new EventEmitter<boolean>();
  objectKeys = Object.keys;
  public newRole:ProductTarget = { productWorkerId:0, targetName:'', postId:'',id:0, targetCrpPost:'', targetCrpPostDescription:'', targetCrpCenterDescription:'', targetCrpCenter:''};

  filterName:string='';

  tmp:string='';
  tmp2:{desc:string,crpPost:string}={desc:'',crpPost:''};
  constructor(private dataService: DataService) {
  }

  activeWorkers():ProductWorker[]{
    if(this.filterName.length>0){
      return this.Workers.filter(z=>z.isActive && z.name.toLowerCase().includes(this.filterName.toLowerCase()));
    }else{
      return this.Workers.filter(z=>z.isActive);
    }

  }

  crpCenterChange(event:string, d:any){
    this.newRole.targetCrpCenter = event;
    console.log(d);
    this.newRole.targetCrpCenterDescription = d.desc;
    this.updateRoleTargetName();
  }
  crpPostChange(event:any){
    this.newRole.targetCrpPostDescription = event.desc;
    this.newRole.targetCrpPost = event.crpPost;
    this.updateRoleTargetName();
  }
  updateRoleTargetName(){
    this.newRole.targetName = `${this.newRole.targetCrpCenter} ${this.newRole.targetCrpPost} ${this.newRole.targetCrpPostDescription}`;
  }

  get q():string[]{
    let a:string[]=[];
    this.Workers.forEach((z,i)=>{
      a.push((i+1).toString());
    })
    return a;
  }
  drop( w:ProductWorker,event: any) {
    let exist = w.targets.find(z=>z.targetName == this.newRole.targetName && z.postId == this.newRole.postId);
    if(exist==null){
      this.newRole.productWorkerId = w.id;
      w.targets.push(this.newRole);
      w.targets.sort((a,b)=>a.postId.localeCompare(b.postId));
      this.newRole =  { productWorkerId:0, targetName:'', postId:'',id:0, targetCrpPost:'', targetCrpPostDescription:'', targetCrpCenterDescription:'', targetCrpCenter:''};
      this.updateRoleTargetName();
      this.tmp='';
      this.tmp2={desc:'',crpPost:''};
    }

  }
  remove(w:ProductWorker,i:number) {
      w.targets.splice(i,1);
  }
  Save(){
    this.Saving.emit(true);
    this.dataService.HR.SaveTargetList(this.Workers).subscribe(r=>{
      if(r){
        this.Workers = r;
        console.log(this.Workers);
      }
    }, err => {}, ()=>this.Saving.emit(false));
  }

}

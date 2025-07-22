import {Component, OnInit} from '@angular/core';
import {DataService} from "../../services/data.service";
import {ProductTarget, ProductWorker} from "../../model/Hr";
import {IPost} from "../../model/Post";

@Component({
  selector: 'app-hr',
  templateUrl: './hr.component.html',
  styleUrl: './hr.component.css'
})
export class HrComponent implements OnInit {

  workers:ProductWorker[]=[];
  activeTabInex=0;


  postTargetsCrp:{[post:string]:{
    [crpPost:string]:{sub:Array<{crpPost:string, desc:string}>, desc:string}
    }}={};
  posts:string[]=[];
  isLoading:boolean = false;

  constructor(private dataService:DataService) {

  }

  saving(e:boolean){
    this.isLoading = e;
  }
  ngOnInit() {
    this.loadWorkers();
    this.loadPosts();

  }
  loadPosts(){


    this.postTargetsCrp={};
    this.posts = [];
    this.dataService.HR.TargetList().subscribe(x=>{
      if(x){
      //  this.targets = x;
        x.forEach(item=>{

          item.targets.forEach(target=>{
            if(!this.postTargetsCrp[item.postId]){
              this.postTargetsCrp[item.postId]={}
            }
            if(!this.postTargetsCrp[item.postId][target.targetCrpCenter]) {
              this.postTargetsCrp[item.postId][target.targetCrpCenter] = {sub:[],desc: target.targetCrpCenterDescription}
            }
            this.postTargetsCrp[item.postId][target.targetCrpCenter].sub.push({desc:target.targetCrpPostDescription, crpPost:target.targetCrpPost});
          })

          this.posts.push(item.postId)
        });
      console.log(this.postTargetsCrp);
      }
    })
  }
  loadWorkers(){
      this.workers=[];
      this.dataService.HR.WorkerList().subscribe(x=>{
        if(x){
          this.workers=x;
        }
      });
  }

}

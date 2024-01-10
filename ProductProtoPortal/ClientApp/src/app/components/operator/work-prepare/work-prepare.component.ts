import { Component, OnInit } from '@angular/core';
import {DataService} from "../../../services/data.service";
import {IPost} from "../../../model/Post";
import {IWork, Work} from "../../../model/Work";



@Component({
  selector: 'app-work-prepare',
  templateUrl: './work-prepare.component.html',
  styleUrls: ['./work-prepare.component.css']
})
export class WorkPrepareComponent implements OnInit {
  isLoading=false;
  orderId:number=0;
  preparedWorks:Work[]=[];
  editMode=false;
  posts:IPost[]=[];
  constructor(private data:DataService) {
    this.data.Post.List().subscribe(x=>{
      if(x){
        this.posts = x;
      }
    });
  }

  remove(workIndex:number){
    this.preparedWorks.splice(workIndex,1);
  }
  ngOnInit(): void {
  }
  clear(){
    this.preparedWorks=[];
  }
  public Create(){
    let badWorks = this.preparedWorks.filter(x=>x.structure.postId.length<1);
    if(badWorks.length>0){
      let a = confirm('Обнаружены работы без указанного поста, они будут проигнорированы, продолжить?');
      if(!a){
        return;

      }
    }
    let createWorks = this.preparedWorks.filter(x=>x.structure.postId.length>0);
    this.isLoading=true;
    this.data.Work.Create(createWorks).subscribe(x=>{
      if(x){
        this.isLoading=false;
        this.clear();
      }
    });

  }
  public Prepare(){
    this.isLoading=true;
    this.data.Work.Prepare([this.orderId]).subscribe(x=>{
      console.log('..');
      if(x){
        this.isLoading=false;
        this.preparedWorks.unshift(...x);
        this.orderId=0;
      }
    });
  }
}

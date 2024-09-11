import { Component } from '@angular/core';
import {IWork, Work} from "../../../model/Work";
import {IPost} from "../../../model/Post";
import {DataService} from "../../../services/data.service";

@Component({
  selector: 'app-operator-starter',
  templateUrl: './operator-starter.component.html',
  styleUrl: './operator-starter.component.css'
})
export class OperatorStarterComponent {

  isLoading=false;
  orderId:number=0;

  editMode=false;
  posts:IPost[]=[];

  errorWorks:IWork[]=[];
  workLines:any[]=[];
  constructor(private data:DataService) {
    this.data.Post.List().subscribe(x=>{
      if(x){
        this.posts = x;
      }
    });
  }
  clear(){

  }
  public Start(){
    this.isLoading = true;
    this.data.Work.StartWorks(this.workLines).subscribe(x=>{
        if(x){
          this.isLoading=false;
          alert('Запущено ' + x.length + ' работ');

          this.errorWorks=[];
          this.workLines=[];
        }
    });
  }
  public Prepare(){
    this.isLoading=true;
    this.errorWorks = [];
    this.workLines=[];
    this.data.Work.PrepareNew([this.orderId]).subscribe(x=>{
      console.log('..');
      if(x){
        console.log(x);
        this.isLoading=false;
        this.errorWorks = x.errorResult;
        this.workLines = x.result;

        this.orderId=0;
      }
    });
  }
}

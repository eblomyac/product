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

  ignoreStarted=true;
  isLoading=false;
  orderId:number=0;
  resultString='';

  editMode=false;
  posts:IPost[]=[];

  errorWorks:IWork[]=[];
  workLines:any[]=[];
  filteredWorkLines:any[]=[];
  suggestOrders:number[] =[];
  constructor(private data:DataService) {
   /* this.data.Statistic.MaconomyCompareOrders().subscribe(x=>{
      if(x){
        this.suggestOrders = x;
      }
    }); /*/
    this.data.Post.List().subscribe(x=>{
      if(x){
        this.posts = x;
      }
    });
  }
  clear(){

  }
  filterLines(){
    if(this.ignoreStarted){
      this.filteredWorkLines = this.workLines.filter(z=>z.startedDate ==null);
    }else{
      this.filteredWorkLines = this.workLines;
    }
  }
  public Start(){
    this.isLoading = true;
    this.data.Work.StartWorks(this.filteredWorkLines).subscribe(x=>{
        if(x){
          this.isLoading=false;
          if(x.length>0){
            this.resultString = 'Запущено ' + x.length + ' работ';
          }else{
            this.resultString = 'Работы не запущены';
          }
          setTimeout(()=>{this.resultString=''},3000)


          this.errorWorks=[];
          this.workLines=[];
          this.filterLines();
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
        this.filterLines();
        this.orderId=0;
      }
    });
  }
}

import { Component } from '@angular/core';
import {DataService} from "../../../services/data.service";
import {IWork} from "../../../model/Work";
import {H} from "@angular/cdk/keycodes";

@Component({
  selector: 'app-count-changer',
  templateUrl: './count-changer.component.html',
  styleUrl: './count-changer.component.css'
})
export class CountChangerComponent {

  orderNumber=0;
  lineNumber=0;
  hideEnded=false;

  isLoading=false;

  article:string='';
  currentCount =0;
  postData:any[]=[];
  lastChangeResult:any[]=[];
  postDataSource :Array<{workId:number,count:number, postId:string,status:number, changeResult:string}>=[];

  constructor(private dataService:DataService) {
  }

  TotalCount(postName:string):number{
    let postWorks = this.postData.find(x=>x.PostName ==postName);
    console.log(postWorks);
    if(postWorks){
      return postWorks.Works.reduce((a:number,b:any)=>a+b.count,0);
    }else{
      return 0;
    }

  }
  TotalCountStr(postName:string):string{
    let postWorks = this.postData.find(x=>x.PostName ==postName);
    //console.log(postWorks);
    if(postWorks){
      let ended= postWorks.Works.filter((w:any)=>w.status==50).reduce((a:number,b:any)=>a+b.count,0);
      let notEnded = postWorks.Works.filter((w:any)=>w.status!=50).reduce((a:number,b:any)=>a+b.count,0);
      return `Завершено ${ended} , не завершено ${notEnded}`;
    }else{
      return '';
    }
  }
  TotalSourceCountStr(postName:string):string{
   // console.log(this.postDataSource);
    let postWorks = this.postDataSource.filter(x=>x.postId ==postName);
    //console.log(postWorks);
    if(postWorks){
      let ended= postWorks.filter((w:any)=>w.status==50).reduce((a:number,b:any)=>a+b.count,0);
      let notEnded = postWorks.filter((w:any)=>w.status!=50).reduce((a:number,b:any)=>a+b.count,0);
      return `Завершено ${ended} , не завершено ${notEnded}`;
    }else{
      return '';
    }
  }
  loadWorks(){
    this.postData = [];
    this.postDataSource = [];
    this.currentCount=0;
    this.isLoading=true;
      this.dataService.OperatorData.WorkList(this.orderNumber, this.lineNumber).subscribe(x=>{
        if(x){
          this.isLoading=false;
          //console.log(x);
          this.postData = x.Posts;
          this.article = x.Article;
          this.currentCount = x.CurrentCount;
          this.postDataSource = this.postData.map(x=>x.Works.map((z:any)=>{return {workId:z.id,count:z.count, postId:z.postId, status:z.status, changeResult:''}})).flat();
         // console.log(this.postDataSource);

        }
      });
  }
  getSourceCount(workId:number):number{
    return this.postDataSource.find(x=>x.workId==workId)!.count;
  }
  getChangeResult(workId:number):string{
    let last = this.lastChangeResult.find(x=>x.workId == workId);
    if(last){
      return `Изменение с ${last.oldCount} на ${last.newCount}: ${last.result}`
    }else{
      return '';
    }

  }

  saveChanges(){
    let data:any[]=[];
    this.postData.forEach(pd=>{
      pd.Works.forEach((w:any)=>{
        let changed = this.postDataSource.find(x=>x.workId == w.id);
        if(changed && changed.count!= w.count){
          data.push({workId:w.id, result:'',newCount:w.count, oldCount:changed.count});
        }
      })

    });
    this.isLoading = true;
    this.dataService.OperatorData.ChangeCount(data).subscribe(x=>{
      this.isLoading =false;
      if(x){

        this.lastChangeResult = [];
        x.forEach(z=>{
          this.lastChangeResult.push({workId:z.workId,result:z.result, oldCount:z.oldCount, newCount:z.newCount})
        });
        this.loadWorks();
      }

    });
  }
}

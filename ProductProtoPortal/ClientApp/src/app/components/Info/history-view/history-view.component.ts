import { Component } from '@angular/core';
import {DataService} from "../../../services/data.service";
import {MatFormFieldModule} from "@angular/material/form-field";
import {DatePipe} from "@angular/common";
import {IPost} from "../../../model/Post";
import {IUser} from "../../../model/User";
import {Transfer} from "../../../model/Transfer";
import {DialogHandlerService} from "../../../services/dialog-handler.service";
import {TransferListComponent} from "../../../dialogs/transfer-list/transfer-list.component";

@Component({
  selector: 'app-history-view',
  templateUrl: './history-view.component.html',
  styleUrl: './history-view.component.css'
})
export class HistoryViewComponent {

  from:Date = new Date();
  to:Date = new Date();
  article:string = '';
  userBy:string ='';
  order:number = 0;
  postId:string = '';


  postList:IPost[]=[];
  userList:IUser[]=[];
  histories:any[]=[];


  constructor(private data:DataService, private datePipe:DatePipe) {
    this.from=new Date();
    this.from.setDate(this.to.getDate()-3);
    this.load();
    this.loadOnce();
  }
  loadOnce(){
    this.data.User.List().subscribe(x=>{
      if(x!=null){
        this.userList = x;
      }
    });
    this.data.Post.List().subscribe(x=>{
      if(x!=null){
        this.postList = x;
      }
    });
  }
  view(transfer:Transfer){
    DialogHandlerService.Singleton.ask(TransferListComponent,{data: {
        dataService:this.data,
        type:'out',
        transfers:[transfer],
        viewItem:transfer
      },
      autoFocus: true,
      hasBackdrop: true,
      disableClose:true
    })

  }
  download(){
    this.data.InfoData.HistoryDownload(this.datePipe.transform(this.from, 'dd.MM.yyyy', "UTC+03","ru-RU")!,
      this.datePipe.transform(this.to, 'dd.MM.yyyy', "UTC+03","ru-RU")!,
      this.userBy,this.postId, this.article, this.order).subscribe(x=>{
      if(x!=null){
        window.open(x,'_blank');
      }
    })
  }
  load(){


    this.data.InfoData.History(this.datePipe.transform(this.from, 'dd.MM.yyyy', "UTC+03","ru-RU")!,
      this.datePipe.transform(this.to, 'dd.MM.yyyy', "UTC+03","ru-RU")!,
      this.userBy,this.postId, this.article, this.order).subscribe(x=>{
      if(x!=null){
        this.histories = x;
      }else{
        this.histories = [];
      }
    })
  }
}

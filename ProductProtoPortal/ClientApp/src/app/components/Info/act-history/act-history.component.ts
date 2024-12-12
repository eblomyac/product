import {Component, OnInit} from '@angular/core';
import {Transfer} from "../../../model/Transfer";
import {DataService} from "../../../services/data.service";
import {IPost} from "../../../model/Post";
import {TransferListComponent} from "../../../dialogs/transfer-list/transfer-list.component";
import {DialogHandlerService} from "../../../services/dialog-handler.service";
import {ActivatedRoute} from "@angular/router";

@Component({
  selector: 'app-act-history',
  templateUrl: './act-history.component.html',
  styleUrl: './act-history.component.css'
})
export class ActHistoryComponent implements OnInit{


  filter:any={
    from:null,
    to:null,
    orderNumber:null,
    articleId:null,
    fromPost:null,
    toPost:null
  }
  isLoadingData=false;
  isLoadingPost = false;
  offset=0;
  transfers:Transfer[]=[];
  postList:IPost[]=[];
  ngOnInit() {
    this.isLoadingPost = true;
    this.data.Post.List().subscribe(x=>{
      if(x){
        this.postList = x;
        this.isLoadingPost=false;
      }
    })
    this.load();
  }

  constructor(private data:DataService,private activatedRoute: ActivatedRoute) {
    this.activatedRoute.queryParams.subscribe(z=>{
      let transferId = z['transferId'];
      if(transferId!=null && transferId.length>0){
        this.loadById(Number(transferId));
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
  print(transfer:Transfer){
    window.open('/transfer/'+transfer.id+'/print');
  }
  clear(){
    this.filter={
      from:null,
      to:null,
      orderNumber:null,
      articleId:null,
      fromPost:null,
      toPost:null
    }
  }
  loadById(id:number){
    this.data.TransferData.byId(id).subscribe(z=>{
      if(z){
        DialogHandlerService.Singleton.ask(TransferListComponent,{data: {
            dataService:this.data,
            type:'out',
            transfers:[z],
            viewItem:z
          },
          autoFocus: true,
          hasBackdrop: true,
          disableClose:true
        })
      }
    })
  }
  load(){
    this.isLoadingData=true;
    this.data.TransferData.list(this.offset,this.filter).subscribe(x=>{
      if(x){
        this.transfers = x;
        this.isLoadingData=false;
      }
    });
  }
}

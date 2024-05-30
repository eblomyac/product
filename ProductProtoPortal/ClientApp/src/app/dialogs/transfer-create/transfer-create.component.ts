import {Component, Inject} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialogRef} from "@angular/material/dialog";
import {IPost} from "../../model/Post";
import {Work} from "../../model/Work";
import {TransportService} from "../../services/transport.service";
import {DataService} from "../../services/data.service";
import {MatSelectModule} from "@angular/material/select";

@Component({
  selector: 'app-transfer-create',
  templateUrl: './transfer-create.component.html',
  styleUrl: './transfer-create.component.css'
})
export class TransferCreateComponent {
  isLoading=false;
  works:Array<Work> = [];
  dataService:DataService;
  posts:IPost[]=[];
  toPost:IPost|undefined;
  selectedWorks:Array<Work>=[];
  currentPost:string='';
  movableWorks:Array<Work>=[];
  artFilter = "";
  orderFilter="";
  applyFilter(){
    this.movableWorks = this.data.availableWorks.filter(x=>{
      let r = true;
      if(this.artFilter.length>0){
        r = r&& x.structure.article.indexOf(this.artFilter)>=0;
      }
      if(this.orderFilter.length>0){
        r = r&& x.structure.orderNumber.toString().indexOf(this.orderFilter)>=0;
      }
      return r;
    });
  }
  constructor(private dialogRef:MatDialogRef<TransferCreateComponent>,@Inject(MAT_DIALOG_DATA)
  public data: {sourcePost:string, availableWorks: Array<Work>, dataService:DataService})
  {
    this.currentPost = data.sourcePost;
    this.dataService = data.dataService;
    this.works = data.availableWorks;
    this.movableWorks = data.availableWorks;
    //this.loadWorksDestination();
    this.loadPosts();
  }
  loadPosts(){
    this.dataService.Post.List().subscribe(x=>{
      this.posts = x;
    })
  }
  loadWorksDestination(){
    this.isLoading=true;
    this.dataService.Work.LoadSuggestions(this.works).subscribe(x=>{
      if(x){
        this.isLoading=false;
      }

    })


  }

  selectWork(w:Work, e:any){


    if(e){
      //добавить
      if(!this.isSelected(w)){
        this.selectedWorks.push(w)
      }
    }else{
      //убрать
      if(this.isSelected(w)){
        let delIndex = this.selectedWorks.findIndex(z=>z.structure.id == w.structure.id);
        this.selectedWorks.splice(delIndex,1);
      }
    }
  }
  isSelected(w:Work):boolean{
    return this.selectedWorks.findIndex(z=>z.structure.id == w.structure.id)>-1;
  }
  filterWorks(){
      this.orderFilter='';
      this.artFilter='';
      this.movableWorks=this.data.availableWorks;
      this.selectedWorks=[];


  }
  create(){

    if(this.toPost){
    this.isLoading=true;
      this.dataService.TransferData.createTransfer(this.currentPost, this.toPost.name, this.selectedWorks.map(x=>x.structure)).subscribe(x=>{
        if(x){

          this.isLoading=false;
          this.dialogRef.close(x);
        }

      })

    }

  }
  cancel(){
    this.dialogRef.close(null);
  }

}

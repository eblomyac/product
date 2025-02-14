import {Component, Inject, Injector, Input, NgZone, OnChanges, OnDestroy, OnInit, SimpleChanges} from '@angular/core';

import {ActivatedRoute} from "@angular/router";
import {firstValueFrom, lastValueFrom, Subscription} from "rxjs";

import {DataService} from "../../../services/data.service";
import {Line, TechCard} from "../../../model/TechCard";
import {MAT_DIALOG_DATA, MatDialog, MatDialogRef} from "@angular/material/dialog";
import {DialogHandlerService} from "../../../services/dialog-handler.service";
import {DailySourceDialogComponent} from "../../../dialogs/daily-source-dialog/daily-source-dialog.component";
import {PostDialogComponent} from "../../../dialogs/post-dialog/post-dialog.component";
import {PhotoUploadComponent} from "../../../dialogs/photo-upload/photo-upload.component";
import {PictureViewComponent} from "../../../dialogs/picture-view/picture-view.component";

@Component({
  selector: 'app-card-view',
  templateUrl: './card-view.component.html',
  styleUrls: ['./card-view.component.css']
})
export class CardViewComponent implements OnInit, OnDestroy, OnChanges {

  @Input('Article') inputArticle: string = '';

  card: TechCard | null = null;
  selectedPost = '';
  postView: Line[] = [];
  isDialog = false;

  isLoadCard = false;
  isLoadRecipe = false;
  isLoadPartOf = false;
  composition: any = null;
  partOf: any = null;
  showUrl: string | null = null;

  set Article(value: string) {
    this.partName = value;
    if(value!=null && value.length>0){
      this.load();
    }

  }

  partName: string = '';
  public dialogRef: MatDialogRef<any> | null = null;
  private dialogData;

  constructor(private injector: Injector, private data: DataService, private dialogService: MatDialog) {

    let dialog = this.injector.get(MatDialogRef, null);
    if (dialog) {
      this.isDialog = true;
      this.dialogRef = dialog;
      this.dialogData = this.injector.get(MAT_DIALOG_DATA, null);
      if (this.dialogData && this.dialogData.partName) {
        this.Article = this.dialogData.partName;
      }

      // console.log('card-view ' +this.Article+ ' dialog');
    } else {

      this.Article = this.inputArticle;

      //console.log('card-view ' +this.Article+ ' comp');

    }


  }

  showPicture(url: string) {
    this.dialogService.open(PictureViewComponent, {
      data: {
        img: url,
      },


      autoFocus: true,
      hasBackdrop: true,


    });

  }

  ngOnChanges(changes: SimpleChanges): void {
    if (!this.isDialog) {
      if(changes.inputArticle.currentValue != changes.inputArticle.previousValue){
        this.Article = changes.inputArticle.currentValue;
      }


    }

  }
  async addPhoto(){
      let x = this.dialogService.open(PhotoUploadComponent, {
        data: {
          article:this.card?.article,
          dataService:this.data
        },


        autoFocus: true,
        hasBackdrop: true,



      });
      let v = await firstValueFrom(x.afterClosed());
      if(v!=null){
        if(v){
          this.load();
        }
      }
      return v;

    }
  ngOnInit(): void {
  }
  ngOnDestroy() {

  }

  updatePostParts(){
    if(this.card){
      this.postView = this.card.postParts.find(z=>z.postId == this.selectedPost)!.lines;
    }else{
      this.postView = [];
    }

  }
  download_partOf(){
    this.data.TechCard.PartItemOf(this.partName,"xlsx").subscribe(x=>{
      if(x){
       window.open(x.link,"_blank")
      }
    })
  }

  load(){
      //console.log(this.partName);
    this.card=null;
    this.isLoadCard=true;
    this.isLoadRecipe = true;
    this.isLoadPartOf = true;
    this.data.TechCard.Card(this.partName).subscribe(x=>{

      if(x!= 'load'){
        this.isLoadCard=false;
        this.card = x;
        if(x && x.postParts.length>0){
          this.selectedPost = x.postParts[0].postId;
        }
        this.updatePostParts();
      }
      if(x=='load'){
        this.isLoadCard=true;
      }
    })
    this.data.TechCard.Composition(this.partName).subscribe(x=>{

      if(x && x!= 'load'){
        this.isLoadRecipe=false;
        this.composition = x;
      }
      if(x=='load'){
        this.isLoadRecipe=true;
      }
    })
    this.data.TechCard.PartItemOf(this.partName,"json").subscribe(x=>{

      if(x && x!= 'load'){
        this.isLoadPartOf=false;
        this.partOf = x;
      }
      if(x=='load'){
        this.isLoadPartOf=true;
      }
    })
  }

}

import {Inject, Injector, INJECTOR} from "@angular/core";
import {DataService} from "../services/data.service";

import {PostDialogComponent} from "../dialogs/post-dialog/post-dialog.component";
import {firstValueFrom} from "rxjs";
import {WorkEventService} from "../services/work-event.service";

import {DialogHandlerService} from "../services/dialog-handler.service";
import {NumberDialogComponent} from "../dialogs/number-dialog/number-dialog.component";
import {Issue} from "./Issue";
import {IssueCreateDialogComponent} from "../dialogs/issue-create-dialog/issue-create-dialog.component";
import {CardViewComponent} from "../components/TechCard/card-view/card-view.component";

export interface IWork {
  id: number;
  orderNumber: number;
  article: string;
  postId: string;
  singleCost: number;
  count: number;
  totalCost: number;
  status: number;
  statusString: string;
  description: string;
  productLine: string;
  createdStamp: Date;
  forwardMoves: string[];
  backwardMoves: string[];
  movedFrom:string;
  movedTo:string;
  issues:Issue[];
  comments:string[];
  commentMap:string;
}

export class Work {
  isLoading = false;
  isSaving = false;
  public structure: IWork;
  private workEventService: WorkEventService = WorkEventService.Singleton;

  constructor(struct: IWork, private dataService: DataService,) {
    this.structure = struct;

    this.loadSuggestions();
    this.loadIssues();
  }
  loadIssues(){
    this.isLoading=true;
    this.dataService.Work.Issues(this.structure.id).subscribe(x=>{
      if(x){
        this.isLoading=false;
        this.structure.issues = x;
      }
    })
  }
  loadSuggestions() {
    if (this.structure.status == 10 || this.structure.status == 40) {


      this.isLoading = true;
      this.dataService.Work.LoadSuggestions([this]).subscribe(x => {
        if (x) {
          this.isLoading = false;
        }


      });
    }
  }

  getActions(): string[] {
    let result: string[] = [];
    if (this.structure) {

      if (this.structure.status < 40) {
        result.push('Разделить');

      }
      if (this.structure.status == 10) {
        if(this.structure.backwardMoves!=null && this.structure.backwardMoves.length>0) {
          result.push('Вернуть');
        }
        result.push('Принять')
      }
      if (this.structure.status == 20) {
        result.push('Выполнять');
        result.push('Зарегистрировать событие');
      }
      if (this.structure.status == 30) {
        result.push('Зарегистрировать событие');
        result.push('Приостановить');
        result.push('Завершить');

      }
      if (this.structure.status == 40 && (this.structure.movedTo==null||this.structure.movedTo.length<1)) {
        result.push('Передать на пост');
      }
      result.push('Открыть тех. карту');

    }
    return result;

  }

  async runAction(action: string) {
    switch (action) {
      case 'Вернуть':
        //this.changeStatus(0);
        this.moveToPosts();
        break;
      case 'Зарегистрировать событие':
        this.registerIssue();
        break;
      case 'Выполнять':
        this.changeStatus(30);
        break;
      case 'Приостановить':
        this.changeStatus(20)
        break;
      case 'Завершить':
        this.changeStatus(40);
        break;
      case 'Принять':
        this.changeStatus(20);
        break;
      case 'Разделить':
        this.splitWork();
        break;
      case 'Передать на пост':
        this.moveToPosts();
        break;
      case 'Открыть тех. карту':
        this.techCard();
        break;
    }
  }

  async moveToPosts() {
    let p: string[] = this.structure.status == 40 ? this.structure.forwardMoves : this.structure.backwardMoves;
    let posts = await DialogHandlerService.Singleton.ask(PostDialogComponent, {data: {posts: p, onlyMain: this.structure.status==10}});
    console.log(posts);
    if(!posts){return;}
    let mainPost = posts.main;
    let additionalPosts = posts.additional;
    if(additionalPosts==null || additionalPosts.length==0){
      additionalPosts = [];
    }
    if (mainPost && mainPost.length > 0) {
      this.isLoading = true;
      let data = {mainPost:mainPost, additional:additionalPosts};
      this.dataService.Work.Move(this.structure.id, data).subscribe(x => {
        if (x != null) {
          this.isLoading = false;
          this.structure.movedTo=mainPost;
        }

      });
    }
  }
  resolveIssue(issue:Issue){
    this.isLoading=true;
    this.dataService.Issue.Resolve(issue.id).subscribe(x=>{
      if(x){
        this.isLoading=false;
        this.loadIssues();
      }
    });
  }
  async registerIssue(){
    let issue = await DialogHandlerService.Singleton.ask(IssueCreateDialogComponent, {data: {}});
    if(issue){
      this.dataService.Issue.Register(issue,this.structure.id).subscribe(x=>{
        if(x){
          this.loadIssues();
        }
      });
    }
  }
  async techCard(){
    let tc = await DialogHandlerService.Singleton.ask(CardViewComponent, {
      data: {
        partName:this.structure.article
      }, height:'95%', width:'95%'
    })
  }

  async splitWork() {

    let splitCount = await DialogHandlerService.Singleton.ask(NumberDialogComponent, {
      data: {
        caption: 'Укажите отделяемое количество',
        currentValue: this.structure.count
      }
    })
    if (splitCount != null) {
      this.isLoading = true
      this.dataService.Work.Split(this, splitCount).subscribe(x => {
        if (x != null) {
          this.isLoading = false;
          let newWork = x.find(z => z.structure.id != this.structure.id);
          if (newWork) {
            this.workEventService.NewWorkBySplit(newWork);
          }

        }
      });
    }
  }


  canChangeStatus(newStatus: number): () => boolean {
    if(this.structure.issues.filter(x=>x.resolved==null).length>0 && !(newStatus==20 && this.structure.status==30)){
      alert('Перед сменой статуса нужно решить событие');
      return (): boolean => {
        return false;
      }
    }
    if (newStatus - this.structure.status > 10) {
      alert('Пропускать статусы нельзя');
      return (): boolean => {
        return false;
      }
    }
    if (this.structure.status == 40) {
      alert('Работа завершена');
      return (): boolean => {
        return false;
      }
    }
    if (this.structure.status == 20 && newStatus == 10) {
      alert('Принятую работу вернуть нельзя');
      return (): boolean => {
        return false;
      }
    }
    return (): boolean => {
      return true;
    }

  }

  updateStructure(){
    this.dataService.Work.View(this.structure.id).subscribe(x=>{
      if(x!=null){
        if(this.structure.status!= x.structure.status) {
          this.workEventService.WorkStatusChange(this, this.structure.status, x.structure.status, true);
        }
          if(this.structure.count != x.structure.count){
            this.workEventService.PossibleNewWork();
          }
          this.structure.count = x.structure.count;
          this.structure.status = x.structure.status;
          this.loadSuggestions();
        }

    });
  }

  changeStatus(to: number) {


    this.isLoading = true;
    this.isSaving = true;
    this.dataService.Work.ChangeStatus(this, to).subscribe(x => {
      if (x != null) {
        this.isLoading = false;
        this.isSaving = false;
        this.workEventService.WorkStatusChange(this, this.structure.status, to, x);
        if (x) {
          this.structure.status = to;
          this.loadSuggestions();
        }

      }

    });

  }
}

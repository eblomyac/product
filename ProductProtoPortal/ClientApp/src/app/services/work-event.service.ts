import { Injectable } from '@angular/core';
import {Work} from "../model/Work";
import {Observable, Subject} from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class WorkEventService {

  public static Singleton:WorkEventService;
  constructor() {
    WorkEventService.Singleton = this;
  }
  private workChangedStatus:Subject<{work:Work, from:number, to:number,isSuccess:boolean}> = new Subject<{work:Work,from:number, to:number,isSuccess:boolean}>();
  public OnWorkChangedStatus: Observable<{work:Work,from:number, to:number, isSuccess:boolean}> = this.workChangedStatus.asObservable();

  public possibleNewWork=new Subject<boolean>();
  public OnPossibleNewWork = this.possibleNewWork.asObservable();

  private newWorkBySplit:Subject<Work> = new Subject<Work>()
  public OnNewWorkBySplit:Observable<Work> = this.newWorkBySplit.asObservable();

  public WorkStatusChange(work:Work,from:number,to:number, isSuccess:boolean){
    //console.log(`Workstatus ${isSuccess} change: ${work.structure.id} from: ${from} to:${work.structure.status}`);
    this.workChangedStatus.next({work,from, to, isSuccess});
  }
  public NewWorkBySplit(work:Work){
    this.newWorkBySplit.next(work);
  }

  public PossibleNewWork(){
    this.possibleNewWork.next(true);
  }
}

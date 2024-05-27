import {Component, OnInit} from '@angular/core';
import {DataService} from "../../services/data.service";
import {SessionService} from "../../services/session.service";
import {IWork, Work} from "../../model/Work";
import {moveItemInArray, transferArrayItem} from "@angular/cdk/drag-drop";
import {Observable, firstValueFrom} from "rxjs";

import {PostDialogComponent} from "../../dialogs/post-dialog/post-dialog.component";
import {WorkEventService} from "../../services/work-event.service";
import {MatDialog} from "@angular/material/dialog";
import {Router} from "@angular/router";
import {TransferCreateComponent} from "../../dialogs/transfer-create/transfer-create.component";
import {Transfer} from "../../model/Transfer";
import {TransferListComponent} from "../../dialogs/transfer-list/transfer-list.component";

@Component({
  selector: 'app-post-view',
  templateUrl: './post-view.component.html',
  styleUrls: ['./post-view.component.css']
})
export class PostViewComponent implements OnInit {

  selectedPost = '';
  isLoading = false;
  allWorks: Work[] = [];
  filteredWorks: Work[] = [];
  incomeWorks: Work[] = [];
  waitWorks: Work[] = [];
  runningWorks: Work[] = [];
  endedWorks: Work[] = [];

  incomeTransfers:Transfer[]=[];
  outcomeTransfers:Transfer[]=[];

  showReturnedWorks = false;
  showSendedWorks = true;

  orderFilter: number[] = [];
  articleFilter: string = '';

  selectedPostChange() {

    this.loadWorks();
    this.transfers();
  }

  sendedUpdateTimer;

  clearFilters() {
    this.orderFilter = [];
    this.articleFilter = '';
    this.applyFilter();
  }

  applyFilter() {
    this.filteredWorks = this.filterWorks();
    this.workDeselect();
  }
  async showTransfers(type:'in'|'out'){
    let x = this.matDialog.open(TransferListComponent, {
      data: {
        dataService:this.data,
        type:type,
        transfers:type=='in'?this.incomeTransfers:this.outcomeTransfers
      },

      autoFocus: true,
      hasBackdrop: true,
      disableClose:true


    });
    let v = await firstValueFrom(x.afterClosed())
    if(v){
      this.selectedPostChange();
    }
  }

  getOrders(works: Work[]): number[] {
    return [...new Set(works.map(x => x.structure.orderNumber))];
  }

  getTotalCost(works: Work[]): number {
    return Math.floor(works.reduce((p, c) => p + c.structure.totalCost, 0));
  }

  filterWorks(): Work[] {

    return this.allWorks.filter(x => {
      let filtered = true;
      if (this.orderFilter.length > 0) {
        filtered = filtered && this.orderFilter.indexOf(x.structure.orderNumber) >= 0;
      }
      if (this.articleFilter.length > 0) {
        filtered = filtered && x.structure.article.includes(this.articleFilter);
      }
      return filtered;
    });
  }
  public transfers(){
    this.data.TransferData.inTransfers(this.selectedPost).subscribe(x=>{
      if(x){
        this.incomeTransfers = x;
      }
    })
    this.data.TransferData.outTransfers(this.selectedPost).subscribe(x=>{
      if(x){
        this.outcomeTransfers = x;
      }
    })
  }

  constructor(private data: DataService, public session: SessionService,
              private matDialog: MatDialog, private workEvent: WorkEventService, private router: Router) {
    if (this.session.currentUser == null || this.session.currentUser?.structure?.postIdMaster == null) {
      this.router.navigate(['/']);
    }

    this.selectedPost = this.session.currentUser?.structure?.postIdMaster[0]!;

    this.transfers();
    this.sendedUpdateTimer = setInterval(() => {
      this.transfers();
      this.endedWorks.forEach(x => {
        x.updateStructure();
      })
    }, 60000 * 7);
    workEvent.OnWorkChangedStatus.subscribe(x => {
      if (!x.isSuccess) {
        this.returnBack(x.work, x.from, x.to);
      } else {
        this.checkRightStatus(x.work, x.from, x.to);
      }
    });
    workEvent.OnNewWorkBySplit.subscribe(x => {
      this.allWorks.unshift(x);
      this.getArrayByStatus(x.structure.status).unshift(x);
    });
  }

  ngOnInit(): void {
    // this.loadWorks();
    this.updateWorks();
  }

  checkRightStatus(work: Work, from: number, to: number) {

    let currentArray: Work[] = this.getArrayByStatus(to);
    let oldArray: Work[] = this.getArrayByStatus(from);

    let currentIndex = currentArray.findIndex(x => x.structure.id == work.structure.id);
    let oldIndex = oldArray.findIndex(x => x.structure.id == work.structure.id);


    if (currentIndex == -1 && oldIndex != -1) {
      transferArrayItem(oldArray, currentArray, oldIndex, 0);
    }
  }

  getArrayByStatus(status: number): Work[] {
    switch (status) {
      case 10:
        return this.incomeWorks;

      case 20:
        return this.waitWorks;

      case 30:
        return this.runningWorks;

      case 40:
        return this.endedWorks;

      default:
        return [];
    }
  }

  returnBack(work: Work, from: number, to: number) {
    console.log(`return back ${to}`);
    let currentArray: Work[] = this.getArrayByStatus(to);
    let oldArray: Work[] = this.getArrayByStatus(from);

    let currentIndex = currentArray.findIndex(x => x.structure.id == work.structure.id);
    if (currentIndex != -1) {
      transferArrayItem(currentArray, oldArray, currentIndex, 0);
    }
  }

  async moveWork(work: Work, newStatus: number): Promise<boolean> {
    let can = await work.canChangeStatus(newStatus).call(null);
    if (can) {
      work.changeStatus(newStatus);
    }
    return can;
  }

  async askPostDirection(posts: string[]): Promise<string[]> {
    let x = this.matDialog.open(PostDialogComponent, {
      data: {
        posts: posts
      },

      autoFocus: true,
      hasBackdrop: true,


    });
    let v = await firstValueFrom(x.afterClosed());
    return v;

  }

  async drop(event: any) {
    // console.log(event);

    if (event.previousContainer === event.container) {
      moveItemInArray(event.container.data, event.previousIndex, event.currentIndex);
    } else {
      let can = await this.moveWork(event.item.data as Work, Number(event.container.element.nativeElement.id));
      if (can) {
        transferArrayItem(
          event.previousContainer.data,
          event.container.data,
          event.previousIndex,
          event.currentIndex,
        );
      }


    }

  }

  workDeselect() {
    this.incomeWorks = this.filteredWorks.filter(x => x.structure.status == 10);
    if (this.showReturnedWorks) {
      this.waitWorks = this.filteredWorks.filter(x => x.structure.status == 20);
    } else {
      this.waitWorks = this.filteredWorks.filter(x => x.structure.status == 20 && x.structure.issues.filter(z => z.returnBackPostId.length > 0).length == 0)
    }

    this.runningWorks = this.filteredWorks.filter(x => x.structure.status == 30);
    if (this.showSendedWorks) {
      this.endedWorks = this.filteredWorks.filter(x => x.structure.status == 40);
    } else {
      this.endedWorks = this.filteredWorks.filter(x => x.structure.status == 40 && x.structure.movedTo.length == 0);
    }


  }

  printWorks(workList: Work[]) {
    let ids = workList.map(x => x.structure.id);
    this.data.Work.PrintWorkList(ids).subscribe(x => {
      if (x) {
        window.open(x.link, '_blank');
      }
    })
  }

  updateWorks() {

    this.data.Post.PostWorksUpdate(this.selectedPost).subscribe(x => {
      if (x && x.length > 0) {
        x.forEach(w => {
          let exist = this.allWorks.find(f => f.structure.id == w.id);
          if (!exist) {
            this.allWorks.push(new Work(w, this.data));
          }
        });
        this.filteredWorks = this.filterWorks();

        this.workDeselect();
      }
    });
  }

  loadWorks() {
    this.isLoading = true;
    this.allWorks = [];
    this.filteredWorks = [];
    this.waitWorks = [];
    this.runningWorks = [];
    this.endedWorks = [];
    this.incomeWorks = [];
    this.data.Post.PostWorks(this.selectedPost).subscribe(x => {

      if (x) {
        this.isLoading = false
      }
      if (x && x.length > 0) {

        this.allWorks = x;
        this.filteredWorks = this.filterWorks();
        this.workDeselect();


      }

    }, error => {
      this.isLoading = false;
      console.log(error)
    });
  }

  async createTransfer(){
    let works = this.endedWorks.filter(x=>x.structure.movedTo.length<1);
    let x = this.matDialog.open(TransferCreateComponent, {
      data: {
        availableWorks:works,
        sourcePost:this.selectedPost,
        dataService:this.data
      },

      autoFocus: true,
      hasBackdrop: true,


    });
    let v = await firstValueFrom(x.afterClosed())
    if(v){
      let t = v as Transfer;

      t.lines.forEach(tl=>{
        let work=works.find(w=>w.structure.id == tl.sourceWorkId);
        if(work){
          work.structure.movedTo = t.postToId;
        }
      });
      window.open('/transfer/'+t.id+'/print');
    }

  }

}

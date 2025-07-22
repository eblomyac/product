import {Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges} from '@angular/core';
import {ProductCalendarRecord, ProductWorker} from "../../../model/Hr";
import {DataService} from "../../../services/data.service";
import {DialogHandlerService} from "../../../services/dialog-handler.service";
import {HrActionDialogComponent} from "../../../dialogs/hr-action-dialog/hr-action-dialog.component";

@Component({
  selector: 'app-calendar',
  templateUrl: './calendar.component.html',
  styleUrl: './calendar.component.css'
})
export class CalendarComponent implements OnChanges, OnInit {


  Workers:ProductWorker[] = [];
  @Input()Posts:string[]=[];
  @Input()isActive:boolean = false;
  isLoading:boolean = false;
  @Output()Saving:EventEmitter<boolean> = new EventEmitter();
  selectedDate=new Date();

  calendarRecords:ProductCalendarRecord[]=[];

  filterName:string='';
  filterPost:string[]=[];

  dayCount = 0;
  days:Array<{num:number,name:string, isWeekend:boolean}>=[];
  objectKeys = Object.keys;
  workerData:{[post:string]:string[]}={};
  targetData:{[post:string]:{
      [worker:string]:string[]
    } } = {};
  calendarData:{
    [post:string]:{
      [worker:string]:{
        [target:string]:{
          [day:number]:
            {data:ProductCalendarRecord, meta:{ selected:boolean }}
        }
      }
    }
  }={};

  sumWorkerData:{[post:string]:{
    [worker:string]:{
      [target:string]:{
        hours:number;
        minutes:number;
      }
    }
    }
  } = {};
  sumPostData:{[post:string]:{hours:number, minutes:number}}={};

  ngOnInit() {
  //  this.loadCalendarRecords();
  }
  saveData(){
    this.dataService.HR.SaveCalendarRecords(this.allDatas()).subscribe(x=>{
      if(x!=null){
        this.calendarRecords = x;
        this.buildData();
      }
    })
  }
  loadCalendarRecords(){
    this.calendarRecords=[];
    this.buildData();
    this.isLoading=true;
    this.dataService.HR.Calendar(this.selectedDate.getMonth()+1, this.selectedDate.getFullYear()).subscribe(z=>{
      if(z){this.calendarRecords=z.data; this.Workers=z.workers; this.buildData();}
    },error => {},()=>{this.isLoading=false});
  }
  ngOnChanges(changes: SimpleChanges){

    if(changes['isActive']){
      this.loadCalendarRecords()
    }
    if(changes['Posts']){
      this.filterPost = this.Posts;
      this.Posts.forEach(x=>{
        this.sumPostData[x]={minutes:0, hours:0};
      })
    }
  }



  constructor(private dataService: DataService, private dialogService:DialogHandlerService) {
    this.selectedDate = new Date(this.selectedDate.getFullYear(), this.selectedDate.getMonth());
   this.changeMonth(0);




  }
  public changeMonth(val:number){
    this.calendarRecords=[];
    this.buildData();
    if(this.selectedDate.getMonth()==0 && val<0){
      this.selectedDate = new Date(this.selectedDate.getFullYear()-1,11);
    }else if(this.selectedDate.getMonth()==11 && val>0){
      this.selectedDate = new Date(this.selectedDate.getFullYear()+1,0);
    }else{
      this.selectedDate = new Date(this.selectedDate.getFullYear(),this.selectedDate.getMonth()+val);
    }
    this.dayCount=this.getDaysInMonth(this.selectedDate.getFullYear(),this.selectedDate.getMonth()+1);
    this.days = [];
    for (let i = 0; i < this.dayCount; i++) {
      let d = {num:i+1, name:'',isWeekend: false};
      let date = new Date(this.selectedDate.getFullYear(), this.selectedDate.getMonth(),i+1);
      d.isWeekend = date.getDay() == 0 || date.getDay() == 6 ;
      d.name = date.toLocaleDateString('ru-RU', {weekday:'short'})
      this.days.push(d);
    }
    this.loadCalendarRecords();

  }

  getDaysInMonth(year: number, month: number): number {

    return new Date(year, month, 0).getDate();
  }


  buildData(){
    this.workerData = {};
    this.targetData = {};



    this.Workers.forEach(worker=>{
      if(this.filterName.length>0){
        if(!worker.name.toLowerCase().includes(this.filterName.toLowerCase())){
          return;
        }
      }
      worker.targets.forEach(target=>{

        if (!this.workerData[target.postId]) {
          this.workerData[target.postId] = [];
        }
        let exist = this.workerData[target.postId].find(z=>z == worker.name);
        if(!exist){
          this.workerData[target.postId].push(worker.name);
        }


        if(!this.targetData[target.postId]){
          this.targetData[target.postId] = {};
        }
        if(!this.targetData[target.postId][worker.name]){
          this.targetData[target.postId][worker.name] = [];
        }
        this.targetData[target.postId][worker.name].push(target.targetName);


        if(!this.calendarData[target.postId]){
          this.calendarData[target.postId]={};
        }
        if(!this.calendarData[target.postId][worker.name]){
          this.calendarData[target.postId][worker.name]={};
        }
        if(!this.calendarData[target.postId][worker.name][target.targetName]){
          this.calendarData[target.postId][worker.name][target.targetName] = {};
        }
        for (let step = 1; step <= this.dayCount; step++) {
          let exist = this.calendarRecords.find(z=>z.day == step && z.month == this.selectedDate.getMonth()+1 && z.year == this.selectedDate.getFullYear()
            && z.productWorkerName == worker.name && z.postId == target.postId && z.targetName == target.targetName);

          if(exist){
            this.calendarData[target.postId][worker.name][target.targetName][step] = {data:exist,meta:{selected:false}};

          }




        }


      })
    })
    this.buildSums();

  }
  buildSums(){
    /*


    this.sumPostData[post].hours+=entry.data.planningHours * entry.data.planToWorkConst;
    this.sumPostData[post].minutes+=this.sumPostData[post].hours*60;
    */
    this.sumWorkerData = {};
    this.sumPostData = {};
    for (const post in this.calendarData) {
      const workers = this.calendarData[post];
      for (const worker in workers) {
        const targets = workers[worker];
        for (const target in targets) {
          const days = targets[target];


          for (const dayStr in days) {
            const day = +dayStr;
            const entry = days[day];

            if(!this.sumWorkerData[post]){
              this.sumWorkerData[post]={};
            }

            if(!this.sumWorkerData[post][worker]){
              this.sumWorkerData[post][worker] = {};
            }
            if(!this.sumWorkerData[post][worker][target]){
              this.sumWorkerData[post][worker][target] = {hours:0, minutes:0};
            }
            this.sumWorkerData[post][worker][target].hours+=entry.data.planningHours * entry.data.planToWorkConst;
            this.sumWorkerData[post][worker][target].minutes = this.sumWorkerData[post][worker][target].hours*60;

          }
        }
      }
    }

    let allData= this.allDatas();
    this.Posts.forEach(post=>{
      let hours = allData.filter(x=>x.postId == post).reduce((accumulator, item) => {
        return accumulator + item.planningHours * item.planToWorkConst;
      }, 0);
      if(!this.sumPostData[post]){
        this.sumPostData[post] = {minutes:0,hours:0};
      }
      this.sumPostData[post].hours = hours;
      this.sumPostData[post].minutes = hours * 60;
    })

    console.log(this.sumWorkerData);
    console.log(this.sumPostData);
  }

  key(event:KeyboardEvent, record:ProductCalendarRecord){
    console.log(record);
    console.log(event);
    let n = Number(event.key);
    if(n!= Number.NaN){
      record.planningHours = n;
    }
  }
  mouse(event:MouseEvent,  record:ProductCalendarRecord, meta:{selected:boolean}){
  if(event.buttons==1){
    meta.selected = false;
  }
    if(event.buttons==2){
      meta.selected = true;
    }
    event.preventDefault()
  }
  mouseClick(event:MouseEvent,record:ProductCalendarRecord|null, meta:{selected:boolean}|null){
    console.log(event);
    event.preventDefault();
    if(record == null || meta == null){
      return
    }
    if(event.button==0){
      meta.selected = false;
    }else if(event.button==2){
      meta.selected = true;
    }

    let selectedItems = this.selectedDatas();
    if(selectedItems.length>0){
      this.dialogService.ask(HrActionDialogComponent, {data:{calendarRecords:selectedItems}}).then((result)=>{
        selectedItems.forEach(z=>z.meta.selected=false)
        this.buildSums();
      })
    }

  }
  allDatas():Array<ProductCalendarRecord> {
    let selectedItems:Array<ProductCalendarRecord>= [];
    for (const post in this.calendarData) {
      const workers = this.calendarData[post];
      for (const worker in workers) {
        const targets = workers[worker];
        for (const target in targets) {
          const days = targets[target];
          for (const dayStr in days) {
            const day = +dayStr;
            const entry = days[day];

              selectedItems.push(entry.data);

          }
        }
      }
    }

    return selectedItems;
  }
  selectedDatas():Array<{data:ProductCalendarRecord,meta:{selected:boolean}}> {
    let selectedItems:Array<{data:ProductCalendarRecord,meta:{selected:boolean}}>= [];
    for (const post in this.calendarData) {
      const workers = this.calendarData[post];
      for (const worker in workers) {
        const targets = workers[worker];
        for (const target in targets) {
          const days = targets[target];
          for (const dayStr in days) {
            const day = +dayStr;
            const entry = days[day];
            if (entry.meta?.selected) {
             selectedItems.push(entry);
            }
          }
        }
      }
    }

    return selectedItems;
  }
  setEfficiency(value:number, postId:string, workerName:string, targetName:string){
    for(let cd in this.calendarData[postId][workerName][targetName]){
      this.calendarData[postId][workerName][targetName][cd].data.planToWorkConst=value;
    }
    this.buildSums();
  }

}

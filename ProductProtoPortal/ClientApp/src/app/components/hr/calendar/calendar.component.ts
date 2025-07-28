import {Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges} from '@angular/core';
import {ProductCalendarRecord, ProductPlan, ProductWorker} from "../../../model/Hr";
import {DataService} from "../../../services/data.service";
import {DialogHandlerService} from "../../../services/dialog-handler.service";
import {HrActionDialogComponent} from "../../../dialogs/hr-action-dialog/hr-action-dialog.component";
import {CalculateCalendarComponent} from "../calculate-calendar/calculate-calendar.component";


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
  onlyNegative:boolean = false;
  selectedDate=new Date();
  plans: ProductPlan[] = [];
  calendarRecords:ProductCalendarRecord[]=[];

  filterName:string='';
  filterPost:string[]=[];
  filterCrpPost:string='';


  dayCount = 0;
  days:Array<{num:number,name:string, isWeekend:boolean}>=[];
  objectKeys = Object.keys;

  data:{[post:string]:{
    [crpPost:string]:{
      [worker:string]:{
        [target:string]:{
          [day:number]:{data:ProductCalendarRecord, meta:{ selected:boolean }
        }
        }
      }
    }
    }}={};
  filteredData:{[post:string]:{
      [crpPost:string]:{
        [worker:string]:{
          [target:string]:{
            [day:number]:{data:ProductCalendarRecord, meta:{ selected:boolean }
            }
          }
        }
      }
    }}={};
  sumData:{
    [post:string]:{
      minutes:number;
      data:{
        [crpPost:string]:{
            minutes:number,
            data:{
              [worker:string]:{
                minutes:number,
                data:{
                  [target:string]:{
                    minutes:number
                  }
                }

              }
          }
        }
      };
    }
  }={};

  filledPlan:boolean = false;
  planData:{[post:string]:{
      minutes:number;
      data:{
        [crpPost:string]:{
          minutes:number}}}}={};

  loadPlans(){
    this.plans=[];
    this.dataService.HR.PlanList(this.selectedDate.getFullYear(), this.selectedDate.getMonth()+1).subscribe(x=>{
      if(x!=null){
        this.plans=x;
        this.buildPlanData();
      }
    })
  }
  buildPlanData(){
    this.filledPlan=this.plans.filter(z=>z.targetMinutes>0).length >0 ;
    this.plans.forEach(plan=>{
      if(!this.planData[plan.postId]){
        this.planData[plan.postId]={data:{},minutes:0};
      }
      if(!this.planData[plan.postId].data[plan.crpCenter]){
        this.planData[plan.postId].data[plan.crpCenter]= {minutes:0};
      }
      this.planData[plan.postId].minutes += plan.ratioMinutes;
      this.planData[plan.postId].data[plan.crpCenter].minutes = plan.ratioMinutes;

    });
  }
  showCalculate(){
    console.log(this.plans);
    this.dialogService.ask(CalculateCalendarComponent, {data:{plans:this.plans, records:this.calendarRecords, sumData:this.sumData}}).then(result=>{
      this.buildPlanData();
    });
  }
  ngOnInit() {
  //  this.loadCalendarRecords();
  }
  saveData(){
    this.dataService.HR.SaveCalendarRecords(this.calendarRecords).subscribe(x=>{
      if(x!=null){
        this.calendarRecords = x;
      }


    }, error => {}, ()=>{
      this.dataService.HR.SavePlanList(this.plans).subscribe(x=>
      {
        if (x != null) {
          this.plans = x;
        }
        this.buildData();
      })});

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
    this.loadPlans();

  }

  getDaysInMonth(year: number, month: number): number {

    return new Date(year, month, 0).getDate();
  }

  buildData(){
    this.data={};
    this.filteredData = {};
    this.Workers.forEach(worker=>{

      if(this.filterName.length>0){
        if(!(worker.name.toLowerCase().includes(this.filterName.toLowerCase()))){
          return
        }
      }


      worker.targets.forEach(target=>{

        if(!this.filterPost.includes(target.postId)){
          return
        }
        if(this.filterCrpPost.length>0) {
          if (!target.targetCrpCenter.toLowerCase().includes(this.filterCrpPost.toLowerCase())) {
            return
          }
        }

        if (!this.data[target.postId]) {
          this.data[target.postId] = {};
        }


        if(!this.data[target.postId][target.targetCrpCenter]){
          this.data[target.postId][target.targetCrpCenter] = {}
        }


        if(!this.data[target.postId][target.targetCrpCenter][worker.name]){
          this.data[target.postId][target.targetCrpCenter][worker.name] = {};
        }


        if(!this.data[target.postId][target.targetCrpCenter][worker.name][target.targetName]){
          this.data[target.postId][target.targetCrpCenter][worker.name][target.targetName]={};
        }


        this.days.forEach(x=>{
          let exist = this.calendarRecords.find(z=>z.day == x.num &&
            z.month == this.selectedDate.getMonth()+1 && z.year == this.selectedDate.getFullYear()
            && z.productWorkerName == worker.name && z.postId == target.postId && z.targetName == target.targetName);
          if(exist){
            this.data[target.postId][target.targetCrpCenter][worker.name][target.targetName][x.num] = {data:exist,meta:{selected:false}};
          }

        });
      })
    })

    this.buildSum();
  }
  setWorkWeek(postId:string,targetCrpCenter:string,workerName:string,targetName:string,workDay:number, saturday:number, sunday:number){
    this.days.forEach(d=>{
      if(d.isWeekend){
        if(d.name=='сб'){
          this.data[postId][targetCrpCenter][workerName][targetName][d.num].data.planningHours=saturday;
        }else{
          this.data[postId][targetCrpCenter][workerName][targetName][d.num].data.planningHours=sunday;
        }

      }else{
        this.data[postId][targetCrpCenter][workerName][targetName][d.num].data.planningHours = workDay;
      }

    })
    this.buildSum();
  }

  buildSum(){
    this.sumData={};
    for(const post in this.data){
      const crpPosts = this.data[post];
      for(const crpPost in crpPosts){
        const workers = crpPosts[crpPost];
        for(const workerName in workers){
          const targets = workers[workerName];
          for(const targetName in targets){
            const days = targets[targetName];
            for(const day in days){
              if(!this.sumData[post]){
                this.sumData[post]= {data:{},minutes:0};
              }
              if(!this.sumData[post].data[crpPost]) {
                this.sumData[post].data[crpPost] = {data:{},minutes:0};

              }
              if(!this.sumData[post].data[crpPost].data[workerName]) {
                this.sumData[post].data[crpPost].data[workerName] = {data:{},minutes:0};
              }
              if(!this.sumData[post].data[crpPost].data[workerName].data[targetName]){
                this.sumData[post].data[crpPost].data[workerName].data[targetName]={minutes:0};
              }
              let minutes = this.data[post][crpPost][workerName][targetName][day].data.planToWorkConst*this.data[post][crpPost][workerName][targetName][day].data.planningHours*60;
              this.sumData[post].minutes+=minutes;
              this.sumData[post].data[crpPost].minutes+=minutes;
              this.sumData[post].data[crpPost].data[workerName].minutes+=minutes;
              this.sumData[post].data[crpPost].data[workerName].data[targetName].minutes+=minutes;
            }
          }
        }
      }
    }
    console.log(this.sumData);
  }
  setEfficiency(value:number, postId:string, crpPost:string, workerName:string, targetName:string){
    for(let cd in this.data[postId][crpPost][workerName][targetName]){
      this.data[postId][crpPost][workerName][targetName][cd].data.planToWorkConst=value;
    }
    this.buildSum();
  }
  selectedDatas():Array<{data:ProductCalendarRecord,meta:{selected:boolean}}>{

    let selectedItems:Array<{data:ProductCalendarRecord,meta:{selected:boolean}}>= [];
    for (const post in this.data) {
      const crpPosts = this.data[post];
      for (const crpPost in crpPosts) {
        const workers = crpPosts[crpPost];
        for (const worker in workers) {
          const targets = workers[worker];
          for (const target in targets) {
            const days = targets[target];
            for (const dayStr in days) {
              const day = Number(dayStr);
              const entry = days[day];
              if (entry?.meta?.selected) {
               selectedItems.push(entry)
              }
            }
          }
        }
      }
    }
    return selectedItems;
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
        this.buildSum();
      })
    }

  }



}

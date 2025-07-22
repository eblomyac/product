import {TransportService} from "../transport.service";
import {map, Observable} from "rxjs";
import {HttpParams} from "@angular/common/http";
import {ApiAnswer} from "../../model/ApiAnswer";
import {IWork} from "../../model/Work";
import {DataService} from "../data.service";
import {OTKAvailableOperation, OTKTargetValue, OTKWorker} from "../../model/OTKAvailableOperation";
import {OTKCheck} from "../../model/OTKCheck";
import {ProductCalendarRecord, ProductTarget, ProductWorker} from "../../model/Hr";

export class HrData {

  constructor(private transportService: TransportService, private dataService: DataService) {

  }
  public SaveCalendarRecords(data:ProductCalendarRecord[]):Observable<ProductCalendarRecord[]> {
    return this.transportService.Post('/hr/SaveCalendarData', new HttpParams(),data).pipe(map<ApiAnswer|null|'not ended',ProductCalendarRecord[]>(x=>{
      if(x!=null && x!='not ended'){
        if(x.isSuccess){
          return (x.result);
        }
      }
      return null;
    }));
  }
  public Calendar(month:number,year:number):Observable<{data:ProductCalendarRecord[], workers:ProductWorker[]}>{
    return this.transportService.Get('/hr/Calendar', new HttpParams().append('month', month).append('year',year)).pipe(map<ApiAnswer|null|'not ended',{data:ProductCalendarRecord[], workers:ProductWorker[]}>(x=>{
      if(x!=null && x!='not ended'){
        if(x.isSuccess){
          return (x.result);
        }
      }
      return null;
    }));
  }
  public WorkerList():Observable<ProductWorker[]> {
    return this.transportService.Get('/hr/WorkerList', new HttpParams()).pipe(map<ApiAnswer|null|'not ended',ProductWorker[]>(x=>{
      if(x!=null && x!='not ended'){
        if(x.isSuccess){
          return (x.result);
        }
      }
      return null;
    }));
  }
  public TargetList():Observable<Array<{postId:string,targets:ProductTarget[]}>> {
    return this.transportService.Get('/hr/Targets', new HttpParams()).pipe(map<ApiAnswer|null|'not ended',Array<{postId:string,targets:ProductTarget[]}>>(x=>{
      if(x!=null && x!='not ended'){
        if(x.isSuccess){
          return (x.result.result);
        }
      }
      return null;
    }));
  }
  public SaveTargetList(data:any):Observable<ProductWorker[]>{
    return this.transportService.Post('/hr/SaveTargets', new HttpParams(),data).pipe(map<ApiAnswer|null|'not ended',ProductWorker[]>(x=>{
      if(x!=null && x!='not ended'){
        if(x.isSuccess){
          return (x.result);
        }
      }
      return null;
    }));
  }

  public SaveWorkerList(data:any):Observable<ProductWorker[]> {
    return this.transportService.Post('/hr/SaveWorkers', new HttpParams(),data).pipe(map<ApiAnswer|null|'not ended',ProductWorker[]>(x=>{
      if(x!=null && x!='not ended'){
        if(x.isSuccess){
          return (x.result);
        }
      }
      return null;
    }));
  }
}

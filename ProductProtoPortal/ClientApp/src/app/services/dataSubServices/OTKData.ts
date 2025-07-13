import {TransportService} from "../transport.service";
import {map, Observable} from "rxjs";
import {HttpParams} from "@angular/common/http";
import {ApiAnswer} from "../../model/ApiAnswer";
import {IWork} from "../../model/Work";
import {DataService} from "../data.service";
import {OTKAvailableOperation, OTKTargetValue, OTKWorker} from "../../model/OTKAvailableOperation";
import {OTKCheck} from "../../model/OTKCheck";

export class OTKData {
  constructor(private transportService: TransportService, private dataService: DataService) {

  }
  public OtkChecks(filter:any):Observable<OTKCheck[]> {
    return this.transportService.Post('/otk/OKTCheckList', new HttpParams(),filter).pipe(map<ApiAnswer|null|'not ended',OTKCheck[]>(x=>{
      if(x!=null && x!='not ended'){
        if(x.isSuccess){
          return (x.result);
        }
      }
      return null;
    }));
  }
  public TargetValues():Observable<OTKTargetValue[]>{
    return this.transportService.Get('/otk/TargetValues', new HttpParams()).pipe(map<ApiAnswer|null|'not ended',OTKTargetValue[]>(x=>{
      if(x!=null && x!='not ended'){
        if(x.isSuccess){
          return (x.result);
        }
      }
      return null;
    }));
  }
  public WorkerList():Observable<OTKWorker[]>{
    return this.transportService.Get('/otk/workerList', new HttpParams()).pipe(map<ApiAnswer|null|'not ended',OTKWorker[]>(x=>{
      if(x!=null && x!='not ended'){
        if(x.isSuccess){
          return (x.result);
        }
      }
      return null;
    }));
  }
  public SaveWorkers(workers:any[]):Observable<OTKWorker[]>{
    return this.transportService.Post('/otk/updateworkerList', new HttpParams(),workers).pipe(map<ApiAnswer|null|'not ended',OTKWorker[]>(x=>{
      if(x!=null && x!='not ended'){
        if(x.isSuccess){
          return (x.result);
        }
      }
      return null;
    }));
  }

  public OperationsList():Observable<OTKAvailableOperation[]>{
    return this.transportService.Get('/otk/operationList', new HttpParams()).pipe(map<ApiAnswer|null|'not ended',OTKAvailableOperation[]>(x=>{
      if(x!=null && x!='not ended'){
        if(x.isSuccess){
          return (x.result);
        }
      }
      return null;
    }));
  }
  public SaveOperations(operations:any[]):Observable<OTKAvailableOperation[]>{
    return this.transportService.Post('/otk/updateList', new HttpParams(),operations).pipe(map<ApiAnswer|null|'not ended',OTKAvailableOperation[]>(x=>{
      if(x!=null && x!='not ended'){
        if(x.isSuccess){
          return (x.result);
        }
      }
      return null;
    }));
  }
  public Template(work:IWork):Observable<OTKCheck>{

    return this.transportService.Get('/otk/Template', new HttpParams().append('workId', work.id)).pipe(map<ApiAnswer|null|'not ended',OTKCheck>(x=>{
      if(x!=null && x!='not ended'){
        if(x.isSuccess){
          return (x.result);
        }
      }
      return null;
    }));
  }
  public SaveCheck(otkCheck:OTKCheck):Observable<OTKCheck>{
    return this.transportService.Post('/otk/SaveCheck', new HttpParams(),otkCheck).pipe(map<ApiAnswer|null|'not ended',OTKCheck>(x=>{
      if(x!=null && x!='not ended'){
        if(x.isSuccess){
          return (x.result);
        }
      }
      return null;
    }));
  }

}

import {HttpParams} from "@angular/common/http";
import {Issue} from "../../model/Issue";
import {TransportService} from "../transport.service";
import {IWork, Work} from "../../model/Work";
import {ApiAnswer} from "../../model/ApiAnswer";
import {map, Observable} from "rxjs";
import {DataService} from "../data.service";

export  class WorkData{
  constructor(private transport:TransportService, private dataService:DataService) {
  }
  public MaconomySyncClose():Observable<any>{
    return this.transport.Get('/transfer/MaconomyOrderSync', new HttpParams()).pipe(map<ApiAnswer|null,any|null>(x=>{
      if(x!=null){
        return x.result;
      }
    }));
  }
  public PrintWorkList(worksIds:Array<number>):Observable<any|null>{
    return this.transport.Post('/works/PrintList', new HttpParams(),worksIds).pipe(map<ApiAnswer|null,any|null>(x=>{
      if(x!=null){
        return x.result;
      }
    }));
  }
  public UpdateDates() :Observable<string|null>{
    return this.transport.Get('/works/UpdateDates', new HttpParams()).pipe(map<ApiAnswer|null, string|null>(x=>{
      if(x!=null){
        return x.message;
      }
      return '';
    }));
  }
  public Prepare(orders:number[]):Observable<Work[]|null>{
    return this.transport.Post('/works/prepare', new HttpParams(), orders).pipe(map<ApiAnswer|null, Work[]|null>(x=>{
      if(x!=null){
        if( x.isSuccess){
          return (x.result as IWork[]).map(z=>new Work(z,this.dataService))
        }
        return [];
      }
     return null;
    }));
  }
  public RemoveUnstarted(data:string[]):Observable<boolean|null>{
    return this.transport.Post('/works/removeUnstarted', new HttpParams(), data).pipe(map<ApiAnswer|null, boolean|null>(x=>{
      if(x!=null){
        return x.isSuccess;
      }
      return false;
    }));
  }
  public Create(works:Work[]):Observable<Work[]|null>{
    let ws = works.map(x=>x.structure);
    return this.transport.Post('/works/create', new HttpParams(), ws).pipe(map<ApiAnswer|null, Work[]|null>(x=>{
      if(x!=null){
        if(x.isSuccess){
          return (x.result as IWork[]).map(z=>new Work(z,this.dataService))
        }
        return [];
      } else{return null;}
    }));
  }
  public UnstartedSuggestions():Observable<any[]|null>{
    return this.transport.Get('/works/NotStartedSuggestions' , new HttpParams()).pipe(map<ApiAnswer|null,any[]|null>(x=>{
      if(x){
        if(x.isSuccess){
          return x.result as any[];
        }
        return [];
      }
      return null;

    }));
  }

  public ChangeStatus(work:Work, to:number):Observable<boolean>{
    return this.transport
      .Put(`/works/${work.structure.id}/newstatus`, new HttpParams().append('status',to.toString()), {})
      .pipe(map<ApiAnswer|null,boolean>(x=>{
        if(x){
          return x.result;
        }
    }));
  }
  public StartWorks(suggestions:any[]):Observable<boolean|null>{
    return this.transport.Post('/works/StartWorks' , new HttpParams(),suggestions).pipe(map<ApiAnswer|null,boolean|null>(x=>{
      if(x){
        return x.isSuccess;
      }
      return null
    }));
  }
  public LoadSuggestions(works:Work[]):Observable<boolean>{
    let ws = works.map(x=>x.structure);
    return this.transport.Post('/works/GetSuggestions' , new HttpParams(),ws).pipe(map<ApiAnswer|null,boolean>(x=>{
      if(x && x.isSuccess){
        x.result.forEach((z: { work: { id: number; }; forward: string[]; backward: string[]; })=>{
          let w = works.find(work=>work.structure.id == z.work.id);
          if(w){
            w.structure.forwardMoves = z.forward;
            w.structure.backwardMoves = z.backward;
          }
        });
        return true;
      }
      return false;
    }));
  }
  public View(id:number):Observable<Work|null>{
    return this.transport.Get(`/works/${id}`, new HttpParams()).pipe(map<ApiAnswer|null,Work|null>(x=>{
      if(x){
        if(x.isSuccess){
          return new Work(x.result as IWork, this.dataService);
        }
      }
      return null;
    }));
  }
  public Split(work:Work,splitCount:number):Observable<Work[]|null>{
    return this.transport.Put(`/works/${work.structure.id}/split`, new HttpParams().append('splitCount',splitCount),{})
      .pipe(map<ApiAnswer|null,Work[]|null>(x=>{
      if(x){
        if(x.isSuccess){
          let newWorks = x.result as IWork[];
          let result:Work[]=[];
          let change = newWorks.find(z=>z.id == work.structure.id);
          if(change){
            work.structure.count = change.count;
            work.structure.totalCost = change.totalCost;
            result.push(work);
          }
          let create = newWorks.find(z=>z.id != work.structure.id);
          if(create){
            result.push(new Work(create,this.dataService));
          }
          return result;
        }
      }
      return null;
    }));
  }

  public Move(id:number,data:any):Observable<boolean|null>{
    return this.transport.Put(`/works/${id}/Move`, new HttpParams(),data)
      .pipe(map<ApiAnswer|null,boolean|null>(x=>{
      if(x!=null){
        return x.isSuccess;
      }
      return null;
    }))
  }
  public Issues(workId:number):Observable<Issue[]|null>{
    return this.transport.Get('/issues/list', new HttpParams().append('workId', workId)).pipe(map<ApiAnswer|null,Issue[]|null>(x=>{
      if(x!=null){
        if(x.isSuccess){
          return x.result as Issue[];
        }else{
          return [];
        }
      }
      return null;
    }))
  }
}

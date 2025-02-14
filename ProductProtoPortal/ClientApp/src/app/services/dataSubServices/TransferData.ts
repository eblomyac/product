import {TransportService} from "../transport.service";
import {DataService} from "../data.service";
import {map, Observable} from "rxjs";
import {Transfer} from "../../model/Transfer";
import {IWork, Work} from "../../model/Work";
import {HttpParams} from "@angular/common/http";
import {ApiAnswer} from "../../model/ApiAnswer";

export class TransferData{
  constructor(private transportService: TransportService, private dataService: DataService) {

  }
  public createTransfer(postId:string, postToId:string, works:Array<IWork>):Observable<Transfer|null>{
    let d = {
      fromPost: postId,
      toPost: postToId,
      works:works
    }
    return this.transportService.Post('/transfer/create', new HttpParams(), d)
      .pipe(map<ApiAnswer|null|'not ended',Transfer|null>(x=>{

        if(x && x!='not ended' && x.isSuccess){
          return x.result as Transfer;
        }
        return null;
    }))
  }
  public list(offset:number, filter:any){
    return this.transportService.Post('/transfer/list', new HttpParams().append('offset',offset), filter)
      .pipe(map<ApiAnswer|null|'not ended',Transfer[]|null>(x=>{

      if(x && x!='not ended' && x.isSuccess ){
        return x.result as Transfer[];
      }else if( x!='not ended' && x?.isSuccess==false){
        console.log(x.message);
      }
      return null;
    }))

  }
  public inTransfers(post:string):Observable<Transfer[]|null>{
    return this.transportService.Get('/transfer/listin', new HttpParams().append('postId',post))
      .pipe(map<ApiAnswer|null|'not ended',Transfer[]|null>(x=>{

        if(x && x!='not ended' && x.isSuccess){
          return x.result as Transfer[];
        }
        return null;
      }))
  }
  public outTransfers(post:string):Observable<Transfer[]|null>{
    return this.transportService.Get('/transfer/listout', new HttpParams().append('postId',post))
      .pipe(map<ApiAnswer|null|'not ended',Transfer[]|null>(x=>{

        if(x && x!='not ended' && x.isSuccess){
          return x.result as Transfer[];
        }
        return null;
      }))
  }
  public applyTransfer(transfer:Transfer):Observable<boolean|null>{
    return this.transportService.Post('/transfer/apply', new HttpParams(), transfer)
      .pipe(map<ApiAnswer|null|'not ended',boolean|null>(x=>{

        if(x && x!='not ended'&& x.isSuccess){
         return true;
        }
        return null;
      }))
  }

  public byId(id:number){
    return this.transportService.Get('/transfer/'+id+'/view', new HttpParams())
      .pipe(map<ApiAnswer|null|'not ended',Transfer|null>(x=>{

        if(x && x!='not ended'&& x.isSuccess){
          return x.result as Transfer;
        }else if( x!='not ended' && x?.isSuccess==false){
          console.log(x.message);
        }
        return null;
      }))
  }

}

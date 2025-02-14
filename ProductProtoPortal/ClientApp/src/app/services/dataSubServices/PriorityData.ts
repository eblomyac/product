import {TransportService} from "../transport.service";
import {DataService} from "../data.service";
import {map, Observable} from "rxjs";
import {OrderPriority} from "../../model/Priority";
import {HttpParams} from "@angular/common/http";
import {IssueTemplate} from "../../model/Issue";
import {ApiAnswer} from "../../model/ApiAnswer";

export class PriorityData {
  constructor(private transportService: TransportService, private dataService: DataService) {
  }

  public Save(list:OrderPriority[]):Observable<boolean | null> {
    return this.transportService.Post('/priority/update', new HttpParams(), list).pipe(map<ApiAnswer | null, boolean | null>(x => {
      if(x){
        return x.isSuccess;
      }else{
        return null;
      }

    }));
  }
  public List():Observable<OrderPriority[]|null>{
    return this.transportService.Get('/priority/', new HttpParams()).pipe(map<ApiAnswer|null|'not ended',OrderPriority[]|null>(x=> {
      if (x != null && x!='not ended') {
        if (x.isSuccess) {
          return x.result as OrderPriority[];
        } else {
          return [];
        }
      }
      return null;
    }));
  }
}

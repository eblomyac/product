import {TransportService} from "../transport.service";
import {DataService} from "../data.service";
import {map, Observable} from "rxjs";
import {HttpParams} from "@angular/common/http";
import {ApiAnswer} from "../../model/ApiAnswer";

export class OperatorData {
  constructor(private transportService: TransportService, private dataService: DataService) {
  }

  public WorkList(orderNumber:number, lineNumber:number):Observable<any>{
    return this.transportService.Get('/OperatorApi/WorkList', new HttpParams()
      .append('orderNumber',orderNumber)
      .append('lineNumber', lineNumber))
      .pipe(map<ApiAnswer|null,any>(x=>{
      if(x){
        if(x.isSuccess){
          return (x.result);
        }
      }
      return null;
    }));
  }
  public ChangeCount(data:any[]):Observable<null | any[] >{
    return this.transportService.Post('/OperatorApi/ChangeCount', new HttpParams(), data)
      .pipe(map<ApiAnswer|null,any>(x=> {
        if (x) {
          if (x.isSuccess) {
            return (x.result);
          }
        }
        return null;
      }));
  }


}

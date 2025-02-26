import {TransportService} from "../transport.service";
import {DataService} from "../data.service";
import {map, Observable} from "rxjs";
import {IssueTemplate} from "../../model/Issue";
import {HttpParams} from "@angular/common/http";
import {ApiAnswer} from "../../model/ApiAnswer";
import {DailySource} from "../../model/DailySource";

export class DailySourceData {
  constructor(private transportService: TransportService, private dataService: DataService) {
  }
  public FillValues(dailySource:DailySource[]):Observable<DailySource[]|null>{
    return this.transportService.Post('/DailySource/FillTodayByLines', new HttpParams(), dailySource)
      .pipe(map<ApiAnswer|null|'not ended', DailySource[] | null>(x => {
        if (x != null && x!='not ended') {
          if (x.isSuccess) {
            return x.result as DailySource[];
          } else {
            return [];
          }
        }
        return null;
      }));
  }
  public Update(dailySource:DailySource[]):Observable<DailySource[]|null>{
    return this.transportService.Post('/DailySource/Update', new HttpParams(), dailySource)
      .pipe(map<ApiAnswer|null|'not ended', DailySource[] | null>(x => {
        if (x != null && x!='not ended') {
          if (x.isSuccess) {
            return x.result as DailySource[];
          } else {
            return [];
          }
        }
        return null;
      }));
  }
  public TodayValues(postId:string):Observable<DailySource[]|null>{
    return this.transportService.Get('/DailySource/TodayValues', new HttpParams().append('postId', postId))
      .pipe(map<ApiAnswer|null|'not ended', DailySource[] | null>(x => {
        if (x != null && x!='not ended') {
          if (x.isSuccess) {
            return x.result as DailySource[];
          } else {
            return [];
          }
        }
        return null;
      }));
  }
  public isFilledToday(postId:string):Observable<number|null> {
    return this.transportService.Get('/DailySource/IsFilledToday', new HttpParams().append('postId', postId))
      .pipe(map<ApiAnswer|null|'not ended', number | null>(x => {
        if (x != null && x!='not ended') {
          if (x.isSuccess) {
            return x.result as number;
          } else {
            return -1;
          }
        }
        return null;
      }));
  }
  public fillToday(postId:string, value:number):Observable<boolean|null>{
    return this.transportService.Get('/DailySource/FillToday', new HttpParams().append('postId', postId).append('value',value))
      .pipe(map<ApiAnswer|null|'not ended', boolean | null>(x => {
        if (x != null && x!='not ended') {
          return x.isSuccess;
        }
        return null;
      }));
  }

  public LastValues(postId:string):Observable<DailySource[]|null>{
    return this.transportService.Get('/DailySource/LastValues', new HttpParams().append('postId', postId))
      .pipe(map<ApiAnswer|null|'not ended', DailySource[] | null>(x => {
        if (x != null && x!='not ended') {
          if (x.isSuccess) {
            return x.result as DailySource[];
          } else {
            return [];
          }
        }
        return null;
      }));
  }
}

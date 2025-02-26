﻿import {HttpParams} from "@angular/common/http";
import {TransportService} from "../transport.service";
import {ApiAnswer} from "../../model/ApiAnswer";
import {map, Observable} from "rxjs";
import {DataService} from "../data.service";


export class StatisticData {
  constructor(private transportService: TransportService, private dataService: DataService) {

  }
  public CostPeriodReport(from:string,to:string):Observable<string|null>{
    return this.transportService.Get('/analytic/CostPeriodReportDownload', new HttpParams().append('from',from).append('to',to)).pipe(map<ApiAnswer|null|'not ended',string>(x=>{
      if(x!=null && x!='not ended'){
        if(x.isSuccess){
          return (x.result.link);
        }
      }
      return null;
    }));
  }
  public CostReport():Observable<string|null>{
    return this.transportService.Get('/analytic/CostReportDownload', new HttpParams()).pipe(map<ApiAnswer|null|'not ended',string>(x=>{
      if(x!=null && x!='not ended'){
        if(x.isSuccess){
          return (x.result.link);
        }
      }
      return null;
    }));
  }
  public MaconomyCompareOrders():Observable<number[]|null>{
    return this.transportService.Get('/analytic/MaconomyOrderCompare', new HttpParams())
      .pipe(map<ApiAnswer|null|'not ended',number[]|null>(x=>{
        if(x!=null && x!='not ended'){
          if(x.isSuccess){
            return x.result as number[];
          }
        }
        return [];
      }))
  }
  public AdditionalCostReport(dateFrom:string,dateTo:string, moveDay:boolean):Observable<boolean|null>{
    return this.transportService.Get('/analytic/AdditionalCostReport', new HttpParams().append('dateFrom',dateFrom).append('dateTo',dateTo).append('moveDay',moveDay))
      .pipe(map<ApiAnswer|null|'not ended',boolean|null>(x=>{
        if(x!=null && x!='not ended'){
          x.isSuccess
        }
        return null;
      }))
  }
  public PeriodReport(dateFrom:string,dateTo:string, moveDay:boolean):Observable<boolean|null>{
    return this.transportService.Get('/analytic/PeriodReport', new HttpParams().append('dateFrom',dateFrom).append('dateTo',dateTo).append('moveDay',moveDay))
      .pipe(map<ApiAnswer|null|'not ended',boolean|null>(x=>{
        if(x!=null && x!='not ended'){
          x.isSuccess
        }
        return null;
      }))
  }
  public ArticlePeriodReport(dateFrom:string,dateTo:string, moveDay:boolean):Observable<boolean|null>{
    return this.transportService.Get('/analytic/ArticlePeriodReport', new HttpParams().append('dateFrom',dateFrom).append('dateTo',dateTo).append('moveDay',moveDay))
      .pipe(map<ApiAnswer|null|'not ended',boolean|null>(x=>{
        if(x!=null && x!='not ended'){
          x.isSuccess
        }
        return null;
      }))
  }
  public DateReport(date:string):Observable<boolean|null>{
    return this.transportService.Get('/analytic/DailyReport', new HttpParams().append('date',date))
      .pipe(map<ApiAnswer|null|'not ended',boolean|null>(x=>{
        if(x!=null && x!='not ended'){
          x.isSuccess
        }
        return null;
      }))
  }
  public PrintTotalOrderStat(articleFilter:string,orderFilter:string):Observable<any|null>{
    let params = new HttpParams();
    if(articleFilter==null || articleFilter.length<1){
      articleFilter='*'
    }
    if(orderFilter==null || orderFilter.length<1){
     orderFilter = '*'
    }
    params = params.append('articleFilter',articleFilter);
    params = params.append('orderFilter',orderFilter);
    return this.transportService.Get('/analytic/PrintTotalOrderStat', params)
      .pipe(map<ApiAnswer|null|'not ended',any|null>(x=>{
        if(x!=null && x!='not ended'){
          if(x.isSuccess){
            return x.result;
          }
          return {};
        }
        return null;
      }))

  }
  public TotalOrderStat():Observable<any|null>{
    return this.transportService.Get('/analytic/TotalOrderStat', new HttpParams())
      .pipe(map<ApiAnswer|null|'not ended',any|null>(x=>{
        if(x!=null && x!='not ended'){
          if(x.isSuccess){
            return x.result;
          }
          return {};
        }
        return null;
      }))

  }
  public OrderTimeLine(orderNumber:number):Observable<any|null>{
    return this.transportService.Get('/analytic/OrderTimeLine', new HttpParams().append('orderNumber',orderNumber))
      .pipe(map<ApiAnswer|null|'not ended',any|null>(x=>{
        if(x!=null && x!='not ended'){
          if(x.isSuccess){
            return x.result;
          }
          return {};
        }
        return null;
      }))
  }
  public PostStatistic():Observable<any|null>{
    return this.transportService.Get('/analytic/postStatistic', new HttpParams())
      .pipe(map<ApiAnswer|null|'not ended',any|null>(x=>{
        if(x!=null && x!='not ended'){
        if(x.isSuccess){
          return x.result as any;
        }
        return [];
      }
      return null;
    }));
  }
  public ActualOrders():Observable<number[]|null>{
    return this.transportService.Get('/analytic/actualOrders', new HttpParams()).pipe(map<ApiAnswer|null|'not ended',number[]|null>(x=>{
      if(x!=null && x!='not ended'){
        if(x.isSuccess){
          return x.result as number[];
        }
        return [];
      }
      return null;
    }));
  }
  public OrderStatisticArticleFiltered(orderId:number, artilceIds:Array<string>):Observable<any|null>{
    return this.transportService.Post('/analytic/OrderStatistic', new HttpParams().append('orderId',orderId), artilceIds).pipe(map<ApiAnswer|null|'not ended',any|null>(x=>{
      if(x!=null && x!='not ended'){
        if(x.isSuccess){
          return x.result;
        }
        return {};
      }
      return null;
    }))
  }
  public OrderStatistic(orderId:number):Observable<any|null>{
    return this.transportService.Get('/analytic/OrderStatistic', new HttpParams().append('orderId',orderId)).pipe(map<ApiAnswer|null|'not ended',any|null>(x=>{
      if(x!=null && x!='not ended'){
        if(x.isSuccess){
          return x.result;
        }
        return {};
      }
      return null;
    }));
  }
  public PostRetroStatistic(from:Date,to:Date):Observable<any|null>{
    return this.transportService.Get('/analytic/PostRetroStat', new HttpParams().append('from',from.toISOString()).append('to',to.toISOString()))
      .pipe(map<ApiAnswer|null|'not ended',any|null>(x=>{
        if(x!=null && x!='not ended'){
          if(x.isSuccess){
            return x.result;
          }
          return {};
        }
        return null;
      }))
  }
}

import {TransportService} from "../transport.service";
import {DataService} from "../data.service";
import {map, Observable} from "rxjs";
import {HttpParams} from "@angular/common/http";
import {ApiAnswer} from "../../model/ApiAnswer";
import {IWork} from "../../model/Work";

export class InfoData {
  constructor(private transportService: TransportService, private dataService: DataService) {
  }

  public ArticleList():Observable<string[]>{
    return this.transportService.Get('/Info/ArticleList', new HttpParams()).pipe(map<ApiAnswer|null,string[]>(x=>{
      if(x){
        if(x.isSuccess){
          return (x.result as string[]);
        }
      }
      return [];
    }));
  }
  public PostList():Observable<string[]>{
    return this.transportService.Get('/Info/CrpPostList', new HttpParams()).pipe(map<ApiAnswer|null,string[]>(x=>{
      if(x){
        if(x.isSuccess){
          return (x.result as string[]);
        }
      }
      return [];
    }));
  }public CostData(art:string):Observable<number[]>{
    return this.transportService.Get('/Info/ArticleCost', new HttpParams().append('article',art)).pipe(map<ApiAnswer|null,number[]>(x=>{
      if(x){
        if(x.isSuccess){
          return (x.result as number[]);
        }
      }
      return [];
    }));
  }
  public CostDataBatch(arts:string[]):Observable<number[][]>{
    return this.transportService.Post('/Info/ArticleCostBatch', new HttpParams(), arts).pipe(map<ApiAnswer|null,number[][]>(x=>{
      if(x){
        if(x.isSuccess){
          return (x.result as number[][]);
        }
      }
      return [];
    }));
  }
}

import {TransportService} from "../transport.service";
import {DataService} from "../data.service";
import {map, Observable} from "rxjs";
import {HttpParams} from "@angular/common/http";
import {ApiAnswer} from "../../model/ApiAnswer";
import {IWork} from "../../model/Work";
import * as http from "http";

export class InfoData {
  constructor(private transportService: TransportService, private dataService: DataService) {
  }

  public HistoryDownload(from:string, to:string, userBy:string, postId:string, article:string, order:number):Observable<string>{

    let hp:HttpParams = new HttpParams();
    hp = hp.append("from", from.toString());
    hp = hp.append("to", to.toString());
    if(userBy!=null && userBy.length>0){
      hp = hp.append('userBy', userBy);
    }
    if(postId!= null && postId.length>0){
      hp = hp.append('postId', postId);
    }
    if(article!= null && article.length>0){
      hp = hp.append('article', article);
    }
    if(order!=null && order>0){
      hp = hp.append('order',order);
    }


    return this.transportService.Get('/Info/HistoryDownload',hp).pipe(map<ApiAnswer|null,string>(x=>{
      if(x){
        if(x.isSuccess){
          return (x.result.link);
        }
      }
      return null;
    }));
  }
  public History(from:string, to:string, userBy:string, postId:string, article:string, order:number):Observable<any[]>{

    let hp:HttpParams = new HttpParams();
    hp = hp.append("from", from.toString());
    hp = hp.append("to", to.toString());
    if(userBy!=null && userBy.length>0){
      hp = hp.append('userBy', userBy);
    }
    if(postId!= null && postId.length>0){
      hp = hp.append('postId', postId);
    }
    if(article!= null && article.length>0){
      hp = hp.append('article', article);
    }
    if(order!=null && order>0){
      hp = hp.append('order',order);
    }


    return this.transportService.Get('/Info/History',hp).pipe(map<ApiAnswer|null,string[]>(x=>{
      if(x){
        if(x.isSuccess){
          return (x.result as any[]);
        }
      }
      return [];
    }));
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

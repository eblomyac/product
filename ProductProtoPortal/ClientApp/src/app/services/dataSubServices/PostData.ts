﻿import {HttpParams} from "@angular/common/http";
import {IPost} from "../../model/Post";
import {TransportService} from "../transport.service";
import {IWork, Work} from "../../model/Work";
import {ApiAnswer} from "../../model/ApiAnswer";
import {map, Observable} from "rxjs";
import {DataService} from "../data.service";


export class PostData {
  constructor(private transportService: TransportService, private dataService:DataService) {
  }

  public PostWorksUpdate(postId:string):Observable<IWork[]>{
    return this.transportService.Get('/posts/CurrentWorks', new HttpParams().append('PostId',postId)).pipe(map<ApiAnswer|null,IWork[]>(x=>{
      if(x){
        if(x.isSuccess){
          return (x.result as IWork[]);
        }
      }
      return [];
    }));
  }
  public PostWorks(postId:string):Observable<Work[]|null>{
    return this.transportService.Get('/posts/CurrentWorks', new HttpParams().append('PostId',postId)).pipe(map<ApiAnswer|null,Work[]|null>(x=>{
      if(x){
        if(x.isSuccess){
          return (x.result as IWork[]).map(z=>new Work(z,this.dataService))
        }
      }
      return null;
    }));
  }
  public List():Observable<IPost[]>{
    return this.transportService.Get('/posts/list', new HttpParams()).pipe(map<ApiAnswer|null, IPost[]>(x=>{
      if(x){
        if(x.isSuccess){
          return x.result as IPost[];
        }
      }
      return [];
    }));
  }
  public Update(posts:IPost[]){
    return this.transportService.Put('/posts/update', new HttpParams(), posts).pipe(map<ApiAnswer|null, IPost[]>(x=>{
      if(x){
        if(x.isSuccess){
          return x.result as IPost[];
        }
      }
      return [];
    }));
  }
}

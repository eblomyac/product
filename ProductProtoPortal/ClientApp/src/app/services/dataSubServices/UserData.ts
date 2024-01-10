import {HttpParams} from "@angular/common/http";
import {TransportService} from "../transport.service";
import {IUser} from "../../model/User";
import {ApiAnswer} from "../../model/ApiAnswer";
import {map, Observable} from "rxjs";


export class UserData{
  constructor(private transportService:TransportService) {
  }
  public Login():Observable<IUser|null>{
    return this.transportService.Get('/users/login', new HttpParams()).pipe(map<ApiAnswer|null, IUser|null>(x=>{
      if(x){
        if(x.isSuccess){
          return x.result as IUser;
        }
      }
      return null;
    }));
  }
  public List():Observable<IUser[]>{
    return this.transportService.Get('/users/list', new HttpParams()).pipe(map<ApiAnswer|null, IUser[]>(x=>{
      if(x){
        if(x.isSuccess){
          return x.result as IUser[];
        }
      }
      return [];
    }));
  }
  public Update(users:IUser[]):Observable<IUser|null>{
    return this.transportService.Put('/users/update', new HttpParams(), users).pipe(map<ApiAnswer|null, IUser|null>(x=>{
      if(x){
        if(x.isSuccess){
          return x.result as IUser;
        }
      }
      return null;
    }));
  }


}

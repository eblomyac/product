import {DataService} from "../services/data.service";
import {Observable, Subject} from "rxjs";

export interface IUser{
    name:string;
    mail:string;
    accName:string;
    isAdmin:boolean;
    isOperator:boolean;
    isMaster:boolean;
    postIdMaster:string;
}
export class User{

  public structure:IUser|null=null;
  private eventSubject:Subject<any>= new Subject<any>();
  public Events:Observable<any> = this.eventSubject.asObservable();

  constructor(private dataService:DataService) {
    dataService.User.Login().subscribe(x=>{
      this.structure = x;
      this.eventSubject.next("logged_in");
    });
  }

}

export interface Role{
    type:string;
    postName:string;
    userAccName:string;
}

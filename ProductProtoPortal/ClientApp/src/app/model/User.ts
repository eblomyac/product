import {DataService} from "../services/data.service";
import {Observable, Subject} from "rxjs";

export interface IUser{
    name:string;
    mail:string;
    accName:string;
    isAdmin:boolean;
    isOperator:boolean;
    isMaster:boolean;
    isPersonnel:boolean;
    postIdMaster:string[];
    roles:Role[];
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
  public checkRoles(){
    this.dataService.User.Login().subscribe(x=>{

      if(this.structure && x){

        this.structure.roles = x.roles;
        this.structure.isAdmin = x.isAdmin;
        this.structure.isOperator = x.isOperator;
        this.structure.isMaster = x.isMaster;
        this.structure.postIdMaster = x.postIdMaster;
        this.structure.isPersonnel = x.isPersonnel;
      }
    });
  }

}

export interface Role{
    type:string;
    postName:string;
    userAccName:string;
}

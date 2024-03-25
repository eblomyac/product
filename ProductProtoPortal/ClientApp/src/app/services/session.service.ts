import { Injectable } from '@angular/core';
import {DataService} from "./data.service";
import {User} from "../model/User";
import {Observable, Subject, Subscription} from "rxjs";



@Injectable({
  providedIn: 'root'
})
export class SessionService {
  public currentUser:User|null=null;
  private subs:Subscription[]=[];
  private event:Subject<any> = new Subject<any>();
  public OnEvent:Observable<any> = this.event.asObservable();
  constructor(private dataService:DataService) {

    this.currentUser = new User(this.dataService);
    this.subs.push(this.currentUser.Events.subscribe(x=>{
      this.userEvent(x);
    }));
  }
  CheckRoles(){
    this.currentUser?.checkRoles();
  }
  private userEvent(event:any){
      if(event=="logged_in"){
        this.event.next(event);
      }
  }
}

import {TransportService} from "../transport.service";
import {DataService} from "../data.service";
import {map, Observable} from "rxjs";
import {TechCard} from "../../model/TechCard";
import {HttpParams} from "@angular/common/http";
import {ApiAnswer} from "../../model/ApiAnswer";
import {IWork} from "../../model/Work";

export class TechCardData{
  constructor(private transportService: TransportService, private dataService: DataService) {

  }
  public Card(Article:string):Observable<TechCard|null>{
    return this.transportService.Get('/techcard/card?article='+encodeURIComponent(Article), new HttpParams()).pipe(map<ApiAnswer|null,TechCard|null>(x=>{
      if(x){
        if(x.isSuccess){
          return (x.result as TechCard);
        }
      }
      return null;
    }));
  }
}

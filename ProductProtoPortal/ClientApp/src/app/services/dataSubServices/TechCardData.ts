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
  public PartItemOf(article:string,format:string):Observable<any|null|'load'>{
    return this.transportService.Get('/techcard/PartMemberOf?article='+encodeURIComponent(article)+`&format=${format}`, new HttpParams())
      .pipe(map<ApiAnswer|null|'not ended',any|null|'load'>(x=>{
        if(x!=null && x!='not ended'){
          if(x.isSuccess){
            return (x.result);
          }
        }
        if(x=='not ended'){
          return 'load'
        }
        return null;
      }));
  }
  public Composition(article:string):Observable<any|null|'load'>{
    return this.transportService.Get('/techcard/Composition?article='+encodeURIComponent(article), new HttpParams())
      .pipe(map<ApiAnswer|null|'not ended',any|null|'load'>(x=>{
        if(x!=null && x!='not ended'){
          if(x.isSuccess){
            return (x.result);
          }
        }
        if(x=='not ended'){
          return 'load'
        }
        return null;
      }));
  }
  public UploadPhoto(article:string, data:any[]):Observable<boolean|null>{
    return this.transportService.Post('/techcard/uploadPhoto?article='+encodeURIComponent(article), new HttpParams(), data)
      .pipe(map<ApiAnswer|null|'not ended',boolean|null>(x=>{
        if(x!=null && x!='not ended'){
        if(x.isSuccess){
          return (x.result);
        }
      }
      return null;
    }));
  }
  public Card(Article:string):Observable<TechCard|null|'load'>{
    return this.transportService.Get('/techcard/card?article='+encodeURIComponent(Article), new HttpParams()).pipe(map<ApiAnswer|null|'not ended',TechCard|null|'load'>(x=>{

      if(x!=null && x!='not ended'){
        if(x.isSuccess){
          return (x.result as TechCard);
        }
      }
      if(x=='not ended'){
        return 'load'
      }
      return null;
    }));
  }
}

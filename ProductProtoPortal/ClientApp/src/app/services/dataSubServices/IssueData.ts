
import {map, Observable} from "rxjs";

import {HttpParams} from "@angular/common/http";
import {Issue, IssueTemplate} from "../../model/Issue";
import {TransportService} from "../transport.service";
import {DataService} from "../data.service";
import {ApiAnswer} from "../../model/ApiAnswer";


export class IssueData {
  constructor(private transportService: TransportService, private dataService: DataService) {
  }

  public ListTemplates():Observable<IssueTemplate[]|null>{
    return this.transportService.Get('/issues/', new HttpParams()).pipe(map<ApiAnswer|null|'not ended',IssueTemplate[]|null>(x=> {
      if (x != null && x!='not ended') {
        if (x.isSuccess) {
          return x.result as IssueTemplate[];
        } else {
          return [];
        }
      }
      return null;
    }));
  }
  public UpdateTemplates(templates:IssueTemplate[]):Observable<IssueTemplate[]|null> {
    return this.transportService.Put('/issues/update', new HttpParams(), templates).pipe(map<ApiAnswer|null|'not ended', IssueTemplate[] | null>(x => {
      if (x != null && x!='not ended') {
        if (x.isSuccess) {
          return x.result as IssueTemplate[];
        } else {
          return [];
        }

      }
      return null;
    }));
  }

  public Register(wi:Issue, workId:number):Observable<Issue|null>{
    return this.transportService.Post('/issues/register', new HttpParams().append('workId',workId), wi).pipe(map<ApiAnswer|null|'not ended', Issue | null>(x => {
      if (x != null && x!='not ended') {
        if (x.isSuccess) {
          return x.result as Issue;
        } else {
          return null;
        }

      }
      return null;
    }));
  }

  public Resolve(id:number):Observable<boolean|null>{
    return this.transportService.Get(`/issues/${id}/resolve`, new HttpParams()).pipe(map<ApiAnswer|null|'not ended',boolean|null>(x=> {
      if (x != null && x!='not ended') {
       return x.isSuccess;
      }
      return null;
    }));
  }
}

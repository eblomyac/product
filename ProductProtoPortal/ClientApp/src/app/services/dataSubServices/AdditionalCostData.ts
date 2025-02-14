import {TransportService} from "../transport.service";
import {DataService} from "../data.service";
import {HttpParams} from "@angular/common/http";
import {map, Observable} from "rxjs";
import {ApiAnswer} from "../../model/ApiAnswer";
import {DailySource} from "../../model/DailySource";
import {AdditionalCost, AdditionalCostTemplate} from "../../model/AdditionalCost";
import {Work} from "../../model/Work";

export class AdditionalCostData {
  constructor(private transportService: TransportService, private dataService: DataService) {
  }

  public CreatePostAddCost(postId:string, lineId:string, cost:AdditionalCost[]):Observable<Work|null>{
    return this.transportService.Post('/AdditionalCost/PostAdditionalCostCreate', new HttpParams().append('post', postId).append('prodLine', lineId), cost)
      .pipe(map<ApiAnswer | null, Work | null>(x => {
      if (x != null) {
        if (x.isSuccess) {
          return x.result as Work;
        } else {
          return null;
        }
      }
      return null;
    }));
  }
  public CreateWorkAddCost(cost:AdditionalCost):Observable<AdditionalCost|null>{
    return this.transportService.Post('/AdditionalCost/Create', new HttpParams(),cost)
      .pipe(map<ApiAnswer | null, AdditionalCost | null>(x => {
        if (x != null) {
          if (x.isSuccess) {
            return x.result as AdditionalCost;
          } else {
            return null;
          }
        }
        return null;
      }));
  }
  public SaveTemplateList(list:AdditionalCostTemplate[]):Observable<AdditionalCostTemplate[] | null>{
    return this.transportService.Post('/AdditionalCost/SaveList', new HttpParams(),list)
      .pipe(map<ApiAnswer | null, AdditionalCostTemplate[] | null>(x => {
        if (x != null) {
          if (x.isSuccess) {
            return x.result as AdditionalCostTemplate[];
          } else {
            return [];
          }
        }
        return null;
      }));
  }
  public TemplatesListItem(): Observable<AdditionalCostTemplate[] | null> {
    {
      return this.transportService.Get('/AdditionalCost/TemplateListForItem', new HttpParams())
        .pipe(map<ApiAnswer|null|'not ended', AdditionalCostTemplate[] | null>(x => {
          if (x != null && x!='not ended') {
            if (x.isSuccess) {
              return x.result as AdditionalCostTemplate[];
            } else {
              return [];
            }
          }
          return null;
        }));
    }
  }
  public TemplatesListPost(): Observable<AdditionalCostTemplate[] | null> {
    {
      return this.transportService.Get('/AdditionalCost/TemplateListForPost', new HttpParams())
        .pipe(map<ApiAnswer|null|'not ended', AdditionalCostTemplate[] | null>(x => {
          if (x != null && x!='not ended') {
            if (x.isSuccess) {
              return x.result as AdditionalCostTemplate[];
            } else {
              return [];
            }
          }
          return null;
        }));
    }
  }
  public TemplatesList(): Observable<AdditionalCostTemplate[] | null> {
    {
      return this.transportService.Get('/AdditionalCost/TemplateList', new HttpParams())
        .pipe(map<ApiAnswer|null|'not ended', AdditionalCostTemplate[] | null>(x => {
          if (x != null && x!='not ended') {
            if (x.isSuccess) {
              return x.result as AdditionalCostTemplate[];
            } else {
              return [];
            }
          }
          return null;
        }));
    }
  }
}

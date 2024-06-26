import {TransportService} from "../transport.service";
import {DataService} from "../data.service";
import {HttpParams} from "@angular/common/http";
import {map, Observable} from "rxjs";
import {ApiAnswer} from "../../model/ApiAnswer";
import {DailySource} from "../../model/DailySource";
import {AdditionalCost, AdditionalCostTemplate} from "../../model/AdditionalCost";

export class AdditionalCostData {
  constructor(private transportService: TransportService, private dataService: DataService) {
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
  public TemplatesList(showDisabled = false): Observable<AdditionalCostTemplate[] | null> {
    {
      return this.transportService.Get('/AdditionalCost/TemplateList', new HttpParams().append('showDisabled', showDisabled))
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
  }
}

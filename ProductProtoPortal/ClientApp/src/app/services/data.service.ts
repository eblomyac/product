import { Injectable } from '@angular/core';
import {TransportService} from "./transport.service";
import {WorkData} from "./dataSubServices/WorkData";
import {IssueData} from "./dataSubServices/IssueData";
import {PostData} from "./dataSubServices/PostData";
import {UserData} from "./dataSubServices/UserData";
import {StatisticData} from "./dataSubServices/StatisticData";
import {TechCardData} from "./dataSubServices/TechCardData";
import {PriorityData} from "./dataSubServices/PriorityData";
import {TransferData} from "./dataSubServices/TransferData";
import {DailySourceData} from "./dataSubServices/DailySource";
import {AdditionalCostData} from "./dataSubServices/AdditionalCostData";
import {InfoData} from "./dataSubServices/InfoData";
import {OperatorData} from "./dataSubServices/OperatorData";
import {OTKData} from "./dataSubServices/OTKData";
import {HrData} from "./dataSubServices/hrData";


@Injectable({
  providedIn: 'root'
})
export class DataService {


  User:UserData;
  Post:PostData;
  Work:WorkData;
  Issue:IssueData;
  Statistic:StatisticData;
  TechCard:TechCardData;
  PriorityData:PriorityData;
  TransferData:TransferData;
  DailySource:DailySourceData;
  AdditionalCostData:AdditionalCostData;
  InfoData:InfoData;
  OTK:OTKData;
  OperatorData:OperatorData;
  HR:HrData;
  constructor(public transport:TransportService) {
      this.User = new UserData(transport);
      this.Post = new PostData(transport,this);
      this.Work = new WorkData(transport,this);
      this.Issue = new IssueData(transport,this);
      this.Statistic = new StatisticData(transport,this);
      this.TechCard = new TechCardData(transport,this);
      this.PriorityData = new PriorityData(transport,this);
      this.TransferData = new TransferData(transport,this);
      this.DailySource = new DailySourceData(transport,this);
      this.AdditionalCostData = new AdditionalCostData(transport,this);
      this.InfoData = new InfoData(transport,this);
      this.OperatorData = new OperatorData(transport,this);
      this.OTK = new OTKData(transport,this);
      this.HR = new HrData(transport,this);
  }
}

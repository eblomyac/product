export interface AdditionalCost{
  id:number;
  description:string;
  comment:string;
  cost:number;
  additionalCostTemplate?:AdditionalCostTemplate;
  additionalCostTemplateId:number;
  workId:number;
  subPost:string;
}
export interface AdditionalCostTemplate{
  id:number;
  name:string;
  canPost:boolean;
  canItem:boolean;

}

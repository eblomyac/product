export interface AdditionalCost{
  id:number;
  description:string;
  comment:string;
  cost:number;
  additionalCostTemplate?:AdditionalCostTemplate;
  additionalCostTemplateId:number;
  workId:number;
}
export interface AdditionalCostTemplate{
  id:number;
  name:string;
  disabled:boolean;

}

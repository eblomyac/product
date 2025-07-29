
export interface OTKAvailableOperation{
   id:number;
   shortName:string;
   fullName:string;
   productLine:string;
   targetValue:string;
   productLines:string[];
   values:string[];
   targetValues:string[];
   availableValues:string;
   availableTargetValues:string;
}
export interface OTKWorker{
  id:number;
  name:string;
}
export interface OTKTargetValue{
  id:number;
  displayName:string;
  source:string;
  target:string;
  values:string;
}

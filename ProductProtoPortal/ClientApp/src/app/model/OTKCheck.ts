export interface OTKCheck {
  id: number;
  stamp: Date;
  productLine: string;
  article: string;
  orderNumber: number;
  orderLineNumber: number;
  iteration: number;
  productCount: number;
  checkedCount: number;
  worker: string;
  lines: OTKCheckLine[];
  name:string;
  result:string;
}

export interface OTKCheckLine {
  id: number;
  otkCheckId: number;
  otkCheck: OTKCheck;
  shortName: string;
  fullName: string;
  value: string;
  description?: string;
  targetValue: string;
  measuredValue: string;
}

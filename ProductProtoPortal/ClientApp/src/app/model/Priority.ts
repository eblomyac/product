export interface OrderPriority{
  orderNumber:number;
  priority:number;
  articles:ArticlePriority[];
}
export interface ArticlePriority{
  orderNumber:number;
  article:string;
  priority:number;
}

export interface Issue{
  id:number;
  created:Date;
  resolved:Date;
  workId:number;
  description:string;
  templateId:number;
  returnBackPostId:string;
  returnedFromPostId:string;
}
export interface IssueTemplate{
  id:number;
  name:string;
}

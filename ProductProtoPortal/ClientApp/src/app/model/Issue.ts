export interface Issue{
  id:number;
  created:Date;
  resolved:Date;
  workId:number;
  description:string;
  templateId:number;
}
export interface IssueTemplate{
  id:number;
  name:string;
}

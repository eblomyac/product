import { Component } from '@angular/core';
import {DataService} from "../../../services/data.service";
import {FormControl} from "@angular/forms";
import {map, Observable, startWith} from "rxjs";
import {Clipboard} from "@angular/cdk/clipboard"
export interface CostData{
  order:string;
  article:string;
  count:number;
  costs:number[];
}
@Component({
  selector: 'app-work-cost-info',
  templateUrl: './work-cost-info.component.html',
  styleUrl: './work-cost-info.component.css'
})
export class WorkCostInfoComponent {

  isLoadPosts=false;
  isLoadArticles=false;
  posts:string[]=[];
  hideNulls=false;

  articles:string[]=[];
  filteredArticles:Observable<string[]>|null=null;
  costData:CostData[]=[];

  PostSum(postIndex:number):number{
    return this.costData.reduce((sum,x)=>x.costs[postIndex]*x.count+sum,0);
  }
  constructor(private data:DataService,private clipboard: Clipboard) {
    this.isLoadPosts=true;
    this.isLoadArticles = true;
    this.myControl.disable();
    this.data.InfoData.PostList().subscribe(x=>{
      if(x && x.length>0){
        this.posts = x;

        this.isLoadPosts=false;
      }

    })
    this.data.InfoData.ArticleList().subscribe(x=>{
      if(x && x.length>0){
        this.articles = x;
        this.isLoadArticles=false;
        this.myControl.enable();
      }

    })
  }

  myControl = new FormControl<string>('');

  copyToClip(){
    let s = "Заказ\tАртикул\tКоличество";
    this.posts.forEach(x=>{
      s+='\t'+x;
    });
    s+='\tСумма\r\n';
    this.costData.forEach(x=>{
      s+=x.order+'\t';
      s+=x.article+'\t';
      s+=x.count+'\t';
      x.costs.forEach(z=>{
        if(this.hideNulls && z*x.count<0.001){
          s+='\t';
        }else{
          s+=(z*x.count).toFixed(2).replace('.',',')+'\t';
        }
      });
      s+='\r\n';
    });
    s+='Сумма:\t\t\t';
    this.posts.forEach((x,i)=>{
      let z = this.PostSum(i);
      if(this.hideNulls && z<0.001) {
        s+='\t';
      }else{
        s+=z.toFixed(2).replace('.',',')+'\t';
      }
    });
    s+=this.PostSum(this.posts.length).toFixed(2).replace('.',',')+'\r\n';
    this.clipboard.copy(s);
  }
  loadArticleData(article:string){
    this.data.InfoData.CostData(article).subscribe(x=>{
      if(x){
        if(x.length>0){
          this.costData.push({order:'',article:article,count:1, costs:x});
        }

      }

    });
  }
  articleImport(){
    navigator.clipboard.readText().then(t=>{
      console.log(t);
      let arts:string[]=t.split('\r\n').filter(z=>z.length>2);
      this.data.InfoData.CostDataBatch(arts).subscribe((z=>{
        z.forEach(((art,index)=>{
          this.costData.push({order:'',article:arts[index],count:1,costs:art})
        }));
      }));
    })
  }
  articleAdd(art:string|null){
    this.myControl.setValue('');
    if(art){
      this.loadArticleData(art);
    }
  }
  ngOnInit() {
    this.filteredArticles = this.myControl.valueChanges.pipe(
      startWith(''),
      map(value => {
        const name =value;
        return name ? this._filter(name as string) : this.articles.slice();
      }),
    );
  }


  private _filter(name: string): string[] {
    const filterValue = name.toLowerCase();
    return this.articles.filter(art => art.toLowerCase().includes(filterValue));
  }
}

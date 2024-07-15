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
        if(this.hideNulls && z<0.001){
          s+='\t';
        }else{
          s+=z.toFixed(2).replace('.',',')+'\t';
        }

      });
      s+='\r\n';
    });
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

import {Component, OnInit} from '@angular/core';
import {ArticlePriority, OrderPriority} from "../../model/Priority";
import {MatIconModule} from "@angular/material/icon";
import {MatInputModule} from "@angular/material/input";
import {FormsModule} from "@angular/forms";
import {DataService} from "../../services/data.service";
import {NgForOf, NgIf} from "@angular/common";
import {MatButtonModule} from "@angular/material/button";
import {animate, keyframes, style, transition, trigger} from "@angular/animations";

@Component({
  selector: 'app-priority-list',
  templateUrl: './priority-list.component.html',
  styleUrl: './priority-list.component.css',
  animations:[
    trigger(
      'valueChanged',
      [
        transition('void => *', []),
        transition('* => void', []),
        transition('* => *', [
          animate(900, keyframes([
            style ({ color: '#000000', offset: 0.0, background : '#ffffff'}),
            style ({ color: 'inherit', offset: 1.0, background : 'inherit'}),
          ])),
        ]),
      ]),
  ]
}
)
export class PriorityListComponent implements OnInit{

  articleFilter = '';
  orderFilter = 0;

  ordersPriorityData:OrderPriority[]|undefined;
  articlePriorityData:ArticlePriority[]|undefined;
  orders:OrderPriority[]|undefined;
  articles:ArticlePriority[]|undefined;

  changeOrderPriority(orderNumber:number, change:number){
    this.ordersPriorityData?.filter(x=>x.orderNumber == orderNumber).every(x=>x.priority+= change);
    this.articlePriorityData?.filter(x=>x.orderNumber == orderNumber).every(x=>x.priority += change);

  }
  changeArticlePriority(orderNumber:number, article:string, change:number){
    this.articlePriorityData?.filter(x=>x.orderNumber == orderNumber && x.article == article).every(x=>x.priority+=change);
  }

  orderClick(order:number){
    if(this.orderFilter != order){
      this.orderFilter = order;
      this.filterUpdate();
    }else{
      this.orderFilter = 0;
      this.filterUpdate();
    }
  }
  articleClick(article:string){
    if(this.articleFilter != article){
      this.articleFilter = article;
      this.filterUpdate();
    }else{
      this.articleFilter = '';
      this.filterUpdate();
    }
  }
  filterUpdate(){
    if(this.articleFilter.length>0 && this.orderFilter != 0){
      //filter by art and order
      this.orders = this.ordersPriorityData?.filter(x=>x.orderNumber.toString().includes(this.orderFilter.toString())&&x.articles.filter(z=>z.article.includes(this.articleFilter)).length>0);
      this.articles = this.articlePriorityData?.filter(x=>x.article.includes(this.articleFilter) && x.orderNumber.toString().includes(this.orderFilter.toString()));
    }else if(this.articleFilter.length>0){
      //filter by article only
      this.articles = this.articlePriorityData?.filter(x=>x.article.includes(this.articleFilter));
      this.orders = this.ordersPriorityData?.filter(x=>x.articles.filter(z=>z.article.includes(this.articleFilter)).length>0);
    }else if(this.orderFilter != 0){
      //filter bu order only
      this.orders = this.ordersPriorityData?.filter(x=>x.orderNumber.toString().includes(this.orderFilter.toString()));
      this.articles = this.articlePriorityData?.filter(x=>x.orderNumber.toString().includes(this.orderFilter.toString()));
    }else{
      //no filter
      this.articles = this.articlePriorityData;
      this.orders = this.ordersPriorityData;
    }
  }

  selectMany<TIn, TOut>(input: TIn[], selectListFn: (t: TIn) => TOut[]): TOut[] {
    return input.reduce((out, inx) => {
      let item = selectListFn(inx);
      if(item){
        out.push(...item);
      }
      return out;
    }, new Array<TOut>());
  }
  constructor(private data:DataService) {

  }
  load(){
    this.data.PriorityData.List().subscribe(x=>{
      if(x){
        this.ordersPriorityData = x;
        this.articlePriorityData = this.selectMany(this.ordersPriorityData, x=>x.articles);
        this.orderFilter = 0;
        this.articleFilter = '';
       this.filterUpdate();
      }else{
        this.ordersPriorityData = [];
        this.articlePriorityData = [];
      }

    });
  }
  ngOnInit(){
  this.load();


  }
  save(){
    if(this.ordersPriorityData){

      this.data.PriorityData.Save(this.ordersPriorityData).subscribe(x=>{
        this.load();
      });
    }
  }
}

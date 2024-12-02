import {Component, OnInit} from '@angular/core';
import {DataService} from "../../../services/data.service";
import {IPost} from "../../../model/Post";
import {F} from "@angular/cdk/keycodes";

@Component({
  selector: 'app-total-order-statistic',
  templateUrl: './total-order-statistic.component.html',
  styleUrl: './total-order-statistic.component.css'
})
export class TotalOrderStatisticComponent implements OnInit{

  articleStat:any[]=[];
  stat:any[]=[];
  posts:IPost[]=[];
  isLoading=false;
  articleFilter='';
  orderFilter='';
  hideCompleted = true;
  constructor(private data:DataService) {

  }
  ngOnInit() {
      this.load();
  }
  filterUpdate(){


    if(this.articleFilter.length < 1 && this.orderFilter.length<1){
      this.articleStat = this.stat;
    }else if(this.articleFilter.length>0 && this.orderFilter.length>0){
      this.articleStat = this.stat.filter((x:any)=>{
        return x.Order.toString().includes(this.orderFilter) && x.Article.toString().includes(this.articleFilter)
      })
    }else if(this.articleFilter.length>0){
      this.articleStat = this.stat.filter((x:any)=>{
        return x.Article.toString().includes(this.articleFilter)
      })
    }else{
      this.articleStat = this.stat.filter((x:any)=>{
        return x.Order.toString().includes(this.orderFilter)
      })
    }
    if(this.hideCompleted){
      this.articleStat = this.articleStat.filter((x:any)=>{
        return  !x.IsEnded;
      });
    }

  }
  print(){
      this.data.Statistic.PrintTotalOrderStat(this.articleFilter,this.orderFilter).subscribe(x=>{

        if(x){
          window.open(x.link,'_blank')
        }
      });
  }
  load(){
    this.isLoading = true;
    this.data.Post.List().subscribe(x=>{
      if(x){
        this.posts = x;

      }

    });
    this.data.Statistic.TotalOrderStat().subscribe(x=>{

      if(x){
        this.isLoading = false;
        this.stat = x.ArticleStat;
        this.filterUpdate();

      }
    })

  }

  protected readonly F = F;
}

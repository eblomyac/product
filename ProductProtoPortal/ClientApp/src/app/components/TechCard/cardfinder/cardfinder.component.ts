import { Component, OnInit } from '@angular/core';
import {DataService} from "../../../services/data.service";

@Component({
  selector: 'app-cardfinder',
  templateUrl: './cardfinder.component.html',
  styleUrls: ['./cardfinder.component.css']
})
export class CardfinderComponent implements OnInit {

  isLoadArticles=true;
  articles:string[]=[];
  filteredArticles :string[] = [];
  filter:string='';
  selectedArticle:string='';
  constructor(private data:DataService) {
    this.data.InfoData.ArticleList().subscribe(x=>{
      if(x && x.length>0){
        this.articles = x;
        this.isLoadArticles=false;
        this.filteredArticles = x;
      }

    })
  }
  filterChange(){
    if(this.filter.length==0){
      this.filteredArticles=this.articles;
    }
    else{
      this.filteredArticles = this.articles.filter(x=>x.includes(this.filter));

    }
  }
  ngOnInit(): void {
  }

}

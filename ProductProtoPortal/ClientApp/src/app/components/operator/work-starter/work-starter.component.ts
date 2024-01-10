import { Component, OnInit } from '@angular/core';
import {DataService} from "../../../services/data.service";


@Component({
  selector: 'app-work-starter',
  templateUrl: './work-starter.component.html',
  styleUrls: ['./work-starter.component.css']
})
export class WorkStarterComponent implements OnInit {

  isLoading=false;
  suggestions:any[]=[];

  constructor(private data:DataService) { }

  ngOnInit(): void {
    this.loadSuggestions();
  }

  loadSuggestions(){
    this.isLoading=true;
    this.data.Work.UnstartedSuggestions().subscribe(x=>{
      if(x){
        this.isLoading=false;
        this.suggestions = x;
      }
    });
  }
  makeDefault(){
    this.suggestions.forEach(x=>{
      x.selectedPosts = [];
      x.selectedPosts.push(x.availablePosts[0]);
    });
  }
  startWorks(){
    let sendArray:any[] = [];
    this.suggestions.forEach(x=>{
      if(x.selectedPosts.length>0 && x.selectedPosts.indexOf('')>0){
        x.selectedPosts=[''];
      }else{
        sendArray.push(x);
      }
    });
    this.isLoading=true;
    this.data.Work.StartWorks(sendArray).subscribe(x=>{
      if(x!=null){
        this.isLoading=false;
        this.loadSuggestions();
      }

    });
  }
}

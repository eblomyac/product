import {Component, ElementRef, OnInit, ViewChild} from '@angular/core';
import {DataService} from "../../../services/data.service";
import {MatCheckbox} from "@angular/material/checkbox";


@Component({
  selector: 'app-work-starter',
  templateUrl: './work-starter.component.html',
  styleUrls: ['./work-starter.component.css']
})
export class WorkStarterComponent implements OnInit {

  isLoading=false;
  suggestions:any[]=[];
  filteredSuggestions:any[]=[];
  orderFilter = 0;
  editMode=false;
  toDelete:Array<{order:number,article:string}>=[];
  delAll=false;


  changeDeleteToAll(value:any){

    let b = value;
    if(b){
      this.toDelete = [];
      this.filteredSuggestions.forEach(x=>this.toDelete.push({order:x.orderNumber, article: x.article}));
    }
    else{
      this.toDelete=[];
    }
  }
  isSelected(order:number,article:string):boolean{
    return  this.toDelete.filter(x=>x.order == order && x.article == article).length>0;
  }
  setSelected(order:number,article:string, value:any){
    let b = value.checked;

    let existIndex = this.toDelete.findIndex(x=>x.article == article&&x.order==order);
    if(b){
      if(existIndex==-1){
        this.toDelete.push({order:order,article:article});
      }
    }else{
      if(existIndex!=-1){
        this.toDelete.splice(existIndex,1);
      }
    }
  }

  filterUpdate(){
    if(this.orderFilter == 0 || this.orderFilter==null){
      this.toDelete=[];
    this.delAll =false;
      this.filteredSuggestions = this.suggestions;
    }else{
      this.toDelete=[];
      this.delAll=false;
      this.suggestions.forEach(x=>x.selectedPosts='');
      this.filteredSuggestions = this.suggestions.filter(x=>x.orderNumber.toString().includes(this.orderFilter.toString()));
    }
  }
  removeOrders(){
    console.log(this.toDelete);
    let data:Array<string> = [];
    this.toDelete.forEach(x=>data.push(`${x.order}\t${x.article}`));
    this.data.Work.RemoveUnstarted(data).subscribe(x=>{
      this.loadSuggestions();
    })

  }

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
        this.filterUpdate();
      }
    });
  }
  makeDefault(){
    this.filteredSuggestions.forEach(x=>{
      x.selectedPosts = [];
      x.selectedPosts.push(x.availablePosts[0]);
    });
  }
  startWorks(){
    let sendArray:any[] = [];
    this.filteredSuggestions.forEach(x=>{
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

import { Component, OnInit } from '@angular/core';
import {DataService} from "../../../services/data.service";
import {IPost} from "../../../model/Post";

@Component({
  selector: 'app-post-settings',
  templateUrl: './post-settings.component.html',
  styleUrls: ['./post-settings.component.css']
})
export class PostSettingsComponent implements OnInit {

  posts:IPost[]=[];
  constructor(private data:DataService) { }

  ngOnInit(): void {
    this.loadPosts();
  }

  loadPosts(){
    this.data.Post.List().subscribe(x=>{
      if(x&&x.length>0){
        this.posts = x;
      }
    });
  }
  savePosts(){
    this.data.Post.Update(this.posts).subscribe(x=>{

    });
  }
  addPost(name:string, keys:string, po:any, tableName:string){
    if(name.length<1){
      return;
    }
    let exist = this.posts.find(x=>x.name == name);
    if(exist){

    }else{
      let p :IPost = {name:name, keys:keys, disabled:true, productOrder:po, isShared:false, tableName:tableName};
      this.posts.push(p);
    }
  }

}

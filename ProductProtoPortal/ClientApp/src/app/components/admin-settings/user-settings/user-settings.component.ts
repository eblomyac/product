import { Component, OnInit } from '@angular/core';
import {DataService} from "../../../services/data.service";
import {IUser} from "../../../model/User";
import {IPost} from "../../../model/Post";
import {MatSelectChange} from "@angular/material/select";
import {SessionService} from "../../../services/session.service";

@Component({
  selector: 'app-user-settings',
  templateUrl: './user-settings.component.html',
  styleUrls: ['./user-settings.component.css']
})
export class UserSettingsComponent implements OnInit {

  userList:IUser[]=[];
  posts:IPost[]=[];
  constructor(private data:DataService, private session:SessionService) { }

  ngOnInit(): void {
    this.loadPostList();
    this.loadUserList();
  }




  loadUserList(){
    this.data.User.List().subscribe(x=>{ if(x && x.length>0){
      this.userList = x;
    }
    });
  }
  loadPostList(){
    this.data.Post.List().subscribe(x=>{if(x && x.length>0){
      this.posts=x;}
    });
  }
  saveChanges(){
    this.data.User.Update(this.userList).subscribe(x=>{
      this.loadUserList();
      this.session.CheckRoles();
    });
  }
}

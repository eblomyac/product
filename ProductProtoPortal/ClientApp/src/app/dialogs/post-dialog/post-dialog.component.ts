import {Component, Inject, OnInit} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialogRef} from "@angular/material/dialog";
import {MatCheckboxChange} from "@angular/material/checkbox";


@Component({
  selector: 'app-post-dialog',
  templateUrl: './post-dialog.component.html',
  styleUrls: ['./post-dialog.component.css']
})
export class PostDialogComponent implements OnInit {

  selectedPosts:string[]=[];
  availablePost:string[]=[];
  mainPost:string='';


  constructor(private dialogRef:MatDialogRef<PostDialogComponent>,@Inject(MAT_DIALOG_DATA)
    public data: {posts:string[], onlyMain:boolean})
  {
    if(data == null || data.posts == null || data.posts.length ==0){
      this.dialogRef.close(null);
    }
  }

  ngOnInit(): void {
    this.mainPost = this.data.posts[0];
    this.filterPosts();
  }
  cancel(){
    this.dialogRef.close(null);
  }
  filterPosts(){
    this.availablePost = this.data.posts.filter(x=>x != this.mainPost);
  }
  matCheckChange(e:MatCheckboxChange, post:string){
    let existIndex = this.selectedPosts.findIndex(x=>x==post);
    if(e.checked){
      if(existIndex==-1){
        this.selectedPosts.push(post);
      }
    }else{
      if(existIndex!=-1){
        this.selectedPosts.slice(existIndex,1);
      }
    }
  }
  ok(){
    let result= {main:this.mainPost, additional:this.selectedPosts};
    this.dialogRef.close(result);
  }
}

import {Component, Inject} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialogRef} from "@angular/material/dialog";
import {DataService} from "../../services/data.service";
import {IPost} from "../../model/Post";

@Component({
  selector: 'app-photo-upload',
  templateUrl: './photo-upload.component.html',
  styleUrl: './photo-upload.component.css'
})
export class PhotoUploadComponent {

  isLoading=false;
  showUrl:string|ArrayBuffer|null=null;

  lastLoadedImage:string|ArrayBuffer|null=null;
  lastLoadedImageName:string='';
  imageData:Array<{image:string|ArrayBuffer, fileName:string, post:string, description:string}>=[];

  selectedPost='';
  description='';

  postList:IPost[]=[];
  constructor(private dialogRef:MatDialogRef<PhotoUploadComponent>,@Inject(MAT_DIALOG_DATA)
  public data: {article:string, dataService:DataService})
  {
      this.data.dataService.Post.List().subscribe(x=>{
        if(x){this.postList = x;}
      })
  }
  imageUpload(e:any){
    const element = e.currentTarget as HTMLInputElement;
    let fileList: FileList | null = element.files;
    if (fileList) {
      console.log("FileUpload -> files", fileList);
      let reader = new FileReader();
      reader.onloadend =async ()=>{

        console.log(reader);
       this.lastLoadedImage=reader.result;

      }
      for (let i = 0; i < fileList.length; i++) {
        this.lastLoadedImageName = fileList.item(i)!.name;
        reader.readAsDataURL(fileList!.item(i)!);
      }


    }
  }
  removeItem(fileName:string){
    let i = this.imageData.findIndex(z=>z.fileName==fileName);
    if(i>=0){
      this.imageData.splice(i,1);
    }
  }
  add(){
    if(this.lastLoadedImage){
     this.imageData.push({image:this.lastLoadedImage, fileName:this.lastLoadedImageName, post: this.selectedPost, description:this.description});
      this.description='';
      this.lastLoadedImageName='';
      this.lastLoadedImage=null;
    }
  }
  save(){
    this.isLoading=true;
    this.data.dataService.TechCard.UploadPhoto(this.data.article,this.imageData).subscribe(x=>{
     if(x){
       this.isLoading=false;
       this.dialogRef.close(true)

     }
    })
  }
}

import {Component, Input, OnInit} from '@angular/core';
import {GalleryItem, ImageItem} from "ng-gallery";
import {ImageSet} from "../../../model/TechCard";

@Component({
  selector: 'app-image-set-view',
  templateUrl: './image-set-view.component.html',
  styleUrls: ['./image-set-view.component.css']
})
export class ImageSetViewComponent implements OnInit {

  gData:GalleryItem[]=[];
  @Input('ImageSet')ImageSet:ImageSet|null=null;
  constructor() { }

  ngOnInit(): void {
    this.galleryInit();
  }

  galleryInit(){
    this.gData = [];
    if(this.ImageSet){
      this.ImageSet.images.forEach(x=>{
        this.gData.push(new ImageItem({ src: x.url, thumb:x.url}))
      });
    }
  }
  getDescription(i:any){
    return this.ImageSet?.images[i].description;
  }

}

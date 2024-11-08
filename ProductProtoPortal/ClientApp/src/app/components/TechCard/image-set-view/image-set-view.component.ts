import {AfterViewInit, Component, Input, OnInit, TemplateRef, ViewChild} from '@angular/core';
import {Gallery, GalleryComponent, GalleryImageDef, GalleryItem, ImageItem} from "ng-gallery";
import {Image} from "../../../model/TechCard";



@Component({
  selector: 'app-image-set-view',
  templateUrl: './image-set-view.component.html',
  styleUrls: ['./image-set-view.component.css']
})
export class ImageSetViewComponent implements OnInit, AfterViewInit {


  gData:GalleryItem[]=[];
  @ViewChild(GalleryImageDef, { static: true }) imageDef!: GalleryImageDef;
  @Input('ImageSet')Images:Image[]=[];
  constructor(public gallery: Gallery) {

  }

  ngOnInit(): void {
    this.gallery.ref('lightbox', {
      imageTemplate: this.imageDef.templateRef
    }).load(this.gData);
    this.galleryInit();
    }
  ngAfterViewInit() {


  }



  galleryInit(){
    this.gData = [];
    if(this.Images.length>0){
      this.Images.forEach(x=>{
        this.gData.push(new ImageItem({ src: x.url, thumb:x.url, args: x.description}))
      });
    }


  }
  getDescription(i:any){
    return this.Images[i].description;
  }

}


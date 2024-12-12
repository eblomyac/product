import { Component } from '@angular/core';
import {ActivatedRoute} from "@angular/router";

@Component({
  selector: 'app-info',
  templateUrl: './info.component.html',
  styleUrl: './info.component.css'
})
export class InfoComponent {
  index = 0;
  constructor(private activatedRoute:ActivatedRoute) {
    this.activatedRoute.queryParams.subscribe(z=>{
      let tab = z['tab'];
      if(tab&&tab.length>0&&(tab==1||tab=='transfers')){
        this.index =1;
      }
    })
  }
}

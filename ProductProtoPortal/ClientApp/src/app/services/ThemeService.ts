import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";


import {StyleManagerService} from "./style-manager.service";
import {Observable, Subject} from "rxjs";

@Injectable()
export class ThemeService {
  public theme:'light'|'dark'='dark';

  private themeChanged:Subject<string>=new Subject<string>();
  public ThemeChanged:Observable<string> = this.themeChanged.asObservable();
  constructor(
    private styleManager:StyleManagerService
  ) {
    let t = localStorage.getItem('theme');
    if(t==null){
      this.setDark();
    }else{
      if(t=='dark'){
        this.setDark();
      }   else{
        this.setLight();
      }
    }
  }

  setDarkColorValues(){
    document.documentElement.style.setProperty('--scroll-color', '#a22d5d');
    document.documentElement.style.setProperty('--post-column-caption-color', '#4444444C');
    document.documentElement.style.setProperty('--drag-preview-color', 'rgb(255,255,255)');
    document.documentElement.style.setProperty( '--drag-placeholder-bg-color','rgba(108, 108, 108, 0.35)');


    document.documentElement.style.setProperty( '--post-bg-grad-from-color','rgba(50, 50, 50, 1)');
    document.documentElement.style.setProperty( '--post-bg-grad-to-color','rgba(0, 0, 0, 0.00)');


    document.documentElement.style.setProperty( '--bg-tooltip-color','rgba(108, 108, 108, 0.70)');
    document.documentElement.style.setProperty( '--tooltip-color','rgb(255,255,255)');


    document.documentElement.style.setProperty( '--post-bg-item-color','rgba(79, 79, 79, 0.25)');
    document.documentElement.style.setProperty( '--post-item-color','rgb(255, 255, 255)');


    document.documentElement.style.setProperty( '--table-hover-bg-color','#a22d5d20');
    document.documentElement.style.setProperty( '--table-even-bg-color','rgb(30, 30, 30,0.20)');
    document.documentElement.style.setProperty( '--table-sticky-bg-color','rgb(74, 74, 74,1)');


    document.documentElement.style.setProperty( '--load-block-back','rgba(32,32,32,0.8)');



  }
  setLightColorValues(){
    document.documentElement.style.setProperty('--scroll-color', '#3F50B4');
    document.documentElement.style.setProperty('--post-column-caption-color', 'rgba(228,243,253,0.3)');
    document.documentElement.style.setProperty('--drag-preview-color', 'rgb(30,30,30)');
    document.documentElement.style.setProperty( '--drag-placeholder-bg-color','rgba(229,229,229,0.35)');

    document.documentElement.style.setProperty( '--post-bg-grad-from-color','rgba(255, 255, 255, 1)');
    document.documentElement.style.setProperty( '--post-bg-grad-to-color','rgba(200, 200, 200, 0.00)');

    document.documentElement.style.setProperty( '--bg-tooltip-color','rgba(63,80,180,0.95)');
    document.documentElement.style.setProperty( '--tooltip-color','rgba(255,255,255,1)');

    document.documentElement.style.setProperty( '--post-bg-item-color','rgba(153,181,204,0.25)');
    document.documentElement.style.setProperty( '--post-item-color','rgb(0,0,0)');

    document.documentElement.style.setProperty( '--table-hover-bg-color','#3F50B420');
    document.documentElement.style.setProperty( '--table-even-bg-color','rgb(60, 60, 60,0.15)');
    document.documentElement.style.setProperty( '--table-sticky-bg-color','rgb(225, 225, 225,1)');

    document.documentElement.style.setProperty( '--load-block-back','rgba(220,220,220,0.8)');
  }

  setDark(){

    this.setDarkColorValues();
    this.styleManager.setStyle(
      "theme",
      `/assets/theme-css/pink-bluegrey.css`
    );
    this.theme = 'dark';
    localStorage.setItem('theme',this.theme)
    this.themeChanged.next(this.theme);
  }
  setLight(){
    this.setLightColorValues();
    this.styleManager.setStyle(
      "theme",
      `/assets/theme-css/indigo-pink.css`
    );this.theme = 'light';
    localStorage.setItem('theme',this.theme)
    this.themeChanged.next(this.theme);
  }
}

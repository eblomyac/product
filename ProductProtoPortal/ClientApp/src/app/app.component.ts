import {Component, ViewChild} from '@angular/core';
import {SessionService} from "./services/session.service";
import {Router} from "@angular/router";

import {DialogHandlerService} from "./services/dialog-handler.service";
import {Location} from "@angular/common";
import {ThemeService} from "./services/ThemeService";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent {


  availableMenu:any[]=[]

  constructor(private session:SessionService, public router:Router, private dialogHandler:DialogHandlerService, private locate:Location, public themeService:ThemeService) {


    this.router.events.subscribe(event => {
      // close sidenav on routing
    //  console.log(event);

    });
    this.session.OnEvent.subscribe(x=>{

      if(x == "logged_in"){

        if(this.session.currentUser && this.session.currentUser.structure){
          this.availableMenu = [];
          if(this.session.currentUser.structure.isAdmin){
            this.availableMenu.push({caption:"Администрирование",route:"/admin"})
          //  this.router.navigate(['/admin']);
          }if(this.session.currentUser.structure.isMaster){
            this.availableMenu.push({caption:"Мастер поста",route:"/post"})
         //   this.router.navigate(['/post']);
          }if(this.session.currentUser.structure.isOperator){
            this.availableMenu.push({caption:"Оператор ИТР",route:"/operate"})
         //   this.router.navigate(['/operate'])
          }if(this.session.currentUser.structure.isAdmin || this.session.currentUser.structure.isOperator){
            this.availableMenu.push({caption:"Приоритет работ",route:"/work-priority"})
          }

            this.availableMenu.push({caption:"Анализ",route:"/statistic"})
            //   this.router.navigate(['/operate'])

            if(this.locate.path().length==0) {
              this.router.navigate([this.availableMenu[0].route])
            }

        }
      }
    });
  }
}

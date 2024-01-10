import {Component, ViewChild} from '@angular/core';
import {SessionService} from "./services/session.service";
import {Router} from "@angular/router";

import {DialogHandlerService} from "./services/dialog-handler.service";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent {


  availableMenu:any[]=[]

  constructor(private session:SessionService, public router:Router, private dialogHandler:DialogHandlerService) {


    this.router.events.subscribe(event => {
      // close sidenav on routing

    });
    this.session.OnEvent.subscribe(x=>{
      console.log(x);
      if(x == "logged_in"){

        if(this.session.currentUser && this.session.currentUser.structure){
          if(this.session.currentUser.structure.isAdmin){
            this.availableMenu.push({caption:"Администрирование",route:"/admin"})
          //  this.router.navigate(['/admin']);
          }if(this.session.currentUser.structure.isMaster){
            this.availableMenu.push({caption:"Мастер поста",route:"/post"})
         //   this.router.navigate(['/post']);
          }if(this.session.currentUser.structure.isOperator){
            this.availableMenu.push({caption:"Оператор ИТР",route:"/operate"})
         //   this.router.navigate(['/operate'])
          }

            this.availableMenu.push({caption:"Анализ",route:"/statistic"})
            //   this.router.navigate(['/operate'])


            this.router.navigate([this.availableMenu[0].route])

        }
      }
    });
  }
}

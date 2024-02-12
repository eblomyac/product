import { Component } from '@angular/core';
import {DataService} from "../../../services/data.service";

@Component({
  selector: 'app-operator-utility',
  templateUrl: './operator-utility.component.html',
  styleUrl: './operator-utility.component.css'
})
export class OperatorUtilityComponent {

  isLoading=false;


  constructor(private data :DataService) {

  }
  updateDates(){
    this.isLoading=true;
    this.data.Work.UpdateDates().subscribe(x=>{

      if(x) {
        alert(x)
      }
    }, error => {
      let ans = error;
      console.log(ans);
    }, ()=>{
      this.isLoading=false;
    })
  }
}

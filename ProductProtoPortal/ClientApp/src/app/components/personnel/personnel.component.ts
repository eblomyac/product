import { Component } from '@angular/core';
import {DataService} from "../../services/data.service";

@Component({
  selector: 'app-personnel',
  templateUrl: './personnel.component.html',
  styleUrl: './personnel.component.css'
})
export class PersonnelComponent {

  constructor(private dataService:DataService) {

  }


}

import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-analytic',
  templateUrl: './analytic.component.html',
  styleUrls: ['./analytic.component.css']
})
export class AnalyticComponent implements OnInit {

  selectedIndex = 0;
  constructor() { }

  ngOnInit(): void {
  }

  indexChanged(e:any){
    console.log(this.selectedIndex);
    console.log(e);
  }

}

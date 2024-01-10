import { Component, OnInit } from '@angular/core';
import {DataService} from "../../services/data.service";

@Component({
  selector: 'app-admin-settings',
  templateUrl: './admin-settings.component.html',
  styleUrls: ['./admin-settings.component.css']
})
export class AdminSettingsComponent implements OnInit {

  constructor(private data:DataService) { }

  ngOnInit(): void {
  }



}

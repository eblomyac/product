import { Injectable } from '@angular/core';

import {ComponentType} from "@angular/cdk/overlay";
import {firstValueFrom} from "rxjs";
import {MatDialog, MatDialogConfig} from "@angular/material/dialog";

@Injectable({
  providedIn: 'root'
})
export class DialogHandlerService {

  public static Singleton:DialogHandlerService;
  constructor(private matDialog:MatDialog) {
    DialogHandlerService.Singleton = this;
  }

  public ask(component:ComponentType<any>,config:MatDialogConfig):Promise<any>{
    let d = this.matDialog.open(component,config);
    return firstValueFrom(d.afterClosed());
  }
}

import { Injectable } from '@angular/core';
import {MatLegacyDialog as MatDialog, MatLegacyDialogConfig as MatDialogConfig} from "@angular/material/legacy-dialog";
import {ComponentType} from "@angular/cdk/overlay";
import {firstValueFrom} from "rxjs";

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

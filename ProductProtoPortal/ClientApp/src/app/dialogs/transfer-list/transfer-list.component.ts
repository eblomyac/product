import {Component, Inject} from '@angular/core';
import {Transfer} from "../../model/Transfer";
import {MAT_DIALOG_DATA, MatDialogRef} from "@angular/material/dialog";
import {Work} from "../../model/Work";
import {DataService} from "../../services/data.service";

@Component({
  selector: 'app-transfer-list',
  templateUrl: './transfer-list.component.html',
  styleUrl: './transfer-list.component.css'
})
export class TransferListComponent {

  selectedTransfer:Transfer|null=null;
  edit=false;
  isLoading=false;

  isAllCancel=false;
  allCancelReason = '';

  viewItem:Transfer|null=null;

  selectAllCancel(){
    if(this.isAllCancel){
      this.selectedTransfer?.lines.forEach(x=>x.transferedCount=0);
    }else{
      this.selectedTransfer?.lines.forEach(x=>x.transferedCount = x.count);
      this.selectedTransfer?.lines.forEach(x=>x.remark = '');
    }
  }
  allCancelReasonChange(){
    if(this.isAllCancel){
      this.selectedTransfer?.lines.forEach(x=>x.remark = this.allCancelReason);
    }
  }
  selectTransfer(t:Transfer){
    this.selectedTransfer = t;
    this.selectedTransfer.lines.forEach(x=>{
      if(!t.closed){
        x.transferedCount = x.count;
      }

    })
  }

  constructor(private dialogRef:MatDialogRef<TransferListComponent>,@Inject(MAT_DIALOG_DATA)
  public data: {transfers:Array<Transfer>, dataService:DataService, type:'in'|'out', viewItem:Transfer}) {
    if(data.viewItem){
      this.selectedTransfer = data.viewItem;
      this.viewItem = data.viewItem;
    }
  }
  close(){
    this.dialogRef.close(false);
  }
  apply(){
    if(this.check() && this.selectedTransfer){
      this.isLoading=true;
      this.data.dataService.TransferData.applyTransfer(this.selectedTransfer).subscribe(x=>{
        if(x){
          this.dialogRef.close(true);
          this.isLoading=false;
        }
      })

    }
  }
  cancelAct(){

  }
  print(t:Transfer){
    window.open('/transfer/'+t.id+'/print');
  }

  check(){

    if(this.selectedTransfer){
      let result = true;
      this.selectedTransfer.lines.forEach(x=>{
        if(x.count != x.transferedCount && (x.remark==null ||x.remark.length==0) && result){
          alert('Если количество принятого не совпадает с отправленным необходимо указать причину возварата');
          result = false;

        }
      })
      return result;
    }
    return false;
  }
}

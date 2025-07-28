import {Component, Inject} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialogRef} from "@angular/material/dialog";
import {DailySource} from "../../../model/DailySource";
import {ProductCalendarRecord, ProductPlan} from "../../../model/Hr";

@Component({
  selector: 'app-calculate-calendar',
  templateUrl: './calculate-calendar.component.html',
  styleUrl: './calculate-calendar.component.css'
})
export class CalculateCalendarComponent {


  areaText='';
  isEdit = false;
  isFilled = false;
  constructor( private dialogRef:MatDialogRef<CalculateCalendarComponent>,@Inject(MAT_DIALOG_DATA)
  public data:{
    plans: ProductPlan[], records:ProductCalendarRecord[],
    sumData:{
      [post:string]:{
        minutes:number;
        data:{
          [crpPost:string]:{
            minutes:number,
            data:{
              [worker:string]:{
                minutes:number,
                data:{
                  [target:string]:{
                    minutes:number
                  }
                }

              }
            }
          }
        };
      }
    }})
  {
    this.data.plans.forEach(z=>{
      if(z.targetMinutes>0){
        this.isFilled=true;
      }
    });
  }
  normalize(s: string): string {
    return s.toLowerCase()
      .replace(/[.,\/#!$%\^&\*;:{}=\-_`~()]/g, "") // убираем пунктуацию
      .replace(/\s+/g, " ")                        // заменяем множественные пробелы
      .trim();
  }
  rationChange(planRecord:ProductPlan){
    setTimeout(() => {
      planRecord.ratioMinutes = planRecord.targetMinutes * planRecord.effectiveRatio * planRecord.directorRatio * planRecord.additionalRatio;
    });
  }
  input(event:string){


    let subString = event.split('\n');
    console.log(subString.length);
    let reorderedPlans: ProductPlan[]=[];

    subString.forEach(z=>{

      let parts = z.split('\t');
      if(parts.length<2){
        return;
      }
      let crpname = this.normalize(parts[0]);
      let crpPost = this.data.plans.find(p => {
        let normDesc = this.normalize(p.crpCenterDescription);
        return normDesc == crpname;
      });
      let s = parts[1].replace(',','.').trim().replace(' ','');

      if(crpPost){


        crpPost.targetMinutes = Number(s);

        reorderedPlans.push(crpPost);
      }else{

      }
    });

    this.data.plans.forEach(z=>{
      let nf = reorderedPlans.find(p=>p.crpCenter == z.crpCenter);
      if(!nf){
        reorderedPlans.push(z);
      }
    })

    this.data.plans = reorderedPlans;
    this.data.plans.forEach(z=>{
      z.ratioMinutes = z.targetMinutes * z.effectiveRatio * z.directorRatio * z.additionalRatio;
      if(z.targetMinutes>0){
        this.isFilled=true;
      }
    });

  }

}

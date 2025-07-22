import { BrowserModule } from '@angular/platform-browser';
import {LOCALE_ID, NgModule} from '@angular/core';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import {TransportService} from "./services/transport.service";
import {DataService} from "./services/data.service";
import {SessionService} from "./services/session.service";
import {PostViewComponent} from "./components/post-view/post-view.component";
import {AdminSettingsComponent} from "./components/admin-settings/admin-settings.component";
import {UserSettingsComponent} from "./components/admin-settings/user-settings/user-settings.component";

import {PostSettingsComponent} from "./components/admin-settings/post-settings/post-settings.component";

import {MatIconModule} from "@angular/material/icon";
import {OperatorComponent} from "./components/operator/operator.component";
import {WorkStarterComponent} from "./components/operator/work-starter/work-starter.component";
import {WorkPrepareComponent} from "./components/operator/work-prepare/work-prepare.component";
import {WorkCompactViewComponent} from "./components/post-view/work-compact-view/work-compact-view.component";
import { DragDropModule} from "@angular/cdk/drag-drop";
import {PostDialogComponent} from "./dialogs/post-dialog/post-dialog.component";

import {InfoViewComponent} from "./dialogs/info-view/info-view.component";

import {WorkEventService} from "./services/work-event.service";

import {MatToolbarModule} from "@angular/material/toolbar";

import {MatSidenavModule} from "@angular/material/sidenav";
import {DialogHandlerService} from "./services/dialog-handler.service";
import {NumberDialogComponent} from "./dialogs/number-dialog/number-dialog.component";
import {AnalyticComponent} from "./components/analytic/analytic.component";
import {IssueSettingsComponent} from "./components/admin-settings/issue-settings/issue-settings.component";
import {IssueCreateDialogComponent} from "./dialogs/issue-create-dialog/issue-create-dialog.component";
import {PostStatisticComponent} from "./components/analytic/post-statistic/post-statistic.component";
import {OrderStatisticComponent} from "./components/analytic/order-statistic/order-statistic.component";
import {NgApexchartsModule} from "ng-apexcharts";
import {MatButtonToggleModule} from "@angular/material/button-toggle";

import {RetroPostStatisticComponent} from "./components/analytic/retro-post-statistic/retro-post-statistic.component";
import {MatDatepickerModule} from "@angular/material/datepicker";
import {DateAdapter, MAT_DATE_LOCALE, MatNativeDateModule} from "@angular/material/core";
import {OrderTimeLineComponent} from "./components/analytic/order-time-line/order-time-line.component";
import {MatExpansionModule} from "@angular/material/expansion";
import {CardViewComponent} from "./components/TechCard/card-view/card-view.component";
import {CardfinderComponent} from "./components/TechCard/cardfinder/cardfinder.component";
import {ImageSetViewComponent} from "./components/TechCard/image-set-view/image-set-view.component";
import {LightboxModule} from "ng-gallery/lightbox";
import {GalleryModule} from "ng-gallery";
import {MatTabsModule} from "@angular/material/tabs";
import {MatProgressSpinnerModule} from "@angular/material/progress-spinner";
import {MatButtonModule} from "@angular/material/button";
import {MatInputModule} from "@angular/material/input";
import {MatSelectModule} from "@angular/material/select";
import {MatCheckboxModule} from "@angular/material/checkbox";
import {MatTooltipModule} from "@angular/material/tooltip";
import {MatListModule} from "@angular/material/list";
import {MatMenuModule} from "@angular/material/menu";
import {MatDialogActions, MatDialogContent, MatDialogTitle} from "@angular/material/dialog";
import {PriorityListComponent} from "./components/priority-list/priority-list.component";
import {
  TotalOrderStatisticComponent
} from "./components/analytic/total-order-statistic/total-order-statistic.component";
import {OperatorUtilityComponent} from "./components/operator/operator-utility/operator-utility.component";
import {ReportComponent} from "./components/admin-settings/report/report.component";
import {CustomDateAdapter} from "./CustomAdapter";
import {NgxEchartsDirective, NgxEchartsModule} from "ngx-echarts";
import {DatePipe, registerLocaleData} from "@angular/common";
import {MatChipsModule} from "@angular/material/chips";
import {TransferCreateComponent} from "./dialogs/transfer-create/transfer-create.component";
import {TransferListComponent} from "./dialogs/transfer-list/transfer-list.component";
import {MatBadgeModule} from "@angular/material/badge";
import {DailySourceDialogComponent} from "./dialogs/daily-source-dialog/daily-source-dialog.component";
import {StyleManagerService} from "./services/style-manager.service";
import {ThemeService} from "./services/ThemeService";
import {
  AdditionalSourceSettingsComponent
} from "./components/admin-settings/additional-source-settings/additional-source-settings.component";
import {AdditionalCostDialogComponent} from "./dialogs/additional-cost-dialog/additional-cost-dialog.component";
import {CalendarComponent} from "./components/hr/calendar/calendar.component";


import localeRu from '@angular/common/locales/ru';
import localeRuExtra from '@angular/common/locales/extra/ru';
import {WorkCostInfoComponent} from "./components/Info/work-cost-info/work-cost-info.component";
import {MatAutocompleteModule} from "@angular/material/autocomplete";
import {ActHistoryComponent} from "./components/Info/act-history/act-history.component";
import {InfoComponent} from "./components/Info/info.component";
import {OperatorStarterComponent} from "./components/operator/operator-starter/operator-starter.component";
import {HistoryViewComponent} from "./components/Info/history-view/history-view.component";
import {PhotoUploadComponent} from "./dialogs/photo-upload/photo-upload.component";
import {CountChangerComponent} from "./components/operator/count-changer/count-changer.component";
import {PictureViewComponent} from "./dialogs/picture-view/picture-view.component";
import {OtkOperationsComponent} from "./components/admin-settings/otk-operations/otk-operations.component";
import {OtkCheckComponent} from "./dialogs/otk-check/otk-check.component";
import {OtkViewComponent} from "./components/Info/otk-view/otk-view.component";
import {HrComponent} from "./components/hr/hr.component";
import {ProductWorkersComponent} from "./components/hr/product-workers/product-workers.component";
import {HrSettingsComponent} from "./components/hr/hr-settings/hr-settings.component";
import {HrActionDialogComponent} from "./dialogs/hr-action-dialog/hr-action-dialog.component";


registerLocaleData(localeRu, 'ru-RU', localeRuExtra);



@NgModule({
    declarations: [
        AppComponent, OtkCheckComponent, OtkViewComponent,
        PostViewComponent, OtkOperationsComponent,
        AdminSettingsComponent, PictureViewComponent,
        UserSettingsComponent, CountChangerComponent,
        PostSettingsComponent, WorkPrepareComponent, CardfinderComponent,
        WorkStarterComponent, HistoryViewComponent,
        OperatorComponent, OperatorStarterComponent,
        WorkCompactViewComponent, InfoComponent,
        PostDialogComponent, ActHistoryComponent,
        InfoViewComponent, WorkCostInfoComponent,
        NumberDialogComponent, CalendarComponent,
        AnalyticComponent,  PhotoUploadComponent, HrComponent, ProductWorkersComponent, HrSettingsComponent,
        IssueSettingsComponent, AdditionalCostDialogComponent,
        IssueCreateDialogComponent, AdditionalSourceSettingsComponent,
        PostStatisticComponent, DailySourceDialogComponent,
        OrderStatisticComponent, TransferListComponent,
        RetroPostStatisticComponent, TransferCreateComponent,
        OrderTimeLineComponent, ReportComponent,
        CardViewComponent, PriorityListComponent,
        CardfinderComponent, OperatorUtilityComponent,
        ImageSetViewComponent, TotalOrderStatisticComponent, HrActionDialogComponent
    ],
    imports: [
        BrowserModule.withServerTransition({appId: 'ng-cli-universal'}),
        HttpClientModule,
        FormsModule,
        RouterModule.forRoot([
            {path: '*', redirectTo: '', pathMatch: 'full'},
            {path: 'post', component: PostViewComponent},
            {path: 'admin', component: AdminSettingsComponent},
            {path: 'operate', component: OperatorComponent},
            {path: 'statistic', component: AnalyticComponent},
            {path: 'calendar', component: HrComponent},
            {path: 'card', component: CardViewComponent},
            {path: 'card-search', component: CardfinderComponent},
            {path: 'work-priority', component: PriorityListComponent},
            {path: 'info', component: InfoComponent}
        ]),
        NgxEchartsModule.forRoot({
            echarts: () => import('echarts')
        }),
        BrowserAnimationsModule,
        MatIconModule,
        DragDropModule,
        MatToolbarModule,
        MatSidenavModule,
        NgApexchartsModule,
        MatButtonToggleModule,
        MatDatepickerModule,
        MatNativeDateModule,
        MatExpansionModule,
        GalleryModule,
        LightboxModule,
        MatTabsModule,
        MatProgressSpinnerModule,
        MatButtonModule,
        MatInputModule,
        MatSelectModule,
        MatCheckboxModule,
        MatTooltipModule,
        MatListModule,
        MatMenuModule,
        MatDialogTitle,
        MatDialogContent,
        MatDialogActions,
        NgxEchartsDirective,
        MatChipsModule,
        MatBadgeModule,
        MatAutocompleteModule,
        ReactiveFormsModule,



    ],
    providers: [TransportService, DataService, SessionService, WorkEventService, StyleManagerService,ThemeService, DialogHandlerService,
      { provide: MAT_DATE_LOCALE, useValue: 'ru-RU' }, { provide: LOCALE_ID, useValue: "ru-RU" },
      { provide: DateAdapter, useClass: CustomDateAdapter },DatePipe],
    bootstrap: [AppComponent]
})
export class AppModule { }

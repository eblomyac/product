import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
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
import {MatLegacyCheckboxModule as MatCheckboxModule} from "@angular/material/legacy-checkbox";
import {MatLegacySelectModule as MatSelectModule} from "@angular/material/legacy-select";
import {MatLegacyTabsModule as MatTabsModule} from "@angular/material/legacy-tabs";
import {MatLegacyButtonModule as MatButtonModule} from "@angular/material/legacy-button";
import {PostSettingsComponent} from "./components/admin-settings/post-settings/post-settings.component";
import {MatLegacyInputModule as MatInputModule} from "@angular/material/legacy-input";
import {MatIconModule} from "@angular/material/icon";
import {OperatorComponent} from "./components/operator/operator.component";
import {WorkStarterComponent} from "./components/operator/work-starter/work-starter.component";
import {WorkPrepareComponent} from "./components/operator/work-prepare/work-prepare.component";
import {WorkCompactViewComponent} from "./components/post-view/work-compact-view/work-compact-view.component";
import { DragDropModule} from "@angular/cdk/drag-drop";
import {PostDialogComponent} from "./dialogs/post-dialog/post-dialog.component";
import {MatLegacyDialogModule as MatDialogModule} from "@angular/material/legacy-dialog";
import {InfoViewComponent} from "./dialogs/info-view/info-view.component";
import {MatLegacyMenuModule as MatMenuModule} from "@angular/material/legacy-menu";
import {WorkEventService} from "./services/work-event.service";
import {MatLegacyProgressSpinnerModule as MatProgressSpinnerModule} from "@angular/material/legacy-progress-spinner";
import {MatToolbarModule} from "@angular/material/toolbar";
import {MatLegacyListModule as MatListModule} from "@angular/material/legacy-list";
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
import {MatLegacyTooltipModule as MatTooltipModule} from "@angular/material/legacy-tooltip";
import {RetroPostStatisticComponent} from "./components/analytic/retro-post-statistic/retro-post-statistic.component";
import {
  NgxMatDatetimePickerModule,
  NgxMatNativeDateModule,
  NgxMatTimepickerModule
} from "@angular-material-components/datetime-picker";
import {MatDatepickerModule} from "@angular/material/datepicker";
import {MatNativeDateModule} from "@angular/material/core";
import {OrderTimeLineComponent} from "./components/analytic/order-time-line/order-time-line.component";
import {MatExpansionModule} from "@angular/material/expansion";
import {CardViewComponent} from "./components/TechCard/card-view/card-view.component";
import {CardfinderComponent} from "./components/TechCard/cardfinder/cardfinder.component";
import {ImageSetViewComponent} from "./components/TechCard/image-set-view/image-set-view.component";
import {LightboxModule} from "ng-gallery/lightbox";
import {GalleryModule} from "ng-gallery";



@NgModule({
    declarations: [
        AppComponent,
        PostViewComponent,
        AdminSettingsComponent,
        UserSettingsComponent,
        PostSettingsComponent,
        WorkPrepareComponent,
        WorkStarterComponent,
        OperatorComponent,
        WorkCompactViewComponent,
        PostDialogComponent,
        InfoViewComponent,
        NumberDialogComponent,
        AnalyticComponent,
        IssueSettingsComponent,
        IssueCreateDialogComponent,
        PostStatisticComponent,
        OrderStatisticComponent,
        RetroPostStatisticComponent,
        OrderTimeLineComponent,
        CardViewComponent,
        CardfinderComponent,
        ImageSetViewComponent
    ],
    imports: [
        BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
        HttpClientModule,
        FormsModule,
        RouterModule.forRoot([
            { path: '*', redirectTo: '', pathMatch: 'full' },
            { path: 'post', component: PostViewComponent },
            { path: 'admin', component: AdminSettingsComponent },
            { path: 'operate', component: OperatorComponent },
            { path: 'statistic', component: AnalyticComponent },
            { path: 'card', component: CardViewComponent },
            { path: 'card-search', component: CardfinderComponent }
        ]),
        BrowserAnimationsModule,
        MatCheckboxModule,
        MatSelectModule,
        MatTabsModule,
        MatButtonModule,
        MatInputModule,
        MatIconModule,
        DragDropModule,
        MatDialogModule,
        MatMenuModule,
        MatProgressSpinnerModule,
        MatToolbarModule,
        MatListModule,
        MatSidenavModule,
        NgApexchartsModule,
        MatButtonToggleModule,
        MatTooltipModule,
        NgxMatDatetimePickerModule,
        MatDatepickerModule,
        MatNativeDateModule,
        NgxMatNativeDateModule,
        MatExpansionModule,
        GalleryModule,
        LightboxModule
    ],
    providers: [TransportService, DataService, SessionService, WorkEventService, DialogHandlerService],
    bootstrap: [AppComponent]
})
export class AppModule { }

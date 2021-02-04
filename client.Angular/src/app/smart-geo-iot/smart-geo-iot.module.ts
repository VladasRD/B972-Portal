import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { SharedModule } from '../common/shared.module';
import { SecurityModule } from '../security/security.module';
import { SettingsComponent } from './settings/settings.component';
import { ReportListComponent } from './report-list/report-list.component';
import { GraphicListComponent } from './graphic-list/graphic-list.component';
import { ClientListComponent } from './client-list/client-list.component';
import { ClientDetailComponent } from './client-detail/client-detail.component';
import { DeviceSelectComponent } from './device-select/device-select.component';
import { DeviceAutoCompleteComponent } from './device-auto-complete/device-auto-complete.component';
import { HelpComponent } from './help/help.component';
import { DashboardDetailComponent } from './dashboard-detail/dashboard-detail.component';
import { GraphicDetailComponent } from './graphic-detail/graphic-detail.component';
import { PackageListComponent } from './package-list/package-list.component';
import { PackageDetailComponent } from './package-detail/package-detail.component';
import { ProjectListComponent } from './project-list/project-list.component';
import { ProjectDetailComponent } from './project-detail/project-detail.component';
import { DeviceConfigListComponent } from './device-config-list/device-config-list.component';
import { DeviceConfigDetailComponent } from './device-config-detail/device-config-detail.component';
import { PackageSelectComponent } from './package-select/package-select.component';
import { ProjectSelectComponent } from './project-select/project-select.component';
import { DashboardAguamonDetailComponent } from './dashboard-aguamon-detail/dashboard-aguamon-detail.component';
import { DashboardDjrfDetailComponent } from './dashboard-djrf-detail/dashboard-djrf-detail.component';
import { ReportDjrfListComponent } from './report-djrf-list/report-djrf-list.component';
import { ReportComponent } from './report/report.component';
import { SubClientListComponent } from './sub-client-list/sub-client-list.component';
import { SubClientDetailComponent } from './sub-client-detail/sub-client-detail.component';
import { RoleClientSelectComponent } from './role-client-select/role-client-select.component';
import { FirmwareListComponent } from './firmware-list/firmware-list.component';
import { FirmwareDetailComponent } from './firmware-detail/firmware-detail.component';
import { ClientAutoCompleteComponent } from './client-auto-complete/client-auto-complete.component';
import { RedirectionComponent } from './redirection/redirection/redirection.component';
import { MapsViewComponent } from './maps-view/maps-view.component';
import { GraphicAguamonDetailComponent } from './graphic-aguamon-detail/graphic-aguamon-detail.component';
import { GraphicDjrfDetailComponent } from './graphic-djrf-detail/graphic-djrf-detail.component';
import { CommercialReportListComponent } from './commercial-report-list/commercial-report-list.component';
import { CommercialReportDetailComponent } from './commercial-report-detail/commercial-report-detail.component';
import { CommercialReportShowComponent } from './commercial-report-show/commercial-report-show.component';
import { DashboardAguamon81DetailComponent } from './dashboard-aguamon81-detail/dashboard-aguamon81-detail.component';
import { ReportAguamonListComponent } from './report-aguamon-list/report-aguamon-list.component';
import { GraphicPqaDetailComponent } from './graphic-pqa-detail/graphic-pqa-detail.component';
import { DashboardAguamon83DetailComponent } from './dashboard-aguamon83-detail/dashboard-aguamon83-detail.component';

@NgModule({
  imports: [
    RouterModule.forChild([
      { path: 'sgi/dashboard/:id', component: DashboardDetailComponent },
      { path: 'sgi/dashboard/aguamon/:id', component: DashboardAguamonDetailComponent },
      { path: 'sgi/dashboard/djrf/:id', component: DashboardDjrfDetailComponent },
      { path: 'sgi/relatorio', component: ReportComponent, data: { shouldReuse: false } },
      { path: 'sgi/relatorio/hidro', component: ReportListComponent, data: { shouldReuse: false } },
      { path: 'sgi/relatorio/djrf', component: ReportDjrfListComponent, data: { shouldReuse: false } },
      { path: 'sgi/relatorio/aguamon', component: ReportAguamonListComponent, data: { shouldReuse: false } },
      { path: 'sgi/grafico', component: GraphicListComponent, data: { shouldReuse: true } },
      { path: 'sgi/grafico/:id', component: GraphicDetailComponent },
      { path: 'sgi/grafico/aguamon/:id', component: GraphicAguamonDetailComponent },
      { path: 'sgi/grafico/djrf/:id', component: GraphicDjrfDetailComponent },
      { path: 'sgi/grafico/pqa/:id', component: GraphicPqaDetailComponent },
      { path: 'sgi/configuracoes', component: SettingsComponent, data: { shouldReuse: true } },
      { path: 'sgi/clientes', component: ClientListComponent, data: { shouldReuse: true } },
      { path: 'sgi/clientes/:id', component: ClientDetailComponent },
      { path: 'sgi/ajuda', component: HelpComponent, data: { shouldReuse: true } },
      { path: 'sgi/pacotes', component: PackageListComponent, data: { shouldReuse: true } },
      { path: 'sgi/pacotes/:id', component: PackageDetailComponent },
      { path: 'sgi/projetos', component: ProjectListComponent, data: { shouldReuse: true } },
      { path: 'sgi/projetos/:id', component: ProjectDetailComponent },
      { path: 'sgi/dispositivos', component: DeviceConfigListComponent, data: { shouldReuse: true } },
      { path: 'sgi/dispositivos/:id', component: DeviceConfigDetailComponent },
      { path: 'sgi/sub-clientes', component: SubClientListComponent, data: { shouldReuse: true } },
      { path: 'sgi/sub-clientes/:id', component: SubClientDetailComponent },
      { path: 'sgi/dados-firmware', component: FirmwareListComponent, data: { shouldReuse: false } },
      { path: 'sgi/dados-firmware/:id', component: FirmwareDetailComponent },
      { path: 'redirect', component: RedirectionComponent },
      { path: 'sgi/maps/:id', component: MapsViewComponent },
      { path: 'sgi/relatorio-comercial', component: CommercialReportListComponent, data: { shouldReuse: true } },
      { path: 'sgi/relatorio-comercial/:id', component: CommercialReportDetailComponent },
      { path: 'sgi/relatorio-comercial-show/:id', component: CommercialReportShowComponent },
      { path: 'sgi/dashboard/aguamon81/:id', component: DashboardAguamon81DetailComponent },
      { path: 'sgi/dashboard/aguamon83/:id', component: DashboardAguamon83DetailComponent }
    ]),
    SharedModule,
    SecurityModule
  ],
  exports: [ RouterModule ],
  schemas: [ CUSTOM_ELEMENTS_SCHEMA ],
  declarations: [
    SettingsComponent,
    ReportListComponent,
    GraphicListComponent,
    ClientListComponent,
    ClientDetailComponent,
    DeviceSelectComponent,
    DeviceAutoCompleteComponent,
    HelpComponent,
    DashboardDetailComponent,
    GraphicDetailComponent,
    PackageListComponent,
    PackageDetailComponent,
    ProjectListComponent,
    ProjectDetailComponent,
    DeviceConfigListComponent,
    DeviceConfigDetailComponent,
    PackageSelectComponent,
    ProjectSelectComponent,
    DashboardAguamonDetailComponent,
    DashboardDjrfDetailComponent,
    ReportDjrfListComponent,
    ReportComponent,
    SubClientListComponent,
    SubClientDetailComponent,
    RoleClientSelectComponent,
    FirmwareListComponent,
    FirmwareDetailComponent,
    ClientAutoCompleteComponent,
    RedirectionComponent,
    MapsViewComponent,
    GraphicAguamonDetailComponent,
    GraphicDjrfDetailComponent,
    CommercialReportListComponent,
    CommercialReportDetailComponent,
    CommercialReportShowComponent,
    DashboardAguamon81DetailComponent,
    ReportAguamonListComponent,
    GraphicPqaDetailComponent,
    DashboardAguamon83DetailComponent
  ],
    // entryComponents: [
    //   BottomsheetComponent,
    //   BottomsheetComponent
    // ]
})
export class SmartGeoIotModule {}

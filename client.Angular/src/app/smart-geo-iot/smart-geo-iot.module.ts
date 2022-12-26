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
import { DashboardAguamon21DetailComponent } from './dashboard-aguamon21-detail/dashboard-aguamon21-detail.component';
import { ReportAguamon2ListComponent } from './report-aguamon2-list/report-aguamon2-list.component';
import { GraphicTrmDetailComponent } from './graphic-trm-detail/graphic-trm-detail.component';
import { DashboardTrm23DetailComponent } from './dashboard-trm23-detail/dashboard-trm23-detail.component';
import { ReportTrmListComponent } from './report-trm-list/report-trm-list.component';
import { GraphicTrm23DetailComponent } from './graphic-trm23-detail/graphic-trm23-detail.component';
import { DataUpdateComponent } from './data-update/data-update.component';
import { ReportTspListComponent } from './report-tsp-list/report-tsp-list.component';
import { GraphicTspDetailComponent } from './graphic-tsp-detail/graphic-tsp-detail.component';
import { DashboardClamponDetailComponent } from './dashboard-clampon-detail/dashboard-clampon-detail.component';
import { ReportTqaListComponent } from './report/report-tqa-list/report-tqa-list.component';
import { SerialModelComponent } from './serial-model/serial-model.component';
import { DashboardNavigationComponent } from './dashboard-navigation/dashboard-navigation.component';
import { DashboardB978DetailComponent } from './dashboard-b978-detail/dashboard-b978-detail.component';
import { ReportB978ListComponent } from './report/report-b978-list/report-b978-list.component';
import { DashboardB987DetailComponent } from './dashboard-b987-detail/dashboard-b987-detail.component';
import { ReportMcondListComponent } from './report-mcond-list/report-mcond-list.component';
import { GraphicB987DetailComponent } from './graphic-b987-detail/graphic-b987-detail.component';

@NgModule({
  imports: [
    RouterModule.forChild([
      { path: 'radiodados/dashboard/:id', component: DashboardDetailComponent },
      { path: 'radiodados/dashboard/aguamon/:id', component: DashboardAguamonDetailComponent },
      { path: 'radiodados/dashboard/djrf/:id', component: DashboardDjrfDetailComponent },
      { path: 'radiodados/relatorio', component: ReportComponent, data: { shouldReuse: false } },
      { path: 'radiodados/relatorio/hidro', component: ReportListComponent, data: { shouldReuse: false } },
      { path: 'radiodados/relatorio/djrf', component: ReportDjrfListComponent, data: { shouldReuse: false } },
      { path: 'radiodados/relatorio/aguamon', component: ReportAguamonListComponent, data: { shouldReuse: false } },
      { path: 'radiodados/relatorio/aguamon2', component: ReportAguamon2ListComponent, data: { shouldReuse: false } },
      { path: 'radiodados/relatorio/trm', component: ReportTrmListComponent, data: { shouldReuse: false } },
      { path: 'radiodados/grafico', component: GraphicListComponent, data: { shouldReuse: true } },
      { path: 'radiodados/grafico/:id', component: GraphicDetailComponent },
      { path: 'radiodados/grafico/aguamon/:id', component: GraphicAguamonDetailComponent },
      { path: 'radiodados/grafico/djrf/:id', component: GraphicDjrfDetailComponent },
      { path: 'radiodados/grafico/pqa/:id', component: GraphicPqaDetailComponent },
      { path: 'radiodados/grafico/trm/:id', component: GraphicTrmDetailComponent },
      { path: 'radiodados/grafico/trm23/:id', component: GraphicTrm23DetailComponent },
      { path: 'radiodados/grafico/tsp/:id', component: GraphicTspDetailComponent },
      { path: 'radiodados/configuracoes', component: SettingsComponent, data: { shouldReuse: true } },
      { path: 'radiodados/clientes', component: ClientListComponent, data: { shouldReuse: true } },
      { path: 'radiodados/clientes/:id', component: ClientDetailComponent },
      { path: 'radiodados/ajuda', component: HelpComponent, data: { shouldReuse: true } },
      { path: 'radiodados/pacotes', component: PackageListComponent, data: { shouldReuse: true } },
      { path: 'radiodados/pacotes/:id', component: PackageDetailComponent },
      { path: 'radiodados/projetos', component: ProjectListComponent, data: { shouldReuse: true } },
      { path: 'radiodados/projetos/:id', component: ProjectDetailComponent },
      { path: 'radiodados/dispositivos', component: DeviceConfigListComponent, data: { shouldReuse: true } },
      { path: 'radiodados/dispositivos/:id', component: DeviceConfigDetailComponent },
      { path: 'radiodados/sub-clientes', component: SubClientListComponent, data: { shouldReuse: true } },
      { path: 'radiodados/sub-clientes/:id', component: SubClientDetailComponent },
      { path: 'radiodados/dados-firmware', component: FirmwareListComponent, data: { shouldReuse: false } },
      { path: 'radiodados/dados-firmware/:id', component: FirmwareDetailComponent },
      { path: 'radiodados/dados-atualizacao', component: DataUpdateComponent, data: { shouldReuse: false } },
      { path: 'redirect', component: RedirectionComponent },
      { path: 'radiodados/maps/:id', component: MapsViewComponent },
      { path: 'radiodados/relatorio-comercial', component: CommercialReportListComponent, data: { shouldReuse: true } },
      { path: 'radiodados/relatorio-comercial/:id', component: CommercialReportDetailComponent },
      { path: 'radiodados/relatorio-comercial-show/:id', component: CommercialReportShowComponent },
      { path: 'radiodados/dashboard/aguamon81/:id', component: DashboardAguamon81DetailComponent },
      { path: 'radiodados/dashboard/aguamon83/:id', component: DashboardAguamon83DetailComponent },
      { path: 'radiodados/dashboard/aguamon21/:id', component: DashboardAguamon21DetailComponent },
      { path: 'radiodados/dashboard/trm23/:id', component: DashboardTrm23DetailComponent },
      { path: 'radiodados/relatorio/tsp', component: ReportTspListComponent },
      { path: 'radiodados/dashboard/clampon/:id', component: DashboardClamponDetailComponent },
      { path: 'radiodados/dashboard/b978/:id', component: DashboardB978DetailComponent },
      { path: 'radiodados/relatorio/tqa', component: ReportTqaListComponent },
      { path: 'radiodados/relatorio/b978', component: ReportB978ListComponent },

      // PROJETO B987 - MCOND
      { path: 'radiodados/dashboard/b987/:id', component: DashboardB987DetailComponent },
      { path: 'radiodados/relatorio/b987', component: ReportMcondListComponent },
      { path: 'radiodados/grafico/b987/:id', component: GraphicB987DetailComponent }
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
    DashboardAguamon83DetailComponent,
    DashboardAguamon21DetailComponent,
    ReportAguamon2ListComponent,
    GraphicTrmDetailComponent,
    DashboardTrm23DetailComponent,
    ReportTrmListComponent,
    GraphicTrm23DetailComponent,
    DataUpdateComponent,
    ReportTspListComponent,
    GraphicTspDetailComponent,
    DashboardClamponDetailComponent,
    ReportTqaListComponent,
    SerialModelComponent,
    DashboardNavigationComponent,
    DashboardB978DetailComponent,
    ReportB978ListComponent,
    DashboardB987DetailComponent,
    ReportMcondListComponent,
    GraphicB987DetailComponent
  ],
    // entryComponents: [
    //   BottomsheetComponent,
    //   BottomsheetComponent
    // ]
})
export class SmartGeoIotModule {}

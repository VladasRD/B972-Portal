import { ChangeDeviceDialogComponent } from './smart-geo-iot/change-device-dialog/change-device-dialog.component';
import { SmartGeoIotModule } from './smart-geo-iot/smart-geo-iot.module';
import { SmartGeoIotMenus } from './smart-geo-iot/smart-geo-iot.menus';

import { HttpClientModule } from '@angular/common/http';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule} from '@angular/platform-browser/animations';
import { RouterModule, RouteReuseStrategy } from '@angular/router';

import { LOCALE_ID, TRANSLATIONS, TRANSLATIONS_FORMAT, NgModule, CompilerFactory, COMPILER_OPTIONS, Compiler } from '@angular/core';

import { OAuthModule, OAuthModuleConfig } from 'angular-oauth2-oidc';

import { environment } from '../environments/environment';

import { AppComponent } from './app.component';
import { GenericYesNoDialogComponent } from './common/generic-yes-no-dialog/generic-yes-no-dialog.component';
import { ChangeNameDialogComponent } from './common/change-name-dialog/change-name-dialog.component';

import { SharedModule } from './common/shared.module';

// load locale data form date, currency, etc..
import { registerLocaleData } from '@angular/common';
import localePt from '@angular/common/locales/pt';

// For dynamic localization: see https://github.com/ngx-translate/i18n-polyfill
import { I18n } from '@ngx-translate/i18n-polyfill';

import { LoginComponent } from './login/login.component';
import { UnauthorizedComponent } from './unauthorized/unauthorized.component';
import { OutComponent } from './out/out.component';
import { HomeComponent } from './home/home.component';

import { CustomRouteReuseStrategy } from './common/custom-route-reuse-strategy';

import { CookieService } from 'ngx-cookie-service';
import { ServiceWorkerModule } from '@angular/service-worker';

import { SecurityMenus } from './security/security.menus';
import { CMSMenus } from './cms/cms.menus';
import { PluginsService } from './common/plugins.service';
import { AuthService } from './common/auth.service';
import { MenuService } from './common/menu.service';

// For plugins AOT
import { JitCompilerFactory } from '@angular/platform-browser-dynamic';
import { SecurityModule } from './security/security.module';

export function createCompiler(compilerFactory: CompilerFactory) {
  return compilerFactory.createCompiler();
}

registerLocaleData(localePt);

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    UnauthorizedComponent,
    OutComponent,
    LoginComponent,
    GenericYesNoDialogComponent,
    ChangeDeviceDialogComponent,
    ChangeNameDialogComponent
  ],
  imports: [
    SharedModule,
    BrowserModule,
    BrowserAnimationsModule,
    HttpClientModule,
    SecurityModule,
    // MsdSeletoModule,
    SmartGeoIotModule,
    OAuthModule.forRoot(),
    RouterModule.forRoot([
      { path: '', component: HomeComponent, data: { shouldReuse: true }  },
      { path: 'index.html', component: HomeComponent, data: { shouldReuse: true } },
      { path: 'unauthorized', component: UnauthorizedComponent, data: { shouldReuse: true } },
      { path: 'bye', component: OutComponent, data: { shouldReuse: true } },

      // LAZY LOAD MODULES
      // { path: 'security', loadChildren: 'app/security/security.module#SecurityModule'},
      // { path: 'cms', loadChildren: 'app/cms/cms.module#CmsModule'},
      // { path: 'seleto', loadChildren: 'app/msd-seleto/msd-seleto.module#MsdSeletoModule'},

    ]),
    ServiceWorkerModule.register('/ngsw-worker.js', { enabled: environment.production })
    /*
    OAuthModule.forRoot({
      resourceServer: {
          allowedUrls: [ environment.IDENTITY_SERVER_URL + '/api'],
          sendAccessToken: true
      }})*/
  ],
  exports: [],
  providers: [
    PluginsService,
    CookieService,
    {
      provide: OAuthModuleConfig,
      useFactory: () => {
        return { resourceServer: { allowedUrls: [ environment.IDENTITY_SERVER_URL + '/api' ], sendAccessToken: true } };
      },
      // deps: [environment]
    },
    { provide: LOCALE_ID, useValue: 'en' },
    {
      provide: TRANSLATIONS,
      useFactory: (locale) => {
          locale = locale || 'en'; // default to english if no locale provided
          return require(`raw-loader!../locale/messages.${locale}.xlf`);
      },
      deps: [LOCALE_ID]
    },
    {provide: TRANSLATIONS_FORMAT, useValue: 'xlf'},
    // {provide: MissingTranslationStrategy, useValue: MissingTranslationStrategy.Ignore},
    I18n,
    {provide: RouteReuseStrategy, useClass: CustomRouteReuseStrategy },

    // for plugins AOT
    {provide: COMPILER_OPTIONS, useValue: {}, multi: true},
    {provide: CompilerFactory, useClass: JitCompilerFactory, deps: [COMPILER_OPTIONS]},
    {provide: Compiler, useFactory: createCompiler, deps: [CompilerFactory]}
  ],
  entryComponents: [
    GenericYesNoDialogComponent,
    ChangeDeviceDialogComponent,
    ChangeNameDialogComponent
  ],
  bootstrap: [AppComponent]
})
export class AppModule {

  constructor(
    private securityMenus: SecurityMenus,
    private sgiMenus: SmartGeoIotMenus
    // private cmsMenus: CMSMenus
    ) {
  }
}

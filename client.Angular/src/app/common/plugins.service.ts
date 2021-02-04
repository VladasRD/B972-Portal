import { Observable } from 'rxjs/Observable';
import { HttpClient } from '@angular/common/http';
import { Injectable, Compiler, Inject, ReflectiveInjector, Injector, COMPILER_OPTIONS } from '@angular/core';

import 'rxjs/add/operator/map';

// Needed for the new modules
import * as AngularCore from '@angular/core';
import * as AngularCommon from '@angular/common';
import * as AngularRouter from '@angular/router';
import * as AngularForms from '@angular/forms';
import * as AngularMaterial from '@angular/material';
import * as BrowserAnimations from '@angular/platform-browser/animations';
import * as FlexLayout from '@angular/flex-layout';
import * as BoxMaterial from 'box-material';



//  HELP
// https://stackoverflow.com/questions/45503497/how-to-load-dynamic-external-components-into-angular-application/45506470
// https://github.com/lmeijdam/angular-umd-dynamic-example
// https://blog.angularindepth.com/here-is-what-you-need-to-know-about-dynamic-components-in-angular-ac1e96167f9e

// https://github.com/angular/angular/issues/20875

// VER COMO OBTER O SYSTEMJS.SRC.JS ou USAR SYSEMNG

declare var SystemJS: any;

@Injectable()
export class PluginsService {

    source = `http://${window.location.host}/`;

    constructor(private compiler: Compiler, private http: HttpClient) {
        console.log(compiler);
    }

    loadAllPlugins() {
      this.getPlugins().subscribe(
        ps => {
          ps.forEach(p => {
            this.loadPlugin(p.location, p.moduleName).then(
              (exports) => {
                const e = exports[0];
              },
              (err) => {
                console.log('Error loading plugin ' + p.moduleName + ' from ' + p.location + '.');
                console.log(err);
              });
          });
        }
      );
    }

    getPlugins(): Observable<IPlugin[]> {
        return this.http.get<IPlugin[]>('./assets/plugins/plugins.json');
    }

    loadPlugin(location: string, moduleName: string): Promise<any> {
        const url = this.source + location;
        SystemJS.set('@angular/core', SystemJS.newModule(AngularCore));
        SystemJS.set('@angular/common', SystemJS.newModule(AngularCommon));
        SystemJS.set('@angular/router', SystemJS.newModule(AngularRouter));
        SystemJS.set('@angular/forms', SystemJS.newModule(AngularForms));
        SystemJS.set('@angular/material', SystemJS.newModule(AngularMaterial));
        SystemJS.set('@angular/flex-layout', SystemJS.newModule(FlexLayout));
        SystemJS.set('@angular/platform-browser/animations', SystemJS.newModule(BrowserAnimations));
        SystemJS.set('box-material', SystemJS.newModule(BoxMaterial));

        // now, import the new module
        return SystemJS.import(`${url}`).then((module) => {
            return this.compiler.compileModuleAndAllComponentsAsync(module[`${moduleName}`]).then(compiled => {
                return compiled;
            });
        });
    }

}

export interface IPlugin {
  path: string;
  location: string;
  moduleName: string;
  rootComponent?: string;
  description: string;
  registered?: boolean;
}

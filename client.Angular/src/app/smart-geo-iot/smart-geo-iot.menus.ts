import { Injectable } from '@angular/core';
import { ModuleMenus } from '../common/moduleMenus';
import { MenuService } from '../common/menu.service';
import { AuthService } from '../common/auth.service';
import { AppLink } from '../common/appLink';
import { RoleClaim } from '../common/roleClaim';
import { SmartGeoIotService } from './smartgeoiot.service';

@Injectable({
  providedIn: 'root'
})
export class SmartGeoIotMenus extends ModuleMenus {

  constructor(authService: AuthService, menuService: MenuService, private sgiService: SmartGeoIotService) {
    super(authService, menuService);
    this.moduleName = 'RadioDados Analítica';
    this.moduleIcon = 'monetization_on';
    this.registerLinksAndClaims();
  }

  get moduleLinks(): AppLink[] {
    return [
      { place: 'SECURITY', route: 'sgi/configuracoes', name: 'Cadastros', requiredRoles: [
        'SGI-CLIENT.READ', 'SGI-CLIENT.WRITE'
      ] },
      { place: 'SECURITY', route: 'sgi/ajuda', name: 'Ajuda', requiredRoles: [] },
      { place: 'TOP', route: '/', name: 'Dashboard', requiredRoles: ['SGI-DASHBOARD.READ'] },
      { place: 'TOP', route: 'sgi/relatorio', name: 'Relatório', requiredRoles: ['SGI-REPORT.READ'] },
      { place: 'TOP', route: 'sgi/grafico', name: 'Gráfico', requiredRoles: ['SGI-GRAPHIC.READ'] },
      { place: 'TOP', route: 'sgi/sub-clientes', name: 'Clientes', requiredRoles: ['SGI-SUBCLIENT.READ'] }
    ];
  }

  get moduleClaims(): RoleClaim[] {
    return [
      {
        claimValue: 'SGI.MASTER', claimName: 'Master read and write',
        claimDescription: 'Administrador MASTER do sistema.'
      },
      {
        claimValue: 'SGI.MASTER-COMMERCIAL', claimName: 'Master commercial read and write',
        claimDescription: 'Administrador MASTER comercial do sistema.'
      },
      {
        claimValue: 'SGI-CLIENT.READ', claimName: 'Client reader',
        claimDescription: 'Pode visualizar clientes.'
      },
      {
        claimValue: 'SGI-CLIENT.WRITE', claimName: 'Client manager',
        claimDescription: 'Pode criar, alterar e remover clientes.'
      },
      {
        claimValue: 'SGI-DASHBOARD.READ', claimName: 'Dashboard reader',
        claimDescription: 'Pode visualizar o dashboard.'
      },
      {
        claimValue: 'SGI-REPORT.READ', claimName: 'Report reader',
        claimDescription: 'Pode visualizar os relatórios.'
      },
      {
        claimValue: 'SGI-GRAPHIC.READ', claimName: 'Graphic reader',
        claimDescription: 'Pode visualizar os gráficos.'
      },
      {
        claimValue: 'SGI-DEVICE.READ', claimName: 'Device reader',
        claimDescription: 'Pode visualizar os dispositivos.'
      },
      {
        claimValue: 'SGI-DEVICE.WRITE', claimName: 'Device write',
        claimDescription: 'Pode criar, alterar e remover dispositivos.'
      },
      {
        claimValue: 'SGI-PACKAGE.READ', claimName: 'Package reader',
        claimDescription: 'Pode visualizar pacotes.'
      },
      {
        claimValue: 'SGI-PACKAGE.WRITE', claimName: 'Package manager',
        claimDescription: 'Pode criar, alterar e remover pacotes.'
      },
      {
        claimValue: 'SGI-PROJECT.READ', claimName: 'Project reader',
        claimDescription: 'Pode visualizar os projetos.'
      },
      {
        claimValue: 'SGI-PROJECT.WRITE', claimName: 'Project manager',
        claimDescription: 'Pode criar, alterar e remover projetos.'
      },

      // Sub-Clientes
      {
        claimValue: 'SGI-SUBCLIENT.READ', claimName: 'Sub Client reader',
        claimDescription: 'Pode visualizar sub clientes.'
      },
      {
        claimValue: 'SGI-SUBCLIENT.WRITE', claimName: 'Sub Client manager',
        claimDescription: 'Pode criar, alterar e remover sub clientes.'
      }
      // Sub-Clientes

    ];
  }

}

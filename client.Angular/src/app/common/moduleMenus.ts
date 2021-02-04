import { RoleClaim } from './roleClaim';
import { AppLink } from './appLink';
import { MenuService } from './menu.service';
import { AuthService } from './auth.service';

export abstract class ModuleMenus {

    protected moduleName: string;
    protected moduleIcon: string;

    public abstract moduleClaims: RoleClaim[];
    public abstract moduleLinks: AppLink[];

    constructor(protected authService: AuthService, protected menuService: MenuService) {
    }

    protected registerLinksAndClaims() {
        this.authService.registerClaimModule(this);
        this.moduleLinks.forEach(link => { this.menuService.registerLink(link); });
    }
}

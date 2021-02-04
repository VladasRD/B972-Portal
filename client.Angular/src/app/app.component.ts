import { Component, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { MatSidenav } from '@angular/material';

import { environment } from '../environments/environment';
import { MenuService } from './common/menu.service';
import { AuthService } from './common/auth.service';
import { MessageService } from './common/message.service';


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})

export class AppComponent implements OnInit {


  constructor(
    public menuService: MenuService,
    public messageService: MessageService,
    private authService: AuthService,
    private router: Router) {

    this.authService.init();
    this.authService.loadDocumentAndSignIn();

    this.router.events.subscribe(() => {
      if (this.sideNavMode === 'over') {
        this.sidenav.close();
      }
      window.scrollTo(0, 0);
    });
  }

  @ViewChild(MatSidenav) sidenav: MatSidenav;

  get isSecurityMenuVisible(): boolean {
    return this.authService.signedUser && this.menuService.getUserLinksFor('SECURITY').length > 0;
  }

  get isUIBlocked() {
    return this.messageService.isUIBlocked;
  }

  get isLoadingData() {
    return this.messageService.isLoadingData;
  }

  get sideNavMode(): string {
    if (window.document.body.clientWidth <= 1366) {
      return 'over';
    }
    return 'side';
  }

  get appName() {
    return environment.appName;
  }

  ngOnInit() {}

  redirect(url) {
    this.router.navigateByUrl('/redirect', {skipLocationChange: true}).then(() => {
      this.router.navigate([url]);
    });
  }
}

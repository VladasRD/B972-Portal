import { Component, OnInit } from '@angular/core';
import { MenuService } from '../common/menu.service';
import { AuthService } from '../common/auth.service';

import { AppUser } from '../common/appUser';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  constructor(private authService: AuthService, public linksService: MenuService) {}

  ngOnInit() {
    this.authService.refreshSignedUser();
  }

  signIn() {
    this.authService.signIn();
  }

  signOut() {
    this.authService.signOut();
  }

  get signedUser(): AppUser {
    return this.authService.signedUser;
  }


  get isSignedIn() {
    return this.signedUser != null;
  }

  get isSigning() {
    return this.authService.isSigning;
  }

}

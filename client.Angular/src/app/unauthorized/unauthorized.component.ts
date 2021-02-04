import { Component, OnInit } from '@angular/core';
import { AuthService } from '../common/auth.service';

@Component({
  selector: 'app-unauthorized',
  templateUrl: './unauthorized.component.html',
  styleUrls: ['./unauthorized.component.css']
})
export class UnauthorizedComponent implements OnInit {

  constructor(private authService: AuthService) { }

  ngOnInit() {
  }

  signInAgain() {
    this.authService.signIn();
    // manually call silentRefresh is not working
    // the request got canceled by the server I dont now why
    // this.securityService.silentRefresh();
  }

}

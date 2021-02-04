import { Component, Input, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { AppRole } from '../../common/appUser';
import { SecurityService } from '../../security/security.service';

@Component({
  selector: 'app-role-client-select',
  templateUrl: './role-client-select.component.html',
  styleUrls: ['./role-client-select.component.css']
})
export class RoleClientSelectComponent implements OnInit {
  roles: AppRole[] = [];

  @Input() control: FormControl;
  @Input() nullable = false;
  @Input() appearance = 'outline';
  @Input() floatLabel = 'float';
  @Input() hasPlaceHolder = true;

  constructor(private securityService: SecurityService) {
  }

  ngOnInit() {
    this.fillSystemRolesList();
  }

  private fillSystemRolesList(): void {
    this.securityService.getSystemRoles('', 0, 0, null).subscribe(roles => {
      if (!roles) {
        return;
      }
      this.roles = roles;
    });
  }

}

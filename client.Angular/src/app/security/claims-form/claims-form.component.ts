import { Component, OnInit, Input } from '@angular/core';
import { FormGroup, FormControl, FormArray } from '@angular/forms';

import { I18n } from '@ngx-translate/i18n-polyfill';

import { AuthService } from '../../common/auth.service';

import { ModuleMenus } from '../../common/moduleMenus';

@Component({
  selector: 'app-claims-form',
  templateUrl: './claims-form.component.html',
  styleUrls: ['./claims-form.component.css']
})
export class ClaimsFormComponent implements OnInit {

  form: FormGroup;
  @Input() public currentClaims: any[] = [];

  constructor(
    private authService: AuthService,
    private i18n: I18n,
  ) {
    this.form = new FormGroup({
      'modules': new FormArray([])
    });
  }

  ngOnInit() {
    this.buildClaimsList();
  }

  get modules(): ModuleMenus[] {
    return this.authService.getClaimModules();
  }

  private buildClaimsList() {
    this.modules.forEach(s => {
      const claims = new FormArray([]);
      s.moduleClaims.forEach(c => {
        const checkbox = new FormControl(false);
        checkbox.valueChanges.subscribe(isChecked => { this.onClaimChecked(isChecked, c.claimValue); });
        claims.push(checkbox);
      });
      this.modulesFormArray.push(claims);
    });

  }

  private onClaimChecked(isChecked: any, claimValue: string) {
    if (isChecked) {
      this.currentClaims.push({ claimValue: claimValue, claimType: 'role' });
    } else {
      const idx = this.currentClaims.findIndex(c => c.claimValue === claimValue);
      if (idx >= 0) {
        this.currentClaims.splice(idx, 1);
      }
    }
  }

  userHasThisClaim(claimValue: string): boolean {
    const claims = this.currentClaims.filter(c => c.claimValue === claimValue);
    return claims.length > 0;
  }

  get modulesFormArray(): FormArray {
    return this.form.controls.modules as FormArray;
  }

}

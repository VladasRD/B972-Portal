import { Directive, ElementRef, Input, OnInit, Renderer2 } from '@angular/core';
import { AuthService } from '../common/auth.service';

// Directive decorator
@Directive({ selector: '[appRequiredRoles]' })

// Directive class
export class RequiredRolesDirective implements OnInit {

    @Input() appRequiredRoles: string;

    constructor(private el: ElementRef, private renderer: Renderer2, private authService: AuthService) {
        this.renderer.setStyle(this.el.nativeElement, 'display', 'none');
    }

    ngOnInit() {
        // Use renderer to render the emelemt with styles
        if (this.appRequiredRoles) {
            const roles = this.appRequiredRoles.replace(new RegExp(' ', 'g'), '').split(',');
            for (const role of roles) {
                if (this.authService.signedUserIsInRole(role)) {
                    this.renderer.setStyle(this.el.nativeElement, 'display', 'inline-block');
                    return;
                }
            }
            this.renderer.setStyle(this.el.nativeElement, 'display', 'none');
        }
    }






}

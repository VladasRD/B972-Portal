import {Directive, ElementRef, HostListener, Input} from '@angular/core';

@Directive({
    selector: '[appNumeric]'
})

export class NumericDirective {

    @Input() numericType: string; // number | decimal

    private regex = {
        number: new RegExp(/^\d+$/),
        decimal: new RegExp(/^[0-9.,]*$/)
    };

    private specialKeys = {
        number: [ 'Backspace', 'Tab', 'End', 'Home', 'ArrowLeft', 'ArrowRight', 'Delete' ],
        decimal: [ 'Backspace', 'Tab', 'End', 'Home', 'ArrowLeft', 'ArrowRight', 'Delete' ],
    };

    constructor(private el: ElementRef) {
    }

    @HostListener('keydown', [ '$event' ])
    onKeyDown(event: KeyboardEvent) {

        if (this.specialKeys[this.numericType].indexOf(event.key) !== -1) {
            return;
        }
        // Do not use event.keycode this is deprecated.
        // See: https://developer.mozilla.org/en-US/docs/Web/API/KeyboardEvent/keyCode
        const current: string = this.el.nativeElement.value;
        const next: string = current.concat(event.key);
        if (next && !String(next).match(this.regex[this.numericType])) {
            event.preventDefault();
        }
    }
}
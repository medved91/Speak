import {Directive, ElementRef, HostListener} from '@angular/core';

@Directive({
  selector: '[appSpecialCharacters]'
})
export class SpecialCharactersDirective {

  constructor(private el: ElementRef) {
  }

  @HostListener('keypress', ['$event']) onKeyPress(event: KeyboardEvent) {
    return new RegExp('^\\w*$').test(event.key);
  }

  @HostListener('paste', ['$event']) blockPaste(event: KeyboardEvent) {
    this.validateFields(event);
  }

  validateFields(event: KeyboardEvent) {
    setTimeout(() => {

      this.el.nativeElement.value = this.el.nativeElement.value.replace(/[^A-Za-z ]/g, '').replace(/\s/g, '');
      event.preventDefault();

    }, 100)
  }

}

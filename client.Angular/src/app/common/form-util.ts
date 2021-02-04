import { FormControl, FormGroup, FormArray, Validators } from '@angular/forms';
import { String } from 'typescript-string-operations';

export class FormUtil {

    /**
     * Updates a givem model with the values from a FormGroup.
     * @param form The form with values
     * @param model The model to be updated
     */
    static updateModel(form: FormGroup, model: any) {

      for (const key in form.controls) {
        if (form.controls.hasOwnProperty(key)) {
          const formValue = form.get(key);
          if  (formValue != null) {
            model[key] = formValue.value;
          }
        }
      }

      // const props = Object.keys(model);
      // for (const p of props) {
      //   const formValue = form.get(p);
      //   if  (formValue != null) {
      //     model[p] = formValue.value;
      //   }
      // }
    }

    static toIndependentDate(date: Date) {
      if (date == null) {
        return '';
      }
      return [date.getFullYear(), date.getMonth() + 1, date.getDate()].join('-');
    }

    static i18nFormat(strFormat: string, ...args: any[]) {
      strFormat = strFormat.replace(new RegExp('\{\{', 'g'), '{');
      strFormat = strFormat.replace(new RegExp('\}\}', 'g'), '}');
      return String.Format(strFormat, args);
    }

    static isMobile() {
      return /iPhone|iPod|Android|Windows Phone/i.test(navigator.userAgent);
    }

}

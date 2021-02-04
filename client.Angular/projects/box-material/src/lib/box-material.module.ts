// angular base
import { NgModule, ModuleWithProviders } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { MatButtonModule, MatIconModule, MatInputModule, MatFormFieldModule, MatListModule,
  MatCheckboxModule, MatSelectModule,  MatDatepickerModule, MatButtonToggleModule,
  MatNativeDateModule } from '@angular/material';

// flex layout
import { FlexLayoutModule } from '@angular/flex-layout';

import { FileUploadComponent } from './file-upload/file-upload.component';
import { QuillEditorComponent } from './quill-editor/quill-editor.component';
import { DatetimePickerComponent } from './datetime-picker/datetime-picker.component';

@NgModule({
  imports: [ CommonModule, FormsModule, ReactiveFormsModule,
    MatButtonModule, MatIconModule, MatInputModule, MatFormFieldModule, MatListModule, MatCheckboxModule,
    MatSelectModule, MatDatepickerModule, MatNativeDateModule, MatButtonToggleModule,
    FlexLayoutModule
  ],
  declarations: [ FileUploadComponent, QuillEditorComponent, DatetimePickerComponent],
  exports: [ FileUploadComponent, QuillEditorComponent, DatetimePickerComponent ]
})
export class BoxMaterialModule {

  static forRoot(environment: any): ModuleWithProviders {
    return {
        ngModule: BoxMaterialModule,
        providers: [ { provide: 'env', useValue: environment} ]
    };
  }
}

import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import {
  MatFormFieldModule, MatInputModule, MatButtonModule, MatCardModule, MatIconModule, MatListModule,
  MatCheckboxModule, MatSelectModule, MatDialogModule, MatTableModule, MatDatepickerModule, MatTabsModule } from '@angular/material';
import { FlexLayoutModule } from '@angular/flex-layout';

import { PageTemplateComponent } from './page-template.component';
import { DocTemplateComponent } from './doc-template.component';

import { BoxMaterialModule } from 'box-material';

@NgModule({
  imports: [
    CommonModule,
    FormsModule, ReactiveFormsModule,
    MatFormFieldModule, MatInputModule, MatButtonModule, MatCardModule, MatIconModule, MatListModule,
    MatCheckboxModule, MatSelectModule, MatDialogModule, MatTableModule, MatDatepickerModule, MatTabsModule,
    FlexLayoutModule, BoxMaterialModule
  ],
  declarations:
  [
    PageTemplateComponent,
    DocTemplateComponent
  ],
  exports:
  [
    CommonModule,
    FormsModule, ReactiveFormsModule,
    MatFormFieldModule, MatInputModule, MatButtonModule, MatCardModule, MatIconModule, MatListModule,
    MatCheckboxModule, MatSelectModule, MatDialogModule, MatTableModule, MatDatepickerModule, MatTabsModule,
    FlexLayoutModule, BoxMaterialModule,
    PageTemplateComponent,
    DocTemplateComponent
  ]
})
export class CaptureTemplatesModule { }

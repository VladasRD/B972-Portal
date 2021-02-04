// angular base
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

// Angular material stuff
import { MatToolbarModule, MatButtonModule, MatMenuModule, MatGridListModule, MatCardModule, MatProgressSpinnerModule,
  MatSnackBarModule, MatIconModule, MatInputModule, MatFormFieldModule, MatListModule,
  MatCheckboxModule, MatSelectModule, MatDialogModule, MatTableModule, MatDatepickerModule,
  MatTabsModule, MatProgressBarModule, MatButtonToggleModule, MatSlideToggleModule, MatNativeDateModule,
  MatSidenavModule, MatChipsModule, MatAutocompleteModule, MatTooltipModule, MatBottomSheetModule } from '@angular/material';

// flex layout
import { FlexLayoutModule } from '@angular/flex-layout';

import { PaginationComponent } from '../pagination/pagination.component';
import { RequiredRolesDirective } from './required-roles.directive';
import { NumericDirective } from './numbers-only.directive';

import { environment } from '../../environments/environment';

// box components
import { BoxMaterialModule } from 'box-material';
import { ChartsModule } from 'ng2-charts';
import { NgxMaskModule } from 'ngx-mask';
import { TextMaskModule } from 'angular2-text-mask';

@NgModule({
  imports: [
    CommonModule,
    ChartsModule,
    FormsModule, ReactiveFormsModule,
    MatToolbarModule, MatButtonModule, MatMenuModule, MatGridListModule, MatCardModule, MatProgressSpinnerModule,
    MatSnackBarModule, MatIconModule, MatInputModule, MatFormFieldModule, MatListModule, MatCheckboxModule,
    MatSelectModule, MatDialogModule, MatTableModule, MatDatepickerModule, MatNativeDateModule,
    MatTabsModule, MatSidenavModule, MatProgressBarModule, MatButtonToggleModule, MatSlideToggleModule,
    MatBottomSheetModule,
    MatChipsModule, MatAutocompleteModule, MatTooltipModule, FlexLayoutModule,
    BoxMaterialModule.forRoot(environment),
    NgxMaskModule.forRoot(),
    TextMaskModule
  ],
  declarations: [
    PaginationComponent,
    RequiredRolesDirective,
    NumericDirective
  ],
  exports: [
    CommonModule,
    ChartsModule,
    FormsModule, ReactiveFormsModule,
    MatToolbarModule, MatButtonModule, MatMenuModule, MatCardModule, MatProgressSpinnerModule,
    MatSnackBarModule, MatIconModule, MatInputModule, MatFormFieldModule, MatListModule, MatCheckboxModule,
    MatSelectModule, MatDialogModule, MatTableModule, MatDatepickerModule, MatNativeDateModule,
    MatTabsModule, MatSidenavModule, MatProgressBarModule, MatButtonToggleModule, MatSlideToggleModule,
    MatChipsModule, MatAutocompleteModule, MatGridListModule, MatTooltipModule,
    MatBottomSheetModule,
    FlexLayoutModule,
    PaginationComponent,
    RequiredRolesDirective,
    NumericDirective,
    BoxMaterialModule,
    NgxMaskModule,
    TextMaskModule,
  ]
})
export class SharedModule { }

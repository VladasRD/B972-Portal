import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

import { SharedModule } from '../common/shared.module';
import { ContentDetailComponent } from './content-detail/content-detail.component';
import { ContentListComponent } from './content-list/content-list.component';
import { FilesListComponent } from './files-list/files-list.component';
import { CrosslinkListComponent } from './crosslink-list/crosslink-list.component';
import { CrosslinkDetailComponent } from './crosslink-detail/crosslink-detail.component';
import { ContentPickerDialogComponent } from './content-picker-dialog/content-picker-dialog.component';


@NgModule({
  imports: [
    RouterModule.forChild([
      { path: 'cms/contents/:kind', component: ContentListComponent, data: { shouldReuse: true } },
      { path: 'cms/contents/:kind/:id', component: ContentDetailComponent },
      { path: 'cms/contents/:kind/:id/:locationIdx', component: ContentDetailComponent },
      { path: 'cms/crosslinks', component: CrosslinkListComponent },
    ]),
    SharedModule
  ],
  exports: [ RouterModule ],
  declarations: [ContentDetailComponent, ContentListComponent, FilesListComponent,
    CrosslinkListComponent, CrosslinkDetailComponent, ContentPickerDialogComponent ],
  entryComponents: [ ContentPickerDialogComponent ]
})
export class CmsModule { }

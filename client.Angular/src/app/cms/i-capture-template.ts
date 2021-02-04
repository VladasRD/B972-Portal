import { CMSService } from './cms.service';
import { IMediaGallery } from 'box-material';
import { FormGroup } from '@angular/forms';

export interface ICaptureTemplate {

    cmsService: CMSService;
    mediaGallery: IMediaGallery;

    head: any;
    content: any;
    form: FormGroup;

    applyValues(): boolean;

}

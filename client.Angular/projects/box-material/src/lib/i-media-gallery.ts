import { FileUploadComponent } from './file-upload/file-upload.component';

export interface IMediaGallery {
    open(folder: string, callBackComponent: any, singleFile: boolean, allowAdd: boolean): void;
}

import { Observable } from 'rxjs';
import { File } from './file';

export interface IUploadService {
    uploadFiles(folder: string, filesToUpload: File[]): Observable<File[]>;
}

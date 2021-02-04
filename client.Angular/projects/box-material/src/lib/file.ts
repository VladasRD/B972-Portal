import { Inject } from '@angular/core';

export class File {

    fileUId: string;
    fileName: string;
    size: number;
    type: string;
    folder: string;
    sent: boolean;
    isSending: boolean;
    _data: any;

    constructor(private environment) {}

    get thumbUrl() {
        if (this.isSending) {
            return this.environment.IDENTITY_SERVER_URL + '/images/cms/file-icons/loading.gif';
        }
        if (this.sent) {
            return this.environment.IDENTITY_SERVER_URL + '/files/' + this.folder + '/' + this.fileUId + '?asThumb=true';
        }
        return this.environment.IDENTITY_SERVER_URL + '/images/cms/file-icons/upload.gif';
    }

    get url() {
        if (!this.sent || this.isSending) {
            return '#';
        }
        return this.environment.IDENTITY_SERVER_URL + '/files/' + this.folder + '/' + this.fileUId;
    }

    toJSON() {
        return { 'fileUId':  this.fileUId, 'fileName': this.fileName, 'size': this.size, 'type': this.type, 'folder': this.folder };
    }

}


export class ServiceDesk {
    serviceDeskId: number;
    deviceId: string;
    createDate: Date;
    finishDate: Date;
    records: ServiceDeskRecord[];
    
    // not mapped
    status: string;
    isOpened: boolean;
}


export class ServiceDeskRecord {
    serviceDeskRecordId: number;
    serviceDeskId: number;
    package: string;
    packageTimestamp: number;
    description: string;
    createDate: Date;
}
import { Project, ProjectEnum } from './project';
import { Package } from './package';
import { Bits } from './Bits';

export class Device {
    id: string;
    name: string;
    sequenceNumber: number;
    lastCom: number;
    state: number;
    comState: number;
    pac: string;
    locationLat: string;
    locationLng: string;
    deviceTypeId: string;
    groupId: string;
    lqi: number;
    activationTime: number;
    tokenState: number;
    tokenDetailMessage: string;
    tokenEnd: number;
    contractId: string;
    creationTime: number;
    modemCertificateId: string;
    prototype: boolean;
    automaticRenewal: boolean;
    automaticRenewalStatus: number;
    createdBy: string;
    lastEditionTime: number;
    lastEditedBy: string;
    activable: boolean;
    bits: Bits;

    get getDeviceNameId(): string {
        return this.id + ' (' + this.name + ')';
    }
}

export class DeviceRegistration {
    deviceCustomUId: string;
    name: string;
    nickName: string;
    deviceId: string;
    device: Device;
    packageUId: string;
    package: Package;
    projectUId: string;
    project: Project;
    model: string;
    serialNumber: string;
    notes: string;

    get getUrlRouterLinkProject(): string {
        if (this.project.code === ProjectEnum.B972) {
            return ('radiodados/dashboard/clampon/' + this.deviceId);
        }
        if (this.project.code === ProjectEnum.B978) {
            return ('radiodados/dashboard/b978/' + this.deviceId);
        }
        if (this.project.code === ProjectEnum.B982_S) {
            return ('radiodados/dashboard/aguamon83/' + this.deviceId);
        }
        if (this.project.code === ProjectEnum.B987) {
            return ('radiodados/dashboard/b987/' + this.deviceId);
        }

        if (this.package.type === '10') {
            return ('radiodados/dashboard/aguamon/' + this.deviceId);
        }
        if (this.package.type === '12') {
            return ('radiodados/dashboard/djrf/' + this.deviceId);
        }
        if (this.package.type === '81') {
            return ('radiodados/dashboard/aguamon81/' + this.deviceId);
        }
        
        if (this.package.type === '21') {
            return ('radiodados/dashboard/aguamon21/' + this.deviceId);
        }
        if (this.project.code === ProjectEnum.B972_P) {
            return ('radiodados/dashboard/trm23/' + this.deviceId);
        }
        
        return ('radiodados/dashboard/' + this.deviceId);
    }

    get getGraphicUrlRouterLinkProject(): string {
        if (this.project.code === ProjectEnum.B972) {
            // return ('radiodados/dashboard/trm23/' + this.deviceId);
            return '/';
        }
        // if (this.project.code === ProjectEnum.B978) {
        //     return ('b978/' + this.deviceId);
        // }
        if (this.project.code === ProjectEnum.B978) {
            return ('trm/' + this.deviceId);
        }
        if (this.project.code === ProjectEnum.B987) {
            return ('b987/' + this.deviceId);
        }
        
        if (this.package.type === '10') {
            return ('aguamon/' + this.deviceId);
        }
        if (this.package.type === '12') {
            return ('djrf/' + this.deviceId);
        }
        if (this.package.type === '81') {
            return ('pqa/' + this.deviceId);
        }
        if (this.package.type === '83') {
            return ('tsp/' + this.deviceId);
        }
        if (this.package.type === '21') {
            return ('trm/' + this.deviceId);
        }
        if (this.package.type === '23') {
            return ('trm23/' + this.deviceId);
        }
        return (this.deviceId);
    }
}

export class DeviceLocation {
    deviceLocationUId: string;
    deviceId: string;
    time: number;
    data: string;
    radius: string;
    latitude: number;
    longitude: number;
}

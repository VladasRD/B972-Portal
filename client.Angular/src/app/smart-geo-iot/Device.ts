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
            return ('clampon/' + this.deviceId);
        }
        if (this.project.code === ProjectEnum.B978) {
            return ('b978/' + this.deviceId);
        }
        if (this.project.code === ProjectEnum.B980) {
            return ('b980/' + this.deviceId);
        }
        if (this.project.code === ProjectEnum.B982_S) {
            return ('aguamon83/' + this.deviceId);
        }
        if (this.project.code === ProjectEnum.B987) {
            return ('b987/' + this.deviceId);
        }
        if (this.project.code === ProjectEnum.B975) {
            return ('b975/' + this.deviceId);
        }

        if (this.package.type === '10') {
            return ('aguamon/' + this.deviceId);
        }
        if (this.package.type === '12') {
            return ('djrf/' + this.deviceId);
        }
        if (this.package.type === '81') {
            return ('aguamon81/' + this.deviceId);
        }
        
        if (this.package.type === '21') {
            return ('aguamon21/' + this.deviceId);
        }
        if (this.project.code === ProjectEnum.B972_P) {
            return ('trm23/' + this.deviceId);
        }
        
        return ('' + this.deviceId);
    }

    get getGraphicUrlRouterLinkProject(): string {
        if (this.project.code === ProjectEnum.B972) {
            // return ('trm23/' + this.deviceId);
            return '/';
        }
        // if (this.project.code === ProjectEnum.B978) {
        //     return ('b978/' + this.deviceId);
        // }
        if (this.project.code === ProjectEnum.B978) {
            return ('trm/' + this.deviceId);
        }
        if (this.project.code === ProjectEnum.B980) {
            return ('trm/' + this.deviceId);
        }
        if (this.project.code === ProjectEnum.B987) {
            return ('b987/' + this.deviceId);
        }
        if (this.project.code === ProjectEnum.B975) {
            return ('b975/' + this.deviceId);
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

export class B975DevicesDashboardViewModels {
    deviceId: string;
    name: string;
    nickName: string;
    date: Date;
    statusDJ: string;
    contadorCarencias: number;
    contadorBloqueios: number;
    locationCity: string;
    hasServiceDeskOpened: boolean;
    hasHistoryServiceDesk: boolean;
}
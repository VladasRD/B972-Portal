import { AppUser } from '../common/appUser';
import { DocumentType } from './documentType';
import { Device } from './device';
import { Utils } from './utils';

export class Client {
    clientUId: string;
    name: string;
    documentType: number;
    document: string;
    address: string;
    addressNumber: string;
    city: string;
    state: string;
    neighborhood: string;
    active: boolean;
    postalCode: string;
    phone: string;

    devices: ClientDevice[];
    users: ClientUser[];
    billings: ClientBilling[];

    startBilling: Date;
    dueDay: number;
    item: string;
    type: number;
    value: number;
    birth: Date;
    cpf: number;
    emailNotification: boolean;
    smsNotification: boolean;
    whatsAppNotification: boolean;
    pushNotification: boolean;
    clientFatherUId: string;

    get documentTypeName(): string {
        return DocumentType.enum[this.documentType];
    }

    get documentMask(): string {
        if (this.document.length < 14) {
            return  Utils.cpf_mask(this.document);
        } else {
            return Utils.cnpj_mask(this.document);
        }
    }

    get clientName(): string {
        return (this.clientFatherUId === null) ? this.name : `${this.name} (Sub-Cliente)`;
    }

    get notificationName(): string {
        let notification = null;
        if (this.emailNotification) {
            notification = 'E-mail';
        }
        if (this.smsNotification) {
            notification = notification === null ? 'SMS' : `${notification} | SMS`;
        }
        if (this.whatsAppNotification) {
            notification = notification === null ? 'WhatsApp' : `${notification} | WhatsApp`;
        }
        if (this.pushNotification) {
            notification = notification === null ? 'Push' : `${notification} | Push`;
        }
        if (notification === null) {
            return 'Não recebe notificações';
        } else {
            return notification;
        }
    }

    static map(obj: any): Client {
        if (!obj.devices) {
            obj.devices = [];
        }
        if (!obj.users) {
            obj.users = [];
        }
        for (let i = 0; i < obj.devices.length; i++) {
            obj.devices[i].appDevice = Object.assign(new Device(), obj.devices[i].appDevice);
        }
        for (let i = 0; i < obj.users.length; i++) {
            obj.users[i].appUser = Object.assign(new AppUser(), obj.users[i].appUser);
        }
        return Object.assign(new Client(), obj);
    }

}

export class ClientDevice {
    clientDeviceUId: string;
    clientUId: string;
    id: string;
    active: boolean;
    appDevice: Device;
}

export class ClientUser {
    clientUserUId: string;
    clientUId: string;
    applicationUserId: string;
    appUser: AppUser;
}

export class BillingType {
    static readonly Mensal = 0;
    static readonly Trimestral = 1;
    static readonly Semestral = 2;
    static readonly Anual = 3;

    static readonly enum = ['Mensal', 'Trimestral', 'Semestral', 'Anual'];
}

export class ClientBilling {
    clientBillingUId: string;
    clientUId: string;
    create: Date;
    paymentDueDate: Date;
    paymentDate: Date;
    sended: boolean;
    externalId: number;
    barCode: string;
    linkPdf: string;
    status: string;
}

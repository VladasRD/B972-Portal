import { SmartGeoIotService } from './smartgeoiot.service';
import { Router } from '@angular/router';

export class Utils {

    public static formatCPF(v: number): string {
        if (v === undefined) {
            return '';
        }
        let str = v.toString();
        if (str.length <= 14) {
            str = str.replace(/(\d{3})(\d)/, '$1.$2');
            str = str.replace(/(\d{3})(\d)/, '$1.$2');
            str = str.replace(/(\d{3})(\d{1,2})$/, '$1-$2');
            return str;
        }
    }

    public static formatCNPJ(v: number): string {
        if (v === undefined) {
            return '';
        }
        const str = v.toString();
        return str.replace(/^(\d{2})(\d{3})(\d{3})(\d{4})(\d{2})/, '$1.$2.$3/$4-$5');
    }

    public static fileDownload(data: any, name: string) {

        // for IE
        if (window.navigator && window.navigator.msSaveOrOpenBlob) {
            window.navigator.msSaveOrOpenBlob(data, name);
            return;
        }

        // for other browsers
        const binaryData = [];
        binaryData.push(data);
        const downloadLink = document.createElement('a');
        downloadLink.href = window.URL.createObjectURL(new Blob(binaryData, { type: data['type'] }));
        downloadLink.download = name;
        document.body.appendChild(downloadLink);
        downloadLink.click();
        window.URL.revokeObjectURL(downloadLink.href);
    }

    public static cpf_mask(v) {
        v = v.replace(/\D/g, '');
        v = v.replace(/(\d{3})(\d)/, '$1.$2');
        v = v.replace(/(\d{3})(\d)/, '$1.$2');
        v = v.replace(/(\d{3})(\d{1,2})$/, '$1-$2');
        return v;
    }

    public static cnpj_mask(v) {
        v = v.replace(/\D/g, '');
        v = v.replace(/^(\d{2})(\d)/, '$1.$2');
        v = v.replace(/^(\d{2})\.(\d{3})(\d)/, '$1.$2.$3');
        v = v.replace(/\.(\d{3})(\d)/, '.$1/$2');
        v = v.replace(/(\d{4})(\d)/, '$1-$2');
        return v;
    }

    public static newGuid(): string {
        return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
            // tslint:disable-next-line: no-bitwise
            const r = Math.random() * 16 | 0, v = c === 'x' ? r : (r & 0x3 | 0x8);
            return v.toString(16);
        });
    }

    // tslint:disable-next-line: member-ordering
    public static readonly enumMonths = ['Todos os meses', 'Janeiro', 'Fevereiro', 'Março', 'Abril', 'Maio', 'Junho', 'Julho', 'Agosto', 'Setembro', 'Outubro', 'Novembro', 'Dezembro'];

}

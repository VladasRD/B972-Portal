import { Bits, Rele } from './Bits';
import { environment } from '../../environments/environment.stage';
import { EstadoDJ } from './dashboard';
import { Utils } from './utils';

export class ReportTypeEnum {
    static readonly enumNameColumnGraphic = ['', 'consumoAgua', 'consumoDia', 'consumoSemana', 'consumoMes'];
}

export class Report {
    deviceId: string;
    package: string;
    typePackage: string;
    date: Date;
    country: string;
    lqi: number;
    bits: Bits;
    level: string;
    light: string;
    temperature: string;
    moisture: string;
    oxigenioDissolvido: string;
    ph: string;
    condutividade: string;
    periodoTransmissao: string;
    baseT: number;
    envio: number;
    alimentacao: string;
    alimentacaoMinima: string;
    estadoDetector: number;
    contadorCarencias: string;
    contadorBloqueios: string;
    latitude: string;
    longitude: string;
    radius: string;
    latitudeConverted: string;
    longitudeConverted: string;
    radiusConverted: string;
    locationCity: string;
    alertaFonteBaixa: boolean;
    tensaoMinima: string;
    fluor: string;
    cloro: string;
    turbidez: string;
    rele: Rele;
    vazao: string;
    totalizacaoParcial: string;
    totalizacao: string;
    tempoParcial: string;

    entradaAnalogica: string;
    saidaAnalogica: string;
    fluxoAgua: string;
    consumoAgua: string;
    modo: string;
    estado: string;
    valvula: string;
    estadoColor: string;
    dateWeekName: string;

    get nameEstadoDetector(): string {
        return EstadoDJ.enum[this.estadoDetector];
    }

    get montYearDisplayName(): string {
        const data = new Date(this.date);
        return `${Utils.enumMonths[data.getUTCMonth()+1]}/${data.getUTCFullYear()}`;
    }
}

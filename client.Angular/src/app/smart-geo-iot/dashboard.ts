import { environment } from './../../environments/environment';
import { B975 } from './B975';
import { Bits, Rele, ReleBoolean } from './Bits';
import { MCond } from './MCond';

export class EstadoDJ {
    static readonly Aguardando = 0;
    static readonly Operacional = 1;
    static readonly EmCarencia = 2;
    static readonly EmCiclos = 3;
    static readonly EmBloqueio = 4;
    static readonly EmDormencia = 5;
    static readonly enum = ['Aguardando', 'Operacional', 'Em Carência', 'Em Ciclos', 'Em Bloqueio', 'Em Dormência'];
    static readonly enumBackground = ['bg-aguardando', 'bg-operacional', 'bg-carencia', 'bg-ciclos', 'bg-bloqueio', 'bg-dormencia'];
    static readonly enumFonteColor = ['fc-aguardando', 'fc-operacional', 'fc-carencia', 'fc-ciclos', 'fc-bloqueio', 'fc-dormencia'];
}

export class DownloadLink {
    numeroEnvios: number;
    tempoTransmissao: number;
    tipoEnvio: boolean;
    tensaoMinima: number;
}

export class Dashboard {
    deviceId: string;
    name: string;
    package: string;
    typePackage: string;
    date: Date;
    country: string;
    lqi: number;
    bits: Bits;
    seqNumber: number;
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
    releBoolean: ReleBoolean;
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
    estadoImage: string;
    modoImage: string
    estadoColor: string;
    seqNumberb: number;

    downloadLink: DownloadLink;
    calha: string;
    calhaAlerta: string;
    calhaImage: string;

    consumoDia: string;
    consumoSemana: string;
    consumoMes: string;
    time: number;

    // B972
    flow: string;
    velocity: string;
    flags: string;
    total: string;
    partial: string;

    serialNumber: string;
    model: string;
    notes: string;
    notesCreateDate: string;
    ed1: string;
    ed2: string;
    ed3: string;
    ed4: string;
    sd1: string;
    sd2: string;
    ea10: string;
    sa3: string;
    mCond: MCond;
    b975: B975;

    get nameEstadoDetector(): string {
        return EstadoDJ.enum[this.estadoDetector];
    }

    get backgroungEstadoDetector(): string {
        return EstadoDJ.enumBackground[this.estadoDetector];
    }

    get fonteColorEstadoDetector(): string {
        return EstadoDJ.enumFonteColor[this.estadoDetector];
    }

    get stadoNameOne(): string {
        return this.estado ? this.estado.split(",", 2)[0] : '';
    }

    get stadoNameTwo(): string {
        return this.estado ? this.estado.split(",", 2)[1] : '';
    }

    get linkMaps(): string {
        return `${environment.URL_MAPS}${this.latitude.replace(',', '.')},${this.longitude.replace(',', '.')}`;
    }
}

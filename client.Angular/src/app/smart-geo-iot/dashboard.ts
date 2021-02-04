import { environment } from './../../environments/environment';
import { Bits, Rele } from './Bits';

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

export class Dashboard {
    deviceId: string;
    name: string;
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

    get nameEstadoDetector(): string {
        return EstadoDJ.enum[this.estadoDetector];
    }

    get backgroungEstadoDetector(): string {
        return EstadoDJ.enumBackground[this.estadoDetector];
    }

    get fonteColorEstadoDetector(): string {
        return EstadoDJ.enumFonteColor[this.estadoDetector];
    }

    get linkMaps(): string {
        return `${environment.URL_MAPS}${this.latitude.replace(',', '.')},${this.longitude.replace(',', '.')}`;
    }
}

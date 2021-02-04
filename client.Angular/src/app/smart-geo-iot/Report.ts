import { Bits } from './Bits';
import { environment } from '../../environments/environment.stage';
import { EstadoDJ } from './dashboard';

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

    get nameEstadoDetector(): string {
        return EstadoDJ.enum[this.estadoDetector];
    }
}

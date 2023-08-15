export class EstadoDJ {
    static readonly Aguardando = 0;
    static readonly Operacional = 1;
    static readonly EmCarencia = 2;
    static readonly EmCiclos = 3;
    static readonly EmBloqueio = 4;
    static readonly EmDormencia = 5;
    // static readonly enum = ['Aguardando', 'Operacional', 'Em Carência', 'Em Ciclos', 'Em Bloqueio', 'Em Dormência'];
    static readonly enumBackground = ['bg-aguardando', 'bg-operacional', 'bg-carencia', 'bg-ciclos', 'bg-bloqueio', 'bg-dormencia'];
    static readonly enumText = ['Aguardando', 'Operacional', 'Em carência', 'Jammer', 'Jammer', 'Em dormência'];
}

export class B975 {
    id: number;
    deviceId: string;
    date: Date;

    // Pacote - 1A P975U4
    packA: string;
    timeA: number;
    pcPosChave: boolean;
    jam: boolean;
    vio: boolean;
    rasIn: boolean;
    bloqueio: boolean;
    rasOut: boolean;
    statusDJ: string;
    alertaFonteBaixa: boolean;
    intervaloUpLink: number;
    contadorCarencias: number;
    contadorBloqueios: number;

    // Pacote - 1B P975U5
    packB: string;
    timeB: number;
    temperaturaInterna: number;
    tensaoAlimentacao: number;

    // Pacote - 1C P975U6
    packC: string;
    timeC: number;
    mediaRFMinimo: number;
    mediaRFMaximo: number;
    mediaLinhaBase: number;
    mediaInterferencia: number;
    deteccaoInterferencia: number;
    deteccaoJammer: number;
    numeroViolacao: number;

    source: string;
    radius: string;
    latitude: number;
    longitude: boolean;
    locationCity: string;
    lqi: number;
}
export class Project {
    projectUId: string;
    name: string;
    description: string;
    code: string;

    get getUrlReportLink(): string {
        if (this.name.toLowerCase() === 'djrf') {
            return 'djrf';
        }
        if (this.name.toLowerCase() === 'aguamon') {
            return 'aguamon';
        }
        if (this.name.toLowerCase() === 'aguamon-2') {
            return 'aguamon2';
        }
        if (this.code === ProjectEnum.B972_P) {
            return 'trm';
        }
        // if (this.name.toLowerCase() === 'tsp') {
        //     return 'tsp';
        // }
        if (this.code === ProjectEnum.B981) {
            return ('tqa');
        }
        if (this.code === ProjectEnum.B972) {
            return ('clampon');
        }
        if (this.code === ProjectEnum.B982_S) {
            return ('tsp');
        }
        if (this.code === ProjectEnum.B978) {
            return ('b978');
        }
        if (this.code === ProjectEnum.B987) {
            return ('b987');
        }
        return 'hidro';
    }
}

export class ProjectEnum {
    static readonly B972 = 'B982-C';
    static readonly B982_S = 'B982-S';
    static readonly Hidroleg = 'Hidroleg';
    static readonly DJRFleg = 'DJRFleg';
    static readonly AM_1leg = 'AM-1leg';
    static readonly FluxoRDleg = 'FluxoRDleg';
    static readonly B972_P = 'B972-P';
    static readonly B981 = 'B981';
    static readonly B978 = 'B978';
    static readonly B987 = 'B987';
}

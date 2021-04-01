export class Project {
    projectUId: string;
    name: string;
    description: string;

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
        if (this.name === 'TRM-10 (B972)') {
            return 'trm';
        }
        return 'hidro';
    }
}

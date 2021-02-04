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
        return 'hidro';
    }
}

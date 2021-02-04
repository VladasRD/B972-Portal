import { Utils } from './utils';

export class OutgoingClient {
    name: string;
    total: number;
}
export class Outgoing {
    outgoingUId: string;
    year: number;
    month: number;
    licensesActive: number;
    clientsActive: number;
    description: string;
    developmentValue: number;
    operationsWNDValue: number;
    dataCenterValue: number;
    operationValue: number;
    totalBilling: number;
    averageLicensesActived: number;
    averageForClient: number;
    averageForLicenseClient: number;
    totalBillingYear: number;
    developmentValueYear: number;
    operationsWNDValueYear: number;
    dataCenterValueYear: number;
    operationValueYear: number;
    licensesActiveYear: number;
    

    get montYearDisplayName(): string {
        return `${Utils.enumMonths[this.month]}/${this.year}`;
    }

    get clientsLicensesDisplayName(): string {
        return `Clientes: ${this.clientsActive} / Licensas: ${this.licensesActive}`;
    }

    get totalCosts(): number {
        return this.developmentValue + this.operationsWNDValue + this.dataCenterValue + this.operationValue;
    }

    get saldo(): number {
        return this.totalBilling - this.totalCosts;
    }

    get totalCostsYear(): number {
        return this.developmentValueYear + this.operationsWNDValueYear + this.dataCenterValueYear + this.operationValueYear;
    }

    get saldoYear(): number {
        return this.totalBillingYear - this.totalCostsYear;
    }

    get totalBillingAverageLicensesYear(): number {
        return this.totalBillingYear / this.licensesActiveYear;
    }

    get totalCostsAverageLicensesYear(): number {
        return this.totalCostsYear / this.licensesActiveYear;
    }
}

export class OutgoingViewModel {
    outgoing: Outgoing;
    clients: OutgoingClient[];
}

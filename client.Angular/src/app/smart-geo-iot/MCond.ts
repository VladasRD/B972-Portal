
export class MCondGraphicFilterTypeEnum {
    static readonly enumNameColumnGraphic = ['', 'supLevelLitros', 'infLevelLitros'];
}
export class MCond {
    id: number;
    deviceId: string;
    time: number;
    packInf: string;
    supStateBomb: boolean;
    infAlarmLevelMin: boolean;
    infAlarmLevelMax: boolean;
    infLevel: number;
    packSup: string;
    supAlarmLevelMin: boolean;
    supAlarmLevelMax: boolean;
    supLevel: number;
    packPort: string;
    portFireAlarm: boolean;
    portIvaAlarm: boolean;
    portFireState: boolean;
    portIvaState: boolean;
    date: Date;

    packPortSubType: number;
    packSupSubType: number;
    packInfSubType: number;
    dateGMTBrasilian: Date;

    supLevelLitros: number;
    infLevelLitros: number;
}
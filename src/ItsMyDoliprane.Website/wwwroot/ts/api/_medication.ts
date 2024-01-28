import { httpToJson } from "./_api-core.js";

export interface Medication {
    personId: number,
    drugId: number,
    date: string,
    hour: string
}

export class ApiMedication {
    static add(medication: Medication): Promise<void> {
        return httpToJson<null>("POST", "api/medication", medication).then(() => {
        });
    }
}
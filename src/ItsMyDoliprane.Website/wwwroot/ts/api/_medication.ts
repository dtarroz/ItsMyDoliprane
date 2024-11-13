import { httpToJson } from "./_api-core.js";

export interface NewMedication {
    personId: number,
    drugId: number,
    date: string,
    hour: string
}

export class ApiMedication {
    static add(newMedication: NewMedication): Promise<void> {
        return httpToJson<null>("POST", "api/medication", newMedication).then(() => {
        });
    }

    static delete(medicationId: number): Promise<void> {
        return httpToJson<null>("DELETE", "api/medication", medicationId).then(() => {
        });
    }
}
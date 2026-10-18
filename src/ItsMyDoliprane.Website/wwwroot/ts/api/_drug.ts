import { httpToJson } from "./_api-core.js";

export interface UpdateDrugRequest {
    id: number;
    forAdult: boolean;
    forChild: boolean;
}

export class ApiDrug {
    static update(request: UpdateDrugRequest): Promise<void> {
        return httpToJson<null>("PUT", "api/drug", request).then(() => {
        });
    }
}


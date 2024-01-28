interface ApiJson<T> {
    code: number,
    result: T
}

enum JsonErrorCode {
    UnknownError = -1,
    Ok = 0
}

export function httpToJson<T>(method: string, url: string, body: any = null): Promise<T> {
    return new Promise((resolve, reject) => {
        fetch(url, {
            method: method,
            headers: body ? { "Content-Type": "application/json" } : {},
            body: body ? JSON.stringify(body) : null
        })
        .then((res) => res.json())
        .then((res: ApiJson<T>) => {
            if (res.code === JsonErrorCode.Ok)
                resolve(res.result);
            else
                reject(errorMessageFromCode(res.code));
        })
        .catch(() => reject(errorMessageFromCode(-1)));
    });
}

function errorMessageFromCode(code: number): string {
    switch (code) {
        case JsonErrorCode.UnknownError:
            return "Une erreur s'est produite, veuillez réessayer";
        default:
            return "Une erreur s'est produite, veuillez réessayer"
    }
}
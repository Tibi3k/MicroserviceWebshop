export interface Headers {
    normalizedNames: any;
    lazyUpdate?: any;
}

export interface Error {
    type: string;
    title: string;
    status: number;
    traceId: string;
}

export interface ErrorObject {
    headers: Headers;
    status: number;
    statusText: string;
    url: string;
    ok: boolean;
    name: string;
    message: string;
    error: Error;
}

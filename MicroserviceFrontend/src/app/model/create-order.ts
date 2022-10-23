export interface Link {
    href: string;
    rel: string;
    method: string;
}

export interface CreatePaypalOrder {
    id: string;
    status: string;
    links: Link[];
}
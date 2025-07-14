/**
 * Abbreviated document.getElementById().
 * Throws if element not found.
 * */
export function gebi(id: string): HTMLElement {
    const el = document.getElementById(id);
    if (!el) {
        throw new Error(`No element found with id ${id}.`);
    }
    return el;
}

export async function fetchOrThrow(url: string): Promise<Response> {
    const response = await fetch(url);
    if (!response.ok) {
        throw new Error (`Failed to fetch ${url}: ${response.status}, ${response.statusText}`);
    }
    return response;
}


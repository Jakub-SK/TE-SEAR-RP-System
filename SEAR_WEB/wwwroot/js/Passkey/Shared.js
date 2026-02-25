function base64UrlToBuffer(base64url) {
    const padding = '='.repeat((4 - base64url.length % 4) % 4);
    const base64 = (base64url + padding)
        .replace(/-/g, '+')
        .replace(/_/g, '/');

    const raw = window.atob(base64);
    const buffer = new ArrayBuffer(raw.length);
    const view = new Uint8Array(buffer);

    for (let i = 0; i < raw.length; ++i)
        view[i] = raw.charCodeAt(i);

    return buffer;
}

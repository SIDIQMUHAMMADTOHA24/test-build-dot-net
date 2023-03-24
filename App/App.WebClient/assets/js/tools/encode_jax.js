function encode_jax(rawStr) {
    return rawStr.replace(/[\u00A0-\u9999<>\&]/g, ((i) => `&#${i.charCodeAt(0)};`));
}
function UrlEncode(string) {
    var ret = string.replace(/\+/g, "-").replace(/\//g, "_");
    if (ret.length === 0) {
        public_key = "";
    }

    var noPadding = ret.replace(/=+$/, "");
    string = noPadding + (ret.length - noPadding.length).toString();
    return string;
}
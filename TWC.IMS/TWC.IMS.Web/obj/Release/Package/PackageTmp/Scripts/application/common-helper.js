commonHelper = {
    getConfigValue: function (name) {
        let url = window.rootUrl + "common/getsystemconfigvalue";
        let data = { name: name };
        return $.getJSON(url, data);
    },
    hasValue: function(str) {
        if(str !== null && str !== undefined)
            str = str.trim();

        return str !== null && str !== "" && str !== undefined
    },
    isDateValid: function(strDate) {
        if(strDate instanceof Date) return strDate;

        var timestamp = Date.parse(strDate);
        return !isNaN(timestamp)

    },
    isNullUndefinedOrWhiteSpace: function (str) {
        if (str !== null && str !== undefined) { return str.trim() === ""; }
        return str === null || str === undefined || str === "";
    },
    extractDate: function(date) {
        
        if(this.isDateValid(date))
            return (date.getMonth() + 1) + "/" + date.getDate() + "/" + date.getFullYear()
        
        return undefined;
    }
}
Date.prototype.addDays = function(days) {
    var dat = new Date(this.valueOf());
    dat.setDate(dat.getDate() + days);
    return dat;
}

Date.prototype.addHours = function(hours) {
    this.setHours(this.getHours() + hours);
    return this;
}
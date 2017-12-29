exports.preTransform = function (model) {
  return model;
}

exports.postTransform = function (model) {
  var date = new Date();
  var fdate = (date.getDate()) + "." + (date.getMonth() + 1) + "." + date.getFullYear();
  model._dateString = "<br /><br />Zuletzt aktualisiert am " + fdate;
  return model;
}

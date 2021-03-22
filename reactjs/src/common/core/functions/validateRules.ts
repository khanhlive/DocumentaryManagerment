import moment from "moment";

export function validatorCode(value: any, validator: any, $field: any) {
  var reg = /^[\d\w_]+$/;
  return reg.test(value);
}

export function validatorTextNumber(value: any, validator: any, $field: any) {
  var reg = /^[\d\w_\-\/\,\/\s]+$/;
  return true; //reg.test(value);
}

export function validatorDate(value: any, validator: any, $field: any) {
  if (value == null || value == undefined || value.trim() == "") {
    return true;
  }
  if (value.length != 10) {
    return false;
  }
  let date = moment(value, "DD/MM/YYYY");
  return date.isValid();
}

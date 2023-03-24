$('#birthdate').datepicker({
    rtl: false,
    orientation: "bottom left",
    todayHighlight: true,
    templates: {
        leftArrow: '<i class="la la-angle-left"></i>',
        rightArrow: '<i class="la la-angle-right"></i>'
    },
    format: "dd-mm-yyyy"
});
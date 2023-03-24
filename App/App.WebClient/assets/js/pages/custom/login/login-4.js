"use strict";

// Class Definition
var KTLogin = function() {
	var _buttonSpinnerClasses = 'spinner spinner-right spinner-white pr-15';

	var _handleFormSignup = function() {
		// Base elements
		var wizardEl = KTUtil.getById('kt_login');
		var form = KTUtil.getById('kt_login_signup_form');
		var wizardObj;
		var validations = [];

		if (!form) {
			return;
		}

		// Init form validation rules. For more info check the FormValidation plugin's official documentation:https://formvalidation.io/
		// Step 1
		validations.push(FormValidation.formValidation(
			form,
			{
				fields: {
					nik: {
						validators: {
							notEmpty: {
								message: 'NIK (No ID Card) is required'
							},
							numeric: {
								message: 'The value is not numeric'
							},
							callback: {
								callback: function (value, validator, $field) {
									// Check for 16 characters length.
									var result = {
										valid: true,
										message: ''
									};
									if (value.value.length !== 16) {
										result.valid = false;
										result.message += "NIK/ No ID Card must contains at 16 characters"
									}
									return result;
								}
							}
						}
					},
					fname: {
						validators: {
							notEmpty: {
								message: 'Full name is required'
							}
						}
					},
					birthplace: {
						validators: {
							notEmpty: {
								message: 'Birth place is required'
							}
						}
					},
					birthdate: {
						validators: {
							notEmpty: {
								message: 'Birth date is required'
							}
						}
					},
					fdata: {
						validators: {
							notEmpty: {
								message: 'ID Card is required'
							},
							file: {
								extension: 'jpeg,jpg,png',
								type: 'image/jpeg,image/png',
								maxSize: 5242880,
								message: 'The selected file is not valid, just image with max size 5 Mb'
							}
							//callback: {
							//	callback: function (value, validator, $field) {
							//		var file = value.elements[0].files;
							//		var reader = new FileReaderSync();
							//		var file_data = new Blob(file);
							//		reader.readAsDataURL(file_data);
							//		reader.onloadend = function (readerEvt) {
							//			var result = {
							//				valid: true,
							//				message: ''
							//			};
							//			var binary_string = window.atob(readerEvt.target.result.split(',')[1]);
							//			var header = "";
							//			for (var i = 0; i < 4; i++) {
							//				var zz = binary_string.charCodeAt(i);
							//				header += zz.toString(16);
							//			}
							//			var list_result = ["89504e47", "ffd8ffe0", "ffd8ffe1", "ffd8ffdb", "ffd8ffe2"];
							//			if (list_result.find(o => o === header) === undefined) {
							//				result.valid = false;
							//				result.message = "The selected file is not valid, just image with max size 5 Mb"
							//			}
							//			return result;
							//		};
							//		//reader.readAsArrayBuffer(file);
							//	}
							//}
						}
					}
				},
				plugins: {
					trigger: new FormValidation.plugins.Trigger(),
					bootstrap: new FormValidation.plugins.Bootstrap()
				}
			}
		));

		// Step 1-1
		validations.push(FormValidation.formValidation(
			form,
			{
				fields: {
					email: {
						validators: {
							notEmpty: {
								message: 'Email is required'
							},
							emailAddress: {
								message: 'The value is not a valid email address'
							}
						}
					},
					phone: {
						validators: {
							notEmpty: {
								message: 'Phone number is required'
							},
							numeric: {
								message: 'The value is not numeric'
							}
						}
					},
					password: {
						validators: {
							notEmpty: {
								message: 'Password is required'
							},
							callback: {
								callback: function (value, validator, $field) {
									// Count the number of digits in your password
									//var digitsCount = value.search(/[0-9]/);
									var r1 = new RegExp('(?=.*[a-z])');
									var r2 = new RegExp('(?=.*[A-Z])');
									var r3 = new RegExp('(?=.*[0-9])');
									var r4 = new RegExp('(?=.*[!@#\$%\^&\*])');

									// Check for 6 characters length.
									var result = {
										valid: true,
										message: ''
									};
									if (value.value.length < 8) {
										result.valid = false;
										result.message += "Password must contains at least min 8 characters <br>"
									}

									if (!r1.test(value.value)) {
										result.valid = false;
										result.message += "Password must contain at least 1 lowercase alphabetical character <br>"
									}

									if (!r2.test(value.value)) {
										result.valid = false;
										result.message += "Password must contain at least 1 uppercase alphabetical character <br>"
									}

									if (!r3.test(value.value)) {
										result.valid = false;
										result.message += "Password must contain at least 1 numeric character <br>"
									}

									if (!r4.test(value.value)) {
										result.valid = false;
										result.message += "Password must contain at least one special character (!@#$%^&*)"
									}

									return result;
								}
							}
						}
					},
					confirm_password: {
						validators: {
							notEmpty: {
								message: 'Confirm Password is required'
							},
							callback: {
								callback: function (value, validator, $field) {
									var password = $('#password').val();

									var result = {
										valid: true,
										message: ''
									};
									if (password !== value.value) {
										result.valid = false;
										result.message = "Confirm password not same with field Password"
									}
									return result;
								}
							}
						}
					}
				},
				plugins: {
					trigger: new FormValidation.plugins.Trigger(),
					bootstrap: new FormValidation.plugins.Bootstrap()
				}
			}
		));

		// Step 2
		validations.push(FormValidation.formValidation(
			form,
			{
				plugins: {
					trigger: new FormValidation.plugins.Trigger(),
					bootstrap: new FormValidation.plugins.Bootstrap()
				}
			}
		));

		//step 3
		validations.push(FormValidation.formValidation(
			form,
			{
				fields: {
					m_option_1: {
						validators: {
							notEmpty: {
								message: 'Product is required'
							}
						}
					}
				},
				plugins: {
					trigger: new FormValidation.plugins.Trigger(),
					bootstrap: new FormValidation.plugins.Bootstrap()
				}
			}
		));

		//step 4
		validations.push(FormValidation.formValidation(
			form,
			{
				fields: {
					confirm_submit: {
						validators: {
							notEmpty: {
								message: 'Please Approval'
							}
						}
					}
				},
				plugins: {
					trigger: new FormValidation.plugins.Trigger(),
					bootstrap: new FormValidation.plugins.Bootstrap()
				}
			}
		));

		// Initialize form wizard
		wizardObj = new KTWizard(wizardEl, {
			startStep: 1, // initial active step number
			clickableSteps: false // to make steps clickable this set value true and add data-wizard-clickable="true" in HTML for class="wizard" element
		});

		// Validation before going to next page
		wizardObj.on('beforeNext', function (wizard) {
			validations[wizard.getStep() - 1].validate().then(function (status) {
				if (status === 'Valid') {
					wizardObj.goNext();
					if (wizard.getStep() === 3) {
						Webcam();
					}
					else {
						StopWebcam();
                    }
					KTUtil.scrollTop();
				} else {
					Swal.fire({
						text: "Sorry, looks like there are some errors detected, please try again.",
						icon: "error",
						buttonsStyling: false,
						confirmButtonText: "Ok, got it!",
						customClass: {
							confirmButton: "btn font-weight-bold btn-light-primary"
						}
					}).then(function () {
						KTUtil.scrollTop();
					});
				}
			});

			wizardObj.stop();  // Don't go to the next step
		});

		wizardObj.on('beforePrev', function (wizard) {
			var a = wizard.getStep();
				if (a === 3) {
					StopWebcam();
				}
				else if(a === 4) {
					Webcam();
				}
		});

		// Change event
		wizardObj.on('change', function (wizard) {
			KTUtil.scrollTop();
		});
    }

    // Public Functions
    return {
        init: function() {
   //         _handleFormSignin();
			//_handleFormForgot();
			_handleFormSignup();
        }
    };
}();

// Class Initialization
jQuery(document).ready(function() {
    KTLogin.init();
});

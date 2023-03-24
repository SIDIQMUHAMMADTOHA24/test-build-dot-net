$(document).ready(function () {
	ClickMenu('m_dash');
	// Shared Colors Definition
	const primary = '#6993FF';
	const success = '#1BC5BD';
	const info = '#8950FC';
	const warning = '#FFA800';
	const danger = '#F64E60';

	// Class definition
	OpenLoading();
	$.ajax({
		type: "GET",
		url: "/User/GetSumStatus",
		success: function (msg) {
			if (msg.result) {
				var list_status = msg.objek.sum_status;
				var sum = list_status.reduce((a, b) => a + b, 0);
				$('#u_total').html(sum);
				$('#u_active').html(list_status[1]);
				$('#u_notactive').html(list_status[0]);
				$('#u_pending').html(list_status[2]);

				var per_weeks = msg.objek.list_sum_per_weeks;
				var for_day = [];
				var for_count = [];
				for (var pw in per_weeks) {
					for_day.push(per_weeks[pw].day);
					for_count.push(per_weeks[pw].count);
                }
				var KTApexChartsDemo = function () {
					var _demo1 = function () {
						const apexChart = "#chart_3";
						var options = {
							series: [{
								name: 'Number of Days',
								data: for_count
							}],
							chart: {
								height: 350,
								type: 'line',
								zoom: {
									enabled: false
								}
							},
							dataLabels: {
								enabled: false
							},
							stroke: {
								curve: 'smooth'
							},
							grid: {
								row: {
									colors: ['#f3f3f3', 'transparent'], // takes an array which will be repeated on columns
									opacity: 0.5
								},
							},
							xaxis: {
								categories: for_day,
							},
							colors: [primary]
						};

						var chart = new ApexCharts(document.querySelector(apexChart), options);
						chart.render();
						CloseLoading();
					}

					return {
						// public functions
						init: function () {
							_demo1();
						}
					};
				}();

				jQuery(document).ready(function () {
					KTApexChartsDemo.init();
				});
			}
			else {
				toastr.error(msg.message);
			}
		}
	});
});

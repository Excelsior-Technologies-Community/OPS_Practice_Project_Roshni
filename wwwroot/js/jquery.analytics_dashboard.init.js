document.addEventListener("DOMContentLoaded", function () {
    let chartElement = document.querySelector("#chart"); // Change ID based on your HTML
    if (!chartElement) {
        console.error("Error: Chart container element not found!");
        return;
    }

    var options = {
        chart: {
            type: "line", // Change based on your chart type
            height: 350
        },
        series: [{ name: "Data", data: [10, 20, 30, 40, 50] }]
    };

    var chart = new ApexCharts(chartElement, options);
    chart.render();
});

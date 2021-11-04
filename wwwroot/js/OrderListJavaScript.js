var dataTable;
$(document).ready(function () {
    var url = window.location.search;
    if (url.includes("cancelled")) {
        loadTable("cancelled");
    }
    else if (url.includes("completed")) {
        loadTable("completed");
    }
    else if (url.includes("inprocess")) {
        loadTable("inprocess");
    }
    else {
        loadTable("");
    }
});
function loadTable(status) {
    dataTable = $('#myTable').DataTable({
        "ajax": {
            "url": "/api/order?status=" + status,
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "orderHeader.orderHeaderID", "width": "20%" },
            { "data": "orderHeader.deliveryName", "width": "20%" },
            { "data": "orderHeader.applicationUser.email", "width": "20%" },
            { "data": "orderHeader.orderTotal", "width": "20%" },
            
            {
                "data": "orderHeader.orderHeaderID",
                "render": function (data) {
                    return ` <div class="text-center">
                            <a href="/Admin/Order/OrderDetails?orderHeaderID=${data}" class="btn btn-success text-white" 
                                    style="cursor:pointer;width:100px">
                               <i class="fas fa-info"></i> Details
                           
                    </div>`;
                }, "width": "20%"
            }
        ],
        "language": {
            "emptyTable": "No data found"
        },
        "width": "100%"
    });


}
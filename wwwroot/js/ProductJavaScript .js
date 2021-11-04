var dataTable;
$(document).ready(function () {
    loadTable();
});
function loadTable() {
    dataTable = $('#myTable').DataTable({
        "ajax": {
            "url": "/api/Product",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "name", "width": "15%" },
            { "data": "cost", "width": "15%" },
            { "data": "category.name", "width": "15%" },
            { "data": "productType.name", "width": "15%" },
            { "data": "applicationUser.fullName", "width": "15%" },
           
            {
                "data": "productID",
                "render": function (data) {
                    return ` <div class="text-center">
                            <a href="/Admin/Product/Upsert?productID=${data}" class="btn btn-success text-white" 
                                       style="cursor:pointer;width:45%">
                                <i class="fas fa-edit"></i> Edit
                            </a>
                                <a  class="btn btn-danger text-white" style="cursor: pointer;width:50%"
                                        onclick="Delete('/api/Product/'+${data})">
                                <i class="fas fa-trash"></i> Delete
                            </a>
                    </div>`;
                }, "width": "25%"
            }
        ],
        "language": {
            "emptyTable": "No data found"
        },
        "width": "100%"
    });


}
function Delete(_url) {
    swal({
        title: "Are you sure?",
        text: "Once deleted, it could not be restored!",
        icon: "warning",
        buttons: true,
        dangerMode: true
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                type: "DELETE",
                url: _url,
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        dataTable.ajax.reload();
                    }
                    else {
                        toastr.error(data.message);
                    }
                },

            });
        }
    }
    )
}
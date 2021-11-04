var dataTable;
$(document).ready(function () {
    loadTable();
});
function loadTable() {
    dataTable = $('#myTable').DataTable({
        "ajax": {
            "url": "/api/category",
            "type": "GET",
            "datatype":"json"
        },
        "columns": [
            { "data": "name", "width": "40%" },
            { "data": "displayOrder", "width": "30%" },
            {
                "data": "categoryID",
                "render": function (data) {
                    return ` <div class="text-center">
                            <a href="/Admin/category/upsert?categoryID=${data}" class="btn btn-success text-white" 
                                    style="cursor:pointer;width:100px">
                                <i class="fas fa-edit"></i> Edit
                            </a>
                                <a  class="btn btn-danger text-white" style="cursor: pointer;width:100px"
                                     onclick="Delete('/api/category/'+${data})">
                                <i class="fas fa-trash"></i> Delete
                            </a>
                    </div>`;
                }, "width":"30%"
            }
        ],
        "language": {
            "emptyTable":"No data found"
        },
        "width":"100%"
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
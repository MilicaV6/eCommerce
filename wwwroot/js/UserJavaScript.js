var dataTable;
$(document).ready(function () {
    loadTable();
});
function loadTable() {
    dataTable = $('#myTable').DataTable({
        "ajax": {
            "url": "/api/ApplicationUser",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "fullName", "width": "25%" },
            { "data": "email", "width": "25%" },
            { "data": "phoneNumber", "width": "25%" },
            {
                "data": { id: "id", lockoutEnd: "lockoutEnd"},
                "render": function (data) {
                    var dateNow = new Date().getTime();
                    var lockout = new Date(data.lockoutEnd).getTime();
                    if (lockout > dateNow) {

                        return ` <div class="text-center">
                            <a  class="btn btn-danger text-white" 
                                    style="cursor:pointer;width:100px"
                                    onclick=LockUnlock('${data.id}')>
                                <i class="fas fa-lock-open"></i> Unlock
                            </a>
                            </div>`;
                    }
                    else {
                        return ` <div class="text-center">
                            <a  class="btn btn-success text-white" 
                                    style="cursor:pointer;width:100px"
                                    onclick=LockUnlock('${data.id}')>
                                <i class="fas fa-lock"></i> Lock        
                            </a>
                            </div>`;
                    }

                   
                }, "width": "25%"
            }
        ],
        "language": {
            "emptyTable": "No data found"
        },
        "width": "100%"
    });


}
function LockUnlock(id) {
   
            $.ajax({
                type: "POST",
                url: '/api/ApplicationUser',
                data: JSON.stringify(id),
                contentType:"application/json",
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
var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/CoverType/GetAll"
        },
        "columns": [
            { "data": "name", "width": "70%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="text-center">
                            <a href="/Admin/CoverType/Upsert/${data}"
                               class="btn btn-success text-white"
                               style="cursor:pointer; width:100px">
                                <i class="fas fa-edit"></i> Edit
                            </a>
                            &nbsp;
                            <a onclick="Delete('/Admin/CoverType/Delete/${data}')"
                               class="btn btn-danger text-white"
                               style="cursor:pointer; width:100px">
                                <i class="fas fa-trash-alt"></i> Delete
                            </a>
                        </div>`;
                },
                "width": "30%"
            }
        ]
    });
}

function Delete(url) {
    swal({
        title: "Are you sure?",
        text: "You will not be able to restore the data!",
        icon: "warning",
        buttons: true,
        dangerMode: true
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                type: "DELETE",
                url: url,
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        dataTable.ajax.reload();
                    } else {
                        toastr.error(data.message);
                    }
                }
            });
        }
    });
}
var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/Product/GetAll"
        },
        "columns": [
            { "data": "title", "width": "15%" },
            { "data": "author", "width": "15%" },
            { "data": "isbn", "width": "15%" },
            { "data": "listPrice", "width": "10%" },
            { "data": "categoryName", "width": "15%" },
            { "data": "coverTypeName", "width": "15%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="text-center">
                            <a href="/Admin/Product/Upsert/${data}"
                               class="btn btn-success text-white"
                               style="cursor:pointer; width:80px">
                                <i class="fas fa-edit"></i> Edit
                            </a>
                            &nbsp;
                            <a onclick="Delete('/Admin/Product/Delete/${data}')"
                               class="btn btn-danger text-white"
                               style="cursor:pointer; width:80px">
                                <i class="fas fa-trash-alt"></i> Delete
                            </a>
                        </div>`;
                },
                "width": "15%"
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
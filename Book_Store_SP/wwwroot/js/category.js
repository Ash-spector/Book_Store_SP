var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/Category/GetAll"
        },
        "columns": [
            { "data": "name" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="text-center">
                            <a href="/Admin/Category/Upsert/${data}" 
                               class="btn btn-success text-white">
                                Edit
                            </a>
                            <a onclick="Delete('/Admin/Category/Delete/${data}')" 
                               class="btn btn-danger text-white">
                                Delete
                            </a>
                        </div>`;
                }
            }
        ]
    });
}
//const { Toast } = require("../lib/bootstrap/dist/js/bootstrap.bundle");

var dataTable;

$(document).ready(function () {
    loadDataTable();
})

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
                "url":"/Admin/Product/GetAll"
        },
            "columns": [
                { "data": "title", "width": "15%" },
                { "data": "isbn", "width": "15%" },
                { "data": "price", "width": "15%" },
                { "data": "author", "width": "15%" },
                { "data": "category.name", "width": "15%" },
                {
                    "data": "id",
                    "render": function (data) {
                        return `
                             <div class="w-75 btn-group" role="group">
                        <a href="/Admin/Product/Upsert?id=${data}">Edit</a>
                    </div>
                     <div class="w-75 btn-group" role="group">
                        <a onClick=Delete('Product/Delete/${data}')>Delete</a>
                    </div>
                            `
                    },
                    "width": "15%"
                },
            ]
    });
}

function Delete(url) {
    Swal.fire({
        title: 'Êtes vous sûr de vouloir supprimer cet élément ?',
        text: "Cette action est irréversible!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Oui, supprimer.'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    if (data.success) {
                        dataTable.ajax.reload();
                        toastr.success(data.message);
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            })
        }
    })
}
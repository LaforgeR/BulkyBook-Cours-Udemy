//const { Toast } = require("../lib/bootstrap/dist/js/bootstrap.bundle");

var dataTable;

$(document).ready(function () {
    loadDataTable();
})

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
                "url":"/Admin/Company/GetAll"
        },
            "columns": [
                { "data": "name", "width": "15%" },
                { "data": "streetAdress", "width": "15%" },
                { "data": "city", "width": "15%" },
                { "data": "state", "width": "15%" },
                { "data": "phoneNumber", "width": "15%" },
                {
                    "data": "id",
                    "render": function (data) {
                        return `
                             <div class="w-75 btn-group" role="group">
                        <a href="/Admin/Company/Upsert?id=${data}">Edit</a>
                    </div>
                     <div class="w-75 btn-group" role="group">
                        <a onClick=Delete('Company/Delete/${data}')>Delete</a>
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
$(document).ready(function () {

        //$("#example1").DataTable({
        //    "responsive": true, "lengthChange": false, "autoWidth": false,
        //    "buttons": ["copy", "csv", "excel", "pdf", "print", "colvis"]
        //}).buttons().container().appendTo('#example1_wrapper .col-md-6:eq(0)');
    loadTableSetting('#tblBorrow');
    $('#statusSearchSelect').select2()
    $('#reservationdate').datetimepicker({
        format: 'L'
    });

    $(document).on("click",".btn-pickup", function(){
        var id = $(this).data("id");
        var status = $("#statusSearchSelect").val();
        var borrowDate = $("#borrowDateSearch").val();
        var model = getSearchModel(status, borrowDate);

        $.ajax({
            url: '/Management/Pickup',
            type: 'POST',
            //async:false,
            data: { borrowId: id, filterModel: model },
            //contentType: 'application/json',
            success: function (data) {
                var tbl = $('#tblBorrow');
                tbl.empty();
                tbl.append(data.view);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert(textStatus);
            }
        });
    });

    $(document).on("click", ".btn-return", function () {
        var id = $(this).data("id");
        var status = $("#statusSearchSelect").val();
        var borrowDate = $("#borrowDateSearch").val();
        var model = getSearchModel(status, borrowDate);

        $.ajax({
            url: '/Management/Return',
            type: 'POST',
            //async:false,
            data: { borrowId: id, filterModel: model },
            //contentType: 'application/json',
            success: function (data) {
                var tbl = $('#tblBorrow > tbody');
                tbl.empty();
                tbl.append(data.view);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert(textStatus);
            }
        });
    });
    $(document).on("click", ".btn-wait-for-pickup", function () {
        var id = $(this).data("id");
        var status = $("#statusSearchSelect").val();
        var borrowDate = $("#borrowDateSearch").val();
        var model = getSearchModel(status, borrowDate);

        $.ajax({
            url: '/Management/WaitForPickup',
            type: 'POST',
            //async:false,
            data: { borrowId: id, filterModel: model },
            //contentType: 'application/json',
            success: function (data) {
                var tbl = $('#tblBorrow > tbody');
                tbl.empty();
                tbl.append(data.view);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert(textStatus);
            }
        });
    });
    $(document).on("click", ".btn-wait-for-ship", function () {
        var id = $(this).data("id");
        var status = $("#statusSearchSelect").val();
        var borrowDate = $("#borrowDateSearch").val();
        var model = getSearchModel(status, borrowDate);

        $.ajax({
            url: '/Management/WaitForShip',
            type: 'POST',
            //async:false,
            data: { borrowId: id, filterModel: model },
            //contentType: 'application/json',
            success: function (data) {
                var tbl = $('#tblBorrow > tbody');
                tbl.empty();
                tbl.append(data.view);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert(textStatus);
            }
        });
    });
    $(document).on("click", ".btn-cancel", function () {
        var id = $(this).data("id");
        var status = $("#statusSearchSelect").val();
        var borrowDate = $("#borrowDateSearch").val();
        var model = getSearchModel(status, borrowDate);

        $.ajax({
            url: '/Management/Cancel',
            type: 'POST',
            //async:false,
            data: { borrowId: id, filterModel: model },
            //contentType: 'application/json',
            success: function (data) {
                var tbl = $('#tblBorrow > tbody');
                tbl.empty();
                tbl.append(data.view);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert(textStatus);
            }
        });
    });

    $(document).on("click", "#btn-search-borrow", function () {
        var status = $("#statusSearchSelect").val();
        var borrowDate = $("#borrowDateSearch").val();
        var model = getSearchModel(status, borrowDate);
        $.ajax({
            url: '/Management/Index',
            type: 'POST',
            //async:false,
            data: { filterModel: model},
            //contentType: 'application/json',
            success: function (data) {
                var tbl = $('#tblBorrow > tbody');
                tbl.empty();
                tbl.append(data.view);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert(textStatus);
            }
        });

    });
})

function getSearchModel(status, date) {
    //var sortColumn = $("#current-sort-column");
    //var sortDir = $("#current-sort-dir");
    var pageSize = $("#page-size").val();
    var currentIndex = $("#current-index").val();

    var SearchModel = { PageIndex: currentIndex, PageSize: pageSize };

    var FilterModel = { BorrowDate: date, Status: status, SearchModel: SearchModel }

    return FilterModel;
}

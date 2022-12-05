$(document).ready(function () {

    $(document).on("click", ".btn-add-to-cart", function () {
        var id = $(this).data("book-id");
        $.ajax({
            url: '/Cart/AddToCartAjax',
            type: 'POST',
            //async:false,
            data: {'bookId':id},
            //contentType: 'application/json',
            success: function (data) {
                toastr["success"](data.message);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert(textStatus);
            }
        });

    });

    $(document).on("click", ".view-all-cart", function () {
        var id = $(this).data("cart-id");
        $.ajax({
            url: '/Cart/Index',
            type: 'GET',
            //async:false,
            //data: { 'bookId': id },
            //contentType: 'application/json',
            success: function (data) {
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert(textStatus);
            }
        });

    });

    $(document).on("click", ".btn-checkout", function () {
        var id = $(this).data("cart-id");
        $.ajax({
            url: '/Cart/Checkout',
            type: 'POST',
            //async:false,
            data: { 'cartId': id },
            //contentType: 'application/json',
            success: function (data) {
                toastr["success"](data.Message);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert(textStatus);
            }
        });

    });

})
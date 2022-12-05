var loginValid;
var token;
var timerId;
var hubUrl;

$(document).ready(function () {
    loadCurrentCart(loginValid);
    loadOrderCount(loginValid);

    if (loginValid) {
        //if (typeof connection === undefined && $.connection.hub.state === $.signalR.connectionState.disconnected)
            stablishConnection();
    }
})

function loadCurrentCart(valid) {
    if (valid) {
        $.ajax({
            url: '/Cart/GetCurrentCartAjax',
            type: 'GET',
            contentType: 'application/json',
            success: function (data) {
                if (data != undefined) {
                    $("#current-cart-dropdown").empty();
                    $("#current-cart-dropdown").append(data.view);
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                //if(jqXHR)
                //    alert(textStatus);
            }

        });
    }
}

function loadOrderCount(valid) {
    if (valid) {
        $.ajax({
            url: '/Borrow/GetCurrentBorrowCount',
            type: 'GET',
            contentType: 'application/json',
            success: function (data) {
                if (data != undefined) {
                    $("#current-order").text(data.count);
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                //if(jqXHR)
                alert(textStatus);
            }
        });
    }
}

function stablishConnection() {
    var connection = new signalR.HubConnectionBuilder()
    .withUrl(hubUrl, {
        accessTokenFactory: () => {
            return "Authorization", token;
        },
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets
    })
    .withAutomaticReconnect()
    .build();

    connection.on("UpdatedBorrowState", (message) => {
        alert(message.borrowId);
        toastr.success('Updated to status: ' + message.status, 'Borrow Id: ' + message.borrowId);
        if (window.location.pathname.split("/").pop() === 'Borrow') {
            refreshOrderList();
        }
    });

    connection.onclose(async () => {
        await start();
    });

    async function start() {
        try {
            await connection.start();
            console.log("SignalR Connected.");
        } catch (err) {
            console.log(err);
            //setTimeout(start, 5000);
        }
    }

    start();
}

function refreshOrderList() {
    clearTimeout(timerId);
    timerId = setTimeout(function () {
        window.location.reload();
    }, 1000);
}

function loadTableSetting(id) {
    $(id).DataTable({
        "paging": false,
        "lengthChange": false,
        "searching": false,
        "ordering": false,
        "info": true,
        "autoWidth": false,
        "responsive": true,
    });
}




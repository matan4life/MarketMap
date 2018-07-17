$(".remove-outlet").click(function () {
    var id = $(this).attr("data-outletid");
    $.ajax({
        method: "get",
        url: "/api/remove-outlet?id=" + id,
        success: function () {
            $("tr.row[data-outletid=" + id + "]").remove();
        },
        failure: function () {
            console.log('failure');
        }
    });
});
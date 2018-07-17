$(function () {
    var timer;
    var interval = 1250;
    var $input = $('#category');

    $input.on('keyup', function () {
        $("#AddCategory").css("display", "none");
        clearTimeout(timer);
        timer = setTimeout(doneTyping, interval);
        $("#query-result-text").text("Searching category...");
    });

    $input.on('keydown', function () {
        clearTimeout(timer);
    });

    function doneTyping() {

        var token = $('input:hidden[name="__RequestVerificationToken"]').val();
        var mydata = { category: $("#category").val() };
        var dataWithAntiforgeryToken = $.extend(mydata, { '__RequestVerificationToken': token });

        var category = $("#category").val();
        console.log(category);
        $.ajax({
            type: "post",
            url: "/api/get-category-list",
            data: dataWithAntiforgeryToken,
            success: function (response) {
                $("#query-result > .table > tbody").empty();
                var json = JSON.parse(JSON.stringify(response));
                console.log(json);
                if (json.length == 0) {
                    $("#AddCategory").css("display", "block");
                }
                
                $("#query-result-text").text(helper(json.length));

                for (var i = 0; i < json.length; i++) {
                    var row = $("<tr></tr>");
                    var column1 = $("<td class=\"col-xs-2\"></td>");
                    var color = "rgba(" + json[i].r + "," + json[i].g + "," + json[i].b + "," + json[i].a + ")";
                    var span = $("<span class=\"label\"></span>").css("background", color).text(json[i].colorName);
                    column1.append(span);
                    var column2 = $("<td class=\"col-xs-10\"></td>");
                    var category = $("<span></span>").text(json[i].name);
                    column2.append(category);
                    row.append(column1).append(column2);
                    $("#query-result > .table > tbody").append(row);
                    console.log(json[i].name);
                }
            },
            failure: function (response) {
                console.log("failure");
            }
        });
    }

    function helper(count) {
        if (count == 0) return "No categories found. Would you like to add one?";
        else return "Found " + count + (count > 1 ? " categories:" : " category:");
    }
});
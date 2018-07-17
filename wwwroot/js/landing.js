(function () {
    $('.transformable').addClass('hidden');

    var $nav = $('.nav'), navHeight = $('.nav').outerHeight(true), check;
    $nav.parent().css('padding-top', navHeight);

    (check = function () {
        var scrollOffset = $(window).scrollTop();
        if (scrollOffset < navHeight)
            $nav.css('height', (navHeight - scrollOffset));
        else
            $nav.css('height', '105px');

        if (scrollOffset > (navHeight - 215)) {
            $nav.addClass('n-fade');
            $(".nav .box").removeClass("outline");
        }
        else {
            $nav.removeClass('n-fade');
            $(".nav .box").addClass("outline");
        }
    })();

    $(window).scroll(function () {
        check();
        $('.t-hidden').each(function () {
            var $el = $(this);
            if ($el.visible(true) && $el.hasClass('t-hidden')) {
                $el.removeClass('t-hidden');
            }
        });
    });
})();
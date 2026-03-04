$('.js-load-more').on('click', function () {

    $el = $(this);
    $el.html('Loading<span>.</span><span>.</span><span>.</span>');
    $el.addClass('load-more');
    setTimeout(onLoaded, 5000);

    function onLoaded() {
        $el.removeClass('load-more');
        $el.html('Load more');
        $el.blur();
    }
});

// $(document).ready(function() {
// 	$('.popup-gallery').magnificPopup({
// 		delegate: 'a',
// 		type: 'image',
// 		tLoading: 'Loading image #%curr%...',
// 		mainClass: 'mfp-img-mobile',
// 		gallery: {
// 			enabled: true,
// 			navigateByImgClick: true,
// 			preload: [0,1] // Will preload 0 - before current, and 1 after the current image
// 		},
// 		image: {
// 			tError: '<a href="%url%">The image #%curr%</a> could not be loaded.',
// 			titleSrc: function(item) {
// 				return item.el.attr('title') + '';
// 			}
// 		}
// 	});
// });

// Show the first tab by default
$('.tabs-stage div').removeClass('tab-inner');
$('.tabs-stage div:first').addClass('tab-inner');
$('.tabs-nav li:first').addClass('tab-active');

// Change tab class and display content
$('.tabs-nav a').on('click', function (event) {
    event.preventDefault();
    $('.tabs-nav li').removeClass('tab-active');
    $(this).parent().addClass('tab-active');

    // Remove tab-inner class from all and add only to the selected one
    $('.tabs-stage div').removeClass('tab-inner');
    $($(this).attr('href')).addClass('tab-inner');
});


$(document).ready(function () {





    $(".vertical-tab .each-tab").on("click", function (e) {
        e.preventDefault(); // Prevent default link behavior if <a>

        var dis = $(this),
            dataTarget = dis.data("target");

        // Activate clicked tab
        dis.addClass("active").siblings(".each-tab").removeClass("active");

        // Show target content by adding 'active', hide others by removing 'active'
        $(".vertical-tab-content").removeClass("active");
        $(dataTarget).addClass("active");
    });


    // Event listener for when any <em> element is clicked within the rows
    $(".ans-table").on("click", "td span em:not(.ans-select)", function () {
        // Get the clicked <em> text
        var selectedText = $(this).text();

        // Find the parent row for the clicked <em>
        var row = $(this).closest("tr");

        // Remove 'selected' class from all <em> elements in the current row
        row.find("td span em").removeClass("selected");

        // Add 'selected' class to the clicked <em> element
        $(this).addClass("selected");

        // Update the ans-select <em> text in the same row
        row.find(".ans-select").text(selectedText);
    });
});

$(window).ready(function () {
    $(".accordion-list>li>a.toggle").on("click", function () {
        if ($(this).hasClass("active")) {
            $(this).removeClass("active");
            $(this)
                .siblings(".inner")
                .slideUp(200);
        } else {
            $(".accordion-list>li>a.toggle").removeClass("active");
            $(this).addClass("active");
            $(".inner").slideUp(200);
            $(this)
                .siblings(".inner")
                .slideDown(200);
        }
    });
});


$(document).ready(function () {
    $('.show-list').on('change', function () {
        var selectedCity = $(this).val().toLowerCase();

        if (selectedCity === 'all') {
            $('table.table tr[data-city]').show();
        } else {
            $('table.table tr[data-city]').each(function () {
                var city = $(this).data('city').toLowerCase();
                $(this).toggle(city === selectedCity);
            });
        }
    });
});
/* -------------------------------
  * Responsive Menu
---------------------------------- */
jQuery(document).ready(function () {
    'use strict';
    jQuery('ul.navigation').slicknav();
});

// mobile menu 

$(document).ready(function () {
    function addDropdownClassForMobile() {
        if ($(window).width() <= 768) { // Adjust the width as needed for your mobile breakpoint
            $('li').each(function () {
                if ($(this).find('.mega-menu.full-width').length) {
                    $(this).children('a').addClass('has-dropdown');
                }
            });
        } else {
            $('li a.has-dropdown').removeClass('has-dropdown'); // Optionally remove the class on larger screens
        }
    }

    // Run on page load
    addDropdownClassForMobile();

    // Run on window resize
    $(window).resize(function () {
        addDropdownClassForMobile();
    });
});


$(document).ready(function () {
    $('a.has-dropdown').on('click', function (e) {
        e.preventDefault(); // Prevent the default action of the link
        var $megaMenu = $(this).siblings('.mega-menu');

        if ($megaMenu.hasClass('show-dropdown')) {
            $megaMenu.removeClass('show-dropdown');
        } else {
            $('.mega-menu').removeClass('show-dropdown'); // Optionally close other open dropdowns
            $megaMenu.addClass('show-dropdown');
        }
    });

    // Close dropdown when clicking outside, but allow clicks within the mega menu
    $(document).on('click', function (e) {
        if (!$(e.target).closest('.mega-menu, a.has-dropdown').length) {
            $('.mega-menu').removeClass('show-dropdown');
        }
    });

    $('.close_btn').on('click', function () {
        $('.mega-menu.full-width').removeClass('show-dropdown');
    });
});

$(document).ready(function () {
    'use strict';

    // // Accordion Toggle
    // $(".accordion-list > li > a.toggle").on("click", function () {
    //   if ($(this).hasClass("active")) {
    //     $(this).removeClass("active").siblings(".inner").slideUp(200);
    //   } else {
    //     $(".accordion-list > li > a.toggle").removeClass("active");
    //     $(this).addClass("active").siblings(".inner").slideDown(200);
    //   }
    // });

    // Slick Slider Initialization
    if ($.fn.slick) {
        $(".news-slides").slick({
            slidesToShow: 4,
            slidesToScroll: 1,
            autoplay: true,
            autoplaySpeed: 2000,
            responsive: [
                {
                    breakpoint: 960,
                    settings: { slidesToShow: 2, slidesToScroll: 1 },
                },
            ],
        });

        $(".slide-view").on("init", function () {
            $(this).css({ visibility: "visible" });
        }).slick({
            infinite: true,
            autoplay: false,
            autoplaySpeed: 1000,
            slidesToShow: 5,
            slidesToScroll: 1,
            responsive: [
                {
                    breakpoint: 960,
                    settings: { slidesToShow: 2, slidesToScroll: 1 },
                },
            ],
        });

        //$(".main-slider").slick({
        //    infinite: true,
        //    autoplay: true,
        //    autoplaySpeed: 1000,
        //    slidesToShow: 1,
        //    slidesToScroll: 1,
        //    dots: false,
        //    lazyLoad: "ondemand",
        //});

        $(".review-slide").on("init", function () {
            $(this).css({ display: "block", visibility: "visible" });
        }).slick({
            infinite: true,
            autoplay: true,
            autoplaySpeed: 1000,
            slidesToShow: 1,
            slidesToScroll: 1,
        });
    }

    $(document).ready(function () {
        var $slider = $(".main-slider");

        // Slick initialize karo autoplay off aur arrows off ke sath
        $slider.slick({
            infinite: true,
            autoplay: false,   // autoplay off
            arrows: false,     // arrows off initially
            slidesToShow: 1,
            slidesToScroll: 1,
            dots: false,
        });

        var totalSlides = $slider.find("img").length;
        var loadedSlides = 0;
        var autoplayStarted = false;

        $slider.find("img").each(function () {
            if (this.complete) {
                loadedSlides++;
                checkAllLoaded();
            } else {
                $(this).on("load", function () {
                    loadedSlides++;
                    checkAllLoaded();
                });
            }
        });

        function checkAllLoaded() {
            if (!autoplayStarted && loadedSlides === totalSlides) {
                autoplayStarted = true;
                // Jab sab images load ho jayein → autoplay aur arrows enable
                $slider.slick("slickSetOption", "autoplay", true, true);
                $slider.slick("slickSetOption", "arrows", true, true);
                $slider.slick("slickPlay");
            }
        }
    });

    // Mobile Menu
    const toggleDropdownClass = () => {
        if ($(window).width() <= 768) {
            $("li").each(function () {
                if ($(this).find(".mega-menu.full-width").length) {
                    $(this).children("a").addClass("has-dropdown");
                }
            });
        } else {
            $("li a.has-dropdown").removeClass("has-dropdown");
        }
    };

    toggleDropdownClass();
    $(window).resize(toggleDropdownClass);

    $("a.has-dropdown").on("click", function (e) {
        e.preventDefault();
        const $megaMenu = $(this).siblings(".mega-menu");
        $(".mega-menu").removeClass("show-dropdown");
        $megaMenu.toggleClass("show-dropdown");
    });

    $(document).on("click", function (e) {
        if (!$(e.target).closest(".mega-menu, a.has-dropdown").length) {
            $(".mega-menu").removeClass("show-dropdown");
        }
    });

    $(".close_btn").on("click", function () {
        $(".mega-menu.full-width").removeClass("show-dropdown");
    });

    document.addEventListener("DOMContentLoaded", () => {
        const lazyImages = document.querySelectorAll(".lazy-load");
        const imageObserver = new IntersectionObserver((entries) => {
            entries.forEach((entry) => {
                if (entry.isIntersecting) {
                    const img = entry.target;
                    img.src = img.dataset.src;
                    img.onload = () => img.classList.add("loaded");
                    imageObserver.unobserve(img);
                }
            });
        });

        lazyImages.forEach((img) => imageObserver.observe(img));
    });

    // "Is this page helpful?" Toggle
    $("ul.yes-no li a").on("click", function () {
        $("ul.yes-no li a").removeClass("active");
        const boxId = $(this).data("box");
        $(this).addClass("active");
        $("div[id^='feedback-box']").hide();
        $(`#${boxId}`).show();
    });

    // Star Rating
    $("#stars li").on("mouseover", function () {
        const onStar = parseInt($(this).data("value"), 10);
        $(this).parent().children("li.star").each(function (index) {
            $(this).toggleClass("hover", index < onStar);
        });
    }).on("mouseout", function () {
        $(this).parent().children("li.star").removeClass("hover");
    }).on("click", function () {
        const onStar = parseInt($(this).data("value"), 10);
        $(this).siblings("li.star").removeClass("selected");
        $(this).prevAll().addBack().addClass("selected");
        const ratingValue = $("#stars li.selected").last().data("value");
        const msg = ratingValue > 1 ? `Thanks! You rated this ${ratingValue} stars.` : `We will improve ourselves. You rated this ${ratingValue} star.`;
        $(".success-box").fadeIn(200).find(".text-message").html(`<span>${msg}</span>`);
    });

    // Dynamic Year
    $("#year").text(new Date().getFullYear());

    // Delayed Content Loading
    setTimeout(() => $("#slider1").fadeIn(), 5000);
    setTimeout(() => $("#banner-container").fadeIn(), 7000);
});


setTimeout(function () {
    if (window.innerWidth > 768) {
        var gtagScript = document.createElement('script');
        gtagScript.async = true;
        gtagScript.src = 'https://www.googletagmanager.com/gtag/js?id=G-245RQY719C';
        document.head.appendChild(gtagScript);

        gtagScript.onload = function () {
            window.dataLayer = window.dataLayer || [];
            function gtag() { dataLayer.push(arguments); }
            gtag('js', new Date());
            gtag('config', 'G-245RQY719C');
        };
    }
}, 7000);
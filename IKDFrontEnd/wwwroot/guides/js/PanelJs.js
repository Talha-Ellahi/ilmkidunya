function DisplayPopUp(chk) {
    var k = $(chk).siblings("div.showpoppanel").html()
    $(".amsg").html(k);
    $("#divpanel2").fadeIn("slow");

}
function Closepanel() {
    $(".amsg").html("");
    $("#divpanel2").fadeOut("slow");
}

var hw = "height = 1024, width = 768";
function setWidth() {

    var h = window.screen.availHeight;
    var w = window.screen.availWidth
    hw = "height =" + h + ", width =" + w;

}
function LoadPrint(chk, Type) {
    var o = "";
    //1= pdf 0=img
    var a = $(chk).siblings("input").val();
    if (Type == 0) {
        o = $(chk).siblings("img").attr("src", a);
        $(o).load(function () {
            OpenWindowPop(chk, Type);

        });
    }
    else {
        window.open(a);
          }

    

    

    

}

function OpenWindowPop(chk, Type) {
    setWidth();

    var a = $(chk).siblings("input").val();
   
    var htm = "<html><body>";
    if (Type == 1) {
        htm += "<div style='clear:both'><embed  type='application/pdf' id='pdf' src='" + a + "' width='100%' height='100%' /></div>";
    }
    else {
        htm+= "<div style='clear:both'><img id='pdf' src='" + a + "' /></div><br />";
    }
    htm += "</body></html>";
    var mywindow = window.open('', 'PRINT', hw);
    mywindow.document.write(htm);
    mywindow.focus();
    mywindow.print();
}

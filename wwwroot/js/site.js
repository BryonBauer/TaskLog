$(function() {
    alert("OK!");
    var showMenu = false;

    $("#menuIcon").click(function () { 
        if(!showMenu){
            $("#sideMenu").toggle();
            $(".card-container").css("margin-left", "15%");
            showMenu = true;
        }
        else{
            $("#sideMenu").toggle();
            $(".card-container").css("margin-left", "0%");
            showMenu = false;
        }
    });
});
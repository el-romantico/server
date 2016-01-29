$(function () {
    var hub = $.connection.ritualsHub;
    // Create a function that the hub can call back to display messages.
    hub.client.checkConnection = function () {
        alert("Connection check successful");
    };
    hub.client.connectedCount = function(count) { 
        $("#active-connections").val(count);
    }
    hub.client.successfulCount = function (count) {
        $('#successful-players').val(count);
    }
    
    $.connection.hub.start().done(function () {
        $('#test').click(function () {
            hub.server.checkConnection();
            console.log("Said hello");
        });
        $('#start-game').click(function () {
            hub.server.startGame();
            alert("Game started");
        })
    });
});
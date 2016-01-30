$(function () {
    var hub = $.connection.ritualsHub;
    // Create a function that the hub can call back to display messages.
    hub.client.checkConnection = function () {
        alert("Connection check successful");
    };
    hub.client.connectedCount = function(count) { 
        $("#active-connections").text(count);
    }
    hub.client.successfulCount = function (count) {
        $('#successful-players').text(count);
    }
    hub.client.endGame = function (win) {
        if(win) {
            append("You win!");
        } else {
            append("You lose!");
        }
    }
    hub.client.updateCountdown = function (countdown) {
        append(countdown);
    }
    
    $.connection.hub.start().done(function () {
        $('#test').click(function () {
            hub.server.checkConnection();
        });
        $('#start-game').click(function () {
            hub.server.startGame();
            append("Game started");
        });
        $('#connect').click(function () {
            hub.server.connect();
            append("Connected");
        });
        $('#disconnect-all').click(function () {
            hub.server.disconnectAll();
            append("Dropped All players");
        });
        $("#success").click(function () {
            hub.server.success();
            append("Player succeeded");
        });
    });
});

function append(text) {
    $("#output").text($("#output").text() + text + '\n');
}
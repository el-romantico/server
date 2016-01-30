var secondsConst = 15;
var interval = null;
var totalSeconds = secondsConst;
$(function () {
    var hub = $.connection.ritualsHub;
    
    hub.client.checkConnection = function () {
        alert('Connection check successful');
    };
    hub.client.connectedCount = function (count) {
        $('#active-connections').text(count);
    };
    hub.client.successfulCount = function (count) {
        $('#successful-players').text(count);
    };
    hub.client.endGame = function (win) {
        clearInterval(interval);
        win ? append('You win!') : append('You lose!');
    };
    hub.client.nextGame = function (playersCount, gesture) {
        initiateGame(playersCount);
    };
    hub.client.startGame = function (playersCount, gesture) {
        initiateGame(playersCount);
    };
    
    function initiateGame(playersCount) {
        append('Initiating game with:' + playersCount + ' players');
        totalSeconds = secondsConst;
        clearInterval(interval);
        interval = setInterval(setTime, 1000);

        function setTime() {
            --totalSeconds;
            if (totalSeconds <= 0) {
                hub.server.timeoutExpired();
                clearInterval(interval);
                totalSeconds = secondsConst;
            }
            $('#timer').text(totalSeconds);
        }
    }

    $.connection.hub.start().done(function () {
        $('#test').click(function () {
            hub.server.checkConnection();
        });
        $('#start-game').click(function () {
            hub.server.startGame();
            append('Game started');
        });
        $('#connect').click(function () {
            hub.server.connect();
            append('Connected');
        });
        $('#disconnect-all').click(function () {
            hub.server.disconnectAll();
            clearInterval(interval);
            totalSeconds = secondsConst;
            $('#timer').text(totalSeconds);
            append('Dropped All players');
        });
        $('#success').click(function () {
            hub.server.success();
            append('Player succeeded');
        });
        $('#refresh-connection').click(function () {
            hub.server.updateUI();
        });
        hub.server.updateUI();
        $('#timer').text(secondsConst);
    });
});

function append(text) {
    $('#output').text($('#output').text() + text + '\n');
}
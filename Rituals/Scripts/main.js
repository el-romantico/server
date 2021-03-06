﻿var secondsConst = 30;
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
        totalSeconds = secondsConst;
        $('#timer').text(totalSeconds);
        win ? append('You win!') : append('You lose!');
    };
    hub.client.nextGame = function (playersCount, gestureId) {
        initiateGame(playersCount, gestureId);
    };
    hub.client.startGame = function (playersCount, gestureId) {
        initiateGame(playersCount, gestureId);
    };

    $('#debug-mode').click(function () {
        $('#debug-tools').toggleClass('hidden');
    });

    $('#clear-output').click(function () {
        $('#output').text('');
    });

    function initiateGame(playersCount, gestureId) {
        append('Initiating game with:' + playersCount + ' players');
        $('.gesture').addClass('hidden');
        $('#gesture-' + gestureId).removeClass('hidden');
        totalSeconds = secondsConst;
        clearInterval(interval);
        interval = setInterval(setTime, 1000);

        function setTime() {
            --totalSeconds;
            $('#timer').text(totalSeconds);
            if (totalSeconds <= 0) {
                hub.server.timeoutExpired();
                clearInterval(interval);
                totalSeconds = secondsConst;
            }
        }
    }

    $.connection.hub.start().done(function () {
        $('#start-game-admin').click(function () {
            hub.server.startGame(true);
            append('Game started as admin');
        })
        $('#test').click(function () {
            hub.server.checkConnection();
        });
        $('#start-game').click(function () {
            hub.server.startGame(false);
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
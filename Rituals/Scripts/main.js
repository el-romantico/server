$(function () {
    var chat = $.connection.ritualsHub;
    // Create a function that the hub can call back to display messages.
    chat.client.hello = function () {
        alert("kur");
    };
    // Get the user name and store it to prepend to messages.
    //$('#displayname').val(prompt('Enter your name:', ''));
    // Set initial focus to message input box.
    //$('#message').focus();
    // Start the connection.
    $.connection.hub.start().done(function () {
        $('#test').click(function () {
            chat.server.hello();
            console.log("asd");
        });
    });
});
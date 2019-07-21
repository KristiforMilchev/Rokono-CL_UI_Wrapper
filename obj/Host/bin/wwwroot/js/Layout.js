var selectedEditor;

$(document).ready(function() {
    const reply = ipcRenderer.send("async-LoadSavedLogins");
    var host = new ej.inputs.TextBox({
        placeholder: 'Host',
        floatLabelType: 'Auto'
    });
    host.appendTo('#host');
    var username = new ej.inputs.TextBox({
        placeholder: 'Username',
        floatLabelType: 'Auto'
    });
    username.appendTo('#username');
    var password = new ej.inputs.TextBox({
        placeholder: 'Password',
        floatLabelType: 'Auto'
    });
    password.appendTo('#password');
    button = new ej.splitbuttons.ProgressButton({
        content: 'Spin Right',
        spinSettings: { position: 'Right' },
        isPrimary: true
    });
    button.appendTo('#BtnConnect');


});




ipcRenderer.on('LoginUsersRecived', (event, arg) => {
    var cData = JSON.parse(arg);
    console.log(cData);
    if (!cData) {
        cData = [];
    }
    // Initialize ListBox component.
    var grid = new ej.grids.Grid({
        dataSource: cData,
        recordDoubleClick: RecordDoubleClicked,
        columns: [
            { field: 'text', headerText: 'Connection', width: 120 },
        ]
    });
    grid.appendTo('#Grid');

});

ipcRenderer.on('DatabaseConnected', (event, arg, databasename) => {
    var cData = JSON.parse(arg);
    console.log(cData);
    $("#ConnectionSelect").append($('<option>', {
        value: arg,
        text: databasename
    }));
});

function RecordDoubleClicked(args) {
    console.log(args);
    console.log(args.rowData.ConnectionId);
    window.location.href = "/Dashboard/Index?id=" + args.rowData.ConnectionId;
}

function LoginAttempt() {
    var checbox = $('#rememberMe').is(":checked")
    console.log(checbox);
    var dto = {
        "Host": $("#host").val(),
        "Username": $("#username").val(),
        "Password": $("#password").val(),
        "Remember": checbox
    }

    const reply = ipcRenderer.send("async-LoginAttempt", JSON.stringify(dto));
    console.log(reply);
}



function TabChanged(id) {
    var editor = $("#editor" + id);
    selectedEditor = editor;
    console.log(editor[0].textContent);
}
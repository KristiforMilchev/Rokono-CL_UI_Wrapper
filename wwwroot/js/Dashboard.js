const { ipcRenderer } = require("electron");
require('events').EventEmitter.defaultMaxListeners = 25

var connectionIdentifier;
var dbName;
var selectedPath;


$(document).ready(function() {

    alert("Its Working");
    // Initialize the uploader component
    var uploadObj = new ej.inputs.Uploader({
        selected: FileSelected
    });
    uploadObj.appendTo('#fileupload');
    var progressButton = new ej.splitbuttons.ProgressButton({
        content: 'Generate Plant UML Schema',
        enableProgress: true,
        animationSettings: { effect: 'SlideRight' },
        spinSettings: { position: 'Center' },
        cssClass: 'e-outline e-success'
    });
    progressButton.appendTo('#btnGenerate');

});


function GenerateSchema(args, id) {
    console.log(args);
    console.log(id);
    connectionIdentifier = id;
    dbName = args;
    var bidnignData = {
        "DatabaseName": args,
        "ConnectionId": id
    }

    ipcRenderer.send("async-LoadSchemaDefaultPath", bidnignData);
}

function FileSelected(args) {
    console.log(args.filesData[0].rawFile.path);
    selectedPath = args.filesData[0].rawFile.path;
}

function GenerateFile() {

    var bindingData = {
        "ConnectionId": connectionIdentifier,
        "DatabaseName": dbName,
        "FilePath": selectedPath
    }
    console.log(bindingData);
    ipcRenderer.send("async-GenerateSchemaSavePath", bindingData);

}

ipcRenderer.on('DatabaseBoundFile', (event, arg) => {
    $('#GenerateModal').modal('toggle');
    //  $("#selectedFile").val(arg);
});


ipcRenderer.on('DatabasePathNull', (event) => {
    // $('#GenerateModal').modal('toggle');
});

ipcRenderer.on('SchemaGenerated', (event, data) => {
    console.log(data);
    $("#GeneratedSchema").val(data);
});
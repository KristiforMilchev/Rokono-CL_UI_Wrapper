const { ipcRenderer } = require("electron");


document.getElementById("sync-msg").addEventListener("click", () => {
    const reply = ipcRenderer.send("async-msg", 'RokonoControl')

});


document.getElementById("async-MenuItem").addEventListener("click", () => {
    const reply = ipcRenderer.send("async-Menu", 'RokonoControl')

});



ipcRenderer.on('MainMenuData', (event, arg) => {
    var cData = JSON.parse(arg);
    console.log(cData);
    $('#tree').treeview({ data: cData, levels: 5 });
    $('#tree').treeview('collapseAll', { silent: true });
    $('#tree').on('nodeSelected', function(event, data) {
        console.log(data);
        var cNode = $('#tree').treeview('getNode', data.nodeId);
        if (cNode.state.expanded === true)
            $('#tree').treeview('collapseNode', [data.nodeId, { silent: true, ignoreChildren: false }]);
        else
            $('#tree').treeview('expandNode', [data.nodeId, { levels: 1, silent: true }]);

    });

});


ipcRenderer.on('asynchronous-reply', (event, arg) => {
    var data = JSON.parse(arg);
    // console.log(data);
    var bindingTables = [];
    data.Tables.forEach(function(element) {
        //console.log(element);

        var bTableNode = {

                id: element.Id,
                shape: {
                    type: "UmlClassifier",
                    class: {
                        name: element.Id,
                            attributes: AddProperties(element.Shape.Attribute),
                            methods: []
                    },
                    classifier: "Class"
                },
                offsetX: element.OffsetX,
                offsetY: element.OffsetY


            }
            //    createNode(element.Id, element.OffsetX, element.OffsetY, element.Id)

        bindingTables.push(bTableNode);
    });
    // console.log(bindingTables);
    var bindingConnections = [];
    var i = 1;
    console.log(bindingTables);
    data.Connections.forEach(function(element) {
        bindingConnections.push(createConnector("connect" + i, element.TableName, element.ConnectionName));
        i = i + 1;

    });
    InitDiagram(bindingTables, bindingConnections);
});

function InitDiagram(bindingTables, bindingConnections) {
    diagram = new ej.diagrams.Diagram({
        width: "100%",
        height: 900,
        nodes: bindingTables,
        connectors: bindingConnections,
        getNodeDefaults: getNodeDefaults,
        getConnectorDefaults: getConnectorDefaults,
        setNodeTemplate: setNodeTemplate
    });
    diagram.appendTo("#diagram");
}

function AddProperties(attributes) {
    var res = [];
    attributes.forEach(function(element) {
        //   console.log(element);
        var current = createProperty(element.Name, element.Column);
        res.push(current);
    });
    return res;
}
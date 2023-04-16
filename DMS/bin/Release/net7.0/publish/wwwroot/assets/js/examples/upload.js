
function initupload() {

  //  initCats();

    var jsonData = loadCategories(categories, false, true);
    
    $("#searchForm").hide();
    $("#filesDiv").hide();
}

function cancelupload() {
    loadCategories(categories, false, false);
   // initCats();
    categoryChanged(selectedCatID);

    $("#searchForm").show();
    $("#filesDiv").show();
}

var isEnterprise;
var permissons;

function toolbarPreparingContacts(e) {

    if (isEnterprise) {
        if (!permissions.includes('Can Add Contacts')) {
            e.toolbarOptions.items.shift();
        }
    }

}

function toolbarPreparingSearchKeys(e) {

    if (isEnterprise) {
        if (!permissions.includes('Can Add Search Keys')) {
            e.toolbarOptions.items.shift();
        }
    }
}

$(document).ready(function () {

    var xmlHttp = new XMLHttpRequest();
    xmlHttp.open("GET", "api/Enterprise/IsEnterprise", false);
    xmlHttp.send();

    isEnterprise = JSON.parse(xmlHttp.responseText);


    if (isEnterprise) {
        // Get user permissions

        var xmlHttp = new XMLHttpRequest();
        xmlHttp.open("GET", "api/Permissions/UserPermissions", false);
        xmlHttp.send();

        var result = JSON.parse(xmlHttp.responseText);

        permissions = result.map(a => a.Name);

    }

    $("#contactsList").dxDataGrid({
        editing: {
            mode: "row",
            allowUpdating: function (e) {
                return isEnterprise == false || permissions.includes('Can Edit Contacts');
            },
            allowDeleting: function (e) {
                return isEnterprise == false || permissions.includes('Can Edit Contacts');
            },
            allowAdding: function (e) {
                return isEnterprise == false || permissions.includes('Can Add Contacts');
            },
            useIcons: true
        }
    });

    $("#searchKeysList").dxDataGrid({
        editing: {
            mode: "row",
            allowUpdating: function (e) {
                return isEnterprise == false || permissions.includes('Can Edit Search Keys');
            },
            allowDeleting: function (e) {
                return isEnterprise == false || permissions.includes('Can Edit Search Keys');
            },
            allowAdding: function (e) {
                return isEnterprise == false || permissions.includes('Can Add Search Keys');
            },
            useIcons: true
        }
    });
});

function onBeforeUpload(args) {
    selectedCategories = $('#files').jstree("get_selected");

    var uploadObj = document.getElementById("uploadFiles").ej2_instances[0];
    var files = uploadObj.filesData;
    filesCount = files.length;
    filesUploaded = 0;

    if (selectedCategories == null || selectedCategories == '') {
        swal("Error!", "You have to choose a category!", "error");
        args.cancel = true;
        return;
    }

    contacts = $('#contactsList').dxDataGrid('instance').getSelectedRowsData();
    keys = $('#searchKeysList').dxDataGrid('instance').getSelectedRowsData();

    var allSize = 0;
    for (var i = 0; i < files.length; i++) {
        var fileSizeMB = files[i].size / 1048576.0;

        allSize = allSize + fileSizeMB;
    }


    var xmlHttp = new XMLHttpRequest();
    xmlHttp.open("GET", "api/authorization/getuserstorage", false);
    xmlHttp.send();
    var d = xmlHttp.responseText;
    var storage = JSON.parse(d);

    if (storage.Storage != -1 && (storage.UsedStorage + allSize) > storage.Storage) {
        swal("Error!", "You don't have enough space!", "error");
        args.cancel = true;
        return;
    }



}
function onFileUpload(args) {

    args.customFormData = [
        {
            'categories': JSON.stringify(selectedCategories)
        },
        {
            'contacts': JSON.stringify(contacts)
        },
        {
            'keys': JSON.stringify(keys)
        },
        {
            'note': $('#txtNote').text()
        }
    ];

}

var filesUploaded = 0;
var filesCount = 0;

function onUploadSuccess(args) {
    if (args.operation === 'upload') {
        filesUploaded = filesUploaded + 1;

        if (filesUploaded == filesCount) {
            swal("Success!", "File has been successfully uploaded!", "success");

            $('#uploadDetailsCloseBtn').click();

            $('#contactsList').dxDataGrid('instance').deselectAll();
            $('#searchKeysList').dxDataGrid('instance').deselectAll();
            $('#txtNote').val('');
            document.getElementById("uploadFiles").ej2_instances[0].clearButton.click();
        }
    }
}


function startUpload() {
    var uploadObj = document.getElementById("uploadFiles").ej2_instances[0];
    uploadObj.upload(uploadObj.getFilesData());

}
var users;
var categories;
var IsEnterprise;

$(function () {

    var xmlHttp = new XMLHttpRequest();

    xmlHttp.open("GET", "api/Permissions/Users?includeSelf=true", false);
    xmlHttp.send();
    users = JSON.parse(xmlHttp.responseText);


    xmlHttp = new XMLHttpRequest();
    xmlHttp.open("GET", "api/Enterprise/IsEnterprise", false);
    xmlHttp.send();

    IsEnterprise = JSON.parse(xmlHttp.responseText);

    initCats();



});

function initCats() {


    var xmlHttp = new XMLHttpRequest();

    xmlHttp.open("GET", "api/Categories", false);
    xmlHttp.send();

    categories = JSON.parse(xmlHttp.responseText);

    loadCategories(categories);


    var table = $('#table-files').DataTable({
        destroy: true,
        'columnDefs': [
            {
                'targets': 0,
                'className': 'dt-body-center',
                'render': function (data, type, full, meta) {
                    return '<div class="custom-control custom-checkbox">' +
                        '<input type="checkbox" class="custom-control-input" id="customCheck' + meta.row + '">' +
                        '<label class="custom-control-label" for="customCheck' + meta.row + '"></label>' +
                        '</div>';
                }
            },
            {
                "orderable": false,
                "targets": [0, 5]
            }
        ]
    });


    $(document).on('click', '#files-select-all', function () {
        // Check/uncheck all checkboxes in the table
        var rows = table.rows({ 'search': 'applied' }).nodes();
        $('input[type="checkbox"]', rows)
            .prop('checked', this.checked);
        if (this.checked) {
            $('input[type="checkbox"]', rows).closest('tr').addClass('tr-selected');
            $('#file-actions').removeClass('d-none');
        } else {
            $('input[type="checkbox"]', rows).closest('tr').removeClass('tr-selected');
            if ($('#table-files .custom-control-input:checked').length == 0) {
                $('#file-actions').addClass('d-none');
            }
        }
    });

}
var selectedCatID;
function categoryChanged(catID) {
    var xmlHttp = new XMLHttpRequest();

    xmlHttp.open("GET", "api/Upload/GetDocuments?CatID=" + catID, false);
    xmlHttp.send();

    var res = JSON.parse(xmlHttp.responseText);
    selectedCatID = catID;

    loadFiles(res);
}

function loadCategories(res, isSearch = false, checkboxes = false) {

    var jsonData = [];;

    if (!isSearch) {
        for (var i = 0; i < res.data.length; i++) {
            var cat = res.data[i];
            var parent = cat.FatherAutoKey;

            if (parent == 0) {
                parent = "#";
            }
            jsonData.push(
                {
                    'id': cat.AutoKey,
                    'parent': parent,
                    'text': cat.Name,
                    'type': 'folder'
                }
            );
        }
    } else {
        for (var i = 0; i < res.length; i++) {
            var cat = res[i];

            jsonData.push(
                {
                    'id': cat.AutoKey,
                    'parent': "#",
                    'text': cat.Name,
                    'type': 'folder'
                }
            );
        }
    }

    $('#files').jstree("destroy").empty();

    var treeConfig = {
        "core": {
            themes: {
                dots: false
            },
            check_callback: true,
            data: jsonData
        },
        "types": {
            "folder": {
                "icon": "far fa-folder text-warning",
            },
            "file": {
                "icon": "ti-file",
            }
        },
        "contextmenu": {
            "select_node": false,
            "items": function (node) {
                return getContextMenu(node);
            }
        },
        plugins: ["types", "contextmenu", "dnd"]
    };

    if (checkboxes) {
        treeConfig.plugins.push("checkbox");
        treeConfig["checkbox"] = {
            "whole_node": false,
            "three_state": false
        };
    }


    $('#files').jstree(treeConfig);


    $('#files').on("changed.jstree", function (e, data) {
        categoryChanged(data.selected);
    });

    $('#files').on("select_node.jstree", function (e, data) {
        $('#files').jstree("toggle_node", data.node);
    });

    $('#files').on("move_node.jstree", function (e, data) {
        
        var node = data.node;
        var allCats = Object.values($("#files").jstree()._model.data).filter(x => x.parent == node.parent);

        if (allCats.filter(x => x.text == node.text && x.id != node.id).length > 0) {
            DevExpress.ui.notify({ message: "That category already exists!", width: 320 }, "error", 2000);
            $('#files').jstree("move_node", node.id, data.old_parent, data.old_position);
            return;
        }

        const xhr = new XMLHttpRequest();

        xhr.open("POST", 'api/Categories/SetParent', true);

        const fd = new FormData();

        fd.append('AutoKey', data.node.id);
        fd.append('FatherAutoKey', data.parent);
        xhr.send(fd);
    });
    return jsonData;


}


function getContextMenu(node) {

    var tmp = $.jstree.defaults.contextmenu.items();
    delete tmp.ccp;
    tmp.create.label = "New";
    tmp.create.action = function (data) {
        var inst = $.jstree.reference(data.reference);
        var obj = inst.get_node(data.reference);
        var allCats = Object.values($("#files").jstree()._model.data).filter(x => x.parent == node.id);
        var duplicatesAmount = allCats.filter(x => x.text.includes("New folder")).length;
        var newName;
        if (duplicatesAmount > 0) {
            newName = "New folder (" + duplicatesAmount + ")"

        } else {
            newName = "New folder";

        }

        inst.create_node(obj, { type: "folder", text: newName }, "last", function (new_node) {


            setTimeout(function () {
                inst.edit(new_node, null, function (node, status) {
                    if (!status) {
                        inst.delete_node(node);
                        return;
                    }
                    newName = node.text;


                    if (allCats.filter(x => x.text == node.text && x.id != node.id).length > 0) {
                        DevExpress.ui.notify({ message: "That category already exists!", width: 320 }, "error", 2000);
                        inst.rename_node(node, node.original.text);
                        newName = node.original.text;
                    }

                    $.ajax({
                        url: "api/Categories/AddCategory",
                        method: "POST",
                        data: { Name: newName, FatherAutoKey: new_node.parent },
                        success: function (data) {
                            console.log(new_node);
                            categories.data.push({ AutoKey: data.Extra, FatherAutoKey: new_node.parent, Name: newName });
                            inst.set_id(new_node, data.Extra);


                        }
                    })

                });
            }, 0);
        });
    }

    tmp.rename.action = function (data) {
        var inst = $.jstree.reference(data.reference);
        var obj = inst.get_node(data.reference);

        inst.edit(obj, null, function (node, status) {
            var allCats = Object.values($("#files").jstree()._model.data).filter(x => x.parent == node.parent);

            if (allCats.filter(x => x.text == node.text && x.id != node.id).length > 0) {
                DevExpress.ui.notify({ message: "That category already exists!", width: 320 }, "error", 2000);
                inst.rename_node(node, node.original.text);

                return;
            }

            renameCat(node, node.text);
        });
    }

    tmp.remove.action = function (data) {
        var inst = $.jstree.reference(data.reference);
        var obj = inst.get_node(data.reference);
        swal({
            title: "Are you sure?",
            text: "Do you want to delete this category?",
            icon: "warning",
            buttons: true,
            dangerMode: true,
        })
            .then((willDelete) => {
                if (willDelete) {
                    // Delete category
                    var id = obj.id;


                    var xmlHttp = new XMLHttpRequest();
                    xmlHttp.open("GET", "api/Categories/AllowDeleteCategory?AutoKey=" + id, false);
                    xmlHttp.send();

                    var allow = JSON.parse(xmlHttp.responseText);

                    if (allow === true) {
                        $.ajax({
                            url: "api/Categories/DeleteCategory",
                            method: "POST",
                            data: { autokey: id }
                        })

                        inst.delete_node(obj);
                        swal("Category has been successfully deleted!", {
                            icon: "success",
                        });

                    } else {
                        DevExpress.ui.notify({ message: "You can't delete this category! It has files and subcategories, delete them first. ", width: 320 }, "error", 2000);
                        return false;
                    }
                }
            });
    }


    if (IsEnterprise) {
        var parentcategory = categories.data.filter(x => x.AutoKey == node.id)[0];
        if (!parentcategory.CanAdd) {
            delete tmp.create;
        }

        if (!parentcategory.CanDelete) {
            delete tmp.remove;
        }

        if (!parentcategory.CanEdit) {
            delete tmp.rename;
        }
    }
    return tmp;
}

function renameCat(node, text) {
    const xhr = new XMLHttpRequest();


    xhr.open("POST", 'api/Categories/UpdateCategory', true);

    const fd = new FormData();

    fd.append('key', node.id);
    fd.append('values', JSON.stringify({ 'Name': text }));
    xhr.send(fd);
}


function loadFiles(res) {

    var jsonData = [];

    if (res != null && res.length > 0) {
        var c = categories.data.find(x => x.AutoKey == selectedCatID);
        if (c == null)
            c = categories.data.find(x => x.AutoKey == res[0].CatAutoKey);
    }
    for (var i = 0; i < res.length; i++) {
        var f = res[i];

        if (f.Name == null)
            continue;
        var dropdown = `<div class="dropdown">
                                        <a href="#" class="btn btn-floating" data-toggle="dropdown">
                                            <i class="ti-more-alt"></i>
                                        </a>
                                        <div class="dropdown-menu dropdown-menu-right">`
        if (!IsEnterprise || c.CanView) {
            dropdown += `<a onclick="javascript:onFileView(` + f.DocumentAutoKey + `, ` + f.LineAutoKey + `);" class="dropdown-item">View</a>`
        }

        if (!IsEnterprise || c.CanDownload) {
            dropdown += `<a onclick="javascript:onFileDownload(` + f.DocumentAutoKey + `, ` + f.LineAutoKey + `, '` + f.Ext + `', '` + f.Name + `');" class="dropdown-item">Download</a>`
        }

        if (!IsEnterprise || c.CanEdit) {
            dropdown += `<a onclick="javascript:onFileEdit(` + f.DocumentAutoKey + `, ` + f.LineAutoKey + `);" class="dropdown-item">Edit</a>
                                            <a onclick="javascript:onFileUpdateClicked(`+ f.DocumentAutoKey + `, ` + f.LineAutoKey + `);" class="dropdown-item">Update</a>`

        }

        if (!IsEnterprise || c.CanEdit) {
            dropdown += `<a onclick="javascript:onFileDelete(` + f.DocumentAutoKey + `, ` + f.LineAutoKey + `);" class="dropdown-item">Delete</a>`
        }

        dropdown += `<a onclick="javascript:onFileViewDetails('` + f.Name + `', '` + f.Ext + `', '` + formatDate(new Date(f.DateTimeAdded)) + `', '` + f.Size + `', '` + f.Note + `', '` + f.DocumentAutoKey + `');" class="dropdown-item" data-sidebar-target="#view-detail">
                                                Details
                                            </a>
                                            <a onclick="javascript:onFileShare(`+ f.DocumentAutoKey + `, ` + f.LineAutoKey + `);" class="dropdown-item">Share</a>
                                            
                                         <!--   <a onclick="javascript:onFileCopy(`+ f.DocumentAutoKey + `, ` + f.LineAutoKey + `);" class="dropdown-item">Copy to</a>
                                            <a onclick="javascript:onFileMove(`+ f.DocumentAutoKey + `, ` + f.LineAutoKey + `);" class="dropdown-item">Move to</a>
                                            <a onclick="javascript:onFileRename(`+ f.DocumentAutoKey + `, ` + f.LineAutoKey + `);" class="dropdown-item">Rename</a>-->
                                            
                                        </div>
                                    </div>`

        jsonData.push(
            [
                '',
                '<i class="tdDrag fas fa-grip-horizontal"></i>',
                '<span>' + f.ID + '</span>',
                `
<a href="javascript:onFileView(`+ f.DocumentAutoKey + `, ` + f.LineAutoKey + `);" class="d-flex align-items-center">
                                        <figure class="avatar avatar-sm mr-3">
                                            <span class="avatar-title rounded-pill">
                                                <i class="far fa-file" aria-hidden="true"></i>
                                                <span id="autokey" style="display:none">`+ f.DocumentAutoKey + `|||` + f.LineAutoKey + `|||` + f.Name + `|||` + f.Ext + `</span>
                                            </span>
                                        </figure>
                                        <span class="d-flex flex-column">
                                            <span class="text-primary">` + truncateString(f.Name, 12) + `.` + f.Ext + `</span>
                                            <span class="small font-italic">` + formatBytes(f.Size) + `</span>
                                        </span>
                                    </a>`,
                formatDate(new Date(f.DateTimeAdded)),
                truncateString(f.Note, 12),
                //     '<div class="badge bg-primary-bright text-primary">Public</div>',
                filterIt(users, f.UserID),
                dropdown
            ]
        );
    }


    var table = $('#table-files').DataTable({
        destroy: true,
        'columnDefs': [
            {
                'targets': 0,
                'className': 'dt-body-center',
                'render': function (data, type, full, meta) {
                    return '<div class="custom-control custom-checkbox">' +
                        '<input type="checkbox" class="custom-control-input" id="customCheck' + meta.row + '">' +
                        '<label class="custom-control-label" for="customCheck' + meta.row + '"></label>' +
                        '</div>';
                }
            },

            {
                "orderable": false,
                "targets": [0, 5]
            }
        ],
        rowReorder: {
            selector: ".tdDrag"
        },
        lengthMenu: [5, 10, 25, 50, 100],
        data: jsonData
    });


    table.on('mousedown', 'tbody tr', function () {
        var $row = $(this);
        isDragging = true;
        selectedRow = $row[0].querySelector("#autokey").innerHTML;
    });

    $('body').on('mouseup', mouseUp);


    $(document).on('click', '#table-files .custom-control-input', function () {
        if ($(this).prop('checked')) {
            $('#file-actions').removeClass('d-none');
            $(this).closest('td').closest('tr').addClass('tr-selected');
        } else {
            $(this).closest('td').closest('tr').removeClass('tr-selected');
            if ($('#table-files .custom-control-input:checked').length == 0) {
                $('#file-actions').addClass('d-none');
            }
        }
    });

    $(document).on('click', '#files-select-all', function () {
        // Check/uncheck all checkboxes in the table
        var rows = table.rows({ 'search': 'applied' }).nodes();

        $('input[type="checkbox"]', rows)
            .prop('checked', this.checked);
        if (this.checked) {
            $('input[type="checkbox"]', rows).closest('tr').addClass('tr-selected');
            $('#file-actions').removeClass('d-none');
        } else {
            $('input[type="checkbox"]', rows).closest('tr').removeClass('tr-selected');
            if ($('#table-files .custom-control-input:checked').length == 0) {
                $('#file-actions').addClass('d-none');
            }
        }
    });
}

var selectedRow;
var isDragging = false;
function mouseUp(event) {

    if (!isDragging) return;
    var nodeItem = $(document.elementsFromPoint(event.clientX, event.clientY))
        .filter('li')[0];
    isDragging = false;


    if (!nodeItem.getAttribute("class").includes("jstree-node")) {
        return;
    }


    var id = nodeItem.getAttribute("id");
    var autokey = selectedRow.split("|||")[0];

    swal({
        title: "Are you sure?",
        text: "Do you want to move the selected files into: " + categories.data.filter(x => x.AutoKey == id)[0].Name,
        icon: "warning",
        buttons: true,
        dangerMode: true,
    })
        .then((willMove) => {
            if (willMove) {
                var table = $('#table-files').DataTable();
                var rows = table.rows('.tr-selected').nodes();
                console.log(rows);
                moveFile(autokey, id);

                for (var i = 0; i < rows.length; i++) {
                    var autokeyTxt = rows[i].querySelector("#autokey").innerHTML;
                    autokey = autokeyTxt.split("|||")[0];

                    moveFile(autokey, id);
                }


                swal("File has been successfully moved!", {
                    icon: "success",
                });
            }
        });
}

function moveFile(autokey, catID) {

    var xmlHttp = new XMLHttpRequest();
    xmlHttp.open("GET", "api/Upload/GetFileInfo?InfoAutoKey=" + autokey, false);
    xmlHttp.send();

    var res = JSON.parse(xmlHttp.responseText);
    var fileCats = res.Categories.filter(x => x.AutoKey != selectedCatID);
    var fileKeys = res.SearchKeys;
    var fileContacts = res.Contacts;

    var outputCats = []
    $.each(fileCats, function (i, e) {
        if (e.AutoKey != null)
            outputCats.push(e.AutoKey);
        else
            outputCats.push(e);
    });

    var outputKeys = []
    $.each(fileKeys, function (i, e) {
        if (e.AutoKey != null)
            outputKeys.push(e.AutoKey);
        else
            outputKeys.push(e);
    });

    var outputCons = []
    $.each(fileContacts, function (i, e) {
        if (e.AutoKey != null)
            outputCons.push(e.AutoKey);
        else
            outputCons.push(e);
    });

    outputCats.push(catID);

    xhr = new XMLHttpRequest();

    xhr.onreadystatechange = function () {
        if (this.readyState == 4 && this.status == 200) {
            var t = this.responseText;
            var obj = JSON.parse(t);

            var statusCode = obj.StatusCode;
            if (statusCode == 0) {/*
                swal("File has been successfully moved!", {
                    icon: "success",
                });*/
                categoryChanged(selectedCatID);
            }
        }
    }


    xhr.open("POST", 'api/Upload/EditFile', true);

    const fd = new FormData();

    fd.append('AutoKey', autokey);
    fd.append('categories', JSON.stringify(outputCats))
    fd.append('contacts', JSON.stringify(outputCons));
    fd.append('keys', JSON.stringify(outputKeys));

    xhr.send(fd);
}

function filterIt(arr, searchKey) {
    var res = arr.find(o => o.ID === searchKey);
    if (res == null || res == '') {
        return "";
    }

    res = res.Email
    if (res == null || res == '') {
        return "";
    }
    return res;
}
function truncateString(str, num) {

    if (str == null || str == '') {
        return '';
    }

    if (str.length > num) {
        return str.slice(0, num) + "...";
    } else {
        return str;
    }
}
function onFileView(infoAutoKey, lineAutoKey) {

    var xmlHttp = new XMLHttpRequest();
    xmlHttp.open("GET", "api/Upload/GetEncryptedString?key=" + infoAutoKey + "|" + lineAutoKey + "|" + selectedCatID, false);
    xmlHttp.send();

    var str = xmlHttp.responseText;

    //      window.location.href = 'Viewer?f=' + str;//?InfoAutoKey=' + infoAutoKey + '&LineAutoKey=' + lineAutoKey;

    $("#popup").dxPopup({
        width: 1280,
        height: 720,
        contentTemplate: function () {
            return $("<iframe frameBorder='0' style='width:100%; height: 100%' src='Share?f=" + str + "'/>");
        },
        showTitle: false,
        title: "View",
        visible: true,
        dragEnabled: true,
        closeOnOutsideClick: true
    });

    $("#popup").dxPopup("instance").show();
}
function onFileViewDetails(filename, ext, date, filesize, note, infoautokey) {
    $("#txtFN").text(filename);
    $("#txtFileSize").text(formatBytes(filesize));
    $("#txtFileType").text(ext);
    $("#txtDate").text(date);
    $("#txtDetailsNote").text(note);

    var xmlHttp = new XMLHttpRequest();
    xmlHttp.open("GET", "api/Upload/GetFileInfo?InfoAutoKey=" + infoautokey, false);
    xmlHttp.send();

    var res = JSON.parse(xmlHttp.responseText);

    var keys = res.SearchKeys.map(a => a.Name).join(', ');
    var contacts = res.Contacts.map(a => a.Name).join(', ');

    $("#txtContacts").text(contacts);
    $("#txtSearchKeys").text(keys);

    /*
    var jsonData = [];
 
    for (var i = 0; i < contacts.length; i++) {
        var contact = contacts[i];
 
        jsonData.push(
            [
                contact.Name,
                contact.Email,
                contact.Phone
            ]
        );
    }
    $('#table-contacts').DataTable({
        destroy: true,
        data: jsonData
    });
 
 
    jsonData = [];
    for (var i = 0; i < keys.length; i++) {
        var key = keys[i];
 
        jsonData.push(
            [
                key.Name
            ]
        );
    }
 
    $('#table-keys').DataTable({
        destroy: true,
        data: jsonData
    });*/

}
function onFileShare(infoautokey, lineautokey) {
    var base_url = window.location.origin;


    var xmlHttp = new XMLHttpRequest();
    xmlHttp.open("GET", "api/Upload/GetEncryptedString?key=" + infoautokey + "|" + lineautokey, false);
    xmlHttp.send();

    var str = xmlHttp.responseText;

    var url = base_url + '/Share?f=' + str;


    var input = document.getElementById('copyInput');
    input.value = url;

    input.focus();
    input.select();

    try {
        document.execCommand('copy');
        DevExpress.ui.notify({ message: 'Link was copied to clipboard', width: 320 }, "success", 2000);

    } catch (err) {
        DevExpress.ui.notify({ message: 'Unable to copy link', width: 320 }, "error", 2000);
    }
}
function onFileCopy(infoautokey, lineautokey) {
    console.log(infoautokey);
}

function onFileMove(infoautokey, lineautokey) {
    console.log(infoautokey);
}

function onFileRename(infoautokey, lineautokey, newName) {
    console.log(infoautokey);
}
function onFileDownload(infoautokey, lineautokey, ext, name) {
    console.log(name);
    download(infoautokey, lineautokey, ext, name);

}

var infoAutoKey;
var lineAutoKey;
function onFileDelete(infoautokey, lineautokey) {
    infoAutoKey = infoautokey;
    showDeleteDialog(false);

}

function showDeleteDialog(deleteMultiple = true) {
    swal({
        title: "Are you sure?",
        text: "Do you want to delete this file?",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    })
        .then((willDelete) => {
            if (willDelete) {
                deleteFiles(deleteMultiple);
                swal("File has been successfully deleted!", {
                    icon: "success",
                });
            }
        });
}

function onFileEdit(infoautokey, lineautokey) {

    var xmlHttp = new XMLHttpRequest();
    xmlHttp.open("GET", "api/Upload/GetEncryptedString?key=" + infoautokey, false);
    xmlHttp.send();

    var str = xmlHttp.responseText;
    window.location.href = 'ManageFile?f=' + str;
}

function onFileUpdateClicked(infoautokey, lineautokey) {
    this.infoAutoKey = infoautokey;
    this.lineAutoKey = lineautokey;
    $("#updateModal").modal('show');
}


function deleteFiles(deleteMultiple) {

    if (!deleteMultiple) {

        var xmlHttp = new XMLHttpRequest();
        xmlHttp.open("POST", "api/Upload/DeleteFile?AutoKey=" + infoAutoKey, false);
        xmlHttp.send();
        console.log(infoAutoKey);
        infoAutoKey = null;
    } else {
        var table = $('#table-files').DataTable();
        var rows = table.rows('.tr-selected').nodes();

        for (var i = 0; i < rows.length; i++) {
            var autokey = rows[i].querySelector("#autokey").innerHTML;
            var infoautokey = autokey.split("|||")[0];

            var xmlHttp = new XMLHttpRequest();
            xmlHttp.open("POST", "api/Upload/DeleteFile?AutoKey=" + infoautokey, false);
            xmlHttp.send();
        }

    }

    swal("Success!", "Files have been successfully deleted!", "success");

    categoryChanged(selectedCatID);
}

function downloadFiles() {
    var table = $('#table-files').DataTable();
    var rows = table.rows('.tr-selected').nodes();

    for (var i = 0; i < rows.length; i++) {
        var autokey = rows[i].querySelector("#autokey").innerHTML;
        var infoautokey = autokey.split("|||")[0];
        var lineautokey = autokey.split("|||")[1];
        var name = autokey.split("|||")[2];
        var ext = autokey.split("|||")[3];

        download(infoautokey, lineautokey, ext, name);

    }
}

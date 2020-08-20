$(document).ready(function () {

    var xmlHttp = new XMLHttpRequest();

    xmlHttp.open("GET", "api/authorization/getuserstorage", false);
    xmlHttp.send();
    storage = JSON.parse(xmlHttp.responseText);

    var usedStorageLbl = document.getElementById('usedStorage');
    var maxStorageLbl = document.getElementById('maxStorage');
    var usedStoragePrefix = document.getElementById('usedStoragePrefix');
    var maxStoragePrefix = document.getElementById('maxStoragePrefix');
    var prefix;
    var usedStorage = storage.UsedStorage;

    if (usedStorage >= 1000) {
        prefix = "GB";
        usedStorage = usedStorage / 1000;
    } else {
        prefix = "MB";
    }


    usedStorageLbl.innerHTML = calc(usedStorage);
    usedStoragePrefix.innerHTML = prefix;

    var maxStorage = storage.Storage;

    if (maxStorage == -1) {
        prefix = "Unlimited";
        maxStorage = null;
    } else {
        prefix = "GB";
        maxStorage = maxStorage / 1000;
    }


    maxStorageLbl.innerHTML = maxStorage;
    maxStoragePrefix.innerHTML = prefix;

    var bar = new ProgressBar.Circle('#progressBar', {
        strokeWidth: 2,
        easing: 'easeInOut',
        duration: 1400,
        color: '#0087FF',
        trailColor: '#eee',
        trailWidth: 1,
        svgStyle: null
    });


    var perc = (storage.UsedStorage / storage.Storage);
    console.log(storage.UsedStorage);
    console.log(storage.Storage);
    bar.animate(perc);

})

function calc(num) {
    var with2Decimals = num.toString().match(/^-?\d+(?:\.\d{0,2})?/)[0]
    return with2Decimals;
}

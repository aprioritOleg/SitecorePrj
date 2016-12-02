window.onload = function () {
    body = document.getElementById("books");
    var pageCounter = 0;
    var list = body.getElementsByTagName("tr");
    var count = 0;
    for (var i = 0; i < body.rows.length; i++) {
        if (i % 5 == 0) {
            if (list[i].style('display') != 'none') {
                pageCounter += 1;
                $("#paging").append('<button type="button" class="btn btn-secondary" onclick ="sort_table(books, null, null, null,' + pageCounter + ');">' + (pageCounter) + '</button>');
            }

        }
    }
    sort_table(body, 2, 1, false, 1);
};

function deleteBook() {
    var idBook = $("#bookIdHandler").val();
    $.ajax({
        url: "/Library/Delete",
        type: "GET",
        dataType: "html",
        data: { id: idBook },
        success: function (data) {
            location.reload();
        }
    })
};

function setNewQuantity() {
    var idBook = $("#bookIdHandler").val();
    var newQuantity = $("#newQuantity").val();
    $.ajax({
        url: "/Library/SetQuantity",
        type: "GET",
        dataType: "html",
        data: { id: idBook, quantity: newQuantity },
        success: function (data) {
            location.reload();
            $('div#libraryBooks').html(data);
        }
    })
};

function afterReload() {
    tbody = document.getElementById("books");
    var pageCounter = 0;
    for (var i = 0; i < tbody.rows.length; i++) {
        if (i % 5 == 0) {
            pageCounter += 1;
            $("#paging").append('<button type="button" class="btn btn-secondary" onclick ="sort_table(books, null, null, null,' + pageCounter + ');">' + (pageCounter) + '</button>');
        }
    }
    //console.log('partial');
    sort_table(tbody, 2, 1, false, 1);
};








function authorAdd(element) {
    var elementText = $(element).text();
    $("#authorsList").val($("#authorsList").val() + elementText + '\n');
    $(element).hide();
}
function addNewAuthor() {
    var elementText = $("#newAuthor").val();
    if (elementText != null && elementText != "") {
        $("#authorsList").val($("#authorsList").val() + elementText + '\n');
    }
}
function clearBox() {
    $("#authorsList").val("");
    $('ul#authorsUl li').each(function () {
        $(this).show();
    });
}
function makeBlack(id) {
    $("label[for='" + $("#" + id).attr('id') + "']").css("color", "black");
}

function saveNewBook() {
    var authorsList = $("#authorsList").val().trim();
    authorsList = authorsList.split("\n");
    var quantity = $("#quantity").val();
    var title = $("#title").val();
    if (title.length <= 0) {
        //console.log('ops');
        var label = $("label[for='" + $("#title").attr('id') + "']").css("color", "red");
    }
    else {
        var book = { Title: title, Quantity: quantity };
        $.ajax({
            url: "/Library/AddNewBook",
            type: "POST",
            dataType: "json",
            data: { book, arr: authorsList }
            //data: { book, arr: authorsList },
            //success: function (data) {               
            //    //location.reload();  
            //    console.log("reloaded");
            //    $('div#libraryBooks').html(data);
            //}
        }).done(function () {
            location.reload();
        });
    }

}













function openDeleteModal(id) {
    $("#bookIdHandler").val(id);
    $("#deleteModal").modal();
}
function changeQuantityModal(id) {
    $("#bookIdHandler").val(id);
    $("#changeQuantityModal").modal();
}


var asc1 = 1,
    asc2 = 1,
    asc3 = 1,
    prevAsc = 1,
    prevCol = 2;
availableOnlyGlobal = false;
var isUserOnly = false;
var maxElements = 5;
var currentPage = 1;
window.onload = function () {
    books = document.getElementById("books");
}


function sort_table(tbody, col, asc, availableOnly, paging) {

    var t = document.getElementById(tbody);
    if (paging == undefined || paging == null) {
        paging = currentPage;
    }
    else {
        currentPage = paging;
    }
    if (availableOnly != null && availableOnly != undefined) {
        availableOnlyGlobal = availableOnly;
    }
    else {
        availableOnly = availableOnlyGlobal;
    }
    if (asc == null) {
        asc = prevAsc;
    }
    else {
        prevAsc = asc;
    }
    if (col == null) {
        col = prevCol;
    }
    else {
        prevCol = col;
    }


    var rows = tbody.rows,
        rlen = rows.length,
        arr = new Array(),
        i, j, cells, clen;
    // fill the array with values from the table
    for (i = 0; i < rlen; i++) {
        cells = rows[i].cells;
        clen = cells.length;
        arr[i] = new Array();
        for (j = 0; j < clen; j++) {
            arr[i][j] = cells[j].innerHTML;
        }
    }
    if (availableOnly) {
        arr.sort(compare);
        function compare(a, b) {
            if (parseInt(a[1]) > 0 && parseInt(b[1]) > 0) {
                return 0;
            }
            else if (parseInt(a[1]) > 0 && parseInt(b[1]) <= 0) {
                return -1;
            }
            else if (parseInt(a[1]) <= 0 && parseInt(b[1]) > 0) {
                return 1;
            }
        };
    }
    else {
        // sort the array by the specified column number (col) and order (asc)
        arr.sort(function (a, b) {
            return (a[col] == b[col]) ? 0 : ((a[col] > b[col]) ? asc : -1 * asc);
        });
    }

    // replace existing rows with new rows created from the sorted array
    for (i = 0; i < rlen; i++) {
        //console.log((currentPage * maxElements) + " " + (currentPage * maxElements - maxElements))
        if (currentPage * maxElements > i && i >= currentPage * maxElements - maxElements) {
            if (availableOnly && parseInt(arr[i][1]) <= 0) {
                rows[i].innerHTML = "<td>" + arr[i].join("</td><td>") + "</td>";
                rows[i].style.display = 'none';
            }
            else {
                rows[i].style.display = 'table-row';
                rows[i].innerHTML = "<td>" + arr[i].join("</td><td>") + "</td>";
            }
            if (!availableOnly) {
                rows[i].style.display = 'table-row';
                rows[i].innerHTML = "<td>" + arr[i].join("</td><td>") + "</td>";
            }
        }
        else {
            rows[i].innerHTML = "<td>" + arr[i].join("</td><td>") + "</td>";
            rows[i].style.display = 'none';
        }
    }
    tested();
    function tested() {
        //hide id of each book
        var list = books.getElementsByTagName("tr");
        for (var i = 0; i < list.length; i++) {
            list[i].getElementsByTagName("td")[0].style.display = 'none';
        }
    }
}

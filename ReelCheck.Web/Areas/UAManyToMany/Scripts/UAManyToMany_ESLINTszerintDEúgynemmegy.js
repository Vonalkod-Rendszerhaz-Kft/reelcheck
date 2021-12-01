/*
* Created by Raju Dasa on 30-oct-2011
* www.RajuDasa.blogspot.com
* free source, change code for ur purpose.
*/

//http://files.myopera.com/RAJUDASA/DragDrop/DragDrop.htm
//http://jqueryui.com/demos/sortable/#connect-lists

// insert elements in #list1 and #list2
function pageload($jsonForList1, $jsonForList2) {
    if ($jsonForList1 !== null) {
        $("#list1").empty().addItems($jsonForList1);
    }
    if ($jsonForList2 !== null) {
        $("#list2").empty().addItems($jsonForList2);
    }

    $("#list1, #list2").sortable({
        connectWith: ".connectedSortable",
        placeholder: "ui-sortable-placeholder",
        forcePlaceholderSize: true,
        beforeStop: function (event, ui) {
            console.log('beforeStop:');
            ui.helper.removeClass("selectedItem");
            ui.helper.off();
        },
        change: function (event, ui) {  //for issue solving  
            console.log('change: vontatás');
            var placeHolder = ui.helper.children(".ui-sortable-placeholder");
            if (placeHolder) {
                placeHolder.detach().appendTo(ui.helper.parent());
            }
        },
        start: function (event, ui) {
            console.log('start:');
            if (ui.helper) {
                var cnt = ui.helper.parent().children(".selectedItem:not(.ui-sortable-placeholder)").length;
                if (cnt > 1) {
                    var parent = ui.helper.parent();
                    var childs = parent.children("div.selectedItem:not(.ui-sortable-placeholder)");
                    $.each(childs, function (index, child) {
                        child = $(child);
                        if (ui.helper.attr('id') !== child.attr('id')) {
                            child = child.detach();
                            child.appendTo(ui.helper);
                            child.css("margin", "0px").css("padding", "0px"); //addClass not working
                        }
                    });
                }
            }
        },
        stop: function (event, ui) {
            console.log('stop:');
            //console.log(ui.helper);
            if (ui.item) {
                var cnt = $(ui.item[0]).children("div").length;
                if (cnt > 0) {
                    //ui.helper is null
                    var dropItem = $(ui.item[0]);
                    $.each(dropItem.children("div"), function (index, child) {
                        child = $(child).detach();
                        child.insertAfter(dropItem);
                        child.removeClass("selectedItem");
                        child.css("margin", "").css("padding", "");
                    });
                }
            }
        }
    }).disableSelection();
}

// custom jq function/plugin : used instead of template plugin
$.fn.addItems = function (data) {
    return this.each(function () {
        var parent = this;
        $.each(data, function (index, itemData) {
            $("<div>")
       .text(itemData.Text)
       .attr("id", "MTMValue:" + itemData.Value)
       .addClass("item")
       .appendTo(parent);
        });
    });
};

// click color handling
$(document).on('click', '#list1>div, #list2>div', function (e) {
    if (!e.ctrlKey) {
        $(this).parent().children().removeClass("selectedItem");
    }
    $(this).toggleClass("selectedItem");
});

// array difference: filter and indexOf not supported in IE8 or below
//http://stackoverflow.com/questions/1187518/javascript-array-difference
Array.prototype.diff = function (a) {
    return this.filter(function (i) { return !(a.indexOf(i) > -1); });
};

var isWasAnAjaxCall = false;   // ajax hívás történt e már?

/**
 * Megvizsgálja a #list1-ben lévő divek id-ját 
 * és ha változás történt akkor ajax hívást kezdeményez
 * 
 * @param {any} name            : Név ami meghatározza, hogy melyik osztály interfészéről van szó.
 * @param {any} divItemIdsArray : #list1-ben lévő divek id-jai
 * @param {any} ajaxAddUrl      : hozzáadás ajax hívás url-je
 * @param {any} isDefault       : true/false
 */
function checkList1Divs(name, divItemIdsArray, ajaxAddUrl, isDefault) {
    $("#list1").bind("DOMSubtreeModified", function () {
        if ($('#mtmDropDownList>div.selectedItem:first') !== null && isWasAnAjaxCall === false) {
            var array = new Array();
            $.each($("#list1").find("div"), function (index, child) {
                child = $(child);
                if (!$(child).hasClass('ui-sortable-placeholder')) {
                    array.push(child.attr('id').substring(9, child.attr('id').length));
                }
            });
            if (array !== null && array.length !== divItemIdsArray.length) {
                if (array.length > divItemIdsArray.length) {
                    ajaxCall(name, ajaxAddUrl, array.diff(divItemIdsArray), isDefault);
                }
            }
        }
    });
}

/**
 * megvizsgálja a #list2-ben lévő divek id-ját 
 * és ha változás történt akkor ajax hívást kezdeményez
 * 
 * @param {any} name            : Név ami meghatározza, hogy melyik osztály interfészéről van szó.
 * @param {any} divItemIdsArray : #list2-ben lévő divek id-jai
 * @param {any} ajaxRemoveUrl   : törlés ajax hívás url-je
 * @param {any} isDefault       : true/false
 */
function checkList2Divs(name, divItemIdsArray, ajaxRemoveUrl, isDefault) {
    $("#list2").bind("DOMSubtreeModified", function () {
        if ($('#mtmDropDownList>div.selectedItem:first') !== null && isWasAnAjaxCall === false) {
            var array = new Array();
            $.each($("#list2").find("div"), function (index, child) {
                child = $(child);
                if (!$(child).hasClass('ui-sortable-placeholder')) {
                    array.push(child.attr('id').substring(9, child.attr('id').length));
                }
            });
            if (array !== null && array.length !== divItemIdsArray.length) {
                if (array.length > divItemIdsArray.length) {
                    ajaxCall(name, ajaxRemoveUrl, array.diff(divItemIdsArray), isDefault);
                }
            }
        }
    });
}

/**
 * ajax hívás
 * 
 * 'messagePleaseWait' nevű változó a ManyToMany.cshtml-ben kap értéket.
 * 
 * @param {any} name     : Név ami meghatározza, hogy melyik osztály interfészéről van szó.
 * @param {any} ajaxUrl  : url
 * @param {any} idsArray : id-kat tartalmazó tömb
 * @param {any} isDefault: true/false
 */
function ajaxCall(name, ajaxUrl, idsArray, isDefault) {
    var thisfn = 'UAManyTOMany.js ajaxCall function: ';
    console.log(thisfn + 'PING name, idsArray, ajaxUrl', name, idsArray, ajaxUrl);
    if (idsArray !== null && idsArray.length > 0 && isWasAnAjaxCall === false) {
        //var waitDialog = vrhct.bootbox.wait(messagePleaseWait, function() {   //egyelőre zavar, mert többször hívódik a függvény vontatás esetén
            var ajaxId = $('#mtmDropDownList>div.selectedItem:first').attr('id').substring(9, $('#mtmDropDownList>div.selectedItem:first').attr('id').length);
            $.ajax({
                cache: false,
                url: ajaxUrl,
                type: "post",
                contenttype: "application/j-son",
                datatype: 'json',
                traditional: true,
                data: { name: name, id: ajaxId, ids: idsArray, isDefault: isDefault },
                beforeSend: function () {
                    disableOperations();
                },
                success: function (responseData) {
                    $('#MTMMultiSelectLists').html(responseData);
                },
                complete: function () {
                    enableOperations();
                    //waitDialog.modal('hide');
                }
            });
        //});
    }
}

/**
 * Összegyűjti egy tömbbe a selectedItem class-al rendelkező divek Id-ját
 * a megadott selector alatt, majd ajax hívást kezdeményez
 * @param {any} name        : Név ami meghatározza, hogy melyik osztály interfészéről van szó.
 * @param {any} listSelector: selector (#list1 / #list2)
 * @param {any} ajaxUrl     : url
 * @param {any} isDefault   : true/false
 */
function getSelectedItems(name, listSelector, ajaxUrl, isDefault) {
    var array = new Array();
    $.each($(listSelector).find("div.selectedItem"), function (index, child) {
        child = $(child);
        array.push(child.attr('id').substring(9, child.attr('id').length));
    });
    if (array.length === 0) {
        array = null;
    }
    ajaxCall(name, ajaxUrl, array, isDefault);
}

// ajax hívás a switch buttonra
// name:        Név ami meghatározza, hogy melyik osztály interfészéről van szó.
// ajaxUrl:     url
// id:          id
// isDefault:   true/false
function ajaxCallSwitchButton(name, ajaxUrl, id, isDefault) {
    var thisfn = 'UAManyTOMany.js ajaxCallSwitchButton function: ';
    console.log(thisfn + 'PING name, id, ajaxUrl', name, id, ajaxUrl);
    if (isWasAnAjaxCall === false) {
        $.ajax({
            cache: false,
            url: ajaxUrl,
            type: "post",
            contenttype: "application/j-son",
            datatype: 'json',
            data: { name: name, id: id, isDefault: isDefault },
            beforeSend: function () {
                disableOperations();
            },
            success: function (responseData) {
                $('#MTMMain').html(responseData);
            },
            complete: function () {
                enableOperations();
            }
        });
    }
}

// műveletek tiltása
function disableOperations() {
    isWasAnAjaxCall = true;
    $('#MTMMain').find(':submit').attr('disabled', 'disabled');
    $("#list1, #list2").sortable('disable');
    $('.item').addClass('itemDisabled');
    $('.itemDisabled').removeClass('item');
}

// műveletek engedélyezése
function enableOperations() {
    isWasAnAjaxCall = false;
    $('#MTMMain').find(':submit').removeAttr('disabled');
    $('#list1, #list2').sortable('enable');
    $('.itemDisabled').addClass('item');
    $('.item').removeClass('itemDisabled');
}
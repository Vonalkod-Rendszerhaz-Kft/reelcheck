/*
 * MasterData kezelést támogató javascript összetevők
 * ==================================================
 * 
 * Egyelőre a régi DataTables megoldásból a form kezelést
 * támogató dolgok kerülnek ide.
 * Aztán majd ha kialakult az egységes koncepció, akkor majd 
 * fejlődnek, átalakulnak vagy megszünnek.
 */

function addClassFormControl(bootboxid, toselector) {
    addClass(bootboxid, 'form-control', toselector);
}




/**
 * A függvény 2 dolgot tesz:
 * 1. A "selector" paraméterben megadott elem alatt
 * az "editor-htmlattributes" attribútummal rendelkező
 * elemek első gyermekéhez hozzáadja a "form-control"
 * nevű osztályt. (Lásd: bootstrap).
 * 2. ".dataTablesDateTimePicker" osztállyal rendelkező
 * elemeken aktiválja a "datetimepicker" plugint.
 * 
 * @param {string} selector Egy jQuery szelektor sztring.
 */
function bootBoxAjaxOnComplete(selector) {
    $(selector).find('.editor-htmlattributes').children(':first-child').addClass('form-control');
    $(selector).find('.dataTablesDateTimePicker').datetimepicker();
}

/**
 * Egy bootbox dialog meghívása, amelyre kiteszi az OK és Cancel
 * gombot, és az OK-ra ráteszi a formSelector-ban megadott
 * form submit-jét.
 * Ha nincs megadva formSelector, akkor elvezérli a lapot
 * egy Delete URL-re.
 * Majd a végén meghívja a bootBoxAjaxOnComplete függvényt.
 * 
 * @param {Object} p Egy paraméter objektum
 * @param {string} p.titleText A modal ablak címe , fejléce.
 * @param {string} p.messageText A modal ablak testét alkotó üzenet vagy html sztring .
 * @param {string} p.formSelector Ha kell submit a modalis ablakban, akkor adjuk meg a formot azonosító selectort.
 */
function bootBoxDialog(p) {
    var dialog = bootbox.dialog({
        title: p.titleText,
        message: p.messageText,
        onEscape: function (event) {
            dialog.modal('hide');
        },
        buttons: {
            ok: {
                label: AjaxParameters.Translations.OK,
                className: AjaxParameters.Classes.OK,
                callback: function () {
                    if (p.formSelector) {
                        $(p.formSelector).submit();
                        return false;
                    } else if (p.dataId) {
                        window.location = AjaxParameters.Delete.AjaxUrl + '/' + p.dataId;
                    }
                }
            },
            cancel: {
                label: AjaxParameters.Translations.Cancel,
                className: AjaxParameters.Classes.Cancel,
                callback: function () {
                }
            }
        }
    });
    bootBoxAjaxOnComplete(dialog);
}

/**
 * A vrhct.bootbox.show őse.
 * 
 * A "p" paraméterben megadott URL-t meghívja, és annak eredményét
 * megmutatja egy modal ablakban, amelyhez a fenti
 * "bootBoxDialog" nevű függvényt használja.
 * 
 * @param {Object} p Paraméter objektum.
 * @param {string} p.ajaxUrl Az akció URL-je, mely eredményét meg kell jeleníteni.
 * @param {any} p.dataId Adat azonosító, amely "id" névű paraméterben küld el az akciónak.
 * @param {string} p.titleText A megjelenő modális ablak fejléce.
 * @param {string} p.formSelector A fomr azonosítója, melyen az OK-ra meghívja a submit()-et.
 */
function ajaxBootboxDialog(p) {
    $.ajax({
        cache: false,
        url: p.ajaxUrl,
        type: 'get',
        contenttype: 'application/json',
        datatype: 'json',
        data: { id: p.dataId },
        success: function (responseData) {
            if (!!responseData[0] && responseData[0].ErrorMessage) {
                bootbox.alert(responseData[0].ErrorMessage, function () {
                    location.reload();
                });
            } else {
                var messageText = '<div id="' + p.dialogId + '">' + responseData + '</div>';
                bootBoxDialog({
                    titleText: p.titleText,
                    messageText: messageText,
                    formSelector: p.formSelector
                });
            }
        },
        error: function (jqXHR, exception) {
            console.log('Ajax hívás sikertelen! ', jqXHR.responseText);
        }
    });
}

/**
 * A régi megoldásban ide voltak vezérelve a Manager felület
 * create gombjai.
 */
function dataTableCreateButton() {
    ajaxBootboxDialog({
        ajaxUrl: AjaxParameters.Create.AjaxUrl,
        dialogId: 'createDialog',
        formSelector: '#createForm',
        titleText: AjaxParameters.Translations.Create
    });
}

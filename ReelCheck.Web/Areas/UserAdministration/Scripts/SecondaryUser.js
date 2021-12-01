/***************************************************
SecondaryUser.js
    A Vonalkód Rendszerház másodlagos bejelentkezések kezeléséhez
    készült metódusok és események.
----------------------
Alapítva:
    2019.02.23.-03.08. Wittmann Antal
Módosult:
****************************************************/
/*##### EVENTS #####*/
$(document).ready(function () {
    'use strict';

    var thisfn = 'UserAdministration.SecondaryUser.js: ready event: ';
    console.log(thisfn + 'PING');

    // Resize figyelése, hogy a card-body mérete kitöltse a képernyőt
    $(window).resize(function () {
        console.log(thisfn + 'window.resize event')
        vrhct.masterdata.resizeCardBody();
    });
    vrhct.masterdata.resizeCardBody();
    if (typeof vrhmenu != 'undefined') {
        vrhmenu.MenuSwapEventFunction = function () {
            vrhct.masterdata.resizeCardBody();  // menüváltozáskor is hívja meg a resize-t
        }
    }

    vrhsls.InitTable();

    console.log(thisfn + 'END');
}); // $(document).ready END 
/*##### EVENTS END #####*/


/**
 * A nyelvi kezelőfelülethez készült prototípus.
 * 
 * @param {ExportToSecondaryUserScript} imp Paraméter objektum, cshtml-ből küldött és ott feltöltött állandókat tartalmaz.
 */
function SecondaryUserScript(imp) {
    'use strict';

    /*##### PROTOTYPE VARIABLES #####*/
    var me = this;  // hogy a belső függvényekben is tudjak hivatkozni a prototype tulajdonságaira
    var thispt = 'SecondaryUserScript: ';
    var isInEdit = false;   // annak jelzése, hogy a szerkesztés ablakot aktiválták
    var isInDelete = false; // annak jelzése, hogy a törlő ablakot aktiválták
    var isInManage = false; // annak jelzése, hogy a funkciók kezelését végző ablakot aktiválták

    // DataTable oszlopok definciója
    var dtColumns = [
        { data: imp.Data.FunctionName },
        { data: imp.Data.PrimaryName },
        { data: imp.Data.SecondaryName },
        { data: imp.Data.SecondaryPassword },
        {
            data: imp.Data.Active,
            render: function (data, type, row) {
                var icon = data ? "fa-check-square" : "fa-square"
                var html = '<div class="text-center"><i class="far ' + icon + '"></i></div>';
                return html;
            }
        },
        {   // a műveletek oszlopa
            render: function (data, type, row) {
                var temp = document.createElement('div');

                if (imp.EditMode.Current != imp.EditMode.Select) {
                    var spane = document.createElement('span');
                    $(spane).attr('title', imp.Titles.SecondaryUserEdit);
                    $(spane).addClass('fas fa-pencil-alt datatableActionIcon');
                    $(spane).attr('onclick', 'vrhsls.Edit(' + row.Id + ')');
                    $(temp).append(spane);
                }

                if (imp.EditMode.Current == imp.EditMode.Delete || imp.EditMode.Current == imp.EditMode.Manage) {
                    var spand = document.createElement('span');
                    $(spand).attr('title', imp.Titles.SecondaryUserDelete);
                    $(spand).addClass('fas fa-trash-alt datatableActionIcon');
                    $(spand).attr('onclick', 'vrhsls.Delete(' + row.Id + ', "' + row.Name + '")');
                    $(temp).append(spand);
                }

                return $(temp).html();
            }
        }
    ];
    var dtColumnDefs = [
        { targets: [4], width: '55px', orderable: false },
        { targets: [5], width: '40px', orderable: false, searchable: false }
    ];
    var dtFilterDefs = [
        { column: 0, id: imp.Ids.Input.FilterFunction },
        { column: 1, id: imp.Ids.Input.FilterUser },
        { column: 4, id: imp.Ids.Input.FilterActive },
    ];
    /*##### PROTOTYPE VARIABLES END #####*/


    /*##### PROPERTIES #####*/

    /** Az inicializált tábla DataTable objektuma */
    this.SecondaryUserTable = null;

    /*##### PROPERTIES END #####*/


    /*##### METHODS #####*/

    /**
     * Tábla inicializálása.
     */
    this.InitTable = function () {
        var thisfn = thispt + 'InitTable method: ';
        me.SecondaryUserTable = new VrhDataTable({
            ajax: { url: imp.Url.GetData }, // az adatlistát eredményező akció
            autoWidth: false,
            columns: dtColumns,
            columnDefs: dtColumnDefs,
            filterDefs: dtFilterDefs,
            lcid: imp.LCID,
            initComplete: function (setting, json) { // a táblázat elkészültekor meghívott függvény
                console.log(thisfn + 'Table.initComplete callback: PING');
                //SetFilterFunction();
            },
            tableId: imp.Ids.Table,
        });
    };// InitTable method END

    /**
     * Másodlagos felhasználó szerkesztés ablak inicializálása.
     * 
     * @param {number} id A másodlagos felhasználó egyedi azonosítója. Ha -1, akkor felvitel.
     */
    this.Edit = function (id) {
        try {
            if (isInEdit) return;
            isInEdit = true;

            var thisfn = thispt + 'Edit method: ';
            console.log(thisfn + 'id=%d', id);
            vrhct.bootbox.edit({
                ajax: {
                    url: imp.Url.Editor,
                    data: { secondaryUserId: id }
                },
                title: imp.Labels.Button.FunctionManage,
                formid: imp.Ids.Form.Editor,
                size: 'large',
                shown: function () {
                    isInEdit = false;
                }
            });
        } catch (e) {
            isInEdit = false;
            bootbox.alert(e.Message);
        }
    }; // Edit method END

    /**
     * Másodlagos felhasználó törlésének indítása.
     * 
     * @param {any} secondaryUserId A törlendő szókód.
     */
    this.Delete = function (secondaryUserId, secondaryUserName) {
        try {
            if (isInDelete) return;
            isInDelete = true;

            var thisfn = thispt + 'Delete method: ';
            console.log(thisfn + 'secondaryUserId=%d, secondaryUserName="%s"', secondaryUserId, secondaryUserName);
            vrhct.bootbox.delete({
                ajax: {
                    url: imp.Url.Delete,
                    data: { secondaryUserId: secondaryUserId }
                },
                title: imp.Titles.Confirmation,
                confirm: imp.Confirmations.SecondaryUserDelete.format(secondaryUserName),
                shown: function () {
                    isInDelete = false;
                },
                success: function () {
                    console.log(thisfn + 'Sikeres törlés. Itt vagyunk a callback-ben.');
                    me.SecondaryUserTable.Table.draw('page');
                }
            });
        } catch (e) {
            isInDelete = false;
            bootbox.alert(e.Message);
        }
    }; // Delete method END   

    /**
     * Másodlagos felhasználókat csoportosító funkciók kezelése
     */
    this.SecondaryFunctionManage = function () {
        try {
            if (isInManage) return;
            isInManage = true;

            var thisfn = thispt + 'SecondaryFunctionManage method: ';
            console.log(thisfn + 'PING');
            vrhct.bootbox.show(imp.Url.Manage, {
                pleaseWaitMessage: imp.Messages.Wait,
                shown: function (bootboxid) {
                    console.log(thisfn + 'SHOWN bootboxid="%s"', bootboxid);
                    isInManage = false;
                },
                hidden: function () {
                    me.SecondaryUserTable.Table.draw('page');
                },
            });

        } catch (e) {
            isInManage = false;
            bootbox.alert(e.Message);
        }
    }; // SecondaryFunctionManage method END

    /*##### METHODS END #####*/


    /*##### PRIVATE FUNCTIONS #####*/
    /*##### PRIVATE FUNCTIONS END #####*/

} // SecondaryUserScript prototype END

function ExportToSecondaryUserScript() {
    /** Megerősítést kérő üzenetek gyűjtőhelye */
    this.Confirmations = {
        SecondaryUserDelete: '',
    };
    /** A táblázat oszlopainak adatelérése */
    this.Data = {
        FunctionName: '',
        PrimaryName: '',
        SecondaryName: '',
        SecondaryPassword: '',
        Active: ''
    };
    /** Szerkesztési mód aktuális és lehetséges értékei */
    this.EditMode = {
        Current: '',
        Select: '',
        Add: '',
        Delete: '',
        Manage: '',
    };
    /** A nézeten lévő html objektumok azonosítóinak gyűjtőhelye */
    this.Ids = {
        Form: {
            Editor: ''
        },
        Input: {
            FilterFunction: '',
            FilterUser: '',
            FilterActive: '',
        },
        Table: '',
    };
    /** A nézeten lévő inputok címkéinek gyűjtőhelye */
    this.Labels = {
        Cancel: '',
        OK: '',
        No: '',
        Yes: '',
        Button: {
            FunctionManage: ''
        },
    };
    /** A környetben érvényes nyelvi kód */
    this.LCID = '';
    /** Az üzenetek gyűjtőhelye */
    this.Messages = {
        Wait: '',
    };
    /** A nézeten lévő eszközök feliratozásának, címeinek és tooltipjeinek gyűjtőhelye */
    this.Titles = {
        Confirmation: '',
        OperationProgress: '',
        SecondaryUserAdd: '',
        SecondaryUserEdit: '',
        SecondaryUserDelete: '',
    };
    /** A nézeten lévő akciókra mutató url-ek gyűjtőhelye */
    this.Url = {
        Editor: '',
        Delete: '',
        GetData: '',
        Manage: '',
    };
}

/***************************************************
UserManager.js
    A Vonalkód Rendszerház felhasználók kezeléséhez
    készült metódusok és események.
----------------------
Alapítva:
    2019.03.05.-03.08. Wittmann Antal
Módosult:
****************************************************/
/*##### EVENTS #####*/
$(document).ready(function () {
    'use strict';

    var thisfn = 'UserAdministration.UserManager.js: ready event: ';
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

    vrhusman.InitTable();

    console.log(thisfn + 'END');
}); // $(document).ready END 
/*##### EVENTS END #####*/


/**
 * A nyelvi kezelőfelülethez készült prototípus.
 * 
 * @param {ExportToUserManagerScript} imp Paraméter objektum, cshtml-ből küldött és ott feltöltött állandókat tartalmaz.
 */
function UserManagerScript(imp) {
    'use strict';

    /*##### PROTOTYPE VARIABLES #####*/
    var me = this;  // hogy a belső függvényekben is tudjak hivatkozni a prototype tulajdonságaira
    var thispt = 'UserManagerScript: ';
    var isInEdit = false;           // annak jelzése, hogy a szerkesztés ablakot aktiválták
    var isInDelete = false;         // annak jelzése, hogy a törlő ablakot aktiválták
    var isInPasswordRenewal = false;// annak jelzése, hogy a törlő ablakot aktiválták
    var isInRoleManage = false;     // annak jelzése, hogy a szerepek kezelését végző ablakot aktiválták
    var isInRoleGroupManage = false;// annak jelzése, hogy a szerep körök kezelését végző ablakot aktiválták
    var isInUnlock = false;         // annak jelzése, hogy a zárolás feloldását aktiválták
    var editIcon = imp.EditMode.Current == imp.EditMode.Select ? 'fa-info-circle' : 'fa-pencil-alt';
    var editTitle = imp.EditMode.Current == imp.EditMode.Select ? imp.Titles.UserDetails : imp.Titles.UserEdit;

    // DataTable oszlopok definciója
    var dtColumns = [
        { data: imp.Data.UserName },
        { data: imp.Data.Email },
        { data: imp.Data.LastLoginString },
        { data: imp.Data.LastActivity },
        { data: imp.Data.Comment },
        {
            data: imp.Data.Status,
            render: function (data, type, row) {
                var temp = document.createElement('div');
                $(temp).append(data);
                if (row.IsLockedOut && imp.EditMode.Current != imp.EditMode.Select) {
                    var spanl = document.createElement('span');
                    $(spanl).attr('title', imp.Titles.Unlock);
                    $(spanl).addClass('fas fa-unlock-alt datatableActionIcon ml-1');
                    $(spanl).attr('onclick', 'vrhusman.Unlock("' + row.UserId + '", "' + row.UserName + '")');
                    $(temp).append(spanl);
                }
                return $(temp).html();
            }
        },
        {   // a műveletek oszlopa
            render: function (data, type, row) {
                var temp = document.createElement('div');

                if (imp.EditMode.Current != imp.EditMode.Select) {
                    var spanp = document.createElement('span');
                    $(spanp).attr('title', imp.Titles.PasswordRenewal);
                    $(spanp).addClass('fas fa-key datatableActionIcon');
                    $(spanp).attr('onclick', 'vrhusman.PasswordRenewal("' + row.UserName + '")');
                    $(temp).append(spanp);
                }

                var spane = document.createElement('span');
                $(spane).attr('title', editTitle);
                $(spane).addClass('fas ' + editIcon + ' datatableActionIcon');
                $(spane).attr('onclick', 'vrhusman.Edit("' + row.UserName + '")');
                $(temp).append(spane);

                if (row.IsRemovable
                    && imp.EditMode.Current != imp.EditMode.Select
                    && imp.EditMode.Current != imp.EditMode.Add) {
                    var spand = document.createElement('span');
                    $(spand).attr('title', imp.Titles.UserDelete);
                    $(spand).addClass('fas fa-trash-alt datatableActionIcon');
                    $(spand).attr('onclick', 'vrhusman.Delete("' + row.UserId + '", "' + row.UserName + '")');
                    $(temp).append(spand);
                }
                return $(temp).html();
            }
        }
    ];
    var dtColumnDefs = [
        //{ targets: [5], width: '55px' }, 
        { targets: [6], width: '60px', orderable: false, searchable: false } // műveletek
    ];
    var dtFilterDefs = [
        { column: 5, id: imp.Ids.Input.FilterStatus },
    ];
    /*##### PROTOTYPE VARIABLES END #####*/


    /*##### PROPERTIES #####*/

    /** Az inicializált tábla DataTable objektuma */
    this.UserTable = null;

    /*##### PROPERTIES END #####*/


    /*##### METHODS #####*/

    /**
     * Tábla inicializálása.
     */
    this.InitTable = function () {
        var thisfn = thispt + 'InitTable method: ';
        me.UserTable = new VrhDataTable({
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

    this.ChangeTemporary = function () {
        var thisfn = thispt + 'ChangeTemporary method: ';
        var urlGetData = imp.Url.GetData + '&isTemporary=' + $('#' + imp.Ids.Input.CheckBoxTemp).is(':checked');
        console.log(thisfn + 'url=%s', urlGetData);
        me.UserTable.Table.ajax.url(urlGetData);
        me.UserTable.Table.draw('page');
    };

    /**
     * Felhasználó szerkesztés ablak inicializálása.
     * 
     * @param {string} userName A felhasználó neve. Ha üres, akkor felvitel.
     */
    this.Edit = function (userName) {
        try {
            if (isInEdit) return;
            isInEdit = true;

            var thisfn = thispt + 'Edit method: ';
            var modalTitle;
            if (imp.EditMode.Current == imp.EditMode.Select) {
                modalTitle = imp.Titles.UserDetails;
            } else {
                if (userName) modalTitle = imp.Titles.UserEdit
                else modalTitle = imp.Titles.UserAdd;
            }
            console.log(thisfn + 'userName="%s", title="%s"', userName, modalTitle);
            vrhct.bootbox.edit({
                ajax: {
                    url: imp.Url.Editor,
                    data: { userName: userName }
                },
                buttonsDefault: imp.EditMode.Current != imp.EditMode.Select,
                title: modalTitle,
                formid: imp.Ids.Form.UserEditor,
                size: 'large',
                shown: function () {
                    isInEdit = false;
                }
            });
        } catch (e) {
            console.error(e);
            isInEdit = false;
        }
    }; // Edit method END

    /**
     * Felhasználó törlésének indítása.
     * 
     * @param {string} userId A törlendő felhasználó azonosítója.
     * @param {string} userName A törlendő felhasználó neve.
     */
    this.Delete = function (userId, userName) {
        try {
            if (isInDelete) return;
            isInDelete = true;

            var thisfn = thispt + 'Delete method: ';
            console.log(thisfn + 'userId="%s", userName="%s"', userId, userName);
            vrhct.bootbox.delete({
                ajax: {
                    url: imp.Url.Delete,
                    data: { userId: userId }
                },
                title: imp.Titles.Confirmation,
                confirm: imp.Confirmations.UserDelete.format(userName),
                shown: function () {
                    isInDelete = false;
                },
                success: function () {
                    console.log(thisfn + 'Sikeres törlés. Itt vagyunk a callback-ben.');
                    me.UserTable.Table.draw('page');
                }
            });
        } catch (e) {
            console.error(e);
            isInDelete = false;
        }
    }; // Delete method END   

    /**
     * Jelszó megújítása soréán a checkbox megváltoztatásakor hívódik.
     */
    this.PasswordGenerateChange = function () {
        try {
            var thisfn = thispt + 'PasswordGenerateChange method: ';
            console.log(thisfn + 'PING');

            var $newPassword = $('#' + imp.Ids.Input.NewPassword);
            if ($('#IsGenerated').is(':checked')) {
                console.log(thisfn + 'checked');
                $newPassword.val('');
                $newPassword.attr('readonly','');
            } else {
                console.log(thisfn + 'unchecked');
                $newPassword.removeAttr('readonly');
                $newPassword.focus();
            }
        } catch (e) {
            console.error(e);
        }
    }; // PasswordRenewal method END

    /**
     * Jelszó megújítása ablak inicializálása.
     * 
     * @param {string} userName A felhasználó neve.
     */
    this.PasswordRenewal = function (userName) {
        try {
            if (isInPasswordRenewal) return;
            isInPasswordRenewal = true;

            var thisfn = thispt + 'PasswordRenewal method: ';
            console.log(thisfn + 'userName="%s"', userName);
            vrhct.bootbox.edit({
                ajax: {
                    url: imp.Url.PasswordRenewal,
                    data: { userName: userName }
                },
                title: imp.Titles.PasswordRenewal + ' - ' + userName,
                formid: imp.Ids.Form.PasswordRenewal,
                //size: 'large',
                shown: function () {
                    isInPasswordRenewal = false;
                }
            });
        } catch (e) {
            console.error(e);
            isInPasswordRenewal = false;
        }
    }; // PasswordRenewal method END

    /**
     * Szerepek kezelése
     */
    this.RoleManage = function () {
        try {
            if (isInRoleManage) return;
            isInRoleManage = true;

            var thisfn = thispt + 'RoleManage method: ';
            console.log(thisfn + 'PING');
            vrhct.bootbox.show(imp.Url.RoleManage, {
                pleaseWaitMessage: imp.Messages.Wait,
                shown: function (bootboxid) {
                    console.log(thisfn + 'SHOWN bootboxid="%s"', bootboxid);
                    isInRoleManage = false;
                }
            });

        } catch (e) {
            console.error(e);
            isInRoleManage = false;
        }
    }; // RoleManage method END

    /**
     * Szerepkörök kezelése
     */
    this.RoleGroupManage = function () {
        try {
            if (isInRoleGroupManage) return;
            isInRoleGroupManage = true;

            var thisfn = thispt + 'RoleGroupManage method: ';
            console.log(thisfn + 'PING');
            vrhct.bootbox.show(imp.Url.RoleGroupManage, {
                pleaseWaitMessage: imp.Messages.Wait,
                shown: function (bootboxid) {
                    console.log(thisfn + 'SHOWN bootboxid="%s"', bootboxid);
                    isInRoleGroupManage = false;
                }
            });

        } catch (e) {
            console.error(e);
            isInRoleGroupManage = false;
        }
    }; // RoleManage method END

    /**
     * Zárolás feloldása
     * 
     * @param {string} userId A felhasználó azonosítója, akit fel kell szabadítani.
     * @param {string} userName A felhasználó neve, akit fel kell szabadítani.
     */
    this.Unlock = function (userId, userName) {
        try {
            if (isInUnlock) return;
            isInUnlock = true;

            var thisfn = thispt + 'Unlock method: ';
            var url = imp.Url.Unlock + '?userId=' + userId; 
            console.log(thisfn + 'PING url=', url);
            vrhct.bootbox.show(url, {
                ajaxType: 'post',
                isReturnInfoJSON: true,
                pleaseWaitMessage: imp.Messages.Wait,
                shown: function (bootboxid) {
                    console.log(thisfn + 'SHOWN bootboxid="%s"', bootboxid);
                    isInUnlock = false;
                },
                hidden: function () {
                    me.UserTable.Table.draw('page');
                }
            });

        } catch (e) {
            console.error(e);
            isInUnlock = false;
        }
    }; // Unlock method END

    /*##### METHODS END #####*/


    /*##### PRIVATE FUNCTIONS #####*/
    /*##### PRIVATE FUNCTIONS END #####*/

} // UserManagerScript prototype END

/** A prototípus számára küldendő állandó értékek osztálya. */
function ExportToUserManagerScript() {
    /** Megerősítést kérő üzenetek gyűjtőhelye */
    this.Confirmations = {
        UserDelete: '',
    };
    /** A táblázat oszlopainak adatelérése */
    this.Data = {
        UserName: '',
        Email: '',
        LastLogin: '',
        LastActivity: '',
        Comment: '',
        Status: ''
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
            UserEditor: '',
            PasswordRenewal: '',
        },
        Input: {
            FilterStatus: '',
            CheckBoxTemp: '',
            NewPassword: '',
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
        PasswordRenewal: '',
        Unlock: '',
        UserAdd: '',
        UserEdit: '',
        UserDelete: '',
        UserDetails: '',
    };
    /** A nézeten lévő akciókra mutató url-ek gyűjtőhelye */
    this.Url = {
        Editor: '',
        Delete: '',
        GetData: '',
        RoleGroupManage: '',
        RoleManage: '',
        Unlock: '',
    };
}

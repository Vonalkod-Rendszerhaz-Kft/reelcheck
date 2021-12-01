/***************************************************
MenuCommon.js
    A Vonalkód Rendszerház webes menü kerethez
    készült külön-külön töltendő eszközök.
----------------------
Alapítva:
    2019.02.13. Wittmann Antal
Módosult:
****************************************************/

/**
 * A Vonalkód Rendszerház webes menü kerethez
 * készült közös eszközök, amelyeket külön kell tölteni
 * az egyes cshtml-ekben. Így Layout nélkül is elérhető.
 * 
 * @param {object} imp A prototípus inicializálásához szükséges objektum a paraméterekkel.
 * @param {object} imp.DirectAuthentication A közvetlen autentikációs metódus állandói. 
 * @param {string} imp.DirectAuthentication.Url A közvetlen autentikáció akciója.
 * @param {string} imp.DirectAuthentication.Title A közvetlen autentikációs ablak címe.
 */
function MenuCommonScripts(imp) {
    'use strict';

    /*##### PROPERTIES #####*/
    /*##### PROPERTIES END #####*/

    /*##### PROTOTYPE VARIABLES #####*/
    var me = this;  //hogy a belső függvényekben is tudjak hivatkozni a prototype tulajdonságaira
    var thispt = 'MenuCommonScripts prototype: ';
    /*##### PROTOTYPE VARIABLES END #####*/


    /*##### METHODS #####*/

    //===============================================
    // Menüpont indítása közvetlen autentikációval.
    //===============================================
    /**
     * A megadott paraméterekből összeállítja a meghívandó URL-t.
     * A végrehajtó akció a '/UserAdministration/Account/DirectAuthentication', amely
     * a Vrh.Web.Membership-ben van.
     * Vagyis ha nincs felinstallálva az alkalmazásba a Membership, akkor nem történik semmi
     * a metódus meghívásával.
     * @param {string} targetUrl Az az akció vagy utasítás, amelyiket eredetileg indítani szerettek volna. Nevezzük funkciónak.
     * @param {string} rolesRequired Azoknak a szerepeknek a listája vesszővel elválasztva, amelyek valamelyike szükséges a funkció eléréséhez.
     * @param {string} responseTarget Az eredeti funció válaszának célja (CurrentPage,NewPage,Dialog,WaitFor).
     */
    this.StartWithDirectAuthentication = function (targetUrl, rolesRequired, responseTarget) {
        var thisfn = thispt + 'StartWithDirectAuthentication method: ';
        console.log(thisfn + 'PING targetUrl="%s"; rolesRequired="%s"; responseTarget="%s"', targetUrl, rolesRequired, responseTarget);

        var formid = 'DirectAuthenticationForm';// ilyen néven van a Vrh.Web.Membership-ben! Egyelőre legyen így.
        vrhct.bootbox.edit({
            ajax: {
                url: imp.DirectAuthentication.Url
                    + '?targetUrl=' + targetUrl
                    + '&rolesRequired=' + rolesRequired
                    + '&responseTarget=' + responseTarget
            },
            buttonsDefault: false,
            buttons: {
                OK: {
                    label: vrhct.bootbox.DefaultOptions.okLabel,
                    className: 'btn btn-success',
                    callback: function () {
                        $('#' + formid).submit();
                        return false;
                    }
                },
                Cancel: {
                    label: vrhct.bootbox.DefaultOptions.cancelLabel,
                    className: 'btn btn-secondary',
                    callback: function () { }
                },
            },
            title: imp.DirectAuthentication.Title,
            formid: formid,
            size: 'small',
            shown: function () {
            }
        });
    }
    //===================================================
    // Menüpont indítása közvetlen autentikációval. VÉGE
    //===================================================

    /*##### METHODS END #####*/

} // MenuCommonScripts prototype END


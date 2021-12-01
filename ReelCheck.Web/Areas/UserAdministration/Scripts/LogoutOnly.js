/***************************************************
LogoutOnly.js
    A Vrh.Web.Membership kijelentkezési logikáját támogató 
    metódusok és események.
----------------------
Alapítva:
    2018.10.02.-10.10. Wittmann Antal
Módosult:
****************************************************/
/*##### EVENTS #####*/
$(function () { // $(document).ready(function () {
    'use strict';
    var thisfn = 'UserAdministration.LogoutOnly.js: ready event: ';
    console.log(thisfn + 'START lgtnl.Logout.UrlBeforeLogout', lgtnl.Logout.UrlBeforeLogout);

    if (lgtnl.Logout.UrlBeforeLogout !== '') {
        $.ajax({
            url: lgtnl.Logout.UrlBeforeLogout,
            cache: false,
            type: "post",
            success: function (response) {
                if (response.ReturnValue === 0) {
                    console.log(thisfn + 'Nem üzent hibát a "beforelogout url".');
                    lgtnl.CallLogoutJSON();// ha sikeres a before logout url, akkor jöhet a kijelentkezés
                } else {
                    console.log(thisfn + 'Hiba jött a "beforelogout url"-től.');
                    $(lgtnl.Id.Message).html(response.ReturnMessage);
                }
            },
            error: function (jqXHR, exception) {
                console.log('Calling the "' + lgtnl.Logout.UrlBeforeLogout + '" unsuccessful! exception=', exception);
            }
        });
    }
    else {  // ha nincs before logout, akkor egyből kijelentkezik
        lgtnl.CallLogoutJSON();
    }

    console.log(thisfn + 'END');
}); // $(document).ready END 
/*##### EVENTS END #####*/

/**
 * A LogoutOnly kezelőfelülethez készült prototípus.
 * 
 * @param {ExportToLogoutOnly} imp Paraméter objektum, cshtml-ben feltöltenő paramétereket tartalmaz.
 */
function LogoutOnlyScripts(imp) {
    'use strict';

    /*##### PROTOTYPE VARIABLES #####*/
    var me = this;  //hogy a belső függvényekben is tudjak hivatkozni a prototype tulajdonságaira
    var thispt = 'LogoutOnly.js: LogoutOnlyScripts.';
    /*##### PROTOTYPE VARIABLES END #####*/

    this.Id = imp.Id;
    this.Logout = imp.Logout;

    /*##### METHODS #####*/

    /** Kijelentkezés végrehajtása */
    this.CallLogoutJSON = function () {
        var thisfn = thispt + 'CallLogoutJSON method: ';
        console.log(thisfn + 'PING');
        $.ajax({
            url: imp.Url.LogoutJSON,
            cache: false,
            type: "post",
            success: function (response) {
                if (response.ReturnValue === 0) {
                    console.log(thisfn + 'LogoutJSON sucess.');
                    if (imp.Logout.IsReloadLogout) {
                        location.reload();
                    }
                    else { //ha nincs reload, de ablakban nyílt meg, akkor azt be kell csukni
                        if (imp.BootboxId) {
                            vrhct.bootbox.hide(imp.BootboxId); 
                        }
                    }
                } else {
                    console.log(thisfn + 'LogoutJSON error! message=', response.ReturnMessage);
                    $(imp.Id.Message).text(response.ReturnMessage);
                }
            },
            error: function (jqXHR, exception) {
                console.log("Calling the LogoutJSON action unsuccessful! exception =", exception);
            }
        });
    };// method CallLogoutJSON END
    
    /*##### METHODS END #####*/

} // LogoutOnlyScripts prototype END

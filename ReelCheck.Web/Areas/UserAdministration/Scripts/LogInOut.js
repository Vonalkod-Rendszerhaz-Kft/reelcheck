/***************************************************
LogInOut.js
    A Vrh.Web.Membership bejelentkezési logikáját támogató 
    metódusok és események.
----------------------
Alapítva:
    2018.10.02.-10.09. Wittmann Antal
Módosult:
****************************************************/
/*##### EVENTS #####*/
$(function () { // $(document).ready(function () {
    'use strict';
    var thisfn = 'UserAdministration.LogInOut.js: ready event: ';
    console.log(thisfn + 'PING');
    //console.log(thisfn + 'END');
}); // $(document).ready END 
/*##### EVENTS END #####*/

/**
 * A Login/Logout kezelőfelülethez készült prototípus.
 * 
 * @param {ExportToLogInOut} imp Paraméter objektum, cshtml-ben feltöltenő paramétereket tartalmaz.
 */
function LogInOutScripts(imp) {
    'use strict';

    /*##### PROTOTYPE VARIABLES #####*/
    var me = this;  //hogy a belső függvényekben is tudjak hivatkozni a prototype tulajdonságaira
    var thispt = 'LogInOut.js: LogInOutScripts.';
    var isInLogin = false;
    /*##### PROTOTYPE VARIABLES END #####*/

    /*##### FUNCTIONS #####*/

    /** Kijelentkezéskor ez a metódus hívódik meg. */
    this.PushLogout = function () {

        var thisfn = thispt + 'PushLogout function: ';
        console.log(thisfn + 'PING');

        if (imp.Logout.UrlBeforeLogout !== '') {
            $.ajax({
                url: imp.Logout.UrlBeforeLogout,
                cache: false,
                type: "post",
                success: function (response) {
                    if (response.ReturnValue === 0) {
                        console.log(thisfn + 'Nem üzent hibát a beforelogout url.');
                        CallLogoutJSON();// ha sikeres a before logout url, akkor jöhet a kijelentkezés
                    } else {
                        console.log(thisfn + 'Hiba jött a beforelogout url-től.');
                        bootbox.alert(response.ReturnMessage);
                    }
                },
                error: function (jqXHR, exception) {
                    console.log('Calling the "' + urlBeforeLogout + '" unsuccessful! exception=', exception);
                }
            });
        }
        else {  // ha nincs before logout, akkor egyből kijelentkezik
            CallLogoutJSON();
        }

        function CallLogoutJSON() {
            var thisfn = thispt + 'CallLogoutJSON function: ';
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
                        } else {
                            if (imp.BootboxId) { // ha nem volt reload, de van ablak, akkor azt csukjuk be
                                vrhct.bootbox.hide(imp.BootboxId);
                            }
                        }
                    } else {
                        console.log(thisfn + 'LogoutJSON error!');
                        bootbox.alert(response.ReturnMessage);
                    }
                },
                error: function (jqXHR, exception) {
                    console.log("Calling the LogoutJSON action unsuccessful! exception =", exception);
                }
            });
        } // CallLogoutJSON function END

    }; //function PushLogout() END


    /** 
     * A bejelentkezést végrehajtó metódus.
     */
    this.PushLogin = function () {
        var thisfn = thispt + 'PushLogin method: ';
        if (isInLogin) {
            console.log(thisfn + 'Pressed too soon!');
            return;
        } 
        isInLogin = true;

        imp.Login.UserName = $(imp.Id.UserName).val();
        console.log(thisfn + 'START UserName="{0}"', imp.Login.UserName);
        if (!imp.Login.UserName) {
            MyAlert(imp.Message.PleaseEnterUsername);
            return;
        }

        imp.Login.Password = $(imp.Id.Password).val();
        imp.Login.Remember = $(imp.Id.Remember).is(':checked');

        if ($(imp.Id.Other).is(':checked')) {
            imp.Login.Type = imp.LoginOther.Type;
            imp.Login.UrlAfterLogin = imp.LoginOther.UrlAfterLogin;
            imp.Login.IsReloadLogin = imp.LoginOther.IsReloadLogin;
            imp.Login.IsReloadLogout = imp.LoginOther.IsReloadLogout;
        } else {
            imp.Login.Type = imp.LoginBase.Type;
            imp.Login.UrlAfterLogin = imp.LoginBase.UrlAfterLogin;
            imp.Login.IsReloadLogin = imp.LoginBase.IsReloadLogin;
            imp.Login.IsReloadLogout = imp.LoginBase.IsReloadLogout;
        }
        //console.log(thisfn + 'imp.Login', imp.Login);

        switch (imp.Login.Type) {
            case imp.LoginTypes.WebReq:
                var mess = imp.WebReq.RequestTemplate
                    .replace(imp.WebReqVars.USERNAME, imp.Login.UserName)
                    .replace(imp.WebReqVars.PASSWORD, imp.Login.Password);

                if (imp.WebReq.IsFromServer) {
                    console.log(thisfn + 'FromServer mess=%s, imp.WebReq.Url=%s', mess, imp.WebReq.Url);
                    $.ajax({
                        url: imp.Url.LoginRemote,
                        cache: false,
                        type: "post",
                        data: { url: imp.WebReq.Url, message: mess },
                        success: function (response) {
                            if (response.ReturnValue === 0) { // sikeres volt a bejelentkezés
                                console.log(thisfn + 'WebReq FromServer sucess. response', response);
                                RemoteResponseParser(response.ReturnMessage);
                            } else {
                                console.log(thisfn + 'WebReq FromServer managed error!');
                                MyAlert(response.ReturnMessage);
                            }
                        },
                        error: function () {
                            console.log(thisfn + "WebReq FromServer unsuccessful!");
                            isInLogin = false;
                        }
                    }); // imp.Url.RemoteLogin ajax END
                } else {
                    console.log(thisfn + 'FromClient mess=%s, imp.WebReq.Url=%s', mess, imp.WebReq.Url);
                    $.ajax({
                        url: imp.WebReq.Url,
                        cache: false,
                        type: "post",
                        data: mess,
                        dataType: 'text',
                        contentType: 'text/plain; charset=UTF-8',
                        success: function (response) {
                            console.log(thisfn + 'WebReq FromClient success. response', response);
                            RemoteResponseParser(response);
                        },
                        error: function (jqXHR, exception) {
                            console.log(thisfn + 'WebReq FromClient unsuccessful! exception', exception);
                            isInLogin = false;
                        }
                    }); // imp.WebReq.Url ajax END
                }
                break;

            case imp.LoginTypes.AD:
                imp.Login.UserName = imp.LoginTypes.ADPrefix + imp.Login.UserName;
                //CallLogin(imp.Login, localRoleGroup);
                isInLogin = false;
                break;

            default:    // integrated
                CallLogin(imp.Login, null); // isInLogin a funkcióban állítva
                break;
        } // switch (loginType) END

        /**
         * A távoli login akció által visszaadott válasz feldolgozása.
         * 
         * @param {string} response A távoli login által vissza adott válasz. Ha üres, akkor az hibajelzést eredményez.
         */
        function RemoteResponseParser(response) {
            if (response) {
                var parts = response.split('|');
                if (parts.length < 2) {
                    MyAlert(imp.Message.ResponseIncorrect + 'Response = "' + response + '"');
                } else {
                    var status;
                    var roleGroup;
                    var errMess;
                    for (var ix = 0; ix < parts.length; ix++) {
                        var items = parts[ix].split('=');
                        if (items.length < 2) {
                            errMess = imp.Message.ResponseIncorrect + 'Response = "' + response + '"';
                            break;
                        } else {
                            switch (items[0]) {
                                case imp.WebReqResponse.FIELD_STATUS:
                                    status = items[1];
                                    break;
                                case imp.WebReqResponse.FIELD_LEVEL:
                                    roleGroup = items[1];
                                    break;
                                case imp.WebReqResponse.FIELD_MSG:
                                    errMess = imp.Title.Response + items[1];
                                    break;
                                default:
                                    errMess = imp.Title.Response + 'Response = "' + response + '"';
                                    break;
                            }
                        }
                    }
                    if (errMess) {  // ha van hibaüzenet, akkor az kitesszük.
                        console.log('WebReq response: Error message =', errMess);
                        MyAlert(errMess);
                    } else {    // ha nincs hiba, akkor jöhet a login
                        var localRoleGroup;
                        for (var jx = 0; jx < imp.RoleGroups.length; jx++) {
                            if (imp.RoleGroups[jx][0] === roleGroup) {
                                localRoleGroup = imp.RoleGroups[jx][1];
                                break;
                            }
                        }
                        if (localRoleGroup) {
                            imp.Login.UserName = imp.LoginTypes.WebReqPrefix + imp.Login.UserName; //hozzáadjuk a típus szerinti prefixet
                            CallLogin(imp.Login, localRoleGroup); // a funkcióban az isInLogin állítva
                        } else {
                            MyAlert(imp.Message.RemoteRoleGroupNotTranslated + roleGroup);
                        }
                    }
                }
            } else {
                MyAlert(remLoginIncorrect + 'Response = "' + response + '"');
            }
        } // RemoteResponseParser Function END

        /**
         * A helyi bejelentkezést végrehajtó függvény.
         *
         * @param {Object} lo Login adatokat tartalmazó objektum
         * @param {string} lo.UserName Felhasználó neve
         * @param {string} lo.Password Felhasználó megadott jelszava
         * @param {Boolean} lo.Remember Bjelenkezéskor megadott "Emlékezz rám!" értéke
         * @param {number} lo.Type Valamelyik LoginTypes enum integer értéke
         * @param {string} lo.UrlAfterLogin Bejelentkezés után meghívandó akció URL-je, ha üres, akkor nem lesz hívás
         * @param {Boolean} lo.IsReloadLogin Sikeres bejelentkezés után történjen-e weblap újratöltés
         * @param {Booelan} lo.IsReloadLogout Sikeres kijelentkezés után történjen-e weblap újratöltés
         * @param {string} roleGroup A lokális szerepkör neve, ahova majd a felhasználót fel kell venni
         */
        function CallLogin(lo, roleGroup) {
            'use strict';

            var thisfn = thispt + 'CallLogin function: ';
            console.log(thisfn + 'roleGroup', roleGroup);
            $.ajax({
                url: imp.Url.LoginJSON,
                cache: false,
                type: "post",
                data: { username: lo.UserName, password: lo.Password, rememberme: lo.Remember, rolegroup: roleGroup },
                success: function (response) {
                    if (response.ReturnValue === 0) { // sikeres volt a bejelentkezés
                        console.log(thisfn + 'LoginJSON sucess.');
                        if (lo.UrlAfterLogin !== '') { //jöhet az afterlogin, ha van
                            $.ajax({
                                url: lo.UrlAfterLogin,
                                cache: false,
                                type: "post",
                                success: function (response) {
                                    if (response.ReturnValue === 0) {
                                        console.log(thisfn + 'Nem üzent hibát az afterlogin url.');
                                        if (lo.IsReloadLogin) {
                                            location.reload(); //vagy inkább újratöltjük a lapot
                                        } else {
                                            if (imp.BootboxId) { // ha nem volt reload, de van ablak, akkor azt csukjuk be
                                                vrhct.bootbox.hide(imp.BootboxId); 
                                            }
                                        }
                                    } else {
                                        // most hiba esetén is becsukjuk, mert nincs értelme
                                        bootbox.alert(response.ReturnMessage, function () { // csak, ha leokézta az üzenetet
                                            if (lo.IsReloadLogin) {
                                                location.reload();
                                            } else {
                                                if (imp.BootboxId) { // ha nem volt reload, de van ablak, akkor azt csukjuk be
                                                    vrhct.bootbox.hide(imp.BootboxId); 
                                                }
                                            }
                                        });
                                    }
                                },
                                error: function (jqXHR, exception) {
                                    var unsuccessAfter = imp.Message.AfterLoginUnsuccessful;
                                    console.log(unsuccessAfter + ' exception =', exception);
                                    bootbox.alert(unsuccessAfter, function () {
                                        if (imp.BootboxId) { // hibaüzenet után, ha ablak, akkor azt csukjuk be
                                            vrhct.bootbox.hide(imp.BootboxId); 
                                        }
                                    });
                                }
                            });
                        } else {  //ha nincs afterlogin
                            if (lo.IsReloadLogin) {
                                location.reload();
                            } else {
                                if (imp.BootboxId) { // // ha nem volt reload, de van ablak, akkor azt csukjuk be
                                    vrhct.bootbox.hide(imp.BootboxId);
                                }
                            }
                        }
                    } else {
                        console.log(thisfn + 'LoginJSON managed error!');
                        bootbox.alert(response.ReturnMessage);
                    }
                    isInLogin = false;
                },
                error: function () {
                    console.log(thisfn + "Calling the LoginJSON action unsuccessful!");
                    isInLogin = false;
                }
            }); // imp.Url.LoginJSON ajax END
        } // CallLogin function END

        function MyAlert(message) {
            bootbox.alert(message);
            isInLogin = false;
        }

    }; // PushLogin function END

    /*##### FUNCTIONS END #####*/

} // LogInOutScripts prototype END

/***************************************************
Menu.js
    A Vonalkód Rendszerház webes menü kerethez
    készült metódusok és események.
----------------------
Alapítva:
    2018.03.30. Wittmann Antal
Módosult:
****************************************************/

/*##### EVENTS #####*/
$(document).ready(function () {
    'use strict';

    //var ua = window.navigator.userAgent;
    var thisfn = vrhmenu.cnst.THISFILE + 'ready event: ';
    console.log(thisfn + 'PING');

    /** Selectorok beállítások itt, mert már létezik az összes objektum, akkor érdemes! */
    vrhmenu.$fullscreenicon = $('#fullscreenicon');

    // Fullscreen icon click eseményre feliratkozás 
    vrhmenu.$fullscreenicon.on('click', function () {
        vrhmenu.switchFullScreen();
    });

    if (vrhmenu.IsUseAuthentication) {
        vrhmenu.$loginouticon = $('#loginouticon'); //selector beállítás

        // loginouticon click eseményre feliratkozás
        vrhmenu.$loginouticon.on('click', function () {
            window.location.href = vrhmenu.UrlWelcome;
        });
    }
    $('#customerlogolink').attr('href', vrhmenu.UrlWelcome);

    // Resize figyelése, hogy mindig a helyes Fullscreen icon legyen kinn
    $(window).resize(function () {
        //console.log('innerHeight=', window.innerHeight, 'outerHeight=', window.outerHeight, 'screenHeight', screen.height, 'availHeight', screen.availHeight);
        vrhmenu.setFullScreenIcon();
    });

    // Ha az induláskor kell a teljes képernyő, akkor itt történik meg az átkapcsolás
    console.log(thisfn + 'vrhmenu.IsFullScreenAtStartUp', vrhmenu.IsFullScreenAtStartUp);
    if (vrhmenu.IsFullScreenAtStartUp) {
        console.log(thisfn + 'vrhct.fullscreen.isOn()', vrhct.fullscreen.isOn());
        //if (!vrhct.fullscreen.isOn()) {
        //    console.log(thisfn + 'vrhmenu.switchFullScreen() before');
        //    vrhmenu.switchFullScreen();
        //    console.log(thisfn + 'vrhmenu.switchFullScreen() after');
        //}
    }

    console.log(thisfn + 'END');
}); // $(document).ready END 
/*##### EVENTS END #####*/


function MenuScripts() {
    'use strict';

    /*##### PROPERTIES #####*/
    this.cnst = {
        THISFILE: 'Menu.js: ',
        TITLE_FULLSCREEN_OFF: '',
        TITLE_FULLSCREEN_ON: '',
    };

    this.ViewModes = {
        Desktop: 0,
        Mobile: 0,
        Touch: 0
    };
    this.ViewMode = 0;

    this.MenuBehaviors = {
        HideAlways: 0,
        HideStartUp: 0,
        VisibleAlways: 0,
        VisibleStartUp: 0
    };
    this.MenuBehavior = 0;
    this.MenuAutoHide = 0;

    /** A tartalmat eltakarja a menü vagy sem. Alapértelmezett érték: false */
    this.IsFlowOver = false;

    /* SELECTORS' CASH  ! a ready-ben érdemes beállítani ! */
    this.$fullscreenicon = '';
    this.$loginouticon = '';
    /* SELECTORS' CASH END */

    /** Annak jelzése, hogy a menükezelő használja-e a hitelesítésre vonatkozó eszközöket. */
    this.IsUseAuthentication = false;

    /** 
     * Annak a jelzése, hogy az induláskor (document.ready esemény bekövetkeztekor)
     * legyen-e automatikusan teljes képernyőre kapcsolás.
     */
    this.IsFullScreenAtStartUp = false;

    /**
     * Az üdvözlő képernyő URL-je, ami akár lehet a "/Home/Index" is.
     * Bejelentkezés és a nyelvváltás helye egyben.
     */
    this.UrlWelcome = '';

    this.MenuSwapEventFunction = null;
    /*##### PROPERTIES END #####*/


    /*##### PROTOTYPE VARIABLES #####*/
    var me = this;  //hogy a belső függvényekben is tudjak hivatkozni a prototype tulajdonságaira
    var isFullscreenISwitched = false;  // annak jelzése, hogy "én" kapcsoltam át teljes képernyőre
    /*##### PROTOTYPE VARIABLES END #####*/


    /*##### METHODS #####*/

    //===============================================
    // A teljes képernyő váltáshoz szükséges scriptek
    //===============================================
    /**
     * Meghívja a teljes képernyő átkapcsolást, és beállítja az isFullscreenISwitched
     * változót igazra, jelezve, hogy programozottan történt az átkapcsolás.
     * Ugyanis az F11 nem kapcsolható ki programból biztonsági okok miatt!
     */
    this.switchFullScreen = function () {
        vrhct.fullscreen.swap();
        isFullscreenISwitched = true;
    };

    /**
     * A teljes képernyő állapotához igazódó icont tesz fel.
     */
    this.setFullScreenIcon = function () {
        if (vrhct.fullscreen.isOn()) {
            //console.log('setFullScreenIcon state = On');
            if (isFullscreenISwitched) {
                me.$fullscreenicon.attr('class', 'px-3 fa fa-compress fa-lg').attr('title', me.cnst.TITLE_FULLSCREEN_OFF);
            }
            else {
                me.$fullscreenicon.attr('class', 'px-0').attr('title', me.cnst.TITLE_FULLSCREEN_OFF);
            }
        } else {
            //console.log('setFullScreenIcon state = Off');
            me.$fullscreenicon.attr('class', 'px-3 fa fa-expand fa-lg').attr('title', me.cnst.TITLE_FULLSCREEN_ON);
        }
        isFullscreenISwitched = false;
    };
    //====================================================
    // A teljes képernyő váltáshoz szükséges scriptek VÉGE
    //====================================================

    /*##### METHODS END #####*/

} // MenuScripts prototype END
try {
    var vrhmenu = new MenuScripts();
} catch (e) {
    console.error(e);
}


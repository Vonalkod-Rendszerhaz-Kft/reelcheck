/***************************************************
Desktop.js
    A Vonalkód Rendszerház webes menü kerethez készült metódusok és események,
    ha a 'Desktop' megjelenési módot választották.
----------------------
Alapítva:
    2018.03.30. Wittmann Antal
Módosult:
****************************************************/

/*##### EVENTS #####*/
$(document).ready(function () {
    'use strict';

    var thisfn = vrhdesktop.cnst.THISFILE + 'ready event: ';
    console.log(thisfn + 'PING');

    /**
     * !!! Selectorok beállítások itt, mert már létezik az összes objektum akkor érdemes!
     */
    vrhdesktop.$body = $('body');
    vrhdesktop.$desktopHeader = $('#desktopheader');
    vrhdesktop.$desktopFooter = $('#desktopfooter');
    vrhdesktop.$desktophidebar = $('#desktophidebar');
    vrhdesktop.$menuHideicon = $('#menuhideicon');

    var $cfbc = $('.container-fluid.body-content');
    vrhdesktop.MarginY = parseFloat($cfbc.css('margin-top')) + parseFloat($cfbc.css('margin-bottom'));
    //console.log(thisfn + 'MarginY', vrhdesktop.MarginY);

    // a menü kezdeti állapotának beállítása
    vrhdesktop.menuInit();

    // Resize figyelése, hogy mindig a helyes Fullscreen icon legyen kinn
    $(window).resize(function () {
        //console.log('innerHeight=', window.innerHeight, 'outerHeight=', window.outerHeight, 'screenHeight', screen.height, 'availHeight', screen.availHeight);
        //console.log(vrhdesktop.$desktopHeader.is(':visible'));
        if (vrhdesktop.$desktopHeader.is(':visible') && !vrhmenu.IsFlowOver) {
            vrhdesktop.$body.css('padding-top', vrhdesktop.$desktopHeader.outerHeight() + 1 + vrhdesktop.BodyTop);
            vrhdesktop.$body.css('padding-bottom', vrhdesktop.$desktopFooter.outerHeight() - vrhdesktop.MarginY);
        } else {
            vrhdesktop.$body.css('padding-top', vrhdesktop.BodyTop);
            vrhdesktop.$body.css('padding-bottom', 0);
        }
    });

    // csak akkor kapcsoljuk be a header mousemove eseményét, ha engedélyezve van az automatikus elrejtés
    if (vrhmenu.MenuAutoHide > 0) {
        console.log(thisfn + 'MenuAutoHide=', vrhmenu.MenuAutoHide);
        vrhdesktop.$desktopHeader.mousemove(function () {
            vrhdesktop.startTiming = new Date().getTime();
        });
    }

    // csak akkor kapcsoljuk be ezeket az eseményeket, ha lehet kapcsolgatni a menü megjelenését.
    if (vrhmenu.MenuBehavior === vrhmenu.MenuBehaviors.HideStartUp || vrhmenu.MenuBehavior === vrhmenu.MenuBehaviors.VisibleStartUp) {
        console.log(thisfn + 'vrhmenu.MenuBehavior=', vrhmenu.MenuBehavior, vrhdesktop.$menuHideicon);

        vrhdesktop.$menuHideicon.click(function () {       // menuHide icon click esemény 
            //console.log(vrhdesktop.cnst.THISFILE + '$menuHideicon.click event PING');
            vrhdesktop.menuSwap();
        });

        vrhdesktop.$desktophidebar.mousemove(function () { // desktop hidebar MouseMove esemény
            //console.log(vrhdesktop.cnst.THISFILE + '$desktophidebar.mousemove event PING');
            vrhdesktop.menuSwap();
        });
    }

    console.log(thisfn + 'END');
}); // $(document).ready END 
/*##### EVENTS END #####*/


function MenuDesktop() {
    'use strict';

    /*##### PROPERTIES #####*/
    this.cnst = {
        THISFILE: 'Desktop.js: '
    };

    this.startTiming = new Date().getTime();   // azért itt, mert eseményből is változik az értéke

    this.BodyTop = 0;       //WA20180331: egyelőre marad fix 0, majd a container-ben kell margót állítani
    this.MarginY;           //.container-fluid.body-content div alsó és felső margójának összege

    /* SELECTORS' CASH  ! a ready-ben érdemes beállítani ! */
    this.$menuHideicon = '';
    this.$desktopHeader = '';
    this.$desktopFooter = '';
    this.$desktophidebar = '';
    this.$body = '';
    /* SELECTORS' CASH END */

    /*##### PROPERTIES END #####*/


    /*##### PROTOTYPE VARIABLES #####*/
    var me = this;                  //hogy a belső függvényekben is tudjak hivatkozni a prototype tulajdonságra
    var isSetTimeoutOn = false;     // annak jelzése, hogy az AutoHide setTimeout metódusa aktiválva van-e.
    var animationSpeed = 200;       // milyen gyorsan csukódjon és záródjon a menü (ms)
    /*##### PROTOTYPE VARIABLES END #####*/

    /*##### METHODS #####*/

    //================================================
    // A DESKTOP menü eltüntetéshez szükséges scriptek
    //================================================

    /**
     * Elrejti a menüt a képernyőről
     */
    function menuHide() {
        var thisfn = vrhdesktop.cnst.THISFILE + 'menuHide(): ';
        //console.log(thisfn + 'PING');
        me.$desktopHeader.slideUp(animationSpeed, function () {
            me.$desktophidebar.show();  // csak akkor mutassa meg a hidebar-t, ha a header teljesen eltünt
            if (!vrhmenu.IsFlowOver) {  //ha eltakarhatja, akkor nem kell jelezni, hogy változott
                if (vrhmenu.MenuSwapEventFunction) {
                    vrhmenu.MenuSwapEventFunction();
                }
            }
        });
        me.$desktopFooter.slideUp(animationSpeed);
        me.$body.animate({ 'padding-top': me.BodyTop }, animationSpeed);
        me.$body.animate({ 'padding-bottom': 0 }, animationSpeed);
    }

    /**
     * Visszateszi a menüt a képernyőre
     */
    function menuVisible() {
        var thisfn = vrhdesktop.cnst.THISFILE + 'menuVisible(): ';
        //console.log(thisfn + 'PING');

        me.$desktophidebar.hide();  // hideBar eltüntetése

        if (!vrhmenu.IsFlowOver) {  //ha eltakarhatja, akkor nem kell padding-ot állítani
            me.$body.animate({ 'padding-top': me.$desktopHeader.outerHeight() + 1 + me.BodyTop }, animationSpeed);
            me.$body.animate({ 'padding-bottom': me.$desktopFooter.outerHeight() - me.MarginY }, animationSpeed);
            //console.log(thisfn + 'top bottom', me.$desktopHeader.outerHeight() + 1 + me.BodyTop, me.$desktopFooter.outerHeight() - me.MarginY);
        }
        me.$desktopHeader.slideDown(animationSpeed);
        me.$desktopFooter.slideDown(animationSpeed, function () {
            if (!vrhmenu.IsFlowOver) {  //ha eltakarhatja, akkor nem kell jelezni, hogy változott
                if (vrhmenu.MenuSwapEventFunction) {
                    vrhmenu.MenuSwapEventFunction();
                }
            }
        });

        if (vrhmenu.MenuAutoHide > 0) {  // ha engedélyezve van az automatikus elrejtés
            //console.log(thisfn + 'MenuAutoHide=', vrhmenu.MenuAutoHide);
            if (isSetTimeoutOn === false) {  // csak akkor indítom, ha még nem lett elindítva, a biztonság kedvéért
                setTimeout(checkHide, 1000);     //elindítom, hogy 1sec múlva ellenőrizze eltelt-e az idő
                isSetTimeoutOn = true;
            }
            me.startTiming = new Date().getTime();  //beállítom a számolás kezdetét
        }
        //$(window).trigger('resize');  //kikényszerítek egy 'resize' eseményt
    }

    /**
     * Ez a függvény ellenőrzi, hogy eltelt-e már az elrejtés késleltetési ideje.
     * Ha még nem telt el, akkor újra futtatja a timout-ot.
     */
    function checkHide() {
        var now = new Date().getTime();
        var timing = now - me.startTiming;
        //console.log(vrhdesktop.cnst.THISFILE + 'checkHide(): timing=' + timing);
        if (timing > vrhmenu.MenuAutoHide) {
            menuHide();
            isSetTimeoutOn = false;
        }
        else {
            setTimeout(checkHide, 1000);
        }
    }

    /**
     * Átkapcsolás a látható és rejtett menü között
     */
    this.menuSwap = function () {
        //console.log(vrhdesktop.cnst.THISFILE + 'menuSwap(): vrhdesktop.$desktopHeader.isVisible=', vrhdesktop.$desktopHeader.is(':visible'));
        if (vrhdesktop.$desktopHeader.is(':visible')) {
            menuHide();
        }
        else {
            menuVisible();
        }
    };

    /**
     * A MenuBehavior beállításnak megfelelően inicializálja a menü-t.
     */
    this.menuInit = function () {
        var thisfn = vrhdesktop.cnst.THISFILE + 'menuInit(): ';
        if (vrhmenu.MenuBehavior === vrhmenu.MenuBehaviors.VisibleStartUp) {
            console.log(thisfn + 'VisibleStartUp');
            menuVisible();
        }
        else if (vrhmenu.MenuBehavior === vrhmenu.MenuBehaviors.VisibleAlways) {
            console.log(thisfn + 'VisibleAlways');
            me.$menuHideicon.hide();
            menuVisible();
        }
        else if (vrhmenu.MenuBehavior === vrhmenu.MenuBehaviors.HideStartUp) {
            console.log(thisfn + 'HideStartUp');
            menuHide();
        }
        else {
            console.log(thisfn + 'HideAlways');
            me.$body.animate({ 'padding-top': me.BodyTop }, animationSpeed);
        }
    };

    //=====================================================
    // A DESKTOP menü eltüntetéshez szükséges scriptek VÉGE
    //=====================================================

    /*##### METHODS END #####*/

} // MenuDesktop prototype END
try {
    var vrhdesktop = new MenuDesktop();
} catch (e) {
    console.log(e);
}


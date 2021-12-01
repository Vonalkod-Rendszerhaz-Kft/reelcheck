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
$(function () {
    'use strict';

    var thisfn = vrhtouchscreen.cnst.THISFILE + 'ready event: ';
    console.log(thisfn + 'PING');

    /**
     * !!! Selectorok beállítások itt, mert már létezik az összes objektum akkor érdemes!
     */
    vrhtouchscreen.$body = $('#touchscreen-body');
    vrhtouchscreen.$button = $('#touchscreen-menubutton');
    vrhtouchscreen.$container = $('#touchscreen-menucontainer');
    vrhtouchscreen.$menuhideicon = $('#menuhideicon');

    vrhtouchscreen.bodyPaddingLeft = parseInt(vrhtouchscreen.$body.css('padding-left').replace(/[\D+]/g, ''));
    console.log(thisfn + 'bodyPaddingLeft', vrhtouchscreen.bodyPaddingLeft);

    // a menü kezdeti állapotának beállítása
    vrhtouchscreen.menuInit();  

    // csak akkor kapcsoljuk be a menu mousemove eseményét, ha engedélyezve van az automatikus elrejtés
    if (vrhmenu.MenuAutoHide > 0) {
        console.log(thisfn + 'MenuAutoHide=', vrhmenu.MenuAutoHide);
        vrhtouchscreen.$container.mousemove(function () {
            vrhtouchscreen.startTiming = new Date().getTime();
        });
    }

    // csak akkor kapcsoljuk be ezeket az eseményeket, ha lehet kapcsolgatni a menü megjelenését.
    if (vrhmenu.MenuBehavior === vrhmenu.MenuBehaviors.HideStartUp || vrhmenu.MenuBehavior === vrhmenu.MenuBehaviors.VisibleStartUp) {
        console.log(thisfn + 'vrhmenu.MenuBehavior=', vrhmenu.MenuBehavior, vrhtouchscreen.$menuhideicon);
        
        vrhtouchscreen.$button.on('click', function () { vrhtouchscreen.menuVisible(); });
        vrhtouchscreen.$menuhideicon.on('click', function () { vrhtouchscreen.menuHide(); });
    }

    $('div.collapse').on('hidden.bs.collapse', function () {
        var headid = $(this).attr('aria-labelledby');
        console.log(vrhtouchscreen.cnst.THISFILE + 'hidden.bs.collapse event occur: headid', headid);
        $('#' + headid).find('i').attr('class', 'fa fa-caret-down');
        return false;   // ha nincs itt a false, akkor a szülőkön is jelentkezik az esemény
    });
    $('div.collapse').on('shown.bs.collapse', function () {
        var headid = $(this).attr('aria-labelledby');
        console.log(vrhtouchscreen.cnst.THISFILE + 'shown.bs.collapse event occur: headid', headid);
        $('#' + headid).find('i').attr('class', 'fa fa-caret-up');
        return false;   // ha nincs itt a false, akkor a szülőkön is jelentkezik az esemény
    });

    console.log(thisfn + 'END');
}); // $(document).ready END 
/*##### EVENTS END #####*/


function MenuTouchScreen() {
    'use strict';

    /*##### PROPERTIES #####*/
    this.cnst = {
        THISFILE: 'TouchScreen.js: '
    };

    this.startTiming = new Date().getTime();    // azért itt, mert eseményből is változik az értéke
    this.bodyPaddingLeft = 0;                   // A body kezdeti padding-left értéke, megőrzés céljából. 

    /* SELECTORS' CASH  ! a ready-ben érdemes beállítani ! */
    this.$body = '';
    this.$button = '';
    this.$container = '';
    this.$menuhideicon = '';
    /* SELECTORS' CASH END */

    /*##### PROPERTIES END #####*/


    /*##### PROTOTYPE VARIABLES #####*/
    var me = this;                  //hogy a belső függvényekben is tudjak hivatkozni a prototype tulajdonságra
    var isSetTimeoutOn = false;     // annak jelzése, hogy az AutoHide setTimeout metódusa aktiválva van-e.
    var animationSpeed = 500;       // milyen gyorsan csukódjon és záródjon a menü (ms)
    var lastMenuWidth = 0;
    var isMenuHide = false;         // "true" annak jelzése, hogy a menü elrejtését bekapcsolták
    /*##### PROTOTYPE VARIABLES END #####*/

    /*##### METHODS #####*/

    //====================================================
    // A TOUCHSCREEN menü eltüntetéshez szükséges scriptek
    //====================================================

    /**
     * Elrejti a menüt a képernyőről, a button-t kiteszi.
     */
    this.menuHide = function () {
        var thisfn = me.cnst.THISFILE + 'menuHide(): ';
        console.log(thisfn + 'PING');

        isMenuHide = true;
        me.$container.hide(animationSpeed, function () {
            vrhtouchscreen.$button.show(1000);  //csak akkor kezdje mutatni a button-t, ha eltünt a menü
            lastMenuWidth = 0;
            if (vrhmenu.MenuSwapEventFunction !== null) {
                vrhmenu.MenuSwapEventFunction();
            }
        });
        //me.$body.css('padding-left', me.bodyPaddingLeft);
        if (!vrhmenu.IsFlowOver) {
            me.$body.animate({ 'padding-left': me.bodyPaddingLeft }, animationSpeed);
        }
    };

    /**
     * Visszateszi a menüt a képernyőre, a button-t eltünteti.
     */
    this.menuVisible = function () {
        var thisfn = me.cnst.THISFILE + 'menuVisible(): ';
        console.log(thisfn + 'PING');

        isMenuHide = false;
        me.$button.hide();  // gomb eltüntetése

        if (!vrhmenu.IsFlowOver) {
            me.$body.animate({ 'padding-left': me.$container.outerWidth() + me.bodyPaddingLeft }, animationSpeed);
        }

        me.$container.show(animationSpeed, checkMenuWidthChange);

        if (vrhmenu.MenuAutoHide > 0) {  // ha engedélyezve van az automatikus elrejtés
            //console.log(thisfn + 'MenuAutoHide=', vrhmenu.MenuAutoHide);
            if (isSetTimeoutOn === false) {  // csak akkor indítom, ha még nem lett elindítva, a biztonság kedvéért
                setTimeout(checkHide, 1000);     //elindítom, hogy 1sec múlva ellenőrizze eltelt-e az idő
                isSetTimeoutOn = true;
            }
            me.startTiming = new Date().getTime();  //beállítom a számolás kezdetét
        }
    };

    /**
     * Ez a függvény ellenőrzi, hogy eltelt-e már az elrejtés késleltetési ideje.
     * Ha még nem telt el, akkor újra futtatja a timout-ot.
     */
    function checkHide() {
        var now = new Date().getTime();
        var timing = now - me.startTiming;
        //console.log(me.cnst.THISFILE + 'checkHide(): timing=' + timing);
        if (timing > vrhmenu.MenuAutoHide) {
            me.menuHide();
            isSetTimeoutOn = false;
        }
        else {
            setTimeout(checkHide, 1000);
        }
    }

    /**
     * A MenuBehavior beállításnak megfelelően inicializálja a menü-t.
     */
    this.menuInit = function () {
        var thisfn = me.cnst.THISFILE + 'menuInit(): ';
        if (vrhmenu.MenuBehavior === vrhmenu.MenuBehaviors.VisibleStartUp) {
            console.log(thisfn + 'VisibleStartUp');
            me.menuVisible();
        }
        else if (vrhmenu.MenuBehavior === vrhmenu.MenuBehaviors.VisibleAlways) {
            console.log(thisfn + 'VisibleAlways');
            me.$menuhideicon.hide();
            me.menuVisible();
        }
        else if (vrhmenu.MenuBehavior === vrhmenu.MenuBehaviors.HideStartUp) {
            console.log(thisfn + 'HideStartUp');
            me.menuHide();
        }
        else {
            console.log(thisfn + 'HideAlways');
        }
    };


    function checkMenuWidthChange() {
        //console.log('checkMenuWidthChange: ');
        //if (me.$button.css('display') == 'none') {
        if (!isMenuHide && !vrhmenu.IsFlowOver) {
            var menuOuterWidth = me.$container.outerWidth();
            //console.log('checkMenuWidthChange: menuOuterWidth, lastMenuWidth', menuOuterWidth, lastMenuWidth);
            if (menuOuterWidth !== lastMenuWidth) {
                me.$body.css('padding-left', menuOuterWidth + me.bodyPaddingLeft);
                lastMenuWidth = menuOuterWidth;
                if (vrhmenu.MenuSwapEventFunction) {
                    vrhmenu.MenuSwapEventFunction();
                }
            }
            setTimeout(checkMenuWidthChange, 200);
        }
    }

    //=========================================================
    // A TOUCHSCREEN menü eltüntetéshez szükséges scriptek VÉGE
    //=========================================================

    /*##### METHODS END #####*/

} // MenuTouchScreen prototype END
try {
    var vrhtouchscreen = new MenuTouchScreen();
} catch (e) {
    console.log(e);
}


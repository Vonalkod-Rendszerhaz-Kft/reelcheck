/***************************************************
WebMonitor.js
    A Vonalkód Rendszerház webes megjelenítő kerethez
    készült metódusok és események.
----------------------
Alapítva:
    2017.11.11. Wittmann Antal: 
        A WebMonitor LearALM-ből való kiszedése kapcsán született.
        A Display.cshtml-ből kiemelve.
Módosult:
****************************************************/

/*##### EVENTS #####*/
$(document).ready(function () {
    'use strict';

    var thisfn = 'WebMonitor.Display.ready event: ';
    //var ua = window.navigator.userAgent;
    console.log(thisfn + 'PING vrhmenu', vrhmenu);

    WebMonitorConstantsSet();

    wmon.$fixedtop = $('.navbar.fixed-top');
    wmon.$fixedbottom = $('.navbar.fixed-bottom');
    wmon.$bodytop = $('body .navbar-fixed-top');
    wmon.$bodymain = $('body .main');
    wmon.$bodybottom = $('body .navbar-fixed-bottom');
    console.log(thisfn + '$framesSelect $wmhead IsCurrentViewModeDesktop', wmon.$framesSelect, wmon.$wmhead, wmon.IsCurrentViewModeDesktop);

    wmon.ajaxGetValues();
    wmon.startTiming = new Date().getTime();

    wmon.$framesSelect.change(function () {
        wmon.setAssemblyLineSelect();
    });

    $(window).resize(function () {
        wmon.WebMonitorResize();
    });
    vrhmenu.MenuSwapEventFunction = wmon.WebMonitorResize;  // menüváltozáskor is hívja meg a resize-t

    $('#SelectedProfileName').on('change', function () {
        wmon.ajaxGetValues();
    });

    wmon.$wmhead.mousemove(function () {
        //console.log('WebMonitor.Display: $wmhead.mousemove event');
        wmon.headNormal();
    });
}); // $(document).ready END 
/*##### EVENTS END #####*/


function WebMonitorScripts() {
    'use strict';

    /*##### PROPERTIES #####*/
    this.cnst = {
        ISHEADHIDE: false,
        TITLE_FULLSCREEN_ON: '',
        TITLE_FULLSCREEN_OFF: '',
        URL_SAVEVISIBLE: '',
        URL_GETVALUES: ''
    };
    this.startTiming = 0;
    this.$wmhead = $('#webmonitorhead');
    this.$framesSelect = $("#blocks-select");
    this.$body = $('body');
    this.$bodytop = '';
    this.$bodymain = '';
    this.$bodybottom = '';
    this.$fixedtop = '';
    this.$fixedbottom = '';
    /*##### PROPERTIES END #####*/

    /*##### SELECTORS' CASH #####*/
    var wmbody = $('#webmonitorbody');
    var wmprogressbar = $('#webmonitorprogressbar');
    var wmheadall = $('#webmonitorhead > form');
    var $assemblyLines = $("#assemblyLines");
    var $btnConfiguration = $("#btnConfiguration");
    /*##### SELECTORS' CASH END #####*/


    /*##### PROTOTYPE VARIABLES #####*/
    var me = this;
    var wmIsHeadHide = this.cnst.ISHEADHIDE; //annak jelezése, ha választó terület egyáltalán ne látszódjon
    var wmHeadVisible = true;   // a választó terület éppen látszódik-e vagy sem
    var wmdeadline = 30000; //alapesetben 30másodpercre kapcsoljuk
    var IntervalID_Minimize;

    /**/
    var profiles;       //ajax GetValues által visszaadott Profiles lista
    var actualProfile;
    var autoSwitchTime;
    var autoSwitchHandler;

    var blocksCount = 0;
    var frames = [];
    var actualFrame = [];
    var frameSwitchTime;
    var frameSwitchHandler = [];
    /*##### PROTOTYPE VARIABLES END #####*/

    /*##### METHODS #####*/
    this.WebMonitorResize = function () {
        var thisfn = 'WebMonitorResize function: ';
        var headheight = me.$wmhead.height();
        //var headcssh = me.$wmhead.css('height');
        //console.log('headheight from css = ' + headcssh);
        var top = 0;
        var bottom = 0;
        var left = parseFloat($('.container-fluid').css('padding-left').replace(/[^0-9.]/g, ''));
        var width = '100%';
        if (vrhmenu.ViewMode === vrhmenu.ViewModes.Touch) {
            width = $(window).innerWidth() - left;
        }
        else if (vrhmenu.ViewMode === vrhmenu.ViewModes.Mobile) {
            width = '100%'; // csak azért, h ne legyen üres az if block
        }
        else {
            if (me.$fixedtop.is(':visible')) {
                //top = $('body').css('padding-top');
                top = me.$fixedtop.height();
                bottom = me.$fixedbottom.height();
            }
            else {
                top = $('#desktophidebar').height();
            }
        }
        console.log(thisfn + 'left, width, top, bottom, headheight', left, width, top, bottom, headheight);
        me.$wmhead.css('left', left).css('top', top).css('width', width);
        wmbody.css('left', left).css('width', width).css('top', top + headheight).css('bottom', bottom);
    };

    this.headMinimize = function () {
        //console.log('headMinimize headVisible, wmIsHeadHide', wmHeadVisible, wmIsHeadHide);
        if (!wmIsHeadHide) {
            if (wmHeadVisible) {
                if (isMSIE()) {
                    me.$wmhead.css('height', '3px');
                }
                else {
                    me.$wmhead.css('height', '0.3vmax');
                }
                wmheadall.hide();
                me.WebMonitorResize();
                wmHeadVisible = false;
            }
        }
    }; // headMinimize END

    this.headNormal = function () {
        //console.log('headNormal headVisible, wmIsHeadHide', wmHeadVisible, wmIsHeadHide);
        if (!wmIsHeadHide) {
            if (wmHeadVisible) {
                me.startTiming = new Date().getTime();
            }
            else {
                if (isMSIE()) {
                    me.$wmhead.css('height', '40px');
                }
                else {
                    me.$wmhead.css('height', '2.2vmax');
                }
                wmheadall.show();
                me.WebMonitorResize();
                $('options').mousemove(function () { console.log('option mousemove'); });
                wmHeadVisible = true;
                me.startTiming = new Date().getTime();
            }
        }
    }; // headNormal END

    function headMinimizeCheck() {
        //clearInterval(IntervalID_Minimize);
        if (me.startTiming > 0 && wmHeadVisible === true) {
            var now = new Date().getTime();
            var timing = now - me.startTiming;
            //console.log('headMinimizeCheck timing=' + timing);
            if (timing > wmdeadline) {
                me.startTiming = 0;
                me.headMinimize();
            }
        }
    } // headMinimizeCheck END

    function clearFrameHandler() {
        for (var i = 0; i < frameSwitchHandler.length; i++) {
            clearInterval(frameSwitchHandler[i]);
        }
    }


    /**
     * Az adott profile adatainak elkérése a szervertől.
     */
    this.ajaxGetValues = function () {
        var thisfn = 'WebMonitor.Display.ajaxGetValues: ';
        console.log(thisfn + ' profilename usegroup', $('#SelectedProfileName').val(), $('#UserGroup').val());
        clearInterval(autoSwitchHandler);
        clearFrameHandler();
        $.ajax({
            cache: false,
            url: me.cnst.URL_GETVALUES,
            type: "get",
            contenttype: "application/json",
            datatype: 'json',
            data: {
                profileName: $('#SelectedProfileName').val(),
                usergroup: $('#UserGroup').val()
            },
            success: function (responseData) {
                //console.log(thisfn + 'responseData', responseData);
                if (responseData.ErrorList.length === 0) {
                    if (responseData.IsProfileGroups === "true") {
                        //console.log(thisfn + ' IsGroups:"true": responseData.AutoSwitchTimer', responseData.AutoSwitchTimer);
                        autoSwitchTime = responseData.AutoSwitchTimer * 1000;
                        actualProfile = 0;
                        profiles = responseData.Profiles;
                        if (profiles.length > 1) {
                            switchProfiles();
                        }
                        else {
                            loadValues(profiles[0], false);
                        }
                    } else {
                        //console.log(thisfn + ' IsGroups:"false": responseData.Profile.Name', responseData.Profile.Name);
                        loadValues(responseData.Profile, false);
                    }
                }
                else {
                    console.log(thisfn + 'responseErrors!');
                    for (var i = 0; i < responseData.ErrorList.length; i++) {
                        console.log(i + '. ' + responseData.ErrorList[i]);
                    }
                }
            },
            error: function (jqXHR, exception) {
                console.log(thisfn + 'Ajax hívás sikertelen! ', jqXHR.responseText);
            }
        });
    }; // ajaxGetValues method END


    /** 
     * A profiles az a struktúra, ami az ajaxGetValus-ban jön.
     */
    function switchProfiles() {
        //console.log('WebMonitor.Display.switchProfiles: actualProfile', actualProfile);
        //clearInterval(autoSwitchHandler);
        if (actualProfile >= profiles.length) {
            actualProfile = 0;
        }
        loadValues(profiles[actualProfile], true);
        actualProfile++;
        //autoSwitchHandler = setInterval(switchProfiles, autoSwitchTime);
        if (autoSwitchTime > 0) {
            autoSwitchHandler = setTimeout(switchProfiles, autoSwitchTime);
        }
    } // switchProfiles END

    function switchFrames(block) {
        $("#" + actualFrame[block]).css('display', 'none');
        for (var i = 0; i < frames[block].length; i++) {
            if (actualFrame[block] === frames[block][i]) {
                if (i + 1 === frames[block].length) {
                    actualFrame[block] = frames[block][0];
                } else {
                    actualFrame[block] = frames[block][i + 1];
                }
                break;
            }
        }
        $("#" + actualFrame[block]).css('display', 'inline');
        //var currentiframe = $("#" + actualFrame[block] + " > iframe");
        //currentiframe.src = currentiframe.src;
        frameSwitchHandler[block] = setTimeout(switchFrames, frameSwitchTime, block);
    } // switchFrames END


    /**
     * Megkap egy profile-t, és a szerint elvégzi a beállításokat.
     * @param {any} profile : ajaxGetValues-ban megszerzett profile struktúra
     * @param {any} isGroup : Egyedi (false) vagy monitor profilcsoportos (true) megjelenítés van éppen.
     */
    function loadValues(profile, isGroup) {
        var thisfn = 'WebMonitor.Display.loadValues: ';
        console.log(thisfn + 'isGroup, profile.Name', isGroup, profile.Name);

        if (!isGroup) {
            wmprogressbar.show(2000);
        } else {
            wmprogressbar.hide();
        }

        clearInterval(IntervalID_Minimize); //törölni kell, mert lehet az új profile-ban ki van kapcsolva

        var html = '';
        wmbody.html(html);

        wmbody.attr('style', profile.Style); //resize előtt kell, mert a méretet a style-ba teszi
        me.WebMonitorResize();

        //RemoveSelects();
        //AddSelects();
        hideSelects();  //frame, assemblyline és a gomb elrejtése (majd megmutatjuk, ha lesz benne elem)

        var blockOptions = '';          // frames
        me.$framesSelect.html(blockOptions);

        var blocks = profile.DisplayBlocks.length;
        for (var i = 0; i < blocks; i++) {
            var db = profile.DisplayBlocks[i];

            if (db.IsFrame) {
                frames[blocksCount] = [];
                for (var j = 0; j < db.ContentOptions.length; j++) {
                    //if (db.IsMAtrix2D) {
                    if (db.ContentOptions[j].WorkPlaceUrl !== "") {
                        html += '<div id="' + db.Frame + "-" + j + '" class="webmonitorblock" style="' + db.Style + '">'
                            + '<iframe src="' + db.ContentOptions[j].Url + '" id="blockiframe" />'
                            + '</div>';
                        frames[blocksCount].push(db.Frame + "-" + j);
                    }
                    //} else {
                    //    html += '<div id="' + db.Frame + "-" + j + '" class="webmonitorblock" style="' + db.Style + '">'
                    //                                + '<iframe src="' + db.ContentOptions[j].Url + '" id="blockiframe" />'
                    //                                + '</div>';
                    //    frames[blocksCount].push(db.Frame + "-" + j);
                    //}
                }
                actualFrame[blocksCount] = db.Frame + "-0";
                //frameSwitchHandler = setInterval(switchFrames, db.FrameAutoSwitchTimer * 1000);
                frameSwitchTime = db.FrameAutoSwitchTimer * 1000;
                frameSwitchHandler[blocksCount] = setTimeout(switchFrames, frameSwitchTime, blocksCount);
                blocksCount++;
            } else {
                // div kezdés
                html += '<div data-parameters="' + db.Parameters + '" data-preurl="' + db.PreUrl
                    + '" data-workplacelist="' + db.WorkPlaceUrlList + '" data-blockname="' + db.BlockName
                    + '" id="' + db.Id + '" class="webmonitorblock" style="' + db.Style + '"';
                if (db.IsMatrix2D) {
                    if (db.WorkPlaceUrl !== "") {
                        html += '><iframe data-divid="' + db.Id + '" src="' + db.Url + '" id="blockiframe" />';
                    } else {
                        html += ' display="none"><iframe data-divid="' + db.Id + '" src="" id="blockiframe" />';
                    }
                } else {
                    html += '><iframe data-divid="' + db.Id + '" src="' + db.Url + '" id="blockiframe" />';
                }
                // div zárás
                html += '</div>';

                if (blocks >= 1 && profile.SelectEnable_FrameContent === "true" && db.WorkPlaceUrl !== "") {
                    //$framesSelect.html($framesSelect.html() + '<option value="' + db.Id + '">' + db.Frame + '</option>');
                    blockOptions += '<option value="' + db.Id + '">' + db.Frame + '</option>';
                }
            }
        }
        wmbody.html(html);

        //ha lettek block-ok
        me.$framesSelect.html('');
        if (blockOptions !== '') {
            me.$framesSelect.html(blockOptions);
            if (profile.SelectEnable_Frame === "true") { // megmutatjuk, ha engedélyezve van
                me.$framesSelect.show();
                $btnConfiguration.show();
            }
        }


        // ha egyáltalán engedélyezve van a keret és szerelősor választás
        if (profile.SelectEnable_FrameContent === "true") {
            me.setAssemblyLineSelect();
            if ($assemblyLines.html()) {
                console.log('Keletkezett szerelősor választó lista.');
                $assemblyLines.show();
                $btnConfiguration.show();
            }
        }// if (profile.SelectEnable_FrameContent == "true") END


        //a kiválasztó terület automatikus elrejtési idejének újrabeállítása
        wmdeadline = profile.SelectionAreaAutoMinimizeTimeout;
        //console.log('wmdeadline=' + wmdeadline);
        if (wmdeadline > 0) { //ha mégsincs kitörölve, akkor visszakapcsoljuk
            //0.3 másodpercenkét ellenőrzi, hogy letelt-e már az idő (ezt lehet máskép kéne megvalósítani :S)
            IntervalID_Minimize = setInterval(function () { headMinimizeCheck(); }, 300);
        }

        if (!isGroup) {
            wmprogressbar.hide(2000);
        }
        console.log(thisfn + 'END. wmdeadline', wmdeadline);
    }// loadValues END

    this.setAssemblyLineSelect = function () {
        // !!!!!!!!!!!!!!!!!!!!!!
        //Az iframe-et tartalmazó div-ben van a szerelősor lista eltárolva,
        //az egyes elemek "¤" vagy "," jellel elválasztva, a Name és Value között ":" jel van. 
        // !!!!!!!!!!!!!!!!!!!!!!
        $assemblyLines.html('');    // kitöröljük a korábbi listát

        var selectedFrameId = '#' + me.$framesSelect.val();    // az aktuális frame 
        var asmblyLines = $(selectedFrameId).data('workplacelist'); //lekérjük a letárolt szerelősor listát
        var keyvalue;
        var ix;
        if (asmblyLines) {
            var assemblyLineOptions = '';
            if (asmblyLines.indexOf("¤") !== -1) {
                keyvalue = asmblyLines.split("¤");
                for (ix = 0; ix < keyvalue.length; ix++) {
                    if (keyvalue[ix] !== "") {
                        var tmp = keyvalue[ix].split(":");
                        assemblyLineOptions += '<option value="' + tmp[1] + '">' + tmp[0] + '</option>';
                    }
                }
            } else {
                keyvalue = options.split(",");
                for (ix = 0; ix < keyvalue.length; ix++) {
                    if (keyvalue[ix].indexOf(":") !== -1) {
                        var keyvaluepair = keyvalue[ix].split(":");
                        assemblyLineOptions += '<option value="' + keyvaluepair[0] + '">' + keyvaluepair[1] + '</option>';
                    } else {
                        assemblyLineOptions += '<option value="' + keyvalue[ix] + '">' + keyvalue[ix] + '</option>';
                    }
                }
            }
            $assemblyLines.html(assemblyLineOptions);   //hozzáadjuk az új listát
        }
    };// setAssemblyLineSelect END

    function AddSelects() {
        me.$framesSelect.appendTo("#webmonitorhead > form");
        $assemblyLines.appendTo("#webmonitorhead > form");
        $btnConfiguration.appendTo("#webmonitorhead > form");
    }

    function RemoveSelects() {
        me.$framesSelect.remove();
        $assemblyLines.remove();
        $btnConfiguration.remove();
    }

    function hideSelects() {
        me.$framesSelect.hide();
        $assemblyLines.hide();
        $btnConfiguration.hide();
    }

    function ChangeIframe(divId) {
        var theDiv = $("#" + divId);
        theDiv.html("");
        theDiv.html('<iframe id="blockiframe" src="' + theDiv.data('preurl') + "?BlockName=" + theDiv.data('blockname') + "&Workplace=" + $assemblyLines.val() + "&" + theDiv.data('parameters') + '">');
    }

    this.ChangeIframeClick = function () {
        if (me.$framesSelect.is(':visible')) {
            console.log('ChangeIframeClick.visible $framesSelect.val()', me.$framesSelect.val());
            ChangeIframe(me.$framesSelect.val());
        }
        else {
            me.$framesSelect.find('option').each(function (index) {
                console.log('ChangeIframeClick.notvisible $(this).val()', $(this).val());
                ChangeIframe($(this).val());
            });
        }
    };

    function isMSIE() {
        var ua = window.navigator.userAgent;
        //console.log('isMSIE()',ua);
        if (ua.indexOf("MSIE ") > 0 || !!ua.match(/Trident.*rv\:11\./))  // If Internet Explorer, return version number
        {
            //alert(parseInt(ua.substring(msie + 5, ua.indexOf(".", msie))));
            return true;
        }
        return false;
    }
    /*##### METHODS END #####*/

} // WebMonitorScripts prototype END
var wmon = new WebMonitorScripts();


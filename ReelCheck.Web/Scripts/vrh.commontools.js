/***************************************************
vrh.commontools.js v1.0.0
    A Vonalkód Rendszerház webes alkalmazásaihoz hasznos megoldások.

----------------------
Alapítva:
    2017.08.25. Wittmann Antal: VrhAutoComplete prototípus beépítése.
Módosult:
    2017.08.27. Wittmann Antal: VrhAutoComplete módosítása
    2018.04.12. Wittmann Antal: VrhBootbox, VrhFullscreen és a String.format megvalósításai.
****************************************************/

function VrhCommonTools() {
    'use strict';


// #######################################################################
// ### VrhAutoComplete prototype START
// #######################################################################
// A prototípust a VrhCommonTools autocomplete tulajdonsága valósítja meg.
// A jquery.autocomplete egy bizonyos kiterjesztése.
function VrhAutoComplete() {

    /**
     * Ha a query értékét megtalálja a data listában, akkor a megadott azonosítójú
     * inputba teszi a megtalált sor Value értékét. Ha nem találja meg, akkor (-1)
     * lesz az érték, amit beletesz.
     * 
     * @param {any} targid : annak az inputnak az azonosítója, amelybe majd bele kell tenni az értéket
     * @param {any} query  : ezt keressük a lista Text tulajdonságában
     * @param {any} data   : egy selectJSON szerkezetű elemeket tartalmazó lista
     */
    function QueryExistInList(targid, query, data) {
        console.log('QueryExistInList', targid, query, data);
        if (query) {
            var lcq = query.toLowerCase();
            //console.log('lcq',lcq);
            for (var i = 0; i < data.length; i++) {
                if (data[i].Text.toLowerCase() === lcq) {
                    $(targid).val(data[i].Value);
                    return;
                }
            }
        }
        $(targid).val(-1);
        return;
    }// QueryExistInList private function END


    /**
     * A listát visszaadó url, meghívása előtt behelyettesíti a filteridX inputok
     * aktuális értékét a filterX karaktersorozat helyére.
     * 
     * @param {string} url       amelyben a behelyettesítés el kell végezni
     * @param {string} query     egy szűrő érték, melyet a "filter" karaktersorozat helyére tesz
     * @param {string} filterid1 Input azonositó 1 (id)
     * @param {string} filterid2 Input azonositó 2 (id)
     * @param {string} filterid3 Input azonositó 3 (id)
     * @returns {string} A kész URL.
     */
    function UrlParser(url, query, filterid1, filterid2, filterid3) {
        const FLTR_PAR1 = '@filter1';
        const FLTR_PAR2 = '@filter2';
        const FLTR_PAR3 = '@filter2';
        const FLTR_PAR = '@filter';
        var thisfn = 'VrhAutoComplete.UrlParser: ';
        //console.log(thisfn + 'url query filterid1 filterid2 filterid3', url, query, filterid1, filterid2, filterid3);

        var _url = url;
        var inpval = '';
        if (filterid1) {
            inpval = $('#' + filterid1).val();
            if (inpval === null) inpval = '';
        }
        while (_url.includes(FLTR_PAR1)) {
            _url = _url.replace(FLTR_PAR1, inpval);
        }

        inpval = '';
        if (filterid2) {
            inpval = $('#' + filterid2).val();
            if (inpval === null) inpval = '';
        }
        while (_url.includes(FLTR_PAR2)) {
            _url = _url.replace(FLTR_PAR2, inpval);
        }

        inpval = '';
        if (filterid3) {
            inpval = $('#' + filterid3).val();
            if (inpval === null) inpval = '';
        }
        while (_url.includes(FLTR_PAR3)) {
            _url = _url.replace(FLTR_PAR3, inpval);
        }

        inpval = '';
        if (query) {
            inpval = query;
            if (inpval === null) inpval = '';
        }
        while (_url.includes(FLTR_PAR)) {
            _url = _url.replace(FLTR_PAR, inpval);
        }

        //console.log(thisfn + '_url', _url);
        return _url;
    }// UrlParser private function END


    /**
     * MultiList esetén megmutatja a kiválasztott elemeket.
     * 
     * @param {any} _url a listát előállító URL
     * @param {any} inputname az input neve
     * @param {any} inputvalue az input értéke
     */
    function showItemsOfMultiList(_url, inputname, inputvalue) {
        var thisfn = 'VrhAutoComplete.showItemsOfMultiList: ';
        console.log(thisfn + 'inputname inputvalue _url', inputname, inputvalue, _url);
        var $slct = $('#' + inputname + 'Multi');

        $('.multicontainer').off('click', ' div:last-child');
        if (inputvalue) {
            $.ajax({
                url: _url,
                type: "GET",
                cache: false,
                dataType: "json",
                success: function (data) {
                    console.log(thisfn + 'Ajax successful. response data', data);
                    var str = '';
                    var ivalue = '';
                    for (var x = 0; x < data.length; x++) {
                        //console.log('inputvalue=' + inputvalue + '; data[x].Value=' + data[x].Value);
                        if (inputvalue.includes(data[x].Value + ',')) {
                            str += '<div class="multicontainer">';
                            str += '<div>' + data[x].Text + '</div>';
                            str += '<div data-id="' + data[x].Value + '">X</div>';
                            str += '</div>';
                        }
                    }
                    $slct.html(str).show();

                    $('.multicontainer').on('click', ' div:last-child', function () {
                        var $input = $('#' + inputname);
                        var replacevalue = $(this).data('id') + ',';
                        var oldvalue = $input.val();
                        var newvalue = oldvalue.replace(replacevalue, '');
                        $input.val(newvalue);
                        console.log(thisfn + ' .multicontainer click event: inputname="' + inputname + '", oldvalue="' + oldvalue + '", replacevalue="' + replacevalue + '", newvalue="' + newvalue + '"');
                        //showItemsOfMultiList(_url, inputname, newvalue);
                        $(this).parent('.multicontainer').remove();
                    });
                },
                error: function (jqXHR, exception) {
                    console.log(thisfn + 'Ajax unsuccessful!', jqXHR.responseText);
                }
            });
        } else {
            $slct.html('').show();
        }
    }// showItemsOfMultiList private function END

    /**
     * Az egyedi választómezőket inicializálja. Általában a READY eseményben érdemes meghívni.
     *
     * p.textName   : annak az inputnak az azonosítója, melyben a lista kiválasztott elemének Text tulajdonsága kerül
     * p.valueName  : annak az inputnak az azonosítója, melyben a lista kiválasztott elemének Value tulajdonsága kerül
     * p.url        : a selectJSON struktúrát szolgáltató akció url-je
     * p.filterX    : inputok azonosítói (id), melyek értékét, majd a meghívandó akciónak kell átadni
     * p.isMultiList: jelzés, hogy a kezelt inputban több elemet is ki lehet választani
     * 
     * @param {any} p: paraméter objektum
     */
    this.init = function (p) {
        var thisfn = 'VrhAutoComplete.init: ';

        // paraméterek ellenőrzése
        if (!p.textName) throw new Error(thisfn + 'The "textName" is a required parameter!');
        if (!p.valueName) throw new Error(thisfn + 'The "valueName" is a required parameter!');
        if (!p.url) throw new Error(thisfn + 'The "url" is a required parameter!');

        // paraméterek átvétele
        var textName = p.textName;
        var valueName = p.valueName;
        var url = p.url;
        var filterid1 = p.filterid1;
        var filterid2 = p.filterid2;
        var filterid3 = p.filterid3;
        var ismultilist = typeof p.isMultiList === 'undefined' ? false : p.isMultiList;
        if (typeof ismultilist !== "boolean") ismultilist = false;

        var _listid = 'input#' + textName;
        var $list = $(_listid);
        var _targetid = 'input#' + valueName;
        var $target = $(_targetid);

        console.log(thisfn + 'textName="' + textName + '", valueName="' + valueName + '", ismultilist=' + ismultilist);
        $(_listid).autocomplete({
            source: function (request, response) {
                console.log(thisfn + 'source event: query="' + request.term + '", valueName="' + valueName + '", currentvalue=', $target.val());
                var _url = UrlParser(url, request.term, filterid1, filterid2, filterid3);
                $.ajax({
                    url: _url,
                    type: "GET",
                    dataType: "json",
                    success: function (data) {
                        console.log(thisfn + 'source event: response data=', data);
                        if (!ismultilist) {
                            QueryExistInList(_targetid, request.term, data);
                            response($.map(data, function (item) {
                                return { label: item.Text, value: item.Text, id: item.Value };
                            }));
                        }
                        else {
                            var adata = new Array();
                            var inputvalue = $(_targetid).val();
                            for (var i = 0; i < data.length; i++) {
                                if (!inputvalue.includes(data[i].Value + ',')) {
                                    adata.push({ Text: data[i].Text, Value: data[i].Value });
                                }
                            }
                            response($.map(adata, function (item) {
                                return { label: item.Text, value: item.Text, id: item.Value };
                            }));
                        }
                    }
                });
            },
            change: function (event, ui) {
                //console.log('MyAutoCompInit.change.event: multi, id', ismultilist, ui.item.id);
                //if (!ui.item) {
                //    if (!ismultilist) {
                //        $(_targetid).val(-1);
                //    }
                //}
                //else {
                //    if (ismultilist) {
                //        var mvalue = $(_targetid).val();
                //        var original = mvalue;
                //        mvalue += ui.item.id + ',';
                //        //console.log('change multi: new=' + mvalue + '; original=' + original);
                //        GetMulti(targetid, mvalue);
                //        //return false;
                //    }
                //    else {
                //        $(_targetid).val(ui.item.id);
                //    }
                //}
            },
            select: function (event, ui) {
                console.log(thisfn + 'select event: multi, id', ismultilist, ui.item.id);
                if (ismultilist) {
                    var mvalue = $target.val();
                    var original = mvalue;
                    mvalue += ui.item.id + ',';
                    console.log(thisfn + 'select event: multi - new=' + mvalue + '; original=' + original);

                    var _url = UrlParser(url, '', filterid1, filterid2, filterid3);
                    showItemsOfMultiList(_url, valueName, mvalue);
                    $target.val(mvalue);
                    this.value = '';
                    return false;
                }
                else {
                    $target.val(ui.item.id);
                }
            },
            autoFocus: true,
            open: function () {
                //$(this).autocomplete('widget').css('z-index', 99999);
                //$('.ui-autocomplete').css('height', 'auto');
                //var $input = $(this);
                //inputTop = $input.offset().top;
                //inputHeight = $input.height();
                //autocompleteHeight = $(this).autocomplete('widget').height();//$('.ui-autocomplete').height(),
                //windowHeight = $(window).height();

                //if ((inputHeight + inputTop + autocompleteHeight) > windowHeight) {
                //    $('.ui-autocomplete').css('height', (windowHeight - inputHeight - inputTop - 20) + 'px');
                // }
            },
            minLength: 0 //hány karakter esetén nyiljon meg, 0 = akkor is megnyilik, ha nincs karakter beütve
        });

        var parlist = "'" + textName + "'";
        //$list.parent('div').append('<i class="glyphicon glyphicon-search" onclick="vrhct.autocomplete.onSearch(' + parlist + ')"></i>');
        $list.parent('div').append('<i class="glyphicon glyphicon-search"></i>');
        $list.parent('div').on('click', 'i', function myfunction() {
            $list.val('');
            $list.autocomplete("search", "");
            $list.trigger("focus");
        });

        if (ismultilist) {
            var multiname = valueName + 'Multi';
            $list.parent('div').append('<div style="display:table-cell;" id="' + multiname + '"></div>');

            var _url = UrlParser(url, '', filterid1, filterid2, filterid3);
            showItemsOfMultiList(_url, valueName, $target.val());
        }
    };//init public method END
}
this.autocomplete = new VrhAutoComplete();
// ##############################################################
// ### VrhAutoComplete prototype END
// ##############################################################

// #######################################################################
// ### VrhBootbox prototype START
// #######################################################################

/**
 * A prototípus a bootstrap.bootbox plugin kiterjesztése.
 * 
 * @description Készült: 2018.04.10 Wittmann Antal : A bootboxAction.js átemelése a VrhCommonTools-ba 'bootbox' néven.
 *                                                   A paraméterekből 'options' nevű objektum paraméter lett.
 *              Módosult:
 */
function VrhBootbox() {

    /*##### PROTOTYPE VARIABLES #####*/
    var me = this;                  //hogy a belső függvényekben is tudjak hivatkozni a prototype tulajdonságaira
    var thispt = "VrhBootbox prototype: ";
    /*##### PROTOTYPE VARIABLES END #####*/


    /*##### BUILT'N PROTOTYPE #####*/
    /** "edit" method paraméter objektuma */
    function ajaxOptions() {
        this.url = new String();
        this.data = null;
    }

    function EditorOptions() {
        this.ajax = new ajaxOptions();
        this.buttons = {};
        this.buttonsDefault = true;
        this.formid = new String();
        this.title = new String();
        this.size = new String();
        this.style = new String();
        this.shown = null;
        this.hidden = null;
    }

    function DeleteOptions() {
        this.ajax = new ajaxOptions();
        this.confirm = new String();
        this.title = new String();
        this.size = new String();
        this.style = new String();
        this.shown = null;
        this.hidden = null;
        this.success = null;
    }
    /*##### BUILT'N PROTOTYPE END #####*/


    /*##### PROPERTIES #####*/
    this.LastDialog = '';

    /**
     * bootbox.show metódus beállításai alapértelmezett értékekkel.
     *
     * @typedef {object} ShowOptions bootbox.show metódus beállításai alapértelmezett értékekkel.
     * @property {string} [ShowOptions.ajaxType='get'] A meghívandó akció típusa. Alapértelmezés: 'get'.
     * @property {boolean} [ShowOptions.isVisibleButtons] Ha true, akkor megjelenik egy 'OK' és egy 'Cancel' gomb.
     * @property {string} [ShowOptions.cancelLabel='Cancel'] Az 'Cancel' gomb címkéje (felirata).
     * @property {simpleCallback} [ShowOptions.cancelCallBack] A 'Cancel' gomb megnyomásakor meghívandó függvény.
     * @property {string} [ShowOptions.okLabel='OK'] Az 'OK' gomb címkéje (felirata).
     * @property {simpleCallback} [ShowOptions.okCallback] Az 'OK' gomb megnyomásakor meghívandó függvény.
     * @property {string} [ShowOptions.saveLabel='Save'] A 'Save' gomb címkéje (felirata).
     * @property {string} [ShowOptions.noLabel='No'] A 'No' gomb címkéje (felirata).
     * @property {string} [ShowOptions.yesLabel='Yes'] A 'Yes' gomb címkéje (felirata).
     * @property {string} [ShowOptions.pleaseWaitMessage=null] Ha nem null az értéke, akkor az akció hívása előtt megjelenik egy ablak ezzek az üzenettel.
     * @property {boolean} [ShowOptions.isReturnInfoJSON=false] Ha true, akkor egy ReturnInfoJSON struktúrát vár, melyet saját magán megjelenít.
     * @property {number} [ShowOptions.autoClose=0] A megjelenő párbeszéd ablak csukódjon be egy idő után. <=0 esetén nem zárul be automatikusan, ez az alapértelmezés.
     * @property {string} [ShowOptions.autoCloseMessage=null] Ha automatikusas csukódik az ablak, akkor a jobb alsó sarokban megjeleníthető egy visszaszámláló üzenet.
     * @property {string[]} [ShowOptions.styles=null] Beállítható a bootstrap alapértelmezéstől eltérő stílus. Ha null, akkor marad a bootstrap alapértelmezés.
     * @property {modalCallback} [ShowOptions.shown=null] Az ablak megjelenítése után meghívandó függvény.
     * @property {simpleCallback} [ShowOptions.hidden=null] Az ablak elrejtése után meghívandó függvény.
     */
    this.DefaultOptions = {
        ajaxType: 'get',
        isVisibleButtons: false,
        cancelLabel: 'Cancel',
        cancelCallBack: null,
        okLabel: 'OK',
        okCallback: null,
        saveLabel: 'Save',
        noLabel: 'No',
        yesLabel: 'Yes',
        pleaseWaitMessage: null,
        isReturnInfoJSON: false,
        autoClose: 0,
        autoCloseMessage: null,
        shown: null,
        hidden: null,
        styles: []
    };
    /*##### PROPERTIES END #####*/

    /*##### PRIVATE FUNCTIONS #####*/

    /**
     * A VrhBootbox belső függvénye.
     * Egyedi azonosító előállítása a megnyílt párbeszédablak számára.
     * 
     * @returns {string} Az egyedi azonosító az ablak számára.
     */
    function bootboxUniqueID() {
        function chr4() {
            return Math.random().toString(16).slice(-4);
        }
        return chr4() + chr4() + chr4() + chr4() + chr4() + chr4() + chr4() + chr4();
    }

    /*##### PRIVATE FUNCTIONS END #####*/


    /*##### METHODS #####*/

    // SHOW ---------------------------------------------------------------------------------------------------------------------------------------------
    /**
     * A függvény alkalmas arra, hogy az átadott "actionUrl" akciót egy bootbox.dialog ablakban megnyissa.
     * A függvény generál egy egyedi azonosítót, mellyel megjelöli a dialog ablakot.
     * A '$('#' + bootboxid)' utasítással a dialog ablak elérhető.
     * A '$('#' + bootboxid + '-body') utasítással közvetlen a belsőt tartalmazó div érhető el.
     * A meghívandó akció 'bootboxid' nevű paraméterben tudja átvenni ezt az azonosítót.
     * Az ablak bezárható a "vrhct.bootbox.hide(bootboxid)" függvény hívással, vagy
     * a "$('#' + bootboxid).modal('hide')" utasítással.
     *
     * A jobb felső sarokban akkor is megjelenik az 'X', ha ez 'isVisibleButtons' paraméter false.
     * FONTOS! Ha nem gondoskodik a futás a bezárásról, akkor ne takarja el semmi az 'X'-et!
     * 
     * @param {string} actionUrl Az akció url címe, amelynek eredményét az ablakban meg kell jeleníteni.
     * @param {ShowOptions} options Objektum, a párbeszédablakra vonatkozó beállítások vannak benne.
     */
    this.show = function (actionUrl, options) {
        "use strict";

        var thisfn = thispt + 'show method: ';
        if (!actionUrl) {
            bootbox.alert('The Url parameter is required for bootbox.show()!');
            return;
        }
        var ops = {};
        $.extend(true, ops, me.DefaultOptions);   // DefaultOptions átmásolása az ops objektumba úgy, hogy ne referencia menjen át (clone)
        //console.log(thisfn + 'ops before', ops);
        if (options) {
            $.extend(true, ops, options);
        }
        //console.log(thisfn + 'ops after', ops);

        var bootboxid = bootboxUniqueID();
        var prefix = actionUrl.indexOf('?') !== -1 ? '&' : '?';
        var url = actionUrl + prefix + 'bootboxid=' + bootboxid;
        console.log(thisfn + ' url', url);
        var autoCloseTimeout;

        if (ops.pleaseWaitMessage) {
            var diagwait = me.wait(ops.pleaseWaitMessage);
            diagwait.on('shown.bs.modal', function (e) {
                //console.log('Most kész! e',e);
                callAction(url, diagwait);
            });
        } else {
            callAction(url, diagwait);
        }

        /**
         * A show metódus saját függvénye. Elvégzi az akció meghívását, melynek 
         * eredménye az ablakban fog látszani.
         * 
         * @param {any} url     : Az akció URL-je, mely eredményezi a tartalmat.
         * @param {any} diagwait: A 'Kérem várjon...' dialog. Az akció válasza után be kell csukni, ha megnyílt.
         */
        function callAction(url, diagwait) {
            var thisfn = thispt + 'callAction function: ';
            $.ajax({
                cache: false,
                url: url,
                type: ops.ajaxType,
                contenttype: 'application/json',
                datatype: 'json',
                data: null,
                success: function (responseData) {
                    if (diagwait) {
                        diagwait.modal('hide');
                    }

                    if (!!responseData[0] && responseData[0].ErrorMessage) {
                        bootbox.alert(responseData[0].ErrorMessage);
                    } else {
                        var buttons = null;
                        if (ops.isVisibleButtons) {
                            buttons = {
                                ok: {
                                    label: ops.okLabel,
                                    className: 'btn btn-success bootboxAction-btn-ok',
                                    callback: ops.okcallback
                                },
                                cancel: {
                                    label: ops.cancelLabel,
                                    className: 'btn btn-secondary bootboxAction-btn-cancel',
                                    callback: ops.cancelcallback
                                }
                            };
                        }
                        var messageDialog = '';
                        if (ops.isReturnInfoJSON) {
                            if (responseData.ReturnValue != 0) {
                                messageDialog += '<div>' + responseData.ReturnValue + '</div>';
                            }
                            messageDialog += '<div>' + responseData.ReturnMessage + '</div>';
                        } else {
                            messageDialog = responseData;
                        }
                        var dialog = bootbox.dialog({
                            show: false,
                            message: messageDialog,
                            onEscape: function (event) {
                                dialog.modal('hide');
                            },
                            buttons: buttons
                        });
                        dialog.attr('id', bootboxid);
                        var $newdialog = $('#' + bootboxid);
                        $newdialog.find('.bootbox-body').attr('id', bootboxid + '-body');
                        if (ops.styles) {
                            var $mdialog = $newdialog.find('.modal-dialog');
                            var $mcontent = $newdialog.find('.modal-content');
                            //console.log(thisfn + 'styles $mod', ops.styles, $mdialog);
                            for (var iy = 0; iy < ops.styles.length; iy++) {
                                var nam = ops.styles[iy].name;
                                var val = ops.styles[iy].value;
                                console.log(thisfn + 'set dialog style: name value', nam, val);
                                if (nam && val) {
                                    if (nam.toLowerCase().indexOf("width") >= 0) {
                                        $mdialog.css('max-width', val);
                                    } else {
                                        $mcontent.css(nam, val);
                                    }
                                }
                            }
                            //console.log('bootboxAction: set dialog size', dialogSize);
                            //dialog.find('.modal-dialog').css('width', dialogSize);
                        }
                        if (ops.autoClose > 0) {    // ha engedélyezték a párbeszédablak automatikus bezárását
                            var $body = $newdialog.find('.modal-body');
                            var autoCloseId = 'autoclose-' + bootboxid;
                            $body.append(''
                                + '<div class="d-flex justify-content-end" style="margin-bottom:-1rem;font-size:.8rem;" id="' + autoCloseId + '">'
                                + '<a class="font-italic text-secondary pt-1"></a>'
                                + '<button class="btn btn-secondary btn-sm py-0 px-1 ml-1" style="opacity:.7;">' + ops.cancelLabel + '</button>'
                                + '</div>'
                            );
                            $newdialog.on('shown.bs.modal', function (e) {
                                var $divMess = $body.find('#' + autoCloseId);
                                setAutoClose($newdialog, $divMess.find('a'));
                                $divMess.find('button').on('click', function () {
                                    clearTimeout(autoCloseTimeout);
                                    ops.autoClose = 0;
                                    //$divMess.html('');
                                    $divMess.remove();
                                });
                            });
                            $newdialog.on('hidden.bs.modal', function (e) {
                                clearTimeout(autoCloseTimeout);
                                ops.autoClose = 0;  // ha netán időközben becsukják, akkor lenullázzuk az időt
                            });
                        }
                        if (ops.shown) {
                            $newdialog.on('shown.bs.modal', ops.shown(bootboxid));
                        }
                        if (ops.hidden) {
                            $newdialog.on('hidden.bs.modal', ops.hidden);
                        }
                        $newdialog.modal('show');
                        me.LastDialog = dialog;
                    }
                },
                error: function (jqXHR, exception) {
                    if (diagwait !== null) diagwait.modal('hide');
                    console.log(thisfn + 'Ajax hívás sikertelen: ', exception, jqXHR.responseText);
                }
            });
        } // callAction private function END

        /**
         * A show metódus saját függvénye.
         * Ha igényelték az időzített bezárást, akkor ez figyeli az idő elteltét.
         * Ha letelt az idő, akkor bezárja a párbeszédablakot.
         * 
         * @param {any} $newdialog: Az éppen megnyílt párbeszédablak jQuery objektuma.
         * @param {any} $acMessage: Az letelt időt jelző felirat jQuery objektuma.
         */
        function setAutoClose($newdialog, $acMessage) {
            //console.log('setAutoClose: autoClose $newdialog $acMessage', ops.autoClose, $newdialog, $acMessage);
            if (ops.autoClose > 0) { // ha még nem telt le az idő, akkor írjuk ki, csökkentsük és nézzük meg 1 másodperc múlva
                // ha van üzenet annak beillesztése
                if (ops.autoCloseMessage) $acMessage.html(ops.autoCloseMessage.format(ops.autoClose));
                ops.autoClose--;
                autoCloseTimeout = setTimeout(function () { setAutoClose($newdialog, $acMessage); }, 1000);
            } else {    // ha letelt, akkor csukjuk be
                $newdialog.modal('hide');
            }
        } // setAutoClose private function END

    }; // show method END
    // SHOW END -------------------------------------------------------------------------------------------------------------------------------------------


    // EDIT -----------------------------------------------------------------------------------------------------------------------------------------------
    /**
     * A függvény alkalmas arra, hogy az átadott "url" akció eredményét egy bootbox.dialog ablakban megmutassa/megnyissa.
     * A függvény generál egy egyedi azonosítót, mellyel megjelöli a dialog ablakot. A '$('#' + bootboxid)' utasítással a dialog ablak elérhető.
     * A '$('#' + bootboxid + '-body') utasítással közvetlen a belsőt tartalmazó div érhető el. 
     * A meghívandó akció 'bootboxid' nevű paraméterben tudja átvenni ezt az azonosítót.
     * Az ablak bezárható a "vrhct.bootbox.hide(bootboxid)" függvény hívással, vagy a "$('#' + bootboxid).modal('hide')" utasítással.
     * A "shown" paraméter akkor is meghívódik, ha hibával tér vissza a GET. A hibaüzenet bezárásakor fog végrehjatódni.
     * 
     *
     * @param {EditorOptions} options Egy objektum, melyben az edit párbeszédablakra vonatkozó beállítások vannak benne.
     * @param {string} options.formid Annak a formnak az azonosítója, amelyet a párbeszédablakban megjelenítünk.
     * @param {Object} options.ajax Az ajax híváshoz szükséges értékek objektuma.
     * @param {string} options.ajax.url Az URL, amely felépíti az ablak belsejét.
     * @param {Object} options.ajax.data Az akciónak küldendő adatok.
     * @param {Object[]} [options.buttons=[]] Az 'OK' és 'Cancel' gombokon túli gombok, amelyeket az ablakba teszünk. Azonosítójuk: bootboxid-buttons.name
     * @param {Boolean} [options.buttonsDefault=true] Az alapértelmezett gombok ('OK' és 'Cancel') hozzáadódjanak-e a gombokhoz.
     * @param {string} [options.title=null] A párbeszéd ablak címe, fejlécének szövege.
     * @param {string} [options.size=null] A bootbox szerinti 'large' vagy 'show', de ha ez akkor egy szám érték, mely pixelben jelenti az ablak szélességét.
     * @param {string} [options.style=null] Egyedi stílus beállítás az ablak számára.
     * @param {function} [options.shown=null] Az ablak megjelenítése után meghívandó függvény. 
     * @param {function} [options.hidden=null] Az ablak elrejtése után meghívandó függvény.
     */
    this.edit = function (options) {
        var thisfn = thispt + 'edit method: ';
        try {
            console.log(thisfn + 'options', options);
            //kapott paraméterek ellenőrzése
            if (!options) throw new 'The "options" parameter is required!';
            if (!options.ajax) throw new 'The "ajax" parameter is required!';
            if (!options.ajax.url) throw new 'The "ajax.url" parameter is required!';

            var ops = new EditorOptions();
            if (jQuery.type(options.buttonsDefault) === 'boolean') ops.buttonsDefault = options.buttonsDefault;
            if (!options.formid && ops.buttonsDefault) throw new 'The "formid" parameter is required!';
            ops.formid = options.formid;
            ops.ajax = options.ajax;
            if (options.title) ops.title = options.title;
            if (options.size) ops.size = options.size;
            if (options.style) ops.style = options.style;

            if (options.buttons) ops.buttons = options.buttons;
            for (var ix = 0; ix < ops.buttons.length; ix++) {
                console.log(thisfn + 'ops.button[' + ix + ']', ops.buttons[ix]);
            }
            if (options.shown) ops.shown = options.shown;
            if (options.hidden) ops.hidden = options.hidden;


            if (ops.buttonsDefault) { // standard gombok hozzáadása, ha nem tiltották meg
                ops.buttons.OK = {
                    label: vrhct.bootbox.DefaultOptions.saveLabel,
                    className: 'btn btn-success',
                    callback: function () {
                        if (ops.formid) {
                            $('#' + ops.formid).submit();
                            return false;
                        }
                    }
                };
                ops.buttons.Cancel = {
                    label: vrhct.bootbox.DefaultOptions.cancelLabel,
                    className: 'btn btn-secondary',
                    callback: function () { }
                };
            }


            // bootboxid megállapítása
            var bootboxid = bootboxUniqueID();
            var prefix = ops.ajax.url.indexOf('?') !== -1 ? '&' : '?';
            ops.ajax.url += prefix + 'bootboxid=' + bootboxid;

            callEditAction(ops);
        } catch (e) {
            console.error(thisfn, e);
        }

        /**
         * A edit metódus saját függvénye. Elvégzi az akció meghívását, melynek 
         * eredménye az ablakban fog látszani.
         * 
         * @param {EditorOptions} ops Az akció URL-je, mely eredményezi a tartalmat.
         * @param {Object} diagwait A 'Kérem várjon...' dialog objektum. Az akció válasza után be kell csukni, ha megnyílt.
         */
        function callEditAction(ops) {
            var thisfn = thispt + 'callEditAction function: ';
            console.log(thisfn + ' ops', ops);
            $.ajax({
                cache: false,
                url: ops.ajax.url,
                type: 'get',
                contenttype: 'application/json',
                datatype: 'json',
                data: ops.ajax.data,
                success: function (responseData) {
                    if (!!responseData[0] && responseData[0].ErrorMessage) {
                        if (ops.shown) {
                            bootbox.alert(responseData[0].ErrorMessage, function() {
                                ops.shown(bootboxid);
                            });
                        }
                        else {
                            bootbox.alert(responseData[0].ErrorMessage);
                        }
                    } else {
                        var width = null;
                        var ssize = null;
                        if (ops.size) {
                            if (ops.size.toLowerCase() === 'large') {
                                ssize = 'large';
                            } else if (ops.size.toLowerCase() === 'small') {
                                ssize = 'small';
                            } else {
                                width = parseInt(ops.size);
                            }
                        }
                        var dialog = bootbox.dialog({
                            title: ops.title,
                            show: false,
                            size: ssize,
                            message: responseData,
                            onEscape: function (event) {
                                dialog.modal('hide');
                            },
                            buttons: ops.buttons
                        });
                        dialog.attr('id', bootboxid);
                        var $newdialog = $('#' + bootboxid);
                        $newdialog.find('.bootbox-body').attr('id', bootboxid + '-body');
                        if (width) {
                            $newdialog.find('.modal-dialog').css('max-width', width);
                        }
                        if (ops.styles) {
                            $newdialog.find('.modal-content').attr('style', ops.styles);
                        }
                        $newdialog.on('shown.bs.modal', function () {
                            var $focus = $(this).find('[autofocus]');
                            console.log(thisfn + '1.try $focus', $focus);
                            if ($focus.length) {
                                $focus.focus();
                            } else {
                                setTimeout(function () {
                                    var $focus = $(this).find('[autofocus]');
                                    console.log('2.try $focus', $focus);
                                    $focus.focus();
                                },300);
                            }
                            if (ops.shown) {
                                ops.shown(bootboxid);
                            }
                        });
                        if (ops.hidden) {
                            $newdialog.on('hidden.bs.modal', ops.hidden);
                        }
                        $newdialog.modal('show');
                        me.LastDialog = dialog;
                    }
                },
                error: function (jqXHR, exception) {
                    console.log(thisfn + 'Ajax hívás sikertelen: ', exception, jqXHR.responseText);
                }
            });
        } // callAction private function END
    };
    // EDIT END ---------------------------------------------------------------------------------------------------------------------------------------------

    // DELETE------------------------------------------------------------------------------------------------------------------------------------------------
    /**
     * A metódus megjelenít egy párberszédablakot egy az 'options.confirm'-ban megfogalmazott 
     * megerősítő üzenettel. A fejlécben az 'options.title' fog megjelenni.
     * Ha a megerősítő kérdésre igen a válasz, akkor az 'options.ajax.url'-ben megadott akciót indítja el, 
     * a melynek az 'options.ajax.data' objektum lesz elküldve. A meghívott akciónak RetrunInfoJSON
     * struktúrával kell visszatérnie.
     * Az 'options.callback'-ben megadott függvény, a sikeres törlés után fog végrehajtódni.
     * 
     * @param { DeleteOptions } options Egy objektum, melyben az delete párbeszédablakra vonatkozó beállítások vannak benne.
     * @param { Object } options.ajax Az ajax híváshoz szükséges értékek objektuma.
     * @param { string } options.ajax.url Az URL, amely elvégzi a törlést és RetrunInfoJSON struktúrával tér vissza.
     * @param { Object } [options.ajax.data = null] Az akciónak küldendő adatok.
     * @param { string } options.confirm A megerősítést kérő üzenet szövege.
     * @param { string } [options.title = null] A párbeszéd ablak címe, fejlécének szövege.
     * @param { string } [options.size = null] A bootbox szerinti 'large' vagy 'show', de ha nem ezek, akkor egy szám érték, mely pixelben jelenti az ablak szélességét.
     * @param { string } [options.style = null] Egyedi stílus beállítás az ablak számára.
     * @param {function} [options.shown=null] Az ablak megjelenítése után meghívandó függvény.
     * @param {function} [options.hidden=null] Az ablak elrejtése után után meghívandó függvény.
     * @param {function} [options.success=null] Sikeres törlés után meghívandó függvény.
    */
    this.delete = function (options) {
        var thisfn = thispt + 'delete method: ';
        try {
            console.log(thisfn + 'options', options);
            //kapott paraméterek ellenőrzése
            if (!options) throw new 'The "options" parameter is required!';
            if (!options.ajax) throw new 'The "ajax" parameter is required!';
            if (!options.ajax.url) throw new 'The "ajax.url" parameter is required!';
            if (!options.confirm) throw new 'The "confirm" parameter is required!';

            var ops = new DeleteOptions();
            ops.ajax = options.ajax;
            ops.confirm = options.confirm;
            if (options.title) ops.title = options.title;
            if (options.size) ops.size = options.size;
            if (options.style) ops.style = options.style;
            if (options.shown) ops.shown = options.shown;
            if (options.hidden) ops.hidden = options.hidden;
            if (options.success) ops.success = options.success;

            // méret beállítása
            var width = null;
            var ssize = null;
            if (ops.size) {
                if (ops.size.toLowerCase() === 'large') {
                    ssize = 'large';
                } else if (ops.size.toLowerCase() === 'small') {
                    ssize = 'small';
                } else {
                    width = parseInt(ops.size);
                }
            }

            // bootboxid megállapítása
            var bootboxid = bootboxUniqueID();
            //var prefix = ops.ajax.url.indexOf('?') !== -1 ? '&' : '?';
            //ops.ajax.url += prefix + 'bootboxid=' + bootboxid;
            ops.ajax.data.bootboxid = bootboxid;

            var dialog = bootbox.dialog({
                title: ops.title,
                show: false,
                size: ssize,
                message: options.confirm,
                onEscape: function (event) {
                    dialog.modal('hide');
                },
                buttons: {
                    ok: {
                        label: me.DefaultOptions.yesLabel,
                        className: 'btn btn-danger mr-auto',
                        callback: function () {
                            $.ajax({
                                cache: false,
                                url: ops.ajax.url,
                                type: 'post',
                                contenttype: 'application/json',
                                datatype: 'json',
                                data: ops.ajax.data,
                                success: function (response) {
                                    if (response.ReturnValue !== 0) {
                                        bootbox.alert({
                                            size: 'large',
                                            message: response.ReturnMessage,
                                            callback: function () {}
                                        });
                                    }
                                    else {
                                        //console.log(thisfn + ' Sikeres törlés. ops.success', ops.success);
                                        if (ops.success) ops.success();
                                    }
                                },
                                error: function (jqXHR, exception) {
                                    console.log(thisfn + 'Ajax hívás sikertelen: ', exception, jqXHR.responseText);
                                }
                            });
                        }
                    },
                    cancel: {
                        label: me.DefaultOptions.noLabel,
                        className: 'btn btn-secondary'
                    }
                }
            });
            dialog.attr('id', bootboxid);
            var $newdialog = $('#' + bootboxid);
            $newdialog.find('.bootbox-body').attr('id', bootboxid + '-body');
            if (width) {
                $newdialog.find('.modal-dialog').css('max-width', width);
            }
            if (ops.styles) {
                $newdialog.find('.modal-content').attr('style', ops.styles);
            }
            if (ops.shown) {
                $newdialog.on('shown.bs.modal', ops.shown(bootboxid));
            }
            if (ops.hidden) {
                $newdialog.on('hidden.bs.modal', ops.hidden);
            }
            $newdialog.modal('show');
            me.LastDialog = dialog;
        } catch (e) {
            console.error(thisfn, e);
        }
    };
    // DELETE END -------------------------------------------------------------------------------------------------------------------------------------------

    // HIDE -------------------------------------------------------------------------------------------------------------------------------------------------
    /**
     * A megadott bootboxid-val rendelkező párbeszéd ablak bezárása.
     * 
     * @param {string} bootboxid A párbeszédablak azonosítója, melyet a show() vagy az edit() gyártott le.
     */
    this.hide = function (bootboxid) {
        //console.log(thispt + 'hide method: bootboxid="' + bootboxid + '"');
        $('#' + bootboxid).modal('hide'); 
    };
    // HIDE END ---------------------------------------------------------------------------------------------------------------------------------------------


    // WAIT -------------------------------------------------------------------------------------------------------------------------------------------------
    /**
     * Egy ablakot jelenít meg a megkapott üzenettel, ami előtt várakozásra utaló animáció látszik.
     * 
     * @param {string} waitMessage : Ha üres, akkor a 'Please wait a moment ...' üzenet jelenik meg.
     * @param {function} shownCallback : Ha nem üres, akkor a megadott funkciót 'shown.bs.modal' esemény után futtatja.
     *   
     * @return {bootbox.dialog} : Egy bootbox.dialog() típus.
     */
    this.wait = function (waitMessage, shownCallback = null) {
        if (!waitMessage) waitMessage = 'Please wait a moment ...';

        var diagwait = bootbox.dialog({ message: '<div class="text-center"><i class="fa fa-spin fa-spinner"></i> ' + waitMessage + '</div>' });
        if (shownCallback) {
            diagwait.on('shown.bs.modal', function (e) {
                //console.log(thispt + 'wait method: shownCallback', shownCallback);
                diagwait.off('shown.bs.modal');
                if (shownCallback) shownCallback();
            });
        }
        return diagwait;
    };
    // WAIT END ---------------------------------------------------------------------------------------------------------------------------------------------
}
/**
 * @callback simpleCallback 
 */

/**
 * @callback modalCallback
 * @param {string} bootboxid A bootbox ablakhoz rendelt egyedi azonosító.
 */
this.bootbox = new VrhBootbox();
// #######################################################################
// ### VrhBootbox prototype END
// #######################################################################
//var x = new VrhBootbox();
//x.show(o, {
//    h
//});

// #######################################################################
// ### VrhFullscreen prototype START
// #######################################################################
/**
 * A prototípus metódusokat tartalmaz, amelyek a teljes képernyős üzemmód
 * használatához hasznosak.
 */
function VrhFullscreen() {
    this.isOn = function () {
        var wh = window.outerHeight;
        var sh = screen.height;
        return wh === sh;
    };

    this.swap = function () {
        var doc = window.document;
        var docEl = doc.documentElement;

        var requestFullScreen = docEl.requestFullscreen || docEl.mozRequestFullScreen || docEl.webkitRequestFullScreen || docEl.msRequestFullscreen;
        var cancelFullScreen = doc.exitFullscreen || doc.mozCancelFullScreen || doc.webkitExitFullscreen || doc.msExitFullscreen;

        if (!doc.fullscreenElement && !doc.mozFullScreenElement && !doc.webkitFullscreenElement && !doc.msFullscreenElement) {
            requestFullScreen.call(docEl);
        }
        else {
            cancelFullScreen.call(doc);
        }
    };
}
/** A vrh.commontools teljes képernyős szolgáltatásai ezen keresztül érhetőek el. */
this.fullscreen = new VrhFullscreen();
// #######################################################################
// ### VrhFullscreen prototype END
// #######################################################################

// #######################################################################
// ### VrhMasterData prototype START
// #######################################################################

/**
 * A prototípus hasznos eszközöket tartalmaz a jquery.datatables pluginnal 
 * végzett adalistázáshoz és az abból hivandó adatkezelési formokhoz.
 * */
function VrhMasterData() {
    'use strict';

    /*##### PROTOTYPE VARIABLES #####*/
    var me = this;                  //hogy a belső függvényekben is tudjak hivatkozni a prototype tulajdonságaira
    var thispt = "VrhMasterData prototype: ";
    /*##### PROTOTYPE VARIABLES END #####*/

    /*##### METHODS #####*/

    /**
     * A 'parentSelector' alatt megtalált 'toSelector' elemeinek oszályához
     * hozzáadja a 'className' értékét.
     * 
     * @param {string} parentSelector Annak a hordozó elemnek (container) a jQuery selectora, amelyekben az 'toSelector' elemek elhelyezkednek.
     * @param {any} [toSelector='input'] Azoknak az elemeknek a selectora, amelyekhez hozzá kell adni az osztályt.
     * @param {any} [className='form-control'] Az osztály[ok], amely[ek]et hozzá kell adni az elemekhez.
     */
    this.addClass = function (parentSelector, toSelector, className) {
        var thisfn = thispt + 'addClass method: ';
        try {
            console.log(thisfn + 'parentSelector, toSelector, className', parentSelector, toSelector, className);

            var cln = className || 'form-control';
            var sel = toSelector || 'input';
            var $sel = $(parentSelector).find(sel);
            if ($sel.length) {
                $sel.addClass(cln);
            }
            else {
                throw new ('The selector "' + parentSelector + '.find(' + toSelector + ') " does not identify an element');
            }
        } catch (e) {
            console.error(e);
        }
    };

    /** 
     *  A táblázat konténerének magasságát állítja be.
     *  Ablak méretezésekor (vagy egyéb befolyásoló változáskor) érdemes elindítani.
     *  
     *  @param {number} [heightAdjuster=0] A metódus által kiszámított magasság értékét módosíthatja. Alapértelmezett értéke: 0.
     */
    this.resizeCardBody = function (heightAdjuster) {

        if (!$.isNumeric(heightAdjuster)) heightAdjuster = 0;
        var top = 0;
        var bottom = 0;
        var thisfn = thispt + 'resizeCardBody method: ';

        try {
            if (vrhmenu.ViewMode === vrhmenu.ViewModes.Desktop) {
                var $fixedtop = $('.navbar.fixed-top');
                if ($fixedtop.is(':visible')) {
                    top = $fixedtop.outerHeight();
                    bottom = $('.navbar.fixed-bottom').outerHeight();
                }
                else {
                    top = $('#desktophidebar').outerHeight();
                }
            }
        } catch (e) {
            console.log(thisfn + 'Nem elérhető a Vrh.Web.Menu komponens!');
            top = 0;
            bottom = 0;
            var $fixedtop = $('.navbar.fixed-top');
            if ($fixedtop.is(':visible')) {
                top = $fixedtop.outerHeight();
                bottom = $('.navbar.fixed-bottom').outerHeight();
            }
        }
        if (!$.isNumeric(top)) top = 0;
        if (!$.isNumeric(bottom)) bottom = 0;
        console.log(thisfn + 'heightAdjuster=%d, top=%d, bottom=%d', heightAdjuster, top, bottom)

        var mtop = 0;
        var mbottom = 0;
        var $content = $('.container-fluid.body-content');  // minden nézetben szereplő beállítás !!!
        if ($content.css('margin-top')) mtop = parseFloat($content.css('margin-top').replace(/[^0-9.]/g, '')) + 1 ; 
        if ($content.css('margin-bottom')) mbottom = parseFloat($content.css('margin-bottom').replace(/[^0-9.]/g, '')) + 1; 

        var $head = $('div.card-header');   // ott, ahol használjuk, ott kell lennie "card-header"-nek
        var head = $head.outerHeight();
        var headtop = parseFloat($head.css('padding-top').replace(/[^0-9.]/g, '')) + 1;
        var headbottom = parseFloat($head.css('padding-bottom').replace(/[^0-9.]/g, '')) + 1;

        var $cardBody = $('div.card-body');// ott, ahol használjuk, ott kell lennie "card-body"-nak
        var bodytop = parseFloat($cardBody.css('padding-top').replace(/[^0-9.]/g, '')) + 1;
        var bodybottom = parseFloat($cardBody.css('padding-bottom').replace(/[^0-9.]/g, '')) + 1;

        console.log(thisfn + 'top=%d, bottom=%d, mtop=%d, mbottom=%d, head=%d, headtop=%d, headbottom=%d, bodytop=%d, bodybottom=%d', top, bottom, mtop, mbottom,  head, headtop, headbottom, bodytop, bodybottom)
        var $tableDiv = $('div.card-body > div.table-responsive');
        $tableDiv.css('height', (window.innerHeight - top - bottom - mtop - mbottom - head - headtop - headbottom - bodytop - bodybottom - 5) + heightAdjuster)
        // WA20190312: Nem tudom mi az a 2, de legalább ennyi kell, hogy ne legyen vertikális scroll a böngésző ablakon.
    }; // resizeCardBody method END

    /*##### METHODS END #####*/
}
this.masterdata = new VrhMasterData();
// #######################################################################
// ### VrhMasterData prototype END
// #######################################################################

// #######################################################################
// ### String prototype kitejesztése(i) START
// #######################################################################

/**
 * A String prototype kiterjesztése a .NET-es String.Format mintájára.
 * 
 * @returns {string} A megformázott karaktersorozat.
 */
String.prototype.format = String.prototype.f = function () {
    var s = this,
        i = arguments.length;

    while (i--) {
        s = s.replace(new RegExp('\\{' + i + '\\}', 'gm'), arguments[i]);
    }
    return s;
};

// #######################################################################
// ###  String prototype kitejesztése(i) END
// #######################################################################


}// VrhCommonTools prototype END
try {
    var vrhct = new VrhCommonTools();
} catch (e) {
    console.log(e);
}


// #######################################################################
// ### VrhDataTable prototype START
// #######################################################################

/**
 * A prototípus a VRH DataTable-re épülő megjelenítéseit támogatja.
 *
 * Elsődleges célja, hogy a szerveroldali feltöltést támogassa,
 * és csak is a megjelenítés, a rendezés, és szűrés beállításait
 * automatizálja.
 * Ellentétben a régebbi megoldással, ahol a create, remove, update, detail
 * formok kezelése is a része volt. Azoknak készül egy másik
 * támogató felület, talán VrhMasterData néven.
 *
 * Ha vannak a megjelenítésen kívüli feladatok, például a műveletek oszlop
 * akciói, azokat az oszlop definiálásakor lehetőleg az onclick attribútumban
 * kell megadni.
 *
 * @param {any} options A beállításokat tartalmazó objektum.
 * @param {VrhDataTableFilter[]} options.filterDefs Az alapértelmezettől eltérő szűrők beállításainak tömbje.
 */
function VrhDataTable(options) {
    'use strict';

    /*##### PROTOTYPE VARIABLES #####*/

    /** hogy a belső függvényekben is tudjak hivatkozni a prototype tulajdonságaira */
    var me = this;

    /**
     * A lehetséges nyelvi fordításoka beállítása.
     * Egyelőre így, aztán majd továbbgondoljuk.
     * lcid-nek olyan értéket adjunk, amely létezik az aktív nyelvek között.
     * */
    var languages = new Array();

    /** A tábla elkészültét jelző logikai változó */
    var isInitComplete = false;

    /** Az initComplete callback számára küldött paraméter, hogy továbbítani tudjuk */
    var jsonInitComplete;
    /*##### PROTOTYPE VARIABLES END #####*/


    /*##### PROPERTIES #####*/

    /** A DataTable plugin által előállított objektum */
    this.Table = null;

    /** A táblázat jQuery objektuma. */
    this.$table = '';

    /** A DataTable Settings megőrzése az inicializálás után. */
    this.Settings = null;

    this.Constants = {
        LCID_ENUS: 'en-US',
        LCID_HUHU: 'hu-HU',
        THISPT: 'VrhDataTable prototype:  '
    };

    /** Beállítások a VrhDataTable számára */
    this.Options = {
        ajax: {
            url: null,      // az adatlistát eredményező akció
            type: 'post'    // akció típusa
        },
        autoWidth: false,    // alapértelmezett érték: true
        columns: null,      // a táblázatot alkotó oszlopok gyűjteménye
        columnDefs: null,

        dom: "<'row'<'col-sm-12 col-md-6'l><'col-sm-12 col-md-6'f>>" +  // Ez egyébként a bootstrap 4-hez beállított alapértelmezés
            "<'row'<'col-sm-12'tr>>" +
            "<'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7'p>>",
        //dom: '<"row"<"col-2"l><"col-2"i><"col-5"p><"col-3"f>>' + '<"row"<"col-12"tr>>',
        //dom: '<"top"i>rt<"bottom"flp><"clear">',
        //dom: '<"d-flex justify-content-between"filpr>' + '<"row pb-3"<"col-12"t>>',
        filtering: true,        // Kell-e a szűrési szolgáltatás. Alapértelmezetten kell.
        filterDefs: null,       // Az input[type=text]-től eltérő szűrések beállítása
        initComplete: null,     //
        lcid: me.Constants.LCID_HUHU, // nyelvi illetve lokalizáció kódja
        tableId: null,
        paging: true,           // A lapozás működjön-e, még nem tudom mi az új API szerinti neve. A régi név 'bPaginate'. Alapértelmett érték: true
        processing: true,       // Mutassa-e a "Feldolgozás..." ablakot. Alapértelmett érték: true
        scrollCollapse: false,  // Csökkentheti-e a megjelenített sorok számát, ha van scrollY előírás (vagyis fix a táblázat magassága).
        scrollY: "",            // !!! Az üres string jelenti, hogy nincs scrollY !!!
        serverSide: true,       // Ezzel jelzem, hogy szerver oldali töltés/frissítés van.
        stateSave: true,
    };

    /*##### PROPERTIES END #####*/


    /*##### METHODS #####*/

    /**
     * Tábla újratöltése.
     * A létező táblát megszünteti (destroy), majd újra inicializálja 
     * a DataTable objektumot.
     */
    this.reload = function () {
        var thisfn = me.Constants.THISPT + 'reload method: ';
        console.log(thisfn + 'PING me.Options', me.Options);

        if (me.Table) me.Table.destroy();
        me.Table = null;

        // Tábla inicializálás
        //isInitComplete = false;   // setAfterComplete-nek kell, de az most comment
        me.Table = me.$table.DataTable({
            ajax: me.Options.ajax,
            autoWidth: me.Options.autoWidth,
            columns: me.Options.columns,
            columnDefs: me.Options.columnDefs,
            dom: me.Options.dom,
            language: me.GetLanguage(me.Options.lcid),
            scrollCollapse: me.Options.scrollCollapse,
            scrollY: me.Options.scrollY,
            serverSide: me.Options.serverSide,
            stateSave: me.Options.stateSave,
            processing: me.Options.processing,
            initComplete: function (settings, json) {
                //console.log(thisfn + 'initComplete OK me.Table', me.Table);
                console.log(thisfn + 'initComplete OK settings', settings);
                //console.log(thisfn + 'initComplete OK json', json);
                me.Settings = settings;
                jsonInitComplete = json;
                //isInitComplete = true;  // setAfterComplete-nek kell, de az most comment
                setTimeout(function () {    // elvileg él a me.Table, de azért hagyjunk neki időt 
                    if (me.Options.filtering) setFiltering();
                    if (me.Options.initComplete) {
                        me.Options.initComplete(me.Settings, jsonInitComplete);
                    }
                }, 50);
            }
        });
        //setAfterComplete(); // !! elvileg nem kell, mert már az initComplete-ben él a me.Table !!
    };

    /**
     * A nyelvi kód alapján visszatér egy a DataTables számára megfelelő 
     * nyelvi objektummal. Ha nincs a készletben a kért nyelv, akkor
     * az angol nyelvi beállítással tér vissza.
     * 
     * @param {string} lcid Nyelvi kód (LCID).
     *
     * @return {any} Egy nyelvi objektum.
     */
    this.GetLanguage = function (lcid) {
        var enusLang = null;
        for (var i = 0; i < languages.length; i++) {
            if (languages[i].lcid === lcid) {
                return languages[i].language;
            } else if (languages[i].lcid === me.Constants.LCID_ENUS) {
                enusLang = languages[i].language;
            }
        }
        return enusLang;
    };

    /*##### METHODS END #####*/


    /*##### PRIVATE FUNCTIONS #####*/

    /** A tábla teljes valódi elkészülte utáni teendők */
    function setAfterComplete() {
        var thisfn = me.Constants.THISPT + 'setAfterComplete method: ';
        var testNo = 40; // ez kb. két másodperc
        if (isInitComplete || testNo <= 0) { //hogy ne legyen végtelen ciklus, ha netán ...
            console.log(thisfn + 'HURRÁ :) Kész a tábla!');
            setTimeout(function () {
                setFiltering();
                if (me.Options.initComplete) {
                    me.Options.initComplete(me.Settings);
                }
            }, 100);
        }
        else {
            console.log(thisfn + 'AJJAJ :( Még készül a tábla! testNo=%d', testNo);
            testNo--;
            setTimeout(function () {
                setAfterComplete();
            }, 50);
        }
    }

    /**
     * Szűrő input mezők hozzáadása.
     * Az inputok egy id="{tableid}-ColumnFilter#" formájú azonosítót kapnak,
     * ahol # az oszlop indexét jelenti. Továbbá a "datatableColumnFilter"
     * osztály lesz beállítva.
     * Ha a text típusú input nem megfelelő, akkor azt a felhasználó modulban
     * kell lecserélni.
     */
    function setFiltering() {
        var thisfn = me.Constants.THISPT + 'setFiltering private method: ';
        console.log(thisfn + 'PING');

        // Szűrő inputok hozzáadása
        var idPrefix = me.Options.tableId + '-ColumnFilter';
        var $head = me.$table.find('thead');
        //console.log(thisfn + '$head', $head);
        var html = '<tr>';
        //console.log('me.Settings.aoColumns', me.Settings.aoColumns);
        for (var jx = 0; jx < me.Settings.aoColumns.length; jx++) {
            var searchable = me.Settings.aoColumns[jx].bSearchable;
            var visible = me.Settings.aoColumns[jx].bVisible;
            //console.log(thisfn + 'searchable ' + jx + ', visible', searchable, visible);
            if (visible) {
                html += '<th>';
                if (searchable) {
                    var filterDef = null;
                    if (me.Options.filterDefs != null) {
                        filterDef = me.Options.filterDefs.find(function (item) {
                            return item.column == jx;
                        });
                    }
                    if (filterDef) {
                        //console.log(thisfn + 'selectfilter filterDef=', filterDef);
                        var selectOptions = $('#' + filterDef.id).html();
                        //console.log(thisfn + 'selectfilter selectOptions=', selectOptions);
                        html += '﻿<select id="' + idPrefix + jx + '" class="datatableColumnFilter">'
                              + selectOptions + '</select>';
                    }
                    else {
                        html += '﻿<input type="text" id="' + idPrefix + jx + '" class="datatableColumnFilter" />';
                    }
                }
                html += '</th>';
            }
        }
        html += '</tr>';
        //console.log('prepend előtt: html', html);
        $head.prepend(html); // ez szúrja be a szűrő inputokat

        // Szűrőmezők értékének visszaállítása
        var savedState = me.Table.state.loaded();
        if (savedState) {   //ha egyáltalán történt állapotmentés
            me.Table.columns().eq(0).each(function (colix) {
                var colSearch = savedState.columns[colix].search;
                //console.log(thisfn + 'colSearch[' + colix + ']', colSearch);
                if (colSearch.search) { // ha van elmentett szűrő érték
                    var filterDef = null;
                    if (me.Options.filterDefs != null) {
                        filterDef = me.Options.filterDefs.find(function (item) {
                            return item.column == colix;
                        });
                    }
                    if (filterDef) {
                        $('select[id="' + idPrefix + colix + '"]').val(colSearch.search);
                    } else {
                        $('input[id="' + idPrefix + colix + '"]').val(colSearch.search);
                    }
                }
            });
            me.Table.draw();
        }

        // Az összes oszlop kereső inputjához az eseményre feliratkozás
        me.Table.columns().every(function () {
            var thatx = this;
            var columnIx = this.index();
            var inpid = idPrefix + columnIx;
            //console.log(thisfn + ' inpid, columns()every.this', inpid, this);
            var filterDef = null;
            if (me.Options.filterDefs != null) {
                filterDef = me.Options.filterDefs.find(function (item) {
                    return item.column == columnIx;
                });
            }
            if (filterDef) {
                $('select[id="' + inpid + '"]').on('change', function () {
                    //console.log(thisfn + 'ColumnFilter change event: this', this);
                    if (thatx.search() !== this.value) {
                        thatx.search(this.value).draw();
                    }
                });
            } else {
                $('input[id="' + inpid + '"]').on('keyup', function () {
                    //console.log(thisfn + 'ColumnFilter keyup event: this', this);
                    if (thatx.search() !== this.value) {
                        thatx.search(this.value).draw();
                    }
                });
            }
        });
    }; // setFiltering method END

    /**
     * A táblázat adott sorszámú oszlop fölé beteszi egy select input filtert.
     * Tudnivalók: A cshtml-ben egy rejtett div-ben legyen a select.
     * Minta: <div hidden>@Html.DropDownList(idSelect, Model.List)</div>
     * 
     * @param {number} columnIx Az oszlop sorszáma, ahova kerül a select szűrő
     * @param {string} idSelect A rejtett "div"-ben lévő select azonosítója
     */
    function setFilterSelect(columnIx, idSelect) {
        var thisfn = me.Constants.THISPT + 'setFilterSelect method: ';
        var idFilterInput = me.Options.tableId + '-ColumnFilter' + columnIx;
        var $filter = $('#' + idFilterInput);
        var currentVal = $filter.val();
        var $filterTH = $filter.parent('th');
        var htmlSelect = ('#' + idSelect).parent('div').html();
        //console.log(thisfn + '$filterTH, currentVal', $filterTH, currentVal);
        $filterTH.empty();
        $filterTH.append(htmlSelect);
        $filterTH.find('select').attr('id', idFilterInput).addClass('datatableColumnFilter').val(currentVal);
        setTimeout(function () {
            $('#' + idFilterInput).on('change', function () {
                console.log(thisfn + 'change event occured: this', this);
                me.Table.columns(0).search(this.value).draw();
            });
        }, 800);
    }; // setFilterSelect method END

    /*##### PRIVATE FUNCTIONS END #####*/


    /*##### CONSTRUCTOR #####*/
    try {
        languages.push({
            lcid: this.Constants.LCID_ENUS,
            language: {
                "decimal": "",
                "emptyTable": "No data available in table",
                "info": "Showing _START_ to _END_ of _TOTAL_ entries",
                "infoEmpty": "Showing 0 to 0 of 0 entries",
                "infoFiltered": "(filtered from _MAX_ total entries)",
                "infoPostFix": "",
                "thousands": ",",
                "lengthMenu": "Show _MENU_ entries",
                "loadingRecords": "Loading...",
                //"processing": "Processing...",
                'processing': '<i class="fas fa-spinner fa-spin fa-3x fa-fw"></i>',
                "search": "Search:",
                "zeroRecords": "No matching records found",
                "paginate": {
                    "first": "First",
                    "last": "Last",
                    "next": "Next",
                    "previous": "Previous"
                },
                "aria": {
                    "sortAscending": ": activate to sort column ascending",
                    "sortDescending": ": activate to sort column descending"
                }
            }
        });
        languages.push({
            lcid: this.Constants.LCID_HUHU,
            language: {
                "decimal": "",
                "emptyTable": "Nincs rendelkezésre álló adat",
                "info": "Találatok: _START_ - _END_ Összesen: _TOTAL_",
                "infoEmpty": "Nulla találat",
                "infoFiltered": "(_MAX_ összes rekord közül szűrve)",
                "infoPostFix": "",
                "thousands": ",",
                "lengthMenu": "<select><option value='5'>5</option><option value='10'>10</option><option value='20'>20</option><option value='50'>50</option><option value='100'>100</option><option value='200'>200</option><option value='500'>500</option><option value='1000'>1000</option><option value='-1'>Összes</option></select> találat oldalanként",
                "loadingRecords": "Betöltés...",
                //"processing": "Feldolgozás...",
                'processing': '<i class="fas fa-spinner fa-spin fa-3x fa-fw"></i>',
                "search": "Keresés:",
                "zeroRecords": "Nincs a keresésnek megfelelő találat",
                "paginate": {
                    "first": "Első",
                    "last": "Utolsó",
                    "next": "Következő",
                    "previous": "Előző"
                },
                "aria": {
                    "sortAscending": ": aktiválja a növekvő rendezéshez",
                    "sortDescending": ": aktiválja a csökkenő rendezéshez"
                }
            }
        });

        if (options) {
            $.extend(true, this.Options, options);
            if (this.Options.tableId) {
                this.$table = $('#' + this.Options.tableId);
                if (!this.$table.length) {
                    throw 'The table is not exists! tableId = "' + options.tableId + '"';
                }
            } else {
                throw 'The options.tableId parameter is required!';
            }
            if (!this.Options.columns) {
                throw 'The options.columns parameter is required!';
            }
            if (this.Options.ajax) {
                if (!this.Options.ajax.url) {
                    throw 'The options.ajax.url parameter is required!';
                }
            } else {
                throw 'The options.ajax parameter is required!';
            }
            //console.log(me.Constants.THISPT,this.Options);

            me.reload();

        } else {
            throw 'The options parameter is required!';
        }

    } catch (e) {
        console.error(this.Constants.THISPT, e);
    }
    /*##### CONSTRUCTOR END #####*/
}
// #######################################################################
// ### VrhDataTable prototype END
// #######################################################################

/** A szűrések számára szükséges típus */
class VrhDataTableFilter {
    constructor(column, id) {
        /** Melyik oszlop szűrésére vonatkozik */
        this.column = column;
        /** Az azonosító, amit a szűrés helyére be kell tenni */
        this.id = id;
    }
}


// #######################################################################
// ### VrhIdNameHandler prototype START
// #######################################################################
/*
 * Készült: 2019.03.05 Wittmann Antal
 * Módosult:
 */

/**
 *  A Vonalkód Rendszerház id-name típusú adatok kezelésében hasznos megoldás.
 * 
 * @param {VrhIdNameHandlerOptions} handlerOptions A prototípus működéséhez szükséges beállítások
 */
function VrhIdNameHandler(handlerOptions) {
    'use strict';

    /*##### PROTOTYPE VARIABLES #####*/
    var thispt = "VrhIdNameHandler prototype: ";
    /*##### PROTOTYPE VARIABLES END #####*/

    var options = new VrhIdNameHandlerOptions();
    if (handlerOptions) {
        $.extend(true, options, handlerOptions);
        console.log(thispt + 'options=',options);
    } else {
        throw 'The handlerOptions parameter is required!';
    }

    var LANGUAGE_CODE = options.LCID.substr(0, 2);

    // Helyek, amikre hivatkozni kell majd
    var dataRowsId = '#' + options.Id.DataRows;
    var dataNewId = '#' + options.Id.DataNew;

    // Selectorok
    var $EntitiesBootboxId = $('#' + options.Id.Editor);

    bootbox.setLocale(LANGUAGE_CODE);
    if (LANGUAGE_CODE === 'hu') {
        bootbox.removeLocale(LANGUAGE_CODE);
        bootbox.addLocale(LANGUAGE_CODE, { OK: 'OK', CANCEL: 'Mégsem', CONFIRM: 'OK' });
    }

    /*##### PRIVATE FUNCTIONS #####*/
    /**
     * Egy ablakot jelenít meg a megkapott üzenettel, ami előtt várakozásra utaló animáció látszik.
     * 
     * @param {string} waitMessage : Ha üres, akkor a p.Message.Wait jelenik meg.
     * @param {function} shownCallback : Ha nem üres, akkor a megadott funkciót 'shown.bs.modal' esemény után futtatja.
     *   
     * @return {bootbox.dialog} : Egy bootbox.dialog() típus.
     */
    function waitMessageDialog(message, shownCallback = null) {
        if (!message) message = options.Message.Wait;
        var diagwait = bootbox.dialog({ message: '<div class="text-center"><i class="fa fa-spin fa-spinner"></i> ' + message + '</div>' });
        if (shownCallback) {
            diagwait.on('shown.bs.modal', function (e) {
                //console.log(thispt + 'wait method: shownCallback', shownCallback);
                diagwait.off('shown.bs.modal');
                if (shownCallback) shownCallback();
            });
        }
        return diagwait;
    }

    /**
     * Adatok listázása.
     * A response.DataList egy List<SelectListItem>.
     * A SelectListItem.Disabled true értéke jelzi azt a rekordot, amit nem lehet törölni.
     * A SelectListItem.Selected true értéke jelzi azt a rekordot, amit nem lehet szerkeszteni.
     */
    function refreshDataRows() {
        var thisfn = thispt + 'refreshDataRows: ';
        //console.log(thisfn + 'START url=' + options.Url.List);
        console.log(thisfn + 'START dataRowsId', dataRowsId);
        $.ajax({
            method: 'GET',
            cache: false,
            url: options.Url.List,
            contenttype: 'application/json',
            datatype: 'json',
            success: function (response) {
                //console.log('response', response);
                if (response.ErrorMessage === '') {
                    var etList = response.DataList;
                    console.log(thisfn + 'Lekérdezés sikeres. etList', etList);
                    var htm = '';
                    for (var ix = 0; ix < etList.length; ix++) {
                        if (options.VisibleIdColumn) {
                            htm += '<tr class="active"><td>' + etList[ix].Value + '</td><td>' + etList[ix].Text + '<td>';
                        }
                        else {
                            htm += '<tr class="active"><td>' + etList[ix].Text + '<td>';
                        }
                        if (options.AllowUpdate || options.AllowDelete) {
                            htm += '<td style="width:50px;">';
                            if (options.AllowUpdate && etList[ix].Selected == false) {
                                htm += '<span'
                                    + ' class="fas fa-pencil-alt idnamehandler-update datatableActionIcon" '
                                    + ' title="' + options.Title.IconPencil + '"'
                                    + ' data-id="' + etList[ix].Value + '"'
                                    + ' data-name="' + etList[ix].Text + '"'
                                    + '></span>';
                            }
                            if (options.AllowDelete && etList[ix].Disabled == false) {
                                htm += '<span'
                                    + ' class="fas fa-trash-alt idnamehandler-delete datatableActionIcon"'
                                    + ' title="' + options.Title.IconTrash + '"'
                                    + ' data-id="' + etList[ix].Value + '"'
                                    + ' data-name="' + etList[ix].Text + '"'
                                    + '></span>';
                            }
                            htm += '</td>';
                        }
                        htm += '</tr>';
                    }
                    //console.log(thisfn + ' htm=', htm);
                    $(dataRowsId).html(htm);
                    $('.idnamehandler-update').click(function () {
                        updateData($(this).data('id'), $(this).data('name'));
                    });
                    $('.idnamehandler-delete').click(function () {
                        deleteData($(this).data('id'), $(this).data('name'));
                    });
                } else {
                    //console.log(thisfn + 'Lekérdezés sikertelen!');
                    bootbox.alert(response.ErrorMessage);
                }
            },
            error: function (jqXHR, exception) {
                console.log(thisfn + 'Ajax hívás sikertelen!', jqXHR.responseText);
            }
        });
    }// refreshDataRows function END

    function updateData(id, name) {
        var thisfn = 'VrhIdNameHandler: updateData: ';
        console.log(thisfn + 'START id=' + id + '; name=' + name + '; allowUpdate=' + options.AllowUpdate);
        if (!options.AllowUpdate) {
            return;
        }
        bootbox.prompt({
            value: name,
            size: 'small',
            title: options.Title.IconPencil,
            callback: function (result) {
                if (result !== null) {
                    $.ajax({
                        method: 'POST',
                        cache: false,
                        url: options.Url.Update,
                        contenttype: 'application/json',
                        datatype: 'json',
                        data: { dataId: id, dataName: result },
                        success: function (errorMessage) {
                            if (errorMessage === '') {
                                console.log(thisfn + 'Sikeres módosítás.');
                                refreshDataRows();
                            } else {
                                bootbox.alert(errorMessage);
                            }
                        },
                        error: function (jqXHR, exception) {
                            console.log(thisfn + 'Ajax hívás sikertelen!', jqXHR.responseText);
                        }
                    });
                }
            }
        });
    }// updateData function END

    function deleteData(id, name) {
        /// A függvény a törlés rákérdezése után a paraméterben megadott id-val, meghívja
        /// a delete url-t. Amely következő függvényt hívja a controllerben: public JsonResult EntityDelete(){ ... }
        /// Ha az üres stringgel tér vissza, akkor minden rendben lezajlott, ha nem, akkor az hibaüzenet.
        var thisfn = 'VrhIdNameHandler: deleteData: ';
        console.log(thisfn + 'id=' + id + '; name=' + name);
        $EntitiesBootboxId.addClass('darken');
        var dialog = bootbox.dialog({
            title: options.Title.Confirm,
            message: options.Label.Data + ': ' + name + '<br />' + '<br />' + options.Message.Delete + '<br />' + '<br />' + options.Confirm.Delete,
            onEscape: function (event) {
                dialog.modal('hide');
                $EntitiesBootboxId.removeClass('darken');
            },
            buttons: {
                ok: {
                    label: options.Label.Button.Yes,
                    className: 'btn btn-danger',
                    callback: function () {
                        console.log(thisfn + 'biztos benne Id=' + id);
                        dialog.modal('hide');

                        var diagwait = waitMessageDialog(options.Message.Wait, function () {
                            $.ajax({
                                method: 'POST',
                                cache: false,
                                url: options.Url.Delete,
                                contenttype: 'application/json',
                                datatype: 'json',
                                data: { dataId: id },
                                success: function (errorMessage) {
                                    diagwait.modal('hide');
                                    if (errorMessage === "") {
                                        $EntitiesBootboxId.removeClass('darken');
                                        console.log(thisfn + 'Sikeres törlés.');
                                        refreshDataRows();
                                    } else {
                                        bootbox.alert(errorMessage, function () {
                                            $EntitiesBootboxId.removeClass('darken');
                                        });
                                    }
                                },
                                error: function (jqXHR, exception) {
                                    diagwait.modal('hide');
                                    console.log(thisfn + 'Ajax hívás sikertelen!', jqXHR.responseText);
                                }
                            });
                        });
                    }
                },
                cancel: {
                    label: options.Label.Button.No,
                    className: 'btn btn-primary',
                    callback: function () {
                        $EntitiesBootboxId.removeClass('darken');
                    }
                }
            }
        });
    }// deleteData function END

    /*##### PRIVATE FUNCTIONS END #####*/


    /*##### METHODS #####*/

    this.addData = function () {
        var thisfn = 'VrhIdNameHandler: addData: ';
        var newName = $(dataNewId).val();
        console.log(thisfn + 'START newName=%s, options=%o', newName, options);
        if (!options.AllowAdd) {
            return;
        }
        $.ajax({
            method: 'POST',
            cache: false,
            url: options.Url.Add,
            contenttype: 'application/json',
            datatype: 'json',
            data: { dataName: newName },
            success: function (errorMessage) {
                if (errorMessage === '') {
                    console.log(thisfn + 'Sikeres hozzáadás.');
                    refreshDataRows();
                } else {
                    bootbox.alert(errorMessage);
                }
            },
            error: function (jqXHR, exception) {
                console.log(thisfn + 'Ajax hívás sikertelen!', jqXHR.responseText);
            }
        });
    };// addData public method END

    /*##### METHODS END #####*/

    refreshDataRows();
}// VrhIdNameHandler prototype END


/**
 * A VrhIdNameHandler prototípus beállításai.
 */
function VrhIdNameHandlerOptions()
{
    /** Engedélyezett-e az egyedek hozzáadása */
    this.AllowAdd = true;
    /** Engedélyezett-e az egyedek törlése */
    this.AllowDelete = true;
    /** Engedélyezett-e az egyedek módosítása */
    this.AllowUpdate = true;
    /** Megerősítést kérő üzenetek gyűjtőhelye */
    this.Confirm = {
        Delete: '',
    };
    /** A handler objektumok azonosítóinak gyűjtőhelye */
    this.Id = {
        /** A handéler ablakának azonosítója. */
        Editor: '',
        /** A táblázat body részének azonosítója, ahová a listát kell illeszteni. */
        DataRows: '',
        /** Az új adatot tartalmazó input azonosítója. */
        DataNew: ''
    };
    /** A nézeten lévő inputok címkéinek gyűjtőhelye */
    this.Label = {
        Button: {
            Yes: '',
            No: '',
        },
        Data: ''
    };
    /** A környetben érvényes nyelvi kód */
    this.LCID = '';
    /** Az üzenetek gyűjtőhelye */
    this.Message = {
        Wait: '',
        Delete: ''  // a régi options.Message.Delete
    };
    /** A nézeten lévő eszközök feliratozásának, címeinek és tooltipjeinek gyűjtőhelye */
    this.Title = {
        Confirm: '', // options.Title.Confirm
        IconPencil: '',
        IconTrash: '',
    };
    /** A handler által hívott akciókra mutató url-ek gyűjtőhelye */
    this.Url = {
        /** Egy elem hozzáadásának akciója */
        Add: '',
        /** Egy elem törlését elvégző akció */
        Delete: '',
        /** A táblázat listáját eredméynező akció */
        List: '',
        /** Egy elem módosítását elvégző akció */
        Update: '',
    };
    /** Annak beállítása, hogy az azonosító oszlop megjelenjen-e */
    this.VisibleIdColumn = true;
} // VrhIdNameHandlerOptions class END
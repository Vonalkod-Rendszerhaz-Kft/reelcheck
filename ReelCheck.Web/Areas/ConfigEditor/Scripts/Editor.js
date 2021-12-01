/***************************************************
ConfigEditor/Scripts/Editor.js
    A Vonalkód Rendszerház ConfigEditor webes felület
    kezeléséhez készült metódusok és események.
----------------------
Alapítva:
    2018.11.15.-11.23. Wittmann Antal
Módosult:
****************************************************/
/*##### EVENTS #####*/
$(document).ready(function () {
    'use strict';

    var thisfn = 'ConfigEditor.Manager.js: ready event: ';
    console.log(thisfn + 'PING');

    // Resize figyelése, hogy a card-body mérete kitöltse a képernyőt
    $(window).resize(function () {
        vrhces.CardBodyResize();
    });
    vrhmenu.MenuSwapEventFunction = vrhces.CardBodyResize;  // menüváltozáskor is hívja meg a resize-t

    $('.collapse').on('shown.bs.collapse', function () {
        vrhces.CardBodyResize();
    });
    $('.collapse').on('hidden.bs.collapse', function () {
        vrhces.CardBodyResize();
    });

    vrhces.ReloadTable();

    console.log(thisfn + 'END');
}); // $(document).ready END 
/*##### EVENTS END #####*/


/**
 * A nyelvi kezelőfelülethez készült prototípus.
 * 
 * @param {ExportToConfigEditor} imp Paraméter objektum, cshtml-ben feltöltenő paramétereket tartalmaz.
 */
function ConfigEditorScripts(imp) {
    'use strict';

    /*##### PROTOTYPE VARIABLES #####*/
    var me = this;  //hogy a belső függvényekben is tudjak hivatkozni a prototype tulajdonságaira
    var thispt = 'ConfigEditorScripts: ';
    var oldValueForEditable = null; // eredeti érték megőrzése az UpdateValue meghívásakor
    var isGroupVisible = (imp.SelectedGroupId === '');
    var isInVariableEdit = false; // ez jelzi, ha változó szerkesztést kezdeményeztek
   console.log(thispt + 'isGroupVisible = ', isGroupVisible);

    // Selector cache
    var $checkWSV = $('#' + imp.Ids.Input.CheckWSV);
    var $counter = $('#' + imp.Ids.Counter);
    var $dataTable = $('#' + imp.Ids.Table);
    var $groupHelp = $('#' + imp.Ids.GroupHelp);                // a help szöveget tartalmazó div containere
    var $groupHelpDiv = $('#' + imp.Ids.GroupHelp).find('div'); // a help szöveget tartalmazó div
    var $hiddenSelect = $('#' + imp.Ids.Input.SelectHidden);    // a rejtett állapotban feltett csoport választó
    var $groupTitle = $('#' + imp.Ids.GroupTitle);              // ide kerül a lap vagy csoport címe
    var $groupTitleSpan = $groupTitle.find('span');             // ide kerül a lap vagy csoport címe
    var $groupTitleI = $groupTitle.find('i');                   // a help szöveg létezését mutató elem
    var $reloadBttn = $('#' + imp.Ids.Button.Reload);           // az újra töltő gomb 
    var $saveBttn = $('#' + imp.Ids.Button.ConfigSave);         // az újra töltő gomb 

    /** Oszlop beállítások a DataTables számára */
    var dtColumns = [
        { data: imp.Data.Group, visible: isGroupVisible },
        { data: imp.Data.Label },
        { data: imp.Data.Name },
        {
            data: imp.Data.Value,
            createdCell: function (td, cellData, rowData, row, col) {
                //console.log('Value.createdCell: td, cellData, rowData, row, col', td, cellData, rowData, row, col);
                if (rowData.State === 0) {
                    $(td).addClass('datatableEditable font-weight-bold');
                    //console.log('Value.createdCell: rowData.XPath, rowData.FileName', rowData.XPath, rowData.FileName);
                    $(td).data('filename', rowData.Connection.File);
                    $(td).data('xpath', rowData.XPath);
                    $(td).data('validate', rowData.Validate);
                } else {
                    $(td).addClass('text-secondary');
                }
            },
        },
        {
            searchable: false,
            orderable: false,
            render: function (data, type, row) {
                //console.log('Edit icon render: row', row);
                if (row.IsContainsVariable) {
                    console.log('Edit icon render: row', row);
                    var span = document.createElement('span');
                    $(span).addClass('fas fa-edit datatableActionIcon');
                    var settingJSON = JSON.stringify(row);
                    $(span).attr('onclick', "vrhces.VariableEdit(" + settingJSON + ")");
                    //$(span).attr('onclick', "vrhces.VariableEdit('" + root + "', '" + file + "', '" + xpath + "')");
                    $(span).attr('title', imp.Titles.EditVariables);

                    var $temp = $(document.createElement('div'));
                    $temp.append(span);
                    return $temp.html();
                } else {
                    return '';
                }
            },

        },
    ];

    /** Oszlop definíciók a DataTables számára */
    var dtColumnDefs = [
        { targets: [0], class: 'nowrap', width: '20%' },
        { targets: [4], width: '20px' }
    ];

    /*##### PROTOTYPE VARIABLES END #####*/

    /*##### PROPERIES #####*/

    /** Az inicializált tábla VrhDataTable objektuma */
    this.ConfigDataTable = null;

    /*##### PROPERIES END #####*/


    /*##### PUBLIC METHODS #####*/

    /** 
     *  Ablak méretezésekor beállítja a táblázat konténerének magasságát.
     */
    this.CardBodyResize = function () {
        var top = 0;
        var bottom = 0;
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

        var $content = $('.container-fluid.body-content');
        var mtop = parseFloat($content.css('margin-top').replace(/[^0-9.]/g, ''));
        var mbottom = parseFloat($content.css('margin-bottom').replace(/[^0-9.]/g, ''));;

        var $head = $('div.card-header');
        var head = $head.outerHeight();
        var ptop = parseFloat($head.css('padding-top').replace(/[^0-9.]/g, ''));
        var pbottom = parseFloat($head.css('padding-bottom').replace(/[^0-9.]/g, ''));

        //console.log('window.resize event: top, bottom, head', top, bottom, head, mtop, mbottom, ptop, pbottom)
        var $tableDiv = $('div.card-body > div.table-responsive');
        $tableDiv.css('height', window.innerHeight - top - bottom - mtop - mbottom - ptop - pbottom - head - 18)
        // WA20181030: Nem tudom mi az a 18, de legalább ennyi kell, hogy ne legyen vertikális scroll a böngésző ablakon.
    }; // CardBodyResize method END


    /**
     * Tábla újrafeltöltése.
     * Ha nem létezik még a DataTable, akkor inicializálja.
     * 
     * @param {string} groupId Melyik csoport adatai kell betölteni. Ha üres, akkor az összeset.
     */
    this.ReloadTable = function () {
        var thisfn = thispt + 'ReloadTable method: ';
        $reloadBttn.prop('disabled', true);

        var urlGetData = imp.Url.GetData + '&isSubstitution=' + $checkWSV.is(':checked');
        if (me.ConfigDataTable === null) {
            $dataTable.off('draw.dt');  // táblázat kirajzolásának befejezését jelző esemény tisztítás
            $dataTable.on('draw.dt', function () { // esemény teendők beállítása
                var thiset = thisfn + 'ConfigDataTable.draw.dt event: ';
                // beállítjuk a szerkesztendő cellákat
                $('.datatableEditable').editable(imp.Url.UpdateValue, {
                    indicator: '<i class="fas fa-spinner fa-spin"></i>',
                    height: '1.3rem',
                    width: '100%',
                    tooltip: imp.Titles.ClickToEditing,
                    placeholder: '',
                    onsubmit: function (settings, original) { // callback az url meghívása előtt
                        console.log(thiset + 'editable.onsubmit: settings, original, $(original)', settings, original, $(original));
                        oldValueForEditable = original.revert;  // eredeti értéke megjegyzése
                    },
                    submitdata: function () {   // a szerver oldali akció meghívása előtt vagyunk
                        // ha szükséges, akkor további paraméterek adhatunk az akciónak
                        var filenamePar = $(this).data('filename');
                        var xpathPar = $(this).data('xpath');
                        var validatePar = $(this).data('validate');
                        console.log(thiset + 'editable.submitdata: xpathPar = %s, filenamePar = %s, validatePar = %s, original = %s', xpathPar, filenamePar, validatePar, oldValueForEditable);
                        return { xpath: xpathPar, filename: filenamePar, validate: validatePar, original: oldValueForEditable };
                    },
                    callback: function (result, settings, submitdata) { // az akció sikeres meghívása után vagyunk (ajax.success); A "this" a cella.
                        console.log(thiset + 'editable.callback: result, settings, submitdata', result, settings, submitdata);
                        var vvu = $.parseJSON(result);
                        console.log(thiset + 'editable.callback: vvu', vvu);
                        if (vvu.ErrorMessage) {
                            bootbox.alert({ size: 'large', message: vvu.ErrorMessage });
                            $(this).text(vvu.OriginalValue);
                        } else {
                            $(this).text(vvu.NewValue);
                        }
                        //$(this).html(GetValueHtml(result, submitdata.instance, submitdata.variable));
                    },
                    //onreset: function (settings, original) {    // amikor megszakítjuk a szerkesztést (Esc lenyomás)
                    //    console.log(thiset + 'editable.onreset: settings, original, $(original)', settings, original, $(original));
                    //    //var instanceName = $(original).data('instance');
                    //    //var variableName = $(original).data('variable');
                    //    //original.revert = GetValueHtml(original.revert, instanceName, variableName);
                    //    //return true;
                    //},
                    //onerror: function (settings, original, xhr) {
                    //    console.log(thiset + 'editable.onerror: settings, original', settings, original, xhr);
                    //},
                    //onedit: function () {
                    //    console.log(thiset + 'editable.onedit this, $(this)', this, $(this));
                    //},
                }); // edititable plugin definition END

                me.CardBodyResize();

                $reloadBttn.prop('disabled', false);    // újra engedélyezzük a reload gombot
            }); // 'draw.dt' event handler END

            console.log(thisfn + 'imp.LCID = "%s", url = "%s"', imp.LCID, urlGetData);
            me.ConfigDataTable = new VrhDataTable({
                ajax: { url: urlGetData }, // az adatlistát eredményező akció
                tableId: imp.Ids.Table,
                columns: dtColumns,
                columnDefs: dtColumnDefs,
                lcid: imp.LCID,
                autoWidth: false,
                stateSave: false,
                initComplete: function (setting, json) { // a táblázat elkészültekor meghívott függvény
                    console.log(thisfn + 'ConfigDataTable.initComplete callback: PING');
                    SetGroupHead(imp.SelectedGroupId);
                    if (isGroupVisible) {   // nincs megadott csoport
                        SetGroupFilter();
                    }
                },
            });
        // if (me.ConfigDataTable === null) END
        } else { 
            console.log(thisfn + 'Refresh url = "%s"', urlGetData);
            me.ConfigDataTable.Table.ajax.url(urlGetData);
            if (isGroupVisible) {
                var filterGroupId =  $('#' + imp.Ids.Input.SelectFilter).val();
                me.ConfigDataTable.Table.columns(0).search(filterGroupId);
                SetGroupHead(filterGroupId);
            } else {
                me.ConfigDataTable.Table.draw('page');
                SetGroupHead(imp.SelectedGroupId);
            }
        }
    }; // ReloadTable method END

    /**
     * A paraméterek mentésének indítása.
     */
    this.ConfigurationsSave = function () {
        var thisfn = thispt + 'ConfigurationsSave method: ';
        $saveBttn.prop('disabled', true);

        $.ajax({
            url: imp.Url.ConfigZipMake,
            type: "POST",
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            success: function (response) {
                //console.log(thisfn + 'ajax success: response', response);
                if (response.ErrorMessage) {
                    bootbox.alert({ message: response.ErrorMessage, size: 'large' });
                } else {
                    window.location.href = imp.Url.ConfigZipDownLoad + '?fileName=' + response.FileName;
                }
                $saveBttn.prop('disabled', false);    // újra engedélyezzük a mentő gombot
            },
            error: function (jqXHR, exception) {
                console.log(thisfn + 'ajax error: ', exception, jqXHR.responseText);
                $saveBttn.prop('disabled', false);    // újra engedélyezzük a mentő gombot
            }
        });
    }; // ConfigurationsSave method END

    /**
     *  Változó szerkesztése.
     * 
     * @param {object} setting
     */
    this.VariableEdit = function (setting) {
        try {
            if (isInVariableEdit) return;
            isInVariableEdit = true;  // billentyű letiltása, ameddig fut az eljárás

            var thisfn = thispt + 'VariableEdit method: ';
            console.log(thisfn + 'PING setting', setting);
            vrhct.bootbox.edit({
                ajax: {
                    url: imp.Url.VariableEdit,
                    data: { settingJSON: JSON.stringify(setting) }
                },
                title: imp.Titles.EditVariables + ' - ' + imp.Labels.SettingName + ': ' + setting.Name,
                formid: imp.Ids.Form.VariableEdit,
                size: 'large',
                shown: function () {
                    isInVariableEdit = false;
                }
            });
        } catch (e) {
            bootbox.alert(e.message);
        }
    }; // VariableEdit method END

    /*##### PUBLIC METHODS END #####*/


    /*##### PRIVATE FUNCTIONS #####*/

    /**
     * Beállítja a csoport név feletti szűrő inputot
     */
    function SetGroupFilter() {
        var thisfn = thispt + 'SetGroupFilter function: ';
        var currentVal = $('#' + imp.Ids.Input.SelectFilter).val();
        var $filterTH = $('#' + imp.Ids.Input.SelectFilter).parent('th');
        var htmlSelect = $hiddenSelect.parent('div').html();
        console.log(thisfn + '$filterTH, currentVal', $filterTH, currentVal);
        $filterTH.empty();
        $filterTH.append(htmlSelect);
        $filterTH.find('select').attr('id', imp.Ids.Input.SelectFilter).addClass('datatableColumnFilter').val(currentVal);
        setTimeout(function () {
            $('#' + imp.Ids.Input.SelectFilter).on('change', function () {
                console.log(thisfn + 'change event occured: this', this);
                SetGroupHead(this.value);
            });
        }, 1000);
    }; // SetGroupFilter function END

    /**
     * A csoport azonosító alapján elkér a szervertől egy struktúrát,
     * melyben benne van a GroupHeading és GroupHelp.
     * Majd ezeket kiteszi e megfelelő helyekre.
     * 
     * @param {string} groupId Csoport azonosító, melynek címe és magyarázó szövege kell.
     */
    function SetGroupHead(groupId) {
        var thisfn = thispt + 'SetGroupHead function: ';
        //console.log(thisfn + 'groupId = "%s"', groupId);

        if (groupId) {  // ha van groupId, akkor le kell kérni az adatokat
            //console.time('setgrouphead');
            $.ajax({
                url: imp.Url.GetGroup,
                type: "GET",
                cache: false,
                datatype: "json",
                data: { groupId },
                success: function (response) {
                    //console.log(thisfn + 'ajax success: response', response);
                    if (response.ErrorMessage) {
                        $groupTitleI.hide();
                        $groupTitleSpan.html('');
                        $groupTitle.hide();

                        bootbox.alert({ message: response.ErrorMessage, size: 'large' });
                    } else {
                        $groupTitle.show();
                        $groupTitleSpan.html(response.Heading);
                        if (response.Help) { //ha van help szöveg
                            $groupHelpDiv.html(response.Help);
                            //$groupHelpDiv.text(response.Help);
                            $groupTitleI.show();
                        }
                        else {
                            $groupHelpDiv.html('');
                            $groupTitleI.hide();
                        }
                        if (imp.SelectedGroupId === '') {
                            me.ConfigDataTable.Table.columns(0).search(groupId).draw();
                        }
                    }
                    //console.timeEnd('setgrouphead');
                },
                error: function (jqXHR, exception) {
                    console.log(thisfn + 'ajax error: ', exception, jqXHR.responseText);
                    $groupTitleI.hide();
                    $groupTitleSpan.html('');
                    $groupTitle.hide();
                }
            });

        } else { // ha nincs groupId
            $groupTitleI.hide();
            $groupTitleSpan.html('');
            $groupTitle.hide();
            if (imp.SelectedGroupId === '') {
                me.ConfigDataTable.Table.columns(0).search('').draw();
            }
        }
    } // SetGroupHead function END

    /*##### PRIVATE FUNCTIONS END #####*/

} // RedisManagerScripts prototype END

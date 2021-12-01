/***************************************************
RedisManager/Scripts/Manager.js
    A Vonalkód Rendszerház RedisManager webes felület
    kezeléséhez készült metódusok és események.
----------------------
Alapítva:
    2018.10.27.-10.30. Wittmann Antal
Módosult:
****************************************************/
/*##### EVENTS #####*/
$(document).ready(function () {
    'use strict';

    var thisfn = 'RedisManager.Manager.js: ready event: ';
    console.log(thisfn + 'PING');

    // Resize figyelése, hogy a card-body mérete kitöltse a képernyőt
    $(window).resize(function () {
        vrhrms.RedisManagerResize();
    });
    vrhmenu.MenuSwapEventFunction = vrhrms.RedisManagerResize;  // menüváltozáskor is hívja meg a resize-t

    vrhrms.ChangeDataPool();

    console.log(thisfn + 'END');
}); // $(document).ready END 
/*##### EVENTS END #####*/


/**
 * A nyelvi kezelőfelülethez készült prototípus.
 * 
 * @param {ExportToRedisManager} imp Paraméter objektum, cshtml-ben feltöltenő paramétereket tartalmaz.
 */
function RedisManagerScripts(imp) {
    'use strict';

    /*##### PROTOTYPE VARIABLES #####*/
    var me = this;  //hogy a belső függvényekben is tudjak hivatkozni a prototype tulajdonságaira
    var thispt = 'RedisManagerScripts: ';
    var isInVariableDelete = false; // ez jelzi, ha változó törlést kezdeményeztek
    var isInVariableCreate = false; // ez jelzi, ha változó létrehozást kezdeményeztek
    var isInVariableErase = false;  // ez jelzi, ha változó értékének törlését kezdeményezték
    var oldValueForEditable = null; // eredeti érték megőrzése az UpdateValue meghívásakor

    // Selector cache
    var $counter = $('#' + imp.Ids.Counter);
    var $dataPool = $('#' + imp.Ids.Input.DataPool);
    var $dataTable = $('#' + imp.Ids.Table);
    var $instance = $('#' + imp.Ids.Input.Instance);
    var $instCreateBttn = $('#' + imp.Ids.Button.InstanceCreate);
    var $instDeleteBttn = $('#' + imp.Ids.Button.InstanceDelete);
    var $reloadBttn = $('#' + imp.Ids.Button.Reload);
    var $reloadIcon = $reloadBttn.find('i');
    var $reloadAuto = $('#' + imp.Ids.Input.AutoReload);

    // Az automatikus frissítés visszaszámlálásához
    var autoReloadProgressCounter = 0;

    /** Oszlop beállítások a DataTables számára */
    var dtColumns = [
        { data: imp.Data.Instance },
        {
            data: imp.Data.Name,
            render: function (data, type, row) {
                var span1 = document.createElement('span');
                $(span1).addClass('far fa-trash-alt datatableActionIcon ml-1');
                $(span1).attr('onclick', "vrhrms.VariableDelete('" + data + "')");
                $(span1).attr('title', imp.Titles.VariableDelete);

                var temp = document.createElement('div');
                $(temp).append(data);
                $(temp).append(span1);
                return $(temp).html();
            }
        },
        { data: imp.Data.TypeName },
        {
            data: imp.Data.Value,
            render: function (data, type, row) {
                //console.log('Value.render: row.TypeName=%s, imp.DateTimeTypeName=%s', row.TypeName, imp.DateTimeTypeName);
                if (type === 'sort' || type === 'type' || data === imp.EmptyValue) {
                    return data;
                }
                var result = data;
                var err = "Invalid date";
                if (row.TypeName === imp.DateTimeTypeName) {
                    var dt = moment(data).format('L')
                    var tm = moment(data).format('LTS');
                    if (dt === err || tm === err)
                    {
                        console.warn('Datetime string = "%s" is invalid', data);
                        result = data + ' (' + err + ')'; 
                    } else {
                        result = dt + ' ' + tm;
                    }
                } 
                return result;
            },
            createdCell: function (td, cellData, rowData, row, col) {
                //console.log('Value.createdCell: td, cellData, rowData, row, col', td, cellData, rowData, row, col);
                $(td).addClass('datatableEditable');
                $(td).data('instance', rowData.InstanceName);
                $(td).data('variable', rowData.DataKey);
            },
        },
        {
            searchable: false,
            orderable: false,
            render: function (data, type, row) {
                var span = document.createElement('span');
                $(span).addClass('fas fa-eraser datatableActionIcon');
                $(span).attr('onclick', "vrhrms.VariableErase('" + row.InstanceName + "', '" + row.DataKey + "')");
                $(span).attr('title', imp.Titles.VariableErase);

                var $temp = $(document.createElement('div'));
                $temp.append(span);
                return $temp.html();
            },

        },
    ];

    /** Oszlop definíciók a DataTables számára */
    var dtColumnDefs = [
        { targets: [0], width: 80 }, 
        { targets: [1], width: '25%' }, 
        { targets: [2], width: 60 }, 
        { targets: [3], width: '75%' }, 
        { targets: [4], width: 20 } 
    ];
    /*##### PROTOTYPE VARIABLES END #####*/

    /*##### PROPERIES #####*/

    /** Az inicializált tábla VrhDataTable objektuma */
    this.RedisDataTable = null;

    /*##### PROPERIES END #####*/


    /*##### PUBLIC METHODS #####*/

    /// Táblázat kezelés, szűrés frissítés ---------------------------------------------------------------------------------------------------------------

    /** 
     *  Ablak méretezésekor beállítja a táblázat konténerének magasságát.
     */
    this.RedisManagerResize = function () {
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
    }; // RedisManagerResize method END


    /**
     * DataPool változáskor hívódik meg.
     * Ha nem létezik még a RedisDataTable, akkor inicializálja.
     */
    this.ChangeDataPool = function () {
        var thisfn = thispt + 'ChangeDataPool method: ';
        $reloadBttn.prop('disabled', true);

        var poolName = $dataPool.val();
        console.log(thisfn + 'poolName = ', poolName);

        $.ajax({
            type: 'GET',
            url: imp.Url.GetInstanceNames,
            data: { pool: poolName }
        }).done(function (response) {
            if (response.errorMessage) {
                bootbox.alert(response.errorMessage);
            }
            else {
                var firstName = '';
                $instance.empty();
                if (response.instanceNames.length > 0) {
                    response.instanceNames.forEach(function (name, index) {
                        if (index == 0) {
                            firstName = name;
                            $instance.append('<option selected value="' + name + '">' + name + ' </option>');
                        } else {
                            $instance.append('<option value="' + name + '">' + name + ' </option>');
                        }
                    });
                    $instance.append('<option value="">' + imp.Labels.All + ' </option>');
                    $instDeleteBttn.prop('disabled', false);
                } else {
                    $instance.append('<option selected value="">' + imp.Labels.All + ' </option>');
                    $instDeleteBttn.prop('disabled', true);
                }
                var urlGetData = imp.Url.GetData + '&pool=' + poolName + '&instance=' + firstName;
                if (me.RedisDataTable == null) {
                    $dataTable.off('draw.dt');  // táblázat kirajzolásának befejezését jelző esemény tisztítás
                    $dataTable.on('draw.dt', function () { // esemény teendők beállítása
                        var thiset = thisfn + 'RedisTable.draw.dt event: ';
                        // beállítjuk a szerkesztendő cellákat
                        $('.datatableEditable').editable(imp.Url.UpdateData + '&pool=' + poolName, {
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
                                var instanceName = $(this).data('instance');
                                var variableName = $(this).data('variable');
                                console.log(thisfn + 'RedisTable.initComplete: editable.submitdata: instance = %s, variable = %s, original = %s', instanceName, variableName, oldValueForEditable);
                                return { instance: instanceName, variable: variableName, original: oldValueForEditable };
                            },
                            callback: function (result, settings, submitdata) { // az akció sikeres meghívása után vagyunk (ajax.success); A "this" a cella.
                                console.log(thisfn + 'RedisTable.initComplete: editable.callback: result, settings, submitdata', result, settings, submitdata);
                                var vvu = $.parseJSON(result);
                                console.log(thisfn + 'RedisTable.initComplete: editable.callback: vvu', vvu);
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

                        me.RedisManagerResize();

                        $reloadBttn.prop('disabled', false);    // újra engedélyezzük az auto reload gombot
                        $reloadAuto.removeAttr('disabled');     // újra engedélyezzük az auto reload checkbox-ot (!!itt a false-ra állítás nem hozott eredményt!!!)
                        console.log('tábla draw.dt event PING checked?', $reloadAuto.is(':checked'));
                        if ($reloadAuto.is(':checked') && autoReloadProgressCounter === 0) {    // ha bekapcsolt állapotban van az auto relaod, akkor
                            autoReloadProgressCounter = imp.AutoRefresh;    // beállítájuk honnan számoljon vissza
                            $counter.text(autoReloadProgressCounter);   // és jelezzük is ezt a számlálóban
                            setTimeout(progressCounter, 1000);  // majd időzítetten elindítjuk a számlálót
                        }
                    }); // 'draw.dt' event handler END

                    me.RedisDataTable = new VrhDataTable({
                        ajax: { url: urlGetData + '&reload=true' }, // az adatlistát eredményező akció
                        tableId: imp.Ids.Table,
                        columns: dtColumns,
                        columnDefs: dtColumnDefs,
                        lcid: imp.lcid,
                        autoWidth: false,
                        //scrollY: '500px',
                        //scrollCollapse: true,
                        //dom: '<"d-flex justify-content-between"filpr>' + '<"row pb-3"<"col-12"t>>'
                        initComplete: function (setting, json) { // a táblázat elkészültekor meghívott függvény
                            console.log(thisfn + 'RedisTable.initComplete callback: PING');
                        },
                    });
                } else {
                    me.RedisDataTable.Table.ajax.url(urlGetData + '&reload=true').load();
                }
                me.RedisDataTable.Table.ajax.url(urlGetData);
            }
        });
    }; // ChangeDataPool method END

    /**
     * Instance változáskor hívódik meg.
     */
    this.InstanceChange = function (isReload) {
        $reloadBttn.prop('disabled', true);
        $reloadAuto.prop('disabled', true);

        var thisfn = thispt + 'InstanceChange method: ';
        var poolName = $dataPool.val();
        var instanceName = $instance.val();
        console.log(thisfn + 'poolName, instanceName', poolName, instanceName);
        var urlGetData = imp.Url.GetData + '&pool=' + poolName + '&instance=' + instanceName;
        if (isReload) {
            me.RedisDataTable.Table.ajax.url(urlGetData + '&reload=true').load();
            me.RedisDataTable.Table.ajax.url(urlGetData);
        } else {
            me.RedisDataTable.Table.ajax.url(urlGetData).load();
        }
    }; // InstanceChange method END

    this.AutoReload = function (chechBox) {
        if (chechBox.checked) {
            $reloadIcon.addClass('fa-spin');
            me.InstanceChange(true);
        } else {
            $reloadIcon.removeClass('fa-spin');
            $counter.text('');
            autoReloadProgressCounter = 0;
        }
    };

    /// Táblázat kezelés, szűrés frissítés END ------------------------------------------------------------------------------------------------------------


    /// Adatcsoport törlés és létrehozás ----------------------------------------------------------------------------------------------------------------

    /**
     * Egy adatcsoport törlésének indítása.
     */
    this.InstanceDelete = function () {
        try {
            if ($instDeleteBttn.is(':disabled')) return;
            $instDeleteBttn.prop('disabled', true); // billentyű letiltása, az ismétlés elkerülése miatt

            var poolName = $dataPool.val();
            var instanceName = $instance.val();
            console.log(thispt + 'InstanceDelete method: poolName, instanceName', poolName, instanceName);
            vrhct.bootbox.delete({
                ajax: {
                    url: imp.Url.InstanceDelete,
                    data: { pool: poolName, instance: instanceName }
                },
                title: imp.Titles.Confirmation,
                confirm: imp.Confirmations.InstanceDelete.format(instanceName),
                shown: function () {
                    $instDeleteBttn.prop('disabled', false);
                },
                success: function () { // A visszatérő hibaüzenetet a bootbox.delete kezeli.
                    console.log(thispt + 'InstanceDelete method: Sikeres törlés. Itt vagyunk a callback-ben.');
                    me.ChangeDataPool();
                }
            });
        } catch (e) {
            bootbox.alert(e.Message);
        }
    }; // InstanceDelete method END   

    /** 
     *  Adatcsoport (instance) hozzáadása.
     */
    this.InstanceCreate = function () {
        try {
            if ($instCreateBttn.is(':disabled')) return;
            $instCreateBttn.prop('disabled', true); // billentyű letiltása, az ismétlés elkerülése miatt

            var thisfn = thispt + 'InstanceCreate method: ';
            var poolName = $dataPool.val();
            console.log(thisfn + 'PING poolName', poolName);
            vrhct.bootbox.edit({
                ajax: {
                    url: imp.Url.InstanceCreate,
                    data: { pool: poolName }
                },
                title: imp.Titles.InstanceCreate,
                formid: imp.Ids.Form.InstanceCreate,
                size: 'small',
                shown: function () {
                    $instCreateBttn.prop('disabled', false);
                }
            });
        } catch (e) {
            bootbox.alert(e.Message);
        }
    }; // InstanceCreate method END

    /// Adatcsoport törlés és létrehozás END -------------------------------------------------------------------------------------------------------------


    /// Változó (DataKey) törlés, létrehozás és érték törlés ---------------------------------------------------------------------------------------------

    /**
     * Egy változó törlésének indítása.
     * 
     * @param {string} variableName A törlendő változó neve.
     */
    this.VariableDelete = function (variableName) {
        try {
            if (isInVariableDelete) return;
            isInVariableDelete = true;  // billentyű letiltása, ameddig fut az eljárás

            var poolName = $dataPool.val();
            console.log(thispt + 'VariableDelete method: poolName, variableName', poolName, variableName);
            vrhct.bootbox.delete({
                ajax: {
                    url: imp.Url.VariableDelete,
                    data: { pool: poolName, variable: variableName }
                },
                title: imp.Titles.Confirmation,
                confirm: imp.Confirmations.VariableDelete.format(variableName),
                shown: function () {
                    isInVariableDelete = false;
                },
                success: function () { // A visszatérő hibaüzenetet a bootbox.delete() kezeli.
                    console.log(thispt + 'VariableDelete method: Sikeres törlés. Itt vagyunk a callback-ben.');
                    me.InstanceChange(true);
                }
            });
        } catch (e) {
            bootbox.alert(e.Message);
        }
    }; // VariableDelete method END   

    /** 
     *  Változó (DataKey) hozzáadása.
     */
    this.VariableCreate = function () {
        try {
            if (isInVariableCreate) return;
            isInVariableCreate = true;  // billentyű letiltása, ameddig fut az eljárás

            var thisfn = thispt + 'VariableCreate method: ';
            var poolName = $dataPool.val();
            console.log(thisfn + 'PING poolName', poolName);
            vrhct.bootbox.edit({
                ajax: {
                    url: imp.Url.VariableCreate,
                    data: { pool: poolName }
                },
                title: imp.Titles.VariableCreate,
                formid: imp.Ids.Form.VariableCreate,
                size: 'small',
                shown: function () {
                    isInVariableCreate = false;
                }
            });
        } catch (e) {
            bootbox.alert(e.Message);
        }
    }; // VariableCreate method END

    /**
     * Egy változó értékének törlésének (NULL-ra állítás) indítása.
     * 
     * @param {string} instanceName Adatcsoport neve, amelyben törölni kell a változó értékét.
     * @param {string} variableName Változó neve, amely rtéke törölve lesz.
     */
    this.VariableErase = function (instanceName, variableName) {
        try {
            if (isInVariableErase) return;
            isInVariableErase = true;  // billentyű letiltása, ameddig fut az eljárás

            var poolName = $dataPool.val();
            console.log(thispt + 'VariableErase method: poolName=%s, instanceName=%s, variableName=%s', poolName, instanceName, variableName);
            vrhct.bootbox.delete({
                ajax: {
                    url: imp.Url.VariableErase,
                    data: { pool: poolName, instance: instanceName, variable: variableName }
                },
                title: imp.Titles.Confirmation,
                confirm: imp.Confirmations.VariableErase.format(variableName),
                shown: function () {
                    isInVariableErase = false;
                },
                success: function () { // A visszatérő hibaüzenetet a bootbox.delete() kezeli.
                    console.log(thispt + 'VariableErase method: Sikeres törlés. Itt vagyunk a callback-ben.');
                    me.RedisDataTable.Table.draw('page');
                }
            });
        } catch (e) {
            bootbox.alert(e.Message);
        }
    }; // VariableErase method END   

    /// Változó (DataKey) törlés, létrehozás és érték törlés END -----------------------------------------------------------------------------------------

    /*##### PUBLIC METHODS END #####*/


    /*##### PRIVATE FUNCTIONS #####*/

    /** 
     *  Automatikus újratöltés bekapcsolásakorr azonnal történik egy újratöltés.
     *  Ha be van kapcsolva az automatikusa újratöltés, a táblázat kirajzolását 
     *  jelző esemény elindítja ezt a függvényt, amely leszámolja mikor is kell
     *  végrehajtani a következő frissítést.
     */
    function progressCounter() {
        if ($reloadAuto.is(':checked')) {   // még mindig be van kapcsolva az automatikus újratöltés ?
            autoReloadProgressCounter--;
            //console.log('progressCounter', autoReloadProgressCounter);
            if (autoReloadProgressCounter > 0) { // ha még nincs itt az idő, akkor csak kiírja a hátralévő időt
                $counter.text(autoReloadProgressCounter);
                setTimeout(progressCounter, 1000);
            } else {    // ha eljött az idő, akkor 
                $counter.text('');  // törli a számlálót
                me.InstanceChange(true); // és elindítja az újratöltést
            }
        }
    } // progressCounter function END

    /*##### PRIVATE FUNCTIONS END #####*/

} // RedisManagerScripts prototype END

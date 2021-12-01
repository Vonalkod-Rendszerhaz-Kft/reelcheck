/***************************************************
Manager.js
    A Vonalkód Rendszerház webes nyelvi fordítások kezeléséhez
    készült metódusok és események.
----------------------
Alapítva:
    2018.08.28.-09.08. Wittmann Antal
Módosult:
****************************************************/
/*##### EVENTS #####*/
$(document).ready(function () {
    'use strict';

    var thisfn = 'MultiLanguageManager.Manager.js: ready event: ';
    console.log(thisfn + 'PING');

    vrhmlm.InitTable();

    console.log(thisfn + 'END');
}); // $(document).ready END 
/*##### EVENTS END #####*/


/**
 * A nyelvi kezelőfelülethez készült prototípus.
 * 
 * @param {ExportToManager} imp Paraméter objektum, cshtml-ben feltöltenő paramétereket tartalmaz.
 */
function ManagerScripts(imp) {
    'use strict';

    /*##### PROTOTYPE VARIABLES #####*/
    var me = this;  //hogy a belső függvényekben is tudjak hivatkozni a prototype tulajdonságaira
    var thispt = 'ManagerScripts: ';
    var dtColumns = [
        {
            data: imp.dataRefs.Language,
            render: function (data, type, row) {
                if (type === 'sort' || type === 'type') {
                    return data;
                }
                return '<span title="' + row.Language.Name + '">' + data + '</span>';
            }
        },
        { data: imp.dataRefs.WordCode },
        { data: imp.dataRefs.Description },
        { data: imp.dataRefs.Translation },
        { data: imp.dataRefs.Modul },
        {   // a műveletek oszlopa
            render: function (data, type, row) {
                var span = document.createElement('span');
                var lcid = row.Language.LCID;
                var code = row.Word.WordCode;
                if (row.Id === -1) { // nincs fordítás
                    $(span).attr('title', imp.titles.TranslationCreate);
                    $(span).addClass('fas fa-plus-circle datatableActionIcon');
                    $(span).attr('onclick', 'vrhmlm.translationEdit( -1, "' + lcid + '", "' + code + '")');
                } else { // már van fordítás
                    $(span).attr('title', imp.titles.TranslationUpdate);
                    $(span).addClass('fas fa-pencil-alt datatableActionIcon');
                    $(span).attr('onclick', 'vrhmlm.translationEdit(' + row.Id + ', "' + lcid + '", "' + code + '")');
                }
                var temp = document.createElement('div');
                $(temp).append(span);
                return $(temp).html();
            }
        }
    ];
    /*##### PROTOTYPE VARIABLES END #####*/

    /** Az inicializált tábla DataTable objketuma */
    this.TranslationTable = null;

    this.InitTable = function () {
        me.TranslationTable = new VrhDataTable({
            tableId: imp.tableId,
            columns: dtColumns,
            //columnDefs: [
            //    { targets: [0,4], width: '35px' }, // className: 'languageCol'
            //    { targets: [5], width: '20px', orderable: false, searchable: false }
            //],
            columnDefs: [
                { targets: [0,4], width: '35px' }, // className: 'languageCol'
                { targets: [1], className: 'wrapWord' }, 
                { targets: [5], width: '20px', orderable: false, searchable: false }
            ],            ajax: { url: imp.url.getData }, // az adatlistát eredményező akció
            lcid: imp.lcid,
            autoWidth: false
            //dom: '<"d-flex justify-content-between"filpr>' + '<"row pb-3"<"col-12"t>>'
        });
    };

    /**
     * Fordítás szerkesztés ablak inicializálása.
     * 
     * @param {number} id A fordítás egyedi azonosítója. Ha -1, akkor felvitel.
     * @param {string} lcid Nyelvkód, amelyikhez tartozó fordítást szerkeszteni akarod.
     * @param {string} wordCode Szókód, amelyikhez tartozó fordítást szerkeszteni akarod.
     */
    this.translationEdit = function (id, lcid, wordCode) {
        var bttns = null;
        if (imp.iamAdmin) { // ha admin, akkor hozzá adjuk a szókód törlése nyomógombot
            bttns = {
                del: {
                    label: imp.titles.WordCodeDelete,
                    className: 'btn btn-danger mr-auto',
                    callback: function () {
                        me.deleteWordCode($(':input#WordCode').val());
                    }
                }
            };
        }
        vrhct.bootbox.edit({
            ajax: {
                url: imp.url.update,
                data: { translationId: id, lcid: lcid, wordCode: wordCode }
            },
            buttons: bttns,
            title: imp.titles.TranslationUpdate,
            formid: imp.forms.Editor,
            size: 'large'
        });
    }; // translationEdit END


    /// Gyorsítótár újratöltés ----------------------------------------------------------------------------------------------------

    /**
     * Gyorsító tár újratöltése a nyelvi adatbázisból.
     */
    var $reloadBttn = $('#reloadButton');
    var $reloadIcon = $reloadBttn.find('i');
    this.reloadCache = function () {
        if ($reloadBttn.is(':disabled')) return;

        $reloadBttn.prop('disabled', true);
        $reloadIcon.addClass('fa-spin');
        $.ajax({
            cache: false,
            url: imp.url.reloadCache,
            type: 'post',
            contenttype: 'application/json',
            datatype: 'json',
            success: function (response) {
                if (response.ReturnValue !== 0) {
                    bootbox.alert({
                        size: 'large',
                        message: response.ReturnMessage
                    });
                }
                else {
                    bootbox.alert(response.ReturnMessage);
                }
            },
            error: function (jqXHR, exception) {
                console.log('reloadCache method: Ajax hívás sikertelen! ', exception, jqXHR.responseText);
            },
            complete: function () {
                $reloadIcon.removeClass('fa-spin');
                $reloadBttn.prop('disabled', false);
            }
        });
    }; // reloadCache END

    /// Gyorsítótár újratöltés END ----------------------------------------------------------------------------------------------------


    ///Szókódok kezelés ---------------------------------------------------------------------------------------------------------------

    /**
     * Egy szókód törlésének indítása.
     * 
     * @param {any} wordCode A törlendő szókód.
     */
    this.deleteWordCode = function (wordCode) {
        try {
            console.log(thispt + 'deleteWordCode method: wordCode', wordCode);
            vrhct.bootbox.delete({
                ajax: {
                    url: imp.url.deleteWordCode,
                    data: { wordCode: wordCode }
                },
                title: imp.titles.Confirmation,
                confirm: imp.confirmations.WordCodeDelete.format(wordCode),
                success: function () {
                    //console.log(thispt + 'deleteWordCode method: Sikeres törlés. Itt vagyunk a callback-ben.');
                    me.TranslationTable.Table.draw('page');
                }
            });
        } catch (e) {
            bootbox.alert(e.Message);
        }
    }; // deleteWordCode END   

    /** Szókód hozzáadó dialog indítása ajax-ból jövő contenttel */
    var $addwcBttn = $('#addWordCodeButton');
    this.addWordCode = function () {
        if ($addwcBttn.is(':disabled')) return;
        $addwcBttn.prop('disabled', true);

        vrhct.bootbox.edit({
            ajax: {
                url: imp.url.addWordCode
            },
            title: imp.titles.WordCodeAdd,
            formid: imp.forms.CreateWordCode,
            size: 'large',
            shown: function () {
                $addwcBttn.prop('disabled', false);
            }
        });
    }; // addWordCode END

    
    /**
     * Szókód hozzáadó dialogon, ha ki-be pipálja, hogy hozzon létre fordítást is a létrehozandó szókódhoz
     *
     * @param {any} chx Egy checbox objektum, amelyből a metódus meghívódott.
     */
    this.pushDescriptionInTranslationsClick = function (chx) {
        $('#' + imp.forms.CreateWordCode).find('#LanguageCode').prop('disabled', !$(chx).is(':checked'));
    }; // pushDescriptionInTranslationsClick END

    ///Szókód kezelése END ---------------------------------------------------------------------------------------------------------------



    /// Nyelv kezelés --------------------------------------------------------------------------------------------------------------------

    /** Nyelv hozzáadás dialog indítása */
    var $addLanguageBttn = $('#addLanguageButton');
    this.addLanguage = function () {
        if ($addLanguageBttn.is(':disabled')) return;
        $addLanguageBttn.prop('disabled', true);
        vrhct.bootbox.edit({
            ajax: {
                url: imp.url.addLanguage
            },
            title: imp.titles.LanguageAdd,
            formid: imp.forms.CreateLanguage,
            shown: function () {
                $addLanguageBttn.prop('disabled', false);
            }
        });
    }; // addLanguage END

    /**
     * A nyelvmódosító meghívása.
     */
    var $updateLanguageBttn = $('#updateLanguageButton');
    this.updateLanguage = function () {
        if ($updateLanguageBttn.is(':disabled')) return;
        $updateLanguageBttn.prop("disabled", true);
        vrhct.bootbox.edit({
            ajax: {
                url: imp.url.updateLanguage
            },
            title: imp.titles.LanguageUpdate,
            formid: imp.forms.UpdateLanguage,
            shown: function () {
                $updateLanguageBttn.prop('disabled', false);
            }
        });
    }; // updateLanguage END

    /** Nyelv törlése dialog indítása */
    var $deleteLanguageBttn = $('#deleteLanguageButton');
    this.deleteLanguage = function () {
        if ($deleteLanguageBttn.is(':disabled')) return;
        $deleteLanguageBttn.prop('disabled', true);

        vrhct.bootbox.edit({
            ajax: {
                url: imp.url.deleteLanguage
            },
            title: imp.titles.LanguageDelete,
            shown: function () {
                $deleteLanguageBttn.prop('disabled', false);
            },
            buttonsDefault: false,
            buttons: {
                OK: {
                    label: imp.titles.LanguageDelete,
                    className: 'btn btn-danger mr-auto',
                    callback: function () { // ha elindítja a törlés, akkor ez hajtódik végre!
                        var $sel = $('select#LanguageToBeDeleted');
                        removeLanguage($sel.val(), $sel.find('option:selected').text());
                    }
                },
                Cancel: {
                    label: vrhct.bootbox.DefaultOptions.cancelLabel,
                    className: 'btn btn-secondary',
                    callback: function () { }
                }
            }
        });
    }; // deleteLanguage END

    /**
     * Nyelv törlés végrehajtása figyelmeztető üzenettel
     * 
     * @param {string} languageCode A törlendő nyelv kódja.
     * @param {string} languageName A törlendő nyelv neve.
     */
    function removeLanguage(languageCode, languageName) {
        var thisfn = thispt + 'removeLanguage function: ';
        console.log(thisfn + 'languageCode', languageCode);

        //exportTranslationsToCSV(languageCode);

        vrhct.bootbox.delete({
            ajax: {
                url: imp.url.deleteLanguage,
                data: { languageCode: languageCode }
            },
            title: imp.titles.Confirmation,
            confirm: imp.confirmations.LanguageDelete.format(languageName + ' (' + languageCode + ')'),
            success: function () {
                //console.log(thispt + 'removeLanguage function: Sikeres törlés. Itt vagyunk a callback-ben.');
                location.reload();  //érdemes a teljes lapot frissíteni
            }
        });
    } // removeLanguage END

    /// Nyelv kezelés VÉGE -----------------------------------------------------------------------------------------------------------------


    /// Import fájl betöltése ---------------------------------------------------------------------------------------------------------------

    var jqXHRData = null;
    this.importTranslations = function () {
        if (hasDisabled()) {
            console.log('disabled');
            return;
        }
        else {
            console.log('enabled');
        }
        $('#importTranslationsButton').prop("disabled", true);
        $.ajax({
            cache: false,
            url: imp.url.import,
            type: 'get',
            contenttype: 'application/json',
            datatype: 'json',
            success: function (responseData) {
                myCreateBootBoxDialog({
                    title: imp.titles.LoadImport,
                    message: '<div id="importDialog">' + responseData + '</div>',
                    callbackOK: function () {
                        progressDialog({
                            url: imp.url.progressDialogContent,
                            title: imp.titles.OperationProgress,
                            progressMessage: imp.messages.ImportProgress
                        });
                        if (jqXHRData) {
                            jqXHRData.submit();
                        }
                        else {
                            $('#importForm').submit();
                        }
                        return false;
                    },
                    admin: false,
                    showCallback: function () {
                        exportTranslationsToCSV();
                        partialContentAjaxOnComplete(false);
                        $('#importTranslationsButton').prop("disabled", false);
                    }
                });
            },
            error: function (jqXHR, exception) {
                console.log('Ajax hívás sikertelen! ', jqXHR.responseText);
            },
            complete: function () {
            }
        });
    }; // importTranslations END

    function ajaxFormSubmitComplete(data) {
        $('#importDialog').html(data.responseJSON.resultHtml);
    }

    function partialContentAjaxOnComplete(closeProgress) {
        if (closeProgress) {
            progress.modal('hide');
        }
        $('#fileUploadButton').fileupload({
            url: imp.url.import,
            dataType: 'json',
            type: 'post',
            add: function (e, data) {
                jqXHRData = data;
            },
            done: function (event, data) {
                if (data.result.isUploaded) {
                    $("#file-path").val(imp.messages.ImportLoadNoFile);
                }
            },
            fail: function (event, data) {
                console.log('ERROR: file upload fail');
            },
            always: function (event, data) {
                if (data.jqXHR.responseJSON.isUploaded) {
                    window.location = data.jqXHR.responseJSON.resultHtml;
                }
                else {
                    $('#importDialog').html(data.jqXHR.responseJSON.resultHtml);
                    progress.modal('hide');
                }
            },
            progress: function (e, data) {
                var progress = parseInt(data.loaded / data.total * 100, 10);
            }
        });
        $("#fileUploadButton").on('change', function () {
            $("#file-path").val(this.files[0].name);
        });
        bootBoxAjaxOnComplete('#importForm');
    } // partialContentAjaxOnComplete END

    ///-------------------------------------------------------------------------------------------------------------- Import fájl betöltése



    var progress;

    function exportTranslationsToCSV(languageCode) {
        progressDialog({
            url: imp.url.progressDialogContent,
            title: imp.titles.OperationProgress,
            progressMessage: imp.messages.BackupProgress
        });
        $.ajax({
            cache: false,
            url: imp.url.getDataForBackup,
            type: 'get',
            contenttype: 'application/json',
            datatype: 'json',
            data: { languageCode: languageCode },
            success: function (responseData) {
                if (responseData.sError !== '') {
                    bootbox.alert(responseData.sError, null);
                }
                else {
                    // Encoder in StringEncoderForExcelCompatibleCSVExport.js
                    // Initialize new encoder
                    encoder = new DataEnc({
                        mime: 'text/csv',
                        charset: 'UTF-16LE',
                        bom: true
                    });
                    encoder.enc(responseData.csv);
                    // Data URI
                    csvData = encoder.pay();
                    if (typeof navigator !== "undefined" &&
                        /MSIE [1-9]\./.test(navigator.userAgent) || navigator.msSaveOrOpenBlob) {//IE10, IE9-
                        var saveTxtWindow = window.frames.saveTxtWindow;
                        if (!saveTxtWindow) {
                            saveTxtWindow = document.createElement('iframe');
                            saveTxtWindow.id = 'saveTxtWindow';
                            saveTxtWindow.style.display = 'none';
                            document.body.insertBefore(saveTxtWindow, null);
                            saveTxtWindow = window.frames.saveTxtWindow;
                            if (!saveTxtWindow) {
                                saveTxtWindow = window.open('text/csv', '_temp', 'width=100,height=100');
                                if (!saveTxtWindow) {
                                    window.alert('Sorry, download file could not be created.');
                                    return false;
                                }
                            }
                        }
                        var doc = saveTxtWindow.document;
                        doc.open('text/html', 'replace');
                        doc.charset = 'UTF-16LE';
                        doc.write(responseData.csv);
                        doc.close();
                        var retValue = doc.execCommand('SaveAs', null, responseData.fileName);
                        saveTxtWindow.close();
                    }
                    else { // Minden más (Chrome, Firefox) SAFARI-n nem megy!
                        doc = window.document;
                        save_link = doc.createElementNS("http://www.w3.org/1999/xhtml", "a");
                        save_link.href = csvData;
                        save_link.download = responseData.fileName;
                        var event = doc.createEvent("MouseEvents");
                        event.initMouseEvent(
                            "click", true, false, window, 0, 0, 0, 0, 0
                            , false, false, false, false, 0, null
                        );
                        save_link.dispatchEvent(event);
                    }
                }
            },
            error: function (jqXHR, exception) {
                console.log('Ajax hívás sikertelen! ', jqXHR.responseText);
            },
            complete: function (progressDialog) {
                progress.modal('hide');
            }
        });
    }

    function hasDisabled() {
        return Boolean($(':disabled#addWordCodeButton').length ||
            $(':disabled#addLanguageButton').length ||
            $(':disabled#deleteLanguageButton').length ||
            $(':disabled#importTranslationsButton').length ||
            $(':disabled#reloadButton').length) ||
            $(':disabled#updateLanguageButton').length;
    }

    /**
     * WordCode szerkeszthetőséggel kapcsolatos függvény
     * 
     * @param {any} element valami ???
     */
    this.ClickWordCodeEditEnable = function (element) {
        var $code = $('input#WordCode');
        var $desc = $('textarea#WordCodeDescription');
        var $modul = $('input#WordCodeModul');
        if (!element.checked) {
            $code.val($('input#OriginalWordCode').val());
            $desc.val($('input#OriginalWordCodeDescription').val());
            $desc.val($('input#OriginalWordCodeModul').val());
        }
        $code.prop('readonly', !element.checked);
        $desc.prop('readonly', !element.checked);
        $modul.prop('readonly', !element.checked);
    };

} // ManagerScripts prototype END

String.prototype.endsWithAny = function () {
    var strArray = Array.prototype.slice.call(arguments),
        $this = this.toLowerCase().toString();
    for (var i = 0; i < strArray.length; i++) {
        if ($this.indexOf(strArray[i], $this.length - strArray[i].length) !== -1) return true;
    }
    return false;
};



/***************************************************
VRHAutoComp.js
    A VRH AutoComplete megoldásához szükséges scriptek.

    listid: annak az inputnak az id-ja (#nélkül), amelyben a megjelenítendő név van a listából
    targetid: annak a rejtett inputnak az id-ja (#nélkül), amelybe a kiválasztott elem id-ját kell beletenni 
    url: a listát feltöltő action elérése
    filterid: annak a mezőnek az id-ja, melynek értéke esetleg szükséges lehet a lista előállításánál
----------------------
Készült :
    2015.05.12-14. Wittmann Antal
Módosult:
    2015.05.18. Wittmann Antal, az targetid paraméter bevezetése.
****************************************************/
function AutoCompInit(listid, targetid, url, filterid) {
    var _listid = '#' + listid;
    var _targetid = '#' + targetid;
    $(_listid).autocomplete({
        source: function (request, response) {
            if (filterid === undefined) {
                $.ajax({
                    url: url,
                    type: "POST",
                    dataType: "json",
                    data: { query: request.term },  // query tartalmazza, amit begépelt
                    success: function (data) {
                        response($.map(data, function (item) {
                            return { label: item.Text, value: item.Text, id: item.Id }
                        }));
                    }
                });
            } else {
                var _filterid = '#' + filterid;
                $.ajax({
                    url: url,
                    type: "POST",
                    dataType: "json",
                    data: { query: request.term, filter: $(_filterid).val() },  
                    success: function (data) {
                        response($.map(data, function (item) {
                            return { label: item.Text, value: item.Text, id: item.Id }
                        }));
                    }
                });
            }

        },
        change: function (event, ui) {
            if (!ui.item) {
                $(_targetid).val(-1);
            }
            else {
                $(_targetid).val(ui.item.id);
            }
        },
        select: function (event, ui) {
            var v = ui.item.id;
            $(_targetid).val(ui.item.id);
        },
        autoFocus: true,
        open: function () {
            $(this).autocomplete('widget').css('z-index', 99999);
            $('.ui-autocomplete').css('height', 'auto');
            var $input = $(this),
                        inputTop = $input.offset().top,
                        inputHeight = $input.height(),
                        autocompleteHeight = $(this).autocomplete('widget').height(),//$('.ui-autocomplete').height(),
                        windowHeight = $(window).height();

            if ((inputHeight + inputTop + autocompleteHeight) > windowHeight) {
                $('.ui-autocomplete').css('height', (windowHeight - inputHeight - inputTop - 20) + 'px');
            }
        },
        minLength: 0, //hány karakter esetén nyiljon meg, 0 = akkor is megnyilik, ha nincs karakter beütve
    });

}
function AutoCompClickOnSearch(elem) {
    var item = '#' + elem;
    $(item).val("");
    $(item).autocomplete("search", "");
    $(item).trigger("focus");
}

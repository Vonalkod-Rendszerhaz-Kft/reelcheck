/***************************************************
MasterData.js
    A Vonalkód Rendszerház webes törzsadatkezelő
    kerethez készült metódusok és események.
----------------------
Alapítva:
    2018.06.12. Wittmann Antal
Módosult:
****************************************************/

/*##### EVENTS #####*/
$(function () {
    'use strict';

    var thisfn = vrhmdata.constants.THISFILE + 'ready event: ';
    console.log(thisfn + 'PING');

    //console.log(thisfn + 'END');
}); // $(document).ready END 
/*##### EVENTS END #####*/


function MasterDataScripts() {
    'use strict';

    /*##### PROPERTIES #####*/
    this.constants = {
        LCID_ENUS: 'en-US',
        THISFILE: 'MasterData.js: '
    };

    /* SELECTORS' CASH  ! a ready-ben érdemes beállítani ! */
    this.$grid = '';
    /* SELECTORS' CASH END */

    /*##### PROPERTIES END #####*/


    /*##### PROTOTYPE VARIABLES #####*/
    var me = this;  //hogy a belső függvényekben is tudjak hivatkozni a prototype tulajdonságaira

    var languages = new Array();
    languages.push({
        lcid: me.constants.LCID_ENUS,
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
            "processing": "Processing...",
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
        lcid: 'hu-HU',
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
            "processing": "Feldolgozás...",
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
    /*##### PROTOTYPE VARIABLES END #####*/


    /*##### METHODS #####*/

    /**
     * A nyelvi kód alapján visszatér egy a DataTables számára megfelelő 
     * nyelvi objektummal. Ha nincs a készletben a kért nyelv, akkor
     * az angol nyelvi beállítással tér vissza.
     * 
     * @param {any} lcid : nyelvi kód (LCID)
     *
     * @return {any} : Egy nyelvi objektum.
     */
    this.GetLanguage = function (lcid) {
        var enusLang = null;
        for (var i = 0; i < languages.length; i++) {
            if (languages[i].lcid === lcid) {
                return languages[i].language;
            } else if (languages[i].lcid === me.constants.LCID_ENUS) {
                enusLang = languages[i].language;
            }
        }
        return enusLang;
    };

    /*##### METHODS END #####*/

} // MasterDataScripts prototype END
try {
    var vrhmdata = new MasterDataScripts();
} catch (e) {
    console.log(e);
}



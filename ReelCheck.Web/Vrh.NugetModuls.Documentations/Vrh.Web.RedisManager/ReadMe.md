# Vrh.Web.RedisManager - Redis DataPool elemek megjelenítése és kezelése
> Fejlesztve és tesztelve **4.5.1** .NET framework alatt. Más framework támogatása további
>  tesztelés függvénye, de nem kizárt alacsonyabb verziókban való mûködõképessége.

WEB felület Redis DataPool handlerek alatt tárolt adatok megjelenítésére és kezelésére.

****
# Version History:

#### 2.1.0 (2019.05.09) Compatible changes - debug
- Átállás a Vrh.Web.Common.Lib 2.0.0 változatára.
- Vrh.XmlProcessing 1.0.0 hozzáadása.
- Frissítés a Vrh.Common 2.3.0 hozzáadása.
- Frissítés a Vrh.Web.Menu 1.14.0 változatára.
- Frissítés a VRH.Log4Pro.MultiLanguageManager 3.4.0 változatára.
- Frissítés a Moment.js 2.24.0 változatára.
- Frissítés a popper.js 1.14.3 változatára.
- Frissítés a Vrh.Redis.DataPoolHandler 1.10.3 változatára.

#### 2.0.0 (2018.11.05) Initial version - debug
- Az új alapösszetevõk bevezetése (Bootsrap 4.4.1, jQuery 3.3.1, Font Awesome 5.4.1, stb.)
- Az új rendszerû DataTables felületek megvalósítása.
- Instance és változó kezelés gombokkal, szerkesztés ablakban.
- Érték módosítása és törlése a saját oszlopban ablak nélkül.
- Gyorsítótár a szûrésekhez és kereséshez a letöltött adatokban. Csak adatmezõ és adatcsoport 
változtatáskor (másik, hozzáadás, törlés) töltõdnek újra az adatok a Redis kiszolgálóról.

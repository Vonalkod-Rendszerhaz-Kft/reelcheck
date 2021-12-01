# Vrh.Web.FileManager - mappa és fájlkezelő, le és feltöltési szolgáltatással
> Fejlesztve és tesztelve **4.5** .NET framework alatt. Más framework támogatása további tesztelés függvénye, 
> de nem kizárt alacsonyabb verziókban való működőképessége.

A FileManager modul az xml paraméter file-jának megfelelően megjeleníti a szerver 
megadott mappájában egy megadott maszknak megfelelő mappákat és fájlokat. 
Lehetőséget ad a fájlok megnyitására, letöltésére, törlésére, illetve feltöltésére.

A komponens meghívható akcióit a **FileManager area** és **FileManager controller** 
elemeken keresztül lehet elérni.
Példa egy URL-re: *[application]/FileManager/FileManager/Index*

## Akciók
* **Index** akció: Az XML-ben lévő definíciók alapján a fájlok és mappák megjelenítése.

### Index akció
Paraméter|Leírás
:----|:------
xml|XmlParser kapcsolati sztring. Ha üres, akkor a "config=FileManager;" lesz az alapértelmezés.
id|Melyik definíció alapján működjön a FileManager. 
lcid|A hívó által kért nyelv érvényes kódja. Ha üres, akkor a releváns nyelvi kód lesz.

### Paraméterező XML fájl felépítése
Az XML részletekben a "<>" jelek közötti részek tartalmazzák az elemneveket és attribútumokat.
Ahol "+" jel van, azok egy zárt elemet jelentenek, egy későbbi címben lesznek kifejtve. 
Így tárva fel a teljes szerkezetet.
#### FileManager xml szerkezet
```xml
<FileManager>
    <XmlParser>
    <General>
        <DefaultDefinition>+
    </General>
    <Definitions>+
</FileManager>
```
A FileManager gyökér elembe ágyazva kell szerepeltetni a FileManager számára szóló 
definíciókat és paraméterezést.

Elem|Leírás
:----|:------
XmlParser|Az egységes xml feldolgozás eleme. Változók gyűjteménye is itt.
General|Az teljes XML tartalomra vonatkozó beállítások helye.
DefaultDefinition|Egy [Definition elem](####Definition-elem), melyben alapértelmezett értékeket lehet megadni.
Definitions|Definíciók gyűjteménye, mely meghatározza a FileManager működési keretparamétereit.

#### Definition elem
```xml
<Definitions>
    <Definition>
        <Format title="fejléc" columns="auto" rowheight="22"/>
        <Order by="name|date|size" direction="asc"/>
        <Enable subdirs="true" download="true" upload="true" open="true" delete="false" overwrite="?" />
        <Masks>+ 
        <RootFolder>Mappa</RootFolder>
        <Help>Szöveg</Help>
    </Definition>
</Definitions>
```
A DownloadDefintions-ban található a DownloadDefinition elemek (definíciók), melyben a FileManager működésére
vonatkozó paraméterek találhatók. Egy definíció egy adott mappát (RootFolder) határoz meg, mely a FileManager
számára egy gyökér mappa, melytől engedélyezett a fájlok és mappák böngészése.

Elem|Leírás
:----|:------
Format|Megjelenítésre vonatkozó értékek. Attribútumok magyarázatát lásd alább.
Order|A mappák és fájlok sorba rendezésére vonatkozó beállítások.
Enable|A működésre vonatkozó engedélyek és előírások beállításai.
Masks|A mappákra és fájlokra vonatkozó szűrési feltételek leírása itt található.
RootFolder|Ez a mappa a FileManager számára a gyökér mappa. Ebben és innen lefelé lehet böngészni.
Help|A FileManager fejléce alatt kiírandó tájékoztató szöveg.

Attribútum|Leírás
:----|:------
title|A FileManager fejléce, címe.
columns|A nevek ennyi oszlopra rendezve jelennek meg. Az "auto" érték jelenti, hogy a FileManager számítsa ki az oszopok számát. Alapértelmezett értéke: "auto".
rowheight|Ilyen magas sorokban jelnnek meg a nevek. Alapértelmezett értéke: 20.
by|Rendezés módja. Lehetséges értékei: name, date, size. Alapértelmezett értéke: name.
direction|Rendezés iránya. Lehetslges értékei: asc, desc. Alapértelmezett értéke: asc.
subdirs|Mappák megjelenítésének engedélyezése (true/false). Alapértelmezett értéke: true.
download|Fájlok letöltésének engedélyezése (true/false). Alapértelmezett értéke: true.
upload|Fájlok feltöltésének engedélyezése (true/false). Alapértelmezett értéke: true.
open|Fájlok megnyitásának engedélyezése (true/false). Alapértelmezett értéke: true.
delete|Fájlok törlésének engedélyezése (true/false). Alapértelmezett értéke: false.
overwrite|Fájlok felülírásának engedélyezése (true/false/?). Kérdőjel esetén rákérdez. Alapértelmezett értéke: ?.

****
# Version History:

#### 1.3.0 (2019.05.14) Compatible changes - debug
- Átállás a Vrh.Web.Common.Lib 2.0.0 változatára.
- Frissítés a Vrh.Common 2.3.0 hozzáadása.
- Frissítés a Vrh.Web.Menu 1.14.0 változatára.
- Frissítés a VRH.Log4Pro.MultiLanguageManager 3.4.0 változatára.

#### 1.2.2 (2018.07.30) Patches - debug
- Frissítés a Vrh.Web.Common.Lib 1.9.0-ás változatára.

#### 1.2.1 (2018.07.26) Patches - debug
- Frissítés a Vrh.Web.Common.Lib 1.8.2-es változatára.

#### 1.2.0 (2018.07.16) Compatible API change - debug
- Vrh.Web.Common.Lib 1.8.0-ás verziójának beépítése.
- XmlParser kapcsolati sztring (xml paraméter) bevezetése.
- URL paraméterek új módszerrel (ParameterQuery) történő feldolgozása és a 
plusz paraméterek hozzáadása az XmlVars gyűjteményhez az Index akciónál.

#### 1.1.2 (2018.06.07) Patches - debug
- Vrh.Web.Common.Lib 1.7.0-ás verziójának beépítése.

#### 1.1.1 (2018.03.07) Patches
- Vrh.Web.Common.Lib újabb verzió.

#### 1.1.0 (2018.03.02) Compatible API change
- XmlParser osztály felhasználása a feldolgozáskor, és a működés e koncepció szerinti módosítása.

#### 1.0.1 (2018.01.15) Patches
- XML elemek átnevezése, és egyéb apró módosítások.

#### 1.0.0 (2018.01.12) Initial version
- A régi MvcDownloader ezen a néven folytatja pályafutását. Egy kicsit korszerűsítve, a már kész 
NuGet csomagok beépítésével, és az XML feldolgozás átírásával.

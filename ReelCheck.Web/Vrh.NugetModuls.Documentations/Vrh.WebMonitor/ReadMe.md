# Vrh.WebMonitor
> Fejlesztve és tesztelve **4.5** .NET framework alatt. Más framework támogatása további tesztelés függvénye, 
> de nem kizárt alacsonyabb verziókban való működőképessége.

A Webmonitor egy web alapú megjelenítő keret, egy táblázat (Matrix1D), 
és egy cella (Matrix2D) alapú megjelenítő modullal.

A komponens indítása a Display akcióval lehetséges, amelyet
a **WebMonitor area** és **WebMonitor controller** 
elemeken keresztül lehet elérni.
Példa egy URL-re: *[application]/WebMonitor/WebMonitor/Display*

## Akciók
* WebMonitor indítása (**Display** akció)

### Display akció
Paraméter|Leírás
:----|:------
BlockName|Melyik profilt kell megjeleníteni. Ha üres, akkor a legutoljára megjelenített, ha az sincs, akkor a legelső. 
XMLFile|A "Nyelvi változatokat leíró XML file" neve a teljes vagy relatív[&#126;] elérési útjával együtt. Alapértelmezés: "~/App_Data/WebMonitor/WebMonitors.xml"
usegroup|Felhasználói csoport neve. Azt a profilt lehet megjeleníteni, amelyikben ez a csoport szerepel. Ha üres, akkor az összes profil elérhető.

### Matrix2D paraméterező XML fájl felépítése
Az XML részletekben a "<>" jelek közötti részek tartalmazzák az elemneveket és attribútumokat.
Ahol "+" jel van, azok egy zárt elemet jelentenek, egy későbbi címben lesznek kifejtve. 
Így tárva fel a teljes szerkezetet.
#### Matrix2D a gyökér elem
```xml
<Matrix2D>
    <GeneralParameters>
        <Options RefreshInterval="0;0" CheckXML="true"/>
    </GeneralParameters>
    <Defaults>
        <DataSource>+
        <DefaultStyles Display="DefaultDisplay" FieldCell="CellStyle_Default" LabelCell="CellStyle_Default" FieldText="TextStyle_Default" LabelText="TextStyle_Default" />
    </Defaults>
    <Displays>+
</Matrix2D>
```
Elem|Leírás
:----|:------
Generalparameters|A működést befolyásoló általános értékek gyűjtőhelye.
Options|Speciális beállítások, értékek.
Defaults|Alapértelmezett értékeket tartalmazó elem.
DataSource|A Matrix2D számára szükséges adatok forrásának paraméterei.
DefaultStyles|Alapértelmezett stílusok megadása.
Displays|A ```<Display>``` elemek gyűjteménye itt található.

Attribútum|Leírás
:----|:------
RefreshInterval|A Matrix2D megjelenésének frissítési gyakorisága.
CheckXML|Legyen-e szintaktikus ellenőrzés az XML fájl feldolgozásakor.
Display|A megjelenő kép alapértelmezett stílusa.
FieldCell|A mező értéket tartalmazó külső cella alapértelmezett stílusa.
LabelCell|A címke értéket tartalmazó külső cella alapértelmezett stílusa.
FieldText|A mező értéket tartalmazó belső cella alapértelmezett stílusa.
LabelCell|A címke értéket tartalmazó belső cella alapértelmezett stílusa.

#### DataSource elem
```xml
<DataSource RedisPool="ALM" RedisServer="127.0.0.1" RedisPort="6379"/>
vagy
<DataSource Type="sql" KeyParameterName="keyp" 
            ConnectionString="data source=(local)\SQLEXPRESS;initial catalog=LearALM;integrated security=True;MultipleActiveResultSets=True;">
    <SQL>
        select a.*, b.* from dbo.WebMonitorA a inner join dbo.WebMonitorB b on a.MKeyA = b.MKeyB where a.MKeyA=@keyp
    </SQL>
</DataSource>
```

Elem|Leírás
:----|:------
SQL|Ha a forrás típusa 'sql', akkor ebben az elemben szerepel a lekérdezés scriptje.

Attribútum|Leírás
:----|:------
Type|Az adatforrás típusa. Lehetséges értékei: 'redis' és 'sql'. Alapértelmezett értéke: 'redis'
RedisPool|Redis tároló esetén a Pool megnevezése.
RedisServer|Redis tároló esetén a kiszolgáló IP címe.
RedisPort|Redis tároló esetén a port száma.
ConnectionString|SQL típus esetén a kapcsolati string.
KeyParameterName|SQL típus esetén annak a paraméternek a neve, ahova a gyártósor azonosítóját kell behelyettesíteni. Ha üres, akkor nem lesz paraméter átadva a lekérdezésnek.


***
## Version History:
#### 2.8.0 (2019.05.14) Compatible changes (debug):
- Átállás a Vrh.Web.Common.Lib 2.0.0 változatára.
- Vrh.XmlParser 1.0.0 hozzáadása.
- Vrh.Common 2.3.0 hozzáadása.
- Frissítés a VRH.Log4Pro.MultiLanguageManager 3.4.0 változatára.
- Frissítés a Vrh.Web.Menu 1.14.0 változatára.


#### 2.7.0 (2019.01.31) Compatible changes (debug):
- A Redis connectionstring-be bekerült a Serialization paraméter. 
Értékei JSON, vagy XML, és ennek feldogozása is megtörténik. M1D és M2D érintett.
- Frissítés a VRH.Log4Pro.MultiLanguageManager 3.1.0 változatára.
- Frissítés a Vrh.Web.Common.Lib 1.17.0 változatára.
- Frissítés a Vrh.Web.Menu 1.12.0 változatára.

#### 2.6.3 Patches:
- Redis.DataPoolHandler dependency update to 1.7.1 (hogy ne a braking.es dll-t hurcolja)

#### 2.6.2 Patches:
- Redis.DataPoolHandler dependency update to 1.7.0

#### 2.6.1 (2018.11.14) Patches (debug):
- Matrix2D Link típusú cella AutoClose korlát megszűnt. 0-nál nagyobb érték esetén bekapcsol.
- Matrix2D.GetValues pontosabb hibaüzenetetjelenít meg.

#### 2.6.0 (2018.10.18) Compatible changes (debug):
- TextMask konverziós típus bevezetése.
- Egy központi statikus osztályban került az adatlista értékeinek kiszámítása a 
konverziós típusokkal. M1D és M2D is ezt használja.
- ```#$WORDCODE_TEST``` és ```#$TEXTMASK_TEST``` speciális változó bevezetése tesztelési célokra.

#### 2.5.8 (2018.09.24) Patches (debug):
- Matrix1D és Matrix2D 'StyleDefinitions' és 'FieldDefinitions' elemekben a ConfigFile egy
xml connection string.

#### 2.5.7 (2018.09.04) Pathes (debug):
- #$USERROLES speciális változó hozzáadásának javítása.

#### 2.5.6 (2018.09.03) Patches (debug):
- #$USERROLES speciális változó hozzáadása. A bejelentkezett felhasználó szerepeinek listája.

#### 2.5.5 (2018.08.29) Patches (debug):
- Webmonitor.css kiegészült egy iframe elemre vonatkozó stílus beállítással
- Matrix2D Index.cshtml-ben a body mérete .1vmax-ra módosult, hogy kis dobozok
esetén se zavarjon bele a méretezésbe.

#### 2.5.4 (2018.08.29) Patches (debug):
- #$USERNAME speciális változó hozzáadása. A bejelentkezett felhasználó neve.

#### 2.5.3 (2018.08.09) Patches (debug):
- A Matrix2D nem egyesével, hanem a menü regisztrációkor megadott bundles csomagokat tölti be.
- Frissítés a Vrh.Web.Menu 1.3.0-ás változatára.
- Frissítés a Vrh.Web.Common.Lib 1.9.2-es változatára.

#### 2.5.2 (2018.08.05) Patches (debug):
- A Matrix2D-ben betöltődik az összes Log4Pro alkalmazás összetevő, hogy
itt is működjenek az egyéb szolgáltatások (ablaknyitás, formkezelés, stb.).

#### 2.5.1 (2018.08.01) Patches (debug):
- Matrix1D-ben is működnek a WordCode-os Field-ek.

#### 2.5.0 (2018.07.31) Compatible API changes (debug):
- Lehet több speciális változó. Jelenlegiek: #$DUMMY és #$DT, de egyszerűen bővíthető.
- Frissítés a Vrh.Web.Common.Lib 1.9.0-es változatára, a ParameterSeparating metódus miatt.
- A 'Conversion' attribútumban jelölhető, ha a változó értéke egy szókód.

#### 2.4.3 (2018.07.27) Patches (debug):
- Matrix1D-be is megérkeznek az URL megadott változók.
- A ListObjects akciók is továbbadják az Xmlparser-nak az elvárton kívül érkező URL paramétereket.

#### 2.4.2 (2018.07.26) Patches (debug):
- Matrix2D is továbbküldi az egyéb változókat az XmlParser felé.

#### 2.4.1 (2018.07.26) Patches (debug):
- WebMonitor, Matrix1D és Matrix2D xml paraméterfájlok gyökerét átnevezi, ha az eltér az elvárttól.

#### 2.4.0 (2018.07.26) Compatible API changes (debug):
- Redis kapcsolati sztring bevezetése a Matrix2D-ben.
- Vrh.Web.Common.Lib 1.8.2 szükséges hozzá.

#### 2.3.0 (2018.07.24) Compatible API changes (debug):
- WebMonitors.xml megszüntetése.
- Display és Index akciók 'xmlfile' paramétere 'xml' lett, és XmlParser kapcsolati stringet vár.
- Az elvárt paramétereken túli paramétereket hozzáadja az XmlVars gyűjteményhez.
- ```<StyleDefinitions>``` és ```<FieldDefinitions>``` bevezetése a paraméterfájlokban.
- ListObjects akciók átírása az új feltételek szerint.

#### 2.2.1 (2018.07.20) Patches (debug):
- Cell.Ref számolási hiba javítása.

#### 2.2.0 (2018.07.19) Compatible API changes (debug):
- Kapcsolati string alapú XmlParser konstruktor használata.

#### 2.1.0 (2018.06.30) Compatible API changes (debug):
- Link típusú cellák bevezetése.
- "Name", "Ref" és "Help" attribútum bevezetése és felhasználása.

#### 2.0.2 (2018.06.13) Patches:
- A WebMonitor.js-ben ki volt "kommentezve" a bármely weblapot megjelenítő
rész. Most már nincs.
- A DisplayBlock előállításakor az Area="*" esetén már nem kerülnek
felesleges jelek a Parameters tulajdonság értékébe.

#### 2.0.1 (2018.06.07) Patches:
- A WebMonitor.js-ben a "db.id"-t le kellett cserélni "db.Id"-re. Így a megjelölt keret frissítése fog megtörténni, nem csak az első kereté.

#### 2.0.0 (2018.06.06) Incompatible API changes:
- Vrh.Web.Menu fölé megvalósított változat.

#### 1.2.1 (2018.05.31) Patches:
- WebMonitor és WorkPlaceGroups definíciós fájlok is tartalmazhatnak XmlParser elemet.

#### 1.2.0 (2018.05.30) Compatible API Change:
- Fields és Styles struktúrák szétválasztása (#10821)
- Matrix2D-ben XML fájl alapú adatforrás bevezetése (#10810)
- 'xmlfile' akció paraméter feloldása mindenhol azonos (#10773)
- Megoldás született az osztályszintű szerializácó és az XmlParser összedolgozására (#10823)

#### 1.1.0 (2018.02.26) Compatible API Change:
- Matrix2D SQL-es lekérdezéssel is ellátható adatokkal.

#### 1.0.2 (2018.02.26) Patches:
- Memóriaszivárgás megszüntetése (eseménykezelőkről mindig le kell irítkozni, mert tartják a referenciaá!!!)
- Memóriafelszabadítás javítása LinqXMLProcessorBaseClass alapú olvasások usingba, hogy a dispose hívás megtörténjen rögtön a scope elhagyásával (Ez a komponens nem webes felhazsnáláshoz van optimalizálva!!!)

#### 1.0.1 (2017.11.17) Patches
- MonitorProfileGroup-ok frissítését kellet korrigálni.

#### 1.0.0 (2017.11.15) Initial version
- A Lear ALM projektből került ki ebben a változatban. Önálló projektként létezése az átemeléskori állapotában kezdődik.




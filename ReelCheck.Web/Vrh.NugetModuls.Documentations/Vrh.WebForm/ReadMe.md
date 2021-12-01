# Vrh.WebForm - adatbekérés, paraméterek összeállítása, akció indítása."
> Fejlesztve és tesztelve **4.5** .NET framework alatt. Más framework támogatása további tesztelés függvénye, 
> de nem kizárt alacsonyabb verziókban való működőképessége.

A WebForms program lehetővé teszi adatok bekérését a felhasználótól a 
kezelőfelületen keresztül, majd részben ezeket, részben a leíró xml file-jában 
definiált paramétereket felhasználva web alkalmazások indítását. Az elindított
web alkalmazásnak lehetnek visszatérő adatai, ezek - egyenlőre - egy int és egy
string típusú érték, amelyeket a Webform egy ablakban megjeleníti. Ha a web 
alkalmazásnak nincsenek visszatérő értékei, akkor egy teljes weboldalt kell 
előállítson, amelyet a WebForm a kívánt helyen (helyben vagy egy új tabon) 
megjelenít.

A komponens jelenleg egy meghívható akciót nyújt, amelyet
a **WebForm area** és **WebForm controller** 
elemeken keresztül lehet elérni.
Példa egy URL-re: *[application]/WebForm/WebForm/Execute*

## Akciók
* **Execute** akció: WebForm hívás, indítás

### Execute akció
Paraméter|Leírás
:----|:------
xml|XmlParser kapcsolati string. Ha üres, akkor a leírókat tartalmazó XML fájl alapértelmezése: "&#126;/App_Data/WebForm/WebForms.xml"
formName|Melyik WebForm adatbekérő felületet kell megjeleníteni. 
viewmode|=mobile: LayoutMobile-ban megadott elrendezést használja, WebForm help nem jelenik meg, és a mezők egy oszlopban kerülnek elhelyezésre.<br /> =desktop: asztali mód; LayoutMobile-ban megadott elrendezést használja, WebForm help megjelenik és a mezők a definiált pozícióik szerint kerülnek elhelyezésre.

### Paraméterező XML fájl felépítése
Az XML részletekben a "<>" jelek közötti részek tartalmazzák az elemneveket és attribútumokat.
Ahol "+" jel van, azok egy zárt elemet jelentenek, egy későbbi címben lesznek kifejtve. 
Így tárva fel a teljes szerkezetet.
#### WebFormXML a gyökér elem
```xml
<WebFormXML>
    <DefaultLCID>alapértelmezett nyelvkód</DefaultLCID>
    <strings>+
    <connectionstrings>+
    <ExternalParameters>+
    <CommonElementsDefaults>+
    <CommonElementsInputs>+
    <CommonElementsActions>+
    <CommonElementsExecuteConditions>+
    <CommonElementsParameters>+
    <WebForms>+
</WebFormXML>
```
A WebFormXML gyökér elembe ágyazva kell szerepeltetni a WebForm számára szóló 
paraméterezést.

Attribútum|Leírás
:----|:------
NameSeparator|A változókra ezen elválasztók közé zárt névvel kell hivatkozni.

Elem|Leírás
:----|:------
DefaultLCID|Az XML-ben alkalmazott alapértelmezett nyelvkód. Ha nincs, az XML hiba.
[CommonElementsDefaults](####CommonElementsDefaults-elem)|Közös alapértelmezések gyűjteménye.
CommonElementsInputs|Közös inputok gyűjteménye.
CommonElementsActions|Közös akciók gyűjteménye.
CommonElementsExecuteConditions|Közös végrehjatási feltételek gyűjteménye.
WebForms|A megjeleníthető formok gyűjteménye.

#### CommonElementsDefaults elem
```xml
<CommonElementsDefaults>
    <Defaults LCID="nyelvkód">
        <FormDesign Layout="cshtml" LayoutMobile="cshtml" Style="html stílus"/>
        <Parameters NameSeparator="@@"/>
        <Inputs Type="Text" Required="false" ReadOnly="false" UploadFolder="mappa"/>
        <SQLList ID="Id" Display="Name" ConnectionString="kapcsolat név"/>
        <FormTexts ReturnMessageOKHeader="szöveg" ReturnMessageNOKHeader="szöveg" RequiredMissing="szöveg"/>
    </Defaults>
</CommonElementsDefaults>
```
Különböző nyelvi változatokhoz alkalmas alapértelmezések paraméterei találhatók itt.

Elem|Leírás
:----|:------
Defaults|Alapértelmezések gyűjteménye, mely minden vagy a jelölt nyelvre vonatkozik.
FormDesign|A form megjelenítésére vonatkozó alapértelmezések.
Parameters|A meghívandó akció paraméterezésére vonatkozó alapértelmezések.
Inputs|A beviteli mezőkre vonatkozó alapértelmezések.
SQLList|A "SQLList" típusú beviteli mezőkre vonatkozó alapértelmezések.
FormTexts|A form használata közben megjelenő szövegekre vonatkozó alapértelmezések.

Attribútum|Leírás
:----|:------
LCID|Érvényes nyelvkód. Ha nincs megadva, akkor az az alapértelmezés minden nyelvre vonatkozik.
Layout|A nem mobile módú megjelenéskor alkalmazandó layout fájl.
LayoutMobile|A mobile módú megjelenéskor alkalmazandó layout fájl.
Style|A form megjelensére vonatkozó szabványos html stílus. Például:"width:100%;"
NameSeparator|Az akció paramétereinek előállításakor az inputokra való hivatkozás ezzel van bekeretezve.
Type|Ha nincs megadva a beviteli mezőnél a típus, akkor ez lesz a típus.
Required|A beviteli mezők kitöltése alapértelmezetten kötelező legyen-e. Értéke "true" vagy "false" lehet.
ReadOnly|A beviteli mező módosítható-e alapértelmezés szerint. Értéke "true" vagy "false" lehet.
UploadFolder|A fájl feltöltés típusú mezők alapértelmezés szerint ebbe a mappába töltik fel a választott fájlokat.
ID|SQL lekérdezésben az azonosító szerepét betöltő mezőnév alapértelmezése.
Display|SQL lekérdezésben a leíró mezőnév alapértelmezése.
ConnectionString|SQL lekérdezéshez használt kapcsolat alapértelmezett neve vagy értéke.
ReturnMessageOKHeader|"ReturnInfoJSON" viszzatérési mód használatakor siker esetén megjelenítendő szöveg alapértelmezése.
ReturnMessageNOKHeader|"ReturnInfoJSON" viszzatérési mód használatakor sikertelenség esetén megjelenítendő szöveg alapértelmezése.
RequiredMissing|Kötelező mező kitöltetlenségekor megjelenő üzenet alapértelmezése.

## DateTimePicker
A DateTimePicker-hez alkalmazott megoldáshoz nincs NuGeT csomag. A GitHub-on találtam működő megoldást:

https://github.com/pingcheng/bootstrap4-datetimepicker/tree/master/src

A megoldás a "Bootstrap.v3.Datetimepicker.CSS" NuGeT csomagra épülne, de
NuGeT-ként csak a 4.17.45-ös verzió érhető el, ám a fentebb linkelt
megoldás nem erre, hanem annak GitHub-on található nagyobb (4.17.47-es)
verziószámú állapotára készült. Vagyis jobb híján nincs telepítve NuGeT
csomag, csak a WebForm area Content és Scripts mappáiba lettek betöltve
a szükséges fájlok.

Remélhetőleg a közeljövőben lesz bootstrap 4-eshez való NuGeT csomagban
elérhető datetimepicker megoldás, ami meg is felel a VRH igényeinek.

****




## Version History:

#### 2.3.0 (2019.05.14) Compatible changes - debug:
- Átállás a Vrh.Web.Common.Lib 2.0.0 változatára.
- Vrh.XmlProcessing 1.0.0 hozzáadása.
- Vrh.Common 2.3.0 hozzáadása.
- Frissítés a VRH.Log4Pro.MultiLanguageManager 3.4.0 változatára.
- Frissítés a WebActivatorEx 2.2.0 változatára.
- Frissítés a WebGrease 1.6.0 változatára.
- Frissítés a Font.Awesome 5.8.2 változatára.
- Frissítés a jQuery 3.4.1 változatára.
- Frissítés a Moment.js 2.24.0 változatára.


#### 2.2.1 (2018.12.04) Patches - debug:
- IgnoredChars tartalmazhat kapcsos zárójelek között billentyűkódot is.

#### 2.2.0 (2018.12.03) Compatible API changes - debug:
- InputFilter bevezetése.

#### 2.1.0 (2018.08.04) Compatible API changes - debug:
- XmlParser connection string bevezetése.
- XmlParser átvette a szerepét a 'strings', 'connectionStrings' és 'ExternalParameters' résznek.

#### 2.0.0 (2018.05.23) Incompatible API changes:
- Bootstrap 4 és jQuery 3.3.1 alapokkal működő változat.

#### 1.2.4 (2018.03.21) Patches:
- ConnectionString-ek feldolgozásán kellett javítani, pontosítani.

#### 1.2.3 (2018.01.05) Patches:
- Az akció új tabon való elindításán kellett módosítani.

#### 1.2.2 (2017.12.19) Patches:
- A paraméter kiértékeléshez is eljut az Execute akcióhoz megadott nem vezérlő paraméterek.
- A változók behelyettesítése itt: Parameter.Value, Input.Uploadfolder, Condition. és ExecuteConditions.Test és With.

#### 1.2.1 (2017.12.18) Patches:
- Csak a formon belül meghatározott akciók is megjelennek.
- Nem csak az URL-ben érkező, hanem az alapértelmezett értékkel bíró ExternalParameter is bekerül a változók közé.

#### 1.2.0 (2017.12.15) Compatibility API changes:
- Mostantól lehet több akciót is definiálni a formokhoz külön végrehajtási feltétel és paraméter listával.
- Az XML-ben lehet strings, connectionstrings, és ExternalParameter elem.
- Az URL jellegű adatok az általános XML struktúrák alapján készülnek, és lehetnek paraméterei is.
- A dokumentáció bővült és pontosodott. 

#### 1.1.2 (2017.11.08) Patches:
- Paraméter feltételkezelés javítása.
- DATList hastnálatakor az akció indítás előtti "oject error" hiba havítása. Időzítésen múlt.

#### 1.1.1 (2017.11.08) Patches:
- Multilista esetén a paraméter átadás javítása.

#### 1.1.0 (2017.11.08) Compatibility API changes:
- Multilista inputok esetében van elem az összes kijelölése, és az összes jelölés törlésére.

#### 1.0.1 (2017.11.06) Patches:
- Hiányzó NuGet csomagok pótlása.
- Teszt programfelület véglegesítése.

#### 1.0.0 (2017.11.06) Initial version




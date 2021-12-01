# Vrh.Web.Menu - Log4Pro alap menü és menülap kezeléssel
> Fejlesztve és tesztelve **4.5** .NET framework alatt. Más framework támogatása további
>  tesztelés függvénye, de nem kizárt alacsonyabb verziókban való működőképessége.

A csomag tartalmazza a Log4Pro alkalmazások layoutját, és a hozzátartozó menü és menülap
kezelést. Gyakorlatilag egy standard MVC alkalmazásra telepítve létrejön a Log4Pro alap.
A **Menu** area alatt találhatóak a layout, a menü és a menülap kezeléssel kapcsolatos 
összetevők.
A [Log4Pro alapok](####Log4Pro-alapok) részletesen is leírja, hogy mit érdemes tudni 
a komponens telepítéséről és annak hatásairól.

## Log4Pro alapok
A csomag telepítése elvégez minden beállítást ahhoz, hogy egy Log4Pro-s kinézetű menükezelő
létrejöjjön és egyből működőképes legyen. Leginkább egy kezdeti alap MVC-s alkalmazásra 
érdemes telepíteni, mert olyan fájlt is feltelepít, amely aztán meghatározza a további 
működést. A csomag a következő fájlokat telepíti a normál area és dll-en kívül:

Fájl|Leírás
:----|:------
App_Data\Menu\MenuSample.xml|Egy minta a menü paraméterezéséhez.
App_Data\Menu\MenuPageSample.xml|Egy minta a menülap paraméterezéséhez.
Content/Log4Pro_Base.css|Egy Log4Pro alkalmazás kinézetét meghatározó stílusbeállítások, amelyek eltérnek a bootstrap-től.
Content/Log4Pro_Base.min.css|Az előbbi tömörített állapotban. 
Content/vrh.commontools.css|VRH web alkalmazások közös javascript alapú megoldásainak stílus beállításai.
Content/vrh.commontools.min.css|Az előbbi tömörített állapotban.
Scripts/vrh.commontools.js|VRH web alkalmazások közös javascriptjei.
Scripts/vrh.commontools.min.js|Az előbbi tömörített állapotban.
Views/_ViewStart.cshtml|Az alap _ViewStart.cshtml felülírása, mely a Menu area-ban lévő Layout-ra mutat.

### Hitelesítés használata
Ha azt szeretnénk elérni, hogy a menükezelő a hitelesítést használja, illetve működés közben
a hitelesítési beállításokat ne hagyja figyelmen kívül, akkor telepíteni kell a Vrh.Web.Membership
nuget csomagot és az alkalmazás start kódjában (Global.asax) el kell helyezni  következő 
utasítást:
```javascript
Vrh.Web.Menu.Global.IsUseAuthentication = true;
```
Azért kell így beállítani, hogy külső paraméterezéseel ne lehessen felülírni a hitelesítés
követelményét.

### Megrendelő embléma
A megrendelő emblémáját is a Global.asax-ban érdmemes beállítani. Ha nem az alapértelmezett
"~/Images/userlogo.jpg" a megrendelő emblémája, akkor az előbb említett helyen ezt is át
lehet állítani:
```javascript
Vrh.Web.Menu.Global.CustomerLogo = "~/Images/customerlogo.png";
```

### Indulás előtt
Az első indítás előtt érdemes belenézni a Web.config-ba, ahol ellenőrizni kell a 
kapcsolati stringek beállítását, és hogy rendben bekerültek a Vrh.Web.Memebership
beállításai. Mindenképp adjunk meg érvényes adatbázis elérést, hogy az első 
induláskor létrejöjjenek a keretrendszer által kezelt táblák.

## Menu XML felépítés
Az XML részletekben a "<>" jelek közötti részek tartalmazzák az elemneveket és attribútumokat.
Ahol "+" jel van, azok egy zárt elemet jelentenek, egy későbbi címben lesznek kifejtve. 
Így tárva fel a teljes szerkezetet.

****
# Version History:

#### 1.14.0 (2019.05.14) Compatible changes - debug
- Átállás a Vrh.Web.Common.Lib 2.0.0 változatára.
- Vrh.XmlProcessing 1.0.0 hozzáadása.
- Vrh.Common 2.3.0 hozzáadása.
- Frissítés a VRH.Log4Pro.MultiLanguageManager 3.4.0 változatára.
- Frissítés a Vrh.Web.Membership 3.8.0 változatára.
- Frissítés a WebActivatorEx 2.2.0 változatára.
- Frissítés a WebGrease 1.6.0 változatára.
- Frissítés a Font.Awesome 5.8.2 változatára.
- Frissítés a jQuery 3.4.1 változatára.

#### 1.13.1 (2019.03.11) Patches - debug
- Frissítés a Vrh.Web.Membership 3.6.0 változatára.

#### 1.13.0 (2019.03.11) Compatible changes - debug
- VrhIdNameHandler prototípus létrehozása a vrh.commontools.js scriptben. Azonosító 
és név jellegű táblák karbantartásának egyfajta általános megoldása.
- Egyéb apróbb módosítások a vrh.commontools.js összetevőben.
- Frissítés a Vrh.Web.Common.Lib 1.18.0 változatára.
- Frissítés a Vrh.Web.Membership 3.5.0 változatára.

#### 1.12.2 (2019.02.14) Patches - debug
- Közvetlen autentikáció metódusa egy külön fájlba került, amit külön kell
betölteni menü fajtánként.

#### 1.12.1 (2019.02.13) Patches - debug
- Közvetlen autentikáció meghívásának módján kellett javítani.

#### 1.12.0 (2019.01.30) Compatible changes - debug
- Frissítés a Vrh.Web.Common.Lib 1.17.0 változatára.
- Frissítés a VRH.Log4Pro.MultiLanguageManager 3.1.0 változatára.
- Frissítés a Vrh.Web.Membership 3.4.0 változatára.
- Közvetlen autentikáció megvalósítása, ha kérdő jel szerepel a szereplista elején.

#### 1.11.3 (2018.11.25) Patches - debug
- Vrh.DataTables.js módosult. Felirat helyett spinner látszódik frissítéskor, és az alapértelmezett
valamint a megadott opciók összefésülésében volt hiba.

#### 1.11.2 (2018.11.15) Patches - debug
- MenuItem-nél Dialog esetén az AutoClose korlát megszűnt. 0-nál nagyobb érték esetén bekapcsol.

#### 1.11.1 (2018.11.05) Patches - debug
- Log4ProBase.css módosult: A modális ablakok bosszantó "body.padding-right:17px" beállításának
hatását küszöböli ki.
- A bootbox.edit autofocus-sal megjelölt inputra teszi a fókuszt. Ezen a szolgáltatásán kellett javítani.
- VrhDataTable prototípus opciói bővültek és pontosodtak.
 
#### 1.11.0 (2018.10.10) Compatible changes - debug
- Frissítés a Vrh.Web.Membership 3.2.0 változatára.
- Frissítés a Vrh.Web.Common.Lib 1.15.0 változatára.
- Frissítés a Font.Awesome 5.4.1 változatára.
- Microsoft.AspNet.Mvc 5.2.6 függőség beállítása. Frissítés korábban megtörtént.
- Microsoft.jQuery.Unobtrusive.Ajax 3.2.6 függőség beállítása. Frissítés korábban megtörtént.
- Microsoft.jQuery.Unobtrusive.Validation 3.2.11 függőség beállítása. Frissítés korábban megtörtént.
- Microsoft.AspNet.TelemetryCorrelation 1.0.4 függőség beállítása. Frissítés korábban megtörtént.
- Microsoft.CodeDom.Providers.DotNetCompilerPlatform 2.0.1 függőség beállítása. Frissítés korábban megtörtént.
- A Welcome lapok hozzáigazítása az új login/logout megoldáshoz.
- Log4ProBase.css módosult. A Font Awesome 5.4.1 változatában már kijavították a függőleges pozíciót, 
nem kell az egyedi beállítás. 

#### 1.10.0 (2018.09.25) Compatible changes - debug
- FlowOver attribútum bevezetése. Ha igaz, akkor a bekúszó menü eltakarja a tartalmat.
- DeskTop módban a lapok aljának eléréséhez a menuHide és menuVisible állítja a padding-bottom értékét.
- DeskTop módban a rejtett menünél a "sárga" menüjező csík mindig a képen marad.
- A MenuPage OpenRight nézetmódban is figyelembe veszi az 'Appearance' elem értékeit.
- Az indító URL-ben az első tag ezentúl nem a nézetmód (ViewMode), hanem a belépési pont (EntryPoint).

#### 1.9.1 (2018.09.21) Compatible changes - debug
- Stílusokra való hivatkozás xml szerkezetén kellett módosítani.
- VisibleDisabledItem kapcsoló alapértelmezése igazra változott.

#### 1.9.0 (2018.09.16) Compatible changes - debug
- Hogy a lapok alja is elérhető legyen Desktop módban a Desktop.css
```.container-fluid.body-content``` szekciója kiegészült egy 
```margin-top: 3rem``` bejegyzéssel.
- Frissítés a Vrh.Web.Membership 3.1.3 változatára.
- A menü számára nem kötelező a "főmenü" (MainManuGroup) beállítása. Ha
nincs beállítva, akkor egyszerűen nincs menü. MenuPage szabályai változatlanok. 
- Megváltozott a StyleKit szerkezete és külső fájlból töltődik fel.
- 'RolePrefix' elem bevezetése, és a nem létező szerepek létrehozása, ha 'CreateRole' igaz.

#### 1.8.0 (2018.08.30) Compatible changes - debug
- Jelentősen módosult/bővült a vrh.commontools.js:
  -  A CRUD műveletek támogatásához a vrhct.bootbox.edit, vrhct.bootbox.delete metódusok.
  -  DataTables támogatásához a VrhDataTables prototípus.
- Módosult a Log4Pro_Base.css is.
- A Layout-ban a bootbox.DefaultOption a MultiLanguageManager.General szókód névtérből 
veszi a fordításokat.
- Frissítés a VRH.Log4Pro.MultiLanguageManager 3.0.0 változatára.
- Frissítés a Vrh.Web.Common.Lib 1.12.0 változatára.
- Frissítés a Font.Awesome 5.3.1 változatára.
- Frissítés a bootstrap 4.1.3 változatára.


#### 1.7.0 (2018.08.29) Compatible changes - debug
- Frissítés a Font.Awesome 5.2.0-ás változatára.
- Frissítés a Vrh.Web.Membership 3.1.2-es változatára.
- MenuPage.DisplayPartial akció létrehozása, amely betölti az alap eszközöket, de a Layout-ot nem.
- MenuPage-ben az 'Appearance' elem bevezetése az egyedi stílusok megadásához.
- OtherParameters most már átadásra kerül az XmlParser részére.

#### 1.6.0 (2018.08.13) Compatible changes - debug
- ```<Reference>``` elem kiegészült a ```<Welcome>``` nevű elemmel, mely egy szabványos URL.
- Amikor szabványosan indítják az alkalmazást, és ```<Welcome>``` url is van, akkor átkapcsol 
oda az alkalmazás.
- A ```<Welcome>``` url lesz az adott referencia esetén a "logo" és "login/logout" link mögött.

#### 1.5.0 (2018.08.13) Compatible changes - debug
- ```MenuPage.Display``` akció ```xml``` nevű paramétert fogad ```cfgname``` helyett,
ami egy XmlParser kapcsolati sztring.
- A menü xml paraméterfájlban a ```<MenuItem>``` elem ```MenuPage``` attribútuma mostantól
XmlParser kapcsolati sztringet vár.
- ```MenuPage.Display``` akció feldolgozza a beépített paramétereken túli paramétereket, 
amit el is juttat az XmlParser-ba.

#### 1.4.0 (2018.08.10) Compatible changes - debug
- Menü indításakor megadható a 'config' és 'ref' URL paraméter.

#### 1.3.0 (2018.08.06) Compatible changes - debug
- Menu/Layout/Login és Menu/Layout/Logout elkészítése.

#### 1.2.3 (2018.08.01) Patches - debug
- Stílusok beállításán kellett javítani.
- A szerepek vizsgálata pontosításra került, és a MenuPage General elemben lévő Roles-t is
figyelembe veszi.
- Az InitialState attribútum hatása már minden esetben működik.
- Frissítés a Vrh.Web.Common.Lib 1.9.0-ás változatára.

#### 1.2.2 (2018.07.26) Patches - debug
- IFrame="true" attribútumos menüpontnál konvertálja (encode) az eredeti URL értékét.

#### 1.2.1 (2018.07.15) Patches - debug
- MenuXmlParser debug részletesebb és a hibaüzenet megjeleníti az aktuális fájlt is.

#### 1.2.0 (2018.07.18) Compatible changes - debug
- Már csak egy '/' jel lesz az url elején, ha '~' jellel kezdődött a LinkOrCommand előállításánál.
- XmlParser kapcsolati sztring alapú használata.
- Egyéb apróbb javítások.

#### 1.1.1 (2018.06.29) Patches - debug
- Apró kiegészítés a teszt céljára készült LayoutController ReturnInfoTest akcióban.

#### 1.1.0 (2018.06.20) Compatible changes - debug
- Kiegészült a 'MenuItem' egy 'IFrame' attribútummal, mely igaz értéke esetén a kért URL-t
egy méretezett 'iframe' elemben jeleníti meg.

#### 1.0.3 (2018.06.04) Patches - debug
- Hiba történt, ha nem kértük az authentikációt, és nem volt jelen a Vrh.Web.Membership NuGeT csomag.

#### 1.0.2 (2018.05.23) Patches - debug
- URL összeállításban volt némi javítani való. A "~" kezelése nem volt teljes.
- A ViewBag.Title a "Wrh.Web.Menu" helyett a Menu.Xml-ben megadott értéket jeleníti meg.
- Log4Pro_Base.css módosult a Font Awesome és az input-group-addon kapcsán.

#### 1.0.1 (2018.05.15) Patches - debug
- A képekre való hivatkozást kellett javítani.

#### 1.0.0 (2018.05.14) Initial version - debug
- Layout megoldás menü és menülap kezeléssel.



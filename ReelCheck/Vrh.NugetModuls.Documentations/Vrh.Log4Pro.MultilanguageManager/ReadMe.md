# Vrh.Log4Pro.MultiLanguageManager
Többnyelvű .NET alkalmazások fejlesztésének támogatására és 
kiszolgálására szolgáló megoldások gyűjteménye.

> A komponens **3.0.0** változatának kiadásakor kezdődött e 
> leírás elkészítése, ám jelenleg nem naprakész.
> Fejlesztve és tesztelve **4.5** .NET framework alatt. 
> Teljes funkcionalitás és hatékonyság kihasználásához szükséges 
> legalacsonyabb framework verzió: **4.5**

> **#TODO: Véglegesítendő a komponens dokumentációja!!!**

# Főbb összetevők
### Interfészek
* **[ITranslation interfész](##ITranslation-interfesz)**

### Hasznos osztályok
* **[MultiLanguageManager osztály](##MultiLanguageManager-osztaly)**
* **[GeneralWordCodes osztály](##GeneralWordCodes-osztaly)**
* **[TranslationBase osztály](##TranslationBase-osztaly)**

## ITranslation interfész
```javascript
/// <summary>
/// Meghatározza és előírja egy fordítási szolgáltatásokat 
/// biztosító osztály elvárt tulajdonságait és módszereit.
/// </summary>
public interface ITranslation
{
    /// <summary>
    /// Az aktuálisan érvényes nyelvi kódot tartalmazó tulajdonság.
    /// </summary>
    string LCID { get; set; }

    /// <summary>
    /// A nyelvi fordítást elvégző metódus.
    /// A szókód típusának megadásával és ha van egyéb paraméter, 
    /// azok behelyettesítésével.
    /// </summary>
    /// <param name="wordCodeType">Egy típus, amely a szókódot jelképezi.</param>
    /// <param name="pars">Objektumok listája, melyek behelyettesítésre kerülnek a <c>String.Format()</c> szabályai szerint.</param>
    /// <returns>A szókódnak megfelelő szöveg a behelyettesítésekkel.</returns>
    string TransFormat(Type wordCodeType, params object[] pars);

    /// <summary>
    /// A nyelvi fordítást elvégző metódus.
    /// A szövegs szókód megadásával és ha van egyéb paraméter, 
    /// azok behelyettesítésével.
    /// </summary>
    /// <param name="wordCodeString">Egy szöveges szókód.</param>
    /// <param name="pars">Objektumok listája, melyek behelyettesítésre kerülnek a <c>String.Format()</c> szabályai szerint.</param>
    /// <returns>A szókódnak megfelelő szöveg a behelyettesítésekkel.</returns>
    string TransFormat(string wordCodeString, params object[] pars);

    /// <summary>
    /// Nyelvi fordítás elvégzése szókód típusa alapján.
    /// </summary>
    /// <param name="wordCodeType"></param>
    /// <returns>A szókódnak megfelelő szöveg.</returns>
    string Trans(Type wordCodeType);

    /// <summary>
    /// Nyelvi fordítás elvégzése szöveges szókód alapján.
    /// </summary>
    /// <param name="wordCodeString">Szöveges szókód.</param>
    /// <param name="defaultTrans">Ha nincs érvényes fordítás, akkor a behelyettesítendő szöveg.</param>
    /// <returns>
    /// A <paramref name="wordCodeString"/> szókódnak megfelelő szöveg, 
    /// vagy a <paramref name="defaultTrans"/> szerinti alapértelmezés.
    /// </returns>
    string Trans(string wordCodeString, string defaultTrans = "");
}
```

***
## GeneralWordCodes osztály
A komponens a **MLM.General** fő csoportot használja a saját 
és a VRH alkalmazásokban általánosan használt szókódok definiálására, mely ebben 
az osztályban található. Az inicializálsákor ezt az osztályt kell megadni. A nyelvi
fordítások felület a **MLM** főcsoportot használja kerülve a "General"
alcsoport elnevezést.

Itt érdemes definiálni minden olyan szókódot, amely:
* komponenstől függetlenül bármely helyzetben hasznáható
* az alapvető megoldásokban használt fordítások szókódjai (pl. DataTables, MasterData)


***
## TranslationBase osztály
Nyelvi fordítást segítő absztrakt osztály, melyet egyéb osztályok építésekor felhasználhatunk.
A [MultiLanguageManager osztály](##MultiLanguageManager-osztaly) metódusaira épít, azok meghívását
egyszerűsíti. Az osztály az ITranslation interfészt valósítja meg.

Felhasználási minta:
```javascript
public class MyClass : TranslationBase { ... }
```

Tulajdonságok|Leírás
:----|:----
LCID|Az aktuális környezetben érvényes nyelvi kód. Alapértelmezett értéke ```MultiLanguageManager.RelevantLanguageCode```

Metódusok|Leírás
:----|:----
```string Trans(Type wordCodeType)```|```MultiLanguageManager.GetTranslation()``` meghívása a fordítás megkönnyítése érdekében a már beállított LCID értékkel.
```string Trans(string wordCodeString, string defaultTrans = "")```|```MultiLanguageManager.GetTranslation()``` meghívása a fordítás megkönnyítése érdekében a már beállított LCID értékkel.
```string TransFormat(Type wordCodeType, params object[] pars)```|```MultiLanguageManager.GetTranslation()``` meghívása a ```pars``` behelyettesítésével. A ```pars``` objektumok listája, melyek behelyettesítésre kerülnek a ```String.Format()``` szabályai szerint.
```string TransFormat(string wordCodeString, params object[] pars)```|```MultiLanguageManager.GetTranslation()``` meghívása a ```pars``` behelyettesítésével. A ```pars``` objektumok listája, melyek behelyettesítésre kerülnek a ```String.Format()``` szabályai szerint.


***
## MultiLanguageManager osztály
Ebben a statikus osztályban összpontosulnak a többnyelvűséggel kapcsolatos 
tulajdonságok és metódusok.

### Tulajdonságok
Tulajdonságok|Leírás
:----|:----
```List<String> ActiveLanguageCodes```|Visszadja az adatbázisban definiált összes aktív nyelv kódját. Csak olvasható.
```List<TranslationLanguage> ActiveLanguages```|Visszaadja az összes aktív nyelvet. Csak olvasható.
```List<String> AllLanguageCodes```|Visszadja az adatbázisban definiált összes nyelv kódját. Csak olvasható.
```List<TranslationLanguage> AllLanguages```|Visszaadja az adatbázisban definiált összes nyelvet. Csak olvasható.
```List<String> AllWordCodes```|Visszaadja az adatbázisban letárolt öszes szókódot. Csak olvasható.
```string CurrentConnectionString```|Az osztályban érvényes/aktuális kapcsolati sztring. Átállítható, de akkor újra beállításra kerül a <c>DefaultLanguageCode</c>, és a TranslationCache is újra töltődik.
```string DefaultLanguageCode```|Alapértelmezett nyelv kódja. Alapértelmezett értéke a Web.config-ban megadott érték, vagy ha ott nincs semmi, akkor "hu-HU" a magyar nyelv értéke. A ```SetDefaultLanguage()``` metódussal állítható az értéke. Csak olvasható.
```string RelevantLanguageCode```|Visszaadja az aktuális nyelv kódját, amelyet a nyelvi cookie-ból olvas ki. Ha nincs cookie, vagy nem is vagyunk WEB-es környezetben, akkor a ```DefaultLanguageCode```-dal egyenértékű. Csak olvasható.

### Metódusok
Metódusok|Leírás
:----|:----

***
## Adatbázis
A komponens jelenleg 3 táblát használ a fordítások tárolására az 
**i18n** sémában. A táblákra az SQL scriptekben így kell hivatkozni:
i18n.Languages, i18n.Words, i18n.Translations.
A csomag a szükséges migrációkat elvégzi az adatbázisban. 
(Például az első használatkor létrehozza a táblákat.)
### Languages tábla
Oszlop|Leírás
:----|:----
```int Id```|A nyelv belső egyedi azonosítója.
```string LCID```|Nyelv szabványos kódja "Windows Language Code Identifier" (pl. hu-HU).
```string Name```|Nyelv megnevezése.
```bool Active```|Aktív-e (használható-e) a nyelv az adott környezetben.

### Words tábla
Oszlop|Leírás
:----|:----
```int Id```|A szókód belső egyedi azonosítója.
```string WordCode```|A szókód.
```string Description```|A szókód magyarázata.
```string Modul```|A szókódok csoportosítására szolgáló mező.

### Translations tábla
Oszlop|Leírás
:----|:----
```int Id```|Fordítások belső egyedi azonosítója.
```int LanguageId```|A nyelv belső egyedi azonosítója, amelyhez a fordítás tartozik.
```int WordId```|A szókód belső egyedi azonosítója, amelyhet a fordítás tartozik.
```string Text```|Az adott nyelvhez és szókódhoz tartozó fordítás szövege.


***
## Version History:

#### 3.4.0 (2019.05.14) Compatible changes - Debug
- ITranslation interfész áthelyezése Vrh.Web.Common.Lib 1.18.1-es változatából.
- TranslationBase abstract osztály áthelyezése Vrh.Web.Common.Lib 1.18.1-es változatából.
- Frissítés a Newtonsoft.Json 12.0.2 változatára.
- Frissítés a VRH.ConnectionStringStore 2.2.1 változatára
- ReadMe.md-ből a "csharp" jelölések cseréje "javascript"-re.

#### 3.2.5 (2019.04.26) Patches - Debug
- ConnectionStringStore verzió upgrade.

#### 3.2.4 (2019.04.26) Patches - Debug
- ConnectionStringStore upgrade.

#### 3.2.3 (2019.04.18) Patches - Debug
- Hibajavítás.

#### 3.2.2 (2019.04.04) Patches - Debug
- Hibajavítás.

#### 3.2.1 (2019.04.03) Patches - Debug
- Microsoft.AspNet.Mvc 5.2.7 hozzáadása a korábbi direkt dll hivatkozás helyett.
- Egy felesleges copy kikommentezése a postbuild scriptből
 
#### 3.2.0 (2019.04.02) Compatible changes - Debug
- VRH.MultiLanguageManager.connectionString néven kéri el az SQL kapcsolat leírót a VrhConnectionStringStore-tól, ha van ilyen bejegyzés az appconfig / AppSettings, vagy az appconfig / connectionStrinfs blokkok valamelyikében, egyébként pedig az eddigi módon a DBConnection név használatával.

#### 3.1.0 (2018.12.01) Compatible changes - Debug
- i18n.Tranlations.Text és i18n.Words.Description mezők átállítása nvarchar(max)-ra.

#### 3.0.1 (2018.10.30) Patches
- MLM.General.Messages és MLM.General.Words szókód terek bővítése és pontosítása.

#### 3.0.0 (2018.08.22) Incompatible API changes - debug
- i18n séma bevezetése (i18n.Languages, i18n.Words, i18n.Translations)
- Módosult táblaszerkezetek, de migrálja az előző változatot.
- Lista helyett szótár lett a gyorsítótár. TranslationDirectory néven.
- EntityFramework.Extended [Deprecated] lett, így ki lett vezetve.
- GeneralWordCodes osztály bevezetése.
- Adatbázis migráció ellenőrzés a ConnectionString tulajdonság változásakor történik meg.
Az első ilyen a statikus MultiLanguageManager osztály konstruktorában történik.
- Az inicializáláshoz már nem, de a visszafejtéshez kell még a tartalmazó osztály 
elnevezésének a végére "WordCodes" szó.

#### 2.1.5 (2018.04.25) Patches
1. Update Newtonsoft.Json to Newtonsoft.Json 11.0.2
2. Translation cache nullvédelme inicializálatlan DB esetén
3. Framework references betéve a nuspec-be
4. Icon URL a nuspechez adva

#### 2.1.4 (2017.11.07) Patches
1. Update EF to: 6.2.0
2. Update EntityFramework.Extended to 6.1.0.168
3. Update Newtonsoft.Json to Newtonsoft.Json 10.0.3
4. Nuget csomaggá alalkítás

#### 2.1.3 (2016.10.27) Patches
1. Connection StringStore componens beépítése: Saját settings kulcs és CS name: 
MultilanguageManager, default settibngs kulcs:  connectionString, default CS name: DbConnection

#### 2.1.2 (2016.09.06) Patches
1. AddOrModifyTranslation, ha a canOverwriteTranslation paramétere false, akkor mielőtt az 
adatbázishoz nyúlna, előbb leellenőrzi a cache-ben, hogy létezik-e az adott fordítás. 
Ha ott megvan akkor kilép. Így a kezdeti Wordcode initeken sokkal gyorsabban jut túl, 
a már inicializált fordítások átugrásával, mintha mindig a db ellenőrzésénél ugrana ki 
az exception ágon.

#### 2.1.1 (2016.05.20) Patches
1. Lockolások finomítása (Multithreading környezetben való konzisztensebb működésért)
2. Fel nem oldható szókódra  a szókódot adja a GetTranslation vissza 

#### 2.1.0 (2016.03.23) Copatible API Changes:
##### 1. **System.ComponentModel.DataAnnotations** Override-ok betétele a komponensbe #5965
> Így nem kell a használt projektben újraírni, vagy odamásolni őket forrás szinten, használhatóak 
> innen.
Ez a 4 darab van:
* **DisplayNameWithTrueWordCodesAttribute**
* **RequiredWithTrueWordCodesAttribute**
* **StringLengthWithTrueWordCodesAttribute**
* **RangeWithTrueWordCodesAttribute**
> **Használati minta:**
```javascript
[DisplayNameWithTrueWordCodes(typeof(TrueWordCodes.MasterData.AndonRequestCode.Columns.Code))]
[RequiredWithTrueWordCodes(typeof(TrueWordCodes.MasterData.DataAnnotations.RequiredWithName))]
[StringLengthWithTrueWordCodes(2, typeof(TrueWordCodes.MasterData.DataAnnotations.StringLengthWithNameAndBetween), MinimumLength = 2)]
[RangeWithTrueWordCodes(1, 5, typeof(TrueWordCodes.MasterData.DataAnnotations.Range))]
```
##### 2. **GetLanguagesInSelectItemList** bővítés #5964
> Kapott egy nem köztelező paramétert (default=true) (így nem okoz kompatibilitási problémát, eddig mindig csak az aktívakat adta). Amely jelzi, hogy csak az aktív nyelvek, vagy mindegyik szerepeljen-e a visszaadott SelectList-ben.
**Használati minta:**
>> Minden nyelvet visszaad:
```javascript
GetLanguagesInSelectItemList(false)
```
>> Csak az aktívakat:
```javascript
GetLanguagesInSelectItemList()
GetLanguagesInSelectItemList(true)
```
##### 3. Elírások javítása #5961
> Sajnos volt pár elírás ami átcsúszott  a kettes  verzióba is. Ezek ebben a verzióban Obsolete-nek lettek jelölve és létre van hozva a helyes írású verziójuk:
>* ~~**GetCashe**~~ helyesen **GetCache**, ezt kell használni helyette
* ~~**GetWordCodesAtGoup**~~  helyesen **GetWordCodesAtGroup**, ezt kell használni helyette
##### Patches
1. CacheToList függvény NullReferencExceptiont dobott, ha nem volt még létrehozva Cache #5881
2. Ha a default nyelv üres adatbázisnál jött létre a default nyelv property beállításával, akkor inaktív volt a nyelvkód, "kézzel" kellett korrigálni. Most aktívnak jön létre ekkor is. #5882
3. WordCode és Translation létrehozás hibák bizonyos esetekben ha egy WordCode GetTranslation híváson át jött létre. #5962
4. Ha a lekért nyelven nincs fordítás akkor a Defult nyelv fordítása helyett inkább a WordCode sttringet adja vissza. Így a fordítási hiányosságok könnyebben észrevehetőek. #5963

#### 2.0.0 (2016.02.29) Incompatibility API changes:
1. 2-es verzió! Sok minden újragondolva átalakítva az eddigi használati tapasztalatok alapján. Az V1.X.X verziók felé inkompatibilis! (ahogy a verziózásból is látszik.)

#### 1.0.4 (2015.01.08) Patches:
1. Add AddNewFirstTagToWordCode public function. Add EntityFramework.Extensions nuget package

#### 1.0.3 (2015.01.08) Patches:
1. Ne ellenőrizze minden alaklommal az init a DB oldalon, hogy megvan-e a az adott nyelvet definiáló rekord.

#### 1.0.2 (2014.11.18) Patches:
1. Update EF 6.1.1 (System.Data --> System.Data.Entity.Core Namespace változás miati módosításaok vannak csak benne, funkcionálisan nem változott semmi az 1.0.1-hez képest)

#### 1.0.1 (2014.11.10) Patches:
1. Meglévő fordítást nem kötelezően ír felül (EF V5)

#### 1.0.0 (2014.09.03) Initial version

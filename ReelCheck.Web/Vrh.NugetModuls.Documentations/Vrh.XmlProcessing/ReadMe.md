# Vrh.XmlProcessing
VRH-s XML paraméter fájlok feldolgozását támogató komponens.
Hasznos függvényekkel, osztályokkal és rendszeresen előforduló szerkezetek leképezésével.

> Fejlesztve és tesztelve **4.5** .NET framework alatt. 
> Teljes funkcionalitás és hatékonyság kihasználásához szükséges legalacsonyabb framework verzió: **4.5**

## Főbb összetevők
  > * [XmlLinqBase osztály](##XmlLinqBase-osztaly) Xml feldogozásokhoz készülő osztályok absztrakt alaposztálya. 
  > * [VariableDictionary](##VariableDictionary) Behelyettesítendő változók összegyűjtése és a behelyettesítés végrehajtása. 
  > * [XmlCondition osztály](##XmlCondition-osztaly) A ```<Condition>``` elem feldogozását segítő osztály. 
  > * [XmlVariable osztály](##XmlVariable-osztaly) Az ```<XmlVar>``` vagy bármely Name, LCID attribútummal és értékkel rendelkező elem feldogozását segítő osztály. 
  > * [XmlConnection osztály](##XmlConnection-osztaly) XmlParser kapcsolati string feldogozása és kifejtése.
  > * [XmlParser osztály](##XmlParser-osztaly) Az ```<XmlParser>``` elem feldogozását elvégző osztály. 
 
## XmlLinqBase osztály
Egy absztrakt alaposztály, mely minden specialitás nélkül alkalmas egy XML állomány 
elemeinek és attribútumainak beolvasására és a típusos értékek előállítására. Az osztály
a System.Xml.Linq névtér beli xml kezeléshez nyújt segédeszközöket. Az osztályban minden 
tulajdonság és metódus protected hatáskörű.

Felhasználási minta:
```javascript
public class MyClass : XmlLinqBase { ... }
```

Tulajdonságok|Leírás
:----|:----
CurrentFileName|A Load metódus által legutóbb sikeresen betöltött xml fájl neve. Csak olvasható. Értéke akkor íródik felül, ha a Load minden tekintetben sikeres.
RootElement|GetXElement metódusban megadott útvonal ettől az elemtől számítódik. A Load metódus beállítja (felülírja) ezt az elemet.
EnableTrim|A GetValue metódusok számára engedélyezi a whitespace karakterek eltávolítását a megtalált érték elejéről és végéről. Alapértelmezett értéke: true.

Metódusok|Leírás
:----|:----
```void Load(string xmlFileName)```|A megadott fájlból betölti az XML struktúrát. Beállítja a CurrentFileName tulajdonságot.
```XElement GetXElement(params string[] elementPath)```|A RootElement-ben lévő elemtől a sorban megadott elemeken keresztül elérhető elemet adja vissza.
```XElement GetXElement(XElement root, params string[] elementPath)```|A root paraméterben megadott elemtől a sorban megadott elemeken keresztül elérhető elemet adja vissza.
```XElement GetXElement(string elementPath, char separator = '/')```|A RootElement-ben lévő elemtől a sorban megadott elemeken keresztül elérhető elemet adja vissza.
```T GetValue<T>(XElement xelement, T defaultValue, bool isThrowException = false, bool isRequired = false)```|Visszad egy element értéket a defaultValue típusának megfelelően.
```T GetValue<T>(string attributeName, XElement xelement, T defaultValue, bool isThrowException = false, bool isRequired = false)```|Visszadja az xelement elem alatti, megnevezett attribútum értékét.
```T GetValue<T>(System.Xml.Linq.XAttribute xattribute, T defaultValue, bool isThrowException = false, bool isRequired = false)```|Visszadja az attribútum értékét a kért típusban.
```T GetValue<T>(string stringValue, T defultValue)```|A megadott típusra konvertál egy stringet, ha a konverzió lehetséges.
```TEnum GetEnumValue<TEnum>(XAttribute xattribute, TEnum defaultValue, bool ignoreCase = true)```|Attribútum értéket enum típus értékké konvertálja.
```TEnum GetEnumValue<TEnum>(XElement xelement, TEnum defaultValue, bool ignoreCase = true)```|Elem értéket enum típus értékké konvertálja.
```string GetXmlPath(XAttribute xattribute)```|Xattribute útvonala '/Element0/Element1/Element2.Attribute' formában.
```string GetXmlPath(XElement xelement)```|XElement útvonala 'Element0/Element1/Element2' formában.
```void ThrowEx(string mess, params object[] args)```|System.ApplicationException dobása a megadott formázott üzenettel.

### Beépített kiterjeszthető osztályok

#### XmlLinqBase.ElementNames osztály
Általában használatos elemnevek kiterjeszthető osztálya.

Állandó|Értéke|Leírás
:----|:----|:----
VALUE|"Value"|XML tagokban lehetséges 'Value' elem eléréséhez hasznos állandó.

#### XmlLinqBase.Messages osztály
Általában használatos üzenetek kiterjeszthető osztálya.

Állandó|Értéke|Leírás
:----|:----|:----
ERR_FILENOTEXIST|"File does not exist! File = {0}"|Nem létező fájl üzenete 1 behelyttesítéssel.
ERR_XMLROOT|"The root element is missing or corrupt! File = {0}"|'Root' elem hiányzik vagy hibás XML üzenet 1 behelyettesítéssel.
ERR_MISSINGELEMENT|"The '{0}' element is missing !"|Az elem hiányzik üzenet 1 behelyettesítéssel.
ERR_MISSINGATTRIBUTE|"The '{0}' attribute is missing in the '{1}' element!"|Az attribútum hiányzik üzenet 2 behelyettesítéssel.
ERR_REQUIREDELEMENT|"Value of the '{0}' element is null or empty!"|Szükséges elem null vagy üres üzenet 1 behelyettesítéssel.
ERR_REQUIREDATTRIBUTE|"Value of the '{0}' attribute is null or empty in the '{1}' element!"|Szükséges attribútum null vagy üres üzenet 2 behelyettesítéssel.
ERR_PARSETOTYPE|"The '{0}' string is not {1} type! Place='{2}'"|Típus konvertálása sikertelen üzenet 3 behelyettesítéssel.


## VariableDictionary
Egy ```System.Collections.Generic.Dictionary<string,string>``` alapú osztály, 
mely kiterjesztésre került abból a célból, hogy alkalmas legyen Név és Érték kulcspárok 
tárolására, és azok behelyettesítésére egy megadott cél szövegben. 
Az osztály példányosításakor szükséges megadni egy érvényes nyelvi kódot (LCID).
A példányosításkor egyből létrejönnek a rendszer változók is a ...BACK változók
kivételével, azok ugyanis behelyettesítéskor értékelődnek ki. A [rendszerváltozók](####A-rendszervaltozok) nevei
a ```SystemVariableNames``` statikus osztályban érhetőek el. A behelyettesítéskor a változókat
az osztály NameSeparator tulajdonság közé illesztve keresi. A NameSeparator tulajdonság 
alapértelmezett értéke "@@", de átállítható, ha a környezetben más használatos. 

> A táblázatokban csak a ```System.Collections.Generic.Dictionary<string,string>```
osztályt kiterjesztő elemeket mutatjuk be.

Tulajdonságok|Leírás
:----|:----
CurentCultureInfo|Az LCID változó beállításakor kap értéket. Csak olvasható.
NameSeparator|A változó neveket e separátorok közé illesztve keresi a szövegben.

Ha a NameSeparator hossza 1, akkor a változót keretező kezdő és befejező karakter azonos, egyébként a 
2. karakter lesz a befejező jel. Alapértelmezett értéke "@@". Amennyiben szükség van egy alternatív
elválasztó párra, akkor párosával növelhető az elválasztó párok száma. Példa: "@@##". 
Ilyenkor elsőként a "@" jelek közé zárt neveket keres, ha ilyet nem talál, akkor kísérletet tesz a "#"
jelek közé zárt változó név keresésre. Ha a Name separator nem 1, 2 vagy páros hoszzúságú, akkor hiba 
keletkezik.

Metódusok|Leírás
:----|:----
```void Add(string name, string value)```|Egy darab név-érték pár hozzáadása a gyűjteményhez.
```void Add((NameValueCollection collection, bool isOverwrite = false)```|Név-érték párok hozzáadása egy létező ```NameValueCollection```-ból. Ha ```isOverwrite``` igaz, akkor az azonos kulcs értékű elemet felülírja.
```void Add((Dictionary<string,string> dictionary, bool isOverwrite = false)```|Név-érték párok hozzáadása egy létező ```Dictionary<string,string>```-ból. Ha ```isOverwrite``` igaz, akkor az azonos kulcs értékű elemet felülírja.
```void Set(string name, string value)```|Megadott nevű változó értékének módosítása.
```bool ContainsVariable(string name)```|Igaz értékkel jelzi, ha a név már szerepel a gyűjteményben.
```bool IsValidName(string name)```|Változónév ellenőrzése. A névnek meg kell felelnie az "[a-zA-Z_]\w*" reguláris kifejezésnek.
```void ResetSystemVariables()```|Rendszerváltozók értékének (újra)beállítása az LCID változó kivételével.
```string Substitution(string text)```|A szövegbe behelyettesíti a gyűjteményben található változók értékét.
```string Substitution(string text, string name)```|A szövegbe behelyettesíti a gyűjteményben a ```name``` változó értékét.
```public List<string> FindVariables(string text)```|A függvény megnézi, hogy a megadott <paramref name="text"/> paraméterben léteznek-e a gyűjteményben szereplő változók. A megtalált elemek kulcsaival egy listát képez.

#### A rendszerváltozók
A rendszerváltozók neve egy statikus SystemVariableNames nevű osztályban érhetőek el.
> Az XML fájban való hivatkozás bemutatásánál az alapértelmezett név elválasztót használtuk.
 
Név|XML hivatkozás|Leírás
:----|:----|:------
LCID|@LCID@|A nyelvi kódot tartalmazó rendszer változó neve.
USERNAME|@USERNAME@|Egy felhasználó név, melyet példányosításkor vagy később is be lehet állítani.
TODAY|@TODAY@|Mai nap rendszer változó neve.
YESTERDAY|@YESTERDAY@|Tegnap rendszer változó neve.
NOW|@NOW@|Most rendszerváltozó neve.
THISWEEKMONDAY|@THISWEEKMONDAY@|E hét hétfő rendszer változó neve.
THISWEEKFRIDAY|@THISWEEKFRIDAY@|E hét péntek rendszer változó neve.
LASTWEEKMONDAY|@LASTWEEKMONDAY@|Múlt hét hétfő rendszer változó neve.
LASTWEEKFRIDAY|@LASTWEEKFRIDAY@|Múlt hét péntek rendszer változó neve
THISMONTH1STDAY|@THISMONTH1STDAY@|E hónap első napja rendszer változó neve.
THISMONTHLASTDAY|@THISMONTHLASTDAY@|E hónap utolsó napja rendszer változó neve.
LASTMONTH1STDAY|@LASTMONTH1STDAY@|Múlt hónap első napja rendszer változó neve.
LASTMONTHLASTDAY|@LASTMONTHLASTDAY@|Múlt hónap utolsó napja rendszer változó neve.
DAYSBACK|@DAYSBACK#@|Valahány(#) nappal korábbi nap.
WEEKSBACK|@WEEKSBACK#@|Valahány(#) héttel (1hét=7nap) korábbi nap.
MONTHSBACK|@MONTHSBACK#@|Valahány(#) hónappal korábbi nap.

#### Osztály használatára egy minta
```javascript
/// <summary>
/// Minta a VariableDictionary osztály használatára.
/// </summary>
private static void VariableDictionaryTest()
{
    try
    {
        // Az osztály példányosítása. Nyelvi kód paraméter kötelező.
        VariableDictionary vd = new VariableDictionary("hu-HU", User.Identity.Name);
        Show(vd);

        System.Threading.Thread.Sleep(1000);

        vd.ResetSystemVariables();  //rendszerváltozók újra beállítása
        Show(vd);

        vd.Add("VLTZ", "vltz értéke");  //egy változó hozzáadása
        vd["TODAY"] = "ma";             //egy változó módosítása
        vd.Remove("NOW");               //egy változó törlése
        Show(vd);

        string text = String.Concat(
            "aaaaa@YESTERDAY@bbbbbbb@TODAY@cccccccc",
            "@LASTMONTHLASTDAY@ddddddddddd\neee@DAYSBACK3@ff",
            "ff@WEEKSBACK4@gggg@MONTHSBACK10@hhhh"
        );
        string result = vd.Substitution(text);  //szövegben lévő hivatkozások behelyettesítése
        Console.WriteLine();
        Console.WriteLine($"text= {result}");
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
}

private static void Show(VariableDictionary vd)
{
    Console.WriteLine();
    foreach (KeyValuePair<string,string> s in vd)
        Console.WriteLine($"Name: {s.Key,-25}Value: {s.Value}");
}
```



## XmlCondition osztály
Az alábbi xml struktúra feldolgozását segítő osztály:
```xml
<Condition Type="equal" Test="@VAR1@" With="one">
  four
  <Conditions>
    <Condition Type="equal" Test="@VAR1@" With="xxx">xxx</Condition>
    <Condition Type="equal" Test="@VAR1@" With="one">five</Condition>
  </Conditions>
</Condition>
```
A 'Condition' XML elem megszerzése után egy példányosítással előáll egy XmlCondition
típus.
```javascript
XmlCondition condition = new XmlCondition(XML.Element("Condition"));
```
Tulajdonság|Típus|Leírás
:----|:----|:----
Type|```string```|A feltétel típusa. Types enum értékek valamelyike. Alapértelmezett érték: Types.Equal.
Test|```string```|A egyik tesztelendő karakterlánc. Lehet benne XML változó. Types.Match esetén a kiértékelendő karakterléncot tartalmazza.
With|```string```|A másik tesztelendő karakterlánc. Lehet benne XML változó. Types.Match esetén a kiértékelő reguláris kifejezést tartalmazza.
Value|```string```|Igaz értékű feltétel esetén, ez lesz a releváns érték.
Conditions|```List<XmlCondition>```|XmlCondition típusú elemek listája. Ha a releváns érték más feltétel(ek)től is függ.

Metódus|Leírás
:----|:----
bool Evaluation(VariableDictionary xmlVars)|A feltétel kiértékelése. Mivel létezhetnek hivatkozások, ezért meg kell adni a változók gyűjteményét.

### XmlCondition.Types enum
A feltételekben a "Type" attribútumban megadható típusok.

Név|Leírás
:----|:----
```XmlCondition.Types.Else```|Mindig igaz feltétel típus.
```XmlCondition.Types.Equal```|Egyenlő. Ez az alapértelmezés.
```XmlCondition.Types.NotEqual```|Nem egyenlő.
```XmlCondition.Types.Match```|Regex kifejezés.

### Beépített kiterjeszthető osztályok

#### XmlCondition.ElementNames osztály
Általában használatos elemnevek kiterjeszthető osztálya. 

Állandó|Értéke|Leírás
:----|:----|:----
CONDITIONS|"Conditions"|XML tagokban lehetséges 'Conditions' elem eléréséhez hasznos állandó.
CONDITION|"Condition"|XML tagokban lehetséges 'Condition' elem eléréséhez hasznos állandó.

#### XmlCondition.AttributeNames osztály
Általában használatos attribútumnevek kiterjeszthető osztálya.

Állandó|Értéke|Leírás
:----|:----|:----
TYPE|"Type"|XML tagokban lehetséges 'Type' attribútum eléréséhez hasznos állandó.
TEST|"Test"|XML tagokban lehetséges 'Test' attribútum eléréséhez hasznos állandó.
WITH|"With"|XML tagokban lehetséges 'With' attribútum eléréséhez hasznos állandó.



## XmlVariable osztály
Az alábbi struktúra feldolgozását segítő osztály:
```xml
<XmlVar Name="VAR3" LCID="hu-HU">
  <Conditions>
    <Condition Type="equal" Test="@VAR1@" With="one">
        four
        <Conditions>
        <Condition Type="equal" Test="@VAR1@" With="xxx">xxx</Condition>
        <Condition Type="equal" Test="@VAR1@" With="one">five</Condition>
        </Conditions>
    </Condition>
  </Conditions>
  three
</XmlVar>

VAGY

<XmlVar Name="VAR4" LCID="hu-HU">érték</XmlVar>
```
Xml változókat leképező osztály. Tulajdonképpen minden elem, melynek 'Name', 'LCID' attribútuma lehetséges, 
és van 'Value' tulajdonsága, és lehetnek benne feltétek (Conditions).
Az 'XmlVar' XML elem megszerzése után egy példányosítással előáll egy XmlVariable típus.
```javascript
XmlVariable variable = new XmlVariable(XML.Element("XmlVar"));
```

Tulajdonság|Típus|Leírás
:----|:----|:----
Name|```string```|A változó neve. Csak olyan változó jön létre a példányosításkor, amelynek létezik a megnevezése.
LCID|```string```|A változó értékét, melyik nyelvi környezet esetén lehet felhasználni. Ha nincs vagy üres, akkor mindegyikben.
Value|```string```| A változó értéke.
Conditions|```List<XmlCondition>```|A változó végleges értékét befolyásoló feltételek listája. Az első igaz érték lesz a végleges érték.

Metódus|Leírás
:----|:----
bool Evaluation(VariableDictionary xmlVars)|A változó kiértékelése. Mivel létezhetnek hivatkozások a feltételekben, ezért meg kell adni a változók gyűjteményét.

### Beépített kiterjeszthető osztályok

#### XmlVariable.ElementNames osztály
Általában használatos elemnevek kiterjeszthető osztálya. Az XmlLinqBase hasonló osztályát terjeszti ki.

Állandó|Értéke|Leírás
:----|:----|:----
XMLVAR|"XmlVar"|XML tagokban lehetséges 'XmlVar' elem eléréséhez hasznos állandó.
CONNECTIONSTRING|"ConnectionString"|XML tagokban lehetséges 'ConnectionString' elem eléréséhez hasznos állandó.

#### XmlVariable.AttributeNames osztály
Általában használatos attribútumnevek kiterjeszthető osztálya.

Állandó|Értéke|Leírás
:----|:----|:----
NAME|"Name"|XML tagokban lehetséges 'Name' attribútum eléréséhez hasznos állandó.
LCID|"LCID"|XML tagokban lehetséges 'LCID' attribútum eléréséhez hasznos állandó.



## XmlConnection osztály
Az XmlPaser példányosításához egy kapcsolati sztring szükséges, amelyet ez az osztály ellenőriz és
kifejt. Az alábbi táblázat "Tulajdonság" oszlopában zárójelben az látható, hogy az adott összetevőnek
milyen néven kell szerepelnie a kapcsolati stringben.

Tulajdonság|Típus|Leírás
:----|:----|:----
Root (root)|```string```|A gyökér XML fájl az elérési útvonalával együtt, tartalmazhat relatív útvonalat is.
ConfigurationName (config)|```string```|A konfiguráció neve, amit keresünk a gyökér XmlParser fájlban.
File (file)|```string```|Ha ez van megadva a connection stringben, akkor itt van a komponens xml paraméter fájlja.
Element (element)|```string```|Ha ez van megadva a connection stringben, akkor a komponens xml paraméter fájljában ezen elem alatt található a struktúra.

Metódus|Leírás
:----|:----
```void SetPath(string appPath)```|A 'Root' és 'File' tulajdonságok relatív értékének feloldása a megadott útvonallal.

#### Alapértelmezettől eltérő konstruktorok
Konstruktor|Leírás
:----|:----
```XmlConnection(string xmlConnectionString, bool isRequired = true)```|XmlConnection példányosítása egy kapcsolati sztring megadásával, melynek során a sztring ellenőrzésre és feldolgozásra kerül.
```XmlConnection(string xmlConnectionString, string defaultValue, DefaultTypes defaultType = DefaultTypes.File)```|XmlConnection példányosítása egy kapcsolati sztring megadásával, melynek során a sztring ellenőrzésre és feldolgozásra kerül. Alapértelmezett értéket lehet megadni a "file" vagy "config" elemnek, ha azok üresek egyébként.

#### Nyilvános állandók
Elérésükre egy minta:
```javascript
XmlConnection.CSNAME_ROOT
```
Állandó|Értéke|Leírás
:----|:----|:----
CSNAME_ROOT|"root"|A kapcsolati sztring "root" elem megnevezésének állandója.
CSNAME_CONFIG|"config"|A kapcsolati sztring "config" elem megnevezésének állandója.
CSNAME_FILE|"file"|A kapcsolati sztring "file" elem megnevezésének állandója.
CSNAME_ELEMENT|"element"|A kapcsolati sztring "element" elem megnevezésének állandója.
SEPARATOR_NAMEVALUE|"="|Egy elemben a név és értéket elválasztó jel.
SEPARATOR_ITEM|";"|A kapcsolati sztringben az egyes elemeket elválasztó jel.

#### Kapcsolati sztring felépítése:
A minimum igény, hogy a 'config' vagy a 'file' tagnak szerepelnie kell.
A config az erősebb, ha mindkettő szerepel. Pár minta:
* "root=D:\SandBox\XmlParser\XmlParser.xml;config=FileManager" vagy
* "root=D:\SandBox\XmlParser\XmlParser.xml;file=D:\aaa\bbb\FileManager.xml;element=RootAlattiElemNév"
* Ha 'root' nem szerepel, akkor a gyökér fájl (1) elsődleges alapértelmezését az alkalmazás appconfig file-jában levő 
"Vrh.XmlParser:root" nevű elem tartalmazza; ha ez nem létezik, vagy értéke üres, akkor (2) a másodlagos alapértelmezés
"~/App_Data/XmlParser/XmlParser.xml". 
* A felhasználó komponensek fogadhatnak üres kapcsolati stringet, ha számukra 
van érvényes alapértelmezett konfiguráció név. Például a FileManager meghívható kapcsolati sztring nélkül, 
akkor a FilManager a következő sztringet generálja: "config:FileManager", és ezzel inicializálja az XmlParser-t.

## XmlParser osztály
Az 'XmlParser' XML elem feldolgozását elvégző absztrakt osztály. A VRH paraméterező XML
állományainak egységes szerkezetű eleme, mely definiálja az állomány részére a változókat, és kapcsolatokat. 
Valamint meghatározza a paraméterezés nyelvi környezetét, ha azt nem adják meg a programban.
Az XmlParser elem általános felépítése és logikája a következő:
```xml
<ParserConfig LCID="" NameSeparator="">        <!-- NameSeparator opcionális -->
  <XmlParser>                              
    <ConnectionString Name="" LCID="">...VALUE...</ConnectionString><!-- LCID opcionális -->
    <XmlVar Name="" LCID="">...VALUE...</XmlVar>
    <XmlVar Name="" LCID="">...VALUE...</XmlVar>
    <XmlVar Name="" LCID="">...VALUE...
      [<Conditions>
        <Condition Type="" Test="" With="">...VALUE...
          [<Conditions>...</Conditions>]
        </Condition>
      </Conditions>]
    </XmlVar>
  </XmlParser>
  <Configurations>
    <Configuration Name="Sample1" File="Sample1File" Element="FirstElement/SecondElement">
    <Configuration Name="Sample2" Element="Sample2Element">        <!-- File,Element valamelyik kötelező -->
    <Configuration Name="Sample3" File="Sample3File">
  </Configurations>
  <Sample2Element>
    [<XmlParser>...<XmlParser>]
    .
    .
    .
  </Sample2Element>
</ParserConfig>
```
Sample1File:
```xml
<Root>
  [<XmlParser>...</XmlParser>]
  <FirstElement>
    <SecondElement>
      [<XmlParser>...</XmlParser>]
      .
      .
      .
    </SecondElement>
  </FirstElement>
</Root>
```
Sample3File:
```xml
<Root>
  [<XmlParser>...</XmlParser>]
  .
  .
  .
</Root>
```
Az osztály a lenti sorrendben és helyeken elvégzi az XmlParser elemek feldolgozását:
* Az XmlParser.xml fájl gyökerében elhelyezett XmlParser elem
* A konfiguráció által meghatározott fájl gyökér eleme alatti XmlParser elem
* A konfiguráció által meghatározott fájl és egy ottani elem alatti XmlParser elem
Az XmlVar elemek (a továbbiakban változók) egy későbbi XmlParser-ban felülírhatóak.
Másképpen fogalmazva: ha a feldolgozás során olyan változót talál, mely már egy korábbi 
XmlParser-ban szerepelt, akkor annak értéke felülíródik a későbben megtalált ilyen nevű
változó értékével. Az XmlParser elemek feldolgozása után az előfeldolgozó a paraméterező XML
állományban elvégzi a változókra való hivatkozások feloldását, azaz behelyettesíti a hivatkozások
helyére a változók értékét. Utána a saját xml feldolgozó részére megtartja a tartalmat.
Az XmlParser.RootElement (XmlLinqBase.RootElement) tulajdonságában már egy olyan XML struktúra van, 
amelyben a behelyettesítések el lettek végezve. Az XmlParser (XmlLinqBase) által szolgáltatott 
metódusok már mind ezen az elemen dolgoznak.

Az osztály egy absztrakt osztály, felhasználása a következő módon lehetséges:
```javascript
public class MyXmlProcessor : XmlParser
{
    public MyXmlProcessor() : base(xmlConnectionString, appPath, lcid, otherVars)
}
```
* **xmlConnectionString** Az XmlParser kapcsolati sztringje. Lásd: [XmlConnection osztály](##XmlConnection-osztaly)
* **appPath** A felhasználó alkalmazásban érvényes alkalmazás mappa. (A '~' jel értéke.)
* **lcid** A nyelvi környezetet meghatárázó nyelvi kód. Ha üres, akkor az XmlParser.xml-ben megadott nyelv kód lesz alkalmazva.
* **otherVars** Egy szótár, mely név érték párokat tartalmaznak, melyek bekerülnek az XmlVars-ok közé. 

Tulajdonság|Típus|Leírás
:----|:----|:----
XmlVars|```VariableCollection```|Változók gyűjteménye, mely tartalmazza az összes változót az értékével együtt.
ConnectionStrings|```VariableCollection```|Kapcsolatok gyűjteménye, mely tartalmazza a struktúra összes különböző nevű kapcsolatát.
CurrentFileName|```string```|Az épp feldolgozás alatt álló Xml fájl a teljes fizikai elérési útvonalával.
Configuration|```ConfigurationType```|A megadott nevű komponens Configuration elemének értékei.

### Beépített static osztályok

#### XmlParser.Defaults osztály

Állandó|Értéke|Leírás
:----|:----|:----
XMLFILE|@"~\App_Data\XmlParser\XmlParser.xml"|Az XMLParser.xml fájl meghatározott helyének állandója.

A controllerben a következő utasítással feloldható: ```Server.MapPath(XmlParser.Defaults.XMLFILE);```

### Beépített osztályok
Ha szükséges vagy hasznos, akkor a felhasználó osztály kiterjesztheti ezeket.

#### XmlParser.ElementNames osztály
Általában használatos elemnevek kiterjeszthető osztálya. Az XmlLinqBase hasonló osztályát terjeszti ki.

Állandó|Értéke|Leírás
:----|:----|:----
XMLPARSER|"XmlParser"|XML tagokban lehetséges 'XmlVar' elem eléréséhez hasznos állandó.
XMLVAR|"XmlVar"|XML tagokban lehetséges 'XmlVar' elem eléréséhez hasznos állandó.
CONNECTIONSTRING|"ConnectionString"|XML tagokban lehetséges 'ConnectionString' elem eléréséhez hasznos állandó.
CONFIGURATIONS|"Configurations"|XML tagokban lehetséges 'Configurations' elem eléréséhez hasznos állandó.
CONFIGURATION|"Configuration"|XML tagokban lehetséges 'Configuration' elem eléréséhez hasznos állandó.

#### XmlParser.AttributeNames osztály
Általában használatos attribútumnevek kiterjeszthető osztálya.

Állandó|Értéke|Leírás
:----|:----|:----
NAME|"Name"|XML tagokban lehetséges 'Name' attribútum eléréséhez hasznos állandó.
LCID|"LCID"|XML tagokban lehetséges 'LCID' attribútum eléréséhez hasznos állandó.
NAMESEPARATOR|"NameSeparator"|XML tagokban lehetséges 'NameSeparator' attribútum eléréséhez hasznos állandó.
FILE|"File"|XML tagokban lehetséges 'File' attribútum eléréséhez hasznos állandó.
ELEMENT|"Element"|XML tagokban lehetséges 'Element' attribútum eléréséhez hasznos állandó.


***
## Version History:

#### 1.0.0 (2019.05.14) Initial version:
- A Vrh.Web.Common.Lib 1.18.1-es változatában lévő változatból készült. A Vrh.Web.Common.Lib 2.0-ás változatában már nem fog szerepelni.
- Ha az XmlParser connection string nem tartalmaz root elemet, akkor az alapértelmezést elsődlegesen az alkalmazás appconfig file-jában levő "VRH.XmlParser:root" nevű elemből veszi ki. Ha ilyen elem nem létezik, vagy a tartalma üres, akkor használja a programba bedrótozott "~\App_Data\XmlParser\XmlParser.xml" értéket
- Dokumentáció frissítése.
- ReadMe.md-ből a "csharp" jelölések cseréje "javascript"-re.


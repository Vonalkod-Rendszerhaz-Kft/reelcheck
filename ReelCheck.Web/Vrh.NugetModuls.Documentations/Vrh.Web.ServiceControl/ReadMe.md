# Vrh.Web.ServiceControl - Windows szerviz menedzser
> Fejlesztve és tesztelve **4.5** .NET framework alatt. Más framework támogatása további tesztelés függvénye, 
> de nem kizárt alacsonyabb verziókban való működőképessége.

A rendszert alkotó windows service-ek menedzselését teszi lehetővé. 
Az összes szervizre vonatkozó (common) funkciók és csak az egyes 
szervizekre vonatkozó, egy url-en keresztül elérhető egyedi funkciók 
definiálhatók, amelyek nyomógombokról érhetők el. 
Az egyes szervizek leállítása és újraindítása funkciót a program maga 
képes kezelni, ezeket nem kell egyedileg definiálni.

A komponens meghívható akcióit a **ServiceControl area** és **ServiceControl controller** 
elemeken keresztül lehet elérni.
Példa egy URL-re: *[application]/ServiceControl/ServiceControl/Index*

## Akciók
* **Index** akció: Az XML-ben lévő definíciók alapján a szolgáltatások megjelenítése.

### Index akció
Paraméter|Leírás
:----|:------

# TODO: Ennek a leírásnak a korrekt elkészítése!

****
## Version History:

#### 2.1.1 (2019.05.15) Patches:
- Assembly név beállítása, hogy azonos legyen a csomag és a projekt nevével.

#### 2.1.0 (2019.05.14) Compatible changes:
- Átállás a Vrh.Web.Common.Lib 2.0.0 változatára.
- Vrh.XmlProcessing 1.0.0 hozzáadása.
- Frissítés a Vrh.Common 2.3.0 hozzáadása.
- Frissítés a Vrh.Web.Menu 1.14.0 változatára.
- Frissítés a VRH.Log4Pro.MultiLanguageManager 3.3.0 változatára.

#### 2.0.0 (2018.10.22) Incompatible changes:
- XmlParser használatának bevezetése, ezért megszűnt az xml-ben a 'strings' elem.
- Bootstrap 4.1.3, jQuery 3.3.1, Font Awesome 5.4.1, Bootbox.JS 4.4.1 és a többi alap összetevő beállítása.

#### 1.2.0 (2018.02.20) Compatible changes:
- Config xml-be a Service tag alá új nem kötelező TimeOut attribútum, amelyben megadható, meddig várjon egy service control státus változtatás bekövetkeztére. Ha nincs megadva, akkor a 30 másodperc.
- Config xml-be a Service tag alá új nem kötelező ForceStop attribútum, amelyben megadható, hogy ha TimeOut-on belül nem következik be a reagálás a STOP parancsra, akkor kiölje a service processét (true esetén). Ha nincs megadva, akkor false.

#### Patches:
- Fenti működés átvezetése a STOP ágakra. Ha a STOP parancs állapotváltozása Timout-ra fut, akkor a ForceStop értékétől függően egy taskkill-lel kivégzi a procest, vagy hibát, dob, mimnt eddig.

#### V1.1.0 (2018.01.22) Initial version:
- A régi ServiceControl ebben a csomagban folytatja pályafutását. Egy kicsit korszerűsítve, a már kész 
NuGet csomagok beépítésével, és az XML feldolgozás átírásával.



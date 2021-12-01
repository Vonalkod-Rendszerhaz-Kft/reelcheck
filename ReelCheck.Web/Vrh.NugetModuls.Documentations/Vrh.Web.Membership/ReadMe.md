# Vrh.Web.Membership - Log4Pro felhasználó és jogosultság kezelés
> Fejlesztve és tesztelve **4.5** .NET framework alatt. Más framework támogatása további tesztelés függvénye, 
> de nem kizárt alacsonyabb verziókban való működőképessége.

A csomag form alapú hitelesítést tartalmaz a felhasználók számára
szerep és szerepkör kezeléssel.

## Főbb összetevők
* **[Fontos tudnivalók](##Fontos-tudnivalok)**: A modul használatával és telepítésével kapcsolatos fontos
információk. Olyan alapinformációk, melyek az elinduálshoz vagy megértéshez lényegesek.

  > * [Telepítés](###Telepites) A csomag telepítésével kapcsolatos információk. 
  > * [Beépített felhasználó](###Beepitett-felhasznalo) Beépített felhasználóval kapcsolatos információk.
 
* **[Felhasználó és jogosultság kezelés](##Felhasznalo-es-jogosultsag-kezeles)**: Elsődleges felhasználókkal és jogosultságaikkal 
kapcsolatos szolgáltatások.

  > * [PasswordService](###PasswordService): Jelszavakkal kapcsolatos szolgáltatások.
  > * [RoleService](###RoleService): Elsődleges felhasználók szerepeivel kapcsolatos szolgáltatások.
  > * [RoleGroupService](###RoleGroupService): Elsődleges felhasználók szerepköreivel kapcsolatos szolgáltatások.
  > * [UserService](###UserService): Elsődleges felhasználók kezelése.

* **[Másodlagos felhasználók](##Masodlagos-felhasznalok)**: A másodlagos felhasználókkal kapcsolatos információk.

  > * [SecondaryFunctionService](###SecondaryFunctionService): A másodlagos felhasználókat csoportosító funkciók kezelése.
  > * [SecondaryLoginService](###SecondaryLoginService): A másodlagos bejelentkezések nyilvántartása és azzal kapcsolatos szolgáltatások.
  > * [SecondaryUserService](###SecondaryUserService): A másodlagos felhasználók kezelése és egyéb szolgáltatások.

* **[Adatbázis](##Adatbazis)**: Modul adatbázis összetevőinek leírása.
 
## Fontos tudnivalók
### Telepítés
A komponens feltelepíti minden függőségét. Ha mégsem, akkor gyorsan be kell tenni 
a függőségek közé.
 
A hitelesítés miatt a Web.config-ot is módosítja a csomag az alábbiakkal.
Ez a tartalom található a "web.config.transform" fájlban:
```xml
<configuration>
  
  <appSettings>
    <add key="enableSimpleMembership" value="false" />
    <add key="autoFormsAuthentication" value="false" />
  </appSettings>
  
  <connectionStrings>
  
    <add name="VrhWebMembership" connectionString="data source=(local)\SQLEXPRESS;initial catalog=ReelCheck;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework" providerName="System.Data.SqlClient" />
    
  </connectionStrings>
  
  <system.web>
    
    <authentication mode="Forms">
      <forms loginUrl="~/" timeout="2880" />
    </authentication>
    
    <membership defaultProvider="DefaultMembershipProvider">
      <providers>
        <add name="DefaultMembershipProvider" type="System.Web.Providers.DefaultMembershipProvider, System.Web.Providers, Version=2.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" connectionStringName="VrhWebMembership" enablePasswordRetrieval="false" enablePasswordReset="true" requiresQuestionAndAnswer="false" requiresUniqueEmail="false" maxInvalidPasswordAttempts="5" minRequiredPasswordLength="6" minRequiredNonalphanumericCharacters="0" passwordAttemptWindow="10" applicationName="/" passwordCompatMode="Framework20" />
      </providers>
    </membership>
    
    <roleManager enabled="true" defaultProvider="DefaultRoleProvider">
      <providers>
        <add name="DefaultRoleProvider" type="System.Web.Providers.DefaultRoleProvider, System.Web.Providers, Version=2.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" connectionStringName="VrhWebMembership" applicationName="/" />
      </providers>
    </roleManager>
    
  </system.web>
  
</configuration>
```  
> **Ha már szerepel "VrhWebMembership" nevű [connectionString] elem, akkor 
> az egyiket törölni kell a telepítés után.**

### Beépített felhasználó
Ha üres, vagy olyan adatbázist talál a megadott kapcsolaton, ahol nem létezik még
az **Administrator** nevű szerep, akkor létrehozza az **Administrator** szerepet
és szerepkört. A szerepre azért van szükség, mert csak olyan felhasználó használhatja
a komponens akcióit, amelyik rendelkezik ezzel a szereppel.

Ha még nincs egyetlen felhasználó sem, vagy van, de nem létezik a beépített felhasználó, 
akkor létrehoz egy kezdeti felhasználót az alábbi névvel és jelszóval:
* Felhasználó név: **Admin**
* Jelszó: **Admin123**

A jelszó természetesen megváltoztatható.

## Felhasználó és jogosultság kezelés
### PasswordService
A ```MembershipUser``` típus jelszóval kapcsolatos szolgáltatásait nyújtó osztály.
Megvalósítja az ```Vrh.Web.Membership.IPasswordService```, és a ```System.IDispose``` interface-t. 
A leírásban a szöveg végén "(i)" jelzi, ha az adott elem egy interface megvalósítás része.
Az osztály a ```Vrh.Web.TranslationBase``` osztály kiterjesztése, az ahhoz 
tartozó összetevők végén "(t)" olvasható.

Tulajdonság|Típus|Leírás
:----|:----
Context|```DAL.MembershipContext```|A példányosításkor létrejött DBContext a közvetlen elérésekhez.
LCID|```string```|A környezetben érvényes nyelvi kód.(t)

Metódusok|Leírás
:----|:----
```void ChangePassword(MembershipUser user, string newPassword)```|Jelszó megváltoztatása az új jelszó beállításával.(i)
```void ChangePassword(MembershipUser user, string oldPassword, string newPassword)```|Jelszó megváltoztatása a régi jelszó ellenőrzésével.(i)
```void Dispose()```|IDispose interface által elvárt metódus megvalósítása.(i)
```string ResetPassword(MembershipUser user)```|Beállítja a felhasználó jelszavát egy új, automatikusan létrehozott jelszóra.(i)
```string ResetPassword(MembershipUser user, string passwordAnswer)```|Beállítja a felhasználó jelszavát egy új, automatikusan létrehozott jelszóra. Továbbá beállít egy új jelszó emlékeztetőt.(i)
```string Trans(Type wordCode)```|```MultiLanguageManager.GetTranslation``` meghívása a fordítás megkönnyítése érdekében.(t)
```string Trans(string wordCode, string defaultTrans = ""```|```MultiLanguageManager.GetTranslation``` meghívása a fordítás megkönnyítése érdekében. Ha nincs érvényes fordítás, akkor a defaultTrans lesz az értéke.(t)
```string TransFormat(Type wordCodeType, params object[] pars)```|```MultiLanguageManager.GetTranslation``` meghívása a pars behelyettesítésével.(t)
```string TransFormat(string wordCode, params object[] pars)```|```MultiLanguageManager.GetTranslation``` meghívása a pars behelyettesítésével.(t)
```void Unlock(MembershipUser user)```|Törli a felhasználó zárolt állapotát.(i)

### RoleService
A DefaultRoleProvider szolgáltatásai nyújtó osztály.
Megvalósítja az ```Vrh.Web.Membership.IRoleService```, és a ```System.IDispose``` interface-t. 
A leírásban a szöveg végén "(i)" jelzi, ha az adott elem egy interface megvalósítás része.
Az osztály a ```Vrh.Web.TranslationBase``` osztály kiterjesztése, az ahhoz 
tartozó összetevők végén "(t)" olvasható.

Tulajdonság|Típus|Leírás
:----|:----
All|```List<string>```|Egy lista a web.config-ban megadott alkalmazásnév összes szerepéről.
Enabled|```bool```|Annak jelzése, hogy használható-e a szerepkezelő.
LCID|```string```|A környezetben érvényes nyelvi kód.(t)

Metódusok|Leírás
:----|:----
```void AddToRole(MembershipUser user, string roleName)```|A user-ben megadott felhasználó hozzárendelése a roleName-ben megadott szerephez.(i)
```void AddUsersToRoles(List<string> userNameList, List<string> roleNameList)```|A userNameList-ben megadott felhasználók hozzárendelése a roleNameList-ben megadott szerepekhez.
```void Create(string roleName)```|Szerep hozzáadása a web.config-ban megadott alkalmazásnév szerepeihez.(i)
```void Delete(string roleName)```|Szerep törlése a web.config-ban megadott alkalmazásnév szerepeiből.(i)
```void Dispose()```|IDispose interface által elvárt metódus megvalósítása.(i)
```IEnumerable<string> FindAll()```|Létrehoz egy listát a web.config-ban megadott alkalmazásnév összes szerepéről.(i)
```IEnumerable<string> FindByUser(MembershipUser user)```|Létrehoz egy szerepneveket tartalmazó listát, amelyben a user-ben megadott felhasználó összes szerepe van benne.(i)
```IEnumerable<string> FindByUserName(string userName)```|Létrehoz egy szerep neveket tartalmazó listát, amelyben userName-ben megadott nevű felhasználó összes szerepe van benne.(i)
```IEnumerable<string> FindUserNamesByRole(string roleName)```|Létrehoz egy felhasználó neveket tartalmazó listát, amelyben roleName-ben megadott nevű szerep összes felhasználója neve található.(i)
```bool IsInRole(bool IsInRole(string userName, string roleName))```|Annak jelzése, hogy a userName-ben megadott nevű felhasználó rendelkezik-e a roleName-ben megadott nevű szereppel.
```bool IsInRole(MembershipUser user, string roleName)```|Annak jelzése, hogy a user-ben megadott felhasználó rendelkezik-e a roleName-ben megadott nevű szereppel.(i)
```void RemoveFromAllRoles(MembershipUser user)```|A user-ben megadott felhasználó törlése az összes szerepből.(i)
```void RemoveFromRole(MembershipUser user, string roleName)```|A user-ben megadott felhasználó törlése a roleName-ben megadott szerepből.(i)
```void RemoveUsersFromRoles(List<string> userNameList, List<string> roleNameList)```|A userNameList-ben megadott felhasználók törlése a roleNameList-ben megadott szerepekből. Ezzel a metódussal a beépített "Administrator" szerepkörből nem törlődnek a felhasználók.(i)
```string Trans(Type wordCode)```|```MultiLanguageManager.GetTranslation``` meghívása a fordítás megkönnyítése érdekében.(t)
```string Trans(string wordCode, string defaultTrans = ""```|```MultiLanguageManager.GetTranslation``` meghívása a fordítás megkönnyítése érdekében. Ha nincs érvényes fordítás, akkor a defaultTrans lesz az értéke.(t)
```string TransFormat(Type wordCodeType, params object[] pars)```|```MultiLanguageManager.GetTranslation``` meghívása a pars behelyettesítésével.(t)
```string TransFormat(string wordCode, params object[] pars)```|```MultiLanguageManager.GetTranslation``` meghívása a pars behelyettesítésével.(t)

### RoleGroupService
Szerepkörökkel kapcsolatos szolgáltatások.
A ```DAL.RoleGroup``` típus kezelését támogató osztály. Megvalósítja az 
```Vrh.Web.Common.Lib.IManage```, és a ```System.IDispose``` interface-t. 
A leírásban a szöveg végén "(i)" jelzi, ha az adott elem egy interface megvalósítás része.
A "DAL" a Vrh.Web.Membership.DAL névteret jelképezi, ahol a modul adatszerkezetei 
találhatók. Az osztály a ```Vrh.Web.TranslationBase``` osztály kiterjesztése, az ahhoz 
tartozó összetevők végén "(t)" olvasható.

Tulajdonság|Típus|Leírás
:----|:----
All|```List<DAL.SecondaryFunction>```|A tábla összes adatát vissza adja egy listában.(i)
Context|```DAL.MembershipContext```|A példányosításkor létrejött DBContext a közvetlen elérésekhez.
LCID|```string```|A környezetben érvényes nyelvi kód.(t)

Metódusok|Leírás
:----|:----
```void void Create(DAL.RoleGroup roleGroup)```|Szerepkör létrehozása.(i)
```void Delete(int id)```|Szerepkör törlése az egyedi azonosítója megadásával. A szerepkörhöz tartozó összerendelések is megszűnnek!(i)
```void Delete(string name)```|Szerepkör törlése az egyedi név megadásával. A szerepkörhöz tartozó összerendelések is megszűnnek!(i)
```void Dispose()```|IDispose interface által elvárt metódus megvalósítása.(i)
```DAL.RoleGroup Get(int id)```|Szerepkör lekérése az egyedi azonosítója alapján.(i)
```DAL.RoleGroup Get(string name)```|Szerepkör lekérése az egyedi neve alapján.(i)
```string Trans(Type wordCode)```|```MultiLanguageManager.GetTranslation``` meghívása a fordítás megkönnyítése érdekében.(t)
```string Trans(string wordCode, string defaultTrans = ""```|```MultiLanguageManager.GetTranslation``` meghívása a fordítás megkönnyítése érdekében. Ha nincs érvényes fordítás, akkor a defaultTrans lesz az értéke.(t)
```string TransFormat(Type wordCodeType, params object[] pars)```|```MultiLanguageManager.GetTranslation``` meghívása a pars behelyettesítésével.(t)
```string TransFormat(string wordCode, params object[] pars)```|```MultiLanguageManager.GetTranslation``` meghívása a pars behelyettesítésével.(t)
```void Update(DAL.RoleGroup roleGroup)```|Szerepkör módosítása a típus megadásával.(i)

### UserService
Felhasználó kezelését támogató szolgáltatások.
Lehetőleg csak ezen osztály metódusait használjuk karbantartás közben.
A DefaultMembershipProvider szolgáltatásait nyújtó osztály.
Megvalósítja az ```Vrh.Web.Membership.IUserService```, és a ```System.IDispose``` interface-t. 
A leírásban a szöveg végén "(i)" jelzi, ha az adott elem egy interface megvalósítás része.
Az osztály a ```Vrh.Web.TranslationBase``` osztály kiterjesztése, az ahhoz 
tartozó összetevők végén "(t)" olvasható.

Tulajdonság|Típus|Leírás
:----|:----
Context|```DAL.MembershipContext```|A példányosításkor létrejött DBContext a közvetlen elérésekhez.
LCID|```string```|A környezetben érvényes nyelvi kód.(t)
TotalUsers|```int```|Az adatbázisban szereplő összes felhasználó számával tér vissza.(i)
UsersOnline|```int```| Az alkalmazáshoz éppen hozzáférő felhasználók számát adja meg. Ahol a ```LastActivityDate``` kisebb, mint a mostani időpont csökkentve a ```UserIsOnlineTimeWindow``` beállítás értékével.(i)

Metódusok|Leírás
:----|:----
```List<MembershipUser> All(bool isWithoutTemporaryUser = true)```|Az összes felhasználót tartalmazó listát ad vissza.(i)
```public MembershipUser Create(string username, string email, string comment, string password, string confirmPassword, bool isApproved)```|Felhasználó létrehozása a paraméterlistában felsorolt paraméterek megadásával.(i)
```void Delete(MembershipUser user)```|Felhasználó törlése. A felhasználóval kapcsolatos egyéb adatok is törlődnek.(i)
```void Dispose()```|IDispose interface által elvárt metódus megvalósítása.(i)
```MembershipUser Get(string userName)```|Egy felhasználó adatainak lekérése a neve alapján úgy, hogy az nem számít aktivitásnak a felhasználónál. Vagyis a ```LastActivityDate``` nem változik.(i)
```MembershipUser Get(object providerUserKey)```|Egy felhasználó adatainak lekérése az egyedi azonosítója alapján úgy, hogy az nem számít aktivitásnak a felhasználónál. Vagyis a ```LastActivityDate``` nem változik.(i)
```MembershipUser Touch(MembershipUser user)```|Egy felhasználó adatainak lekérése az objektuma alapján úgy, hogy az aktivitásnak számít a felhasználónál. Vagyis a ```LastActivityDate``` megváltozik.(i)
```MembershipUser Touch(string userName)```|Egy felhasználó adatainak lekérése a neve alapján úgy, hogy az aktivitásnak számít a felhasználónál. Vagyis a ```LastActivityDate``` megváltozik.(i)
```MembershipUser Touch(object providerUserKey)```|Egy felhasználó adatainak lekérése az egyedi azonosítója alapján úgy, hogy az aktivitásnak számít a felhasználónál. Vagyis a ```LastActivityDate``` megváltozik.(i)
```void Update(MembershipUser user)```|Felhasználó adatainak módosítása.(i)

## Másodlagos felhasználók
### SecondaryFunctionService
A ```DAL.SecondaryFunction``` típus kezelését támogató osztály. Megvalósítja az 
```Vrh.Web.Common.Lib.IManage```, és a ```System.IDispose``` interface-t. 
A leírásban a szöveg végén "(i)" jelzi, ha az adott elem egy interface megvalósítás része.
A "DAL" a Vrh.Web.Membership.DAL névteret jelképezi, ahol a modul adatszerkezetei 
találhatók. Az osztály a ```Vrh.Web.TranslationBase``` osztály kiterjesztése, az ahhoz 
tartozó összetevők végén "(t)" olvasható.

Tulajdonság|Típus|Leírás
:----|:----
All|```List<DAL.SecondaryFunction>```|A tábla összes adatát vissza adja egy listában.(i)
Context|```DAL.MembershipContext```|A példányosításkor létrejött DBContext a közvetlen elérésekhez.
LCID|```string```|A környezetben érvényes nyelvi kód.(t)

Metódusok|Leírás
:----|:----
```void Create(DAL.SecondaryFunction secondaryFunction)```|Funkció létrehozása.(i)
```void Delete(int id)```|Funkció törlése az egyedi azonosítója megadásával. A funkcióhoz tartozó összes másodlagos bejelentkezés is törlődik!(i)
```void Delete(string name)```|Funkció törlése az egyedi név megadásával. A funkcióhoz tartozó összes másodlagos bejelentkezés is törlődik!(i)
```void Dispose()```|IDispose interface által elvárt metódus megvalósítása.(i)
```DAL.SecondaryFunction Get(int id)```|Funkció lekérése az egyedi azonosítója alapján.(i)
```DAL.SecondaryFunction Get(string name)```|Funkció lekérése az egyedi neve alapján.(i)
```string Trans(Type wordCode)```|```MultiLanguageManager.GetTranslation``` meghívása a fordítás megkönnyítése érdekében.(t)
```string Trans(string wordCode, string defaultTrans = ""```|```MultiLanguageManager.GetTranslation``` meghívása a fordítás megkönnyítése érdekében. Ha nincs érvényes fordítás, akkor a defaultTrans lesz az értéke.(t)
```string TransFormat(Type wordCodeType, params object[] pars)```|```MultiLanguageManager.GetTranslation``` meghívása a pars behelyettesítésével.(t)
```string TransFormat(string wordCode, params object[] pars)```|```MultiLanguageManager.GetTranslation``` meghívása a pars behelyettesítésével.(t)
```void Update(DAL.SecondaryFunction secondaryFunction)```|Funkció módosítása a típus megadásával.(i)

Ajánlott az Xml dokumentáció tanulmányozása is, mert az apróbb változások ott lesznek követhetőek az 
IntelliSense-en keresztül.

### SecondaryLoginService
Másodlagos felhasználók bejelentkezéseinek nyilvántartása.
Másképpen a ```DAL.SecondaryLogin``` típus kezelését támogató osztály. Megvalósítja az 
a ```System.IDispose``` interface-t. Az osztály a ```Vrh.Web.TranslationBase``` osztály kiterjesztése, az ahhoz 
tartozó összetevők végén "(t)" olvasható.
A "DAL" a Vrh.Web.Membership.DAL névteret jelképezi, ahol a modul adatszerkezetei 
találhatók.
Alapvető szabály, hogy egy adott célon csak 1 bejelentkezése lehet egy másodlagos felhasználónak.

Tulajdonság|Típus|Leírás
:----|:----
All|```List<DAL.SecondaryLogin>```|A nyilvántartásban lévő összes bejelentkezés listája, függetlenül a lejárattól. A lista rendezetlen.
AllowMultipleLogin|```bool```|Engedélyezett-e egy célon több bejelentkezés (másik másodlagos felhasználó). Alapértelmezett értéke: igen
Context|```DAL.MembershipContext```|A példányosításkor létrejött DBContext a közvetlen elérésekhez.
ExpirationTime|```int```|Egy bejelentkezés lejárati ideje másodpercben. Alapértelmezett értéke: 600 (10 perc). Az expirationTime nélküli metódusok ezt az értéket használják.
Function|```DAL.SecondaryFunction```|A bejelentkezés nyilvántartás melyik funkcióhoz tartozik.
LCID|```string```|A környezetben érvényes nyelvi kód.(t)
TargetKey|```string```|A cél, amihez a bejelentkezés tartozik. Alapértelmezett értéke: null. A targetKey nélküli metódusok ezt az értéket használják.

Metódus|Leírás
:----|:----
```void Dispose()```|IDispose interface által elvárt metódus megvalósítása.(i)
```DAL.SecondaryLogin Get(string secondaryUserName, string targetKey)```|Egy bejelentkezés keresése / lekérése. Megvizsgálja, hogy az adott célon a megadott névvel létezik-e bejelentkezés, ha nem akkor null-lal tér vissza.
```DAL.SecondaryLogin Get(int secondaryUserId, string targetKey)```|Egy bejelentkezés keresése / lekérése. Megvizsgálja, hogy az adott célon a megadott azonosítóval létezik-e bejelentkezés, ha nem akkor null-lal tér vissza.
```DAL.SecondaryUser GetSecondaryUser(string secondaryUserName)```|Egy másodlagos felhasználó rekord lekérése a példányosításkor megadott funkció és a secondaryUserName paraméterben megadott név alapján.
```bool IsExpired(string secondaryUserName), string targetKey, int expirationTime)```|Igaz, ha a secondaryUserName másodlagos felhasználó targetKey célon lévő bejelentkezés utolsó aktivitása óta már letelt az expirationTime-ban megadott idő.
```bool IsExpired(string secondaryUserName), string targetKey)```|Igaz, ha a secondaryUserName másodlagos felhasználó targetKey célon lévő bejelentkezés utolsó aktivitása óta már letelt az osztályban beállított ExpirationTime tulajdonságban beállított idő.
```bool IsExpired(string secondaryUserName)```|Igaz, ha a secondaryUserName másodlagos felhasználó TargetKey tulajdonság szerinti célon lévő bejelentkezés utolsó aktivitása óta már letelt az osztályban beállított ExpirationTime tulajdonságban beállított idő.
```bool IsExpiredAll(string targetKey, int expirationTime)```|Ha a targetKey célon lévő összes bejelentkezésnél az utolsó aktivitás óta eltelt expirationTime másodperc, akkor igaz értékkel tér vissza.
```bool IsExpiredAll(string targetKey)```|Ha a targetKey célon lévő összes bejelentkezésnél az utolsó aktivitás óta eltelt a beállított ExpirationTime másodperc, akkor igaz értékkel tér vissza.
```bool IsInRole(string name, string roleList)```|A másodlagos felhasználó elsődleges felhasználója rendelkezik-e valamelyik megadott szereppel. A roleList paraméter szerepnév vagy nevek listája vesszővel elválasztva.
```DAL.SecondaryLogin LazyLogin(string secondaryUserName, string targetKey)```|Bejelentkezés végrehajtása a jelszó ellenőrzése nélkül. Ha létezik már a bejelentkezés, akkor a LoginTime és a LastActivityTime frissítése.
```DAL.SecondaryLogin LazyLogin(string secondaryUserName)```|Bejelentkezés végrehajtása a jelszó ellenőrzése nélkül a beállított TargetKey alapján. Ha létezik már a bejelentkezés, akkor a LoginTime és a LastActivityTime frissítése.
```DAL.SecondaryLogin LoggedIn(string targetKey)```|A targetKey paraméterben megadott célon lévő legrégebben létrejött bejelentkezés. 
```DAL.SecondaryLogin LoggedIn()```|Az osztályban beállított TargetKey paraméterben megadott célon lévő legrégebben létrejött bejelentkezés. 
```List<DAL.SecondaryLogin> LoggedIns(string targetKey)```|A targetKey paraméterben megadott célon lévő összes bejelentkezés listája. A lista rendezetlen.
```List<DAL.SecondaryLogin> LoggedIns()```|Az osztályban beállított TargetKey paraméterben megadott célon lévő összes bejelentkezés listája. A lista rendezetlen. 
```DAL.SecondaryLogin Login(string secondaryUserName, string secondaryUserPassword, string targetKey)```|Bejelentkezés végrehajtása. Ha létezik már a bejelentkezés, akkor a LoginTime és a LastActivityTime frissítése.
```DAL.SecondaryLogin Login(string secondaryUserName, string secondaryUserPassword)```|Bejelentkezés végrehajtása a beállított TargetKey alapján. Ha létezik már a bejelentkezés, akkor a LoginTime és a LastActivityTime frissítése.
```void Logout(DAL.SecondaryLogin secondaryLogin)```|A secondaryLogin paraméterben megadott bejelentkezés törlése.
```void Logout(string targetKey)```|A secondaryUserName nevű másodlagos felhasználó targetKey célon lévő bejelentkezésének törlése.
```void Logout(DAL.SecondaryLogin secondaryLogin)```|A targetKey célon lévő összes bejelentkezés törlése.
```void Logout()```|Az osztályban beállítot TargetKey célon lévő összes bejelentkezés törlése.
```void LogoutAll()```|Az osztály létrehozásakor megadott funkció alatti összes bejelentkezés törlése.
```void Touch(DAL.SecondaryLogin secondaryLogin)```|Az utolsó aktivitás idejének frissítése a megadott secondaryLogin-ban.
```void Touch(string secondaryUserName, string targetKey)```|Az utolsó aktivitás idejének frissítése a megadott targetKey célon lévő secondaryUserName nevű felhasználó bejelentkezésénél.
```void Touch(string targetKey)```|Az utolsó aktivitás idejének frissítése a megadott targetKey célon lévő összes bejelentkezésnél.
```string Trans(Type wordCode)```|```MultiLanguageManager.GetTranslation``` meghívása a fordítás megkönnyítése érdekében.(t)
```string Trans(string wordCode, string defaultTrans = "")```|```MultiLanguageManager.GetTranslation``` meghívása a fordítás megkönnyítése érdekében. Ha nincs érvényes fordítás, akkor a defaultTrans lesz az értéke.(t)
```string TransFormat(Type wordCodeType, params object[] pars)```|```MultiLanguageManager.GetTranslation``` meghívása a pars behelyettesítésével.(t)
```string TransFormat(string wordCode, params object[] pars)```|```MultiLanguageManager.GetTranslation``` meghívása a pars behelyettesítésével.(t)
```bool Validate(string name, string password)```|Ellenőrzés, hogy a példányosításkor megadott funkción a megadott néven és jelszóval létezik-e másodlagos felhasználó.

### SecondaryUserService
A ```DAL.SecondaryUser``` típus kezelését támogató osztály. Megvalósítja az 
```Vrh.Web.Common.Lib.IManage```, és a ```System.IDispose``` interface-t.
Az osztály a ```Vrh.Web.TranslationBase``` osztály kiterjesztése, az ahhoz 
tartozó összetevők végén "(t)" olvasható.
A "DAL" a Vrh.Web.Membership.DAL névteret jelképezi, ahol a modul adatszerkezetei 
találhatók.

Tulajdonság|Típus|Leírás
:----|:----
All|```List<DAL.SecondaryUser>```|A tábla összes adatát vissza adja egy listában.(i)
FieldNameForTheLastException|```string```|Kivétel létrehozásakor jelzi, melyik mező miatt történt a kivétel. Ha null az értéke, akkor nem mezőhöz köthető a kivétel létrehozása.
Context|```DAL.MembershipContext```|A példányosításkor létrejött DBContext a közvetlen elérésekhez.
LCID|```string```|A környezetben érvényes nyelvi kód.(t)

Metódus|Leírás
:----|:----
```void Create(DAL.SecondaryUser secondaryUser)```|Másodlagos felhasználó létrehozása.(i)
```void Delete(int id)```|Másodlagos felhasználó törlése az egyedi azonosítója megadásával.(i)
```void Delete(string name)```|Másodlagos felhasználó törlése a név megadásával. Ha több azonos nevű van, más funkcióknál, azokat is törli. Ha nem talál egy törlendőt sem, akkor nem tesz semmit.(i)
```void Delete(string functionName, string name)```|Másodlagos felhasználó törlése a funkció nevének és a bejelentkezés nevének megadásával.
```DAL.SecondaryUser Get(int id)```|Másodlagos felhasználó lekérése az egyedi azonosítója alapján.(i)
```DAL.SecondaryUser Get(string name)```|Másodlagos felhasználó lekérése a név alapján. Ha több azonos nevű van más funkcióknál, akkor az elsőt fogja eredményezni.(i)
```DAL.SecondaryUser Get(string functionName, string name)```|Másodlagos felhasználó lekérése a funkció és a másodlagos bejelentkezés neve alapján.
```bool IsInRole(string functionName, string name, string roleList)```|A másodlagos felhasználó elsődleges felhasználója rendelkezik-e valamelyik megadott szereppel. A roleList paraméter szerepnév vagy nevek listája vesszővel elválasztva.
```string Trans(Type wordCode)```|```MultiLanguageManager.GetTranslation``` meghívása a fordítás megkönnyítése érdekében.(t)
```string Trans(string wordCode, string defaultTrans = ""```|```MultiLanguageManager.GetTranslation``` meghívása a fordítás megkönnyítése érdekében. Ha nincs érvényes fordítás, akkor a defaultTrans lesz az értéke.(t)
```string TransFormat(Type wordCodeType, params object[] pars)```|```MultiLanguageManager.GetTranslation``` meghívása a pars behelyettesítésével.(t)
```string TransFormat(string wordCode, params object[] pars)```|```MultiLanguageManager.GetTranslation``` meghívása a pars behelyettesítésével.(t)
```void Update(DAL.SecondaryUser secondaryUser)```|Másodlagos felhasználó módosítása a típus megadásával.(i)
```bool Validate(string functionName, string name, string password)```|Ellenőrzés, hogy az adott funkción és néven és jelszóval létezik-e másodlagos felhasználó.

****

## Adatbázis
### RoleGroups tábla
Szerepkörök táblázata.

Oszlop|Típus|Leírás
:----|:----
Id|```int```|Szerepkör egyedi azonosítója.
Name|```string```|Szerepkör egyedi megnevezése.

### RoleGroupRoles tábla
Szerepkörök és szerepek összerendelése.

Oszlop|Típus|Leírás
:----|:----
RoleGroupId|```int```|Szerepkör egyedi azonosítója.
RoleId|```Guid```|Szerep egyedi azonosítója.

### RoleGroupUsers tábla
Szerepkörök és felhasználók összerendelése.

Oszlop|Típus|Leírás
:----|:----
RoleGroupId|```int```|Szerepkör egyedi azonosítója.
UserId|```Guid```|Felhasználó egyedi azonosítója.

### SecondaryFunctions tábla
Lehetséges funkciókat tartalmazó táblázat, mely funkciókhoz
tartozhat a másodlagos felhasználó.

Oszlop|Típus|Leírás
:----|:----
Id|```int```|A funkció egyedi azonosítója.
Name|```string```|A funkció egyedi megnevezése.

### SecondaryLogins tábla
Másodlagos felhasználók bejelentkezéseit nyilvántartó táblázat.
Egy másodlagos felhasználónak egy bejelentkezése tartozhat egy célhoz.
Vagyis a UserID+TargetKey együtt egyedi kulcsot alkot.

Oszlop|Típus|Leírás
:----|:----
Id|```int```|A bejelentkezés belső egyedi azonosítója.
SecondaryUserId|```int```|Másodlagos felhasználó azonosítója, akinek a nevében történt a bejelentkezés.
TargetKey|```string```|A bejelentkezéshez tartozó azonosító. Értelmezése: Mi célból történt a bejelentkezés? MaxLength(20)
LoginTime|```DateTime```|Bejelentkezés időpontja.
LastActivityTime|```DateTime```|A legutóbbi aktivitás időpontja.

### SecondaryUsers tábla

Oszlop|Típus|Leírás
:----|:----
Id|```int```|Másodlagos felhasználó belső egyedi azonosítója.
UserId|```Guid```|Az elsődleges felhasználó egyedi azonosítója a dbo.Users táblából. Ő hozzá tartozik a másodlagos felhasználó.
SecondaryFunctionId|```int```|Melyik funkcióhoz tartozik a másodlagos felhasználó.
Name|```string```|Másodlagos felhasználó neve.
Password|```string```|Másodlagos felhasználó jelszava.
Active|```bool```|Másodlagos felhasználó érvényes-e jelenleg.

### UserSupplements tábla
A DefaultMembershipProvider 'User' táblájában nem szabad változtatásokat eszközölni
a jövőbeni esetleges Microsoft fejlesztések miatt.
Ez a tábla arra való, hogy a felhasználóra vonatkozó egyéb kiegészítő adatokat
legyen hol tárolni.
Ha nincsenek a felhasználónak kiegészítő adatai akkor nem lesz itt rekordja.

Oszlop|Típus|Leírás
:----|:----
UserId|```Guid```|A felhasználó egyedi azonosítója a 'User' táblából. Itt is csak 1 db azonosító lehet.
IsTemporary|```bool```|Annak jelzése, hogy a felhasználó ideiglenesen létrehozott felhasználó.  Ha igaz, akkor ideiglenes, egyébként hamis. Ha nem létezik a felhasználónak itt rekordja, akkor nem ideiglenes felhasználó.


***
# Version History:

#### 3.8.0 (2019.05.14) Compatible changes - debug:
- Átállás a Vrh.Web.Common.Lib 2.0.0 változatára.
- Vrh.XmlProcessing 1.0.0 hozzáadása.
- Vrh.Common 2.3.0 hozzáadása.
- Frissítés a VRH.Log4Pro.MultiLanguageManager 3.4.0 változatára.
- Frissítés a Microsoft.AspNet.WebPages.Data 3.2.7 változatára.
- Frissítés a Microsoft.AspNet.WebPages.WebData 3.2.7 változatára.
- Frissítés a WebActivatorEx 2.2.0 változatára.
- Frissítés a WebGrease 1.6.0 változatára.
- Frissítés a Font.Awesome 5.8.1 változatára.
- Frissítés a jQuery 3.4.1 változatára.
- RolesToUserRoleGroups felületen a dialóg ablakok "Mégse" gombjának stílusjavítása.
- Felhasználó kezelés felületen az "Utoljára aktív" oszlop kiírásainak javítása.

#### 3.7.2 (2019.04.04) Patches - debug:
- Database.SetInitializer meghívása a MembershipContext példány létrehozásakor.

#### 3.7.1 (2019.04.03) Patches - debug:
- SecondaryLogin LazyLogin javítás.

#### 3.7.0 (2019.04.03) Compatible changes - debug:
- SecondaryLogin "int TargetId" mező módosítása "string TargetKey"-re.
- SecondaryLoginService módosítása a fenti mező változása miatt.
- Dokumentáció módosítása. 

#### 3.6.0 (2019.03.27) Compatible changes - debug:
- SecondaryLogin átnevezése SecondaryUser-re, és a másodlagos bejelentkezés helyett a 
másodlagos felhasználó fogalom bevezetése.
- A SecondaryLoginService új neve SecondaryUserService.
- A SecondaryUserService osztály újabb metódusokkal bővült.
- Másodlagos bejelentkezések nyilvántartásának megvalósítása a SecondaryLoginService 
osztály létrehozásával.
- Dokumentáció bővítése, pontosítása.

#### 3.5.0 (2019.03.11) Compatible changes - debug:
- DataTier féle adatkezelés (vele a projekt is) megszűnt
- Entity Framwork 6.2 Code First bevezetése a Lib projekt DAL mappájában.
- DefaultMembershipProvider-en kívűli táblák külön sémába (UAdmin) helyezve.
- Új tábla és mezőnevek (RoleGroups, RoleGroupRoles, RoleGroupUsers)
- Másodlagos bejelentkezések kezelése. (Táblák: SecondaryLogins, SecondaryFunctions)
- Ideiglenes felhasználók elkülönítése. (Tábla: UserSupplements)
- Ideiglenes felhasználók bejelentkezéskori vizsgálata (törlése) a UserSupplements tábla alapján.
- VrhWebMembership_Connections.xml nem szükséges a továbbiakban.
- SecondaryFunctionService osztály elkészítése. A felület is ennek szolgáltatásait használja.
- SevondaryLoginService osztály elkészítése. A web felület is ezt használja.
- AspNetMembershipProviderWrapper osztály átnevezése UserService-re.
- AspNetRoleProviderWrapper osztály átnevezése RoleService-re.
- PasswordService osztály létrehozása, kikerült a UserService osztályból.
- RoleGroupService osztály létrehozása a szerepkörök kezelésére.
- A régi felületen sem lehet már közvetlen kivenni egy felhasználót egy szerep alól.
- Frissítés a Vrh.Web.Common.Lib 1.18.0 változatára.
- Frissítés a Moment.js 2.24.0 változatára.
- Frissítés a Microsoft.AspNet.Mvc 5.2.7 változatára.
- Frissítés a Microsoft.jQuery.Unobtrusive.Ajax 3.2.6 változatára.
- Frissítés a Microsoft.jQuery.Unobtrusive.Validation 3.2.11 változatára.

#### 3.4.1 (2019.02.12) Patches - debug:
- Megváltozott az ideiglenes felhasználónév képzés algoritmusa.
- A bejelentkezési gombot csak a folyamatok végeztével lehet újra megnyomni.
- Bejelentkezéskor törli a "WebReq_" prefixxel 5 napnál régebben létrehozott
  ideiglenes felhasználókat.

#### 3.4.0 (2019.01.30) Compatible changes - debug:
- Közvetlen bejelentkezés (DirectAuthentication) megvalósítása.
- Frissítés a VRH.Log4Pro.MultiLanguageManager 3.1.0 változatára.
- Frissítés a Vrh.Web.Common.Lib 1.17.0 változatára.

#### 3.3.0 (2018.11.07) Compatible changes - debug:
- Az XML-ben a WebReq típusnál a RequestTemplate elemben lett egy FromServer attribútum. 
Alapértelmezett értéke: false. Ha true, akkor a szervertől megy a kérés a távoli loginhoz. 

#### 3.2.0 (2018.10.10) Compatible changes - debug:
- Összes korábbi Login és Logout akció megszűnt.
- Új "Login" akció: Be vagy kijelentkezési felületet nyomógombbal aszerint, hogy van-e érvényes 
autentikáció vagy nincs. Vagy ablakban vagy beágyazva használható fel.
- Új "Logout" akció:  Csak ablakban hívható, ami csak akkor jelenik meg, ha valami megakadályozza a 
kijelentkezést.
- Membership.xml paraméterfájl bevezetése, amelyre az új akciók épülnek.

#### 3.1.3 (2018.09.14) Patches - debug:
- A loginJSON akció most már jó üzenetet ad vissza, ha nem sikeres a belépés.
- A "card-primary" osztálynév törlések is megtörténtek, mivel olyan nem is létezik.
- Míg nem betonbiztos, addig debug csomagok készülnek.
- Frissítés a VRH.Log4Pro.MultiLanguageManager 3.0.0 változatára.
- Frissítés a Vrh.Web.Common.Lib 1.12.0 változatára.
- Frissítés a Microsoft.AspNet.Mvc 5.2.6 változatára.
- Frissítés a Font.Awesome 5.3.1 változatára

#### 3.1.2 (2018.08.29) Patches:
- UserAdministration/UserAdministration Index.cshtml-en az 
"Új felhasználó létrehozása" link Bootstrap succes gombra formázva
- Csak Release configra buildeljen nugetet

#### 3.1.1 (2018.08.14) Patches - debug
- Szerepek (Roles) lapon a lista sötét háttérben világos betűvel, és piros nyomógombbal.
- A hivatkozás pedig működik a Vrh.Web.Menu alatt is.

#### 3.1.0 (2018.08.09) Compatible changes - debug
- LoginJSON és LogoutJSON post akciók létrehozása az AccountController-ben.

#### 3.0.1 (2018.05.10) Compatible changes - debug
- A ManyToMany integrálása a forrásba. Ezzel megszűnt az MvcContrib függőség.
- A legtöbb helyen a gombok és inputok bootstrap-es stílusokra támaszkodnak. 
- Futási időben létrejönnek a szükséges táblák. Létrehozza az "Administrator" 
szerepet és "Admin" felhasználót.
- Még fejlődik ezért csak a verzió 3. jegye módosult. 

#### 3.0.0 (2018.04.24) Initial version
1. Az új összetevőkhöz átalakított változat. Nem kompatibilis a kisebb verziókkal.



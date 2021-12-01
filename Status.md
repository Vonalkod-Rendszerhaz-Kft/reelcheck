# REDIS státusz változók

|REDIS változó      |Enum Típusa            |Lehetséges értékei |
|--------------|-------------|------------------|
|STATUS_ID_READ     |IdentificationResult   |NONE, PASS, NOREAD, EMPTY, DATAERR, INPROGRESS|
|STATUS_ID_RCCHECK  |IdentificationResult   |NONE, PASS, EMPTY, DATAERR, INPROGRESS|
|STATUS_ID_EXTCHECK |IdentificationResult   |NONE, PASS, EMPTY, DATAERR, INPROGRESS, NOACK, REFUSE|
|STATUS_PRN_PRINT   |PrintResult            |NONE, PASS, SKIP, EMPTY, INPROGRESS|
|STATUS_PRN_APPLY   |PrintResult            |NONE, PASS, SKIP, EMPTY, INPROGRESS|
|STATUS_CHK_READ    |CheckResult            |NONE, PASS, NOREAD, EMPTY, INPROGRESS|
|STATUS_CHK_RCCHECK |CheckResult            |NONE, PASS, NOREAD, EMPTY, DATAERR, INPROGRESS|
|STATUS_CHK_BOOKING |ExternalBookingResult  |NONE, EMPTY, DATAERR, NOK, INPROGRESS, PASSBOOKOK, PASSBOOKNOK|
|STATUS_EXIT        |Result                 |NONE, FAIL, PASS, EMPTY, GSPASS, GSFAIL|

# Enum érték magyarázatok

| Enum.érték    |Magyarázat|
|----------|---------------|
|IdentificationResult.None|Az azonosításnak (még) nincs eredménye, a művelet nem kezdődött el.|
|IdentificationResult.Pass|Sikeres művelet/részművelet|
|IdentificationResult.NoRead|Az olvasó NOREAD-et küldött. (Sikertelen olvasás/címke felismerés)|
|IdentificationResult.Empty|Üres a tekercspozició.|
|IdentificationResult.DataErr|Adathiba. Általában a konfig fájl definiciókban lévő hiba, vagy azok alapján nem felismerhető adatok.|
|IdentificationResult.NoAck|A CUSTOMFVS rendszer nem válaszolt (kommunikációs hiba)|
|IdentificationResult.Refuse|A CUSTOMFVS rendszer elutasította a tekercset|
|IdentificationResult.InProgress|A művelet folymmatban van.|
|||
|PrintResult.None|A Nyomtatásnak (még) nincs eredménye, a művelet nem kezdődött el.|
|PrintResult.Pass|Sikeres (a kinyomtatandó címke rendben kinyomtatódott)|
|PrintResult.Skip|Hiba lépett fel, és végül kihagyták a nyomtatást, vagy az aplikálást.|
|PrintResult.Empty|Üres a tekercspozició.|
|PrintResult.InProgress|A művelet folymmatban van.|
|||
|CheckResult.None|Az ellenőrzésnek (még) nincs eredménye, a művelet nem kezdődött el.|
|CheckResult.Pass|Sikeres művelet.|
|CheckResult.NoRead|Az ellenőrző kamera NOREAD-et küldött. (Sikertelen olvasás/címke felismerés)|
|CheckResult.Empty|Üres a tekercspozició.|
|CheckResult.DataErr|Adathiba. Általában a konfig fájl definiciókban lévő hiba, vagy azok alapján nem felismerhető adatok.|
|CheckResult.InProgress|A művelet folymmatban van.|
|||
|ExternalBookingResult.None|A külső könyvelésnek (még) nincs eredménye, a művelet nem kezdődött el.|
|ExternalBookingResult.InProgress|A művelet folymmatban van.|
|ExternalBookingResult.Empty|Üres a tekercspozició.|
|ExternalBookingResult.DataErr|A CustomFVS hibát jelezett, vagy hiba a config adatokban|
|ExternalBookingResult.NOK|Hiba a CustomFVS-sel való kommunikációban. Nem elérhető, nem válaszol, stb.|
|ExternalBookingResult.PassBookOK|Sikeresen jelezve a customFVS-nek, hogy a tekercs zöld státuszt kapva forgott ki|
|ExternalBookingResult.PassBookNOK|Sikeresen jelezve a customFVS-nek, hogy a tekercs piros státuszt kapva forgott ki|
|||
|Result.None|Még nincs a tekercsnek minősítése.|
|Result.Fail|A tekercs hibás (piros) minősítést kapott|
|Result.Pass|A tekercs bevételezett (zöld) minősítést kapott|
|Result.Empty|Üres a tekercspozició.|
|Result.GsPass|Aranymintateszt során berakott tekercs státusza: sikeres |
|Result.GsFail|Aranymintateszt során berakott tekercs státusza: sikertelen|

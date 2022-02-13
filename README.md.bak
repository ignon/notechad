# NoteChad++ :fire: :ok_hand: :sweat_drops: 


### Idea

Projektin tarkoitus on tehdä komentoriviltä käytettävä minimalistinen järjestelmä muistiinpanojen hallitsemiseen. Käyttäjä voi luoda noteja, jotka ovat käytännössä .txt-tiedostoja, joilla on tietty formaatti.

### Tavoitteita:

- helposti luettava formaatti
- muokattavissa kaikilla tekstieditoreilla
- notet helposti siirrettävissä muihin formaatteihin (Anki?)
- pelkästään näppäimistöllä käytettävissä ilman hiirtä
- portable exe-file, voi säilöä esim. dropboxissa notejen kanssa

### Ominaisuuksia:
- Noteille voi antaa tägejä, joiden perusteella tietystä kansiosta voi etsiä kaikki notet tietyllä (tai usemmalla) tägillä.
- Noteille voi antaa vuosiluvun (esim. 1916) tai aikavälin (1914-1918) ja noteja voi etsiä tietyltä ajankohdalta.

### Komentoja:

Notejen luonti ja avaaminen:

`notechad new Toinen maailmansota`

- luo tiedoston "toinen_maailmansota.txt" ja avaa sen oletustekstieditorissa tai käyttäjän määrittämässä tekstieditorissa

`notepad open Kolmas maailmansota`

- Jos "kolmas_maailmansota.txt" luotu jo:
	* Avaa sen.

- Jos kyseistä notea ei olemassa:
	* "No note with name 'Kolmas maailmansota' exists, create one? (Y/n)"

### Notejen formaatti
`notechad new Toinen maailmansota 1939-1945 #hitler #natsisaksa`

- luo tekstitiedoston *"toinen_maailmansota.txt"* seuraavalla formaatilla:

````
@Name Toinen maailmansota
Summary 					//Tiivistelmä otsikon alle.
Text						//Tiivistelmää täydentävä teksti.
@Tags hitler, natsisaksa
@TimeDate 1939-1945
````
- Tämän jälkeen käyttäjä voi muokata notea:

```
@Title: Toinen maailmansota

Toinen maailmansota oli vuosina 1939–1945 käyty maailmanlaajuinen konflikti. (<-- Tiivistelmä)

Konfliktin pääasialliset osapuolet olivat Saksan, Italian ja Japanin johtamat akselivallat sekä Britannian, Kiinan tasavallan, Neuvostoliiton, Ranskan ja Yhdysvaltain johtamat liittoutuneet. Yhteensä 73 miljoonaa ihmishenkeä..... (<-- Tiivistelmää täydentävä teksti)

@Tags: hitler, natsisaksa
@Datetime: 1939-1945
```



### Notejen haku komentoriviltä

`notechad find #toinen_maailmansota`

- Etsii kaikki Notet nykyisestä kansiossa ja sen alakansioista joissa on tägi #toinen_maailmansota

`notechad find 1943`

- Etsii kaikki Notet, jonka vuosi 1943 tai jonka aikaväliin 1943 sisältyy (esim. 1939-1945)

`notechad find #toinen_maailmansota 1943`

- Etsii Notet joissa tägi #toinen_maailmansota ja täsmää vuosi 1943

* Tulostaa terminaaliin vastaavan listan:

Name | File | Folder | Date
:-- | :-- | :-- | :--
**H**itler | hitler.txt | natsisaksa | 1889 - 1945 
**N**atsisaksa | natsisaksa.txt | natsisaksa | 1933 - 1945

```
[O]pen all (show summary and text)
	Open in:
	- [S]eperate files [Read/Write]
	- [O]ne file [Read only]

[P]eek all (show summary only)
	- [O]pen in editor
	- [E]xit
[E]xit
```


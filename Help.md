# Categories
- Build
- Work
- Manage

<br>

----

# Starting without db
You can:
- Populate Temp: 


## Populate Temp
### Load XML
Select an XML file containing ROM data; it will load and parse it, then display the results in a grid.
<img width="1651" height="165" alt="2025-10-28_20h07_12" src="./_Ressources/populate_sample.png" />

After it, you can save:
- The content to a sqlite database to populate it => table `TempRoms` (MyMameHelper create it)
- Build:
    - Constructeurs[^inc]
    - Genres[^inc]
    - Machines (model1, model2)[^Mach]

[^inc]: Il en manque, voir pourquoi
[^Mach]: Je n'ai pas vu de cps, mais ça vient peut être du set que j'ai employé

<br>

## Compare

<br>

## File Manager
<br>

## DbTools
You can create an empty database here.

<br>

----

# With a db
## Populate Temp
## Compare
## File Manager
## Move Games1

<br>

----
With a db AND Developers built




<br>

----
# Menu
## Build Developers
You can feed the Developers table with the field `manufacturer` from the temporary roms table.

<br>

## Build Constructors
> :warning: Unavailable until TempRoms table is populated  

You can feed the Constructeurs table with the temporary roms table.

<br>

## Build Roms
> :warning: Unavailable until TempRoms table is populated  
> :warning: Unavailable until Developers table is populated  

> :warning: refactorisation en cours.

Select the roms you want to keep and save it in the `roms` table.

Methods:
- Machine: Combine your destination path with the machine name.
- Favorites (not implemented)

<br>

Options:
- OverWrite: you overwrite the file if he is existing (usefull for a new romset)
- Unwanted.
- Use Files Information
> :warning: A voir une fois le code refactorisé

<br>

## DB Tools
- Create table
- Empty temp roms table



----

# Not Working
## Add Games1.
Need to revisiting the code behind.

## Move Games1

<br>

----

# FAQ
## How i begin ?
- Create db by using dbtools with MyMameHelper
- Create an xml file with M.A.M.E
- Populate temp 
    - Load XML with MyMameHelper
    - Save it
- Build Developers
- Now you have the list of the games you can add in the rom table

> :warning: Work In Progress

## What happens if i want to add some games
Currently you can add new games by feed with load XML the data base. You don’t have to populate it all at once.

## What happens if i save over a non empty database
You will be notified about constraint violations if some data already exist[^violation]. 

[^violation] peut être sortir sur un système récapitulatif en fin, car actuellement c'est assez pénible.
# MyMameHelper
 Help to manage M.A.M.E roms


27/10/2025: reprise du programme et publication, j'avais totalement oublié de le faire depuis 2020 ou 2022, je ne sais plus.

# Todo
- Urgent: en cas de base manquante le data context n'est pas initialisé (pourquoi pas). Mais à la création de la table il faudrait l'initialiser sinon on ne peut plus travailler... peut être que tout appel de DB Tools devrait recharger.
- Versioning de la base
- Mettre sur la page de copie des roms les aides pour expliquer  à quoi ça sert
- Mettre sur la page help.md à quoi servent les options
- Voir dans le code si j'ai pensé à utiliser du hash ou pas.
- Créatoin de la base, mettre un waiting et probablement passer en parallèle, bref à voir
- Revoir l'algo de feed surtout pour le rom.count
- Passer en .net core
- Reprendre contact avec le code pour sortir une documentation sur le fonctionnement, j'ai totalement oublié
- Voir s'il répond le besoin de filtrage des roms de manière simple.

# But
- Identifier les roms de types:
 - pinball (méchanical
 - fruit (méchanical)
- Voir éventuellement:
 - CPS1
 - CPS2
 - Naomi
 > (doit le faire normalement)
- Rajouter éventuellement le genre du jeu en xml.

# Notes
- Visiblement j'ai travaillé sur une base sqlite.

# Fonctionnalités
## MainPage
### Populate Temp
Load an xml from M.A.M.E


## dbtools
- Création de la base
<<<<<<< HEAD
- Ne lit pas la base si non présente
=======
- Ne lit pas la base si non présente

# Tests
## Exemples de résultat
Chargement d'un fichier XML tiré de MAME
<img width="2304" height="1117" alt="image" src="https://github.com/user-attachments/assets/eda39a20-bc90-4ac8-b870-c2b851ef8525" />
>>>>>>> 8e271a0c6aff76e5d462c63e1dc4d59289a05204

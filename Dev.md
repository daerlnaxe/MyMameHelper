
27/10/2025: reprise du programme et publication, j'avais totalement oublié de le faire depuis 2020 ou 2022, je ne sais plus. Analyse du fonctionnement via le code (je n'avais laissé aucune doc :/)

# Notes
- Build Roms permet en sélectionnant une des roms de récupérer tout le set associé pour feeder la base des roms
- Visiblement avant j'avais utilisé plusieurs couches telles que 
    - Je load le xml
    - Je feed les roms (qui sont les fichiers nommés dans le xml)
    - Je feed une base de jeux vidéos (donc indépendant des langues, versions etc.)
    - Sur cette base de jeux vidéos je pouvait déplacer, organiser... Des modifs indiquent que j'avais commencer à plancher pour me passer de l'étape sur la création des jeux pour recenter ça plutôt sur les roms.. Enfin je pense vu que j'ai commenté tout le code et fait disparaitre la page de préparation des jeux
- En bout l'attendu... je peux aussi déplacer mes jeux sur un simple système de load sur un fichier txt ... Je pense que ce fichier doit être un export par MyMameHelper, ça me parait la meilleure solution
- La partie Games, permettait de spécifier si c'était un flipper, un mahjong, un fruit games... Une table dédiée serait intéressante. Ca limite les colonnes.



# Todo

- [ ] Ajouter dans le build
- [ ] :warning: Réforme en urgence de la base de données.
    - [x] Structure
    - [ ] Feed par roms, récupératon de la description jusqu'à () + réfléchir à :
        - check avant pour éviter les erreurs ?
        - insert or ignore sous sqlite
        - récupérer l'erreur selon
    - Représentation des jeux avec édition
    - L'utilisateur doit pouvoir updater games via liste de jeux à partir d'un ancien export (plus de souplesse dans le travail + collaboratif possible).
        - Besoin d'export (les clés ne devraient pas spécialement être utiles, se baser sur la rom pour retrouver le jeu)
            - rom
            - +structure du jeu (genre, isquizz, is majhong...)
            - :warning: Automatiser un backup avant au cas où l'utilisateur ferait une erreur + warning.
    - Charger à minima un fichier pour les fruits/mahjong/quizz/fruits et pinball 
    - Table de jonction entre game et roms par sémantisme ?
- [x] Ajout des fruits
- [x] Ajout Pinball
- Voir pour mettre ma librairie pour les box, notamment pour choisir les dossiers dans file manager.
- :warning: voir en urgence le certificat
- :warning: Urgent: en cas de base manquante le data context n'est pas initialisé (pourquoi pas). Mais à la création de la table il faudrait l'initialiser sinon on ne peut plus travailler... peut être que tout appel de DB Tools devrait recharger => Décidé: oui
- :warning: Comprendre pour le mode archive et le mode game dans la creation des roms.
- :warning: Voir en urgence pour le déplacement des roms
    - Renommer que c'est du move
    - Voir pour identifier si c'est le même lecteur
        - Implanter si ce n'est pas le même lecteur le calcul de somme
        - Implémenter le déplacement si c'est le même lecteur
        - Favorites ?? Quelle était l'ID ? Un dossier peut être
        - Implémenter le mode full, on déplace juste dans le dossier
- :warning: Idée à voir, générer un dat compatible avec CLRMamePro quoiqu'il n'y a pas les hashs ...
- Identifier pourquoi je rajoute automatiquement des marques à la création de la base...
    - Par défaut pour faciliter
    - Sample pour le débug qui est resté
- Page d'accueil avec des stats
- Versioning de la base
- Mettre sur la page de copie des roms les aides pour expliquer  à quoi ça sert
- Mettre sur la page help.md à quoi servent les options
- Voir dans le code si j'ai pensé à utiliser du hash ou pas.
- Créatoin de la base, mettre un waiting et probablement passer en parallèle, bref à voir
- Revoir l'algo de feed surtout pour le rom.count
- Passer en .net core
- Reprendre contact avec le code pour sortir une documentation sur le fonctionnement, j'ai totalement oublié
- Voir s'il répond le besoin de filtrage des roms de manière simple.

<br>

----
# Done
- `Copy all` devient `File Manager`, ce qui permet copy ou déplacement.

----

# Functions
## DB Tools
### Création de Table
:warning: identifier le besoin

- Ajout à Constructeurs de 
    - Capcom
    - Konami
    - Sega


- Genres
    - Shoot Them Up
    - Fight



> Toutes les marques citées sont sous copyright etc etc... je ne sais plus le disclamer qu'il faut mettre mais vous êtes bien entendu au courant qu'elles ne m'appartiennent pas.


# Versions
06/11/2025 : 0.3.0.0
- Refactorisation et réintégration du système "Games" avec automatisation.
    - Le but était de: 
        - Lever une page de plus dans la construction des roms
        - Les déplacements de fichiers auront lieu sur Roms au lieu de Games (plus logique)
    - Le feed de `roms` feedera aussi `games` en prenant une partie de la description (sans les `()` par exemple).
    - L'utilisateur pourra revenir dessus, mais globalement games est différent au sens qu'il rajoute une couche d'options dessus, il est contournable à présent.
- La table Companies est renommée Manufacturers
- La page des Manufacturers prend le contenu de la table au chargement, pas simplement la partie provenant des roms
- L'ajout de rom de la liste de gauche à droite bénéficie maintenant d'une fenêtre de progression car le temps de traitement peut aller à 5 minutes.

# -
## Build Roms
Chargement de la page:
- Récupération des tables:
    - Developers
    - TempRoms

Passage de gauche à droite:
- Conversion du nom du manufacturer en Id Developper


## Map To Roms
- Lister les jeux:
    - ❌ Ils n'existent pas sauf si le build roms crée une base minimale => n'afficherait rien
    - ❌ Toutes les roms non linkées vont polluer une entrée "null".
    - ✅ Il y a moins de jeux que de roms et on peut mettre les roms comme de petites collections dans une case.
- Lister les roms ...
    - ❌ Plus de roms que de jeux
    - ✅ Plus simple.
    - ❌ Travail beaucoup plus important pour l'utilisateur


Dans l'intérêt de l'utilisateur, si l'on part sur lister les jeux:
- Au build de rom faire un pseudo build de games basé sur la description (until `()`)
- Au map to game je récupère les roms avec une jonction valide sinon rien.
- Je récupère une liste à droite qui contient les roms non linkées
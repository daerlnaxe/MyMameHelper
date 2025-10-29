
27/10/2025: reprise du programme et publication, j'avais totalement oublié de le faire depuis 2020 ou 2022, je ne sais plus. Analyse du fonctionnement via le code (je n'avais laissé aucune doc :/)

# Todo
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
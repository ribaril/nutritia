# Nutritia
﻿
[![Build status](https://ci.appveyor.com/api/projects/status/axic4ds0j9mcl3dc/branch/develop?svg=true)](https://ci.appveyor.com/project/Tri125/nutritia)
[![Version number](https://img.shields.io/badge/Version-0.5.9.0-blue.svg)](https://github.com/Nutritia/nutritia/releases)

[![Join the chat at https://gitter.im/Nutritia/nutritia](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/Nutritia/nutritia?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

### Progression du travail

[![Throughput Graph](https://graphs.waffle.io/nutritia/nutritia/throughput.svg)](https://waffle.io/nutritia/nutritia/metrics)

### Procédure de déploiement du logiciel Nutritia

#### Note de départ

Les *fichiers de configurations* sont: 
* `Nutritia_Creation.sql`
* `Nutritia_Insertion.sql`
* `Nutritia.exe.config`

Pour utiliser Nutritia, il est nécessaire d'avoir une connexion à une base de données MySql.

Veuillez vous assurez d'avoir accès à un base de données MySql avec des privilèges de création, d'insertion et de sélection pour pouvoir créer la base de données. Il est aussi nécessaire d'avoir le privilège d'update pour utiliser le logiciel Nutritia:

| Privilege     | Column        | Context                         |
| ------------- |:-------------:| -----:                          |
| CREATE        | Create_priv   | databases, tables, or indexes   |
| INSERT        | Insert_priv   |   tables or columns             |
| SELECT        | Select_priv   |    tables or columns            |
| UPDATE        | Update_priv   |    tables or columns            |
	
Le script de création créera la base de donnée avec le nom `420-5A5-A15_Nutritia` par défault, mais il est possible de choisir son propre nom. Il suffit de modifier les lignes contenant `420-5A5-A15_Nutritia`dans les *fichiers de configurations* par le nom désiré.

### Installation

- Connectez-vous sur votre serveur de base de données avec votre logiciel de développement de votre choix.

  - Lancer le script de création de base de données `Nutritia_Creation.sql`

  - Lancer le script d'insertion de données `Nutritia_Insertion.sql`

- Dans le dossier contenant l'exécutable Nutritia.exe, modifier le fichier `Nutritia.exe.config`.

   1. Localiser la ligne:
`<add name="MySqlConnexion" connectionString="server=localhost;userid=root;password=;database=420-5A5-A15_Nutritia" />`
   2.  Remplacer `localhost` par l'adresse de votre serveur de base de données. Alternativement, laissez le à `localhost` si votre serveur est local.
   3.  Remplacer `root` par votre nom d'utilisateur.
   4.  Si votre compte utilise un mot de passe, rajouter votre mot de passe suivant `password=`, mais avant le point-virgule.
   5.  Si vous avez remplacé le nom de la base de données, remplacer `420-5A5-A15_Nutritia` par le nom choisi.

Procédure de déploiement du logiciel Nutritia

1. Créer une base de données locale
	1a. Lancer le script de création de base de données (Nutritia_Creation.sql)
	1b. Lancer le script d'insertion de données (Nutritia_Insertion.sql)
2. Modifier le fichier app.config
	Changer le nom de la base de données dans la ligne suivante en fonction de celle choisie plus haut
	<add name="MySqlConnexion" connectionString="server=localhost;userid=root;password=;database=420-5A5-A15_Nutritia" />
using Nutritia.Logic.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nutritia.Toolkit
{
    /// <summary>
    /// Classe statique pour aider à la manipulation et la conversion d'objet sessions/ string de connexion MySql
    /// </summary>
    public static class SessionHelper
    {

        /// <summary>
        /// Méthode statique qui converti une string de connexion MySql en on objet Session
        /// </summary>
        /// <param name="connexion">String de connexion MySql</param>
        /// <returns>Session équivalent à la connexion</returns>
        private static Session GetSession(string connexion)
        {
            //Prend tout les champs formant la string de connexion. Chaque champ étant séparé par un point-virgule.
            String[] words = connexion.Split(';');

            if (words.Length < 5 || words.Length > 6)
                throw new ArgumentException(connexion + " n'est pas d'un format valide");
            //Pour chacun des champs
            for (int i = 0; i < words.Length; i++)
            {
                //Inutile
                if (!String.IsNullOrEmpty(words[i]))
                {
                    //Trouve l'index du symbole d'égalité.
                    int index = words[i].IndexOf('=');
                    //Si "=" a été trouvé
                    if (index != -1)
                    {
                        //Remplace le contenu de words[i] pour juste la valeur du champ, au lieu d'avoir le nom du champ, le égal et la valeur.
                        words[i] = words[i].Substring(index + 1);
                    }
                }
            }
            //Dans le cas où il n'y a pas de port spécifié.
            if (words.Length == 5)
            {
                return new Session(words[0], words[1], words[2], words[3], words[4]);
            }
            //Test limite couvre déjà < 5 et > 6, donc Length == 6 ici.
            else
                //Lorsqu'il y a un port spécifié.
                return new Session(words[0], words[1], words[3], words[4], words[5], int.Parse(words[2]));
        }

        /// <summary>
        /// Méthode statique qui convertie une string dans le format spécifié par la configuration du logiciel en List de sessions
        /// Le format de la string est essentiellement la même qu'une connexion string de MySql, mais qui supporte plusieurs connexion tous entouré par "{" et "}"
        /// </summary>
        /// <param name="sessions">String de format sessions: {name=x;server=y;userid=z;password=;database=a}{[...]}</param>
        /// <returns>List d'objets sessions</returns>
        public static List<Session> StringToSessions(string sessions)
        {
            //Liste de connexion string. Essentiellement comme sessions, mais sans les accolades.
            List<string> listConnexionStrings = new List<string>();
            List<Session> listSessions = new List<Session>();
            int indexStart;
            int indexEnd;
            //Trouve la position dans l'index de l'ouverture et de la fermeture de l'accolade.
            //Trouve les premiers, nous donne donc notre première connexion string.
            indexStart = sessions.IndexOf('{');
            indexEnd = sessions.IndexOf('}');
            //Tant et aussi longtemps qu'une accolade fermante existe et que celle ouvrante est à l'index 0 (donc, ne fait pas partie d'une des valeurs d'un champ).
            while (indexStart == 0 && indexEnd != -1)
            {
                //Prend la connexion string sans les accolades.
                string connexion = sessions.Substring(++indexStart, indexEnd - indexStart);
                //Enlève la partie traité de sessions et le remplace pour continuer s'il y a d'autres sessions.
                sessions = sessions.Substring(++indexEnd);

                listConnexionStrings.Add(connexion);
                //Récupère les nouvelles index des accolades.
                indexStart = sessions.IndexOf('{');
                indexEnd = sessions.IndexOf('}');
            }
            //Pour chaque connexion string MySql récupéré de sessions
            foreach (string item in listConnexionStrings)
            {
                try
                {
                    //Convertie la string en objet Session et le rajoute dans la liste.
                    listSessions.Add(GetSession(item));
                }
                //Dans le cas d'un format invalide.
                catch (ArgumentException)
                {
                    continue;
                }
            }
            return listSessions;
        }

        /// <summary>
        /// Méthode statique qui avec une liste d'objets sessions, retourne un string sessions qui peut être enregistrer dans la configuration du logiciel
        /// Le format de la string est essentiellement la même qu'une connexion string de MySql, mais qui supporte plusieurs connexion tous entouré par "{" et "}"
        /// </summary>
        /// <param name="listS">Liste de sessions devant être converties en format utilisable pour la configuration du logiciel</param>
        /// <returns>String de format sessions: {name=x;server=y;userid=z;password=;database=a}{[...]}</returns>
        public static string SessionsToString(List<Session> listS)
        {
            StringBuilder sb = new StringBuilder();

            foreach (Session s in listS)
            {
                //On ne fait que rajouter des accolades entre. Le ToString de la classe Session se charge du reste.
                sb.Append("{" + s + "}");
            }
            return sb.ToString();
        }
    }
}

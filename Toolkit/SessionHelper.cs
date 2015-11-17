using Nutritia.Logic.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nutritia.Toolkit
{
    public static class SessionHelper
    {

        private static Session GetSession(string connexion)
        {
            String[] words = connexion.Split(';');
            if (words.Length < 5 || words.Length > 6)
                throw new ArgumentException(connexion + " n'est pas d'un format valide");

            for (int i = 0; i < words.Length; i++)
            {
                if (!String.IsNullOrEmpty(words[i]))
                {
                    int index = words[i].IndexOf('=');
                    if (index != -1)
                    {
                        words[i] = words[i].Substring(index + 1);
                    }
                }
            }
            if (words.Length == 5)
            {
                return new Session(words[0], words[1], words[2], words[3], words[4]);
            }
            //Test limite couvre déjà < 5 et > 6, donc Length == 6 ici.
            else
                return new Session(words[0], words[1], words[3], words[4], words[5], int.Parse(words[2]));
        }

        public static List<Session> StringToSessions(string sessions)
        {
            List<string> listConnexionStrings = new List<string>();
            List<Session> listSessions = new List<Session>();
            int indexStart;
            int indexEnd;
            indexStart = sessions.IndexOf('{');
            indexEnd = sessions.IndexOf('}');
            while (indexStart == 0 && indexEnd != -1)
            {
                string connexion = sessions.Substring(++indexStart, indexEnd - indexStart);
                sessions = sessions.Substring(++indexEnd);
                listConnexionStrings.Add(connexion);
                indexStart = sessions.IndexOf('{');
                indexEnd = sessions.IndexOf('}');
            }
            foreach (string item in listConnexionStrings)
            {
                try
                {
                    listSessions.Add(GetSession(item));
                }
                catch (ArgumentException)
                {
                    continue;
                }
            }
            return listSessions;
        }


        public static string SessionsToString(List<Session> listS)
        {
            StringBuilder sb = new StringBuilder();

            foreach (Session s in listS)
            {
                sb.Append("{" + s + "}");
            }
            return sb.ToString();
        }
    }
}

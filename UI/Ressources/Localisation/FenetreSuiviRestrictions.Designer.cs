﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Nutritia.UI.Ressources.Localisation {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class FenetreSuiviRestrictions {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal FenetreSuiviRestrictions() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Nutritia.UI.Ressources.Localisation.FenetreSuiviRestrictions", typeof(FenetreSuiviRestrictions).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Vous n&apos;avez aucune restriction alimentaire et/ou objectifs..
        /// </summary>
        public static string AucuneRestriction {
            get {
                return ResourceManager.GetString("AucuneRestriction", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Enregistrer.
        /// </summary>
        public static string Enregistrer {
            get {
                return ResourceManager.GetString("Enregistrer", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cochez les plats que vous désirez rendre admissible à la génération..
        /// </summary>
        public static string Instructions {
            get {
                return ResourceManager.GetString("Instructions", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Plats non admis à la génération.
        /// </summary>
        public static string PlatsNonAdmis {
            get {
                return ResourceManager.GetString("PlatsNonAdmis", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Nutritia - Suivi des restrictions.
        /// </summary>
        public static string Titre {
            get {
                return ResourceManager.GetString("Titre", resourceCulture);
            }
        }
    }
}
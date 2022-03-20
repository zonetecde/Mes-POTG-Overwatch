using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Mes_POTG_Overwatch
{
    public class Utilities
    {
        public static string resourcesPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Zonedetec\Mes Temps Forts Overwatch";

        public Resource resource = new Resource();

        /// <summary>
        /// Initialize toutes les resources
        /// </summary>
        public void InitializeResourcesPath()
        {
            Directory.CreateDirectory(resourcesPath);
            resourcesPath += @"\resources.zdt";

            if (!File.Exists(resourcesPath))
            {
                File.Create(resourcesPath).Close();
            }
        }

        /// <summary>
        /// Charge les resources
        /// </summary>
        public void LoadResources()
        {
            string json = File.ReadAllText(resourcesPath);
            resource = JsonConvert.DeserializeObject<Resource>(json);

            if (resource == null)
            {
                resource = new Resource();
            }
            if (resource.TempsForts == null)
            {
                resource.TempsForts = new List<TempsFort>();
            }
        }

        /// <summary>
        /// Sauvegarde les resources
        /// </summary>
        public void SaveResources()
        {
            File.WriteAllText(resourcesPath, JsonConvert.SerializeObject(MainWindow.Utilities.resource, Formatting.Indented));
        }

        /// <summary>
        /// Premier lancement du logiciel ?
        /// </summary>
        /// <returns></returns>
        public bool IsFirstLaunch()
        {
            return true;

            if (Properties.Settings.Default.FirstLaunch)
            {
                Properties.Settings.Default.FirstLaunch = false;
                Properties.Settings.Default.Save();

                return true;
            }

            return false;
        }


    }
}
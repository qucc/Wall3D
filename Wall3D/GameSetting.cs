using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Wall3D
{
    public class GameSetting
    {
        //default values
        public static string Column = "40";
        public static string Scale = "1";
        public static string CameraZ = "12";
        public static string CameraY = "0";

        static GameSetting()
        {
            XDocument doc = null;
            try
            {
                doc = LoadDocument();
            }
            catch (Exception)
            {
                doc = new XDocument(new XElement("Setting"));
            }
            foreach (var f in typeof(GameSetting).GetFields())
            {
                var settingName = f.Name;
                var defaultSettingValue = f.GetValue(null);
                var settingElement = doc.Root.Element(settingName);
                if (settingElement == null)
                {
                    settingElement = new XElement(settingName, defaultSettingValue.ToString());
                    settingElement.Add(new XAttribute("DefaultValue", defaultSettingValue.ToString()));
                    doc.Root.Add(settingElement);
                }
                else
                {
                    //here override default value 
                    f.SetValue(null, settingElement.Value);
                }
            }
            SaveDocument(doc);
        }

        private static XDocument LoadDocument()
        {
            var appRoot = AppDomain.CurrentDomain.BaseDirectory;
            var settingPath = appRoot + "/wall3dsetting.xml";
             return XDocument.Load(settingPath);
        }

        private static void SaveDocument(XDocument doc)
        {
            doc.Save("wall3dsetting.xml");
        }

        public static void Save()
        {
            var doc = LoadDocument();
            //write xml document
            foreach (var f in typeof(GameSetting).GetFields())
            {
                var fieldName = f.Name;
                var value = f.GetValue(null);
                doc.Root.Element(fieldName).Value = value.ToString();
            }
            SaveDocument(doc);
        }

        public static void Reset()
        {
            var doc = LoadDocument();
            foreach (var f in typeof(GameSetting).GetFields())
            {
                var fieldName = f.Name;
                var defaultValue = doc.Root.Element(fieldName).Attribute("DefaultValue").Value;
                f.SetValue(null, defaultValue);
            }
            Save();
        }
    }
}

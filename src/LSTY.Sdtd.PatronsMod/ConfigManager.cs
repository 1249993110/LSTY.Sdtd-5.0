using IceCoffee.Common.Xml;
using LSTY.Sdtd.PatronsMod.ExceptionCatch;
using LSTY.Sdtd.PatronsMod.Primitives;
using System;
using System.IO;
using System.Threading;
using System.Xml;

namespace LSTY.Sdtd.PatronsMod
{
    static class ConfigManager
    {
        private const string _functionConfigPath = "LSTY/functionConfig.xml";

        private const string _baseNodeName = "FunctionConfig";

        private static FileSystemWatcher _fileWatcher;

        static ConfigManager()
        {
            CreateFileWatcher();
        }

        private static void CreateFileWatcher()
        {
            try
            {
                if(_fileWatcher != null)
                {
                    _fileWatcher.Changed -= OnConfigFileChanged;
                    _fileWatcher.Dispose();
                }

                _fileWatcher = new FileSystemWatcher("LSTY", "functionConfig.xml");

                _fileWatcher.NotifyFilter = NotifyFilters.LastWrite;

                _fileWatcher.Changed += OnConfigFileChanged;

                _fileWatcher.EnableRaisingEvents = true;
            }
            catch (Exception ex)
            {
                CustomLogger.Error(ex, "Failed to create config file watcher");
            }
        }

        private static void OnConfigFileChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
            {
                return;
            }

            LoadAll();

            CustomLogger.Info("Reload configuration file");
        }

        public static void Load<T>(T obj) where T : IFunction
        {
            if (System.IO.File.Exists(_functionConfigPath) == false)
            {
                return;
            }

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(_functionConfigPath);

                // Take the class name inherited from FunctionBase as the parent node.
                XmlNode baseNode = doc.SelectSingleNode(_baseNodeName + "/" + obj.FunctionName);

                XmlHelper.LoadConfig(obj, baseNode);
            }
            catch (Exception ex)
            {
                CustomLogger.Error(ex, "Failed to load configuration of function: " + obj.FunctionName);
            }
        }
        [CatchException("Failed to load configuration")]
        public static void LoadAll()
        {
            if (File.Exists(_functionConfigPath) == false)
            {
                SaveAll();
                return;
            }

            var functions = FunctionManager.Functions;

            XmlDocument doc = new XmlDocument();
            doc.Load(_functionConfigPath);

            XmlNode baseNode = doc.SelectSingleNode(_baseNodeName);

            string functionName;
            XmlNode currentNode;

            foreach (var function in functions)
            {
                functionName = function.FunctionName;
                try
                {
                    // Take the class name inherited from FunctionBase as the parent node.
                    currentNode = baseNode.SelectSingleNode(functionName);

                    XmlHelper.LoadConfig(function, currentNode);
                }
                catch (Exception ex)
                {
                    CustomLogger.Error(ex, "Failed to load configuration of function: " + functionName);
                }
            }
        }

        private static XmlDocument GetDocument()
        {
            XmlDocument doc = new XmlDocument();

            if (System.IO.File.Exists(_functionConfigPath) == false)
            {
                XmlDeclaration dec = doc.CreateXmlDeclaration("1.0", "utf-8", null);
                doc.AppendChild(dec);
                doc.AppendChild(doc.CreateElement(_baseNodeName));
            }
            else
            {
                doc.Load(_functionConfigPath);
            }

            return doc;
        }

        public static void Save<T>(T obj) where T : IFunction
        {
            try
            {
                _fileWatcher.EnableRaisingEvents = false;
                _fileWatcher = null;

                XmlDocument contextDoc = GetDocument();

                XmlNode baseNode = contextDoc.SelectSingleNode(_baseNodeName);

                // Take the class name inherited from FunctionBase as the parent node.
                baseNode = baseNode.GetSingleChildNode(contextDoc, obj.FunctionName);

                XmlHelper.SaveConfig(obj, contextDoc, baseNode);

                contextDoc.Save(_functionConfigPath);
            }
            catch (Exception ex)
            {
                CustomLogger.Error(ex, "Failed to save configuration of function: " + obj.FunctionName);
            }
            finally
            {
                CreateFileWatcher();
            }
        }

        public static void DisableConfigFileWatcher()
        {
            _fileWatcher.EnableRaisingEvents = false;
        }

        public static void SaveAll()
        {
            try
            {
                _fileWatcher.EnableRaisingEvents = false;

                var functions = FunctionManager.Functions;

                XmlDocument doc = GetDocument();

                XmlNode baseNode = doc.SelectSingleNode(_baseNodeName);

                string functionName;
                XmlNode currentNode;

                foreach (var function in functions)
                {
                    functionName = function.FunctionName;

                    try
                    {
                        // Take the class name inherited from FunctionBase as the parent node.
                        currentNode = baseNode.GetSingleChildNode(doc, functionName);

                        XmlHelper.SaveConfig(function, doc, currentNode);
                    }
                    catch (Exception ex)
                    {
                        CustomLogger.Error(ex, "Failed to save configuration of function: " + functionName);
                    }
                }

                doc.Save(_functionConfigPath);

                CustomLogger.Info("SaveAll");
            }
            catch (Exception ex)
            {
                CustomLogger.Error(ex, "Failed to save configuration");
            }
            finally
            {
                CreateFileWatcher();
            }
        }
    }
}

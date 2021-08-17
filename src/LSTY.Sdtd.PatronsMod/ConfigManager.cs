using IceCoffee.Common.Xml;
using LSTY.Sdtd.PatronsMod.ExceptionCatch;
using LSTY.Sdtd.PatronsMod.Internal;
using LSTY.Sdtd.PatronsMod.Primitives;
using System;
using System.IO;
using System.Threading;
using System.Xml;

namespace LSTY.Sdtd.PatronsMod
{
    public static class ConfigManager
    {
        private static readonly object _lockObj = new object();

        private const string _functionConfigPath = "LSTY/functionConfig.xml";

        private const string _baseNodeName = "FunctionConfig";

        private static FileSystemWatcher _fileWatcher;

        private static void CreateFileWatcher()
        {
            try
            {
                DisposeFileWatcher();

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
            if (File.Exists(_functionConfigPath) == false)
            {
                return;
            }

            try
            {
                XmlDocument doc = new XmlDocument();

                using (FileStream fs = new FileStream(_functionConfigPath, FileMode.Open, FileAccess.Read))
                {
                    doc.Load(fs);
                }

                // Take the class name inherited from FunctionBase as the parent node.
                XmlNode baseNode = doc.SelectSingleNode(_baseNodeName + "/" + obj.FunctionName);

                XmlHelper.LoadConfig(obj, baseNode);

                doc.RemoveAll();
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
            else
            {
                CreateFileWatcher();
            }

            var functions = FunctionManager.Functions;

            XmlDocument doc = new XmlDocument();
            using (FileStream fs = new FileStream(_functionConfigPath, FileMode.Open, FileAccess.Read))
            {
                doc.Load(fs);
            }

            XmlNode baseNode = doc.SelectSingleNode(_baseNodeName);

            string functionName;
            XmlNode currentNode;

            foreach (var function in functions)
            {
                if (function is ISubFunction)
                {
                    continue;
                }

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

            doc.RemoveAll();
        }

        private static XmlDocument GetDocument()
        {
            XmlDocument doc = new XmlDocument();

            if (File.Exists(_functionConfigPath) == false)
            {
                XmlDeclaration dec = doc.CreateXmlDeclaration("1.0", "utf-8", null);
                doc.AppendChild(dec);
                doc.AppendChild(doc.CreateElement(_baseNodeName));
            }
            else
            {
                using (FileStream fs = new FileStream(_functionConfigPath, FileMode.Open, FileAccess.Read))
                {
                    doc.Load(fs);
                }
            }

            return doc;
        }

        public static void Save<T>(T obj) where T : IFunction
        {
            if(obj is ISubFunction)
            {
                return;
            }

            lock (_lockObj)
            {
                try
                {
                    DisposeFileWatcher();

                    XmlDocument contextDoc = GetDocument();

                    XmlNode baseNode = contextDoc.SelectSingleNode(_baseNodeName);

                    // Take the class name inherited from FunctionBase as the parent node.
                    baseNode = baseNode.GetSingleChildNode(contextDoc, obj.FunctionName);

                    XmlHelper.SaveConfig(obj, contextDoc, baseNode);

                    using (FileStream fs = new FileStream(_functionConfigPath, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        contextDoc.Save(fs);
                    }

                    contextDoc.RemoveAll();
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
        }

        public static void DisposeFileWatcher()
        {
            if(_fileWatcher != null)
            {
                _fileWatcher.EnableRaisingEvents = false;
                _fileWatcher.Changed -= OnConfigFileChanged;
                _fileWatcher.Dispose();
                _fileWatcher = null;
            }
        }

        public static void SaveAll()
        {
            lock (_lockObj)
            {
                try
                {
                    DisposeFileWatcher();

                    var functions = FunctionManager.Functions;

                    XmlDocument doc = GetDocument();

                    XmlNode baseNode = doc.SelectSingleNode(_baseNodeName);

                    string functionName;
                    XmlNode currentNode;

                    foreach (var function in functions)
                    {
                        if (function is ISubFunction)
                        {
                            continue;
                        }

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

                    using (FileStream fs = new FileStream(_functionConfigPath, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        doc.Save(fs);
                    }

                    doc.RemoveAll();
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
}

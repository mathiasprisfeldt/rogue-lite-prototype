using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace AcrylecSkeleton.MVC
{
    /// <summary>
    /// Class used for creating MVC application assets.
    /// </summary>
    public class MVCMaker : EditorWindow
    {
        private static readonly Vector2 WindowMaxSize = new Vector2(250, 50);
        private static readonly Vector2 WindowMinSize = WindowMaxSize;

        //Template fields
        private string _applicationTemplate;
        private string _modelTemplate;
        private string _viewTemplate;
        private string _controllerTemplate;

        private string _namespace;
        private string _rootPath;
        private string _name; //Name of newly created MVC object.
        private GameObject _newPrefab; //GameObject of newly created MVC prefab.

        [MenuItem("Assets/Create/MVC Object")]
        public static void ShowWindow()
        {
            //Create the new window and set min max size to a constant.
            var newWindow = GetWindow(typeof(MVCMaker), true, "Choose a name for your new MVC Object", true);
            newWindow.maxSize = WindowMaxSize;
            newWindow.minSize = WindowMinSize;
        }

        /// <summary>
        /// Initializes needed data for MVCMaker.
        /// </summary>
        void Awake()
        {
            //Grab the directory path to the MVCMaker.
            string systemRootPathId = AssetDatabase.FindAssets("MVCApplication").FirstOrDefault();
            _rootPath = AssetDatabase.GUIDToAssetPath(systemRootPathId);
            _rootPath = _rootPath.Substring(0, _rootPath.LastIndexOf("/MVCApplication.cs", StringComparison.Ordinal));

            //Read all the template files from template file folder.
            _applicationTemplate = File.ReadAllText(_rootPath + "/Templates/MVCApplicationTemplate.cs.txt");
            _modelTemplate = File.ReadAllText(_rootPath + "/Templates/MVCModelTemplate.cs.txt");
            _viewTemplate = File.ReadAllText(_rootPath + "/Templates/MVCViewTemplate.cs.txt");
            _controllerTemplate = File.ReadAllText(_rootPath + "/Templates/MVCControllerTemplate.cs.txt");
        }

        /// <summary>
        /// Draws the MVC Wizard
        /// </summary>
        void OnGUI()
        {
            Event currEvent = Event.current;

            GUILayout.Space(5);

            //Name field
            GUI.SetNextControlName("NameField");
            _name = EditorGUILayout.TextField("Name:", _name);

            GUI.FocusControl("NameField");
            EditorGUI.FocusTextInControl("NameField");
            //

            GUILayout.Space(5);

            //What to do text
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(!_newPrefab ? "Press 'ENTER' to proceed." : "Please Wait...");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            //

            //Listen for input on 'RETURN' key, and create new MVC object if pressed.
            if (currEvent.keyCode == KeyCode.Return)
                Create();
        }

        /// <summary>
        /// Takes a string containing template code for a new MVC class, 
        /// and replaces keywords with actual data.
        /// </summary>
        /// <param name="template">Template code string.</param>
        /// <param name="namespaceString">Namespace string.</param>
        /// <returns>Return modified variant of template string with actual data.</returns>
        private string SetupTemplate(string template, string namespaceString)
        {
            return template
                .Replace("#NAME#", _name)
                .Replace("#NAMESPACE#", namespaceString)
                .Replace("#CREATOR#", Environment.UserName)
                .Replace("#DATE#", DateTime.Now.ToLongDateString());
        }

        /// <summary>
        /// Method used to create the new MVC object.
        /// </summary>
        private void Create()
        {
            _name = _name.Trim();
            _name = _name.Replace(" ", ".");

            string targetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            string targetPathExtended = targetPath + @"/" + _name;

            //Create new MVC object folder
            var rootDirFull = Directory.CreateDirectory(targetPathExtended).FullName;

            //Replacing hashtag strings in templates
            _namespace = targetPath.Replace("/", ".").Replace(" ", "") + "." + _name;

            var applicationTemplate = SetupTemplate(_applicationTemplate, _namespace);
            var applicationModel = SetupTemplate(_modelTemplate, _namespace);
            var applicationView = SetupTemplate(_viewTemplate, _namespace);
            var applicationController = SetupTemplate(_controllerTemplate, _namespace);

            //Create new MVC application
            string applicationPath = rootDirFull + "/" + _name + "Application.cs";
            File.Create(applicationPath).Close();
            File.WriteAllText(applicationPath, applicationTemplate);

            //Create new MVC model
            string modelPath = rootDirFull + "/" + _name + "Model.cs";
            File.Create(modelPath).Close();
            File.WriteAllText(modelPath, applicationModel);

            //Create new MVC view
            string viewPath = rootDirFull + "/" + _name + "View.cs";
            File.Create(viewPath).Close();
            File.WriteAllText(viewPath, applicationView);

            //Create new MVC controller
            string controllerPath = rootDirFull + "/" + _name + "Controller.cs";
            File.Create(controllerPath).Close();
            File.WriteAllText(controllerPath, applicationController);

            //Create new MVC prefab
            var newPrefabObject = PrefabUtility.CreateEmptyPrefab(targetPath + "/" + _name + "/" + _name + ".prefab");
            var prefab = (GameObject)AssetDatabase.LoadAssetAtPath(_rootPath + "/[NAME]Application.prefab", typeof(GameObject));
            _newPrefab = PrefabUtility.ReplacePrefab(prefab, newPrefabObject, ReplacePrefabOptions.ConnectToPrefab);

            AssetDatabase.Refresh();
        }

        /// <summary>
        /// If a newly MVC object has been creates this wait for the new MVC classes
        /// to compile so it can create prefab from them.
        /// </summary>
        private void OnInspectorUpdate()
        {
            if (_newPrefab)
            {
                Type applicationType = GetType(_namespace + "." + _name + "Application");
                Type modelType = GetType(_namespace + "." + _name + "Model");
                Type viewType = GetType(_namespace + "." + _name + "View");
                Type controllerType = GetType(_namespace + "." + _name + "Controller");

                if (applicationType != null && modelType != null && viewType != null && controllerType != null)
                {
                    //Add components to prefab
                    var applicationComp = _newPrefab.AddComponent(applicationType);
                    var modelComp = _newPrefab.gameObject.AddComponent(modelType);
                    var viewComp = _newPrefab.transform.Find("View").gameObject.AddComponent(viewType);
                    var controllerComp = _newPrefab.transform.Find("Controller").gameObject.AddComponent(controllerType);

                    const BindingFlags searchFlags = BindingFlags.NonPublic | BindingFlags.Instance;

                    //Setup application comp references
                    applicationType.BaseType.GetField("_model", searchFlags).SetValue(applicationComp, modelComp);
                    applicationType.BaseType.GetField("_view", searchFlags).SetValue(applicationComp, viewComp);
                    applicationType.BaseType.GetField("_controller", searchFlags).SetValue(applicationComp, controllerComp);

                    //Setup model comp references
                    modelType.BaseType.GetProperty("App", searchFlags).SetValue(modelComp, applicationComp, null);

                    //Setup view comp references
                    viewType.BaseType.GetProperty("App", searchFlags).SetValue(viewComp, applicationComp, null);

                    //Setup controller comp references
                    controllerType.BaseType.GetProperty("App", searchFlags).SetValue(controllerComp, applicationComp, null);

                    GetWindow(typeof(MVCMaker)).Close();
                    AssetDatabase.Refresh();
                }
            }
        }

        /// <summary>
        /// Searches assemblies for a specific type given the name as parameter.
        /// </summary>
        /// <param name="typeName">Name of type Ex. (Namespace.a.b.Class)</param>
        /// <returns>Return found type, returns null if none found.</returns>
        private static Type GetType(string typeName)
        {
            var type = Type.GetType(typeName);
            if (type != null) return type;
            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = a.GetType(typeName);
                if (type != null)
                    return type;
            }
            return null;
        }
    }
}
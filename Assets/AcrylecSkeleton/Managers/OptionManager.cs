using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using AcrylecSkeleton.Utilities;
using UnityEngine;
using IniParser;
using IniParser.Model;
using UnityEngine.UI;

namespace AcrylecSkeleton.Managers
{
    public class OptionManager : Singleton<OptionManager>
    {
        [SerializeField]
        private List<OptionData> _optionData = new List<OptionData>();

        public static string Filepath;
        public static string Filename = @"/PlayerPref.ini";

        private readonly FileIniDataParser _parser = new FileIniDataParser();
        private IniData _data;

        public static string GetFullPath()
        {
            return Filepath + Filename;
        }

        protected override void Awake()
        {
            Debug.Log("ASD");
            Filepath = Application.dataPath + @"/Resources/PlayerData";
            Load();

            foreach (OptionData optionData in _optionData)
            {
                if (!optionData.Target)
                    Debug.LogError(String.Format("PLAYERPREF: {0} option is missing target selectable!", optionData.Name));
            }
        }

        public void Save()
        {
            CheckFileSanity();

            foreach (OptionData optionData in _optionData)
            {
                _data["Options"][optionData.Name] = optionData.Export();
            }

            _parser.WriteFile(GetFullPath(), _data);
        }

        public void Load()
        {
            CheckFileSanity();

            //Try to read ini file, if it fails create/clean a new one.
            try
            {
                _data = _parser.ReadFile(GetFullPath());

                if (_data.ToString() == string.Empty)
                    throw new Exception("INI file isn't initialized.");

                foreach (OptionData optionData in _optionData)
                {
                    optionData.Import(_data["Options"][optionData.Name]);
                }
            }
            catch (Exception)
            {
                Debug.LogWarning("INI FILE is corrupt/invalid/non-existant, creating a new one.");

                if (File.Exists(GetFullPath()))
                    File.WriteAllText(GetFullPath(), string.Empty);

                _data = _parser.ReadFile(GetFullPath());

                Save();

                foreach (OptionData optionData in _optionData)
                {
                    optionData.Import(_data["Options"][optionData.Name]);
                }
            }
        }

        private void CheckFileSanity()
        {
            if (!Directory.Exists(Filepath))
                Directory.CreateDirectory(Filepath);

            if (!File.Exists(GetFullPath()))
                File.Create(GetFullPath()).Dispose();
        }

        private void OnApplicationQuit()
        {
            Save();
        }

        public void Flush()
        {
            if (File.Exists(GetFullPath()))
                File.Delete(GetFullPath());

            foreach (var data in _optionData)
            {
                var selectableResetter = data.Target.GetComponent<SelectableResetter>();
                if (selectableResetter)
                    selectableResetter.SetToDefault();
            }

            Load();
        }
    }
}

[Serializable]
public struct OptionData
{
    public string Name;
    public Selectable Target;

    public string Export()
    {
        //Export for sliders
        var slider = Target as Slider;
        if (slider != null)
        {
            return slider.value.ToString(CultureInfo.InvariantCulture);
        }

        //Export for Toggles
        var toggle = Target as Toggle;
        if (toggle != null)
        {
            return toggle.isOn.ToString();
        }

        //Export for Dropdown
        var dropdown = Target as Dropdown;
        if (dropdown != null)
        {
            return dropdown.value.ToString(CultureInfo.InvariantCulture);
        }

        return string.Empty;
    }

    public void Import(string data)
    {
        if (data == null)
            return;

        //Import for sliders
        var slider = Target as Slider;
        if (slider != null)
        {
            slider.value = Convert.ToSingle(data);
            slider.onValueChanged.Invoke(slider.value);
        }

        //Import for toggles
        var toggle = Target as Toggle;
        if (toggle != null)
        {
            toggle.isOn = Convert.ToBoolean(data);
            toggle.onValueChanged.Invoke(toggle.isOn);
        }

        //Import for dropdown
        var dropdown = Target as Dropdown;
        if (dropdown != null)
        {
            dropdown.value = Convert.ToInt32(data);
            dropdown.onValueChanged.Invoke(dropdown.value);
        }
    }
}

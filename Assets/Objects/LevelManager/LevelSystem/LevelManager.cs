using AcrylecSkeleton.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Manages and loads levels
/// </summary>
[RequireComponent(typeof(LevelDataManager))]
public class LevelManager : Singleton<LevelManager>
{

    //Forced levels are loaded in succession, then a random level is loaded randomly
    [SerializeField]
    private List<TextAsset> _forcedLevelsText;
    [SerializeField]
    private List<TextAsset> _randomLevelsText;

    private List<Level> _forcedLevels = new List<Level>();
    private List<Level> _randomLevels = new List<Level>();

    //Current level
    public Level CurrentLevel { get; set; }

    protected override void Awake()
    {
        base.Awake();

        LoadLevels();
        LoadNextLevel();
    }

    /// <summary>
    /// Loads the next level
    /// </summary>
    public void LoadNextLevel()
    {
        //There is already a level loaded, and we are ingame
        if (CurrentLevel != null)
        {

        }
        else
        {
            if (_forcedLevels.Any())
            {
                CurrentLevel = _forcedLevels.FirstOrDefault();
            }
            else if (_randomLevels.Any())
            {
                CurrentLevel = _randomLevels[UnityEngine.Random.Range(0, _randomLevels.Count)];
            }
            else
            {
                Debug.Log("There are no levels to loaded");
            }
        }

        CurrentLevel.Spawn(transform);
    }

    /// <summary>
    /// levels form given level files
    /// </summary>
    public void LoadLevels()
    {
        foreach (var item in _forcedLevelsText)
        {
            _forcedLevels.Add(new Level(CSVReader.SplitCsvGridToInt(item.text, false), Convert.ToInt16(item.name)));
        }

        foreach (var item in _randomLevelsText)
        {
            _randomLevels.Add(new Level(CSVReader.SplitCsvGridToInt(item.text, false), Convert.ToInt16(item.name)));
        }
    }
}
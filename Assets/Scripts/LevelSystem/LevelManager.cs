using AcrylecSkeleton.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    [SerializeField]
    private List<TextAsset> _randomLevelsText;
    [SerializeField]
    private List<TextAsset> _forcedLevelsText;

    private List<Level> _forcedLevels = new List<Level>();
    private List<Level> _randomLevels = new List<Level>();

    public Level CurrentLevel { get; set; }

    protected override void Awake()
    {
        base.Awake();

        LoadLevels();
        LoadNextLevel();
    }

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

    public void LoadLevels()
    {
        foreach (var item in _forcedLevelsText)
        {
            _forcedLevels.Add(new Level(CSVReader.SplitCsvGridToInt(item.text), Convert.ToInt16(item.name)));
        }

        foreach (var item in _randomLevelsText)
        {
            _randomLevels.Add(new Level(CSVReader.SplitCsvGridToInt(item.text), Convert.ToInt16(item.name)));
        }
    }
}
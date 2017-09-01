using AcrylecSkeleton.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Objects.PlayerMovement.Player.Prefab.Player;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

/// <summary>
/// Manages and loads levels
/// </summary>
[RequireComponent(typeof(LevelDataManager))]
public class LevelManager : Singleton<LevelManager>
{
    [SerializeField]
    private Vector2 _tileSpawnOffset = Vector2.one / 2;

    [SerializeField]
    private GameObject _borderTile;
    public GameObject BorderTile { get { return _borderTile; } set { _borderTile = value; } }

    [SerializeField]
    private GameObject _backGround;
    public GameObject BackGround { get { return _backGround; } set { _backGround = value; } }

    //Forced levels are loaded in succession, then a random level is loaded randomly
    [SerializeField]
    private List<TextAsset> _forcedLevelsText;
    [SerializeField]
    private List<TextAsset> _randomLevelsText;

    private List<int[,]> _forcedLevels = new List<int[,]>();
    private List<int[,]> _randomLevels = new List<int[,]>();

    //Current level
    public Level CurrentLevel { get; set; }

    protected override void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        base.Awake();
        LoadLevels();

        if (SceneManager.GetActiveScene().name != "LevelScene")
        {
            if (_forcedLevels.Any())
            {
                var levelArray = _forcedLevels.FirstOrDefault();
                CurrentLevel = new Level(levelArray);
                _forcedLevels.Remove(levelArray);
            }
            else if (_randomLevels.Any())
            {
                CurrentLevel = new Level(_randomLevels[UnityEngine.Random.Range(0, _randomLevels.Count)]);
            }
            else
            {
                Debug.Log("There are no levels to loaded");
                return;
            }

            GameObject go = new GameObject("LevelParent");
            go.AddComponent<Platforms>();
            CurrentLevel.Spawn(go.transform);
        }
        else
            LoadNextLevel();
    }

    /// <summary>
    /// Loads the next level
    /// </summary>
    public void LoadNextLevel()
    {
        //There is already a level loaded, and we are ingame
        if (CurrentLevel != null)
            CurrentLevel.Despawn();

        SceneManager.LoadScene("LevelScene");
    }

    private void OnSceneLoaded(Scene level, LoadSceneMode sceneMode)
    {
        if (level.name != "LevelScene")
            return;

        if (_forcedLevels.Any())
        {
            var levelArray = _forcedLevels.FirstOrDefault();
            CurrentLevel = new Level(levelArray);
            _forcedLevels.Remove(levelArray);
        }
        else if (_randomLevels.Any())
        {
            CurrentLevel = new Level(_randomLevels[UnityEngine.Random.Range(0, _randomLevels.Count)]);
        }
        else
            Debug.Log("There are no levels to loaded");

        GameObject go = new GameObject("LevelParent");
        go.AddComponent<Platforms>();
        CurrentLevel.Spawn(go.transform);
    }

    public void SpawnBackGround(Vector2 v)
    {
        Vector2 size = (v * 2);

        var bg = Instantiate(BackGround, new Vector2(v.x - .5f, v.y + .5f) + _tileSpawnOffset, Quaternion.identity);
        bg.GetComponent<SpriteRenderer>().size = size;
    }

    /// <summary>
    /// Used for spawning a tile in a level.
    /// It accounts for positional offset described in LevelManager.
    /// </summary>
    /// <returns>GameObject of the instantiated tile.</returns>
    public GameObject SpawnTile(Vector2 pos, GameObject tileRecipe = null, Quaternion rot = default(Quaternion), Transform parent = null)
    {
        return Instantiate(tileRecipe ?? BorderTile, pos + _tileSpawnOffset, rot, parent);
    }

    public void ResetGame()
    {
        _forcedLevels.Clear();
        _randomLevels.Clear();

        LoadLevels();
        LoadNextLevel();
    }

    /// <summary>
    /// levels form given level files
    /// </summary>
    public void LoadLevels()
    {
        if (_forcedLevels.Any())
        {
            foreach (var item in _forcedLevelsText)
            {
                _forcedLevels.Add(CSVReader.SplitCsvGridToInt(item.text, false));
            }
        }

        if (_randomLevels.Any())
        {
            foreach (var item in _randomLevelsText)
            {
                _randomLevels.Add(CSVReader.SplitCsvGridToInt(item.text, false));
            }
        }
    }
}
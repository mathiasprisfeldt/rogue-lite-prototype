using AcrylecSkeleton.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Objects.PlayerMovement.Player.Prefab.Player;
using Managers;
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
    private bool _resetMapOnDeath;

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
    private List<TextAsset> _randomLevelsEasyText;
    [SerializeField]
    private List<TextAsset> _randomLevelsMediumText;
    [SerializeField]
    private List<TextAsset> _randomLevelsHardText;

    private List<int[,]> _forcedLevels = new List<int[,]>();
    private List<int[,]> _randomLevelsEasy = new List<int[,]>();
    private List<int[,]> _randomLevelsMedium = new List<int[,]>();
    private List<int[,]> _randomLevelsHard = new List<int[,]>();

    private bool _nextLevelLoaded = false;
    private bool _setup;
    private bool _loadNextLevel;

    public Level CurrentLevel { get; set; }
    public float SavedPlayerHealth { get; set; }


    /// <summary>
    /// Awake
    /// </summary>
    protected override void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        base.Awake();
        LoadLevels();

        if (SceneManager.GetActiveScene().name != "LevelScene")
        {
            FindNextLevel();

            GameObject go = new GameObject("LevelParent");
            go.AddComponent<Platforms>();

            if (CurrentLevel != null)
                CurrentLevel.Spawn(go.transform);
        }
        else
            LoadNextLevel();
        _setup = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            ResetGame(true);
    }

    /// <summary>
    /// Loads the next level
    /// </summary>
    public void LoadNextLevel()
    {
        if (_nextLevelLoaded)
            return;

        //There is already a level loaded, and we are ingame
        if (CurrentLevel != null)
            CurrentLevel.Despawn();

        _nextLevelLoaded = true;
        _loadNextLevel = true;

        SceneManager.LoadScene("LevelScene");
    }

    /// <summary>
    /// Finds the next level to spawn in all the lists
    /// </summary>
    private void FindNextLevel()
    {
        if (_resetMapOnDeath && _setup && !_loadNextLevel)
        {
            CurrentLevel = new Level(CurrentLevel.LayoutsData);
            return;
        }

        _loadNextLevel = false;

        if (_forcedLevels.Any())
        {
            var levelArray = _forcedLevels.FirstOrDefault();
            CurrentLevel = new Level(levelArray);
            _forcedLevels.Remove(levelArray);
        }
        else if (_randomLevelsEasy.Any())
        {
            var levelArray = _randomLevelsEasy[UnityEngine.Random.Range(0, _randomLevelsEasy.Count)];
            CurrentLevel = new Level(levelArray);
            _randomLevelsEasy.Remove(levelArray);
        }
        else if (_randomLevelsMedium.Any())
        {
            var levelArray = _randomLevelsMedium[UnityEngine.Random.Range(0, _randomLevelsMedium.Count)];
            CurrentLevel = new Level(levelArray);
            _randomLevelsMedium.Remove(levelArray);
        }
        else if (_randomLevelsHard.Any())
        {
            if (_randomLevelsHard.Count > 1)
            {

                int prevId = CurrentLevel.Layouts[0, 0].ID;
                while (prevId == CurrentLevel.Layouts[0, 0].ID)
                    CurrentLevel = new Level(_randomLevelsHard[UnityEngine.Random.Range(0, _randomLevelsHard.Count)]);
            }
            else
                CurrentLevel = new Level(_randomLevelsHard[UnityEngine.Random.Range(0, _randomLevelsHard.Count)]);
        }
        else
            Debug.Log("There are no levels to load");
    }

    /// <summary>
    /// Called when the scene has been changed
    /// </summary>
    /// <param name="level"></param>
    /// <param name="sceneMode"></param>
    private void OnSceneLoaded(Scene level, LoadSceneMode sceneMode)
    {
        if (level.name != "LevelScene")
            return;

        FindNextLevel();

        GameObject go = new GameObject("LevelParent");
        go.AddComponent<Platforms>();
        CurrentLevel.Spawn(go.transform);

        _nextLevelLoaded = false;
    }

    /// <summary>
    /// Spawns the background
    /// </summary>
    /// <param name="v"></param>
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
        ResetGame(false);
    }

    public void ResetGame(bool overloadMapReset)
    {
        if (!_resetMapOnDeath || overloadMapReset)
        {
            if (GameManager.Instance)
                Destroy(GameManager.Instance.Player.gameObject);
            _forcedLevels.Clear();
            _randomLevelsHard.Clear();

            if (overloadMapReset)
                _setup = true;

            LoadLevels();
        }
        else
            GameManager.Instance.Player.C.Health.HealthAmount = SavedPlayerHealth;
        
        LoadNextLevel();
    }

    /// <summary>
    /// levels form given level files
    /// </summary>
    public void LoadLevels()
    {
        if (_forcedLevelsText != null)
            foreach (var item in _forcedLevelsText)
            {
                if (item != null)
                    _forcedLevels.Add(CSVReader.SplitCsvGridToInt(item.text, false));
            }

        if (_randomLevelsEasyText != null)
            foreach (var item in _randomLevelsEasyText)
            {
                if (item != null)
                    _randomLevelsEasy.Add(CSVReader.SplitCsvGridToInt(item.text, false));
            }

        if (_randomLevelsMediumText != null)
            foreach (var item in _randomLevelsMediumText)
            {
                if (item != null)
                    _randomLevelsMedium.Add(CSVReader.SplitCsvGridToInt(item.text, false));
            }

        if (_randomLevelsHardText != null)
            foreach (var item in _randomLevelsHardText)
            {
                if (item != null)
                    _randomLevelsHard.Add(CSVReader.SplitCsvGridToInt(item.text, false));
            }
    }

    /// <summary>
    /// Called on destroy
    /// </summary>
    public void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
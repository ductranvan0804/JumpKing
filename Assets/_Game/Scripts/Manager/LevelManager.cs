using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    [SerializeField] private Level[] levels;

    private Dictionary<int, GameObject> activeLevels = new Dictionary<int, GameObject>();
    private int currentLevel = 0;

    private void Start()
    {
        OnLoadLevel(0); 
        OnInit();
    }

    public void OnInit()
    {
        
    }

    public void OnDespawn()
    {
        foreach (var kvp in activeLevels)   //kvp stand for key-value pair use for the dictionary
        {
            Destroy(kvp.Value);
        }
        activeLevels.Clear();
    }

    public void OnLoadLevel(int levelIndex)
    {
        if (activeLevels.ContainsKey(levelIndex)) return;
        if (levelIndex < 0 || levelIndex >= levels.Length) return;

        Level levelData = levels[levelIndex];
        Vector3 spawnPos = new Vector3(0, levelData.yOffset, 0);
        GameObject instance = Instantiate(levelData.prefab, spawnPos, Quaternion.identity);
        activeLevels.Add(levelIndex, instance);
    }

    public void OnUnloadLevel(int levelIndex)
    {
        if (activeLevels.ContainsKey(levelIndex))
        {
            Destroy(activeLevels[levelIndex]);
            activeLevels.Remove(levelIndex);
        }
    }

    public void OnNextLevel()
    {
        OnDespawn();
        currentLevel++;
        OnLoadLevel(currentLevel);
        OnInit();
    }

    public int GetCurrentLevel() => currentLevel;

}

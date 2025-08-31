using UnityEngine;
using System.IO;


public static class SaveManager
{
    public static string path => Application.persistentDataPath + "/save.json";


    public static void SaveGame(Player player, float elapsedTime, Vector2 checkpoint, int currentLevel)
    {
        GameData data = new GameData
        {
            playerPos = new float[] { player.transform.position.x, player.transform.position.y },
            checkpointPos = new float[] { checkpoint.x, checkpoint.y },
            currentLevel = currentLevel,
            jumpCount = player.GetJumpCount(),
            fallCount = player.GetFallCount(),
            attemptCount = player.GetAttemptCount(),
            hp = player.GetHp(),
            elapsedTime = elapsedTime
        };

        File.WriteAllText(path, JsonUtility.ToJson(data));
        Debug.Log("Game Saved to: " + path);
    }

    public static GameData LoadGame()
    {
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<GameData>(json);
        }

        Debug.LogWarning("No save file found!");
        return null;
    }

    public static bool HasSave() => File.Exists(path);

}

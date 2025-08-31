using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIMainMenu : UICanvas
{
    [SerializeField] RectTransform[] menuItems;
    [SerializeField] RectTransform cursor;
    private int currentIndex = 0;
    public float indentOffsetX = 30f;
    [SerializeField] Vector2[] originalPositions;
    [SerializeField] GameObject[] objectsToActivateOnStart;
    [SerializeField] GameObject mainMenuCanvas;
    [SerializeField] private RecordDisplay recordDisplay;


    public override void Open()
    {
        base.Open();
        Time.timeScale = 0f;
        if (GameTimer.Ins != null)
        {
            GameTimer.Ins.PauseTimer();
        }
        if (recordDisplay != null)
        {
            recordDisplay.UpdateDisplay();
        }
    }

    public override void Setup()
    {
        base.Setup();
        originalPositions = new Vector2[menuItems.Length];
        for (int i = 0; i < menuItems.Length; i++)
        {
            originalPositions[i] = menuItems[i].anchoredPosition;
        }
        UpdateSelectionVisual();
    }

    void Update()
    {
        if (!gameObject.activeInHierarchy) return;

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentIndex = (currentIndex - 1 + menuItems.Length) % menuItems.Length;
            UpdateSelectionVisual();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentIndex = (currentIndex + 1) % menuItems.Length;
            UpdateSelectionVisual();
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            ExecuteSelection();
        }
    }

    void UpdateSelectionVisual()
    {
        for (int i = 0; i < menuItems.Length; i++)
        {
            Vector2 pos = originalPositions[i];
            if (i == currentIndex)
            {
                pos.x += indentOffsetX;
            }
            menuItems[i].anchoredPosition = pos;
        }

        Vector3 cursorPos = cursor.position;
        cursorPos.y = menuItems[currentIndex].position.y;
        cursor.position = cursorPos;
    }

    void ExecuteSelection()
    {
        string selectedItem = menuItems[currentIndex].name;
        Debug.Log("Selected: " + selectedItem);

        switch (selectedItem)
        {
            case "Button - Continue":
                if (SaveManager.HasSave())
                {
                    StartCoroutine(LoadGameCoroutine());
                }
                break;

            case "Button - NewGame":
                StartNewGame();
                break;

            case "Button - Exit":
                Application.Quit();
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
                break;

            default:
                Debug.LogWarning("Unknown menu item selected: " + selectedItem);
                break;
        }
    }

    void StartNewGame()
    {
        // Reset GameTimer
        if (GameTimer.Ins != null)
        {
            GameTimer.Ins.ResetTimer();
            GameTimer.Ins.ResumeTimer();
        }

        // Reset LevelManager
        if (LevelManager.Ins != null)
        {
            LevelManager.Ins.OnDespawn(); // Clear all active levels
            LevelManager.Ins.OnLoadLevel(0); // Load the first level
            LevelManager.Ins.OnInit(); // Initialize level
        }

        // Reset CheckpointManager
        if (CheckpointManager.Ins != null)
        {
            CheckpointManager.Ins.ClearCheckpoint(); // Clear any existing checkpoint
            if (CheckpointManager.Ins.startingCheckpoint != null)
            {
                CheckpointManager.Ins.SetCheckpoint(CheckpointManager.Ins.startingCheckpoint.position); // Set to starting checkpoint
            }
        }

        // Reset Player
        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            player.OnInit(); // Reset player state (position, health, etc.)
            if (CheckpointManager.Ins.HasCheckpoint())
            {
                player.transform.position = CheckpointManager.Ins.GetCheckpoint(); // Move to starting checkpoint
            }
            player.LoadStats(0, 0, 0, player.GetHp()); // Reset jump, fall, attempt counts
        }

        // Clear save file to prevent loading old data
        if (SaveManager.HasSave())
        {
            File.Delete(SaveManager.path);
            Debug.Log("Save file deleted for new game.");
        }

        // Activate gameplay objects
        foreach (GameObject obj in objectsToActivateOnStart)
        {
            obj.SetActive(true);
        }

        // Deactivate main menu
        if (mainMenuCanvas != null)
        {
            mainMenuCanvas.SetActive(false);
        }
    }

    private IEnumerator LoadGameCoroutine()
    {
        GameData data = SaveManager.LoadGame();
        if (data == null)
        {
            Debug.LogError("Failed to load game data.");
            yield break;
        }

        LevelManager.Ins.OnLoadLevel(data.currentLevel);
        yield return null;

        Player player = FindObjectOfType<Player>();
        if (player == null)
        {
            Debug.LogError("Player not found after loading level.");
            yield break;
        }

        player.transform.position = new Vector2(data.playerPos[0], data.playerPos[1]);
        player.LoadStats(data.jumpCount, data.fallCount, data.attemptCount, data.hp);

        // Checkpoint
        CheckpointManager.Ins.SetCheckpoint(new Vector2(data.checkpointPos[0], data.checkpointPos[1]));

        // Set elapsedTime from saved data and resume timer
        if (GameTimer.Ins != null)
        {
            GameTimer.Ins.SetElapsedTime(data.elapsedTime);
            GameTimer.Ins.ResumeTimer();
        }

        if (mainMenuCanvas != null)
            mainMenuCanvas.SetActive(false);
    }
}
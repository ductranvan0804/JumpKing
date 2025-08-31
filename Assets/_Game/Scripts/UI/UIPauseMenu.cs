using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIPauseMenu : UICanvas
{
    [SerializeField] RectTransform[] menuItems;
    [SerializeField] RectTransform[] confirmItems;
    [SerializeField] RectTransform cursor;
    [SerializeField] RectTransform cursorConfirm;

    private int currentConfirmIndex = 0;
    private int currentIndex = 0;
    public float indentOffsetX = 30f;
    [SerializeField] Vector2[] originalPositions;
    [SerializeField] Vector2[] originalConfirmPositions;

    [SerializeField] GameObject menuPanel;
    [SerializeField] GameObject confirmPanel;
    [SerializeField] GameObject recordPanel;

    [SerializeField] GameObject mainMenuCanvas;
    [SerializeField] GameObject[] gameplayObjectsToDisable;
    [SerializeField] TMPro.TextMeshProUGUI recordTimeText;

    


    public override void Open()
    {
        base.Open();
        Time.timeScale = 0f;
    }

    public override void Close()
    {
        base.Close();
        Time.timeScale = 1f; 
    }


    public override void Setup()
    {
        base.Setup();

        originalPositions = new Vector2[menuItems.Length];
        for (int i = 0; i < menuItems.Length; i++)
        {
            originalPositions[i] = menuItems[i].anchoredPosition;
        }

        UpdateSelectionVisualMenu();

        originalConfirmPositions = new Vector2[confirmItems.Length];
        for (int i = 0; i < confirmItems.Length; i++)
        {
            originalConfirmPositions[i] = confirmItems[i].anchoredPosition;
        }

        UpdateSelectionVisualConfirm();

    }

    void Update()
    {
        if (confirmPanel.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                currentConfirmIndex = (currentConfirmIndex - 1 + confirmItems.Length) % confirmItems.Length;
                UpdateSelectionVisualConfirm();
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                currentConfirmIndex = (currentConfirmIndex + 1) % confirmItems.Length;
                UpdateSelectionVisualConfirm();
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                ExecuteConfirmSelection();
            }

            return;
        }

        if (menuPanel.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                currentIndex = (currentIndex - 1 + menuItems.Length) % menuItems.Length;
                UpdateSelectionVisualMenu();
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                currentIndex = (currentIndex + 1) % menuItems.Length;
                UpdateSelectionVisualMenu();
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                ExecuteMenuSelection();
            }
        }
    }



    void UpdateSelectionVisualMenu()
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

    void UpdateSelectionVisualConfirm()
    {
        for (int i = 0; i < confirmItems.Length; i++)
        {
            Vector2 pos = originalConfirmPositions[i];
            if (i == currentConfirmIndex)
            {
                pos.x += indentOffsetX;
            }
            confirmItems[i].anchoredPosition = pos;
        }

        Vector3 cursorPos = cursorConfirm.position;
        cursorPos.y = confirmItems[currentConfirmIndex].position.y;
        cursorConfirm.position = cursorPos;
    }


    void ExecuteMenuSelection()
    {
        string selectedItem = menuItems[currentIndex].name;
        Debug.Log("Menu Selected: " + selectedItem);

        switch (selectedItem)
        {
            case "Button - SaveAndExit":
                confirmPanel.SetActive(true);
                break;

            case "Button - Back":
                this.Close();
                gameObject.SetActive(false);
                PauseManager.isGamePaused = false;
                break;
        }
    }

    void ExecuteConfirmSelection()
    {
        string selectedItem = confirmItems[currentConfirmIndex].name;
        Debug.Log("Confirm Selected: " + selectedItem);

        switch (selectedItem)
        {
            case "Button - Yes":
                SaveAndReturnToMainMenu();
                break;
            case "Button - No":
                confirmPanel.SetActive(false);
                currentIndex = 0;
                UpdateSelectionVisualMenu();
                break;
        }
    }

    void SaveAndReturnToMainMenu()
    {
        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            SaveManager.SaveGame(
                player,
                GameTimer.Ins.GetElapsedTime(),
                CheckpointManager.Ins.GetCheckpoint(),
                LevelManager.Ins.GetCurrentLevel()
            );
        }

        // Resume game time
        Time.timeScale = 1f;
        PauseManager.isGamePaused = false;

        
        foreach (GameObject obj in gameplayObjectsToDisable)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        if (mainMenuCanvas != null)
            mainMenuCanvas.SetActive(true);

        RecordDisplay record = mainMenuCanvas.GetComponentInChildren<RecordDisplay>();
        if (record != null)
        {
            record.UpdateDisplay();
        }

        this.Close();
        gameObject.SetActive(false);
    }

    void ShowRecordPanel()
    {
        recordPanel.SetActive(true);

        float time = GameTimer.Ins.GetElapsedTime();
        TimeSpan ts = TimeSpan.FromSeconds(time);

        recordTimeText.text = string.Format("Play Time: {0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);
    }

}

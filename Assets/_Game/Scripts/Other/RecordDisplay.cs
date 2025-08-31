using UnityEngine;
using TMPro;

public class RecordDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI jumpValueText;
    [SerializeField] private TextMeshProUGUI fallValueText;
    [SerializeField] private TextMeshProUGUI attemptValueText;
    [SerializeField] private TextMeshProUGUI hourValueText;
    [SerializeField] private TextMeshProUGUI minuteValueText;
    [SerializeField] private TextMeshProUGUI secondValueText;

    private Player player;
    private GameTimer gameTimer;
    private float updateInterval = 0.5f;
    private float timeSinceLastUpdate = 0f;
    private bool isInMainMenu = false;

    private void Start()
    {
        isInMainMenu = FindObjectOfType<UIMainMenu>() != null;
        player = FindObjectOfType<Player>();
        gameTimer = GameTimer.Ins;
        UpdateDisplay(); 
    }

    private void Update()
    {
        if (!isInMainMenu)
        {
            timeSinceLastUpdate += Time.deltaTime;
            if (timeSinceLastUpdate >= updateInterval)
            {
                UpdateDisplay();
                timeSinceLastUpdate = 0f;
            }
        }
    }

    public void UpdateDisplay()
    {
        if (isInMainMenu || player == null || gameTimer == null)
        {
            if (SaveManager.HasSave())
            {
                GameData data = SaveManager.LoadGame();
                if (data != null)
                {
                    jumpValueText.text = data.jumpCount.ToString();
                    fallValueText.text = data.fallCount.ToString();
                    attemptValueText.text = data.attemptCount.ToString();

                    int hours = Mathf.FloorToInt(data.elapsedTime / 3600f);
                    int minutes = Mathf.FloorToInt((data.elapsedTime % 3600f) / 60f);
                    int seconds = Mathf.FloorToInt(data.elapsedTime % 60f);

                    hourValueText.text = hours.ToString("D2");
                    minuteValueText.text = minutes.ToString("D2");
                    secondValueText.text = seconds.ToString("D2");
                }
                else
                {
                    Debug.LogWarning("Failed to load game data.");
                    ClearDisplay();
                }
            }
            else
            {
                Debug.LogWarning("No save data found.");
                ClearDisplay();
            }
        }
        else
        {
            jumpValueText.text = player.GetJumpCount().ToString();
            fallValueText.text = player.GetFallCount().ToString();
            attemptValueText.text = player.GetAttemptCount().ToString();

            float elapsedTime = gameTimer.GetElapsedTime();
            int hours = Mathf.FloorToInt(elapsedTime / 3600f);
            int minutes = Mathf.FloorToInt((elapsedTime % 3600f) / 60f);
            int seconds = Mathf.FloorToInt(elapsedTime % 60f);

            hourValueText.text = hours.ToString("D2");
            minuteValueText.text = minutes.ToString("D2");
            secondValueText.text = seconds.ToString("D2");
        }
    }

    private void ClearDisplay()
    {
        jumpValueText.text = "0";
        fallValueText.text = "0";
        attemptValueText.text = "0";
        hourValueText.text = "00";
        minuteValueText.text = "00";
        secondValueText.text = "00";
    }
}
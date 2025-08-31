using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private UIPauseMenu pauseMenu;
    [SerializeField] private RecordDisplay recordDisplay;
    public static bool isGamePaused = false;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pauseMenu.gameObject.SetActive(true);
            pauseMenu.Open();
            isGamePaused = true;
            recordDisplay.UpdateDisplay();
        }
    }
}

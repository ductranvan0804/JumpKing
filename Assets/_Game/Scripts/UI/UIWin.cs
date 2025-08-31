using UnityEngine;
using UnityEngine.SceneManagement;

public class UIWin : MonoBehaviour
{
    [SerializeField] GameObject winCanvas;
    [SerializeField] GameObject menuCanvas;
    private bool canReturnToMenu = false;

    public void ShowWinScreen()
    {
        winCanvas.SetActive(true);
        Time.timeScale = 0f;
        GameTimer.Ins.PauseTimer();
        canReturnToMenu = true;
    }

    void Update()
    {
        if (canReturnToMenu && Input.GetKeyDown(KeyCode.Space))
        {
            Time.timeScale = 0f; 
            winCanvas.SetActive(false); 

            if (menuCanvas != null)
            {
                menuCanvas.SetActive(true); 
            }

            UIMainMenu menu = FindObjectOfType<UIMainMenu>();
            if (menu != null)
            {
                menu.Open();
            }

            gameObject.SetActive(false); 
        }
    }
}

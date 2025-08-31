using UnityEngine;

public class LoadPrefabTrigger : MonoBehaviour
{
    [SerializeField] private int[] levelsToLoad;
    [SerializeField] private int[] levelsToUnload;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Constant.TAG_PLAYER))
        {
            for (int i = 0; i < levelsToLoad.Length; i++)
            {
                LevelManager.Ins.OnLoadLevel(levelsToLoad[i]);
            }

            for (int i = 0; i < levelsToUnload.Length; i++)
            {
                LevelManager.Ins.OnUnloadLevel(levelsToUnload[i]);
            }
        }
    }
}

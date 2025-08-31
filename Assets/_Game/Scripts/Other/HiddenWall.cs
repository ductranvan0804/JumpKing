using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenWall : MonoBehaviour
{
    [SerializeField] private SpriteRenderer wallRenderer;
    [SerializeField] private Collider2D wallCol;
    [SerializeField] private Collider2D playerCol;
    private HashSet<GameObject> playersInside = new HashSet<GameObject>();

    private void Start()
    {

        ShowWall();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(Constant.TAG_PLAYER))
        {
            playersInside.Add(other.gameObject);
            HideWall();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(Constant.TAG_PLAYER))
        {
            StartCoroutine(CheckExitDelay(other.gameObject));
        }
    }

    private IEnumerator CheckExitDelay(GameObject player)
    {
        yield return new WaitForSeconds(0.1f); 

        if (!IsPlayerStillInside(player))
        {
            playersInside.Remove(player);

            if (playersInside.Count == 0)
                ShowWall();
        }
    }

    private bool IsPlayerStillInside(GameObject player)
    {
        if (wallCol != null && playerCol != null)
        {
            return wallCol.bounds.Intersects(playerCol.bounds);
        }
        return false;
    }

    private void HideWall()
    {
        wallRenderer.enabled = false;
    }

    private void ShowWall()
    {
        wallRenderer.enabled = true;
    }
}

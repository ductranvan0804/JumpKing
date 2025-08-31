using UnityEngine;

public class CheckpointManager : Singleton<CheckpointManager>
{
    [Header("Checkpoint")]
    public Transform startingCheckpoint;  

    private Vector2 checkpointPos;
    private bool hasCheckpoint = false;

    private void Start()
    {
        if (startingCheckpoint != null)
        {
            checkpointPos = startingCheckpoint.position;
            hasCheckpoint = true;
            Debug.Log("Starting checkpoint initialized at: " + checkpointPos);
        }
    }

    public void SetCheckpoint(Vector2 pos)
    {
        checkpointPos = pos;
        hasCheckpoint = true;
        Debug.Log("Checkpoint set at: " + pos);
    }

    public Vector2 GetCheckpoint()
    {
        return checkpointPos;
    }

    public bool HasCheckpoint()
    {
        return hasCheckpoint;
    }

    public void ClearCheckpoint()
    {
        hasCheckpoint = false;
    }
}

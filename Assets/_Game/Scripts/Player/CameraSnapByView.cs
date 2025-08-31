using UnityEngine;

public class CameraSnapByView : MonoBehaviour
{
    public Transform player;
    private float cameraHeight;
    private float cameraWidth;

    private Vector3 currentCameraPosition;

    void Start()
    {
        cameraHeight = Camera.main.orthographicSize * 2f;
        cameraWidth = cameraHeight * Camera.main.aspect;

        currentCameraPosition = transform.position;
    }

    void LateUpdate()
    {
        float playerY = player.position.y;
        float cameraBottom = currentCameraPosition.y - cameraHeight / 2f;
        float cameraTop = currentCameraPosition.y + cameraHeight / 2f;

        if (playerY > cameraTop)
        {
            currentCameraPosition.y += cameraHeight;
            transform.position = currentCameraPosition;
        }
        else if (playerY < cameraBottom)
        {
            currentCameraPosition.y -= cameraHeight;
            transform.position = currentCameraPosition;
        }
    }

    Vector3 GetSnappedPosition(float playerY)
    {
        int zoneIndex = Mathf.RoundToInt(playerY / cameraHeight);
        float snappedY = zoneIndex * cameraHeight;
        return new Vector3(0f, snappedY, -10f);
    }


    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(currentCameraPosition, new Vector3(cameraWidth, cameraHeight, 0f));
    }
}

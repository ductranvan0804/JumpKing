using UnityEngine;

public class WindDirectionChanger : MonoBehaviour
{
    [SerializeField] AreaEffector2D areaEffector;
    public float windForce = 5f;
    public float switchTime = 9f; 
    private float timer;
    private bool isBlowingRight = true;

    void Start()
    {
        timer = switchTime;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            isBlowingRight = !isBlowingRight;
            areaEffector.forceAngle = isBlowingRight ? 0 : 180; 
            timer = switchTime;
        }
    }
}
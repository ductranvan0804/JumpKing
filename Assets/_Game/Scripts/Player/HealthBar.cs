using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Slider healthSlider;
    [SerializeField] Vector3 offset;
    Transform target;

    float hp;
    float maxHp;

    void LateUpdate()
    {
        if (target != null)
        {
            transform.position = target.position + offset;

            transform.rotation = Quaternion.identity;
        }

        healthSlider.value = Mathf.Lerp(healthSlider.value, hp, Time.deltaTime * 5f);
    }

    public void OnInit(float maxHp, Transform target)
    {
        this.maxHp = maxHp;
        this.hp = maxHp;
        this.target = target;

        healthSlider.maxValue = maxHp;
        healthSlider.value = maxHp;
    }

    public void SetNewHp(float hp)
    {
        this.hp = hp;
    }
}

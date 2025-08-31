using UnityEngine;

public class Raven : MonoBehaviour
{
    public float flyUpForce = 50f;
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private GameObject nextBird;
    private bool hasFlown = false;


    void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasFlown && other.CompareTag(Constant.TAG_PLAYER))
        {
            hasFlown = true;
            FlyAway();
        }
    }

    void FlyAway()
    {
        animator.SetTrigger(Constant.ANIM_RAVENFLYUP);
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.linearVelocity = new Vector2(0, flyUpForce);

        Invoke(nameof(Disappear), 0.5f);
    }

    void Disappear()
    {
        gameObject.SetActive(false);

        if (nextBird != null)
        {
            nextBird.SetActive(true);
        }
    }
}

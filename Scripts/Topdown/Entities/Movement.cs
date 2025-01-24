using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour
{
    protected bool moveEnable = true;
    protected Rigidbody2D rb;

    [SerializeField] protected int velocity = 10;

    public void EnableMove(bool value) => moveEnable = value;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.gravityScale = 0;
        rb.freezeRotation = true;
    }

}

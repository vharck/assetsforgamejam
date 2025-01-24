using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Projectile : MonoBehaviour
{
    protected int velocity = 10;
    protected int damage = 1;
    protected float lifeTime = 1;
    protected Vector2 direction = Vector2.zero;
    protected bool originPlayer;

    protected Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        var collider = GetComponent<Collider2D>();
        collider.isTrigger = true;

        rb.gravityScale = 0;
        rb.freezeRotation = true;
    }

    public void Setup(int velocity, int damage, float lifeTime, Vector2 direction, bool originPlayer)
    {
        this.velocity = velocity;
        this.damage = damage;
        this.lifeTime = lifeTime;
        this.direction = direction;
        this.originPlayer = originPlayer;
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        switch (collider.gameObject.layer)
        {
            case int layer when layer == GameController.Instance.EnemyLayer:
                if (originPlayer)
                {
                    collider.GetComponent<HealthSystem>().TakeDamage(damage);
                    Destroy(gameObject);
                }
                break;
            case int layer when layer == GameController.Instance.PlayerLayer:
                if (!originPlayer)
                {
                    collider.GetComponent<HealthSystem>().TakeDamage(damage);
                    Destroy(gameObject);
                }
                break;
            case int layer when layer == GameController.Instance.ObstacleLayer:
                Destroy(gameObject);
                break;
        }
    }

    private void FixedUpdate()
    {
        rb.velocity = Time.deltaTime * velocity * direction;
    }
}

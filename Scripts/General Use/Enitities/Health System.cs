using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class HealthSystem : MonoBehaviour
{
    public int Health { get; private set; }
    public int MaxHealth { get; private set; }
    public float PriorityMult { get; private set; }
    private int defense;

    private void Start()
    {
        var collider = GetComponent<BoxCollider2D>();
        collider.isTrigger = true;
    }

    public void Setup(int maxHealth, int defense = 0, float priorityMult = 1)
    {
        PriorityMult = priorityMult;
        this.defense = defense;
        MaxHealth = maxHealth;
        Health = maxHealth;
    }

    public void UpdateDefense(int value) => defense = value;
    public void Heal(int value) => Health = Mathf.Clamp(value + Health, 0, MaxHealth);
    public void TakeDamage(int value) => Health -= Mathf.Clamp(value - defense, 0, 2 * value);

    private void Death()
    {
        Destroy(gameObject);
    }

    private void Update()
    {
        if (Health <= 0) Death();
    }

}
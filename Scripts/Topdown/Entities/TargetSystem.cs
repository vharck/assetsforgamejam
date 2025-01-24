using System.Collections;
using UnityEngine;

public class TargetSystem : MonoBehaviour
{
    [SerializeField] private LayerMask targetsLayers;

    [SerializeField] private Transform target;

    void Awake()
    {
        StartCoroutine(TargetSelection());
    }

    private void ChooseTarget(Collider2D[] entities)
    {
        float minPoints = float.MaxValue;
        foreach (var entity in entities)
        {
            var distance = Vector2.Distance(transform.position, entity.transform.position);
            if (!entity.TryGetComponent<HealthSystem>(out var healthSys)) continue;

            var points = healthSys.Health / healthSys.MaxHealth * distance * healthSys.PriorityMult;

            if (minPoints > points)
            {
                minPoints = points;
                target = entity.transform;
            }
        }
    }

    private IEnumerator TargetSelection()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            if (!isActiveAndEnabled) break;

            var colliders = Physics2D.OverlapCircleAll(transform.position, 40, targetsLayers);
            switch (colliders.Length)
            {
                case 0: continue;
                case 1:
                    target = colliders[0].transform;
                    break;
                default:
                    ChooseTarget(colliders);
                    break;
            }


        }
    }


}

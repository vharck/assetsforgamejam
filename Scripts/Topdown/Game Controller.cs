using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    #region Instance
    public static GameController Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    #endregion

    #region Layer, Tags and Names

    [SerializeField] public LayerMask PlayerLayer { get; private set; }
    [SerializeField] public LayerMask EnemyLayer { get; private set; }
    [SerializeField] public LayerMask ProjectileLayer { get; private set; }
    [SerializeField] public LayerMask ObstacleLayer { get; private set; }
    [SerializeField] public LayerMask InteractableLayer { get; private set; }

    #endregion

}

using UnityEngine;
using Youregone.LevelGeneration;

public class WaterSplash : MovingObject
{
    [CustomHeader("Settings")]
    [SerializeField] private float _destructionDelay;

    protected override void Awake()
    {
        base.Awake();
        Destroy(gameObject, _destructionDelay);
    }
}

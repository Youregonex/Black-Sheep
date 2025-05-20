using UnityEngine;
using Youregone.LevelGeneration;
using Youregone.SL;
using Youregone.YPlayerController;

public class WaterSplash : MovingObject
{
    [CustomHeader("Settings")]
    [SerializeField] private float _destructionDelay;

    public void Initialize(bool playerSplash)
    {
        if(!playerSplash)
            ChangeVelocity(new Vector2(ServiceLocator.Get<PlayerController>().CurrentSpeed, 0f));
    }

    protected override void Awake()
    {
        base.Awake();
        Destroy(gameObject, _destructionDelay);
    }
}

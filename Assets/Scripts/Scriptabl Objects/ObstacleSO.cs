using UnityEngine;
using System.Collections.Generic;

namespace Youregone.SO
{
    [CreateAssetMenu(fileName = "NewObstacle", menuName = "ScriptableObjects/ObstacleSO")]
    public class ObstacleSO : ScriptableObject
    {
        public EObstacleType obstacleType;
        public List<Sprite> sprites;
    }
}
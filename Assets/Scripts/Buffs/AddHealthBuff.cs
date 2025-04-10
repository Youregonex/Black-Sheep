using Youregone.YPlayerController;

namespace Youregone.LevelGeneration
{
    public class AddHealthBuff : Buff
    {
        protected override void Apply(PlayerController player)
        {
            player.AddHealth();
            Destroy(gameObject);
        }
    }
}
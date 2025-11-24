using Link;
using UnityEngine;
namespace HuyThanh.Cooking.SarmaleRomania
{
    public class BlackItem : ItemIdleBase
    {
        [SerializeField] MinigameControl Minigame;
        public override void OnClickDown()
        {
            base.OnClickDown();
            Minigame.MoveInMiniGame();
        }
    }

}

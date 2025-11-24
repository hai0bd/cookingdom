using DG.Tweening;
using Link;
using UnityEngine;

namespace HuyThanh.Cooking.TunaSaladFrench
{
    public class MiniGame5 : Singleton<MiniGame5>
    {
        [SerializeField] Oven oven;
        [SerializeField] Pan pan;

        [SerializeField] MiniGame3 miniGame3;
        [SerializeField] MiniGame4 miniGame4;
        [SerializeField] ActionMove stoveActionMove;

        private bool isWin = false;

        public void CheckDone()
        {
            if (oven.IsState(Oven.State.Done) && pan.IsState(Pan.State.Done) && isWin == false)
            {
                miniGame3.OnFinish(null);
                miniGame4.OnFinish(null);
                stoveActionMove.Active();
                isWin = true;
            }
        }

        public void CheckDone(float time)
        {
            DOVirtual.DelayedCall(time, () => CheckDone());
        }
    }
}

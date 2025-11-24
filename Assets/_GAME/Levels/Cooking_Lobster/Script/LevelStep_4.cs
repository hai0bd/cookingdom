using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link.Cooking.Lobster
{
    public class LevelStep_4 : LevelStepBase
    {
        [SerializeField] Lobster lobster;
        [SerializeField] LobsterPlate lobsterPlate;

        public override void OnStart()
        {
            DOVirtual.DelayedCall(1f, () => lobster.ChangeState(Lobster.State.MoveToBoard));
            base.OnStart();
        }

        public override bool IsDone()
        {
            return lobsterPlate.IsDone;
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link.Cooking.Lobster
{
    public class LevelStep_3 : LevelStepBase
    {
        [SerializeField] ActionAnim[] anims;

        //check finish
        [SerializeField] WoodBowl woodBowl;
        [SerializeField] ElectricStove stove, stoveStream;
        [SerializeField] PotLid potLid;
        [SerializeField] Sprite hint_1, hint_2;

        [SerializeField] ActionAnim[] animas;
        [SerializeField] ItemSpice[] items;

        public override void OnStart()
        {
            base.OnStart();
        }

        public override bool IsDone()
        {
            if (!animas.Last().gameObject.activeSelf)
            {
                foreach (var item in items)
                {
                    if (!item.IsState(ItemSpice.State.Spice))
                    {
                        return false;
                    }
                }

                foreach (var anima in animas)
                {
                    anima.Active();
                }

                LevelControl.Ins.SetHint(hint_1);
                LevelControl.Ins.SetHintTextDone(4, 1);
                
            }
            return woodBowl.IsState(WoodBowl.State.Fill) && !stove.IsOn && potLid.IsDone && !stoveStream.IsOn;
        }

        public override void OnFinish(Action action)
        {
            anims[0].doneEvent.AddListener(() => woodBowl.TF.position = new Vector3(-1.54f, 3f, 0));
            for (int i = 0; i < anims.Length; i++)
            {
                anims[i].Active();
            }

            base.OnFinish(action);
        }

        public override void NextHint()
        {
            base.NextHint();
            LevelControl.Ins.SetHint(hint_2);
        }
    }
}
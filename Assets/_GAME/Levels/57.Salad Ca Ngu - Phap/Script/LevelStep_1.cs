using Link;
using System.Collections.Generic;
using UnityEngine;

namespace HuyThanh.Cooking.TunaSaladFrench
{


    public class LevelStep_1 : LevelStepBase
    {
        [SerializeField] private CutItem tomato, xaLachBaby;
        [SerializeField] private Bean bean;
        [SerializeField] private Egg egg;
        [SerializeField] private Potato potato;
        [SerializeField] private List<EggFragile> eggsFragile;
        [SerializeField] private List<EggCut> eggsCut;

        [SerializeField] private Stove stove;
        [SerializeField] private WaterSink waterSink;
        [SerializeField] private Pot pot;
        [SerializeField] private NapNoi napNoi;
        public override bool IsDone()
        {
            bool cutItemOk = tomato.IsState(CutItem.State.Done) && xaLachBaby.IsState(CutItem.State.Done);
            bool otherItemOk = bean.IsState(Bean.State.Done) && egg.IsState(Egg.State.Done) && potato.IsState(Potato.State.Done);

            bool eggsFragileOk = true;

            foreach (EggFragile eggFragile in eggsFragile)
            {
                if (!eggFragile.IsState(EggFragile.State.Done))
                {
                    eggsFragileOk = false;
                }
            }

            bool eggsCutOk = true;
            foreach (EggCut eggCut in eggsCut)
            {
                if (!eggCut.IsState(EggCut.State.Done))
                {
                    eggsCutOk = false;
                }
            }
            //Debug.Log(waterSink.CheckDone());
            //Debug.Log($"CutItem:{cutItemOk}");
            //Debug.Log($"otherItemOk:{otherItemOk}");
            //Debug.Log($"eggsFragileOk:{eggsFragileOk}");
            //Debug.Log($"eggsCutOk:{eggsCutOk}");

            return cutItemOk && otherItemOk && eggsFragileOk && eggsCutOk && waterSink.CheckDone() && pot.IsState(Pot.State.Done) && napNoi.IsState(NapNoi.State.Done);
        }

    }
}

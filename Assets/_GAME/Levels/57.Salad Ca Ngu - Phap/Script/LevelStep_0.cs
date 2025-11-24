using Link;
using UnityEngine;

namespace HuyThanh.Cooking.TunaSaladFrench
{
    public enum Fx
    {
        Click,
        Boil,
        KnifeCut,
        KnifeTakeOut,
        OilBoil,
        OpenMustard,
        CloseMustard,
        WinePour,
        OilPour,
        PepperPour,
        MortarPestle,
        Take,
        Button,
        WaterFall,
        KnifeCutEgg,
        EggCracking,
        Grater,
        ButterCutting,
        BeardCutting,
        SaltPouring,
        OvenBaking,
        OvenOpen,
        KnifeChop,
        PearlCutting,
        CatSound,
    }
    public class LevelStep_0 : LevelStepBase
    {
        [SerializeField] MortarSpice mortar;
        [SerializeField] Tuna tuna;
        [SerializeField] MustardIdle mustardIdle;
        public override bool IsDone()
        {
            return mortar.IsState(MortarSpice.State.Done) && tuna.IsState(Tuna.State.Trash)
                && mustardIdle.IsState(MustardIdle.State.Done);
        }

        public override void NextHint()
        {
            base.NextHint();
        }
    }

}

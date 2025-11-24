using Link;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HuyThanh.Cooking.Burrito
{

    public enum Fx
    {
        Click,
        PutDown,
        KnifeCut,
        WaterBoil,
        OilBoil,
        WaterFall,
        Button,
        Take,
        Tissue,
        WaterSplash,
        Grinder,
        WaterPouring,
        OilPouring,
        PepperPouring,
        SpoonFry,
        BeanPouring,
        OreganoPouring,
        BurritoBoil,
        CatSound,
        PouringSpice,
        KnifeSplash,
        DoneSomething,
        HappyCatSound,
    }
    public class LevelStep_0 : LevelStepBase
    {
        [SerializeField] WaterTap waterTap;
        [SerializeField] WaterSink waterSink;
        [SerializeField] Basket basket;
        [SerializeField] Colander colander;
        [SerializeField] Corn corn;
        [SerializeField] TissueBox tissueBox;
        [SerializeField] MeatClean meatClean;
        [SerializeField] DirtyItem onion, vegetable;

        [BoxGroup("Hint")]
        [FoldoutGroup("Sprite")]
        [SerializeField] Sprite hint_corn, hint_bean, hint_meat, hint_vegetable;

        public override void NextHint()
        {
            if (corn.IsState(Corn.State.Done) == false)
            {
                LevelControl.Ins.SetHint(hint_corn);
                return;
            }
            if (colander.IsState(Colander.State.Done) == false)
            {
                LevelControl.Ins.SetHint(hint_bean);
                return;
            }
            if (meatClean.IsState(MeatClean.State.Done) == false)
            {
                LevelControl.Ins.SetHint(hint_meat);
                return;
            }
            if (onion.IsState(DirtyItem.State.Done) == false || vegetable.IsState(DirtyItem.State.Done) == false)
            {
                return;
            }
            LevelControl.Ins.SetHint(hint_vegetable);
        }


        public override bool IsDone()
        {
            if (tissueBox.CheckDone() == false)
            {
                return false;
            }

            if (meatClean.IsState(MeatClean.State.Done) == false)
            {
                return false;
            }

            if (waterSink.CheckWin() && waterTap.IsOff() &&
                basket.CheckDone() && colander.IsState(Colander.State.Done))
            {
                return true;
            }
            return false;
        }
    }

}


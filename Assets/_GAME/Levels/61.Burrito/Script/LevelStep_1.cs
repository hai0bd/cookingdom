using Link;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HuyThanh.Cooking.Burrito
{
    public class LevelStep_1 : LevelStepBase
    {

        [SerializeField] BowlGetBasketItem[] bowlGetBasketItems;
        [SerializeField] Grinder grinder;
        [SerializeField] GarlicCutItem garlicCut;
        [SerializeField] BasketCutItem onion, vegetable, tomato;

        [FoldoutGroup("Hint")]
        [SerializeField] Sprite hint_meat, hint_item, hint_corn;
        public override void NextHint()
        {
            if (grinder.IsState(Grinder.State.Done) == false)
            {
                LevelControl.Ins.SetHint(hint_meat);
                return;
            }
            if (onion.IsState(BasketCutItem.State.Done) == false || vegetable.IsState(BasketCutItem.State.Done) == false ||
                tomato.IsState(BasketCutItem.State.Done) == false || garlicCut.IsState(GarlicCutItem.State.Done) == false)
            {
                LevelControl.Ins.SetHint(hint_item);
                return;
            }
            LevelControl.Ins.SetHint(hint_corn);
        }

        public bool IsDoneBowl()
        {
            for (int i = 0; i < bowlGetBasketItems.Length; i++)
            {
                if (!bowlGetBasketItems[i].IsDone)
                    return false;
            }
            return true;
        }

        public override bool IsDone()
        {
            if (IsDoneBowl() == false)
            {
                return false;
            }

            if (grinder.IsState(Grinder.State.Done) == false)
            {
                return false;
            }

            return true;
        }
    }
}
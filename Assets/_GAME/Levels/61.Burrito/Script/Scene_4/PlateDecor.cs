using Link;
using UnityEngine;

namespace HuyThanh.Cooking.Burrito
{
    public class PlateDecor : ItemIdleBase
    {
        public enum State
        {
            Vegetable,
            Lemon,
            Tomato,
            Burrito,
            Leaf,
            Done
        }
        [SerializeField] State state;

        [SerializeField] GameObject lemon, tomato, burrito, leaf;

        [SerializeField] HintText hintVegetable, hintLemon, hintTomato, hintBurrito, hintLeaf;

        private int vegetableCount = 0;
        private int lemonCount = 0;
        private int tomatoCount = 0;
        private int burritoCount = 0;
        private int leafCount = 0;

        public override bool IsState<T>(T t)
        {
            return state == (State)(object)t;
        }
        public override void ChangeState<T>(T t)
        {

            state = (State)(object)t;

            switch (state)
            {
                case State.Vegetable:

                    break;
                case State.Lemon:
                    hintVegetable.OnActiveHint();
                    lemon.SetActive(true);
                    break;
                case State.Tomato:
                    hintLemon.OnActiveHint();
                    tomato.SetActive(true);
                    break;
                case State.Burrito:
                    LevelControl.Ins.NextHint();
                    hintTomato.OnActiveHint();
                    burrito.SetActive(true);
                    break;
                case State.Leaf:
                    hintBurrito.OnActiveHint();
                    leaf.SetActive(true);
                    break;
                case State.Done:
                    hintLeaf.OnActiveHint();
                    // Handle done state
                    LevelControl.Ins.CheckStep(1f);
                    break;
            }
        }

        public void Remove(DecorItemType type)
        {
            switch (type)
            {
                case DecorItemType.Vegetable:
                    vegetableCount++;
                    if (vegetableCount == 3)
                    {
                        this.ChangeState(State.Lemon);
                    }
                    break;
                case DecorItemType.Tomato:
                    tomatoCount++;
                    if (tomatoCount == 5)
                    {
                        this.ChangeState(State.Burrito);
                    }
                    break;
                case DecorItemType.Lemon:
                    lemonCount++;
                    if (lemonCount == 3)
                    {
                        this.ChangeState(State.Tomato);
                    }

                    break;
                case DecorItemType.LeafLeft:
                    leafCount++;
                    if (leafCount == 2)
                    {
                        this.ChangeState(State.Done);
                    }
                    break;
                case DecorItemType.LeafRight:
                    leafCount++;
                    if (leafCount == 2)
                    {
                        this.ChangeState(State.Done);
                    }
                    break;
                case DecorItemType.BurritoLeft:
                    burritoCount++;
                    if (burritoCount == 2)
                    {
                        this.ChangeState(State.Leaf);
                    }
                    break;
                case DecorItemType.BurritoRight:
                    burritoCount++;
                    if (burritoCount == 2)
                    {
                        this.ChangeState(State.Leaf);
                    }
                    break;
            }
        }
    }

}
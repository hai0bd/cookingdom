using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link.Turkey
{
    public enum ItemName { Apple, Leaf, Onion, Garlic, Tomato, Lemon, Chicken, Chili, Salt, Pepper, Oil, Butter, Wine }
    public class LevelStep_1 : LevelStepBase
    {
        //public enum FX { Bubble, Sharper, Scrap , Cut, Put, Clean, Water, Hold }
        public enum FX { Hold, Put, Bubble, Sharper, Scrap , Cut, Clean, Water, Toggle, Fry, Spoon, Clock, Ting, Tissue, Oven }

        [SerializeField] Plate[] plates;
        [SerializeField] WaterSink waterSink;
        [SerializeField] Sprite sinkHint;
        private List<IItemMoving> trashes = new List<IItemMoving>();

        public override void OnFinish(Action action)
        {
            for (int i = 0; i < plates.Length; i++)
            {
                //show trai tim tat ca cac dia
                plates[i].ChangeState(Plate.State.Heart);
            }
            DOVirtual.DelayedCall(0.75f, () =>
            {
                //delay xong chuyen man
                base.OnFinish(action);
                LevelControl.Ins.BlockControl(0.5f);
                transform.DOMove(Vector3.left * 7, 1f).OnComplete(() => gameObject.SetActive(false));
                LevelControl.Ins.SetHintTextDone(1, 5);

            });
        }

#if UNITY_EDITOR
        public bool isTest = false;
#endif

        public override bool IsDone()
        {
            for (int i = 0; i < plates.Length; i++)
            {
                if (!plates[i].IsDone) return false;
            }

            LevelControl.Ins.SetHint(sinkHint);
            LevelControl.Ins.SetHintTextDone(1, 4);

            if (!waterSink.IsDone) return false;
            if (trashes.Count > 0) return false;
            return true;
        }

        public override void AddTrash(IItemMoving item)
        {
            trashes.Add(item);
        }

        public override void RemoveTrash(IItemMoving item)
        {
            trashes.Remove(item);
        }

#if UNITY_EDITOR
        [SerializeField] Chicken chickenTest;
        [SerializeField] Plate[] platesTest;
        [SerializeField] Transform[] platePointsTest;
        [SerializeField] Fruit[] fruitsTest;

        [Button]
        private void FinishStep()
        {
            chickenTest.TF.position = platePointsTest[0].position;
            chickenTest.ChangeState(Chicken.State.Dish);

            for (int i = 0; i < fruitsTest.Length; i++)
            {
                fruitsTest[i].TF.position = platePointsTest[i + 1].position;
                fruitsTest[i].ChangeState(Fruit.State.Prepare);
            }
            for (int i = 0; i < platesTest.Length; i++)
            {
                platesTest[i].ChangeState(Plate.State.Full);
                platesTest[i].gameObject.SetActive(false);
            }

            LevelControl.Ins.CheckStep();
        }

#endif
    }
}
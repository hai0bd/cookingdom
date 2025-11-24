using DG.Tweening;
using Link;
using System.Collections.Generic;
using UnityEngine;

namespace HuyThanh.Cooking.SarmaleRomania
{
    public class Spatula : ItemMovingBase
    {
        [SerializeField] Transform spatulaPoint;

        [SerializeField] PanBase panBase;

        Dictionary<Collider2D, SpiceItemFry> items = new Dictionary<Collider2D, SpiceItemFry>();


        public override void OnClickDown()
        {
            base.OnClickDown();
            TF.DORotate(Vector3.forward * 30, 0.1f);
        }

        private void LateUpdate()
        {
            if (LevelControl.Ins.IsHaveObject<PanBase>(spatulaPoint.position))
            {
                panBase.SetCookingRate(spatulaPoint.position);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!panBase.IsState(PanBase.State.Mixing1, PanBase.State.Mixing2, PanBase.State.Mixing3, PanBase.State.Mixing4))
            {
                return;
            }

            if (!items.ContainsKey(collision))
            {
                items.Add(collision, collision.GetComponent<SpiceItemFry>());
            }
            if (items.ContainsKey(collision) && items[collision] != null)
            {
                items[collision].AddForce(this, (items[collision].transform.position - spatulaPoint.position).normalized);
                //if (!SoundControl.Ins.IsPlaying(Fx.SpoonFry))
                //{
                //    SoundControl.Ins.PlayFX(Fx.SpoonFry);
                //}
            }
        }

    }
}
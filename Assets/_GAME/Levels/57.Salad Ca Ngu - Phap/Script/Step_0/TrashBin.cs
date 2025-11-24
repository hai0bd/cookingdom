using DG.Tweening;
using Link;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HuyThanh.Cooking.TunaSaladFrench
{
    public class TrashBin : ItemIdleBase
    {
        [BoxGroup("Anim Name")][SerializeField] Animation anim;
        [BoxGroup("Anim Name")][SerializeField] string animTrashOpen;
        public override bool OnTake(IItemMoving item)
        {
            if (item is TunaCap && item.IsState(TunaCap.State.Trash)) /// neu la nap hop thi vut di
            {
                anim.Play(animTrashOpen);
                item.ChangeState(TunaCap.State.Done);
                item.OnMove(TF.position, Quaternion.identity, 0.3f);
                item.TF.DOScale(Vector3.zero, 0.3f);
                ///Destroy((item as TunaCap).gameObject, 0.35f); khong nen de Destroy, chung ta se can de check dieu kien thang
                //LevelControl.Ins.CheckStep();
                return true;
            }
            else if (item is Tuna && item.IsState(Tuna.State.Trash))
            {
                anim.Play(animTrashOpen);
                item.ChangeState(Tuna.State.Done);
                item.OnMove(TF.position, Quaternion.identity, 0.3f);
                item.TF.DOScale(Vector3.zero, 0.3f);
                //LevelControl.Ins.CheckStep();
                return true;
            }
            else if (item is EggFragile && item.IsState(EggFragile.State.Fragile))
            {
                anim.Play(animTrashOpen);
                item.ChangeState(EggFragile.State.Done);
                item.OnMove(TF.position, Quaternion.identity, 0.3f);
                item.TF.DOScale(Vector3.zero, 0.3f);
                return true;
            }
            else
            {
                item.OnBack();
            }
            return base.OnTake(item);
        }
    }

}

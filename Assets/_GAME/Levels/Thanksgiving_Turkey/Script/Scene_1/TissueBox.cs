using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link.Turkey
{
    public class TissueBox : ItemIdleBase
    {
        [SerializeField] Tissue tissue;

        public override void OnClickDown()
        {
            Tissue t = Instantiate(tissue, LevelControl.Ins.GetPoint(), Quaternion.identity);
            t.gameObject.SetActive(true);
            LevelControl.Ins.SetItemMoving(t);
        }
    }
}
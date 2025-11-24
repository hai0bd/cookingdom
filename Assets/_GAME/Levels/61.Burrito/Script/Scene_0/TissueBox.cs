using Link;
using System.Collections.Generic;
using UnityEngine;

namespace HuyThanh.Cooking.Burrito
{
    public class TissueBox : ItemIdleBase
    {
        [SerializeField] Tissue tissue;

        private List<Tissue> tissues = new List<Tissue>();

        public bool CheckDone()
        {
            for (int i = tissues.Count - 1; i >= 0; i--)
            {
                if (tissues[i].IsState(Tissue.State.Done) == false)
                {
                    return false;
                }
            }
            return true;
        }

        public override void OnClickDown()
        {
            Tissue t = Instantiate(tissue, LevelControl.Ins.GetPoint(), Quaternion.identity);
            tissues.Add(t);
            t.gameObject.SetActive(true);
            LevelControl.Ins.SetItemMoving(t);
        }

        public void DeactiveCollider()
        {
            this.GetComponent<Collider2D>().enabled = false;
        }
    }
}
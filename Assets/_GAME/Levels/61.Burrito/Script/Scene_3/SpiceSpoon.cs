using DG.Tweening;
using Link;
using UnityEngine;

namespace HuyThanh.Cooking.Burrito
{
    public class SpiceSpoon : ItemIdleBase
    {
        [SerializeField] Transform spiceTF;

        public void SpoonTake()
        {
            if (Vector3.Distance(spiceTF.localScale, Vector3.one) < 0.01f)
            {
                spiceTF.DOScale(Vector3.one * 0.75f, 0.1f);
            }

            else
            {
                spiceTF.DOScale(Vector3.zero, 0.1f);
            }
        }

        public void SpoonUntake()
        {
            if (Vector3.Distance(spiceTF.localScale, Vector3.one * 0.75f) < 0.01f)
            {
                spiceTF.DOScale(Vector3.one, 0.1f);
            }

            else
            {
                spiceTF.DOScale(Vector3.one * 0.75f, 0.1f);
            }
        }
    }

}

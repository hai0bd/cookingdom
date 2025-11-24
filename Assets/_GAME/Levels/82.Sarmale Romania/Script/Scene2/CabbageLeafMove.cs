using DG.Tweening;
using Link;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HuyThanh.Cooking.SarmaleRomania
{
    public class CabbageLeafMove : MonoBehaviour
    {
        [SerializeField] ItemAlpha leafItemAlpha;

        public void OnMove()
        {
            this.transform.DOMove(transform.position + Vector3.down, 0.5f);
            leafItemAlpha.DoAlpha(0, 0.5f);
        }


        [Button]
        public void SetUp()
        {
            leafItemAlpha = GetComponent<ItemAlpha>();
        }
    }
}
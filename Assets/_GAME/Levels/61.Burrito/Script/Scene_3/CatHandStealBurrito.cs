using DG.Tweening;
using Link;
using Link.Cooking;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HuyThanh.Cooking.Burrito
{
    public class CatHandStealBurrito : MonoBehaviour
    {
        [SerializeField] Transform burritoTransform, doneTransform;
        [SerializeField] Vector3 targetRotation, doneRotation;

        [SerializeField] Emoji emoji;

        private Vector3 saveRotation, savePosition;

        public Transform TF => transform;
        private void Start()
        {
            savePosition = TF.position;
            saveRotation = TF.eulerAngles;
        }

        [Button]
        public void NewSavePoint()
        {
            savePosition = TF.position;
        }

        [Button]

        public void Reset()
        {
            TF.position = savePosition;
            TF.eulerAngles = saveRotation;
        }

        [Button]
        public void StealBurrito()
        {
            TF.DORotate(targetRotation, .3f);
            TF.DOMove(burritoTransform.position, .3f);

            DOVirtual.DelayedCall(.3f, () =>
            {
                TF.DORotate(doneRotation, 0.3f);
                TF.DOMove(doneTransform.position, 0.3f);
                emoji.PlayEmoji(Emoji.EmojiType.Lovely);
                SoundControl.Ins.PlayFX(Fx.HappyCatSound);
            });
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out Crust crust) && crust.TF == burritoTransform)
            {
                crust.TF.SetParent(TF);
                crust.TF.DOKill();
                crust.TF.DOLocalMove(Vector3.zero, .1f);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(burritoTransform.position, .1f);
            Gizmos.DrawWireSphere(doneTransform.position, .1f);
        }
    }
}
using DG.Tweening;
using System.Collections;
using UnityEngine;

namespace Satisgame
{
    public class EmojiControl : MonoBehaviour
    {
        private static readonly int AnimPositive = Animator.StringToHash("Positive");
        private static readonly int AnimNegative = Animator.StringToHash("Negative");
        public Animator spriteAnimator;

        public Transform scaleTransform;
        public float durationShow = 0.25f;
        public float durationHold = 2f;
        public float durationHide = 0.25f;
        private Sequence _sequenceShowEmoji;

        public AudioSource audioSource;
        public AudioClip sfxPositive;
        public AudioClip sfxNegative;

        private Vector3 _originScale;

        private void Start()
        {
            _originScale = scaleTransform.localScale;
            if (_originScale == Vector3.zero) _originScale = Vector3.one; // default
            scaleTransform.localScale = Vector3.zero;
        }

        public void HideEmoji()
        {
            if (_sequenceShowEmoji.IsActive())
            {
                _sequenceShowEmoji.Kill();
                _sequenceShowEmoji = DOTween.Sequence().Append(scaleTransform.DOScale(Vector3.zero, durationHide / 2f).SetEase(Ease.OutQuad)).SetUpdate(true).Play();
            }
        }

        public void ShowPositive(float delay = 0f)
        {
            if (_sequenceShowEmoji.IsActive()) _sequenceShowEmoji.Complete();
            spriteAnimator.Play(AnimPositive);
            _sequenceShowEmoji = DOTween.Sequence();
            if (delay > 0) _sequenceShowEmoji.AppendInterval(delay);
            _sequenceShowEmoji
                .Append(scaleTransform.DOScale(_originScale, durationShow).SetEase(Ease.OutBack))
                .AppendCallback(PlaySfx)
                .AppendInterval(durationHold)
                .Append(scaleTransform.DOScale(Vector3.zero, durationHide).SetEase(Ease.InBack))
                .Play();

            void PlaySfx()
            {
                if (audioSource) audioSource.PlayOneShot(sfxPositive);
            }
        }

        public void ShowNegative(float delay = 0f)
        {
            if (_sequenceShowEmoji.IsActive()) _sequenceShowEmoji.Complete();
            spriteAnimator.Play(AnimNegative);
            _sequenceShowEmoji = DOTween.Sequence();
            if (delay > 0) _sequenceShowEmoji.AppendInterval(delay);
            _sequenceShowEmoji
                .Append(scaleTransform.DOScale(_originScale, durationShow).SetEase(Ease.OutBack))
                .AppendCallback(PlaySfx)
                .AppendInterval(durationHold)
                .Append(scaleTransform.DOScale(Vector3.zero, durationHide).SetEase(Ease.InBack))
                .Play();

            void PlaySfx()
            {
                if (audioSource) audioSource.PlayOneShot(sfxNegative);
            }
        }
    }
}
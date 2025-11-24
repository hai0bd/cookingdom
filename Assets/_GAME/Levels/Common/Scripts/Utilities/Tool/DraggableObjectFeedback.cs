using MoreMountains.NiceVibrations;
using System;
using System.Collections;
using UnityEngine;

namespace Utilities
{
    public class DraggableObjectFeedback : MonoBehaviour
    {
        public DraggableRigidbody2D box;
        public AudioSource audioSource;
        public AudioClip sfxStartDrag;
        public float sfxStartDragVolumeScale = 1f;
        public AudioClip sfxEndDrag;
        [ValueCurve(true, true)] public AnimationCurve sfxEndDragVolumeBySpeed = AnimationCurve.Linear(0, 0, 1, 1);
        public HapticTypes hapticStartDrag;
        public HapticTypes hapticEndDrag;

        public AudioClip sfxCollide;
        public float[] collideSpeedStartTriggerHaptics;
        public HapticTypes[] hapticCollides;
        [ValueCurve(true, true)] public AnimationCurve collideSpeedToVolumeMul = AnimationCurve.Linear(0, 0, 1, 1);
        public Vector2 sfxCollideRandomPitch = Vector2.one;
        private float _lastCollideSpeedAtHitPoint;

        private void Start()
        {
            box.OnStartDrag.AddListener(OnStartDrag);
            box.OnEndDrag.AddListener(OnEndDrag);
        }

        private void OnStartDrag()
        {
            if (audioSource != null) audioSource.PlayOneShot(sfxStartDrag, sfxStartDragVolumeScale);
            MMVibrationManager.Haptic(hapticStartDrag);
        }

        private void OnEndDrag()
        {
            if (audioSource != null) audioSource.PlayOneShot(sfxEndDrag, sfxEndDragVolumeBySpeed.Evaluate(box.Rb.velocity.magnitude));
            MMVibrationManager.Haptic(hapticEndDrag);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            _lastCollideSpeedAtHitPoint = (collision.relativeVelocity + collision.otherRigidbody.GetPointVelocity(collision.contacts[0].point)).magnitude;
            if (audioSource != null)
            {
                audioSource.pitch = UnityEngine.Random.Range(sfxCollideRandomPitch.x, sfxCollideRandomPitch.y);
                audioSource.PlayOneShot(sfxCollide, collideSpeedToVolumeMul.Evaluate(_lastCollideSpeedAtHitPoint));
            }

            for (int i = collideSpeedStartTriggerHaptics.Length - 1; i >= 0; i--)
            {
                if (_lastCollideSpeedAtHitPoint > collideSpeedStartTriggerHaptics[i])
                {
                    MMVibrationManager.Haptic(hapticCollides[i]);
                    break;
                }
            }
        }
    }
}
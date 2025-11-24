using MoreMountains.NiceVibrations;
using System;
using System.Collections;
using UnityEngine;

namespace Utilities
{
    public class DraggableRigidbody2DJointFeedback : MonoBehaviour
    {
        public DraggableRigidbody2DJoint draggableObject;
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
        private float _lastCollideSpeedAtHitPoint;

        private void Start()
        {
            draggableObject.OnStartDrag.AddListener(OnStartDrag);
            draggableObject.OnEndDrag.AddListener(OnEndDrag);
        }

        private void OnStartDrag(DraggableRigidbody2DJoint body)
        {
            if (audioSource != null) audioSource.PlayOneShot(sfxStartDrag, sfxStartDragVolumeScale);
            MMVibrationManager.Haptic(hapticStartDrag);
        }

        private void OnEndDrag(DraggableRigidbody2DJoint body)
        {
            if (audioSource != null) audioSource.PlayOneShot(sfxEndDrag, sfxEndDragVolumeBySpeed.Evaluate(draggableObject.Rb.velocity.magnitude));
            MMVibrationManager.Haptic(hapticEndDrag);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            _lastCollideSpeedAtHitPoint = (collision.relativeVelocity + collision.otherRigidbody.GetPointVelocity(collision.contacts[0].point)).magnitude;
            if (audioSource != null) audioSource.PlayOneShot(sfxCollide, collideSpeedToVolumeMul.Evaluate(_lastCollideSpeedAtHitPoint));

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
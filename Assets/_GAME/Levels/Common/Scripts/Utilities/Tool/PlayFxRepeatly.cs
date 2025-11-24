using MoreMountains.NiceVibrations;
using System.Collections;
using UnityEngine;

namespace Utilities
{
    public class PlayFxRepeatly : MonoBehaviour
    {
        public AudioSource audioSource;
        public Vector2 delayRange = Vector2.zero;
        public Vector2 intervalRange = Vector2.one;
        public ParticleSystem vfx;
        public ParticleSystem vfxLoop;
        public AudioClip clip;
        public float volume = 1f;
        public HapticTypes haptic = HapticTypes.None;
        public bool isUseScaledTime = true;
        public int numLoop = -1;

        private float CurrentTime => isUseScaledTime ? Time.time : Time.unscaledTime;
        private float _nextTimePlay;
        private int _countLoop;

        public void Play() => this.enabled = true;
        public void Stop() => this.enabled = false;

        private void OnEnable()
        {
            _nextTimePlay = CurrentTime + Random.Range(delayRange.x, delayRange.y);
            _countLoop = 0;
            vfxLoop.PlayIfExisted();
        }

        private void OnDisable()
        {
            vfxLoop.StopIfExisted();
        }

        public void Trigger()
        {
            _countLoop += 1;

            if (audioSource != null) audioSource.PlaySfx(clip, volume);
            MMVibrationManager.Haptic(haptic);
            vfx.PlayIfExisted();

            if (numLoop >= 0 && _countLoop >= numLoop)
            {
                this.enabled = false;
            }
            else
            {
                _nextTimePlay = CurrentTime + Random.Range(intervalRange.x, intervalRange.y);
            }
        }

        private void Update()
        {
            if (CurrentTime > _nextTimePlay)
            {
                Trigger();
            }
        }
    }
}
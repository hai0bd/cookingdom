using MoreMountains.NiceVibrations;
using System.Collections;
using UnityEngine;

namespace Utilities
{
    public class PlayFxOnTransformMove : MonoBehaviour
    {
        public Transform target;
        public float deltaDistanceTriggerFx = 0.1f;
        private float _deltaDistance;
        private Vector3 _lastPosition;

        public ParticleSystem vfx;
        public AudioSource audioSource;
        public AudioClip sfx;
        public Vector2 sfxRandomPitch = new Vector2(0.9f, 1.1f);
        public HapticTypes haptic;

        private void OnEnable()
        {
            _deltaDistance = 0;
            _lastPosition = target.position;
        }

        private void Update()
        {
            _deltaDistance += Vector3.Distance(_lastPosition, target.position);
            _lastPosition = target.position;

            if (_deltaDistance >= deltaDistanceTriggerFx)
            {
                _deltaDistance = 0;
                vfx.PlayIfExisted();
                audioSource.pitch = Random.Range(sfxRandomPitch.x, sfxRandomPitch.y);
                audioSource.PlayOneShot(sfx);
                MMVibrationManager.Haptic(haptic);
            }
        }
    }
}
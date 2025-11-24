using System.Collections;
using UnityEngine;

namespace Utilities
{
    public static class AudioSourceExtension
    {
        private const float PITCH_VARIANT_DEFAULT = 0.2f;
        public static void PlaySfx(this AudioSource audioSource, AudioClip clip, float volumeScale = 1f, float pitchVariant = PITCH_VARIANT_DEFAULT)
        {
            if (!audioSource || !clip) return;
            audioSource.pitch = Random.Range(1f - pitchVariant, 1f + pitchVariant);
            audioSource.PlayOneShot(clip, volumeScale);
        }
    }
}
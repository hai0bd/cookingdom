using DG.Tweening;
using MoreMountains.NiceVibrations;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

namespace Utilities
{
    public class DraggableObjectFx : MonoBehaviour
    {
        public DraggableObject draggableObject;
        public AudioSource audioSource;
        public AudioClip sfxStartDrag;
        public HapticTypes hapticStartDrag;
        public AudioClip sfxEndDrag;
        public HapticTypes hapticEndDrag;

        private void Awake()
        {
            if (draggableObject == null)
            {
                draggableObject = GetComponent<DraggableObject>();
            }
            draggableObject.OnStartDrag.AddListener(OnStartDrag);
            draggableObject.OnEndDrag.AddListener(OnEndDrag);
        }

        private void OnStartDrag()
        {
            audioSource.PlaySfx(sfxStartDrag);
            MMVibrationManager.Haptic(hapticStartDrag);
        }

        private void OnEndDrag()
        {
            audioSource.PlaySfx(sfxEndDrag);
            MMVibrationManager.Haptic(hapticEndDrag);
        }
    }
}
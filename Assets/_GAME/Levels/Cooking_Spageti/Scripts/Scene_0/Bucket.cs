using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link.Cooking.Spageti
{
    public class Bucket : MonoBehaviour
    {
        [SerializeField] Animation anima;
        [SerializeField] AudioClip clip;
        public void PlayAnim()
        {
            anima.Stop();
            anima.Play();
            SoundControl.Ins.PlayFX(clip);
        }
    }
}
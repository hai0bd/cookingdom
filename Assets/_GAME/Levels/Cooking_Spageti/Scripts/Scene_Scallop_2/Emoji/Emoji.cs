using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link.Cooking
{
    public class Emoji : MonoBehaviour
    {
        public enum EmojiType { Angry, Lovely, Dizz, Sneeze, Scare, Funny  }
        [SerializeField] ItemAlpha itemAlpha;
        [SerializeField] GameObject[] icons;
        [SerializeField] AudioClip[] audioClip;

        void Awake()
        {
            itemAlpha.SetAlpha(0);
            gameObject.SetActive(false);
        }

        public void PlayEmoji(EmojiType emojiType, float time = 1.5f)
        {
            for (int i = 0; i < icons.Length; i++)
            {
                icons[i].SetActive(i == (int)emojiType);
            }
            if (audioClip[(int)emojiType] != null)
            {
                SoundControl.Ins.PlayFX(audioClip[(int)emojiType]);
            }

            gameObject.SetActive(true);
            itemAlpha.DoAlpha(1, 0.5f);

            CancelInvoke(nameof(StopEmoji));
            CancelInvoke(nameof(OnDespawn));
            Invoke(nameof(StopEmoji), time);
        }

        public void StopEmoji()
        {
            itemAlpha.DoAlpha(0, 0.5f);
            Invoke(nameof(OnDespawn), 0.5f);
        } 

        private void OnDespawn()
        {
            gameObject.SetActive(false);
        }



    }
}
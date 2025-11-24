using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace Link.Cooking.Lobster
{
    public class Boat : MonoBehaviour
    {
        [SerializeField] Transform target;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private SpriteRenderer[] fishes;
        [SerializeField] private AudioClip sfxCorrect, sfxWrong, sfxComplete;

        int count = 0;

        public void OnCatchFish(int index)
        {
            transform.DOPunchScale(Vector3.one * .1f, .3f);

            if(index <= 4)
            {
                count++;
                fishes[index].gameObject.SetActive(true);
                AudioManager.PlaySFX(sfxCorrect);

                if (count >= 5)
                {
                    AudioManager.PlaySFX(sfxComplete);
                    this.WaitToDo(OnComplete, .5f);
                }
            }
            else
            {
                //LevelControl.Ins.LoseFullHeart(transform.position);
                AudioManager.PlaySFX(sfxWrong);
            }
        }
        private void OnComplete()
        {

            //LevelMakeSushi.Instance.OnCompleteCatchFish();
            // transform.SetParent(null);

            // spriteRenderer.sortingOrder = 99;
            // for (int i = 0; i < fishes.Length; i++)
            // {
            //     fishes[i].sortingOrder = 98;
            // }

            // transform.DOMove(target.position, 1.4f).OnComplete(() =>
            // {
            //     transform.DOPunchScale(Vector3.one * .1f, .3f).OnComplete(() =>
            //     {
            //         gameObject.SetActive(false);
            //     });
            // });
        }
    }
}


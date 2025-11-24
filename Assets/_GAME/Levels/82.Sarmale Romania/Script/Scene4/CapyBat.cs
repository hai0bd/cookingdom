using DG.Tweening;
using Link;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

namespace HuyThanh.Cooking.SarmaleRomania
{
    public class CapyBat : ItemIdleBase
    {
        [SerializeField] GameObject bat, batSteal;
        [SerializeField] ParticleSystem smokeVFX1, smokeVFX2;

        [SerializeField] Transform rib;

        [Button("Start Capy Steal")]
        public void StartCapySteal()
        {
            StartCoroutine(WaitForCapyShow());
        }

        IEnumerator WaitForCapyShow()
        {
            bat.transform.DOMove(Vector3.up * 3.6f, 1f);
            yield return WaitForSecondCache.Get(1f);
            bat.SetActive(false);
            smokeVFX1.Play();

            yield return WaitForSecondCache.Get(0.5f);
            smokeVFX2.Play();
            batSteal.SetActive(true);
            rib.SetParent(batSteal.transform);
            yield return WaitForSecondCache.Get(1f);
            batSteal.transform.DOMoveY(7.5f, 1f);
        }
    }
}
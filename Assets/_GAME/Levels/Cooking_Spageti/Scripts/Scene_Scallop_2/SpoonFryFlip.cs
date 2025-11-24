using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Link.Cooking.Spageti;
using UnityEngine;

namespace Link.Cooking
{

    public class SpoonFryFlip : MonoBehaviour
    {
        Seafood seafood;

        private void OnTriggerEnter2D(Collider2D other)
        {

            if (other.TryGetComponent<Seafood>(out Seafood seafood))
            {
                if (seafood.IsState(Seafood.State.Ripe))
                {
                    seafood.Flip();
                }
                else 
                if (seafood.IsState(Seafood.State.Riped))
                {
                    if (this.seafood != null) return;
                    this.seafood = seafood;
                    seafood.ChangeState(Seafood.State.InSpoon);
                    seafood.TF.SetParent(transform);
                    seafood.TF.DOLocalMove(Vector3.zero, 0.2f);
                }
            }else
            if(other.TryGetComponent<SeafoodPoint>(out SeafoodPoint seafoodPoint) && seafoodPoint.OnTake(this.seafood))
            {
                    this.seafood = null;
            }
        }
    }
}
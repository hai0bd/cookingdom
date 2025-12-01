using DG.Tweening;
using Hai.Cooking.NewTest;
using Link;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PouringAnim : MonoBehaviour
{
    enum Type { Bowl, Spice}

    [SerializeField] private SpriteRenderer spriteIdle;
    [SerializeField] private SpriteRenderer spritePouring;
    [SerializeField] private Type type;
    [SerializeField] private float offsetY = 0.5f;
    [SerializeField] private float rotateAngle = 45f;
    [SerializeField] private float pouringDuration = 1f;
    [SerializeField] private float delayAfterAnim = 1f;

    public void PutUp(Fx fx)
    {
        transform.DOMoveY(transform.position.y + offsetY, pouringDuration);
        transform.DORotate(new Vector3(0, 0, rotateAngle), pouringDuration).OnComplete(() =>
        {
            spriteIdle.gameObject.SetActive(false);
            spritePouring.gameObject.SetActive(true);
            SoundControl.Ins.PlayFX(fx);

            if (type == Type.Bowl)
            {
                spritePouring.transform.DOMove(Vector3.zero, pouringDuration).OnComplete(ChangeSprite);
            }
            //else SoundControl.Ins.PlayFX(Fx.OilPour);

        });
    }

    private void ChangeSprite()
    {
            spritePouring.DOFade(0, .3f).OnComplete(() => 
            {
                spritePouring.gameObject.SetActive(false); 
            });
    }

    public void PutDown()
    {
        if(type == Type.Spice)
        {
            spriteIdle.gameObject.SetActive(true);
            spritePouring.gameObject.SetActive(false);
        }
    }
}

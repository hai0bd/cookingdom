using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PouringAnim : MonoBehaviour
{
    enum Type { Bowl, Spice}

    [SerializeField] private GameObject spriteIdle;
    [SerializeField] private GameObject spritePouring;
    [SerializeField] private Type type;
    [SerializeField] private float offsetY = 0.5f;
    [SerializeField] private float rotateAngle = 45f;
    [SerializeField] private float pouringDuration = 1f;
    [SerializeField] private float delayAfterAnim = 1f;

    public void PutUp()
    {
        transform.DOMoveY(transform.position.y + offsetY, pouringDuration);
        transform.DORotate(new Vector3(0, 0, rotateAngle), pouringDuration).OnComplete(() =>
        {
            spriteIdle.SetActive(false);
            spritePouring.SetActive(true);
            if (type == Type.Bowl)
            {
                spritePouring.transform.DOMove(Vector3.zero, pouringDuration).OnComplete(() => { spritePouring.SetActive(false); });
            }

        });
    }

    public void PutDown()
    {
        if(type == Type.Spice)
        {
            spriteIdle.SetActive(true);
            spritePouring.SetActive(false);
        }
    }
}

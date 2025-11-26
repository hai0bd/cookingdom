using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PouringAnim : MonoBehaviour
{
    [SerializeField] private GameObject spritePouring;
    [SerializeField] private float offsetY = 0.5f;
    [SerializeField] private float rotateAngle = 45f;
    [SerializeField] private float pouringDuration = 1f;
    [SerializeField] private float delayAfterAnim = 1f;

    public void PlayPouringAnimation()
    {
        transform.DOMoveY(transform.position.y + offsetY, pouringDuration);
        transform.DORotate(new Vector3(0, 0, rotateAngle), pouringDuration);
    }
}

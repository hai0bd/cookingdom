using UnityEngine;
using DG.Tweening;
using Link;

public class ItemInBat : ItemMovingBase
{
    public enum TypeItem { Vegar, Meat }

    [Header("Hiệu ứng rơi")]
    private float fallDistance = 3f;
    private float fallDuration = 1f;
    public float rotationSpeed = 360f;
    private float scaleDuration = 1f; // Thời gian scale về 1

    [SerializeField] Transform TFSpriterender;
    [SerializeField] Transform Tf_JUmpvegar;
    [SerializeField] Transform Tf_JUmpMeat;
    [SerializeField ] MinigameControl Minigame;

    public TypeItem itemType;

    public void Drop()
    {
        // Bỏ parent để rơi tự do
        transform.SetParent(null);

        // Bật collider
        collider.enabled = false;

        // Vị trí rơi
        Vector3 targetPos = transform.position + Vector3.down * fallDistance;

        // Rơi + bounce nhẹ
        transform
            .DOMove(targetPos, fallDuration)
            .SetEase(Ease.InQuad)
            .OnComplete(() =>
            {
                transform.DOMoveY(transform.position.y + 0.3f, 0.2f)
                    .SetEase(Ease.OutQuad);
            });

        // Xoay khi rơi
        transform
            .DORotate(new Vector3(0, 0, rotationSpeed), fallDuration, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear);

        // Scale về (0.4, 0.4, 0.4)
        TFSpriterender.DOScale(new Vector3(0.4f, 0.4f, 0.4f), scaleDuration)
            .SetEase(Ease.OutBack);
    }

    public override void OnClickDown()
    {
        base.OnClickDown();
        

        Transform targetJump = null;

        if (itemType == TypeItem.Vegar)
            targetJump = Tf_JUmpvegar;
        else if (itemType == TypeItem.Meat)
            targetJump = Tf_JUmpMeat;

        if (targetJump != null)
        {
            float jumpPower = 2f;  // độ cao nhảy
            int numJumps = 1;        // số lần nhảy
            float duration = 1f;   // thời gian nhảy

            transform
                .DOJump(targetJump.position, jumpPower, numJumps, duration)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    // Sau khi nhảy xong, có thể destroy hoặc ẩn
                    OnDone();   
                    if(itemType == TypeItem.Vegar)
                    {
                        Minigame.CheckVegarWin();
                    } 
                    if(itemType == TypeItem.Meat)
                    {
                        Minigame.CheckMeatWin();
                    }

                });
        }
    }
}

using Link;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector.Editor;

public class MinigameControl : MonoBehaviour
{
    [SerializeField] CameraControl Camcontrol;
    [SerializeField] GameObject MinigameObject;
    [SerializeField] GameObject[] Itemblack;
    [SerializeField] GameObject[] ItemInBatonSence1;
    [SerializeField] GameObject[] ItemInBatOnMiniGame;
    [Header("Changeminigame")]
    [SerializeField] private Image batImage; // Sprite con dơi
    [SerializeField] private float scaleUpSize = 100f; // Kích thước phóng to
    [SerializeField] private float scaleUpTime = 2f; // Thời gian phóng to
    private bool hasMoved = false;
    int itemvergarwin = 0;
    int itemMeatwin = 0;
    public int Bathitcount =0;
    public bool startGame=false;    
    public void BackScrenActive()
    {  // Reset trạng thái ban đầu
        batImage.rectTransform.localScale = Vector3.zero;
        batImage.gameObject.SetActive(true);

        // Con dơi to dần
        batImage.rectTransform.DOScale(scaleUpSize, scaleUpTime)
            .SetEase(Ease.InQuad)
            .OnComplete(() =>
            {
                if(!CheckdoneShootBat())
                {
                    Camcontrol.TF.position += new Vector3(8.6f, 0, 0);
                    MinigameObject.SetActive(true);
                }
                else if(CheckdoneShootBat())
                {
                    Camcontrol.TF.position -= new Vector3(8.6f, 0, 0);
                }

             
              

                foreach (var item in Itemblack)
                {
                    item.SetActive(false);
                }

                // Sau đó thu nhỏ lại
                batImage.rectTransform.DOScale(0f, scaleUpTime)
                    .SetEase(Ease.OutQuad)
                    .OnComplete(() =>
                    {
                        batImage.gameObject.SetActive(false);
                    });
            });
    }
    public void MoveInMiniGame()
    {
        if (hasMoved) return; // Nếu đã chạy rồi thì thoát luôn

        hasMoved = true; // Đánh dấu là đã chạy

        BackScrenActive();

        StartCoroutine(WaitforStart());

    }
    IEnumerator WaitforStart()
    {
        yield return new WaitForSeconds(4f);

        startGame = true;
    }

    public void WinMiniGame()
    {
        BackScrenActive();
      
      
        foreach (var item in ItemInBatonSence1)
        {
            item.SetActive(true);

        }

      StartCoroutine(WaitforEnd());

    }
    IEnumerator WaitforEnd()
    {
        yield return new WaitForSeconds(4f);

        MinigameObject.SetActive(false);
    }

    public void LoseMiniGame()
    {
        LevelControl.Ins.LoseGame(0.5f);


    }
    public void CheckVegarWin()
    {
        itemvergarwin++;
        if(itemMeatwin ==1 && itemvergarwin== 2)
        {
            WinMiniGame();  
        }

    }
    public void CheckMeatWin()
    {
        itemMeatwin++;
        if (itemMeatwin == 1 && itemvergarwin == 2)
        {
            WinMiniGame();
        }

    }

   public bool CheckdoneShootBat()
    {
      
        if(Bathitcount >= 3)
        {
            return true;

        }

        return false;
    }

   public void CHeckActiveBoxItemOnBat()
    {
        Bathitcount++;
        if(Bathitcount >= 3)
        {
            foreach(var item in ItemInBatOnMiniGame)
            {
                item.gameObject.GetComponent<CircleCollider2D>().enabled = true;
            } 
        }
    }

   


}

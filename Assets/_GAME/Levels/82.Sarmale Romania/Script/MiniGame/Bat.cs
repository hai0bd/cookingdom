using UnityEngine;

public class Bat : MonoBehaviour
{
    [SerializeField] GameObject batImage;
    [SerializeField] ItemInBat itemInBat; // Script thay vì GameObject
    [SerializeField] BoxCollider2D boxCollider;
    [SerializeField] MinigameControl minigame ;    

    public void OnHitBySpoon()
    {
        batImage.SetActive(false);
        if (itemInBat != null)
        {
            itemInBat.gameObject.SetActive(true);
            itemInBat.Drop();
            minigame.CHeckActiveBoxItemOnBat();
            boxCollider.enabled = false;    
        }
    }
}

using UnityEngine;

public class SpoonBullet : MonoBehaviour
{
    public SpoonManager manager;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Nếu va chạm với Bat
        Bat bat = collision.GetComponent<Bat>();
        if (bat != null)
        {
            bat.OnHitBySpoon(); // Gọi hàm bên Bat
            Destroy(gameObject); // Huỷ viên thìa
            manager.LoseLife();
            return;
        }

        // Nếu va chạm với vùng trượt
        MissZoneTrigger missZone = collision.GetComponent<MissZoneTrigger>();
        if (missZone != null)
        {
            manager.LoseLife();
            Destroy(gameObject);
        }
    }
}

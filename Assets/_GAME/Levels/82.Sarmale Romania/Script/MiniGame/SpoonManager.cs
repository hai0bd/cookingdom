using UnityEngine;
using System.Collections;

public class SpoonManager : MonoBehaviour
{
    [Header("Mạng")]
    public GameObject[] spoonLives; // 5 icon thìa đại diện cho mạng
    private int currentLives;

    [Header("Shooter")]
    public Transform spoonSpawnPoint; // vị trí spawn thìa mới
    public float rotateSpeed = 45f;
    public float rotateLimit = 45f;
    public GameObject spoonPrefab; // prefab thìa
    public float shootForce = 10f;
    public float spawnDelay = 0.5f; // thời gian delay sinh thìa mới

    private GameObject currentSpoon;
    private float angle = 0;
    private bool rotatingRight = true;
    private bool isShooting = false;
    [Header("End")]
   
    [SerializeField ] MinigameControl minigame;
   

    void Start()
    {
        currentLives = spoonLives.Length;
        SpawnNewSpoon();
    }

    void Update()
    {
        if (currentSpoon != null && !isShooting && minigame.startGame )
        {
            RotateSpoon();

            if (Input.GetMouseButtonDown(0) && !minigame.CheckdoneShootBat() )
            {
                StartCoroutine(ShootCurrentSpoon());
            }
        }
    }

    void RotateSpoon()
    {
        if (rotatingRight)
        {
            angle += rotateSpeed * Time.deltaTime;
            if (angle >= rotateLimit) rotatingRight = false;
        }
        else
        {
            angle -= rotateSpeed * Time.deltaTime;
            if (angle <= -rotateLimit) rotatingRight = true;
        }

        currentSpoon.transform.localRotation = Quaternion.Euler(0, 0, angle);
    }

    IEnumerator ShootCurrentSpoon()
    {
        isShooting = true;

        Rigidbody2D rb = currentSpoon.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.velocity = currentSpoon.transform.up * shootForce;
        }

        // Bắt đầu kiểm tra va chạm trong SpoonBullet
        currentSpoon.GetComponent<SpoonBullet>().manager = this;

        currentSpoon = null;

        yield return new WaitForSeconds(spawnDelay);
        if (currentLives > 0)
        {
            SpawnNewSpoon();
        }

        isShooting = false;
    }

    void SpawnNewSpoon()
    {
        if (currentLives <= 0) return;

        angle = 0;
        rotatingRight = true;

        currentSpoon = Instantiate(spoonPrefab, spoonSpawnPoint.position, Quaternion.identity, transform);
        currentSpoon.GetComponent<Rigidbody2D>().isKinematic = true;

        // Gán SpoonManager vào SpoonBullet của prefab
        SpoonBullet bulletScript = currentSpoon.GetComponent<SpoonBullet>();
        if (bulletScript != null)
        {
            bulletScript.manager = this;
        }
    }


    public void LoseLife()
    {
        if (currentLives > 0)
        {
            currentLives--;
           Destroy( spoonLives[currentLives]);
        }

        if (currentLives <= 0&& !minigame.CheckdoneShootBat())
        {
            minigame.LoseMiniGame();    
        }
    }
}

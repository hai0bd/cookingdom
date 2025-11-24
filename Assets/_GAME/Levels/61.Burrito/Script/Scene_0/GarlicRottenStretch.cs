using Link;
using UnityEngine;

namespace HuyThanh.Cooking.Burrito
{
    public class GarlicRottenStretch : MonoBehaviour
    {
        [SerializeField] private float maxDistanceAllowed = 5f;

        [SerializeField] private Transform bottomAnchor;
        [SerializeField] private float minScale = 1f;
        [SerializeField] private float maxScale = 3f;
        [SerializeField] private float stretchFactor = 1f;
        [SerializeField] Animation anim;
        [SerializeField] string animStretch;

        [SerializeField] Transform levelStep_0;
        [SerializeField] GarlicRotten garlicRotten;
        [SerializeField] ParticleSystem vfxRottenSteam;

        private Camera cam;
        private Vector3 originalScale;
        private bool isDragging = true;

        void Start()
        {
            cam = Camera.main;
            originalScale = transform.localScale;
        }

        void OnMouseDrag()
        {
            if (isDragging == false)
            {
                return;
            }
            Vector3 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = bottomAnchor.position.z;

            Vector3 dir = mouseWorld - bottomAnchor.position;

            if (dir.y <= 0 || dir.x <= 0)
            {
                ResetObject();
                return;
            }

            float distance = dir.magnitude;

            if (distance > maxDistanceAllowed)
            {
                anim.Play(animStretch);
                vfxRottenSteam.Play();
                isDragging = false;
                transform.localScale = originalScale;
                transform.rotation = Quaternion.identity;
                transform.position = bottomAnchor.position + dir / 2f;
                garlicRotten.enabled = true;
                garlicRotten.OnSave(0.1f);
                this.transform.SetParent(levelStep_0);
                this.enabled = false; /// tat script
                SoundControl.Ins.PlayFX(Fx.Click);
                return;
            }

            float scaleY = Mathf.Clamp(1 + distance * stretchFactor, minScale, maxScale);
            transform.localScale = new Vector3(originalScale.x, scaleY, originalScale.z);

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);

            Vector3 midPoint = bottomAnchor.position + dir / 2f;
            transform.position = midPoint;
        }

        void OnMouseUp()
        {
            if (isDragging)
            {
                ResetObject();
            }
        }

        private void ResetObject()
        {
            anim.Play(animStretch);
            transform.localScale = originalScale;
            transform.rotation = Quaternion.identity;
            transform.position = bottomAnchor.position;
        }
    }
}

using Link;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HuyThanh.Cooking.SarmaleRomania
{
    public class CabbageLeafStretch : ItemIdleBase
    {
        public enum DirectionType
        {
            Left,
            Right,
            Up,
            Down,
        }

        [SerializeField] DirectionType direction;
        [SerializeField] Animation anim;
        [SerializeField] Cabbage cabbage;
        [SerializeField] CabbageMoveTrash cabbageMoveTrash;
        [SerializeField] SpriteMask spriteMask;
        [SerializeField] float maxScale = 1f;
        [SerializeField] float minAngle, maxAngle;

        private Vector3 savePoint;
        private Vector3 firstPos, mousePos;
        private bool isClick = false;
        private float firstZRotation = 0f;

        private void OnEnable()
        {
            savePoint = this.TF.position;
            GetComponent<Collider2D>().enabled = true;
        }

        public override void OnClickDown()
        {
            base.OnClickDown();
            isClick = true;
            firstPos = mousePos = LevelControl.Ins.GetPoint();
        }

        public bool IsCorrectDirection(Vector3 currentMousePos)
        {
            switch (direction)
            {
                case DirectionType.Left:
                    if (currentMousePos.x > mousePos.x)
                        return false;
                    break;
                case DirectionType.Right:
                    if (currentMousePos.x < mousePos.x)
                        return false;
                    break;
                case DirectionType.Up:
                    if (currentMousePos.y < mousePos.y)
                        return false;
                    break;
                case DirectionType.Down:
                    if (currentMousePos.y > mousePos.y)
                        return false;
                    break;
            }
            return true;
        }

        private void LateUpdate()
        {
            if (isClick && Input.GetMouseButtonUp(0))
            {
                isClick = false;
                ResetObject();
            }

            if (isClick)
            {
                Vector3 mouseCurrentPos = LevelControl.Ins.GetPoint();

                if (!IsCorrectDirection(mouseCurrentPos))
                {
                    isClick = false;
                    ResetObject();
                    return;
                }

                float scaleRate = Vector3.Distance(mouseCurrentPos, firstPos);

                if (scaleRate > maxScale)
                {
                    isClick = false;
                    OnDoneStretch();
                    return;
                }

                cabbage.PlayAnim();
                TF.rotation = Quaternion.Euler(Vector3.forward * Mathf.Clamp(Vector2.SignedAngle(Vector3.up, mouseCurrentPos - firstPos), firstZRotation + minAngle, firstZRotation + maxAngle));
                TF.position += mouseCurrentPos - mousePos;
                scaleRate = Mathf.Clamp(scaleRate, 0, maxScale);
                TF.localScale = new Vector3(1 - scaleRate / 10, scaleRate + 1, 0);

                mousePos = mouseCurrentPos;
            }
        }

        public void ResetObject()
        {
            TF.localScale = Vector3.one;
            TF.position = savePoint;
            TF.rotation = Quaternion.Euler(Vector3.forward * firstZRotation);
        }

        public void OnDoneStretch()
        {
            this.enabled = false;

            if (spriteMask != null)
                spriteMask.enabled = false;
            TF.localScale = Vector3.one;
            anim.Play("Bounce");
            TF.SetParent(LevelControl.Ins.LevelStep.transform);

            cabbageMoveTrash.OnActive();
        }

        [Button("Find Set Up")]
        public void FindSetUp()
        {
            anim = TF.GetComponent<Animation>();
            cabbageMoveTrash = TF.GetComponent<CabbageMoveTrash>();
            cabbage = FindObjectOfType<Cabbage>();
            spriteMask = TF.GetComponentInChildren<SpriteMask>();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(TF.position, maxScale);
        }
    }
}
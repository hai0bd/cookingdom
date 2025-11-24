using DG.Tweening;
using UnityEngine;

namespace HuyThanh.Cooking.SarmaleRomania
{
    public class Bat : MonoBehaviour
    {

        public enum State { Sleep, Fly, WaitForDoneAnim }
        [SerializeField] State state;
        [SerializeField] Vector3 position;

        [SerializeField] GameObject batSleep;
        [SerializeField] GameObject batFly;

        [SerializeField] float flyTime = 4f;
        [SerializeField] Transform TFEndFlyPos;
        [SerializeField] Transform TFStartFlyPos;
        [SerializeField] Vector3 endFlyAngle, startFlyAngle;

        [SerializeField] Animation anim;

        private void Start()
        {
            anim.Play("BatSleep");
        }

        public void OnMouseDown()
        {
            if (state == State.Sleep)
            {
                ChangeState(State.Fly);
            }
        }

        private void ChangeState(State newState)
        {
            state = newState;

            switch (state)
            {
                case State.Sleep:
                    batFly.SetActive(false);
                    batSleep.SetActive(true);
                    anim.Play("BatSleep");
                    break;
                case State.Fly:
                    batFly.transform.position = position;
                    batFly.transform.eulerAngles = endFlyAngle;
                    batSleep.SetActive(false);
                    batFly.SetActive(true);

                    batFly.transform.DOMove(TFEndFlyPos.position, flyTime).OnComplete(() =>
                    {
                        DOVirtual.DelayedCall(Random.Range(3f, 6f), () => ChangeState(State.WaitForDoneAnim));
                    });
                    break;
                case State.WaitForDoneAnim:
                    batFly.transform.position = TFStartFlyPos.position;
                    batFly.transform.eulerAngles = startFlyAngle;
                    batFly.transform.DOMove(this.position, flyTime).OnComplete(() =>
                    {
                        ChangeState(State.Sleep);
                    });
                    break;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            //Gizmos.DrawLine(TFStartFlyPos.position, TFEndFlyPos.position);
            Gizmos.DrawWireSphere(TFStartFlyPos.position, 0.1f);
            Gizmos.DrawWireSphere(TFEndFlyPos.position, 0.1f);
        }
    }
}

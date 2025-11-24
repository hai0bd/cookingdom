using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;

public class BatManager : MonoBehaviour
{
    [System.Serializable]
    public class BatData
    {
        public Transform batTransform;
        [HideInInspector] public Tween moveTween;
        [HideInInspector] public Tween floatTween;
        [HideInInspector] public Transform currentWaypoint;
    }

    public List<BatData> bats;
    public List<Transform> waypoints;

    public float moveSpeed = 2f;
    public float stopTime = 3f;
    public float floatAmplitude = 0.3f;
    public float floatDuration = 0.8f;

    private List<Transform> tempAvailable = new List<Transform>();

    void Start()
    {
        if (waypoints.Count == 0 || bats.Count == 0) return;

        tempAvailable.AddRange(waypoints);

        foreach (var bat in bats)
        {
            if (tempAvailable.Count == 0) tempAvailable.AddRange(waypoints);

            Transform wp = tempAvailable[0];
            tempAvailable.RemoveAt(0);

            bat.currentWaypoint = wp;
            MoveBatTo(bat, wp.position);
        }
    }

    void MoveBatTo(BatData bat, Vector3 targetPos)
    {
        StopFloating(bat);
        bat.moveTween?.Kill();

        float distance = Vector3.Distance(bat.batTransform.position, targetPos);
        float duration = distance / moveSpeed;

        bat.moveTween = bat.batTransform
            .DOMove(targetPos, duration)
            .SetEase(Ease.InOutSine)
            .SetUpdate(UpdateType.Normal, true) // dùng unscaled time
            .OnComplete(() =>
            {
                StartFloating(bat);
                DOVirtual.DelayedCall(stopTime, () =>
                {
                    AssignNewWaypoint(bat);
                }, false) // false = dùng unscaled time
                .SetUpdate(true);
            });
    }

    void AssignNewWaypoint(BatData bat)
    {
        var usedWaypoints = bats.Select(b => b.currentWaypoint).ToList();
        tempAvailable.Clear();
        foreach (var wp in waypoints)
        {
            if (!usedWaypoints.Contains(wp))
                tempAvailable.Add(wp);
        }

        if (tempAvailable.Count == 0)
        {
            tempAvailable.AddRange(waypoints);
        }

        Transform newWp = tempAvailable[Random.Range(0, tempAvailable.Count)];
        bat.currentWaypoint = newWp;
        MoveBatTo(bat, newWp.position);
    }

    void StartFloating(BatData bat)
    {
        bat.floatTween?.Kill();
        bat.floatTween = bat.batTransform.DOMoveY(floatAmplitude, floatDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo)
            .SetRelative()
            .SetUpdate(true); // chạy khi TimeScale=0
    }

    void StopFloating(BatData bat)
    {
        bat.floatTween?.Kill();
    }
}

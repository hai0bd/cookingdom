using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace LawNguyen.CookingGame.LRisottoY
{
    public class Cleanwatersinkcontroll : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private WaterSink waterSink;
        [SerializeField] private Tapwater tapwater;

        [SerializeField] private List<Plate> plates;     // Inspector gán
        [SerializeField] private Rice rice;
        [SerializeField] private Glasssafron glasssafron;
        [SerializeField] private CHese cHese;
        [SerializeField] private List<DirtyendSence1> dirties; // Danh sách các dirty cuối

        private HashSet<UnityEngine.Object> completedObjects = new HashSet<UnityEngine.Object>();
        private int totalTasks = 0;
        private int completedCount = 0;
        private bool isReadyToClean = false;

        private int totalDirty = 3;
        private int cleanedDirtyCount = 0;

        void Awake()
        {
            // Tính tổng số object cần hoàn thành
            totalTasks = plates.Count;
            if (rice != null) totalTasks++;
            if (glasssafron != null) totalTasks++;
            if (cHese != null) totalTasks++;
            if (tapwater != null) totalTasks++;
            if (waterSink != null) totalTasks++;

            totalDirty = dirties.Count;

            Debug.Log($"[Cleanwatersinkcontroll] ✅ Tổng task cần hoàn thành: {totalTasks}");
            Debug.Log($"[Cleanwatersinkcontroll] ✅ Tổng dirty cần lau: {totalDirty}");
        }

        void OnEnable()
        {
            // Đăng ký sự kiện hoàn thành
            foreach (var plate in plates)
                if (plate != null)
                {
                    Debug.Log($"[OnEnable] Đăng ký event cho Plate: {plate.name}");
                    plate.OnCompleteEvent += OnCompleted;
                }

            if (rice != null)
            {
                Debug.Log($"[OnEnable] Đăng ký event cho Rice");
                rice.OnCompleteEvent += OnCompleted;
            }

            if (glasssafron != null)
            {
                Debug.Log($"[OnEnable] Đăng ký event cho Glasssafron");
                glasssafron.OnCompleteEvent += OnCompleted;
            }

            if (cHese != null)
            {
                Debug.Log($"[OnEnable] Đăng ký event cho CHese");
                cHese.OnCompleteEvent += OnCompleted;
            }

            if (tapwater != null)
            {
                Debug.Log($"[OnEnable] Đăng ký event cho Tapwater");
                tapwater.OnCompleteEvent += OnCompleted;
            }

            if (waterSink != null)
            {
                Debug.Log($"[OnEnable] Đăng ký event cho WaterSink");
                waterSink.OnCompletewatercoverEvent += OnCompleted;
            }

            // Đăng ký dirty end
            foreach (var dirty in dirties)
            {
                if (dirty != null)
                {
                    Debug.Log($"[OnEnable] Đăng ký event cho Dirty: {dirty.name}");
                    dirty.OnCompleteDirtyEvent += OnDirtyCleaned;
                }
            }
        }

        void OnDisable()
        {
            foreach (var plate in plates)
                if (plate != null) plate.OnCompleteEvent -= OnCompleted;

            if (rice != null) rice.OnCompleteEvent -= OnCompleted;
            if (glasssafron != null) glasssafron.OnCompleteEvent -= OnCompleted;
            if (cHese != null) cHese.OnCompleteEvent -= OnCompleted;
            if (tapwater != null) tapwater.OnCompleteEvent -= OnCompleted;
            if (waterSink != null) waterSink.OnCompletewatercoverEvent -= OnCompleted;

            foreach (var dirty in dirties)
                if (dirty != null) dirty.OnCompleteDirtyEvent -= OnDirtyCleaned;
        }

        private void OnCompleted(UnityEngine.Object obj)
        {
            if (obj == null)
            {
                Debug.LogWarning("[OnCompleted] ❌ obj null");
                return;
            }

            if (completedObjects.Contains(obj))
            {
                Debug.Log($"[OnCompleted] ⚠️ {obj.name} đã hoàn thành trước đó");
                return;
            }

            completedObjects.Add(obj);
            completedCount++;

            Debug.Log($"[OnCompleted] ✅ Hoàn thành: {obj.name} ({completedCount}/{totalTasks})");

            if (completedCount >= totalTasks && !isReadyToClean)
            {
                Debug.Log("[OnCompleted] 🎯 Tất cả task đã xong. Bắt đầu kiểm tra nước...");
                isReadyToClean = true;
                StartCoroutine(WaitUntilWaterIsOff());
            }
        }

        private IEnumerator WaitUntilWaterIsOff()
        {
            Debug.Log("[WaitUntilWaterIsOff] ⏳ Kiểm tra vòi nước và bồn...");

            while (waterSink != null && (waterSink.waterTapOn || waterSink.Iswater))
            {
                Debug.Log("[WaitUntilWaterIsOff] ⛔ Vòi nước vẫn đang bật hoặc nước chưa rút...");
                yield return new WaitForSeconds(0.5f);
            }

            Debug.Log("[WaitUntilWaterIsOff] ✅ Nước đã tắt và bồn trống. Bắt đầu rửa bồn.");
            StartFinalCleaning();
        }

        private void StartFinalCleaning()
        {
            Debug.Log("[StartFinalCleaning] 💧 Bắt đầu quá trình rửa bồn");
            waterSink?.StartCleaning();
        }

        private void OnDirtyCleaned(UnityEngine.Object dirtyObj)
        {
            if (dirtyObj == null || completedObjects.Contains(dirtyObj)) return;

            completedObjects.Add(dirtyObj);
            cleanedDirtyCount++;

            Debug.Log($"🧽 Lau sạch: {dirtyObj.name} ({cleanedDirtyCount}/{totalDirty})");

            if (cleanedDirtyCount >= totalDirty)
            {
                Debug.Log("[Cleanwatersinkcontroll] ✅ Đã lau xong tất cả dirty → gọi EndCleaning");
                EndFinalCleaning();
            }
        }

        private void EndFinalCleaning()
        {
            if (cleanedDirtyCount < totalDirty)
            {
                Debug.LogWarning($"[Cleanwatersinkcontroll] ❌ Chưa lau đủ dirty ({cleanedDirtyCount}/{totalDirty}) → KHÔNG gọi EndCleaning");
                return;
            }

            Debug.Log("[Cleanwatersinkcontroll] 🧼 Lau đủ dirty → gọi EndCleaning");
            waterSink?.EndCleaning();
        }

    }
}

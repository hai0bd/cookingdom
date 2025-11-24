using PaintIn3D;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace HoangHH
{
    public class DrawP3DController : H3MonoBehaviour
    {
        private enum ActionType
        {
            Draw = 0,
            Erase = 1
        }
        
        private enum TargetStartType
        {
            None = 0,
            Full = 1
        }

        [SerializeField] private ActionType actionType;
        [SerializeField] private TargetStartType targetStartType;
        
        [Title("P3D System")]
        [Header("Target")]
        [SerializeField] private P3dPaintableTexture p3dPaintableTexture;
        [SerializeField] private P3dChangeCounter p3dChangeCounter; 
        [SerializeField] private P3dChangeCounterEvent p3dChangeCounterEvent;
        [Header("Painter")]
        [SerializeField] private P3dPaintDecal p3dPaintDecal;

        [Title("Sound Controller")]
        [SerializeField] private TouchSpeedSoundController touchSpeedSoundController;
        
        [Title("Properties")] 
        [SerializeField] private Texture texture;
        [SerializeField] private Vector2 targetRatioRange;
        [SerializeField] private bool isReachOnce = true;
        [Tooltip("If true, Painter will draw on only TargetTexture, else draw a PaintableTexture")]
        [SerializeField] private bool isPainterTargetTexture;
        
        [Title("Event on Reach ChangeCounter")]
        [SerializeField] private UnityEvent onReachChangeCounter;
            
        private readonly P3dBlendMode _eraseMode = P3dBlendMode.SubtractiveSoft(new Vector4(0, 0, 0, 1));
        private readonly P3dBlendMode _drawMOde = P3dBlendMode.ReplaceOriginal(new Vector4(0, 0, 0, 1));
        
        public P3dPaintDecal P3dPaintDecal => p3dPaintDecal;
        
        private void Awake()
        {
            Setup();
        }

        public void ChangeDrawType()
        {
            p3dPaintDecal.BlendMode = actionType switch
            {
                ActionType.Draw => _drawMOde,
                ActionType.Erase => _eraseMode,
                _ => p3dPaintDecal.BlendMode
            };
        }
        
        [Tooltip("Set up before play and Awake, not work in realtime")]
        [Button]
        private void Setup()
        {
            #region Painter
            
            p3dPaintDecal.TargetTexture = isPainterTargetTexture ? p3dPaintableTexture : null;
            ChangeDrawType();

            #endregion

            #region Target

            p3dPaintableTexture.Texture = texture;
            p3dChangeCounter.Texture = texture;
            p3dPaintableTexture.Color = targetStartType switch
            {
                TargetStartType.None => new Color(1,1,1,0),
                TargetStartType.Full => Color.white,
                _ => p3dPaintableTexture.Color
            };

            #endregion

            #region Counter Event

            p3dChangeCounterEvent.Counters.Clear();
            p3dChangeCounterEvent.Counters.Add(p3dChangeCounter);
            p3dChangeCounterEvent.OnInside.RemoveAllListeners();
            p3dChangeCounterEvent.Range = new Vector2(targetRatioRange.x, targetRatioRange.y);
            p3dChangeCounterEvent.OnInside.AddListener(() =>
            {
                onReachChangeCounter.Invoke();
                if (isReachOnce)
                {
                    p3dChangeCounterEvent.enabled = false;
                }
            });

            #endregion
        }

        private void OnValidate()
        {
            ChangeDrawType();
        }
    }
}
// using System.Threading.Tasks;
// using DG.Tweening;
// using UnityEngine;
// using UnityEngine.UI;
// using Utilities;

// namespace TidyCooking.InGame
// {
//     public class UIHintTextRecipeItemStep : MonoBehaviour
//     {
//         public enum State
//         {
//             HiddenUnreached,
//             Hidden,
//             Revealed,
//             Passed
//         }

//         [System.Serializable]
//         public class GeneralConfig
//         {
//             [Header("Checkmark")]
//             public string txtAppendCheckmarkUndone = "<size=40><b>☐</b></size> ";
//             public string txtAppendCheckmarkDone = "<size=40><b>🗹</b></size> ";

//             [Header("Main Txt")]
//             public float durationChageColor = 0.5f;
//             public Color colorTxtHiddenUnreached = Color.grey;
//             public Color colorTxtHidden = Color.black;
//             public Color colorNotShow = Color.clear;
//             public Color colorRevealed = Color.black;
//             public Color colorPassed = Color.grey;
//         }

//         [SerializeField] private Text _txt;
//         [SerializeField] private Text _hiddenTxt;
//         [SerializeField] private LoopAnimTransformScaleSeparatedAxis _animHighlight;
//         [SerializeField] private Outline _outlineTxt;
//         public State CurrentState { get; private set; } = State.HiddenUnreached;
//         private GeneralConfig _generalConfig;
//         private string _text;
//         private Tween _tweenChangeText;

//         private CookingLevelSO _levelSO;
//         public int PhaseId { get; private set; }
//         public int StepId { get; private set; }

//         public async Task SetUp(GeneralConfig config, CookingLevelSO levelSO, int phaseId, int stepId, State initState)
//         {
//             _generalConfig = config;
//             _levelSO = levelSO;
//             PhaseId = phaseId;
//             StepId = stepId;

//             _text = string.Empty;
//             SetStateImmediate(initState);

//             _text = await _levelSO.GetLocalizedPhaseStepAsync(phaseId, stepId);
//             SetStateImmediate(initState);
//             _animHighlight.enabled = false;

//             ChangeState(CurrentState);
//         }

//         public void SetHighlight(bool isHighlight) => _animHighlight.enabled = _outlineTxt.enabled = isHighlight;

//         private async Task UpdateMainText(string text)
//         {
//             _text = await _levelSO.GetLocalizedPhaseStepAsync(PhaseId, StepId);
//             switch (CurrentState)
//             {
//                 case State.HiddenUnreached:
//                 case State.Hidden:
//                 case State.Revealed:
//                     _txt.text = _generalConfig.txtAppendCheckmarkUndone + _text;
//                     break;
//                 case State.Passed:
//                     _txt.text = _generalConfig.txtAppendCheckmarkDone + _text;
//                     break;
//             }
//         }

//         public void SetStateImmediate(State state)
//         {
//             CurrentState = state;
//             _tweenChangeText.Kill();
//             switch (state)
//             {
//                 case State.HiddenUnreached:
//                     _txt.text = _generalConfig.txtAppendCheckmarkUndone + _text;
//                     _txt.color = _generalConfig.colorNotShow;
//                     _hiddenTxt.color = _generalConfig.colorTxtHiddenUnreached;
//                     break;
//                 case State.Hidden:
//                     _txt.text = _generalConfig.txtAppendCheckmarkUndone + _text;
//                     _txt.color = _generalConfig.colorNotShow;
//                     _hiddenTxt.color = _generalConfig.colorTxtHidden;
//                     break;
//                 case State.Revealed:
//                     _txt.text = _generalConfig.txtAppendCheckmarkUndone + _text;
//                     _txt.color = _generalConfig.colorRevealed;
//                     _hiddenTxt.color = _generalConfig.colorNotShow;
//                     break;
//                 case State.Passed:
//                     _txt.text = _generalConfig.txtAppendCheckmarkDone + _text;
//                     _txt.color = _generalConfig.colorPassed;
//                     _hiddenTxt.color = _generalConfig.colorNotShow;
//                     break;
//             }
//         }

//         public void ChangeState(State state)
//         {
//             if (CurrentState == state) return;
//             CurrentState = state;
//             _tweenChangeText.Kill();

//             if (gameObject.activeInHierarchy)
//             {
//                 Sequence sequence = DOTween.Sequence();
//                 switch (state)
//                 {
//                     case State.HiddenUnreached:
//                         _txt.text = _generalConfig.txtAppendCheckmarkUndone + _text;
//                         sequence.Append(_txt.DOColor(_generalConfig.colorNotShow, _generalConfig.durationChageColor))
//                             .Join(_hiddenTxt.DOColor(_generalConfig.colorTxtHiddenUnreached, _generalConfig.durationChageColor));
//                         break;
//                     case State.Hidden:
//                         _txt.text = _generalConfig.txtAppendCheckmarkUndone + _text;
//                         sequence.Append(_txt.DOColor(_generalConfig.colorNotShow, _generalConfig.durationChageColor))
//                             .Join(_hiddenTxt.DOColor(_generalConfig.colorTxtHidden, _generalConfig.durationChageColor));
//                         break;
//                     case State.Revealed:
//                         _txt.text = _generalConfig.txtAppendCheckmarkUndone + _text;
//                         sequence.Append(_txt.DOColor(_generalConfig.colorRevealed, _generalConfig.durationChageColor))
//                             .Join(_hiddenTxt.DOColor(_generalConfig.colorNotShow, _generalConfig.durationChageColor));
//                         break;
//                     case State.Passed:
//                         _txt.text = _generalConfig.txtAppendCheckmarkDone + _text;
//                         sequence.Append(_txt.DOColor(_generalConfig.colorPassed, _generalConfig.durationChageColor))
//                             .Join(_hiddenTxt.DOColor(_generalConfig.colorNotShow, _generalConfig.durationChageColor));
//                         break;
//                 }
//                 _tweenChangeText = sequence;
//                 _tweenChangeText.SetUpdate(true).Play();
//             }
//             else
//             {
//                 switch (state)
//                 {
//                     case State.HiddenUnreached:
//                         _txt.text = _generalConfig.txtAppendCheckmarkUndone + _text;
//                         _txt.color = _generalConfig.colorNotShow;
//                         _hiddenTxt.color = _generalConfig.colorTxtHiddenUnreached;
//                         break;
//                     case State.Hidden:
//                         _txt.text = _generalConfig.txtAppendCheckmarkUndone + _text;
//                         _txt.color = _generalConfig.colorNotShow;
//                         _hiddenTxt.color = _generalConfig.colorTxtHidden;
//                         break;
//                     case State.Revealed:
//                         _txt.text = _generalConfig.txtAppendCheckmarkUndone + _text;
//                         _txt.color = _generalConfig.colorRevealed;
//                         _hiddenTxt.color = _generalConfig.colorNotShow;
//                         break;
//                     case State.Passed:
//                         _txt.text = _generalConfig.txtAppendCheckmarkDone + _text;
//                         _txt.color = _generalConfig.colorPassed;
//                         _hiddenTxt.color = _generalConfig.colorNotShow;
//                         break;
//                 }
//             }
//         }

//         private void OnDisable()
//         {
//             if (_tweenChangeText.IsActive()) _tweenChangeText.Complete();
//         }
//     }
// }
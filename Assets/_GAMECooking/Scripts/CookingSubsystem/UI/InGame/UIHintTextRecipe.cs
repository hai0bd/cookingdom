// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Threading.Tasks;
// using DG.Tweening;
// using HierarchyUISystem;
// using TidyCooking.CookingSubsystem;
// using TidyCooking.IapAdsSystem;
// using UnityEngine;
// using UnityEngine.EventSystems;
// using UnityEngine.UI;
// using Utilities;

// namespace TidyCooking.InGame
// {
//     public class UIHintTextRecipe : UIPopupCooking
//     {
//         private const string KeyLocalizePhaseCountSubtitle = "PhaseXpY";

//         private bool _isDoneSetUp;
//         private System.Action _onCloseAndContinue;

//         [Header("Layouts")]
//         [SerializeField] private Text _txtRecipeTitle;
//         [SerializeField] private GameObject _prefabDivider;
//         [SerializeField] private Text _prefabPhaseSubtitle;
//         [SerializeField] private Text _prefabPhaseTitle;
//         [SerializeField] private UIHintTextRecipeItemStep _prefabItemStep;
//         [SerializeField] private RectTransform _itemStepContainer;
//         [SerializeField] private UIHintTextRecipeItemStep.GeneralConfig _configItemStep;
//         [SerializeField] private int _numHintSpawnPerFrame = 5;
//         [SerializeField] private GameObject[] _objNotiInProgress;

//         [Header("Scroll")]
//         [SerializeField] private ScrollRect _scrollRect;
//         [SerializeField] private ScrollAnimConfig _scrollAnimConfig;

//         [Header("Bookmark")]
//         [SerializeField] private RectTransform _transformBookmarks;
//         [SerializeField] private float _anchoredPosYBookmarkShow = 0;
//         [SerializeField] private float _anchoredPosYBookmarkHide = 500;
//         private Tween _tweenBookmarkClose;

//         [System.Serializable]
//         public class ScrollAnimConfig
//         {
//             public float lerpSpeedToStartPos = 0.4f;

//             public float focusPosOffsetY = 100f;
//             public float delay = 0.25f;

//             public float maxScrollSpeed = 100f;
//             public float maxAcceleration = 100f;
//             public float distanceStop = 100f;

//             public float delayBetweenUpdateItem = 0.15f;
//         }

//         [Header("Section Buttons")]
//         [SerializeField] private CanvasGroup _sectionButtons;
//         [SerializeField] private ForwardUIPointerEvent[] _sectionButtonsDragZone;
//         [SerializeField] private Vector2 _sectionButtonsDragRangeX = new Vector2(0, 100f);
//         [SerializeField] private Vector2 _sectionButtonsDragRangeXOvershot = new Vector2(-100, 200f);
//         [SerializeField] private RectTransform _sectionButtonsContainer;
//         [SerializeField] private LoopAnimUIFloating _animSectionButtons;
//         [SerializeField] private Text _txtUnlockNextStep;
//         private const string KeyLocalizeUnlockNextXStep = "UnlockNextXStep";
//         private RectTransform _sectionButtonsTransform;
//         private Vector2 _mouseStartDragPos;
//         private Vector2 _anchoredStartDragPos;
//         private bool _isDraggingSectionButtons = false;
//         private float _anchoredPosYSectionButtons;
//         private Tween _tweenShowHideSectionButton;
//         private Tween _tweenClampSectionButton;
//         private Canvas _baseCanvas;
//         private List<Vector2Int> nextStepToUnlock = new();

//         [Header("Buttons")]
//         [SerializeField] private CookCurrencySO _itemSO;
//         [SerializeField] private CookCurrencySO _goldSO;
//         [SerializeField] private Button _btnUnlockByItem;
//         [SerializeField] private Button _btnUnlockByGold;
//         [SerializeField] private Button _btnUnlockByAds;
//         [SerializeField] private Text _txtGoldRequired;
//         [SerializeField] private RectTransform _sectionCountItemRemained;
//         [SerializeField] private Text _txtCountItemRemained;

//         private ILevelPlayControl _levelPlayControl;
//         private IUIInLevelCooking _uiInLevel;
//         private ILevelBaseCooking _levelBase;
//         private System.Action<List<Vector2Int>> _onUseUnlockHint;
//         private PhaseLayout[] _phaseLayouts;

//         [System.Serializable]
//         private class PhaseLayout
//         {
//             public GameObject divider;
//             public Text subtitle;
//             public Text title;
//             public UIHintTextRecipeItemStep[] steps;
//             public bool[] isRevealed;
//         }

//         private Coroutine _corScroll;
//         private Coroutine _corUpdateItems;
//         private bool _isOpened;
//         private bool _isWin;

//         protected override void Awake()
//         {
//             base.Awake();
//             _btnUnlockByAds.onClick.AddListener(OnClickUnlockGroupByAds);
//             _btnUnlockByGold.onClick.AddListener(OnClickUnlockGroupByGold);
//             _btnUnlockByItem.onClick.AddListener(OnClickUnlockGroupByItem);
//             _scrollRect.onValueChanged.AddListener(UpdateCloseBookmark);

//             foreach (var section in _sectionButtonsDragZone)
//             {
//                 section.onMouseDown += OnPlayBeginDragSectionButton;
//                 section.onMouseUp += OnPlayEndDragSectionButton;
//             }

//             _sectionButtonsTransform = _sectionButtons.GetComponent<RectTransform>();
//             if (CookingSystemDataManager.IsExistInstance)
//             {
//                 CookingSystemDataManager.Instance.onCurrencyChanged += OnCurrencyChanged;
//             }
//         }

//         private void UpdateCloseBookmark(Vector2 anchoredPos)
//         {

//         }

//         protected override void SetupPositionAndAdsOnShow()
//         {
//             // nothing happen
//         }

//         public async Task SetUp(ILevelPlayControl levelPlayControl, IUIInLevelCooking uiInLevel, ILevelBaseCooking levelBase, System.Action<List<Vector2Int>> onUseUnlockHint, RectTransform activateButton)
//         {
//             _levelPlayControl = levelPlayControl;
//             _uiInLevel = uiInLevel;
//             _levelBase = levelBase;
//             _onUseUnlockHint = onUseUnlockHint;
//             _panelActivatePosition = activateButton;

//             _baseCanvas = this.GetComponentInParent<Canvas>();

//             CookingLevelSO levelSO = _levelPlayControl.LevelPlaying;
//             _txtRecipeTitle.text = levelSO.GetLocalizedTitle();

//             if (!levelSO.IsHintTextValid())
//             {
//                 foreach (GameObject obj in _objNotiInProgress)
//                 {
//                     obj.SetActive(true);
//                 }
//                 _sectionButtons.gameObject.SetActive(false);
//                 _isDoneSetUp = true;
//                 return;
//             }

//             bool isPassedLv = CookingLevelsDataManager.IsExistInstance 
//                 && levelPlayControl.EventPlaying
//                 && CookingLevelsDataManager.Instance.IsLevelWinned(levelPlayControl.EventPlaying.Id, levelPlayControl.LevelIndexPlaying);

//             int numPhases = levelSO.NumStepsPerPhase.Length;

//             int totalSteps = 0;
//             for (int phaseIndex = 0; phaseIndex < numPhases; phaseIndex++) totalSteps += levelSO.NumStepsPerPhase[phaseIndex];
//             Task[] tasks = new Task[totalSteps];
//             int countSteps = 0;

//             _phaseLayouts = new PhaseLayout[numPhases];
//             for (int phaseIndex = 0; phaseIndex < numPhases; phaseIndex++)
//             {
//                 int phaseId = phaseIndex + 1;

//                 GameObject divider = Instantiate(_prefabDivider, _itemStepContainer);

//                 Text subtitle = Instantiate(_prefabPhaseSubtitle, _itemStepContainer);
//                 subtitle.text = CookingGameConstants.GetFormattedLocalize(KeyLocalizePhaseCountSubtitle, phaseId, numPhases);

//                 Text title = Instantiate(_prefabPhaseTitle, _itemStepContainer);
//                 title.text = levelSO.GetLocalizedPhaseTitle(phaseId);

//                 int numSteps = levelSO.NumStepsPerPhase[phaseIndex];
//                 bool[] isRevealed = new bool[numSteps];
//                 UIHintTextRecipeItemStep[] steps = new UIHintTextRecipeItemStep[numSteps];
//                 for (int stepIndex = 0; stepIndex < numSteps; stepIndex++)
//                 {
//                     int stepId = stepIndex + 1;

//                     UIHintTextRecipeItemStep.State initState = UIHintTextRecipeItemStep.State.HiddenUnreached;
//                     if (_levelPlayControl.IngameConfig.IsRevealHintTxtAfterPassedLv && isPassedLv)
//                     {
//                         initState = UIHintTextRecipeItemStep.State.Revealed;
//                         isRevealed[stepIndex] = true;
//                     }

//                     steps[stepIndex] = Instantiate(_prefabItemStep, _itemStepContainer);
//                     tasks[countSteps] = steps[stepIndex].SetUp(_configItemStep, levelSO, phaseId, stepId, initState);

//                     countSteps += 1;
//                     if (countSteps % _numHintSpawnPerFrame == 0) await Task.Yield();
//                 }
//                 _phaseLayouts[phaseIndex] = new PhaseLayout
//                 {
//                     divider = divider,
//                     subtitle = subtitle,
//                     title = title,
//                     steps = steps,
//                     isRevealed = isRevealed
//                 };
//             }

//             _sectionButtonsContainer.SetAsLastSibling();

//             _levelBase.onDonePhaseStep += OnDonePhaseStep;
//             _levelBase.onBeginWin += OnWinLevel;

//             await Task.WhenAll(tasks);

//             //_scrollRect.normalizedPosition = new Vector2(0, 1f);
//             LayoutRebuilder.ForceRebuildLayoutImmediate(_scrollRect.content);

//             _isDoneSetUp = true;
//         }

//         private void OnWinLevel()
//         {
//             _isWin = true;
//         }

//         private void OnDestroy()
//         {
//             if (_levelBase != null)
//             {
//                 _levelBase.onDonePhaseStep -= OnDonePhaseStep;
//                 _levelBase.onBeginWin -= OnWinLevel;
//             }
//             if (CookingSystemDataManager.IsExistInstance)
//             {
//                 CookingSystemDataManager.Instance.onCurrencyChanged -= OnCurrencyChanged;
//             }
//         }

//         private void OnCurrencyChanged(string id, int oldValue, int newValue)
//         {
//             if (_itemSO.Id == id || _goldSO.Id == id)
//             {
//                 UpdateUnlockButtons();
//             }
//         }

//         public void Show(System.Action onClosed)
//         {
//             _onCloseAndContinue = onClosed;
//             //_uiInLevel.CurrencyBar.Show();
//             Show(OnClosed, OnDoneAnimShow);

//             void OnDoneAnimShow()
//             {
//                 if (_levelPlayControl.LevelPlaying.IsHintTextValid())
//                 {
//                     //_corScroll = StartCoroutine(IEScrollToLastUnlockedLine());
//                     _corUpdateItems = StartCoroutine(IEUpdateItems());
//                 }

//                 _tweenBookmarkClose.Kill();
//                 _tweenBookmarkClose = _transformBookmarks.DOAnchorPosY(_anchoredPosYBookmarkShow, 0.25f)
//                     .SetEase(Ease.OutBack).SetUpdate(true).Play();
//             }

//             void OnClosed(UIHierarchyElement element)
//             {
//                 //_uiInLevel.CurrencyBar.Hide();
//                 _onCloseAndContinue?.Invoke();
//             }
//         }

//         public void OnPlayBeginDrag()
//         {
//             if (_corScroll != null)
//             {
//                 StopCoroutine(_corScroll);
//                 _corScroll = null;
//             }
//         }

//         protected override void HideElement(Action onDoneAnim = null)
//         {
//             if (!_isDoneSetUp) return;

//             _tweenBookmarkClose.Kill();
//             _tweenBookmarkClose = _transformBookmarks.DOAnchorPosY(_anchoredPosYBookmarkHide, 0.15f)
//                 .SetEase(Ease.InBack).SetUpdate(true).Play();

//             base.HideElement(onDoneAnim);
//         }

//         protected override void HideElementImmediate()
//         {
//             if (!_isDoneSetUp) return;

//             _tweenBookmarkClose.Kill();
//             _transformBookmarks.anchoredPosition = new Vector2(_transformBookmarks.anchoredPosition.x, _anchoredPosYBookmarkHide);

//             base.HideElementImmediate();
//         }

//         protected override void UpdateOnShow()
//         {
//             base.UpdateOnShow();
//             TimeControlCooking.StopTime(this);
//             UpdateButtonPosition(false);
//             UpdateUnlockButtons();
//             _scrollRect.vertical = true;
//             if (!_isOpened)
//             {
//                 _scrollRect.verticalNormalizedPosition = 1f;
//                 _scrollRect.content.anchoredPosition = new Vector2(0, -_scrollRect.content.sizeDelta.y);
//                 _isOpened = true;
//             }

//             if (_levelPlayControl.BoosterUsedDataAds.hintTxt < _levelPlayControl.IngameConfig.hintTxtNumAds)
//             {
//                 _btnUnlockByAds.gameObject.SetActive(true);
//             }
//             else
//             {
//                 _btnUnlockByAds.gameObject.SetActive(false);
//             }
//         }

//         private void UpdateButtonPosition(bool isUseAnim = true)
//         {
//             const float DurationTween = 0.25f;

//             CookingLevelSO levelSO = _levelPlayControl.LevelPlaying;
//             if (!levelSO.IsHintTextValid()) return;

//             int numPhases = levelSO.NumStepsPerPhase.Length;

//             if (_isWin)
//             {
//                 for (int phaseIndex = 0; phaseIndex < numPhases; phaseIndex++)
//                 {
//                     int numSteps = levelSO.NumStepsPerPhase[phaseIndex];
//                     for (int stepIndex = 0; stepIndex < numSteps; stepIndex++)
//                     {
//                         _phaseLayouts[phaseIndex].steps[stepIndex].SetHighlight(false);
//                     }
//                 }
//                 _tweenShowHideSectionButton.Kill();
//                 if (isUseAnim)
//                 {
//                     _tweenShowHideSectionButton = _sectionButtonsTransform.DOScale(0f, DurationTween).SetUpdate(true).OnComplete(DisableSection).Play();
//                 }
//                 else
//                 {
//                     _sectionButtonsTransform.localScale = Vector3.zero;
//                     DisableSection();
//                 }
//                 return;
//             }

//             int numStepsToHighlight = _levelPlayControl.IngameConfig.numHintStep;
//             nextStepToUnlock.Clear();
//             int countHiddenSteps = 0;
//             int firstPhaseHaveHiddenSteps = -1;

//             UIHintTextRecipeItemStep firstHighlightStep = null;
//             UIHintTextRecipeItemStep lastHighlightStep = null;
//             for (int phaseIndex = 0; phaseIndex < numPhases; phaseIndex++)
//             {
//                 int numSteps = levelSO.NumStepsPerPhase[phaseIndex];
//                 for (int stepIndex = 0; stepIndex < numSteps; stepIndex++)
//                 {
//                     if (!_phaseLayouts[phaseIndex].isRevealed[stepIndex])
//                     {
//                         if (firstPhaseHaveHiddenSteps < 0) firstPhaseHaveHiddenSteps = phaseIndex;
//                         nextStepToUnlock.Add(new Vector2Int(phaseIndex + 1, stepIndex + 1));
//                         _phaseLayouts[phaseIndex].steps[stepIndex].SetHighlight(true);
//                         if (!firstHighlightStep) firstHighlightStep = _phaseLayouts[phaseIndex].steps[stepIndex];
//                         lastHighlightStep = _phaseLayouts[phaseIndex].steps[stepIndex];
//                         countHiddenSteps++;

//                         if (countHiddenSteps >= numStepsToHighlight) break;
//                     }
//                     else
//                     {
//                         _phaseLayouts[phaseIndex].steps[stepIndex].SetHighlight(false);
//                     }
//                 }

//                 if (firstPhaseHaveHiddenSteps >= 0) break;
//             }

//             _txtUnlockNextStep.text = CookingGameConstants.GetFormattedLocalize(KeyLocalizeUnlockNextXStep, nextStepToUnlock.Count);

//             _tweenShowHideSectionButton.Kill();
//             if (firstHighlightStep)
//             {
//                 Vector2 targetAnchorPos = _sectionButtonsTransform.anchoredPosition;
//                 targetAnchorPos.x = Mathf.Clamp(targetAnchorPos.x, _sectionButtonsDragRangeX.x, _sectionButtonsDragRangeX.y);
//                 targetAnchorPos.y = _anchoredPosYSectionButtons = firstHighlightStep.GetComponent<RectTransform>().anchoredPosition.y - _sectionButtonsContainer.anchoredPosition.y;
//                 _sectionButtons.gameObject.SetActive(true);

//                 if (isUseAnim)
//                 {
//                     _tweenShowHideSectionButton = DOTween.Sequence()
//                         .Append(_sectionButtonsTransform.DOScale(1f, DurationTween).SetEase(Ease.OutBack))
//                         .Join(_sectionButtonsTransform.DOAnchorPos(targetAnchorPos, DurationTween))
//                         .SetUpdate(true).Play();
//                 }
//                 else
//                 {
//                     _sectionButtonsTransform.localScale = Vector3.one;
//                     _sectionButtonsTransform.anchoredPosition = targetAnchorPos;
//                 }
//             }
//             else
//             {
//                 if (isUseAnim)
//                 {
//                     _tweenShowHideSectionButton = _sectionButtonsTransform.DOScale(0f, DurationTween).SetUpdate(true).OnComplete(DisableSection).Play();
//                 }
//                 else
//                 {
//                     _sectionButtonsTransform.localScale = Vector3.zero;
//                     DisableSection();
//                 }
//             }

//             void DisableSection()
//             {
//                 _tweenClampSectionButton.Kill();
//                 _isDraggingSectionButtons = false;
//                 _scrollRect.vertical = true;

//                 Vector2 anchoredPos = new Vector2(_sectionButtonsTransform.anchoredPosition.x, _anchoredPosYSectionButtons);
//                 _sectionButtonsTransform.anchoredPosition = anchoredPos;
//                 _animSectionButtons.enabled = true;
//                 _sectionButtons.gameObject.SetActive(false);
//             }
//         }

//         private void OnPlayBeginDragSectionButton(ForwardUIPointerEvent @event, PointerEventData data)
//         {
//             if (_isDraggingSectionButtons) return;
//             if (_tweenShowHideSectionButton.IsActive()) _tweenShowHideSectionButton.Complete();
//             _tweenClampSectionButton.Kill();
//             _isDraggingSectionButtons = true;
//             _mouseStartDragPos = data.position;
//             _anchoredStartDragPos = _sectionButtonsTransform.anchoredPosition;
//             _anchoredStartDragPos.y = _anchoredPosYSectionButtons;

//             _animSectionButtons.enabled = false;
//             _scrollRect.vertical = false;
//         }

//         private void OnPlayEndDragSectionButton(ForwardUIPointerEvent @event, PointerEventData data)
//         {
//             //if (!_isDraggingSectionButtons) return;
//             //_isDraggingSectionButtons = false;

//             //Vector2 dragDelta = data.position - _mouseStartDragPos;
//             //Vector2 targetPos = _anchoredStartDragPos;
//             //targetPos.x += dragDelta.x / (_baseCanvas ? _baseCanvas.scaleFactor : 1f);
//             //targetPos.x = Mathf.Clamp(targetPos.x, _sectionButtonsDragRangeX.x, _sectionButtonsDragRangeX.y);
            
//             //_tweenClampSectionButton.Kill();
//             //_tweenClampSectionButton = _sectionButtonsTransform.DOAnchorPosX(targetPos.x, 0.25f).SetUpdate(true).Play();
            
//             //_scrollRect.vertical = true;
//         }

//         private void EndDragSectionButton()
//         {
//             if (!_isDraggingSectionButtons) return;
//             _isDraggingSectionButtons = false;

//             Vector2 targetPos = _sectionButtonsTransform.anchoredPosition;
//             targetPos.x = Mathf.Clamp(targetPos.x, _sectionButtonsDragRangeX.x, _sectionButtonsDragRangeX.y);

//             _tweenClampSectionButton.Kill();
//             _tweenClampSectionButton = _sectionButtonsTransform.DOAnchorPosX(targetPos.x, 0.25f).SetUpdate(true).Play();

//             _scrollRect.vertical = true;
//             Vector2 anchoredPos = new Vector2(_sectionButtonsTransform.anchoredPosition.x, _anchoredPosYSectionButtons);
//             _sectionButtonsTransform.anchoredPosition = anchoredPos;
//             _animSectionButtons.enabled = true;
//         }

//         private void Update()
//         {
//             if (_sectionButtons.gameObject.activeSelf && _isDraggingSectionButtons)
//             {
//                 Vector2 mousePos = Input.mousePosition;
//                 Vector2 dragDelta = mousePos - _mouseStartDragPos;
//                 Vector2 targetPos = _anchoredStartDragPos;
//                 targetPos.x += dragDelta.x / (_baseCanvas ? _baseCanvas.scaleFactor : 1f);
//                 targetPos.x = Mathf.Clamp(targetPos.x, _sectionButtonsDragRangeXOvershot.x, _sectionButtonsDragRangeXOvershot.y);
//                 _sectionButtonsTransform.anchoredPosition = targetPos;

//                 if (Input.GetMouseButtonUp(0))
//                 {
//                     EndDragSectionButton();
//                 }
//             }
//         }

//         private void UpdateItemsNow()
//         {
//             if (!_isDoneSetUp) return;

//             CookingLevelSO levelSO = _levelPlayControl.LevelPlaying;
//             int numPhases = levelSO.NumStepsPerPhase.Length;

//             for (int phaseIndex = 0; phaseIndex < numPhases; phaseIndex++)
//             {
//                 int numSteps = levelSO.NumStepsPerPhase[phaseIndex];
//                 for (int stepIndex = 0; stepIndex < numSteps; stepIndex++)
//                 {
//                     if (_levelBase.IsDonePhaseAndStep(phaseIndex + 1, stepIndex + 1) || _isWin)
//                     {
//                         if (_phaseLayouts[phaseIndex].steps[stepIndex].CurrentState < UIHintTextRecipeItemStep.State.Passed)
//                         {
//                             _phaseLayouts[phaseIndex].steps[stepIndex].ChangeState(UIHintTextRecipeItemStep.State.Passed);
//                             _phaseLayouts[phaseIndex].isRevealed[stepIndex] = true;
//                         }
//                     }
//                 }
//             }
//         }

//         private IEnumerator IEUpdateItems()
//         {
//             while (!_isDoneSetUp) yield return null;

//             CookingLevelSO levelSO = _levelPlayControl.LevelPlaying;
//             int numPhases = levelSO.NumStepsPerPhase.Length;

//             var wait = new WaitForSecondsRealtime(_scrollAnimConfig.delayBetweenUpdateItem);

//             for (int phaseIndex = 0; phaseIndex < numPhases; phaseIndex++)
//             {
//                 int numSteps = levelSO.NumStepsPerPhase[phaseIndex];
//                 for (int stepIndex = 0; stepIndex < numSteps; stepIndex++)
//                 {
//                     if (_levelBase.IsDonePhaseAndStep(phaseIndex + 1, stepIndex + 1) || _isWin)
//                     {
//                         if (_phaseLayouts[phaseIndex].steps[stepIndex].CurrentState < UIHintTextRecipeItemStep.State.Passed)
//                         {
//                             yield return wait;
//                             _phaseLayouts[phaseIndex].steps[stepIndex].ChangeState(UIHintTextRecipeItemStep.State.Passed);
//                             _phaseLayouts[phaseIndex].isRevealed[stepIndex] = true;
//                         }
//                     }
//                 }
//             }

//             _corUpdateItems = null;

//             UpdateButtonPosition();
//         }

//         private IEnumerator IEScrollToLastUnlockedLine()
//         {
//             while (!_isDoneSetUp) yield return null;

//             float timeStart = Time.unscaledDeltaTime;

//             CookingLevelSO levelSO = _levelPlayControl.LevelPlaying;
//             int numPhases = levelSO.NumStepsPerPhase.Length;

//             int firstPhaseIndexDoneButHaveNotUpdated = -1;
//             int firstStepIndexDoneButHaveNotUpdated = -1;

//             int lastestPhaseIndexDone = 0;
//             int lastestStepIndexDone = 0;

//             List<UIHintTextRecipeItemStep> stepsToUpdate = new List<UIHintTextRecipeItemStep>();

//             for (int phaseIndex = 0; phaseIndex < numPhases; phaseIndex++)
//             {
//                 int numSteps = levelSO.NumStepsPerPhase[phaseIndex];
//                 for (int stepIndex = 0; stepIndex < numSteps; stepIndex++)
//                 {
//                     if (_levelBase.IsDonePhaseAndStep(phaseIndex + 1, stepIndex + 1))
//                     {
//                         if (_phaseLayouts[phaseIndex].steps[stepIndex].CurrentState < UIHintTextRecipeItemStep.State.Passed)
//                         {
//                             stepsToUpdate.Add(_phaseLayouts[phaseIndex].steps[stepIndex]);

//                             if (firstPhaseIndexDoneButHaveNotUpdated < 0)
//                             {
//                                 firstPhaseIndexDoneButHaveNotUpdated = phaseIndex;
//                                 firstStepIndexDoneButHaveNotUpdated = stepIndex;
//                             }
//                         }

//                         lastestPhaseIndexDone = phaseIndex;
//                         lastestStepIndexDone = stepIndex;
//                     }
//                 }
//             }

//             if (firstPhaseIndexDoneButHaveNotUpdated < 0)
//             {
//                 firstPhaseIndexDoneButHaveNotUpdated = 0;
//                 firstStepIndexDoneButHaveNotUpdated = 0;
//             }

//             float scrollWindowHeight = _scrollRect.GetComponent<RectTransform>().sizeDelta.y;
//             float fullPaperHeight = _scrollRect.content.sizeDelta.y;
//             float paperPivotY = _scrollRect.content.pivot.y;

//             Vector2 scrollPosRange = new Vector2(
//                 -scrollWindowHeight / 2f + fullPaperHeight * paperPivotY,
//                 +scrollWindowHeight / 2f - fullPaperHeight * (1f - paperPivotY)
//                 );

//             float startOffsetY = _phaseLayouts[firstPhaseIndexDoneButHaveNotUpdated].steps[firstStepIndexDoneButHaveNotUpdated].GetComponent<RectTransform>().anchoredPosition.y;
//             float startPos = startOffsetY + fullPaperHeight * (1f - paperPivotY);
//             float startNormalizedPos = Mathf.InverseLerp(scrollPosRange.x, scrollPosRange.y, startPos);
//             Vector2 normalizedPos = _scrollRect.normalizedPosition;

//             while (Time.unscaledDeltaTime < timeStart + _scrollAnimConfig.delay)
//             {
//                 normalizedPos.y = Mathf.Lerp(normalizedPos.y, startNormalizedPos, _scrollAnimConfig.lerpSpeedToStartPos * Time.unscaledDeltaTime);
//                 _scrollRect.normalizedPosition = normalizedPos;
//                 yield return null;
//             }

//             Vector2 paperAnchoredPos = _scrollRect.content.anchoredPosition;
//             float targetOffsetY = _phaseLayouts[lastestPhaseIndexDone].steps[lastestStepIndexDone].GetComponent<RectTransform>().anchoredPosition.y;
//             float targetPos = targetOffsetY + fullPaperHeight * (1f - paperPivotY);
//             targetPos = Mathf.Clamp(targetPos, scrollPosRange.x, scrollPosRange.y);
//             float currentSpeed = 0f;
//             float desiredSpeed;

//             int nextStepIndexToUpdate = 0;

//             while (paperAnchoredPos.y < targetPos)
//             {
//                 float deltaTime = Time.unscaledDeltaTime;
//                 desiredSpeed = Mathf.Min(_scrollAnimConfig.maxScrollSpeed, Mathf.Abs(targetPos - paperAnchoredPos.y)) / deltaTime;
//                 currentSpeed = Mathf.MoveTowards(currentSpeed, desiredSpeed, _scrollAnimConfig.maxAcceleration * deltaTime);
//                 paperAnchoredPos.y += currentSpeed * deltaTime;
//                 normalizedPos.y = Mathf.InverseLerp(scrollPosRange.x, scrollPosRange.y, paperAnchoredPos.y);
//                 _scrollRect.normalizedPosition = normalizedPos;

//                 if (nextStepIndexToUpdate < stepsToUpdate.Count && paperAnchoredPos.y < stepsToUpdate[nextStepIndexToUpdate].GetComponent<RectTransform>().anchoredPosition.y)
//                 {
//                     stepsToUpdate[nextStepIndexToUpdate].ChangeState(UIHintTextRecipeItemStep.State.Passed);
//                     int phaseIndex = stepsToUpdate[nextStepIndexToUpdate].PhaseId - 1;
//                     int stepIndex = stepsToUpdate[nextStepIndexToUpdate].StepId - 1;
//                     _phaseLayouts[phaseIndex].isRevealed[stepIndex] = true;
//                     nextStepIndexToUpdate++;
//                 }

//                 yield return null;
//             }

//             UpdateButtonPosition();
//         }

//         private void OnDonePhaseStep(int phaseId, int stepId)
//         {
//             int phaseIndex = Mathf.Clamp(phaseId - 1, 0, _phaseLayouts.Length - 1);
//             int stepIndex = Mathf.Clamp(stepId - 1, 0, _phaseLayouts[phaseIndex].steps.Length - 1);
//             _phaseLayouts[phaseIndex].isRevealed[stepIndex] = true;
//             if (IsShowing) UpdateButtonPosition();
//         }

//         protected override void UpdateOnHide()
//         {
//             base.UpdateOnHide();
//             TimeControlCooking.ResumeTime(this);
//             if (_corScroll != null)
//             {
//                 StopCoroutine(_corScroll);
//                 _corScroll = null;
//             }
//             if (_corUpdateItems != null)
//             {
//                 StopCoroutine(_corUpdateItems);
//                 _corUpdateItems = null;
//             }
//         }

//         #region Reveal Hint
//         private const string KEY_TRACKING_ACTION_UNLOCK_RECIPE = "unlock_recipe";
//         private void UpdateUnlockButtons()
//         {
//             CookingLevelSO levelSO = _levelPlayControl.LevelPlaying;
//             if (!levelSO.IsHintTextValid()) return;

//             if (_levelPlayControl.BoosterUsedDataAds.hintTxt >= _levelPlayControl.IngameConfig.hintTxtNumAds)
//             {
//                 _btnUnlockByAds.gameObject.SetActive(false);
//             }
//             else
//             {
//                 _btnUnlockByAds.gameObject.SetActive(true);
//             }

//             int numItem = CookingSystemDataManager.IsExistInstance ? CookingSystemDataManager.Instance.GetCurrencyAmount(_itemSO.Id) : 0;
//             if (numItem > 0)
//             {
//                 _btnUnlockByItem.gameObject.SetActive(true);
//                 _sectionCountItemRemained.gameObject.SetActive(true);
//                 _txtCountItemRemained.text = numItem.ToString();

//                 _btnUnlockByGold.gameObject.SetActive(false);
//             }
//             else
//             {
//                 _btnUnlockByItem.gameObject.SetActive(false);
//                 _btnUnlockByGold.gameObject.SetActive(true);
//                 _txtGoldRequired.text = _levelPlayControl.IngameConfig.hintTxtPrice.ToString();

//                 int currentGold = CookingSystemDataManager.IsExistInstance ? CookingSystemDataManager.Instance.GetCurrencyAmount(_goldSO.Id) : 0;
//                 if (currentGold >= _levelPlayControl.IngameConfig.hintTxtPrice)
//                 {
//                     _txtGoldRequired.color = _goldSO.colorNormal;
//                 }
//                 else
//                 {
//                     _txtGoldRequired.color = _goldSO.colorNotEnough;
//                 }
//             }
//         }

//         private void OnClickUnlockGroupByAds()
//         {
//             CrashlyticStateControl.SetButton(KEY_TRACKING_ACTION_UNLOCK_RECIPE + "_ads");

//             CookingAudioAndHapticManager.PlaySFX(CookSystemSfx.Click);

// #if UNITY_EDITOR
//             if (!AdsManagerCooking.IsExistInstance)
//             {
//                 FinishUnlockHintByAds();
//                 return;
//             }
// #endif

//             if (AdsManagerCooking.IsExistInstance && AdsManagerCooking.Instance.isRewardAvail)
//             {
//                 AdsManagerCooking.Instance.ShowVideo(AdsPosCooking.UnlockRecipeTxt, FinishUnlockHintByAds);
//             }
//             else
//             {
//                 string toastStr = CookingGameConstants.GetLocalize(CookingGameConstants.KEY_LOCALIZE_NEED_INTERNET_TO_PROCESS);
//                 ToastManagerCooking.Instance.ShowToast(toastStr, 2f);
//             }

//             void FinishUnlockHintByAds()
//             {
//                 _levelPlayControl.BoosterUsedDataAds.hintTxt += 1;
//                 if (_levelPlayControl.BoosterUsedDataAds.hintTxt >= _levelPlayControl.IngameConfig.hintTxtNumAds)
//                 {
//                     _btnUnlockByAds.gameObject.SetActive(false);
//                 }
//                 FinishUnlockHint();
//             }
//         }

//         private void OnClickUnlockGroupByItem()
//         {
//             CrashlyticStateControl.SetButton(KEY_TRACKING_ACTION_UNLOCK_RECIPE + "_item");

//             CookingAudioAndHapticManager.PlaySFX(CookSystemSfx.Click);
//             if (CookingSystemDataManager.Instance.GetCurrencyAmount(_itemSO.Id) > 0)
//             {
//                 CookingSystemDataManager.Instance.ChangeCurrencyAmount(_itemSO.Id, -1);
//                 CookingSystemDataManager.Instance.SaveData();
//                 TrackingManagerCooking.Instance.SpendCurrency(_itemSO.Id, KEY_TRACKING_ACTION_UNLOCK_RECIPE, 1);
//                 FinishUnlockHint();
//             }
//             else
//             {
//                 _uiInLevel.ShowShop(Shop.CookShopSection.ChefPacks);
//             }
//         }

//         private void OnClickUnlockGroupByGold()
//         {
//             CrashlyticStateControl.SetButton(KEY_TRACKING_ACTION_UNLOCK_RECIPE + "_gold");

//             CookingAudioAndHapticManager.PlaySFX(CookSystemSfx.Click);
//             int currentGold = CookingSystemDataManager.IsExistInstance ? CookingSystemDataManager.Instance.GetCurrencyAmount(_goldSO.Id) : 0;
//             if (currentGold >= _levelPlayControl.IngameConfig.hintTxtPrice)
//             {
//                 CookingSystemDataManager.Instance.ChangeCurrencyAmount(_goldSO.Id, -_levelPlayControl.IngameConfig.hintTxtPrice);
//                 CookingSystemDataManager.Instance.SaveData();
//                 TrackingManagerCooking.Instance.SpendCurrency(_goldSO.Id, KEY_TRACKING_ACTION_UNLOCK_RECIPE, _levelPlayControl.IngameConfig.hintTxtPrice);
//                 FinishUnlockHint();
//             }
//             else
//             {
//                 _uiInLevel.ShowShop(Shop.CookShopSection.GoldPacks);
//             }
//         }

//         private void FinishUnlockHint()
//         {
//             CookingLevelSO levelSO = _levelPlayControl.LevelPlaying;
//             int numPhases = levelSO.NumStepsPerPhase.Length;

//             for (int i = 0; i < nextStepToUnlock.Count; i++)
//             {
//                 int phaseId = nextStepToUnlock[i].x;
//                 int stepId = nextStepToUnlock[i].y;
//                 _phaseLayouts[phaseId - 1].isRevealed[stepId - 1] = true;
//                 if (_phaseLayouts[phaseId - 1].steps[stepId - 1].CurrentState < UIHintTextRecipeItemStep.State.Revealed)
//                 {
//                     _phaseLayouts[phaseId - 1].steps[stepId - 1].ChangeState(UIHintTextRecipeItemStep.State.Revealed);
//                 }
//             }

//             _levelPlayControl.BoosterUsedDataAll.hintTxt += 1;

//             _onUseUnlockHint?.Invoke(nextStepToUnlock);

//             if (_corUpdateItems != null)
//             {
//                 StopCoroutine(_corUpdateItems);
//                 _corUpdateItems = null;
//             }
//             UpdateItemsNow();
//             UpdateButtonPosition();
//         }
//         #endregion

// #if UNITY_EDITOR
//         [Header("Editor Tool")]
//         [SerializeField] private int _phaseIdEditor = 0;
//         [SerializeField] private int _stepIdEditor = 0;

//         [Sirenix.OdinInspector.Button]
//         private void ScrollToFocusedLineImmediateEditor()
//         {
//             float scrollWindowHeight = _scrollRect.GetComponent<RectTransform>().sizeDelta.y;
//             float fullPaperHeight = _scrollRect.content.sizeDelta.y;
//             float paperPivotY = _scrollRect.content.pivot.y;

//             Vector2 scrollPosRange = new Vector2(
//                 -scrollWindowHeight / 2f + fullPaperHeight * paperPivotY,
//                 +scrollWindowHeight / 2f - fullPaperHeight * (1f - paperPivotY)
//                 );

//             float startOffsetY = _phaseLayouts[_phaseIdEditor].steps[_stepIdEditor].GetComponent<RectTransform>().anchoredPosition.y;
//             float startPos = startOffsetY + fullPaperHeight * (1f - paperPivotY);
//             float startNormalizedPos = Mathf.InverseLerp(scrollPosRange.x, scrollPosRange.y, startPos);
//             Vector2 normalizedPos = _scrollRect.normalizedPosition;
//             normalizedPos.y = startNormalizedPos;
//             _scrollRect.normalizedPosition = normalizedPos;
//             UnityEditor.EditorUtility.SetDirty(_scrollRect);
//         }
// #endif
//     }
// }
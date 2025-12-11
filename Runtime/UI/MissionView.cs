using System;
using System.Threading;
using System.Threading.Tasks;
using ModelView;
using SerializableCallback;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

namespace Missions.UI
{
    public class MissionView : ViewBaseBehaviour<MissionData>
    {
        [SerializeField] private LocalizeStringEvent _descriptionText;
        [SerializeField] private GenericView _rewardView;
        [SerializeField] private Button _claimRewardButton;
        [SerializeField] private GameObject _progress;
        [SerializeField] private TextMeshProUGUI _progressText;
        [SerializeField] private RectTransform _progressBar;
        [SerializeField] private string _progressFormat = "{0} / {1}";

        [SerializeField] private SerializableCallback<CancellationToken, Task> _showAnimation;
        [SerializeField] private SerializableCallback<CancellationToken, Task> _hideAnimation;

        [SerializeField] private UnityEvent _onNotCompleteState;
        [SerializeField] private UnityEvent _onCompleteState;
        [SerializeField] private UnityEvent _onRewardNotApplied;
        [SerializeField] private UnityEvent _onRewardApplied;

        private bool _updatingView;
        private IMissionProgress _missionProgress;
        
        public bool HasModel => _model;

        private void Start()
        {
            _claimRewardButton.onClick.AddListener(ClaimReward);
        }

        private void OnEnable()
        {
            RegisterListeners(_model);
        }

        private void OnDisable()
        {
            UnregisterListener(_model);
        }
        
        private void RegisterListeners(MissionData model)
        {
            UnregisterListener(model);  // ensure no double registration
            if (model)
            {
                model.OnCompleted += UpdateCompletedState;
                model.Reward.RewardClaimed += UpdateClaimedState;
                UpdateCompletedState(model);
                UpdateClaimedState(model);
            }
            
            if (_missionProgress != null)
            {
                _missionProgress.OnProgressChanged += UpdateProgress;
                UpdateProgress();
            }
        }

        private void UnregisterListener(MissionData model)
        {
            if (model)
            {
                model.OnCompleted -= UpdateCompletedState;
                model.Reward.RewardClaimed -= UpdateClaimedState;
            }
            
            if (_missionProgress != null)
            {
                _missionProgress.OnProgressChanged -= UpdateProgress;
            }
        }

        public override bool CanRenderModel(MissionData model)
        {
            return true;
        }

        public override void Initialize(MissionData model)
        {
            UpdateView(model);
        }

        public override async void UpdateView(MissionData model)
        {
            var ct = destroyCancellationToken;
            await WaitPreviousUpdate(ct);
            _updatingView = true;
            
            UnregisterListener(_model);  // unregister listeners for previous model
            
            if (!model)
            {
                await PlayAnimation(_hideAnimation, ct);
            }
            else
            {
                _descriptionText.StringReference = model.Description;

                if (_rewardView)
                {
                    _rewardView.Initialize(model.Reward);
                }

                // handle completed state
                UpdateCompletedState(model);
                UpdateClaimedState(model);
                
                // handle progress
                if (model.Mission is IMissionProgress missionProgress)
                {
                    _progress.SetActive(true);
                    _missionProgress = missionProgress;
                }
                else
                {
                    _progress.SetActive(false);
                }
                
                RegisterListeners(model);
                
                await PlayAnimation(_showAnimation, ct);
            }
            
            _updatingView = false;
        }

        private async Task PlayAnimation(SerializableCallback<CancellationToken, Task> animation, CancellationToken ct)
        {
            if (!animation.Target) return;
            await animation.Invoke(ct);
        }

        private async Task WaitPreviousUpdate(CancellationToken ct)
        {
            while (_updatingView && !ct.IsCancellationRequested)
            {
                await Task.Yield();
            }
        }

        private void UpdateCompletedState()
        {
            UpdateCompletedState(_model);
        }
        
        private void UpdateCompletedState(MissionData model)
        {
            _claimRewardButton.interactable = model.RequiresClaim && model.IsCompleted;
            var evt = model.IsCompleted ? _onCompleteState : _onNotCompleteState;
            evt.Invoke();
        }

        private void UpdateClaimedState()
        {
            UpdateClaimedState(_model);
        }

        private void UpdateClaimedState(MissionData model)
        {
            _claimRewardButton.gameObject.SetActive(!model.IsRewardClaimed && model.RequiresClaim);
            var evt = model.IsRewardClaimed ? _onRewardApplied : _onRewardNotApplied;
            evt.Invoke();
        }

        private void UpdateProgress()
        {
            UpdateProgress(_missionProgress);
        }
        
        private void UpdateProgress(IMissionProgress mission)
        {
            var current = mission.FormatProgressValues(mission.GetCurrentProgress());
            var max = mission.FormatProgressValues(mission.GetMaxProgress());
            var normalized = mission.GetNormalizedProgress();
            
            _progressText.text = string.Format(_progressFormat, current, max);
            var anchorMax = _progressBar.anchorMax;
            anchorMax.x = normalized;
            _progressBar.anchorMax = anchorMax;
        }

        private void ClaimReward()
        {
            if (_model && _model.RequiresClaim && _model.IsCompleted && !_model.IsRewardClaimed)
            {
                _model.ApplyReward();
            }
        }
    }
}
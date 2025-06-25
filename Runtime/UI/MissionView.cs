using System;
using System.Threading;
using System.Threading.Tasks;
using ModelView;
using SerializableCallback;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization.Components;

namespace Missions.UI
{
    public class MissionView : ViewBaseBehaviour<MissionData>
    {
        [SerializeField] private LocalizeStringEvent _descriptionText;
        [SerializeField] private GenericView _rewardView;
        [SerializeField] private GameObject _progress;
        [SerializeField] private TextMeshProUGUI _progressText;
        [SerializeField] private RectTransform _progressBar;
        [SerializeField] private string _progressFormat = "{0} / {1}";

        [SerializeField] private SerializableCallback<CancellationToken, Task> _showAnimation;
        [SerializeField] private SerializableCallback<CancellationToken, Task> _hideAnimation;

        [SerializeField] private UnityEvent _onNotCompleteState;
        [SerializeField] private UnityEvent _onCompleteState;

        private bool _updatingView;
        private IMissionProgress _missionProgress;
        
        public bool HasModel => _model;

        private void OnEnable()
        {
            RegisterListeners();
        }

        private void OnDisable()
        {
            UnregisterListener();
        }
        
        private void RegisterListeners()
        {
            if (_model)
            {
                _model.OnCompleted += UpdateCompletedState;
                UpdateCompletedState();
            }
            
            if (_missionProgress != null)
            {
                _missionProgress.OnProgressChanged += UpdateProgress;
                UpdateProgress();
            }
        }

        private void UnregisterListener()
        {
            if (_model)
            {
                _model.OnCompleted -= UpdateCompletedState;
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
            
            if (!model)
            {
                await PlayAnimation(_hideAnimation, ct);
            }
            else
            {
                _descriptionText.StringReference = model.Description;
                _rewardView.Initialize(model.Reward);

                // handle completed state
                UpdateCompletedState();
                
                // handle progress
                UnregisterListener();
                if (model.Mission is IMissionProgress missionProgress)
                {
                    _progress.SetActive(true);
                    _missionProgress = missionProgress;
                    RegisterListeners();
                }
                else
                {
                    _progress.SetActive(false);
                }
                
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
            var evt = _model.IsCompleted ? _onCompleteState : _onNotCompleteState;
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
    }
}
using System;
using System.Threading;
using System.Threading.Tasks;
using ModelView;
using SerializableCallback;
using TMPro;
using UnityEngine;
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

        [SerializeField] private SerializableCallback<CancellationToken, Task> _showAnimation;
        [SerializeField] private SerializableCallback<CancellationToken, Task> _hideAnimation;

        private bool _updatingView;
        private IMissionProgress _missionProgress;
        
        public bool HasModel => _model != null;

        private void OnDisable()
        {
            UnregisterProgressListener();
        }

        private void UnregisterProgressListener()
        {
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
            
            if (model == null)
            {
                await PlayAnimation(_hideAnimation, ct);
            }
            else
            {
                _descriptionText.StringReference = model.GetDescription();
                _rewardView.Initialize(model.GetReward());

                UnregisterProgressListener();
                if (model.Mission is IMissionProgress missionProgress)
                {
                    _progress.SetActive(true);
                    _missionProgress = missionProgress;
                    missionProgress.OnProgressChanged += UpdateProgress;
                    UpdateProgress(missionProgress);
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

        private void UpdateProgress()
        {
            UpdateProgress(_missionProgress);
        }
        
        private void UpdateProgress(IMissionProgress mission)
        {
            var current = mission.GetCurrentProgress();
            var max = mission.GetMaxProgress();
            var normalized = mission.GetNormalizedProgress();
            
            _progressText.text = $"{current}/{max}";
            var anchorMax = _progressBar.anchorMax;
            anchorMax.x = normalized;
            _progressBar.anchorMax = anchorMax;
        }
    }
}
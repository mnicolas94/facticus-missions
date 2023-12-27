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
    public class MissionView : ViewBaseBehaviour<IMission>
    {
        [SerializeField] private LocalizeStringEvent _descriptionText;
        [SerializeField] private MultiViewDelegate _rewardView;
        [SerializeField] private GameObject _progress;
        [SerializeField] private TextMeshProUGUI _progressText;
        [SerializeField] private RectTransform _progressBar;

        [SerializeField] private SerializableCallback<CancellationToken, Task> _showAnimation;
        [SerializeField] private SerializableCallback<CancellationToken, Task> _hideAnimation;

        private bool _updatingView;

        public bool HasModel => _model != null;

        public override bool CanRenderModel(IMission model)
        {
            return true;
        }

        private CancellationTokenSource _cts;

        private void OnEnable()
        {
            _cts = new CancellationTokenSource();
        }

        private void OnDisable()
        {
            if (!_cts.IsCancellationRequested)
            {
                _cts.Cancel();
            }

            _cts.Dispose();
            _cts = null;
        }

        public override void Initialize(IMission model)
        {
            UpdateView(model);
        }

        public override async void UpdateView(IMission model)
        {
            var ct = _cts.Token;
            await WaitPreviousUpdate(ct);
            _updatingView = true;
            
            if (model == null)
            {
                await PlayAnimation(_hideAnimation);
            }
            else
            {
                _descriptionText.StringReference = model.GetDescription();
                _rewardView.Initialize(model.GetReward());

                if (model is IMissionProgress missionProgress)
                {
                    _progress.SetActive(true);
                    missionProgress.AddProgressChangedCallback(() => UpdateProgress(missionProgress));
                    UpdateProgress(missionProgress);
                }
                else
                {
                    _progress.SetActive(false);
                }
                
                await PlayAnimation(_showAnimation);
            }
            
            _updatingView = false;
        }

        private async Task PlayAnimation(SerializableCallback<CancellationToken, Task> animation)
        {
            await animation.Invoke(_cts.Token);
        }

        private async Task WaitPreviousUpdate(CancellationToken ct)
        {
            while (_updatingView && !ct.IsCancellationRequested)
            {
                await Task.Yield();
            }
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
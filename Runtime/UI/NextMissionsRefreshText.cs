using System;
using System.Text;
using TMPro;
using UnityEngine;

namespace Missions.UI
{
    public class NextMissionsRefreshText : MonoBehaviour
    {
        [SerializeField] private MissionsPoolData _missionsData;
        [SerializeField] private MissionsSerializableState _missionsPersistedData;
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private int _updateIntervalInFrames = 5;
        
        [SerializeField] private bool _useUppercaseLetters;

        private readonly StringBuilder _stringBuilder = new ();
        
        private void Update()
        {
            if (Time.frameCount % _updateIntervalInFrames == 0)
            {
                var secondsToNextRefresh = MissionsPoolData.SecondsToNextRefresh(_missionsData, _missionsPersistedData);
                _text.text = ToText(secondsToNextRefresh);
            }
        }
        
        private string ToText(int totalSeconds)
        {
            var days = totalSeconds / 86400;
            var hours = (totalSeconds % 86400) / 3600;
            var minutes = (totalSeconds % 3600) / 60;
            var seconds = totalSeconds % 60;

            _stringBuilder.Clear();
            
            if (days > 0) _stringBuilder.Append(days).Append(_useUppercaseLetters ? "D:" : "d:");
            if (_stringBuilder.Length > 0 || hours > 0) _stringBuilder.Append(hours).Append(_useUppercaseLetters ? "H:" : "h:");
            if (_stringBuilder.Length > 0 || minutes > 0) _stringBuilder.Append(minutes).Append(_useUppercaseLetters ? "M:" : "m:");
            _stringBuilder.Append(seconds).Append(_useUppercaseLetters ? "S" : "s");

            return _stringBuilder.ToString();
        }
    }
}
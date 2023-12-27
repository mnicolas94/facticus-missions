using System;
using UnityEngine.Localization;

namespace Missions
{
    /// <summary>
    /// Missions should be plain c# classes instead of Unity's Object (e.g. ScriptableObject) because then you can
    /// save an IMission object for data persistence.
    /// </summary>
    public interface IMission
    {
        LocalizedString GetDescription();
        
        IMissionReward GetReward();
        
        /// <summary>
        /// Set any mission data, e.g. progress, to its initial state.
        /// </summary>
        void Initialize();
        
        /// <summary>
        /// Start tracking mission completion condition.
        /// </summary>
        /// <param name="onCompleted"></param>
        void StartMission(Action onCompleted);
        
        /// <summary>
        /// Stop tracking mission completion condition.
        /// </summary>
        void EndMission();

        IMission Clone();
    }
}
using System;

namespace Missions
{
    /// <summary>
    /// Missions should be plain c# classes instead of Unity's Object (e.g. ScriptableObject) because then you can
    /// save an IMission object for data persistence.
    /// </summary>
    public interface IMissionImplementation
    {
        /// <summary>
        /// Set any mission data, e.g. progress, to its initial state.
        /// </summary>
        void Initialize();
        
        /// <summary>
        /// Start tracking mission completion condition.
        /// </summary>
        void StartMission();
        
        /// <summary>
        /// Stop tracking mission completion condition.
        /// </summary>
        void EndMission();

        Action OnCompleted { get; set; }
    }
}
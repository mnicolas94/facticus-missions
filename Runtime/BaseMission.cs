using System;
using Unity.Properties;

namespace Missions
{
    [Serializable]
    [GeneratePropertyBag]
    [Obsolete]
    public abstract partial class BaseMission : IMissionImplementation
    {
        public abstract void Initialize();

        public abstract void StartMission();

        public abstract void EndMission();
        
        public Action OnCompleted { get; set; }
    }
}
using System;
using Unity.Properties;

namespace Missions
{
    [Serializable]
    [GeneratePropertyBag]
    [Obsolete]
    public abstract partial class BaseMission : IMissionImplementation
    {
        public bool IsCompleted { get; }
       
        public Action OnCompleted { get; set; }
        
        public abstract void Initialize();

        public abstract void StartMission();

        public abstract void EndMission();
    }
}
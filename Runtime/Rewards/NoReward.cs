using System;

namespace Missions.Rewards
{
    [Serializable]
    public class NoReward : IMissionReward
    {
        public void Initialize()
        {
        }

        public void ApplyReward()
        {
        }
    }
}
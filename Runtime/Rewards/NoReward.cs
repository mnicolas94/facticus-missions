using System;

namespace Missions.Rewards
{
    [Serializable]
    public class NoReward : IMissionReward
    {
        public Action RewardClaimed { get; set; }

        public void Initialize()
        {
        }

        public void ApplyReward()
        {
        }
    }
}
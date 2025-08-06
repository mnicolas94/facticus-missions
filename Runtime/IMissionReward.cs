using System;

namespace Missions
{
    public interface IMissionReward
    {
        Action RewardClaimed { get; set; }
        void Initialize();
        void ApplyReward();
    }
}
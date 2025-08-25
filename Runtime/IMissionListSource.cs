using System;
using System.Collections.Generic;

namespace Missions
{
    public interface IMissionListSource
    {
        IEnumerable<MissionData> GetMissions();
        Action ListChanged { get; set; }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wonderplanet.PerformanceEstimation
{
    public interface IPerformanceEstimation
    {
        SpecEstimation DevicePerformanceEstimate();
    }
}

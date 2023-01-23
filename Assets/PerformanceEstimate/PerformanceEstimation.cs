using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wonderplanet.PerformanceEstimation
{
    public class PerformanceEstimation : IPerformanceEstimation
    {
        /// <summary>
        /// 世代(iPhone11など) = ModelIdではない
        /// ModelIdは以下を参照
        /// https://www.theiphonewiki.com/wiki/Models
        /// </summary>
        readonly int MiddleSpecIphoneModelId = 13;
    
        public SpecEstimation DevicePerformanceEstimate()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            var gpuVendor = SystemInfo.graphicsDeviceVendor;
            if (KnownGPUVendor.Contains(gpuVendor))
            {
                switch (gpuVendor)
                {
                    case "ARM":
                    {
                        ushort estimation = (ushort) SpecEstimation.High;
                        if (SystemInfo.processorCount < ARMMinimumProcCount) estimation -= 1;
                        if (SystemInfo.processorFrequency < ARMMinimumFreq) estimation -= 1;
                        if (SystemInfo.graphicsMemorySize < ARMRequiredMemorySize) estimation -= 1;
                        if (SystemInfo.systemMemorySize < ARMMinimumSysMemory) estimation -= 1;
                        estimation = (ushort) Mathf.Max(estimation, 1);
                        return (SpecEstimation) estimation;
                    }
                    case "Qualcomm":
                    default:
                    {
                        ushort estimation = (ushort)SpecEstimation.High;
                        if (SystemInfo.processorCount < QualcommMinimumProcCount) estimation -= 1;
                        if (SystemInfo.processorFrequency < QualcommMinimumFreq) estimation -= 1;
                        if (SystemInfo.graphicsMemorySize < QualcommRequiredMemorySize) estimation -= 1;
                        if (SystemInfo.systemMemorySize < QualcommMinimumSysMemory) estimation -= 1;
                        estimation = (ushort)Mathf.Max(estimation, 1);
                        return (SpecEstimation)estimation;
                    }
                }
            }
                
#endif
#if UNITY_IPHONE
            string modelName = SystemInfo.deviceModel;
            if (modelName.StartsWith("iPhone"))
            {
                return EstimateIPhone(modelName);
            }

            if (modelName.StartsWith("iPad"))
            {
                return EstimateIPad(modelName);
            }

            if (modelName.StartsWith("iPod"))
            {
                return EstimateIPod(modelName);
            }
#endif
            return SpecEstimation.Unknown;
        }

        SpecEstimation EstimateIPhone(string model)
        {
            string modelNumbers = model.Replace("iPhone", "");
            string[] numbers = modelNumbers.Split(',');
            if (int.TryParse(numbers[0], out int firstNum))
            {
                if (firstNum < MiddleSpecIphoneModelId) return SpecEstimation.Low;
                if (firstNum == MiddleSpecIphoneModelId) return SpecEstimation.Middle;
                return SpecEstimation.High;
            }

            return SpecEstimation.Unknown;
        }
        
        SpecEstimation EstimateIPad(string model)
        {
            string modelNumbers = model.Replace("iPad", "");
            string[] numbers = modelNumbers.Split(',');
            if (int.TryParse(numbers[0], out int firstNum))
            {
                return SpecEstimation.Low;
                if (firstNum < 7) return SpecEstimation.Low;
                if (firstNum == 7) return SpecEstimation.Middle;
                return SpecEstimation.High;
            }

            return SpecEstimation.Unknown;
        }
        
        SpecEstimation EstimateIPod(string model)
        {
            string modelNumbers = model.Replace("iPod", "");
            string[] numbers = modelNumbers.Split(',');
            if (int.TryParse(numbers[0], out int firstNum))
            {
                if (firstNum < 9) return SpecEstimation.Low;
                return SpecEstimation.Middle;
            }

            return SpecEstimation.Unknown;
        }
        
        private static readonly List<string> KnownGPUVendor = new List<string>()
        {
            "Qualcomm",
            "Apple"
        };

        private const int QualcommMinimumProcCount = 8;
        private const int QualcommRequiredMemorySize = 1024;
        private const int QualcommMinimumFreq = 2150;
        private const int QualcommMinimumSysMemory = 3680;
        
        private const int ARMMinimumProcCount = 8;
        private const int ARMRequiredMemorySize = 1024;
        private const int ARMMinimumFreq = 2320;
        private const int ARMMinimumSysMemory = 3610;
    }
}

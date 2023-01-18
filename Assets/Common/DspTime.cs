using UnityEngine;

namespace Symphogear.Common
{
    public class DspTime : Singleton<DspTime>
    {
        public double Time { get; set; }

        public double AdaptiveTime { get; set; }

        protected DspTime()
        {
            Time = AudioSettings.dspTime;
            AdaptiveTime = Time;
        }

        public override bool Prepare()
        {
            Time = AudioSettings.dspTime;
            AdaptiveTime = Time;

            return base.Prepare();
        }

        private void Update()
        {
            if (Time == AudioSettings.dspTime)
            {
                AdaptiveTime += UnityEngine.Time.unscaledDeltaTime;
            }
            else
            {
                Time = AudioSettings.dspTime;
                AdaptiveTime = Time;
            }
        }
    }
}

using CSCore.CoreAudioAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundCoreAPItest
{
    class VolumeController
    {
         public AudioSessionControl2 AudioSessionControl { get; }
        public SimpleAudioVolume SimpleAudioVolume { get; }
         public String ProcessName { get; }

        public VolumeController(AudioSessionControl2 audioSessionControl,SimpleAudioVolume simpleAudioVolume)
        {
            AudioSessionControl = audioSessionControl;
            SimpleAudioVolume = simpleAudioVolume;
            ProcessName = audioSessionControl.Process.ProcessName;
        }

        public void Dispoce()
        {
            this.AudioSessionControl.Dispose();
            this.SimpleAudioVolume.Dispose();
        }
    }
}

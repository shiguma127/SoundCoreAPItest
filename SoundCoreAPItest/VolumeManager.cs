using CSCore.CoreAudioAPI;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SoundCoreAPItest
{
    class VolumeManager
    {
        public List<VolumeController> list;

        public VolumeManager()
        {
            list = new List<VolumeController>();
            using (var sessionManager = GetDefaultAudioSessionManager(DataFlow.Render))
            {
                AudioSessionEnumerator sessionEnumerator = sessionManager.GetSessionEnumerator();
                foreach (var session in sessionEnumerator)
                {
                    try
                    {
                        var simpleVolume = session.QueryInterface<SimpleAudioVolume>();
                        var sessionControl = session.QueryInterface<AudioSessionControl2>();
                        Process process = Process.GetProcessById(sessionControl.ProcessID);
                        Console.Write(process.ProcessName + "[" + sessionControl.ProcessID + "]");
                        //Console.WriteLine(process.MainModule.FileName);
                        //システムのメインモジュールは取得できないのでid[0]で弾く
                        //filepathからiconを取得できる
                        //Icon ico = Icon.ExtractAssociatedIcon(@"C:\WINDOWS\system32\notepad.exe");
                        Console.Write(" : ");
                        Console.WriteLine(simpleVolume.MasterVolume);
                        //session.RegisterAudioSessionNotification(events);
                        sessionControl.SimpleVolumeChanged += (o, e) => { Console.WriteLine(e.NewVolume); };
                        list.Add(new VolumeController(sessionControl,simpleVolume));
                    }
                    catch (ArgumentException)
                    {
                        //エラーを握りつぶす
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }
        }


        public void SetProcessVolume(String processName, float volume)
        {
            foreach (var item in list)
            {
                if (item.ProcessName.Equals(processName))
                {
                    item.SimpleAudioVolume.MasterVolume = volume;
                }
            }
        }




        private static AudioSessionManager2 GetDefaultAudioSessionManager(DataFlow dataFlow)
        {
            using (var enumerator = new MMDeviceEnumerator())
            {
                using (var device = enumerator.GetDefaultAudioEndpoint(dataFlow, Role.Multimedia))
                {
                    using (AudioEndpointVolume endpointVolume = AudioEndpointVolume.FromDevice(device))
                    {
                        //デバイスの音量を取得
                        Debug.WriteLine("DefaultDevice: " + device.FriendlyName);
                        Debug.WriteLine("Volume: " + endpointVolume.MasterVolumeLevel);
                        AudioSessionManager2 sessionManager = AudioSessionManager2.FromMMDevice(device);
                        return sessionManager;
                    }
                }
            }
        }
    }
}

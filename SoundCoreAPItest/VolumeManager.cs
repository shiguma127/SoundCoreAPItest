using CSCore.CoreAudioAPI;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SoundCoreAPItest
{
    class VolumeManager
    {
        public List<(SimpleAudioVolume, AudioSessionControl2)> list;

        public VolumeManager()
        {
            list = new List<(SimpleAudioVolume, AudioSessionControl2)>();
            using (var sessionManager = GetDefaultAudioSessionManager(DataFlow.Render))
            {
                AudioSessionEnumerator sessionEnumerator = sessionManager.GetSessionEnumerator();
                foreach (var session in sessionEnumerator)
                {
                    try
                    {
                        // usingで書くべき箇所

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
                        list.Add((simpleVolume, sessionControl));
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
            list.ForEach((item) =>
            {
                /*
                 * Processを取りに行くのが遅いのでクラス作ってProcess名とSimpleAudioVolumeを取りに行くのが良いかも
                 * イベントのためにAudioSessionControlも必要
                 *
                */
                
                item.Item2.SimpleVolumeChanged += (o,e)=> { Console.WriteLine(item.Item2.Process.ProcessName + ":" + e.NewVolume); };
            });
        }


        public void SetProcessVolume(String processName, float volume)
        {
            foreach (var item in list)
            {
                if (item.Item2.Process.ProcessName.Equals(processName))
                {
                    item.Item1.MasterVolume = volume;
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

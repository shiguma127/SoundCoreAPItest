using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading;
using CSCore.CoreAudioAPI;

namespace SoundCoreAPItest
{
    class Program
    {
        [MTAThread]

        static void Main(string[] args)
        {
            VolumeManager manager = new VolumeManager();
            SerialPort serialPort = new SerialPort("COM3");
            serialPort.BaudRate = 9600;
            serialPort.Parity = Parity.None;
            serialPort.StopBits = StopBits.One;
            serialPort.DataBits = 8;
            serialPort.Handshake = Handshake.None;
            serialPort.RtsEnable = true;
            serialPort.DataReceived += new SerialDataReceivedEventHandler((o,e) => DataReceivedHandler(o,e,manager));
            serialPort.Open();
            Thread.CurrentThread.Join();
        }
        private static void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e, VolumeManager instance)
        {
            SerialPort sp = (SerialPort)sender;
            string indata = sp.ReadLine().Trim();
            try
            {
                indata = indata.Split(':')[1];
                float volume = (float)(Double.Parse(indata) * 0.01);
                instance.SetProcessVolume("Spotify", volume);
            }
            catch
            {
                //ignore
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

        

        private static void SetMasterVolume(float volume)
        {
            using (var enumerator = new MMDeviceEnumerator())
            {
                using (var device = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia))
                {
                    using (AudioEndpointVolume endpointVolume = AudioEndpointVolume.FromDevice(device))
                    {
                            endpointVolume.MasterVolumeLevelScalar = volume;
                    }
                }
            }
        }
        
    }

    
}

using System;
using System.Collections;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using RyapUnity.Extention;
using RyapUnity.Ryap;
using UnityEngine;

namespace RyapUnity.Network
{
    public class UDPReceiver : MonoBehaviour
    {
        public enum Stats
        {
            Connecting,
            Connected,
            Error,
        }
        
        private const int DATA_NUMBER = 3;
        private const int DATA__NUMBER = 4;
    
        [SerializeField] private int localPort = 22222;
    
        public float[] AccData { get; private set; } = new float[DATA_NUMBER];
        public float[] GyroData { get; private set; } = new float[DATA_NUMBER];
        public float[] AhrsData { get; private set; } = new float[DATA__NUMBER];
        public bool IsButtonAClicked = false;

        public Stats Status { get; private set; } = Stats.Connecting;
        
        private UdpClient udp;

        void Start()
        {
            udp = new UdpClient(localPort) {Client = {ReceiveTimeout = 500}};
            StartCoroutine(ThreadMethod());
        }

        private IEnumerator ThreadMethod()
        {
            IPEndPoint remoteEp = null;
            Header header = default;
            ImuData imu = default;
            ButtonData button = default;
            var hasError = false;
            
            while (true)
            {
                try
                {
                    var data = udp.Receive(ref  remoteEp);
                    header = new Header(data.Take(HeaderDef.HeaderLength).ToArray());
                    var body = data.Skip(HeaderDef.HeaderLength).ToArray();
                    if (header.DataId == HeaderDef.ImuDataId)
                    {
                        if (TryReadImuData(body, ref imu))
                        {
                            AccData = imu.Acc;
                            GyroData = imu.Gyro;
                            AhrsData = imu.Quaternion;
                        }
                    }
                    else if (header.DataId == HeaderDef.ButtonDataId)
                    {
                        if (TryReadButtonData(body, ref button))
                        {
                            if (button.ButtonA == ButtonState.Push)
                            {
                                Debug.Log("ButtonA Clicked");
                                IsButtonAClicked = true;
                            }
                            if (button.ButtonB == ButtonState.Push)
                            {
                                Debug.Log("ButtonB Clicked");
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    hasError = true;
                    Debug.Log(e);
                }

                if (hasError)
                {
                    Status = Stats.Error;
                    Debug.Log("エラーが起こっているので3秒待機します。");
                    yield return new WaitForSeconds(3f);
                    hasError = false;
                }
                else
                {
                    Status = Stats.Connected;
                    // 1ms is less than Ryap(10ms) and a little wait for unlock main thread
                    yield return new WaitForSeconds(0.001f);
                }
            }
        }

        private bool TryReadImuData(byte[] bytes, ref ImuData imu) {
            var timestamp = bytes.ToUInt(0);
            var acc = bytes.ToVector3(4);
            var gyro = bytes.ToVector3(16);
            var quat = bytes.ToQuaternion(28);
            imu = new ImuData(timestamp, acc, gyro, quat);
            return true;
        }
        
        public bool TryReadButtonData(byte[]  bytes, ref ButtonData button) {
            var timestamp = bytes.ToUInt(0);
            button = new ButtonData(timestamp, bytes[4]);
            return true;
        }
    }
}

using System;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;

namespace SC2Crawler
{
    public class BilibiliClient
    {
        private string _CIDInfoUrl = "http://live.bilibili.com/api/player?id=cid:";
        private int _roomId = 0;
        private int _ChatPort = 788;
        private int _protocolversion = 1;
        private TcpClient _client;
        private NetworkStream _stream;
        private bool connected = false;
        private int _UserCount = 0;
        private string _ChatHost = "livecmt-1.bilibili.com";

        public BilibiliClient(int roomId) 
        {
            _roomId = roomId;
        }

        public async Task ConnectServer()
        {
            Debug.WriteLine("正在进入房间。。。。。");
            HttpClient httpClient = new HttpClient();
            string html = await httpClient.GetStringAsync("http://live.bilibili.com/" + _roomId);
            Match m = Regex.Match(html, @"ROOMID\s=\s(\d+)");
            string ROOMID = m.Groups[1].Value;
            _roomId = int.Parse(ROOMID);
            string xmlString = "<root>" + await httpClient.GetStringAsync(_CIDInfoUrl + ROOMID) + "</root>";
            XmlDocument dom = new XmlDocument();
            dom.LoadXml(xmlString);
            string server = dom.GetElementsByTagName("server")[0].InnerText;
            _ChatHost = server;

            _client = new TcpClient(_ChatHost, _ChatPort);
            _stream = _client.GetStream();
            Debug.WriteLine("链接弹幕中。。。。。");
            if (await SendJoinChannel(_roomId))
            {
                connected = true;
                Debug.WriteLine("进入房间成功。。。。。");
                Debug.WriteLine("链接弹幕成功。。。。。");
                await ReceiveMessageLoop();
            }
        }

        private async Task HeartbeatLoop()
        {
            while (!connected)
                await Task.Delay(500);

            while (connected)
            {
                await SendSocketData(0, 16, _protocolversion, 2, 1, "");
                await Task.Delay(30000);
            }
        }

        private async Task<bool> SendJoinChannel(int channelId)
        {
            Random random = new Random();
            long uid = 100000000000000L + (long)(200000000000000L * random.NextDouble());
            string body = "{\"roomid\":" + channelId + ",\"uid\":" + uid + "}";
            await SendSocketData(0, 16, _protocolversion, 7, 1, body);
            return true;
        }

        private async Task SendSocketData(int packetlength, int magic, int ver, int action, int param, string body)
        {
            byte[] bytearr = Encoding.UTF8.GetBytes(body);
            if (packetlength == 0)
                packetlength = bytearr.Length + 16;
            byte[] sendbytes = BitConverter.GetBytes(packetlength)
                .Concat(BitConverter.GetBytes((ushort)magic))
                .Concat(BitConverter.GetBytes((ushort)ver))
                .Concat(BitConverter.GetBytes(action))
                .Concat(BitConverter.GetBytes(param))
                .Concat(bytearr)
                .ToArray();
            await _stream.WriteAsync(sendbytes, 0, sendbytes.Length);
            await _stream.FlushAsync();
        }

        private async Task ReceiveMessageLoop()
        {
            while (connected)
            {
                byte[] tmp = new byte[4];
                await _stream.ReadAsync(tmp, 0, 4);
                int expr = BitConverter.ToInt32(tmp, 0);
                await _stream.ReadAsync(tmp, 0, 2);
                await _stream.ReadAsync(tmp, 0, 2);
                await _stream.ReadAsync(tmp, 0, 4);
                int num = BitConverter.ToInt32(tmp, 0);
                await _stream.ReadAsync(tmp, 0, 4);
                int num2 = expr - 16;

                if (num2 != 0)
                {
                    num--;
                    if (num == 0 || num == 1 || num == 2)
                    {
                        await _stream.ReadAsync(tmp, 0, 4);
                        int num3 = BitConverter.ToInt32(tmp, 0);
                        Debug.WriteLine("房间人数为 " + num3);
                        _UserCount = num3;
                        continue;
                    }
                    else if (num == 3 || num == 4)
                    {
                        byte[] data = new byte[num2];
                        await _stream.ReadAsync(data, 0, num2);
                        string messages = Encoding.UTF8.GetString(data);
                        parseDanMu(messages);
                        continue;
                    }
                    else if (num == 5 || num == 6 || num == 7)
                    {
                        byte[] data = new byte[num2];
                        await _stream.ReadAsync(data, 0, num2);
                        continue;
                    }
                    else
                    {
                        if (num != 16)
                        {
                            byte[] data = new byte[num2];
                            await _stream.ReadAsync(data, 0, num2);
                        }
                        else
                            continue;
                    }
                }
            }
        }

        private void parseDanMu(string messages)
        {
            // Your parsing logic here
        }
    }
}

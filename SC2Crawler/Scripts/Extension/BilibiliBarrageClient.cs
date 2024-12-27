using MetalMaxSystem;
using System;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace SC2Crawler
{
    public class BilibiliBarrageClient
    {
        private readonly ClientWebSocket _webSocket = new ClientWebSocket();
        private readonly Uri _barrageServerUri;

        public BilibiliBarrageClient(string roomId)
        {
            _barrageServerUri = new Uri($"wss://broadcastlv.chat.bilibili.com/sub?room_id={roomId}&platform=web");
        }

        public async Task ConnectAsync(CancellationToken cancellationToken)
        {
            await _webSocket.ConnectAsync(_barrageServerUri, cancellationToken);
            await ReceiveMessages(cancellationToken);
        }

        private async Task ReceiveMessages(CancellationToken cancellationToken)
        {
            var buffer = new byte[1024 * 4];
            while (_webSocket.State == WebSocketState.Open)
            {
                WebSocketReceiveResult result;
                var segment = new ArraySegment<byte>(buffer);
                do
                {
                    result = await _webSocket.ReceiveAsync(segment, cancellationToken);
                    var message = System.Text.Encoding.UTF8.GetString(buffer, 0, result.Count);
                    //处理接收到的弹幕数据
                    HandleMessage(message);
                }
                while (!result.EndOfMessage);
            }
        }

        private void HandleMessage(string message)
        {
            //解析弹幕数据并进行处理
            MMCore.Tell(message);
        }

        /// <summary>
        /// 异步方式获取弹幕
        /// </summary>
        /// <param Name="roomID">房间号</param>
        /// <returns></returns>
        public static async Task GetDanmu(string roomID)
        {
            var client = new BilibiliBarrageClient(roomID);
            try
            {
                //CancellationToken.None: 这是一个CancellationToken实例，用于通知异步操作何时应该被取消
                //在这个例子中，CancellationToken.None意味着这个操作不应该被取消，或者没有提供取消操作的机制
                await client.ConnectAsync(CancellationToken.None);
            }
            catch (Exception ex)
            {
                MMCore.Tell(ex.Message);
            }
        }

        /// <summary>
        /// 添加重连逻辑的ReceiveMessages
        /// </summary>
        /// <param Name="cancellationToken"></param>
        /// <returns></returns>
        private async Task ReceiveMessagesAgain(CancellationToken cancellationToken)
        {
            while (true)
            {
                try
                {
                    var buffer = new byte[1024 * 4];
                    WebSocketReceiveResult result;
                    var segment = new ArraySegment<byte>(buffer);
                    do
                    {
                        result = await _webSocket.ReceiveAsync(segment, cancellationToken);
                        var message = System.Text.Encoding.UTF8.GetString(buffer, 0, result.Count);
                        HandleMessage(message);
                    }
                    while (!result.EndOfMessage);
                }
                catch (WebSocketException ex)
                {
                    MMCore.Tell($"WebSocketException: {ex.Message}");
                    //在这里添加重连逻辑
                    await Task.Delay(TimeSpan.FromSeconds(5)); //等待一段时间后尝试重新连接
                    await ConnectAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    MMCore.Tell(ex.Message);
                    break; //其他异常导致退出循环
                }
            }
        }
    }
}

using Microsoft.AspNet.SignalR;
using NativeCommand;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LiteRemote
{
    public class MyRemoteHub : Hub
    {
        public void Ping()
        {
            Console.WriteLine("PING");
        }

        public override Task OnConnected()
        {
            string ip = getIpAddress();
            Console.WriteLine("Connection from : " + ip);
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            string ip = getIpAddress();
            Console.WriteLine("Disconnected from : " + ip);
            return base.OnDisconnected(stopCalled);
        }

        public void L_Click(int x, int y)
        {
            NativeIOCommand.Instance.SendLeftClick(x, y);
        }

        public void R_Click(int x, int y)
        {
            NativeIOCommand.Instance.SendRightClick(x, y);
        }

        public void SequenceKeyPress(string sequence)
        {
            var tokens = sequence.Split('+').Select(x => x.Trim().ToUpper()).ToList();
            var keyCodes = tokens.Select(x => KeyMapping.KeyCode[x]).ToList();
            NativeIOCommand.Instance.SendKeyboardSequence(keyCodes);
        }

        public void MessageKeyPress(string message)
        {
            for(int i = 0; i < message.Length; i++)
            {
                NativeIOCommand.Instance.SendKeyboard(KeyMapping.KeyCode[("" + message[i]).ToUpper()]);
            }
        }

        public string Refresh()
        {
            var bmpScreenshot = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, PixelFormat.Format32bppArgb);
            var gfxScreenshot = Graphics.FromImage(bmpScreenshot);
            gfxScreenshot.CopyFromScreen(Screen.PrimaryScreen.Bounds.X, Screen.PrimaryScreen.Bounds.Y, 0, 0, Screen.PrimaryScreen.Bounds.Size, CopyPixelOperation.SourceCopy);
            using(var ms = new MemoryStream())
            {
                bmpScreenshot.Save(ms, ImageFormat.Png);
                bmpScreenshot.Dispose();

                var imgBase64 = Convert.ToBase64String(ms.ToArray());
                return "data:image/png;base64," + imgBase64;
            }
        }

        private string getIpAddress()
        {
            object ipAddress;
            Context.Request.Environment.TryGetValue("server.RemoteIpAddress", out ipAddress);
            if (ipAddress == null)
                return "unknown";
            else
            {
                if (string.IsNullOrEmpty((string)ipAddress))
                    return "empty";
                else
                    return (string)ipAddress;
            }
        }
    }
}

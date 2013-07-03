using System.Text;
using System.Net;
using System.Net.Sockets;

namespace DTNHistoryBridge.Helpers
{
    public class BufferedSocket
    {

        private Buffer buffer;
        private Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        private string Host;
        private int Port;
        private SocketLineHandler slh;

        public BufferedSocket(string Host, int Port, SocketLineHandler slh)
        {
            this.Host = Host;
            this.Port = Port;
            this.slh = slh;
        }

        public void Add(SocketLineHandler slh)
        {
            buffer.Add(slh);
        }

        public void Open()
        {
            IPAddress ipAddress = IPAddress.Parse(Host);
            IPEndPoint ipep = new IPEndPoint(ipAddress, Port);
            sock.Connect(ipep);

            buffer = new Buffer(sock);
            buffer.Add(slh);
            buffer.Open();
        }

        public void Send(string s)
        {
            if(sock == null)
            {
                Close();
                sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                Open();
            }

            sock.Send(Encoding.ASCII.GetBytes(s));
        }

        public void Remove(SocketLineHandler slh)
        {
            buffer.Remove(slh);
        }

        public void Close()
        {
            if (buffer != null)
                buffer.Close();

            buffer = null;
            slh = null;

            if (sock != null)
            {
                sock.Shutdown(SocketShutdown.Both);
                sock.Close();
                sock = null;
            }
        }
    }
}
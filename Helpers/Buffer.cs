using System;
using System.Text;
using System.Net.Sockets;

namespace DTNHistoryBridge.Helpers
{

    public delegate void SocketLineHandler(object source, BufferArgs args);  // can store static or object references

    public class BufferArgs : EventArgs
    {

        private readonly string line;  // string to be processed
        public string[] items;  // for split items 
        public string headline;  // news event will update this headline

        public BufferArgs(string line)
        {
            this.line = line;
        }

        public string Line
        {
            get { return line; }
        }
    }

    public class Buffer
    {

        public static event EventHandler<UnhandledExceptionEventArgs> UnhandledException;

        private static void OnUnhandledException(object exception, bool isTerminating)
        {
            if (UnhandledException != null)
                UnhandledException(null, new UnhandledExceptionEventArgs(exception, isTerminating));
        }

        private const int blocksize = 8192;				 // max we can get at one time
        private byte[] buf;									 // buffer where the socket puts stuff
        private string sPartialLine = "";		 // we build our strings here
        private readonly Socket sock;

        private event SocketLineHandler HandleSocketLine;
        private IAsyncResult asyncResult;

        private bool ProcessingBuffer;

        internal bool bClosed = true;
        private bool bClosing ;
        private bool bInReceive = false;

        public Buffer(Socket sock)
        {
            this.sock = sock;
            bClosing = false;
        }

        public void Add(SocketLineHandler slh)
        {
            HandleSocketLine += slh;
        }

        public void Open()
        {
            buf = new byte[blocksize];
            bClosed = false;
            asyncResult = sock.BeginReceive(buf, 0, blocksize, SocketFlags.None, new AsyncCallback(cbProcessBuffer), this);
        }

        public void Remove(SocketLineHandler slh)
        {
            HandleSocketLine -= slh;
        }

        public void Close()
        {
            bClosed = true;
            if (ProcessingBuffer)
            {
                bClosing = true;
            }
            else
            {
                HandleSocketLine = null;
                buf = null;
            }
        }

        public static void cbProcessBuffer(IAsyncResult ar)
        {

            try
            {
                Buffer buffer = (Buffer)ar.AsyncState; 
                if (buffer.bClosed)
                {
                }
                else
                {

                    if (buffer.ProcessingBuffer)
                    {
                    }

                    buffer.ProcessingBuffer = true;

                    if (!buffer.sock.Connected)
                    {
                        throw new Exception("cbProcessBuffer socket is closed");
                    }

                    int cntBytesReceived = buffer.sock.EndReceive(ar);
                    buffer.bInReceive = false;

                    if (cntBytesReceived > 0)
                    {
                        int i;  
                        for (i = 0; i < cntBytesReceived; i++)
                        {
                            try
                            {
                                char[] ch = Encoding.ASCII.GetChars(buffer.buf, i, 1);  //each character of the buffer is checked and moved
                                if (0x0a == ch[0])
                                {
                                    BufferArgs args = new BufferArgs(buffer.sPartialLine);

                                    if (null != buffer.HandleSocketLine) 
                                        buffer.HandleSocketLine(buffer, args);
                                    
                                    buffer.sPartialLine = "";
                                }
                                else
                                {
                                    if (0x0d == ch[0])
                                    {
                                    }
                                    else
                                    {
                                        // move the character to the parial line and try for another
                                        buffer.sPartialLine += ch[0];
                                        //Console.Write(ch[0]);
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(@"Buffer::cbProcessBuffer Exception : error = {0}", e.Message);
                                throw new Exception("cbProcessBuffer Exception: cntBytesReceived=" + cntBytesReceived
                                  + ", i=" + i
                                  + ", buffer.buf=" + buffer.buf
                                  + ", e=" + e
                                );
                            }
                        }
                    }
                    buffer.ProcessingBuffer = false;
                }
                if (buffer.bClosing)
                {
                    buffer.Close();
                }
                else
                {
                    if (!buffer.bClosed)
                    {
                        buffer.bInReceive = true;
                        buffer.asyncResult =
                          buffer.sock.BeginReceive(buffer.buf, 0, blocksize, SocketFlags.None, new AsyncCallback(cbProcessBuffer), buffer);
                    }
                }

            }

            catch (Exception e)
            {
                Console.WriteLine(@"Buffer::cbProcessBuffer Exception {0}", e);
                OnUnhandledException(e, true);
            }
        }

    }
}


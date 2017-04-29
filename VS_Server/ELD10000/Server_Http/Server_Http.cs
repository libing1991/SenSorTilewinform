using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Net;
using System.Threading;
using System.Collections;

namespace Server_Http
{
    public struct HttpRequestInfo
    {
        public String http_method;
        public String http_url;
        public String http_data;
        public String http_IP; 
    }

    public class HttpProcessor
    {
        public TcpClient socket;
        public HttpServer srv;

        private Stream inputStream;
        public StreamWriter outputStream;

        public String http_method;
        public String http_url;
        public String http_protocol_versionstring;
        public Hashtable httpHeaders = new Hashtable();

        HttpRequestInfo info = new HttpRequestInfo();


        private static int MAX_POST_SIZE = 10 * 1024 * 1024; // 10MB

        public HttpProcessor(TcpClient s, HttpServer srv)
        {
            this.socket = s;
            this.srv = srv;
        }


        private string streamReadLine(Stream inputStream)
        {
            int next_char;
            string data = "";
            while (true)
            {
                next_char = inputStream.ReadByte();
                if (next_char == '\n') { break; }
                if (next_char == '\r') { continue; }
                if (next_char == -1) { Thread.Sleep(1); continue; };
                data += Convert.ToChar(next_char);
            }
            return data;
        }
        public void process()
        {

            try
            {
                // we can't use a StreamReader for input, because it buffers up extra data on us inside it's
                // "processed" view of the world, and we want the data raw after the headers
                //获取客户端IP
                IPEndPoint ip = (IPEndPoint)socket.Client.RemoteEndPoint;
                string IpStr = ip.Address.ToString();//IP地址点分表达方式
                info.http_IP = IpStr;


                inputStream = new BufferedStream(socket.GetStream());

                // we probably shouldn't be using a streamwriter for all output from handlers either
                outputStream = new StreamWriter(new BufferedStream(socket.GetStream()));
                try
                {
                    parseRequest();
                    readHeaders();
                    if (http_method.Equals("GET"))
                    {
                        handleGETRequest();
                    }
                    else if (http_method.Equals("POST"))
                    {
                        handlePOSTRequest();
                    }
                }
                catch
                {
                    // Console.WriteLine("Exception: " + e.ToString());
                    writeFailure();
                }
                outputStream.Flush();
                // bs.Flush(); // flush any remaining output
                inputStream = null; outputStream = null; // bs = null;            
                socket.Close();
            }
            catch
            {
                Console.WriteLine("process err!");
            }
        }

        public void parseRequest()
        {
            String request = streamReadLine(inputStream);
            string[] tokens = request.Split(' ');
            if (tokens.Length != 3)
            {
                throw new Exception("invalid http request line");
            }
            http_method = tokens[0].ToUpper();
            info.http_method = http_method;
            http_url = tokens[1];
            info.http_url = http_url;
            http_protocol_versionstring = tokens[2];

            //Console.WriteLine("starting: " + request);
        }

        public void readHeaders()
        {
            //Console.WriteLine("readHeaders()");
            String line;
            while ((line = streamReadLine(inputStream)) != null)
            {
                if (line.Equals(""))
                {
                    // Console.WriteLine("got headers");
                    return;
                }

                int separator = line.IndexOf(':');
                if (separator == -1)
                {
                    throw new Exception("invalid http header line: " + line);
                }
                String name = line.Substring(0, separator);
                int pos = separator + 1;
                while ((pos < line.Length) && (line[pos] == ' '))
                {
                    pos++; // strip any spaces
                }

                string value = line.Substring(pos, line.Length - pos);
                //Console.WriteLine("header: {0}:{1}", name, value);
                httpHeaders[name] = value;
            }
        }

        public void handleGETRequest()
        {
            String http_url = info.http_url;
            if (http_url.IndexOf("?") == -1)
            {
                info.http_data = "";
            }
            else if (http_url.IndexOf("?") - http_url.IndexOf("/") == 1)
            {
                info.http_url = "/";
                info.http_data = http_url.Substring(2);
            }
            else
            {
                info.http_url = http_url.Substring(0, http_url.IndexOf("?"));
                info.http_data = http_url.Substring(http_url.IndexOf("?") + 1);
            }

            srv.handleGETRequest(this, info);
        }

        private const int BUF_SIZE = 4096;
        public void handlePOSTRequest()
        {
            // this post data processing just reads everything into a memory stream.
            // this is fine for smallish things, but for large stuff we should really
            // hand an input stream to the request processor. However, the input stream 
            // we hand him needs to let him see the "end of the stream" at this content 
            // length, because otherwise he won't know when he's seen it all! 

            //Console.WriteLine("get post data start");
            int content_len = 0;
            MemoryStream ms = new MemoryStream();
            if (this.httpHeaders.ContainsKey("Content-Length"))
            {
                content_len = Convert.ToInt32(this.httpHeaders["Content-Length"]);
                if (content_len > MAX_POST_SIZE)
                {
                    throw new Exception(
                        String.Format("POST Content-Length({0}) too big for this simple server",
                          content_len));
                }
                byte[] buf = new byte[BUF_SIZE];
                int to_read = content_len;
                while (to_read > 0)
                {
                    // Console.WriteLine("starting Read, to_read={0}", to_read);

                    int numread = this.inputStream.Read(buf, 0, Math.Min(BUF_SIZE, to_read));
                    //Console.WriteLine("read finished, numread={0}", numread);
                    if (numread == 0)
                    {
                        if (to_read == 0)
                        {
                            break;
                        }
                        else
                        {
                            throw new Exception("client disconnected during post");
                        }
                    }
                    to_read -= numread;
                    ms.Write(buf, 0, numread);
                }
                ms.Seek(0, SeekOrigin.Begin);
            }
            info.http_data = new StreamReader(ms).ReadToEnd();
            //Console.WriteLine("get post data end");
            srv.handlePOSTRequest(this, info);

        }

        public void writeSuccess()
        {
            outputStream.WriteLine("HTTP/1.0 200 OK");
            outputStream.WriteLine("Content-Type: text/html");
            outputStream.WriteLine("Connection: close");
            outputStream.WriteLine("");
        }

        public void writeFailure()
        {
            outputStream.WriteLine("HTTP/1.0 404 File not found");
            outputStream.WriteLine("Connection: close");
            outputStream.WriteLine("");
        }
    }
    public abstract class HttpServer
    {

        protected int port;
        TcpListener listener;
        bool is_active = true;

        public HttpServer(int port)
        {
            this.port = port;
        }

        public void listen()
        {
            //string Ip = "192.168.191.1";
            //string Ip = "10.104.59.80";
            string Ip = "112.74.213.160";
            Console.WriteLine("listen:"+Ip+":"+port);
            listener = new TcpListener(IPAddress.Parse(Ip), port);
            listener.Start();
            while (is_active)
            {
                try
                {
                    TcpClient s = listener.AcceptTcpClient();
                    HttpProcessor processor = new HttpProcessor(s, this);
                    Console.WriteLine("Add Con");

                    Thread thread = new Thread(new ThreadStart(processor.process));
                    thread.Start();
                }
                catch
                {
                    Console.WriteLine("listen err!");
                }
                //Thread.Sleep(1);
            }
        }

        public abstract void handleGETRequest(HttpProcessor p, HttpRequestInfo info);
        public abstract void handlePOSTRequest(HttpProcessor p, HttpRequestInfo info);
    }
}

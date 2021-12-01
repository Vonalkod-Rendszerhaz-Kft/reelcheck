using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Vrh.CameraService.EventHubContract;
using Vrh.EventHub.Core;
using Vrh.EventHub.Protocols.RedisPubSub;
using StackExchange.Redis;
using ReelCheck.Core;
using System.Diagnostics;

namespace CameraServiceTester
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(new DateTime(2018, 2, 1).WeekNo());

            Stopwatch stopwatch = new Stopwatch();                        
            do
            {
                stopwatch.Restart();
                Thread.Sleep(1000);
                var e = stopwatch.Elapsed;
                Console.WriteLine(e);
                Console.WriteLine(e.ToString("s\\.f"));
                Console.WriteLine(e.ToString("s\\.ff"));
                Console.WriteLine(e.ToString("s\\.fff"));
                Console.WriteLine(e.ToString("s\\.ffff"));
                Console.WriteLine(e.ToString("s\\.fffff"));
                //for (int i = 0; i < 5; i++)
                //{
                //    Task.Run(() => ReadID());
                //    Task.Run(() => ReadCheck());
                //}
                if (Console.ReadLine() == "e")
                {
                    return;
                }
            } while (true);



            //EventHubCore.Send<RedisPubSubChannel, int>("LABELD_ID", 0);
            if (Connection.IsConnected)
            {
                Console.WriteLine("connected");
            }
            else
            {
                Console.WriteLine("NOT connected");
            }
            EventHubCore.RegisterHandler<RedisPubSubChannel, IOEXTIOStateMessageResult>("LABELD_ID", IOInputChanged);
            EventHubCore.RegisterHandler<RedisPubSubChannel, IOChangeMessageResult>("LABELD_ID", IOInputChanged);
            EventHubCore.RegisterHandler<RedisPubSubChannel, SetIOMessage, CommonMessageResult>("LABELD_ID", SetIOEvent);
            EventHubCore.RegisterHandler<RedisPubSubChannel, SetIOEXTIOMessage, CommonMessageResult>("LABELD_ID", SetIOExtEvent);
            Console.WriteLine($"Write Exit for EXIT!");
            do
            {
                string command = Console.ReadLine();
                command = command.ToLower();
                switch (command)
                {
                    case "clear":
                        Console.Clear();
                        break;
                    case "exit":
                        return;
                    //EventHubCore.Call<RedisPubSubChannel, io>
                    default:
                        string[] parts = command.Split(' ');
                        if (parts.Length > 0)
                        {
                            if (parts[0] == "io")
                            {
                                if (parts.Length == 3)
                                {
                                    Bulbs b = parts[2] == "1" ? Bulbs.Green : Bulbs.Red;
                                    EventHubCore.Call<RedisPubSubChannel, SetIOMessage, CommonMessageResult>("LABELD_ID", new SetIOMessage(b, false));
                                }
                            }
                            if (parts[0] == "r")
                            {
                            }

                        }
                        break;
                }

            } while (true);

            do
            {
                //Console.Clear();
                Console.WriteLine("Number of test runs (leave empty for exit):");
                string strNum = Console.ReadLine();
                if (String.IsNullOrEmpty(strNum))
                {
                    //return;
                }
                else
                {
                    int.TryParse(strNum, out _testruns);
                }
                _round = 0;
                for (int i = 0; i < _testruns; i++)
                {
                    Task.Factory.StartNew(() => RunTest(), TaskCreationOptions.LongRunning);
                    //Task.Run(() => RunTest());
                }
                _waitForTestRuns.WaitOne();
                Console.WriteLine($"All tests {_testruns} passed");
                Console.WriteLine($"Press a key for new round!");
                Console.ReadKey();
                _round = 0;
                _testruns = 1;
            } while (true);
        }

        private static void ReadCheck()
        {
            try
            {
                var readData = EventHubCore.Call<RedisPubSubChannel, ReadMessage, LabelDataMessageResult>("LABELD_CHECK", new ReadMessage(), new TimeSpan(0, 0, 10));
                Console.WriteLine($"CHECK: {readData.LabelData}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void ReadID()
        {
            try
            {
                var readData = EventHubCore.Call<RedisPubSubChannel, ReadMessage, LabelDataMessageResult>("LABELD_ID", new ReadMessage(), new TimeSpan(0, 0, 10));
                Console.WriteLine($"ID: {readData.LabelData}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static Response<CommonMessageResult> SetIOExtEvent(Request<SetIOEXTIOMessage, CommonMessageResult> msg)
        {
            string meaning = "";
            switch (msg.RequestContent.IOEXTPort)
            {
                case 0:
                    meaning = "ALLSTATION FINISHED";
                    break;
                case 1:
                    meaning = "WORKPIECE OK";
                    break;
                case 2:
                    meaning = "WORKPIECE NOK";
                    break;
                case 3:
                    meaning = "EMPTY";
                    break;
            }
            Console.WriteLine($"<---{DateTime.Now.ToString("hh:mm:ss.fffffff")}: {meaning} {msg.RequestContent.TurneOn}");
            var resp = msg.MyResponse;
            resp.ResponseContent = new CommonMessageResult();
            return resp;
        }

        private static Response<CommonMessageResult> SetIOEvent(Request<SetIOMessage, CommonMessageResult> msg)
        {
            string meaning = "";
            switch (msg.RequestContent.Bulb)
            {
                case Bulbs.Green:
                    meaning = "READY";
                    break;
                case Bulbs.Red:
                    meaning = "LABEL PRINTED";
                    break;
            }
            Console.WriteLine($"<---{DateTime.Now.ToString("hh:mm:ss.fffffff")}: {meaning} {msg.RequestContent.BulbTurneOn}");
            var resp = msg.MyResponse;
            resp.ResponseContent = new CommonMessageResult();
            return resp;
        }

        private static void IOInputChanged(IOChangeMessageResult msg)
        {
            Console.WriteLine($"--->{DateTime.Now.ToString("hh:mm:ss.fffffff")}: EANBLE {msg.IOState}");
        }

        private static void IOInputChanged(IOEXTIOStateMessageResult msg)
        {
            string meaning = String.Empty;
            switch (msg.IOEXTIOPort)
            {
                case 0:
                    meaning = "STICKING DONE";
                    break;
                case 1:
                    meaning = "STATUS RESET";
                    break;
                case 2:
                    meaning = "HARD RESET";
                    break;
                default:
                    meaning = $"IOEXT{msg.IOEXTIOPort.ToString()}";
                    break;
            }
            Console.WriteLine($"--->{DateTime.Now.ToString("hh:mm:ss.fffffff")}: {meaning} {msg.IOEXTIOState}");
        }

        async static void RunTest()
        {
            lock (_staticlocker)
            {
                TestType type = TestType.SetIO;
                byte port = 0;
                bool high = true;
                TimeSpan ts = new TimeSpan(0, 0, 0);
                //type = (TestType)_rnd.Next(1, 5);
                switch (type)
                {
                    case TestType.SetIOExt:
                        high = _rnd.Next(0, 2) == 0 ? false : true;
                        port = (byte)_rnd.Next(0, 4);
                        break;
                    case TestType.GetIOExt:
                        port = (byte)_rnd.Next(0, 4);
                        break;
                    case TestType.SetIO:
                        port = (byte)_rnd.Next(0, 2);
                        high = _rnd.Next(0, 2) == 0 ? false : true;
                        break;
                }
                try
                {
                    switch (type)
                    {
                        case TestType.SetIO:
                            ts = SetIO((Bulbs)port, high);
                            break;
                        case TestType.SetIOExt:
                            ts = SetIOExt(port, high);
                            break;
                        case TestType.GetIO:
                            //ts = GetIO();
                            break;
                        case TestType.GetIOExt:
                            ts = GetIOExt(port);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    ts = new TimeSpan(0, 0, 10);
                    var originalbgColor = Console.BackgroundColor;
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex.Message);
                    Console.BackgroundColor = originalbgColor;
                }
                _round++;
                Console.WriteLine($"{_round}: {type} (t: {ts.TotalMilliseconds}, port: {port}, high: {high}");
                if (_round == _testruns)
                {
                    _waitForTestRuns.Set();
                }
            }
        }

        static private TimeSpan SetIO(Bulbs bulb, bool high)
        {
            Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId}");
            SetIOMessage request = new SetIOMessage(bulb, high);
            //Connection.GetDatabase();
            DateTime start = DateTime.Now;
            EventHubCore.Call<RedisPubSubChannel, SetIOMessage, CommonMessageResult>("LABELD_ID", request, new TimeSpan(0, 0, 10));
            return DateTime.Now.Subtract(start);
        }

        private static Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            return ConnectionMultiplexer.Connect("localhost");
        });

        public static ConnectionMultiplexer Connection
        {
            get
            {
                return lazyConnection.Value;
            }
        }

        static private TimeSpan GetIO()
        {
            GetIOMessage request = new GetIOMessage();
            DateTime start = DateTime.Now;
            EventHubCore.Call<RedisPubSubChannel, GetIOMessage, IOChangeMessageResult>("LABELD_ID", request, new TimeSpan(0, 0, 10));
            return DateTime.Now.Subtract(start);
        }

        static private TimeSpan GetIOExt(byte port)
        {
            GetIOEXTIOMessage request = new GetIOEXTIOMessage(port);
            DateTime start = DateTime.Now;
            EventHubCore.Call<RedisPubSubChannel, GetIOEXTIOMessage, IOEXTIOStateMessageResult>("LABELD_ID", request, new TimeSpan(0, 0, 10));
            return DateTime.Now.Subtract(start);
        }

        static private TimeSpan SetIOExt(byte port, bool high)
        {
            SetIOEXTIOMessage request = new SetIOEXTIOMessage(port, high);
            DateTime start = DateTime.Now;
            EventHubCore.Call<RedisPubSubChannel, SetIOEXTIOMessage, CommonMessageResult>("LABELD_ID", request, new TimeSpan(0, 0, 10));
            return DateTime.Now.Subtract(start);

        }

        static AutoResetEvent _waitForTestRuns = new AutoResetEvent(false);

        static private Random _rnd = new Random();

        static private int _round = 0;

        static private int _testruns = 1;

        static object _staticlocker = new object();
    }

    public enum TestType
    {
        SetIO = 1,
        SetIOExt = 2,
        GetIO = 3,
        GetIOExt = 4,
    }
}

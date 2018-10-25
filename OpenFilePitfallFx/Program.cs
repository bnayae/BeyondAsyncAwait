using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

#pragma warning disable SG0018 // Path traversal
namespace Bnaya.Samples
{
    internal class Program
    {
        private const string SOURCE_FILE_NAME = "sample.txt";
        private const string TARGET_FILE_NAME = "sample.copy.txt";
        private const int FILE_SIZE = 50 * 1024 * 1024; // 50 mb
        private const int BUFFER_SIZE = 8192;
        private const int CONSOLE_RATIO = 100;
        private static bool _openForAsync = false;

        private static void Main(string[] args)
        {
            Console.WriteLine(".NET Core don't include the file's APM API");
            ThreadPool.QueueUserWorkItem(s => { });
            CreateLargeFile();
            var cts = new CancellationTokenSource();
            Console.WriteLine("1. APM (not open for async)");
            Console.WriteLine("2. TPL (not open for async)");
            Console.WriteLine("3. APM (open for async)");
            Console.WriteLine("4. TPL (open for async)");

            char c = Console.ReadKey().KeyChar;
            Task _ = MonitorAsync(cts.Token);
            switch (c)
            {
                case '1':
                    APMWithFiles(); // This is a problem (only in APM)
                    break;
                case '2':
                    TPLWithFiles();
                    break;
                case '3':
                    _openForAsync = true;
                    APMWithFiles(); // This is a problem (only in APM)
                    break;
                case '4':
                    _openForAsync = true;
                    TPLWithFiles();
                    break;
                default:
                    Console.WriteLine($"{c} is not valid option");
                    break;
            }

            Console.WriteLine("\r\nCopy completed");
            cts.Cancel();
            Console.ReadKey();
        }

        #region CreateLargeFile

        /// <summary>
        /// Creates a large file for demonstration purposes.
        /// </summary>
        private static void CreateLargeFile()
        {
            //File.Delete(SOURCE_FILE_NAME);
            File.Delete(TARGET_FILE_NAME);

            if (!File.Exists(SOURCE_FILE_NAME))
            {
                using (StreamWriter writer = new StreamWriter(SOURCE_FILE_NAME))
                {
                    for (int i = 0; i < 5000000; ++i)
                    {
                        writer.WriteLine(i);
                    }
                }
            }
        }

        #endregion // CreateLargeFile

        #region APMWithFiles

        /// <summary>
        /// Demonstrates the use of the APM with files, through the FileStream class.
        /// This method performs asynchronous reads and writes to copy data from an input
        /// file to an output file.  Reads and writes are interlaced, and proceed in chunks
        /// of 8KB at a time (displaying progress to the console).
        /// </summary>
        private static void APMWithFiles()
        {
            FileStream reader, writer;
            int i = 0;
            #region reader = new FileStream(...), writer = new FileStream(...)

            if (_openForAsync)
            {
                reader = new FileStream(SOURCE_FILE_NAME, FileMode.Open, FileAccess.Read, FileShare.Read, BUFFER_SIZE,
                                        FileOptions.Asynchronous);
                writer = new FileStream(TARGET_FILE_NAME, FileMode.Create, FileAccess.Write, FileShare.None, BUFFER_SIZE,
                                        FileOptions.Asynchronous);
            }
            else
            {
                reader = File.OpenRead(SOURCE_FILE_NAME);   // DO NOT USE THIS API FOR ASYNC OPERATIONS !!!
                writer = File.OpenWrite(TARGET_FILE_NAME);      // DO NOT USE THIS API FOR ASYNC OPERATIONS !!!
            }

            #endregion // reader = new FileStream(...), writer = new FileStream(...)
            using (reader)
            using (writer)
            {
                byte[] buffer1 = new byte[BUFFER_SIZE];
                while (true)
                {
                    i++;
                    IAsyncResult ar1 = reader.BeginRead(buffer1, 0, buffer1.Length, null, null);
                    do
                    {
                        if (i % CONSOLE_RATIO == 0)
                            Console.Write("R");
                    } while (!ar1.IsCompleted);

                    int bytesRead;
                    if ((bytesRead = reader.EndRead(ar1)) == 0)
                        break;  //No more data to read

                    IAsyncResult ar2 = writer.BeginWrite(buffer1, 0, bytesRead, null, null);
                    do
                    {
                        if (i % CONSOLE_RATIO == 0)
                            Console.Write("W");
                    } while (!ar2.IsCompleted);
                }
            }
        }


        #endregion // APMWithFiles

        #region TPLWithFiles

        /// <summary>
        /// Demonstrates the use of the Task with files, through the FileStream class.
        /// This method performs asynchronous reads and writes to copy data from an input
        /// file to an output file.  Reads and writes are interlaced, and proceed in chunks
        /// of 8KB at a time (displaying progress to the console).
        /// </summary>
        private static void TPLWithFiles()
        {
            int i = 0;
            FileStream reader, writer;
            #region reader = new FileStream(...), writer = new FileStream(...)

            if (_openForAsync)
            {
                reader = new FileStream(SOURCE_FILE_NAME, FileMode.Open, FileAccess.Read, FileShare.Read, BUFFER_SIZE,
                                        FileOptions.Asynchronous);
                writer = new FileStream(TARGET_FILE_NAME, FileMode.Create, FileAccess.Write, FileShare.None, BUFFER_SIZE,
                                        FileOptions.Asynchronous);
            }
            else
            {
                reader = File.OpenRead(SOURCE_FILE_NAME);   // DO NOT USE THIS API FOR ASYNC OPERATIONS !!!
                writer = File.OpenWrite(TARGET_FILE_NAME);      // DO NOT USE THIS API FOR ASYNC OPERATIONS !!!
            }

            #endregion // reader = new FileStream(...), writer = new FileStream(...)
            using (reader)
            using (writer)
            {
                byte[] buffer1 = new byte[BUFFER_SIZE];
                while (true)
                {
                    i++;
                    Task<int> tr = reader.ReadAsync(buffer1, 0, buffer1.Length);
                    do
                    {
                        if (i % CONSOLE_RATIO == 0)
                            Console.Write("R");
                    } while (!tr.IsCompleted);

                    int bytesRead;
                    if ((bytesRead = tr.Result) == 0)
                        break;  //No more data to read

                    Task ar2 = writer.WriteAsync(buffer1, 0, bytesRead);
                    do
                    {
                        if (i % CONSOLE_RATIO == 0)
                            Console.Write("W");
                    } while (!ar2.IsCompleted);
                }
            }
        }


        #endregion // TPLWithFiles

        #region WriteItRight

        private static async Task WriteItRight(string fileName)
        {
            using (Stream srm = new FileStream(TARGET_FILE_NAME, FileMode.Create,
                FileAccess.Write, FileShare.None, BUFFER_SIZE,
                FileOptions.Asynchronous /*| FileOptions.WriteThrough*/))
            using (StreamWriter writer = new StreamWriter(srm))
            {
                for (int i = 0; i < 100000; i++)
                {
                    await writer.WriteAsync("I will never cause ThreadPool starvation...").ConfigureAwait(false);
                }
            }
        }

        #endregion // WriteItRight

        #region MonitorAsync

        private static async Task MonitorAsync(CancellationToken ct)
        {

            ThreadPool.GetMaxThreads(out var wMax, out var ioMax);
            while (!ct.IsCancellationRequested)
            {
                ThreadPool.GetAvailableThreads(out var w, out var io);
                int ioCount = ioMax - io;
                if (ioCount != 0)
                {
                    Console.WriteLine($"\r\nIn Used:[Workers = {wMax - w}, IO = {ioMax - io}]");
                }
                await Task.Delay(1).ConfigureAwait(false);
            }
        }

        #endregion // MonitorAsync
    }
}

//#define OK

using System;
using System.IO;
using System.Threading.Tasks;

namespace Bnaya.Samples
{
    class Program
    {
        private const string SOURCE_FILE_NAME = "sample.txt";
        private const string TARGET_FILE_NAME = "sample.copy.txt";
        private const int FILE_SIZE = 50 * 1024 * 1024; // 50 mb
        private const int BUFFER_SIZE = 8192;

        static void Main(string[] args)
        {
            Console.WriteLine(".NET Core don't include the file's APM API");

            CreateLargeFile();
            APMWithFiles(); // This is a problem (only in APM)
            //TPLWithFiles();

            Console.WriteLine("\r\nCopy completed");
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
        static void APMWithFiles()
        {
#if OK
            using (FileStream reader = new FileStream(SOURCE_FILE_NAME, FileMode.Open, FileAccess.Read, FileShare.Read, BUFFER_SIZE, true))
            using (Stream writer = new FileStream(TARGET_FILE_NAME, FileMode.Create, FileAccess.Write, FileShare.None, BUFFER_SIZE, true))
#else
            using (FileStream reader = File.OpenRead(SOURCE_FILE_NAME))   // DO NOT USE THIS API FOR ASYNC OPERATIONS !!!
            using (Stream writer = File.OpenWrite(TARGET_FILE_NAME))      // DO NOT USE THIS API FOR ASYNC OPERATIONS !!!
#endif
            {
                byte[] buffer1 = new byte[BUFFER_SIZE];
                while (true)
                {
                    IAsyncResult ar1 = reader.BeginRead(buffer1, 0, buffer1.Length, null, null);
                    do
                    {
                        Console.Write("R");
                    } while (!ar1.IsCompleted);

                    int bytesRead;
                    if ((bytesRead = reader.EndRead(ar1)) == 0)
                        break;  //No more data to read

                    IAsyncResult ar2 = writer.BeginWrite(buffer1, 0, bytesRead, null, null);
                    do
                    {
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
        static void TPLWithFiles()
        {
#if OK
            using (FileStream reader = new FileStream(SOURCE_FILE_NAME, FileMode.Open, FileAccess.Read, FileShare.Read, BUFFER_SIZE, true))
            using (Stream writer = new FileStream(TARGET_FILE_NAME, FileMode.Create, FileAccess.Write, FileShare.None, BUFFER_SIZE, true))
#else
            using (FileStream reader = File.OpenRead(SOURCE_FILE_NAME))   // DO NOT USE THIS API FOR ASYNC OPERATIONS !!!
            using (Stream writer = File.OpenWrite(TARGET_FILE_NAME))      // DO NOT USE THIS API FOR ASYNC OPERATIONS !!!
#endif
            {
                byte[] buffer1 = new byte[BUFFER_SIZE];
                while (true)
                {
                    Task<int> tr = reader.ReadAsync(buffer1, 0, buffer1.Length);
                    do
                    {
                        Console.Write("R");
                    } while (!tr.IsCompleted);

                    int bytesRead;
                    if ((bytesRead = tr.Result) == 0)
                        break;  //No more data to read

                    Task ar2 = writer.WriteAsync(buffer1, 0, bytesRead);
                    do
                    {
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
                    await writer.WriteAsync("I will never cause ThreadPool starvation...");
                }
            }
        }

        #endregion // WriteItRight
    }
}

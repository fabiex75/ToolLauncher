namespace EnvironmentLauncher
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            if (args != null && args.Contains("--run-tests"))
            {
                RunAutoTests();
                return;
            }

            Application.Run(new MainForm());
        }

        private static void RunAutoTests()
        {
            Console.WriteLine("Starting automated environment launch test...");
            var temp = Path.GetTempPath();
            var file1 = Path.Combine(temp, "env_test_1.txt");
            var file2 = Path.Combine(temp, "env_test_2.txt");

            if (File.Exists(file1)) File.Delete(file1);
            if (File.Exists(file2)) File.Delete(file2);

            try
            {
                var psi1 = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c set > \"{file1}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                psi1.Environment["TEST_VAR"] = "one";

                var psi2 = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c set > \"{file2}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                psi2.Environment["TEST_VAR"] = "two";

                var p1 = System.Diagnostics.Process.Start(psi1);
                var p2 = System.Diagnostics.Process.Start(psi2);

                if (p1 != null)
                    p1.WaitForExit(5000);
                if (p2 != null)
                    p2.WaitForExit(5000);

                // wait a bit for files
                var sw = System.Diagnostics.Stopwatch.StartNew();
                while (( !File.Exists(file1) || !File.Exists(file2) ) && sw.ElapsedMilliseconds < 5000)
                    System.Threading.Thread.Sleep(100);

                var ok1 = File.Exists(file1) && File.ReadAllText(file1).Contains("TEST_VAR=one");
                var ok2 = File.Exists(file2) && File.ReadAllText(file2).Contains("TEST_VAR=two");

                Console.WriteLine($"File1: {file1} exists={File.Exists(file1)} ok={ok1}");
                Console.WriteLine($"File2: {file2} exists={File.Exists(file2)} ok={ok2}");

                if (ok1 && ok2)
                    Console.WriteLine("Automated test PASSED: both processes received their environment variables.");
                else
                    Console.WriteLine("Automated test FAILED: environment variables not found in outputs.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Automated test error: " + ex.Message);
            }
        }
    }
}
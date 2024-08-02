using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using Newtonsoft.Json.Linq;

namespace Noveler
{
    internal static class Program
    {

        public static string sdirectory = "";
        public static bool startopen = false;

        /// <summary>
        /// 해당 애플리케이션의 주 진입점입니다.
        /// </summary>
        [STAThread]

        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form1 f1 = new Form1();

            if (args.Length < 1)
            {
                Application.Run(f1);
                return;
            }
            startopen = true;
            sdirectory = args[0];
            Application.Run(f1);
        }
    }
}

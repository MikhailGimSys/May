using May.MAINWIn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace May
{
    internal static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

			// Открытие окна авторизации
			var avt = new authorization();
            Application.Run(avt);

            // Если авторизация успешная, открытие главного окна
            if(avt.Status == true)
                Application.Run(new main());
        }
	}
}

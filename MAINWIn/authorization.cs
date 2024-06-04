using May.MAINWIn;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Core.Metadata.Edm;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace May
{
    public partial class authorization : Form
    {
        public authorization()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
        }

        // Статус авторизации
        public bool Status = false;


        // Процесс авторизации (запрос данных из базы)
        private bool CheckAvtorization()
        {   
            string loginuser = loginText.Text;
            string passuser = passText.Text;

            DIPLEntities2 context = new DIPLEntities2();
            var result = context.Sotrudnik.Where(x => x.login == loginuser && x.password == passuser).FirstOrDefault();
            if (result != null)
            {
                return true;
            }
            return false;

        }

        // Нажатие на кнопку Войти
        private void loginButton_Click(object sender, EventArgs e)
        {
            Status = CheckAvtorization();
            if (Status == false)
            {
                MessageBox.Show("Некорректно ведены данные");
                return;
            }
            else
            {
                Close();
            }
        }
       
    }
}

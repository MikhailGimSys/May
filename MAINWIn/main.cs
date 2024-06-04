using May.MAINWIn.addwin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace May.MAINWIn
{
    public partial class main : Form
    {
        public main()
        {
            InitializeComponent();
            HeadersCreate();
            ZayavSpisokShow();
            StartPosition = FormStartPosition.CenterScreen;
        }

        // Создание заголовка для списка заявок
        private void HeadersCreate()
        {
            // Создание загововка таблицы 
            dataGridView1.Columns.Add("", "№ Заявки");
            dataGridView1.Columns.Add("", "Дата создания");
            dataGridView1.Columns.Add("", "Клиент");
            dataGridView1.Columns.Add("", "Тип неисправности");
            dataGridView1.Columns.Add("", "Статус");
            dataGridView1.Columns.Add("", "Сотрудник");
            dataGridView1.Columns.Add("", "Описание");
            dataGridView1.Columns.Add("Word", "");
            dataGridView1.Columns.Add("Redact", "");
            dataGridView1.Columns.Add("Del", "");

            dataGridView1.MouseClick += DataGridView1_MouseClick;
        }

        // Вывод списка заявок
        void ZayavSpisokShow()
        {
            // Предварительная очиска списка перед его очередным выводом
            dataGridView1.Rows.Clear();

            // Получение списка заявок из базы
            DIPLEntities2 context = new DIPLEntities2();
            var zaiavkaList = context.ZaiavkaNeispravnost;

            // Вывод данных в датагрид
            foreach (var item in zaiavkaList)
            {
                int i = dataGridView1.Rows.Add
                    (item.id,
                    item.datedob,
                    item.Client.fio,
                    item.TypeNeispravnost.name,
                    item.StatusZaiavka.name,
                    item.Sotrudnik.fio,
                    item.opisanie,
                    "Word",
                    "Изменить",
                    "Удалить");

                // Форматирование ячейки Word
                var cell = dataGridView1["Word", i];
                cell.Style.ForeColor = Color.Blue;
                cell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                cell.Tag = item.id;

                // Форматирование ячейки Redact
                cell = dataGridView1["Redact", i];
                cell.Style.ForeColor = Color.DarkOrange;
                cell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                cell.Tag = item.id;

                // Форматирование ячейки Del
                cell = dataGridView1["Del", i];
                cell.Style.ForeColor = Color.Red;
                cell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                cell.Tag = item.id;
            }
        }


        // Нажатие на Датагрид (Word, Изменить, Удалить)
        private void DataGridView1_MouseClick(object sender, MouseEventArgs e)
        {
            var cell = dataGridView1.SelectedCells[0];
            int id = (int)cell.Tag;
            var dipl = new DIPLEntities2();
            var zayav = dipl.ZaiavkaNeispravnost.Where(x => x.id == id).ToList()[0];

            switch (cell.Value.ToString())
            {
                case "Word":
                    MessageBox.Show("Нажали на Word");
                    break;

                case "Изменить":
                    var redact = new addzaiavka();
                    redact.ID = zayav.id;
                    redact.textBoxCodeZaiavka.Text = zayav.id.ToString();
                    redact.dTPDat.Text = zayav.datedob.ToString();

                    //var item = dipl.StatusZaiavka.Where(x => x.id == zayav.status_id).ToList()[0];
                    //redact.comboBoxStatusZaiavka.SelectedItem = item;
                    //redact.comboBoxNeispravnost.SelectedValue = zayav.neispravost_id.ToString();
                    //redact.comboBoxSotrudnik.SelectedValue = zayav.sotrudnik_id.ToString();
                    //redact.comboBoxClient.SelectedValue = zayav.client_id.ToString();
                    redact.textBoxOpisProm.Text = zayav.opisanie;
                    redact.ShowDialog();
                    ZayavSpisokShow();
                    break;

                case "Удалить":
                    var result = MessageBox.Show(
                        "Вы действтельно хотите удалить запись?",
                        "Удаление записи",
                        MessageBoxButtons.YesNo);
 
                    if (result == DialogResult.Yes)
                    {
                        dipl.ZaiavkaNeispravnost.Remove(zayav);
                        dipl.SaveChanges();
                        ZayavSpisokShow();
                    }
                    break;
            }
        }

        // Нажатие на кнопку Новая заявка
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            addzaiavka add = new addzaiavka();
            add.ShowDialog();
            ZayavSpisokShow();
        }


    }
}



using May.QDataSetTableAdapters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Migrations;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace May.MAINWIn.addwin
{
    public partial class addzaiavka : Form
    {
        public addzaiavka()
        {
            InitializeComponent();
            LoadComboBox();
            StartPosition = FormStartPosition.CenterScreen;

        }

        public int ID = 0;

        // Заполнение КомбоБоксов
        private void LoadComboBox()
        {
            DIPLEntities2 context = new DIPLEntities2();

            // ComboBox Status
            var status = context.StatusZaiavka.ToList();
            comboBoxStatusZaiavka.DataSource = status;
            comboBoxStatusZaiavka.DisplayMember = "Name";
            comboBoxStatusZaiavka.ValueMember = "id";

            // ComboBox Тип неисправности
            comboBoxNeispravnost.DataSource = context.TypeNeispravnost.ToList();
            comboBoxNeispravnost.DisplayMember = "Name";
            comboBoxNeispravnost.ValueMember = "id";

            // ComboBox Сотрудники
            comboBoxSotrudnik.DataSource = context.Sotrudnik.ToList();
            comboBoxSotrudnik.DisplayMember = "fio";
            comboBoxSotrudnik.ValueMember = "id";


            // ComboBox Клиенты
            var client = context.Client.ToList();
            comboBoxClient.DataSource = client;
            comboBoxClient.DisplayMember = "fio";
            comboBoxClient.ValueMember= "id";

        }


        // Закрытие окна
        private void Otmena(object sender, EventArgs e)
        {
            Close();
        }

        // Сохранение данных заявки
        private void Save(object sender, EventArgs e)
        {
            // Создание объекта новой заявки
            var zayav = new ZaiavkaNeispravnost();

            // id заявки. Если равен 0, то создание новой заявки
            zayav.id = ID;

            // Поле дата
            zayav.datedob = DateTime.Now;

            // Статус
            zayav.status_id = (int)comboBoxStatusZaiavka.SelectedValue;

            // Тип неисправности
            zayav.neispravost_id = (int)comboBoxNeispravnost.SelectedValue;

            // Сотрудник
            zayav.sotrudnik_id = (int)comboBoxSotrudnik.SelectedValue;

            // Клиент
            zayav.client_id = (int)comboBoxClient.SelectedValue;

            // Описание
            zayav.opisanie = textBoxOpisProm.Text;

            // Подключение к базе
            var dipl = new DIPLEntities2();

            // Внесение данных заявки в базу (или изменение)
            dipl.ZaiavkaNeispravnost.AddOrUpdate(zayav);

            // Сохранение изменений в базе
            dipl.SaveChanges();

            Close();
        }
    }
}

using System;
using System.Linq;
using System.Windows.Forms;
using System.Data.Entity.Migrations;

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
            var dipl = new DIPLEntities2();

            // ComboBox Status
            comboBoxStatusZaiavka.DataSource = dipl.StatusZaiavka.ToList(); ;
            comboBoxStatusZaiavka.DisplayMember = "Name";
            comboBoxStatusZaiavka.ValueMember = "id";

			// ComboBox Тип неисправности
			comboBoxNeispravnost.DataSource = dipl.TypeNeispravnost.ToList();
            comboBoxNeispravnost.DisplayMember = "Name";
            comboBoxNeispravnost.ValueMember = "id";

            // ComboBox Сотрудники
            comboBoxSotrudnik.DataSource = dipl.Sotrudnik.ToList();
            comboBoxSotrudnik.DisplayMember = "fio";
            comboBoxSotrudnik.ValueMember = "id";

            // ComboBox Клиенты
            comboBoxClient.DataSource = dipl.Client.ToList();
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
            zayav.datedob = dTPDat.Value;

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

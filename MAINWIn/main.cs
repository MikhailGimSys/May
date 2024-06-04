using System;
using System.Data;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using May.MAINWIn.addwin;
using System.IO;

namespace May.MAINWIn
{
	public partial class main : Form
	{
		public main()
		{
			InitializeComponent();
			StartPosition = FormStartPosition.CenterScreen;

			new filter();
			ZayavHeadersShow();
			//ZayavSpisokShow();

			// ComboBox FilterStatus
			var dipl = new DIPLEntities2();
			var list = dipl.StatusZaiavka.ToList();
			// Вставка значения по умолчанию
			var def = new StatusZaiavka();
			def.id = 0;
			def.name = "Любой статус";
			list.Insert(0, def);
			FilterStatus.DataSource = list;
			FilterStatus.DisplayMember = "Name";
			FilterStatus.ValueMember = "id";
		}

		// Вывод заголовка для списка заявок
		private void ZayavHeadersShow()
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

			// Привязка события клика мыши по ДатаГриду
			dataGridView1.MouseClick += DataGridView1_MouseClick;
		}

		// Вывод списка заявок
		void ZayavSpisokShow()
		{
			Console.WriteLine("ZSS");


			// Предварительная очиска списка перед его очередным выводом
			dataGridView1.Rows.Clear();

			// Получение списка заявок из базы
			var dipl = new DIPLEntities2();

			// Вывод данных в датагрид
			foreach (var item in dipl.ZaiavkaNeispravnost)
			{
				// Филтрация по Фио
				var fio = filter.fio.ToLower().Trim();
				if (fio.Length > 0)
					if (!item.Client.fio.ToLower().Contains(fio))
						continue;

				// Фильтрация по статусу
				if(filter.status > 0)
					if(filter.status != item.StatusZaiavka.id)
						continue;

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
			// Получение данных выбранной ячейки
			var cell = dataGridView1.SelectedCells[0];

			// Был выбрана ненужная ячейка
			if (cell.Tag == null)
				return;

			int id = (int)cell.Tag;
			var dipl = new DIPLEntities2();
			var zayav = dipl.ZaiavkaNeispravnost.Where(x => x.id == id).ToList()[0];

			switch (cell.Value.ToString())
			{
				case "Word":
					MessageBox.Show("Нажали на Word");
					break;

				// Редактирование данных заявки
				case "Изменить":
					var redact = new addzaiavka();
					redact.ID = zayav.id;
					redact.textBoxCodeZaiavka.Text = zayav.id.ToString();
					redact.dTPDat.Value = Convert.ToDateTime(zayav.datedob);
					redact.comboBoxStatusZaiavka.SelectedValue = zayav.status_id;
					redact.comboBoxNeispravnost.SelectedValue = zayav.neispravost_id;
					redact.comboBoxSotrudnik.SelectedValue = zayav.sotrudnik_id;
					redact.comboBoxClient.SelectedValue = zayav.client_id;
					redact.textBoxOpisProm.Text = zayav.opisanie;

					// Подмена текста в заголовке и кнопке сохранения
					if(redact.ID > 0)
					{
						redact.Text = "Редактирование заявки";
						redact.buttonSave.Text = "Сохранить";
					}

					redact.ShowDialog();
					ZayavSpisokShow();
					break;

				// Удаление заявки
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
			var add = new addzaiavka();
			add.ShowDialog();
			ZayavSpisokShow();
		}



		// Поиск заявок по ФИО клиента
		private void FilterFioChanged(object sender, EventArgs e)
		{
			filter.fio = FilterFio.Text;
			ZayavSpisokShow();
		}

		private void FilterStatusChanged(object sender, EventArgs e)
		{
			var box = sender as ComboBox;
			var item = box.SelectedItem as StatusZaiavka;
			filter.status = item.id;
			ZayavSpisokShow();
		}

		// Фильтр для списка Заявок на неисправности
		class filter
		{
			// Очистка фильтра
			public filter()
			{
				fio = "";
				status = 0;
			}
			public static string fio { get; set; }
			public static int status { get; set; }
		}
	}
}



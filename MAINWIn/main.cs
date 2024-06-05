using System;
using System.Data;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using May.MAINWIn.addwin;

namespace May.MAINWIn
{
	public partial class main : Form
	{
		public main()
		{
			InitializeComponent();
			StartPosition = FormStartPosition.CenterScreen;

			new Filter();
			ZayavHeadersShow();
			FilterInit();
		}

		// Вывод заголовка для списка заявок
		private void ZayavHeadersShow()
		{
			// Создание загововка таблицы 
			dataGridView1.Columns.Add("", "№ Заявки");
			dataGridView1.Columns.Add("", "Дата создания");
			dataGridView1.Columns.Add("", "Клиент");
			dataGridView1.Columns.Add("", "Тип ошибки");
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
		private void ZayavSpisokShow()
		{
			Console.WriteLine("....");

			// Предварительная очиска списка перед его очередным выводом
			dataGridView1.Rows.Clear();

			// Получение списка заявок из базы
			var dipl = new DIPLEntities2();

			// Вывод данных в датагрид
			foreach (var item in dipl.ZaiavkaNeispravnost)
			{
				if (!Filter.Check(item))
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
			FilterClear();
		}



		private void FilterInit()
		{
			// Заполнение данными ComboBox FilterStatus
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


			var MinDate = dipl.ZaiavkaNeispravnost.Min(x => x.datedob.Value);
			var MaxDate = dipl.ZaiavkaNeispravnost.Max(x => x.datedob.Value);

			FilterDateFrom.MinDate = MinDate;
			FilterDateFrom.MaxDate = MaxDate;
			FilterDateFrom.Value = MinDate;

			FilterDateTo.MinDate = MinDate;
			FilterDateTo.MaxDate = MaxDate;
			FilterDateTo.Value = MaxDate;
		}

		// Ввод текста в фильтре "Поиск заявок по ФИО клиента"
		private void FilterFioChanged(object sender, EventArgs e)
		{
			Filter.fio = FilterFio.Text;
			ZayavSpisokShow();
		}

		// Ввод текста в фильтре "Поиск заявок по Статусу"
		private void FilterStatusChanged(object sender, EventArgs e)
		{
			var box = sender as ComboBox;
			var item = box.SelectedItem as StatusZaiavka;
			Filter.status = item.id;
			ZayavSpisokShow();
		}

		// Очистка значений фильтра в полях и в классе
		private void FilterClear()
		{
			FilterFio.Text = "";
			FilterStatus.SelectedIndex = 0;

			new Filter();
		}

		// Класс: фильтр для списка Заявок на неисправности
		class Filter
		{
			// Очистка фильтра
			public Filter()
			{
				fio = "";
				status = 0;
			}

			// Проверка записи по всем фильтрам
			public static bool Check(ZaiavkaNeispravnost item)
			{
				if(!CheckFio(item))
					return false;
				if(!CheckStatus(item))
					return false;

				return true;
			}

			// Проверка по полю Фио клиента, сотдудника, ID заявки
			static bool CheckFio(ZaiavkaNeispravnost item)
			{
				var txt = fio.ToLower().Trim();

				if (txt.Length == 0)
					return true;

				if (!item.Client.fio.ToLower().Contains(txt))
					if (!item.Sotrudnik.fio.ToLower().Contains(txt))
						if (!item.opisanie.ToLower().Contains(txt))
							if (item.id.ToString() != txt)
								return false;

				return true;
			}

			// Проверка по статусу
			static bool CheckStatus(ZaiavkaNeispravnost item)
			{
				if (status == 0)
					return true;

				return status == item.StatusZaiavka.id;
			}

			public static string fio { get; set; } // Фио клиента, сотдудника, ID заявки
			public static int status { get; set; }

		}
	}
}



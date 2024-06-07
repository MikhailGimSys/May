using System;
using System.IO;
using System.Data;
using System.Linq;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Data.Entity.Migrations;

using May.MAINWIn.addwin;
//using Microsoft.Office.Interop.Word;
//using Word = Microsoft.Office.Interop.Word;

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




			// ComboBox Должность сотрудника
			var dipl = new DIPLEntities2();
			SotrDoljnost.DataSource = dipl.Doljnost.ToList(); ;
			SotrDoljnost.DisplayMember = "Name";
			SotrDoljnost.ValueMember = "id";

			SotrudnikHeadersShow();
			SotrudnikSpisokShow();

			ClientHeadersShow();
			ClientSpisokShow();

			TypeHeadersShow();
			TypeSpisokShow();
		}



		// Изменение ширины колонок в ДатаГридах
		private void ColumnWidthSave(object sender, DataGridViewColumnEventArgs e)
		{
			// Получение ДатаГрида, у которого были изменены колонки
			var grid = sender as DataGridView;

			int count = grid.Columns.Count;
			string[] mass = new string[count];

			// Сохранение всех ширин в массив
			for (int i = 0; i < count; i++)
				mass[i] = $"{grid.Columns[i].Width}";

			// Сохранение данных в файл
			var fileName = $"{grid.Name}.txt";
			var txt = string.Join(" ", mass);
			File.WriteAllText(fileName, txt);
		}

		// Установка ширины колонок в ДатаГридах
		private void ColumnWidthSet(DataGridView grid)
		{
			var fileName = $"{grid.Name}.txt";

			// Проверка, существует ли файл
			if (!File.Exists(fileName))
				return;

			// Получение данных
			var txt = File.ReadAllText(fileName);
			string[] mass = txt.Split(' ');

			// Установка ширины для колонок
			for (int i = 0; i < grid.Columns.Count; i++)
				grid.Columns[i].Width = Convert.ToInt32(mass[i]);
		}



		#region ЗАЯВКИ

		// Вывод заголовка для списка заявок
		private void ZayavHeadersShow()
		{
			// Создание загововка таблицы 
			ZayavDataGrid.Columns.Add("", "№");
			ZayavDataGrid.Columns.Add("", "Дата создания");
			ZayavDataGrid.Columns.Add("", "Клиент");
			ZayavDataGrid.Columns.Add("", "Тип ошибки");
			ZayavDataGrid.Columns.Add("", "Статус");
			ZayavDataGrid.Columns.Add("", "Сотрудник");
			ZayavDataGrid.Columns.Add("", "Описание");
			ZayavDataGrid.Columns.Add("Word", "");
			ZayavDataGrid.Columns.Add("Redact", "");
			ZayavDataGrid.Columns.Add("Del", "");

			// Установка ширины для колонок
			ColumnWidthSet(ZayavDataGrid);

			// Привязка события клика мыши по ДатаГриду
			ZayavDataGrid.MouseClick += DataGridView1_MouseClick;
		}

		// Вывод списка заявок
		private void ZayavSpisokShow()
		{
			// Предварительная очиска списка перед его очередным выводом
			ZayavDataGrid.Rows.Clear();

			// Получение списка заявок из базы
			var dipl = new DIPLEntities2();

			// Вывод данных в датагрид
			foreach (var item in dipl.ZaiavkaNeispravnost)
			{
				if (!Filter.Check(item))
					continue;

				int i = ZayavDataGrid.Rows.Add (
					item.id,
					item.datedob,
					item.Client.fio,
					item.TypeNeispravnost.name,
					item.StatusZaiavka.name,
					item.Sotrudnik.fio,
					item.opisanie,
					item.StatusZaiavka.id == 3 ? "Word" : "",
					"Изменить",
					"Удалить");

				// Форматирование ячейки Word
				var cell = ZayavDataGrid["Word", i];
				cell.Style.ForeColor = Color.Blue;
				cell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
				cell.Tag = item.id;

				// Форматирование ячейки Redact
				cell = ZayavDataGrid["Redact", i];
				cell.Style.ForeColor = Color.DarkOrange;
				cell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
				cell.Tag = item.id;

				// Форматирование ячейки Del
				cell = ZayavDataGrid["Del", i];
				cell.Style.ForeColor = Color.Red;
				cell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
				cell.Tag = item.id;
			}
		}

		// Нажатие на Датагрид (Word, Изменить, Удалить)
		private void DataGridView1_MouseClick(object sender, MouseEventArgs e)
		{
			// Получение выбранной ячейки
			var cell = ZayavDataGrid.SelectedCells[0];

			// Был выбрана ненужная ячейка
			if (cell.Tag == null)
				return;

			// Получение ID для редактирования или удаления
			int id = (int)cell.Tag;
			// Подключение к Базе
			var dipl = new DIPLEntities2();
			// Получение данных записи
			var zayav = dipl.ZaiavkaNeispravnost.Where(x => x.id == id).ToList()[0];

			switch (cell.Value.ToString())
			{
				// Вывод Акта в формате Ворд
				case "Word":
					// Файл-щаблон
					var tmpFile = $"{Environment.CurrentDirectory}\\Resources\\act.tmp.docx";
					//Console.WriteLine(tmpFile);
					//var word = new Word.Application();
					//_Document doc = word.Documents.Add(tmpFile);
					//doc.Bookmarks["{NOMER}"].Range.Text = "123";
					//doc.SaveAs(@"Resources\act.docx");
					//doc.Close();

					Process.Start(tmpFile);
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

		#endregion

		#region ФИЛЬТР ЗАЯВОК

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
			Filter.fio = FilterFio.Text.ToLower().Trim();
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

		// Класс: фильтр для списка Заявок на неисправности, хранящий состояния полей фильтра
		class Filter
		{
			// Очистка фильтра
			public Filter()
			{
				fio = "";
				status = 0;

				clientTxt = "";
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
				if (fio.Length == 0)
					return true;

				if (!item.Client.fio.ToLower().Contains(fio))
					if (!item.Sotrudnik.fio.ToLower().Contains(fio))
						if (!item.opisanie.ToLower().Contains(fio))
							if (item.id.ToString() != fio)
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

			public static string fio { get; set; } // Фио клиента, сотрудника, ID заявки
			public static int status { get; set; } // Статус





			// СПИСОК КЛИЕНТОВ
			public static string clientTxt { get; set; } // поиск по Фио, ИНН, номеру телефона, емаилу
		}

		#endregion

		#region СОТРУДНИКИ

		// Идентификатор сотрудника для редактирования
		int SotrudnikId = 0;

		// Нажатие на кнопку "Добавить сотрудника"
		private void SotrudnikSave(object sender, EventArgs e)
		{
			var item = new Sotrudnik();

			item.id = SotrudnikId;
			
			// ФИО
			item.fio = SotrFio.Text.Trim();
			if(item.fio.Length == 0)
			{
				MessageBox.Show("Не указано ФИО сотрудника");
				return;
			}

			// Внутренний телефон
			item.phone = SotrPhone.Text.Trim();
			if (item.phone.Length == 0)
			{
				MessageBox.Show("Не указан Внутренний телефон");
				return;
			}

			// Должность
			item.doljnost_id = (int)SotrDoljnost.SelectedValue;

			// Логин
			item.login = SotrLogin.Text.Trim();
			if (item.login.Length == 0)
			{
				MessageBox.Show("Не указан Логин");
				return;
			}

			// Пароль
			item.password = SotrPass.Text.Trim();
			if (item.password.Length == 0)
			{
				MessageBox.Show("Не указан Пароль");
				return;
			}




			// Подключение к базе
			var dipl = new DIPLEntities2();

			// Внесение данных Сотрудника в базу (или изменение)
			dipl.Sotrudnik.AddOrUpdate(item);

			// Сохранение изменений в базе
			dipl.SaveChanges();

			MessageBox.Show(SotrudnikId == 0 ? "Новый сотрудник успешно добавлен!" : "Данные сохранены");



			// Очистка полей после успешного добавления
			SotrClear();

			// Обновление списка сотрудников
			SotrudnikSpisokShow();
		}

		// Отмена редактирования данных сотрудника и очистка полей
		private void SotrClear(object sender, EventArgs e) =>
			SotrClear();
		private void SotrClear()
		{
			SotrFio.Text = "";
			SotrPhone.Text = "";
			SotrLogin.Text = "";
			SotrPass.Text = "";

			SotrudnikId = 0;
			SotrSaveBox.Text = "Добавление сотрудника";
			SotrCancelBut.Visible = false;
			SotrudnikSaveBut.Text = "Добавить сотрудника";
		}

		// Вывод заголовка списка сотрудников
		private void SotrudnikHeadersShow()
		{
			// Создание загововка таблицы 
			SotrDataGrid.Columns.Add("", "ФИО");
			SotrDataGrid.Columns.Add("", "Телефон");
			SotrDataGrid.Columns.Add("", "Должность");
			SotrDataGrid.Columns.Add("Redact", "");
			SotrDataGrid.Columns.Add("Del", "");

			// Установка ширины для колонок
			ColumnWidthSet(SotrDataGrid);

			// Привязка события клика мыши по ДатаГриду
			SotrDataGrid.MouseClick += SotrDataGridClick;
		}

		// Нажатие на Датагрид Сотрудников (Изменить, Удалить)
		private void SotrDataGridClick(object sender, MouseEventArgs e)
		{
			// Получение выбранной ячейки
			var cell = SotrDataGrid.SelectedCells[0];

			// Был выбрана ненужная ячейка
			if (cell.Tag == null)
				return;

			// Получение ID для редактирования или удаления
			SotrudnikId = (int)cell.Tag;
			// Подключение к Базе
			var dipl = new DIPLEntities2();
			// Получение данных записи
			var item = dipl.Sotrudnik.Where(x => x.id == SotrudnikId).ToList()[0];

			switch (cell.Value.ToString())
			{
				// Редактирование данных сотрудника
				case "Изменить":
					SotrFio.Text = item.fio;
					SotrPhone.Text = item.phone;
					SotrDoljnost.SelectedValue = item.doljnost_id;
					SotrLogin.Text = item.login;
					SotrPass.Text = item.password;

					SotrSaveBox.Text = "Изменение данных сотрудника";
					SotrCancelBut.Visible = true;
					SotrudnikSaveBut.Text = "Сохранить";
					break;

				// Удаление сотрудника
				case "Удалить":
					var use = dipl.ZaiavkaNeispravnost.Where(x => x.sotrudnik_id == SotrudnikId).Count();
					if (use > 0)
					{
						MessageBox.Show("Невозможно удалить сотрудника, так как он используется в заявках.");
						break;
					}

					var result = MessageBox.Show(
						"Вы действтельно хотите удалить запись?",
						"Удаление записи",
						MessageBoxButtons.YesNo);

					if (result != DialogResult.Yes)
						break;

					dipl.Sotrudnik.Remove(item);
					dipl.SaveChanges();
					SotrudnikId = 0;
					SotrudnikSpisokShow();

					break;
			}
		}

		// Вывод списка Сотрудников
		private void SotrudnikSpisokShow()
		{
			// Предварительная очиска списка перед его очередным выводом
			SotrDataGrid.Rows.Clear();

			// Получение списка заявок из базы
			var dipl = new DIPLEntities2();

			// Вывод данных в датагрид
			foreach (var item in dipl.Sotrudnik)
			{
				int i = SotrDataGrid.Rows.Add(
					item.fio,
					item.phone,
					item.Doljnost.name,
					"Изменить",
					"Удалить");

				// Форматирование ячейки Redact
				var cell = SotrDataGrid["Redact", i];
				cell.Style.ForeColor = Color.DarkOrange;
				cell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
				cell.Tag = item.id;

				// Форматирование ячейки Del
				cell = SotrDataGrid["Del", i];
				cell.Style.ForeColor = Color.Red;
				cell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
				cell.Tag = item.id;
			}
		}

		#endregion

		#region КЛИЕНТЫ

		// Идентификатор сотрудника для редактирования
		int ClientId = 0;

		// Вывод заголовка списка Клиентов
		private void ClientHeadersShow()
		{
			// Создание загововка таблицы 
			ClientDataGrid.Columns.Add("", "ФИО");
			ClientDataGrid.Columns.Add("", "ИНН");
			ClientDataGrid.Columns.Add("", "Телефон");
			ClientDataGrid.Columns.Add("", "E-mail");
			ClientDataGrid.Columns.Add("Redact", "");
			ClientDataGrid.Columns.Add("Del", "");

			// Установка ширины для колонок
			ColumnWidthSet(ClientDataGrid);

			// Привязка события клика мыши по ДатаГриду
			ClientDataGrid.MouseClick += ClientDataGridClick;
		}

		// Нажатие на Датагрид Сотрудников (Изменить, Удалить)
		private void ClientDataGridClick(object sender, MouseEventArgs e)
		{
			// Получение выбранной ячейки
			var cell = ClientDataGrid.SelectedCells[0];

			// Был выбрана ненужная ячейка
			if (cell.Tag == null)
				return;

			// Получение ID для редактирования или удаления
			ClientId = (int)cell.Tag;
			// Подключение к Базе
			var dipl = new DIPLEntities2();
			// Получение данных записи
			var item = dipl.Client.Where(x => x.id == ClientId).ToList()[0];

			switch (cell.Value.ToString())
			{
				// Редактирование данных клиента
				case "Изменить":
					ClientFio.Text = item.fio;
					ClientINN.Text = item.inn;
					ClientPhone.Text = item.phone;
					ClientEmail.Text = item.email;

					ClientSaveBox.Text = "Редактирование клиента";
					ClientCancelBut.Visible = true;
					ClientSaveBut.Text = "Сохранить";
					break;

				// Удаление клиента
				case "Удалить":
					var use = dipl.ZaiavkaNeispravnost.Where(x => x.client_id == ClientId).Count();
					if (use > 0)
					{
						MessageBox.Show("Невозможно удалить клиента, так как он используется в заявках.");
						break;
					}

					var result = MessageBox.Show(
						"Вы действтельно хотите удалить запись?",
						"Удаление записи",
						MessageBoxButtons.YesNo);

					if (result != DialogResult.Yes)
						break;

					dipl.Client.Remove(item);
					dipl.SaveChanges();
					ClientId = 0;
					ClientSpisokShow();
					break;
			}
		}

		// Вывод списка Клиентов
		private void ClientSpisokShow()
		{
			// Предварительная очиска списка перед его очередным выводом
			ClientDataGrid.Rows.Clear();

			// Получение списка заявок из базы
			var dipl = new DIPLEntities2();

			// Вывод данных в датагрид
			foreach (var item in dipl.Client)
			{
				// Фильрование списка на основании поиска
				if (Filter.clientTxt.Length > 0)
					if (!item.fio.ToLower().Contains(Filter.clientTxt))
						if (!item.inn.ToLower().Contains(Filter.clientTxt))
							if (!item.phone.ToLower().Contains(Filter.clientTxt))
								if (!item.email.ToLower().Contains(Filter.clientTxt))
									continue;

				int i = ClientDataGrid.Rows.Add(
					item.fio,
					item.inn,
					item.phone,
					item.email,
					"Изменить",
					"Удалить");

				// Форматирование ячейки Redact
				var cell = ClientDataGrid["Redact", i];
				cell.Style.ForeColor = Color.DarkOrange;
				cell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
				cell.Tag = item.id;

				// Форматирование ячейки Del
				cell = ClientDataGrid["Del", i];
				cell.Style.ForeColor = Color.Red;
				cell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
				cell.Tag = item.id;
			}
		}

		// Ввод текста в фильтре поиска клиентов
		private void ClientFilterTxtChanged(object sender, EventArgs e)
		{
			Filter.clientTxt = ClientFilterTxt.Text.ToLower().Trim();
			ClientSpisokShow();
		}

		// Внесение или сохранение данных клиента
		private void ClientSave(object sender, EventArgs e)
		{
			var item = new Client();

			item.id = ClientId;

			// ФИО
			item.fio = ClientFio.Text.Trim();
			if (item.fio.Length == 0)
			{
				MessageBox.Show("Не указаны ФИО");
				return;
			}

			// ИНН
			item.inn = ClientINN.Text.Trim();
			if (item.inn.Length == 0)
			{
				MessageBox.Show("Не указан ИНН");
				return;
			}

			// Tелефон
			item.phone = ClientPhone.Text.Trim();
			if (item.phone.Length == 0)
			{
				MessageBox.Show("Не указан телефон");
				return;
			}

			// Email
			item.email = ClientEmail.Text.Trim();
			if (item.email.Length == 0)
			{
				MessageBox.Show("Не указан email");
				return;
			}




			// Подключение к базе
			var dipl = new DIPLEntities2();

			// Внесение данных Клиента в базу (или изменение)
			dipl.Client.AddOrUpdate(item);

			// Сохранение изменений в базе
			dipl.SaveChanges();

			MessageBox.Show(ClientId == 0 ? "Новый клиент успешно добавлен!" : "Данные сохранены");


			// Очистка полей
			ClientClear();

			// Обновление списка
			ClientSpisokShow();
		}

		// Отмена редактирования клиента и очистка полей
		private void ClientClear(object sender, EventArgs e) => ClientClear();
		private void ClientClear()
		{
			ClientSaveBox.Text = "Добавление клиента";
			ClientCancelBut.Visible = false;
			ClientSaveBut.Text = "Добавить клиента";

			ClientId = 0;
			ClientFio.Text = "";
			ClientINN.Text = "";
			ClientPhone.Text = "";
			ClientEmail.Text = "";
		}

		#endregion

		#region ТИПЫ ОШИБОК

		// Идентификатор ошибки при редактировании
		int TypeId = 0;

		// Вывод заголовка списка Типов ошибок
		private void TypeHeadersShow()
		{
			// Создание загововка таблицы 
			TypeDataGrid.Columns.Add("", "Название");
			TypeDataGrid.Columns.Add("Redact", "");
			TypeDataGrid.Columns.Add("Del", "");

			// Установка ширины для колонок
			ColumnWidthSet(TypeDataGrid);

			// Привязка события клика мыши по ДатаГриду
			TypeDataGrid.MouseClick += TypeDataGridClick;
		}

		// Нажатие на Датагрид Типов ошибок (Изменить, Удалить)
		private void TypeDataGridClick(object sender, MouseEventArgs e)
		{
			// Получение выбранной ячейки
			var cell = TypeDataGrid.SelectedCells[0];

			// Был выбрана ненужная ячейка
			if (cell.Tag == null)
				return;

			// Получение ID для редактирования или удаления
			TypeId = (int)cell.Tag;
			// Подключение к Базе
			var dipl = new DIPLEntities2();
			// Получение данных записи
			var item = dipl.TypeNeispravnost.Where(x => x.id == TypeId).ToList()[0];

			switch (cell.Value.ToString())
			{
				// Редактирование данных Типа ошибок
				case "Изменить":
					TypeName.Text = item.name;
					TypeBox.Text = "Редактирование типа ошибки";
					TypeSaveBut.Text = "Сохранить";
					TypeCancelBut.Visible = true;
					break;

				// Удаление Типа ошибок
				case "Удалить":
					var use = dipl.ZaiavkaNeispravnost.Where(x => x.neispravost_id == TypeId).Count();
					if (use > 0)
					{
						MessageBox.Show("Невозможно удалить запись, так как она используется в заявках.");
						break;
					}

					var result = MessageBox.Show(
						"Вы действтельно хотите удалить запись?",
						"Удаление записи",
						MessageBoxButtons.YesNo);

					if (result == DialogResult.Yes)
					{
						dipl.TypeNeispravnost.Remove(item);
						dipl.SaveChanges();
						TypeId = 0;
						TypeSpisokShow();
					}
					break;
			}
		}

		// Нажатие на кнопку Сохранить
		private void TypeSave(object sender, EventArgs e)
		{
			var item = new TypeNeispravnost();

			item.id = TypeId;

			// Название
			item.name = TypeName.Text.Trim();
			if (item.name.Length == 0)
			{
				MessageBox.Show("Не указано Название");
				return;
			}


			// Подключение к базе
			var dipl = new DIPLEntities2();

			// Внесение данных Клиента в базу (или изменение)
			dipl.TypeNeispravnost.AddOrUpdate(item);

			// Сохранение изменений в базе
			dipl.SaveChanges();

			MessageBox.Show(TypeId == 0 ? "Новый тип ошибки успешно добавлен!" : "Данные сохранены");


			// Очистка полей
			TypeClear();

			// Обновление списка сотрудников
			TypeSpisokShow();
		}

		// Очистка полей
		private void TypeClear(object sender, EventArgs e) => TypeClear();
		private void TypeClear()
		{
			TypeId = 0;
			TypeName.Text = "";
			TypeBox.Text = "Добавление типа ошибки";
			TypeSaveBut.Text = "Добавить";
			TypeCancelBut.Visible = false;
		}

		// Вывод списка Клиентов
		private void TypeSpisokShow()
		{
			// Предварительная очиска списка перед его очередным выводом
			TypeDataGrid.Rows.Clear();

			// Получение списка заявок из базы
			var dipl = new DIPLEntities2();

			// Вывод данных в датагрид
			foreach (var item in dipl.TypeNeispravnost)
			{
				int i = TypeDataGrid.Rows.Add(
					item.name,
					"Изменить",
					"Удалить");

				// Форматирование ячейки Redact
				var cell = TypeDataGrid["Redact", i];
				cell.Style.ForeColor = Color.DarkOrange;
				cell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
				cell.Tag = item.id;

				// Форматирование ячейки Del
				cell = TypeDataGrid["Del", i];
				cell.Style.ForeColor = Color.Red;
				cell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
				cell.Tag = item.id;
			}
		}

		#endregion
	}
}



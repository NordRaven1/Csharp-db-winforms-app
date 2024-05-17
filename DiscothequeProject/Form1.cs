using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace DiscothequeProject
{
    public partial class Form1 : Form
    {
        SqlConnection con;
        DataTable sTable;
        DataTable actionTable;
        DataTable actionpersTable;
        DataTable persTable;
        DataTable friendsTable;
        DataTable locTable;
        DataTable pubTable;
        DataTable mediumTable;
        DataTable placeTable;
        SqlDataAdapter sAdapter;
        SqlDataAdapter actionAdapter;
        SqlDataAdapter actionpersAdapter;
        SqlDataAdapter persAdapter;
        SqlDataAdapter friendsAdapter;
        SqlDataAdapter locAdapter;
        SqlDataAdapter pubAdapter;
        SqlDataAdapter mediumAdapter;
        SqlDataAdapter placeAdapter;
        public Form1()
        {
            InitializeComponent();
        }

        private string allListCmd = "SELECT d.disk_id AS #, " +
                "CASE d.disk_type " +
                "WHEN 'FILM' THEN N'Фильм' " +
                "WHEN 'BOOK' THEN N'Аудиокнига' " +
                "WHEN 'MUSIC' THEN N'Аудиодиск' " +
                "WHEN 'DOC' THEN N'Диск с документацией' " +
                "WHEN 'PROG' THEN N'Диск с программами' " +
                "END AS 'Тип диска', " +
                "l.location_name + ' , ' +  pl.place_type + ' ' + CAST(pl.place_number AS VARCHAR) AS 'Место', " +
                "med.medium_name AS 'Тип носителя', " +
                "pb.publisher_name AS 'Издатель', " +
                "d.price AS 'Цена', " +
                "d.comments AS 'Комментарии', " +
                "fr.first_name + ' ' + fr.last_name AS 'У кого сейчас' " +
                "FROM disk d " +
                "JOIN storage_place pl ON d.place_id = pl.place_id " +
                "JOIN location l ON pl.location_id = l.location_id " +
                "JOIN storage_medium med ON d.medium_id = med.medium_id " +
                "JOIN publisher pb ON d.publisher_id = pb.publisher_id " +
                "LEFT OUTER JOIN friends fr ON d.hand_over = fr.friend_id ";

        private void Form1_Load(object sender, EventArgs e)
        {
            con = new SqlConnection();
            string projectPath = AppDomain.CurrentDomain.BaseDirectory;
            AppDomain.CurrentDomain.SetData("DataDirectory", projectPath);
            con.ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;
                                     AttachDbFilename=|DataDirectory|\Discotheque.mdf;Integrated Security=True;Connect Timeout=30";
            con.Open();

            sAdapter = new SqlDataAdapter(allListCmd, con);
            sTable = new DataTable();
            sAdapter.Fill(sTable);
             
            actionAdapter = new SqlDataAdapter("SELECT disk_id AS #, " +
                "disk_type AS 'Тип диска', " +
                "place_id AS 'Место', " +
                "medium_id AS 'Тип носителя', " +
                "publisher_id AS 'Издатель', " +
                "price AS 'Цена', " +
                "comments AS 'Комментарии', " +
                "hand_over AS 'У кого сейчас' " +
                "FROM disk ", con);
            actionTable = new DataTable();
            actionAdapter.Fill(actionTable);

            persAdapter = new SqlDataAdapter("SELECT * FROM disk", con);
            persTable = new DataTable();
            persAdapter.Fill(persTable);

            actionpersAdapter = new SqlDataAdapter("SELECT * FROM disk", con);
            actionpersTable = new DataTable();
            actionpersAdapter.Fill(actionpersTable);

            friendsAdapter = new SqlDataAdapter("SELECT friend_id AS 'Id друга', first_name AS 'Имя', last_name AS 'Фамилия', address AS 'Адрес', phone_number AS 'Телефон', email AS 'Почта'  FROM friends", con);
            friendsTable = new DataTable();
            friendsAdapter.Fill(friendsTable);

            locAdapter = new SqlDataAdapter("SELECT location_id AS 'Id локации', location_name AS 'Название локации' FROM location", con);
            locTable = new DataTable();
            locAdapter.Fill(locTable);

            pubAdapter = new SqlDataAdapter("SELECT publisher_id AS 'Id издателя', publisher_name AS 'Название издателя' FROM publisher", con);
            pubTable = new DataTable();
            pubAdapter.Fill(pubTable);

            mediumAdapter = new SqlDataAdapter("SELECT medium_id AS 'Id носителя', medium_name AS 'Название носителя' FROM storage_medium", con);
            mediumTable = new DataTable();
            mediumAdapter.Fill(mediumTable);

            placeAdapter = new SqlDataAdapter("SELECT place_id AS 'Id места', place_type AS 'Тип места', place_number AS 'Номер места', location_id AS 'Id локации' FROM storage_place", con);
            placeTable = new DataTable();
            placeAdapter.Fill(placeTable);

            new SqlCommandBuilder(actionAdapter);
            new SqlCommandBuilder(actionpersAdapter);
            new SqlCommandBuilder(friendsAdapter);
            new SqlCommandBuilder(locAdapter);
            new SqlCommandBuilder(pubAdapter);
            new SqlCommandBuilder(mediumAdapter);
            new SqlCommandBuilder(placeAdapter);

            dataGridView1.DataSource = sTable;
            dataGridView2.DataSource = persTable;
            dataGridView3.DataSource = friendsTable;
            dataGridView4.DataSource = locTable;
            dataGridView5.DataSource = pubTable;
            dataGridView6.DataSource = mediumTable;
            dataGridView7.DataSource = placeTable;
            dataGridView8.DataSource = actionTable;
            dataGridView9.DataSource = actionpersTable;

            SqlCommand mediums = con.CreateCommand();
            mediums.CommandText = "SELECT medium_name FROM storage_medium";
            using (SqlDataReader reader = mediums.ExecuteReader())
            {
                while (reader.Read())
                {
                    comboBox3.Items.Add(String.Format("{0}", reader[0]));
                } 
            }

            SqlCommand publishers = con.CreateCommand();
            publishers.CommandText = "SELECT publisher_name FROM publisher";
            using (SqlDataReader reader = publishers.ExecuteReader())
            {
                while (reader.Read())
                {
                    comboBox4.Items.Add(String.Format("{0}", reader[0]));
                }
            }

            SqlCommand friends = con.CreateCommand();
            friends.CommandText = "SELECT first_name + ' ' + last_name FROM friends";
            using (SqlDataReader reader = friends.ExecuteReader())
            {
                while (reader.Read())
                {
                    comboBox5.Items.Add(String.Format("{0}", reader[0]));
                }
            }

            SqlCommand locations = con.CreateCommand();
            locations.CommandText = "SELECT location_name FROM location";
            using (SqlDataReader reader = locations.ExecuteReader())
            {
                while (reader.Read())
                {
                    comboBox6.Items.Add(String.Format("{0}", reader[0]));
                }
            }


            InputLanguage.CurrentInputLanguage = InputLanguage.FromCulture(new CultureInfo("ru-RU"));

            dataGridView1.Columns[0].Width = 30;
            dataGridView1.Columns[2].Width = 140;
            dataGridView1.Columns[3].Width = 65;
            dataGridView1.ReadOnly = true;
            dataGridView2.ReadOnly = true;
            comboBox1.SelectedIndex = comboBox1.FindStringExact("");


        }

        private string persTableInfo(string disktype)
        {
            string response = "";
            switch (disktype)
            {
                case "Фильм":
                    response = "SELECT film_genre AS 'Жанр фильма', film_name AS 'Название', CAST(film_duration AS VARCHAR) + N' минут' AS 'Хронометраж', release_year AS 'Год выпуска', film_rating AS 'Рейтинг' FROM disk WHERE disk_id = @d_id";
                    break;
                case "Аудиокнига":
                    response = "SELECT audiobook_author AS 'Автор', audiobook_title AS 'Название книги', CAST(audiobook_duration AS VARCHAR) + N' минут' AS 'Длительность', CAST(audiobook_chapters AS VARCHAR) + N' глав(ы)' AS 'Кол - во глав' FROM disk WHERE disk_id = @d_id";
                    break;
                case "Аудиодиск":
                    response = "SELECT audiodisk_album AS 'Название альбома', music_genre AS 'Жанр', audiodisk_performer AS 'Исполнитель', audiodisk_label AS 'Лейбл', songs_quantity AS 'Кол - во песен' FROM disk WHERE disk_id = @d_id";
                    break;
                case "Диск с документацией":
                    response = "SELECT docdisk_product AS 'Товар', docdisk_pages AS 'Кол - во страниц' FROM disk WHERE disk_id = @d_id";
                    break;
                case "Диск с программами":
                    response = "SELECT progdisk_prog AS 'Программа', release_year AS 'Год выпуска', progdisk_systems AS 'Список ОС' FROM disk WHERE disk_id = @d_id";
                    break;
                default:
                    response = "SELECT NULL";
                    break;
            }
            return response;
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                string disk_id = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                string disk_type = dataGridView1.CurrentRow.Cells[1].Value.ToString();
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT scan_img FROM disk WHERE disk_id = @d_id";
                cmd.Parameters.AddWithValue("@d_id", disk_id);
                byte[] bytes = (byte[])cmd.ExecuteScalar();

                string command = persTableInfo(disk_type);

                persAdapter.SelectCommand = new SqlCommand(command, con);
                persAdapter.SelectCommand.Parameters.AddWithValue("@d_id", disk_id);

                persTable = new DataTable();
                persAdapter.Fill(persTable);
                dataGridView2.DataSource = persTable;
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBox1.Image = Image.FromStream(new MemoryStream(bytes));
            }
            catch (NullReferenceException ex)
            {
                MessageBox.Show("Кажется возникла ошибка при поиске такого пользователя, попробуйте еще раз");
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Кажется возникла ошибка при поиске такого пользователя, попробуйте еще раз");
            }
            catch(InvalidCastException ex)
            {
                string disk_id = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = "UPDATE disk SET scan_img = (SELECT * FROM OPENROWSET(BULK N'C:\\error.png', SINGLE_BLOB) AS t1)  WHERE disk_id = @d_id";
                cmd.Parameters.AddWithValue("@d_id", disk_id);
                cmd.ExecuteScalar();
            }
        }

        private void Дубликаты_Click(object sender, EventArgs e)
        {
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandText = allListCmd +
                "JOIN disk d2 ON d.disk_id != d2.disk_id " +
                "WHERE ( (d.disk_type = 'MUSIC' AND d2.disk_type = 'MUSIC' AND d.audiodisk_album = d2.audiodisk_album AND d.music_genre = d2.music_genre " +
                "AND d.audiodisk_performer = d2.audiodisk_performer AND d.audiodisk_label = d2.audiodisk_label AND d.songs_quantity = d2.songs_quantity) " +
                "OR (d.disk_type = 'FILM' AND d2.disk_type = 'FILM' AND d.film_genre = d2.film_genre AND d.film_name = d2.film_name AND d.film_duration = d2.film_duration " +
                "AND d.release_year = d2.release_year AND d.film_rating = d2.film_rating) " +
                "OR (d.disk_type = 'BOOK' AND d2.disk_type = 'BOOK' AND d.audiobook_author = d2.audiobook_author AND d.audiobook_title = d2.audiobook_title " +
                "AND d.audiobook_duration = d2.audiobook_duration AND d.audiobook_chapters = d2.audiobook_chapters) " +
                "OR (d.disk_type = 'DOC' AND d2.disk_type = 'DOC' AND d.docdisk_product = d2.docdisk_product AND d.docdisk_pages = d2.docdisk_pages) " +
                "OR (d.disk_type = 'PROG' AND d2.disk_type = 'PROG' AND d.progdisk_prog = d2.progdisk_prog AND d.release_year = d2.release_year AND d.progdisk_systems = d2.progdisk_systems))";

            sAdapter = new SqlDataAdapter(cmd);
            sTable = new DataTable();
            sAdapter.Fill(sTable);
            dataGridView1.DataSource = sTable;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandText = allListCmd;
            sAdapter = new SqlDataAdapter(cmd);
            sTable = new DataTable();
            sAdapter.Fill(sTable);
            dataGridView1.DataSource = sTable;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandText = allListCmd + "WHERE comments LIKE N'%" + textBox1.Text + "%'";
            sAdapter = new SqlDataAdapter(cmd);
            sTable = new DataTable();
            sAdapter.Fill(sTable);
            dataGridView1.DataSource = sTable;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SqlCommand maxprice = con.CreateCommand();
            SqlCommand minprice = con.CreateCommand();
            SqlCommand avgprice = con.CreateCommand();
            SqlCommand quantity = con.CreateCommand();
            SqlCommand diskhandover = con.CreateCommand();
            SqlCommand popularformat = con.CreateCommand();
            maxprice.CommandText = "SELECT MAX(price) FROM disk";
            minprice.CommandText = "SELECT MIN(price) FROM disk";
            quantity.CommandText = "SELECT COUNT(disk_id) FROM disk";
            avgprice.CommandText = "SELECT FORMAT(AVG(price),'N2') FROM disk";
            diskhandover.CommandText = "SELECT COUNT(disk_id) FROM disk WHERE hand_over IS NOT NULL";
            popularformat.CommandText = "SELECT disk_type FROM disk GROUP BY disk_type HAVING COUNT(disk_type) = (SELECT MAX(t.num) FROM (SELECT COUNT(disk_type) AS num FROM disk GROUP BY disk_type) t)";
            string maxpriceValue = maxprice.ExecuteScalar().ToString();
            string minpriceValue = minprice.ExecuteScalar().ToString();
            string avgpriceValue = avgprice.ExecuteScalar().ToString();
            string diskq = quantity.ExecuteScalar().ToString();
            string diskhandoverValue = diskhandover.ExecuteScalar().ToString();
            string popularformatValue = "";
            switch (popularformat.ExecuteScalar().ToString())
            {
                case "MUSIC":
                    popularformatValue = "Аудиодиск";
                    break;
                case "FILM":
                    popularformatValue = "Фильм";
                    break;
                case "BOOK":
                    popularformatValue = "Аудиокнига";
                    break;
                case "DOC":
                    popularformatValue = "Диск с документацией";
                    break;
                case "PROG":
                    popularformatValue = "Диск с программами";
                    break;
                default:
                    popularformatValue = "Неизвестный тип";
                    break;
            }
            richTextBox1.Text = "Максимальная цена за диск: " + maxpriceValue
                + "\nМинимальная цена за диск: " + minpriceValue
                + "\nСредняя цена за диск: " + avgpriceValue
                + "\nКоличество дисков: " + diskq
                + "\nКол- во одолженных дисков: " + diskhandoverValue
                + "\nСамый частый тип диска: " + popularformatValue;
        }

        private void makeFiltersVisibilityFalse()
        {
            label1.Visible = false;
            label2.Visible = false;
            label3.Visible = false;
            label4.Visible = false;
            label5.Visible = false;
            textBox2.Visible = false;
            textBox3.Visible = false;
            textBox4.Visible = false;
            textBox5.Visible = false;
            textBox6.Visible = false;
        }

        private void makeFilters2VisibilityFalse()
        {
            label10.Visible = false;
            label11.Visible = false;
            label12.Visible = false;
            label13.Visible = false;
            label14.Visible = false;
            textBox13.Visible = false;
            textBox14.Visible = false;
            textBox15.Visible = false;
            textBox16.Visible = false;
            textBox17.Visible = false;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            makeFiltersVisibilityFalse();
            switch (comboBox1.SelectedItem.ToString())
            {
                case "Аудиокнига":
                    label1.Visible = true;
                    label2.Visible = true;
                    label3.Visible = true;
                    label4.Visible = true;
                    label1.Text = "Автор";
                    label2.Text = "Название книги";
                    label3.Text = "Длительность";
                    label4.Text = "Кол - во глав";
                    textBox2.Visible = true;
                    textBox3.Visible = true;
                    textBox4.Visible = true;
                    textBox5.Visible = true;
                    break;
                case "Фильм":
                    label1.Visible = true;
                    label2.Visible = true;
                    label3.Visible = true;
                    label4.Visible = true;
                    label5.Visible = true;
                    label1.Text = "Жанр фильма";
                    label2.Text = "Название";
                    label3.Text = "Хронометраж";
                    label4.Text = "Год выпуска";
                    label5.Text = "Рейтинг";
                    textBox2.Visible = true;
                    textBox3.Visible = true;
                    textBox4.Visible = true;
                    textBox5.Visible = true;
                    textBox6.Visible = true;
                    break;
                case "Аудиодиск":
                    label1.Visible = true;
                    label2.Visible = true;
                    label3.Visible = true;
                    label4.Visible = true;
                    label5.Visible = true;
                    label1.Text = "Альбом";
                    label2.Text = "Жанр";
                    label3.Text = "Исполнитель";
                    label4.Text = "Лейбл";
                    label5.Text = "Кол - во песен";
                    textBox2.Visible = true;
                    textBox3.Visible = true;
                    textBox4.Visible = true;
                    textBox5.Visible = true;
                    textBox6.Visible = true;
                    break;
                case "Диск с документацией":
                    label1.Visible = true;
                    label2.Visible = true;
                    label1.Text = "Товар";
                    label2.Text = "Кол - во страниц";
                    textBox2.Visible = true;
                    textBox3.Visible = true;
                    break;
                case "Диск с программами":
                    label1.Visible = true;
                    label2.Visible = true;
                    label3.Visible = true;
                    label1.Text = "Программа";
                    label2.Text = "Год выпуска";
                    label3.Text = "Список ОС";
                    textBox2.Visible = true;
                    textBox3.Visible = true;
                    textBox4.Visible = true;
                    break;

            }
        }

        private string OneConditionSwitch(string condition, string category, string type)
        {
            string res = "";
            string value = "";
            List<string> values = condition.Split('&').ToList();
            List<char> signs = new List<char>() { '!', '>', '<', '=' };
            foreach (char c in condition)
            {
                if (signs.Contains(c))
                {
                    res += c;
                }
                else
                {
                    value += c;
                }
            }
            switch (res)
            {
                case "!=":
                case ">":
                case ">=":
                case "<":
                case "<=":
                    if (type == "num")
                        return $"{category} {res} {value}";
                    else
                        return $"{category} {res} N'{value}'";
                    break;
                default:
                    if (type == "num")
                        return $"{category} = {value}";
                    else
                        return $"{category} = N'{value}'";
                    break;
            }
        }

        private string ConditionSwitch(string condition, string category, string type)
        {
            string res = " (";
            int index = 0;
            List<string> values = condition.Split('&').ToList();
            foreach (string value in values)
            {
                if (index == 0)
                {
                    res += $" {OneConditionSwitch(value, category, type)}";
                }
                else
                {
                    res += $" AND {OneConditionSwitch(value, category, type)}";
                }
                index++;
            }
            return res + ")";
        }

        private string MakeCommand(string textBoxText, string category, string type)
        {
            string res = " AND(";
            int index = 0;
            List<string> conditions = textBoxText.Split(',').ToList();
            foreach (string cond in conditions)
            {
                if (index == 0)
                {
                    res += $" {ConditionSwitch(cond, category, type)}";
                }
                else
                {
                    res += $" OR {ConditionSwitch(cond, category, type)}";
                }
                index++;
            }
            return res + ")";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SqlCommand cmd = con.CreateCommand();
            switch (comboBox1.SelectedItem.ToString())
            {
                case "Аудиокнига":
                    string bookCommand = allListCmd + "WHERE disk_type = 'BOOK'";
                    if (!String.IsNullOrEmpty(textBox2.Text))
                    {
                        bookCommand += MakeCommand(textBox2.Text, "audiobook_author", "str");
                    }
                    if (!String.IsNullOrEmpty(textBox3.Text))
                    {
                        bookCommand += MakeCommand(textBox3.Text, "audiobook_title", "str");
                    }
                    if (!String.IsNullOrEmpty(textBox4.Text))
                    {
                        bookCommand += MakeCommand(textBox4.Text, "audiobook_duration", "num");
                    }
                    if (!String.IsNullOrEmpty(textBox5.Text))
                    {
                        bookCommand += MakeCommand(textBox5.Text, "audiobook_chapters", "num");
                    }
                    if (!String.IsNullOrEmpty(textBox7.Text))
                    {
                        bookCommand += MakeCommand(textBox7.Text, "price", "num");
                    }
                    if (!String.IsNullOrEmpty(textBox8.Text))
                    {
                        bookCommand += MakeCommand(textBox8.Text, "med.medium_name", "str");
                    }
                    if (!String.IsNullOrEmpty(textBox9.Text))
                    {
                        bookCommand += MakeCommand(textBox9.Text, "l.location_name", "str");
                    }
                    cmd.CommandText = bookCommand;
                    sAdapter = new SqlDataAdapter(cmd);
                    sTable = new DataTable();
                    sAdapter.Fill(sTable);
                    dataGridView1.DataSource = sTable;
                    break;
                case "Фильм":
                    string filmCommand = allListCmd + "WHERE disk_type = 'FILM'";
                    if (!String.IsNullOrEmpty(textBox2.Text))
                    {
                        filmCommand += MakeCommand(textBox2.Text, "film_genre", "str");
                    }
                    if (!String.IsNullOrEmpty(textBox3.Text))
                    {
                        filmCommand += MakeCommand(textBox3.Text, "film_name", "str");
                    }
                    if (!String.IsNullOrEmpty(textBox4.Text))
                    {
                        filmCommand += MakeCommand(textBox4.Text, "film_duration", "num");
                    }
                    if (!String.IsNullOrEmpty(textBox5.Text))
                    {
                        filmCommand += MakeCommand(textBox5.Text, "release_year", "num");
                    }
                    if (!String.IsNullOrEmpty(textBox6.Text))
                    {
                        filmCommand += MakeCommand(textBox6.Text, "film_rating", "num");
                    }
                    if (!String.IsNullOrEmpty(textBox7.Text))
                    {
                        filmCommand += MakeCommand(textBox7.Text, "price", "num");
                    }
                    if (!String.IsNullOrEmpty(textBox8.Text))
                    {
                        filmCommand += MakeCommand(textBox8.Text, "med.medium_name", "str");
                    }
                    if (!String.IsNullOrEmpty(textBox9.Text))
                    {
                        filmCommand += MakeCommand(textBox9.Text, "l.location_name", "str");
                    }
                    cmd.CommandText = filmCommand;
                    sAdapter = new SqlDataAdapter(cmd);
                    sTable = new DataTable();
                    sAdapter.Fill(sTable);
                    dataGridView1.DataSource = sTable;
                    break;
                case "Аудиодиск":
                    string musicCommand = allListCmd + "WHERE disk_type = 'MUSIC'";
                    if (!String.IsNullOrEmpty(textBox2.Text))
                    {
                        musicCommand += MakeCommand(textBox2.Text, "audiodisk_album", "str");
                    }
                    if (!String.IsNullOrEmpty(textBox3.Text))
                    {
                        musicCommand += MakeCommand(textBox3.Text, "music_genre", "str");
                    }
                    if (!String.IsNullOrEmpty(textBox4.Text))
                    {
                        musicCommand += MakeCommand(textBox4.Text, "audiodisk_performer", "str");
                    }
                    if (!String.IsNullOrEmpty(textBox5.Text))
                    {
                        musicCommand += MakeCommand(textBox5.Text, "audiodisk_label", "str");
                    }
                    if (!String.IsNullOrEmpty(textBox6.Text))
                    {
                        musicCommand += MakeCommand(textBox6.Text, "songs_quantity", "num");
                    }
                    if (!String.IsNullOrEmpty(textBox7.Text))
                    {
                        musicCommand += MakeCommand(textBox7.Text, "price", "num");
                    }
                    if (!String.IsNullOrEmpty(textBox8.Text))
                    {
                        musicCommand += MakeCommand(textBox8.Text, "med.medium_name", "str");
                    }
                    if (!String.IsNullOrEmpty(textBox9.Text))
                    {
                        musicCommand += MakeCommand(textBox9.Text, "l.location_name", "str");
                    }
                    cmd.CommandText = musicCommand;
                    sAdapter = new SqlDataAdapter(cmd);
                    sTable = new DataTable();
                    sAdapter.Fill(sTable);
                    dataGridView1.DataSource = sTable;
                    break;
                case "Диск с документацией":
                    string docCommand = allListCmd + "WHERE disk_type = 'DOC'";
                    if (!String.IsNullOrEmpty(textBox2.Text))
                    {
                        docCommand += MakeCommand(textBox2.Text, "docdisk_product", "str");
                    }
                    if (!String.IsNullOrEmpty(textBox3.Text))
                    {
                        docCommand += MakeCommand(textBox3.Text, "docdisk_pages", "num");
                    }
                    if (!String.IsNullOrEmpty(textBox7.Text))
                    {
                        docCommand += MakeCommand(textBox7.Text, "price", "num");
                    }
                    if (!String.IsNullOrEmpty(textBox8.Text))
                    {
                        docCommand += MakeCommand(textBox8.Text, "med.medium_name", "str");
                    }
                    if (!String.IsNullOrEmpty(textBox9.Text))
                    {
                        docCommand += MakeCommand(textBox9.Text, "l.location_name", "str");
                    }
                    cmd.CommandText = docCommand;
                    sAdapter = new SqlDataAdapter(cmd);
                    sTable = new DataTable();
                    sAdapter.Fill(sTable);
                    dataGridView1.DataSource = sTable;
                    break;
                case "Диск с программами":
                    string progCommand = allListCmd + "WHERE disk_type = 'PROG'";
                    if (!String.IsNullOrEmpty(textBox2.Text))
                    {
                        progCommand += MakeCommand(textBox2.Text, "progdisk_prog", "str");
                    }
                    if (!String.IsNullOrEmpty(textBox3.Text))
                    {
                        progCommand += MakeCommand(textBox3.Text, "release_year", "num");
                    }
                    if (!String.IsNullOrEmpty(textBox4.Text))
                    {
                        progCommand += MakeCommand(textBox4.Text, "progdisk_systems", "str");
                    }
                    if (!String.IsNullOrEmpty(textBox7.Text))
                    {
                        progCommand += MakeCommand(textBox7.Text, "price", "num");
                    }
                    if (!String.IsNullOrEmpty(textBox8.Text))
                    {
                        progCommand += MakeCommand(textBox8.Text, "med.medium_name", "str");
                    }
                    if (!String.IsNullOrEmpty(textBox9.Text))
                    {
                        progCommand += MakeCommand(textBox9.Text, "l.location_name", "str");
                    }
                    cmd.CommandText = progCommand;
                    sAdapter = new SqlDataAdapter(cmd);
                    sTable = new DataTable();
                    sAdapter.Fill(sTable);
                    dataGridView1.DataSource = sTable;
                    break;
                default:
                    string defCommand = allListCmd + "WHERE disk_type IS NOT NULL";
                    if (!String.IsNullOrEmpty(textBox7.Text))
                    {
                        defCommand += MakeCommand(textBox7.Text, "price", "num");
                    }
                    if (!String.IsNullOrEmpty(textBox8.Text))
                    {
                        defCommand += MakeCommand(textBox8.Text, "med.medium_name", "str");
                    }
                    if (!String.IsNullOrEmpty(textBox9.Text))
                    {
                        defCommand += MakeCommand(textBox9.Text, "l.location_name", "str");
                    }
                    cmd.CommandText = defCommand;
                    sAdapter = new SqlDataAdapter(cmd);
                    sTable = new DataTable();
                    sAdapter.Fill(sTable);
                    dataGridView1.DataSource = sTable;
                    break;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                placeAdapter.Update(placeTable);
            }
            catch (DBConcurrencyException dbcx)
            {
                MessageBox.Show("Возникла ошибка параллелизма - невозможно обновить базу данных. Перезапустите программу");
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Все столбцы должны быть заполнены, а также должны соответствовать типу данных");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                pubAdapter.Update(pubTable);
            }
            catch (DBConcurrencyException dbcx)
            {
                MessageBox.Show("Возникла ошибка параллелизма - невозможно обновить базу данных. Перезапустите программу");
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Все столбцы должны быть заполнены, а также должны соответствовать типу данных");
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            try
            {
                mediumAdapter.Update(mediumTable);
            }
            catch (DBConcurrencyException dbcx)
            {
                MessageBox.Show("Возникла ошибка параллелизма - невозможно обновить базу данных. Перезапустите программу");
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Все столбцы должны быть заполнены, а также должны соответствовать типу данных");
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            try
            {
                locAdapter.Update(locTable);
            }
            catch (DBConcurrencyException dbcx)
            {
                MessageBox.Show("Возникла ошибка параллелизма - невозможно обновить базу данных. Перезапустите программу");
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Все столбцы должны быть заполнены, а также должны соответствовать типу данных");
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                friendsAdapter.Update(friendsTable);
            }
            catch (DBConcurrencyException dbcx)
            {
                MessageBox.Show("Возникла ошибка параллелизма - невозможно обновить базу данных. Перезапустите программу");
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Все столбцы должны быть заполнены, а также должны соответствовать типу данных");
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            try
            {
                actionAdapter.Update(actionTable);
                actionpersAdapter.Update(actionpersTable);

                actionTable = new DataTable();
                actionAdapter.Fill(actionTable);
                dataGridView8.DataSource = actionTable;
            }
            catch (DBConcurrencyException dbcx)
            {
                MessageBox.Show("Возникла ошибка параллелизма - невозможно обновить базу данных. Перезапустите программу");
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Все столбцы должны быть заполнены, должны соответствовать типу данных, ссылаться на известные объекты");
            }
        }

        private void dataGridView8_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            switch (e.Exception.GetType().ToString())
            {
                case "System.FormatException":
                    MessageBox.Show("System.FormatException" + "\n\n" + "Значение имеет недопустимый формат");
                    break;
                case "System.Data.NoNullAllowedException":
                    MessageBox.Show("System.Data.NoNullAllowedException" + "\n\n" + "В столбце name не допускаются значения равные nulls");
                    break;
                default:
                    MessageBox.Show(e.Exception.GetType().ToString() + "\n\n" + e.Exception.Message);
                    break;
            }     
        }

        private void dataGridView8_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                string disk_id = dataGridView8.CurrentRow.Cells[0].Value.ToString();
                string disk_type = dataGridView8.CurrentRow.Cells[1].Value.ToString();
                SqlCommand cmd = con.CreateCommand();

                string command = "";
                switch (disk_type)
                {
                    case "FILM":
                        command = "SELECT disk_id AS '#', film_genre AS 'Жанр фильма', film_name AS 'Название', film_duration AS 'Хронометраж', release_year AS 'Год выпуска', film_rating AS 'Рейтинг' FROM disk WHERE disk_id = @d_id";
                        break;
                    case "BOOK":
                        command = "SELECT disk_id AS '#', audiobook_author AS 'Автор', audiobook_title AS 'Название книги', CAST(audiobook_duration AS VARCHAR) + N' минут' AS 'Длительность', CAST(audiobook_chapters AS VARCHAR) + N' глав(ы)' AS 'Кол - во глав' FROM disk WHERE disk_id = @d_id";
                        break;
                    case "MUSIC":
                        command = "SELECT disk_id AS '#', audiodisk_album AS 'Название альбома', music_genre AS 'Жанр', audiodisk_performer AS 'Исполнитель', audiodisk_label AS 'Лейбл', songs_quantity AS 'Кол - во песен' FROM disk WHERE disk_id = @d_id";
                        break;
                    case "DOC":
                        command = "SELECT disk_id AS '#', docdisk_product AS 'Товар', docdisk_pages AS 'Кол - во страниц' FROM disk WHERE disk_id = @d_id";
                        break;
                    case "PROG":
                        command = "SELECT disk_id AS '#', progdisk_prog AS 'Программа', release_year AS 'Год выпуска', progdisk_systems AS 'Список ОС' FROM disk WHERE disk_id = @d_id";
                        break;
                    default:
                        command = "SELECT NULL AS 'Ничего'";
                        break;
                }

                actionpersAdapter.SelectCommand = new SqlCommand(command, con);
                actionpersAdapter.SelectCommand.Parameters.AddWithValue("@d_id", disk_id);

                actionpersTable = new DataTable();
                actionpersAdapter.Fill(actionpersTable);
                dataGridView9.DataSource = actionpersTable;
            }
            catch (NullReferenceException ex)
            {
                MessageBox.Show("Кажется возникла ошибка при поиске такого пользователя, попробуйте еще раз");
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Кажется возникла ошибка при поиске такого пользователя, попробуйте еще раз");
            }

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            makeFilters2VisibilityFalse();
            button11.Visible = true;
            switch (comboBox2.SelectedItem.ToString())
            {
                case "Аудиокнига":
                    label10.Visible = true;
                    label11.Visible = true;
                    label12.Visible = true;
                    label13.Visible = true;
                    label10.Text = "Автор";
                    label11.Text = "Название книги";
                    label12.Text = "Длительность";
                    label13.Text = "Кол - во глав";
                    textBox13.Visible = true;
                    textBox14.Visible = true;
                    textBox15.Visible = true;
                    textBox16.Visible = true;
                    break;
                case "Фильм":
                    label10.Visible = true;
                    label11.Visible = true;
                    label12.Visible = true;
                    label13.Visible = true;
                    label14.Visible = true;
                    label10.Text = "Жанр фильма";
                    label11.Text = "Название";
                    label12.Text = "Хронометраж";
                    label13.Text = "Год выпуска";
                    label14.Text = "Рейтинг";
                    textBox13.Visible = true;
                    textBox14.Visible = true;
                    textBox15.Visible = true;
                    textBox16.Visible = true;
                    textBox17.Visible = true;
                    break;
                case "Аудиодиск":
                    label10.Visible = true;
                    label11.Visible = true;
                    label12.Visible = true;
                    label13.Visible = true;
                    label14.Visible = true;
                    label10.Text = "Альбом";
                    label11.Text = "Жанр";
                    label12.Text = "Исполнитель";
                    label13.Text = "Лейбл";
                    label14.Text = "Кол - во песен";
                    textBox13.Visible = true;
                    textBox14.Visible = true;
                    textBox15.Visible = true;
                    textBox16.Visible = true;
                    textBox17.Visible = true;
                    break;
                case "Диск с документацией":
                    label10.Visible = true;
                    label11.Visible = true;
                    label10.Text = "Товар";
                    label11.Text = "Кол - во страниц";
                    textBox13.Visible = true;
                    textBox14.Visible = true;
                    break;
                case "Диск с программами":
                    label10.Visible = true;
                    label11.Visible = true;
                    label12.Visible = true;
                    label10.Text = "Программа";
                    label11.Text = "Год выпуска";
                    label12.Text = "Список ОС";
                    textBox13.Visible = true;
                    textBox14.Visible = true;
                    textBox15.Visible = true;
                    break;

            }
           
        }

        private void button11_Click(object sender, EventArgs e)
        {
            string command = "INSERT INTO disk(disk_id, disk_type, place_id, medium_id, publisher_id, price, comments, hand_over,";

            try
            {
                int id = Int32.Parse(new SqlCommand("SELECT TOP 1 disk_id FROM disk ORDER BY disk_id DESC", con).ExecuteScalar().ToString()) + 1;
                string disk_type = "";
                switch (comboBox2.SelectedItem.ToString())
                {
                    case "Аудиодиск":
                        disk_type = "MUSIC";
                        break;
                    case "Фильм":
                        disk_type = "FILM";
                        break;
                    case "Аудиокнига":
                        disk_type = "BOOK";
                        break;
                    case "Диск с документацией":
                        disk_type = "DOC";
                        break;
                    case "Диск с программами":
                        disk_type = "PROG";
                        break;
                    default:
                        disk_type = "Неизвестный тип";
                        break;
                }
                int place_num = Int32.Parse(new SqlCommand("SELECT p.place_id FROM storage_place p JOIN location l ON p.location_id = l.location_id WHERE l.location_name = N'" + comboBox6.SelectedItem.ToString() + "' AND p.place_type = N'" + comboBox7.SelectedItem.ToString() + "' AND p.place_number = " + comboBox8.SelectedItem.ToString() + "", con).ExecuteScalar().ToString());
                int medium_id = Int32.Parse(new SqlCommand("SELECT medium_id FROM storage_medium WHERE medium_name = N'" + comboBox3.SelectedItem.ToString() + "'", con).ExecuteScalar().ToString());
                int publisher_id = Int32.Parse(new SqlCommand("SELECT publisher_id FROM publisher WHERE publisher_name = N'" + comboBox4.SelectedItem.ToString() + "'", con).ExecuteScalar().ToString());
                int price = Int32.Parse(textBox11.Text);
                string comments = textBox12.Text;
                int hand_over = Int32.Parse(new SqlCommand("SELECT friend_id FROM friends WHERE first_name + ' ' + last_name = N'" + comboBox5.SelectedItem.ToString() + "'", con).ExecuteScalar().ToString());



                switch (comboBox2.SelectedItem.ToString())
                {
                    case "Аудиокнига":
                        string author = textBox13.Text;
                        string title = textBox14.Text;
                        int dur = Int32.Parse(textBox15.Text);
                        int chap = Int32.Parse(textBox16.Text);
                        command += " audiobook_author, audiobook_title, audiobook_duration, audiobook_chapters) " +
                            "VALUES(" + id + ", '" + disk_type + "', " + place_num + ", " + medium_id + ", " + publisher_id + ", " + price + ",'" + comments + "', " + hand_over + ", N'" + author + "', N'" + title + "', " + dur + ", " + chap + ")";
                        new SqlCommand(command, con).ExecuteNonQuery();
                        break;
                    case "Фильм":
                        string genre = textBox13.Text;
                        string name = textBox14.Text;
                        int f_dur = Int32.Parse(textBox15.Text);
                        int year = Int32.Parse(textBox16.Text);
                        string rate = textBox17.Text;  
                        command += " film_genre, film_name, film_duration, release_year, film_rating) " +
                            "VALUES(" + id + ", '" + disk_type + "', " + place_num + ", " + medium_id + ", " + publisher_id + ", " + price + ",'" + comments + "', " + hand_over + ", N'" + genre + "', N'" + name + "', " + f_dur + ", " + year + ", " + rate + ")";
                        Console.WriteLine(command);
                        new SqlCommand(command, con).ExecuteNonQuery();
                        break;
                    case "Аудиодиск":
                        string album = textBox13.Text;
                        string m_genre = textBox14.Text;
                        string perf = textBox15.Text;
                        string label = textBox16.Text;
                        int quant = Int32.Parse(textBox17.Text);
                        command += " audiodisk_album, music_genre, audiodisk_performer, audiodisk_label, songs_quantity) " +
                           "VALUES(" + id + ", '" + disk_type + "', " + place_num + ", " + medium_id + ", " + publisher_id + ", " + price + ",'" + comments + "', " + hand_over + ", N'" + album + "', N'" + m_genre + "', N'" + perf + "', N'" + label + "', " + quant + ")";
                        new SqlCommand(command, con).ExecuteNonQuery();
                        break;
                    case "Диск с документацией":
                        string prod = textBox13.Text;
                        int pages = Int32.Parse(textBox14.Text);
                        command += " docdisk_product, docdisk_pages) " +
                           "VALUES(" + id + ", '" + disk_type + "', " + place_num + ", " + medium_id + ", " + publisher_id + ", " + price + ",'" + comments + "', " + hand_over + ", N'" + prod + "', " + pages + ")";
                        new SqlCommand(command, con).ExecuteNonQuery();
                        break;
                    case "Диск с программами":
                        string prog = textBox13.Text;
                        int p_year = Int32.Parse(textBox14.Text);
                        string sys = textBox15.Text;
                        command += " progdisk_prog, release_year, progdisk_systems) " +
                           "VALUES(" + id + ", '" + disk_type + "', " + place_num + ", " + medium_id + ", " + publisher_id + ", " + price + ",'" + comments + "', " + hand_over + ", N'" + prog + "', " + p_year + ", N'" + sys + "')";
                        new SqlCommand(command, con).ExecuteNonQuery();
                        break;

                }
            }
            catch (NullReferenceException ex)
            {
                MessageBox.Show("Обязательно все столбцы, исключая комментарии и одолжение, должны быть заполнены, должны соответствовать типу данных, ссылаться на известные объекты");
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
                MessageBox.Show("Обязательно все столбцы, исключая комментарии и одолжение, должны быть заполнены, должны соответствовать типу данных, ссылаться на известные объекты");
            }
            catch(ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void button12_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog1 = new OpenFileDialog())
            {
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string fileName = openFileDialog1.FileName;
                    string disk_id = textBox10.Text;
                    string sql = "UPDATE disk SET scan_img = (SELECT * FROM OPENROWSET(BULK N'" + fileName + "', SINGLE_BLOB) AS t1)  WHERE disk_id = @d_id";
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@d_id", disk_id);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            label22.Visible = true;
            comboBox7.Visible = true;
            comboBox7.Items.Clear();
            comboBox8.Items.Clear();

            SqlCommand locations_place = con.CreateCommand();
            locations_place.CommandText = "SELECT DISTINCT s.place_type FROM storage_place s JOIN location l ON s.location_id = l.location_id WHERE l.location_name = N'" + comboBox6.SelectedItem.ToString() + "'";
            using (SqlDataReader reader = locations_place.ExecuteReader())
            {
                while (reader.Read())
                {
                    comboBox7.Items.Add(String.Format("{0}", reader[0]));
                }
            }
        }

        private void comboBox7_SelectedIndexChanged(object sender, EventArgs e)
        {
            label23.Visible = true;
            comboBox8.Visible = true;
            comboBox8.Items.Clear();

            SqlCommand place_num = con.CreateCommand();
            place_num.CommandText = "SELECT DISTINCT s.place_number FROM storage_place s JOIN location l ON s.location_id = l.location_id WHERE l.location_name = N'" + comboBox6.SelectedItem.ToString() + "' AND s.place_type = N'" + comboBox7.SelectedItem.ToString() + "'";
            using (SqlDataReader reader = place_num.ExecuteReader())
            {
                while (reader.Read())
                {
                    comboBox8.Items.Add(String.Format("{0}", reader[0]));
                }
            }
        }
    }
}

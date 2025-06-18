using BCrypt.Net;
using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
           string username = textBox1.Text.Trim();
           string password = textBoxPassword.Text;

           string connStr = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=postgres";
           
           using (var conn = new NpgsqlConnection(connStr))
            {
                conn.Open();
                string sql = "select * from employees where username = @username";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("username", username);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string storedHash = reader.GetString(2);
                            string role = reader.GetString(3);

                            if (BCrypt.Net.BCrypt.Verify(password, storedHash))
                            {
                                MessageBox.Show($"Добро пожаловать, {username}! Роль: {role}");

                                this.Hide();
                                Form2 mainForm = new Form2(role, username);

                                mainForm.Show();
                            }
                            else
                            {
                                MessageBox.Show("❌ Неверный пароль");
                            }
                        }
                        else
                        {
                            MessageBox.Show("❌ Пользователь не найден");
                        }
                    }
                }

            }
        
        
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace WindowsFormsApp1
{
    public partial class Form2 : Form
    {
        private string role;
        private string username;
        public Form2(string role, string username)
        {
            InitializeComponent();

            this.role = role;
            this.username = username;

            lblWelcome.Text = $"Добро пожаловать, {username}! Роль: {role}";

            if (role == "manager")
            {
                btnEmployees.Enabled = false;
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void lblWelcome_Click(object sender, EventArgs e)
        {

        }

        private void btnEmployees_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Открывается управление сотрудниками");
        }

        private void btnCars_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Открывается управление машинами");
        }

        private void btnRentals_Click(object sender, EventArgs e)
        {
            RentalsForm rentals = new RentalsForm();
            rentals.ShowDialog();
        }
    }
}

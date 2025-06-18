using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;

namespace WindowsFormsApp1
{
    public partial class RentalsForm : Form
    {
        private string connStr = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=postgres";
        public RentalsForm()
        {
            InitializeComponent();
            LoadRentals();
        }

        private void LoadRentals()
        {
            using (var conn = new NpgsqlConnection(connStr))
            {
                conn.Open();
                string sql = @"
                    SELECT 
                        rentals.id,
                        cars.brand || ' ' || cars.model AS car,
                        employees.full_name AS employee,
                        start_time,
                        end_time,
                        price_per_hour,
                        total_price,
                        return_condition
                    FROM rentals
                    JOIN cars ON rentals.car_id = cars.id
                    JOIN employees ON rentals.employee_id = employees.id
                    ORDER BY start_time DESC";

                using (var da = new NpgsqlDataAdapter(sql, conn))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dataGridView1.DataSource = dt;
                }
            }
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            LoadRentals();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void buttonAverageIncome_Click(object sender, EventArgs e)
        {
            using (var conn = new NpgsqlConnection(connStr))
            {
                conn.Open();
                string sql = @"
                    SELECT 
                        SUM(total_price)/(SELECT count(*) from rentals where total_price is not NULL)
                    FROM rentals";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (!reader.IsDBNull(0))
                            {
                                decimal avg = reader.GetInt16(0);
                                MessageBox.Show($"Средний доход: {avg} ₽");
                            }
                            else
                            {
                                MessageBox.Show("Нет завершённых аренд для подсчёта.");
                            }
                        }
                    }
                }
            }
        }

        private void RentalsForm_Load(object sender, EventArgs e)
        {

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите запись для удаления.");
                return;
            }

            var id = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["id"].Value);

            var confirm = MessageBox.Show("Удалить аренду?", "Подтверждение", MessageBoxButtons.YesNo);
            if (confirm != DialogResult.Yes) return;

            using (var conn = new NpgsqlConnection(connStr))
            {
                conn.Open();
                string sql = "DELETE FROM rentals WHERE id = @id";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("id", id);
                    cmd.ExecuteNonQuery();
                }
            }

            LoadRentals();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            RentalEditForm form = new RentalEditForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadRentals();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите аренду для редактирования.");
                return;
            }

            int id = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["id"].Value);

            RentalEditForm form = new RentalEditForm(id);
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadRentals();
            }
        }
    }
}

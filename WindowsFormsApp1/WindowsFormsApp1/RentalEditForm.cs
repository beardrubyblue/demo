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
    public partial class RentalEditForm : Form
    {
        private string connStr = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=postgres";
        private int? rentalId = null;
        public RentalEditForm(int? rentalId = null)
        {
            InitializeComponent();
            this.rentalId = rentalId;
            LoadCars();
            LoadEmployees();
            if (rentalId.HasValue)
                LoadRentalData(rentalId.Value);
        }
        private void LoadRentalData(int id)
        {
            using (var conn = new NpgsqlConnection(connStr))
            {
                conn.Open();
                var cmd = new NpgsqlCommand("SELECT * FROM rentals WHERE id = @id", conn);
                cmd.Parameters.AddWithValue("id", id);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        cbCar.SelectedValue = reader.GetInt32(reader.GetOrdinal("car_id"));
                        cbEmployee.SelectedValue = reader.GetInt32(reader.GetOrdinal("employee_id"));
                        dtStart.Value = reader.GetDateTime(reader.GetOrdinal("start_time"));

                        int endIdx = reader.GetOrdinal("end_time");
                        if (!reader.IsDBNull(endIdx))
                        {
                            dtEnd.Value = reader.GetDateTime(endIdx);
                            dtEnd.Checked = true;
                        }
                        else
                        {
                            dtEnd.Checked = false;
                        }

                        txtPricePerHour.Text = reader["price_per_hour"].ToString();
                        txtTotalPrice.Text = reader["total_price"]?.ToString();
                        txtCondition.Text = reader["return_condition"]?.ToString();
                    }
                }
            }
        }

        private void LoadCars()
        {
            using (var conn = new NpgsqlConnection(connStr))
            {
                conn.Open();
                var cmd = new NpgsqlCommand("SELECT id, brand || ' ' || model AS title FROM cars", conn);
                var reader = cmd.ExecuteReader();

                var dict = new Dictionary<int, string>();
                while (reader.Read())
                {
                    dict[reader.GetInt32(0)] = reader.GetString(1);
                }

                cbCar.DataSource = new BindingSource(dict, null);
                cbCar.DisplayMember = "Value";
                cbCar.ValueMember = "Key";
            }
        }

        private void LoadEmployees()
        {
            using (var conn = new NpgsqlConnection(connStr))
            {
                conn.Open();
                var cmd = new NpgsqlCommand("SELECT id, full_name FROM employees", conn);
                var reader = cmd.ExecuteReader();

                var dict = new Dictionary<int, string>();
                while (reader.Read())
                {
                    dict[reader.GetInt32(0)] = reader.GetString(1);
                }

                cbEmployee.DataSource = new BindingSource(dict, null);
                cbEmployee.DisplayMember = "Value";
                cbEmployee.ValueMember = "Key";
            }
        }
        private void RentalEditForm_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void txtCondition_TextChanged(object sender, EventArgs e)
        {

        }

        private void cbCar_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cbEmployee_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void dtStart_ValueChanged(object sender, EventArgs e)
        {

        }

        private void dtEnd_ValueChanged(object sender, EventArgs e)
        {

        }

        private void txtPricePerHour_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtTotalPrice_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            int carId = ((KeyValuePair<int, string>)cbCar.SelectedItem).Key;
            int empId = ((KeyValuePair<int, string>)cbEmployee.SelectedItem).Key;
            DateTime start = dtStart.Value;
            DateTime? end = dtEnd.Checked ? dtEnd.Value : (DateTime?)null;
            decimal pricePerHour = decimal.Parse(txtPricePerHour.Text);
            decimal? totalPrice = string.IsNullOrWhiteSpace(txtTotalPrice.Text) ? (decimal?)null : decimal.Parse(txtTotalPrice.Text);
            string condition = txtCondition.Text;

            using (var conn = new NpgsqlConnection(connStr))
            {
                conn.Open();
                string sql;

                if (rentalId.HasValue)
                {
                    sql = @"
                UPDATE rentals
                SET car_id = @car, employee_id = @emp, start_time = @start, end_time = @end,
                    price_per_hour = @pph, total_price = @total, return_condition = @cond
                WHERE id = @id";
                }
                else
                {
                    sql = @"
                INSERT INTO rentals (car_id, employee_id, start_time, end_time, price_per_hour, total_price, return_condition)
                VALUES (@car, @emp, @start, @end, @pph, @total, @cond)";
                }

                var cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("car", carId);
                cmd.Parameters.AddWithValue("emp", empId);
                cmd.Parameters.AddWithValue("start", start);
                cmd.Parameters.AddWithValue("end", (object)end ?? DBNull.Value);
                cmd.Parameters.AddWithValue("pph", pricePerHour);
                cmd.Parameters.AddWithValue("total", (object)totalPrice ?? DBNull.Value);
                cmd.Parameters.AddWithValue("cond", condition);

                if (rentalId.HasValue)
                    cmd.Parameters.AddWithValue("id", rentalId.Value);

                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Аренда сохранена.");
            DialogResult = DialogResult.OK;
            Close();
        }

    }
}

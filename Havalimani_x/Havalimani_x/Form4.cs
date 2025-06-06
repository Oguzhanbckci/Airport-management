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

namespace Havalimani_x
{
    public partial class Form4 : Form
    {
        string girisYapanKullanici;
        string kullaniciGorev;
        SqlConnection con = new SqlConnection(@"Server=.;Database=HavalimaniX2;Trusted_Connection=True;");

        public Form4(string kullanıcı,string gorev)
        {
            InitializeComponent();
            girisYapanKullanici = kullanıcı;
            kullaniciGorev = gorev; 
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            YolculariGetir();
            UcuslariGetir();
            BiletleriListele();
        }

        private void YolculariGetir()
        {
            SqlDataAdapter da = new SqlDataAdapter("SELECT YolcuID, Ad + ' ' + Soyad AS AdSoyad FROM Yolcular", con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            comboBox1.DisplayMember = "AdSoyad";
            comboBox1.ValueMember = "YolcuID";
            comboBox1.DataSource = dt;
        }

        private void UcuslariGetir()
        {
            SqlDataAdapter da = new SqlDataAdapter("SELECT UcusID, UcusNo FROM Ucuslar", con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            comboBox2.DisplayMember = "UcusNo";
            comboBox2.ValueMember = "UcusID";
            comboBox2.DataSource = dt;
        }
        private void BiletleriListele()
        {
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Vw_YolcuUcusBilgisi", con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dataGridView1.DataSource = dt;
        }



        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                int biletID = Convert.ToInt32(dataGridView1.CurrentRow.Cells["BiletID"].Value);

                SqlCommand cmd = new SqlCommand("DELETE FROM Biletler WHERE BiletID = @ID", con);
                cmd.Parameters.AddWithValue("@ID", biletID);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                MessageBox.Show("Bilet silindi.");
                BiletleriListele();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                int biletID = Convert.ToInt32(dataGridView1.CurrentRow.Cells["BiletID"].Value);

                SqlCommand cmd = new SqlCommand("UPDATE Biletler SET YolcuID = @YolcuID, UcusID = @UcusID, KoltukNo = @KoltukNo WHERE BiletID = @BiletID", con);
                cmd.Parameters.AddWithValue("@YolcuID", comboBox1.SelectedValue);
                cmd.Parameters.AddWithValue("@UcusID", comboBox2.SelectedValue);
                cmd.Parameters.AddWithValue("@KoltukNo", textBox1.Text.Trim());
                cmd.Parameters.AddWithValue("@BiletID", biletID);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                MessageBox.Show("Bilet güncellendi.");
                BiletleriListele();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("INSERT INTO Biletler (YolcuID, UcusID, KoltukNo, SatinAlmaTarihi) VALUES (@YolcuID, @UcusID, @KoltukNo, GETDATE())", con);
                cmd.Parameters.AddWithValue("@YolcuID", comboBox1.SelectedValue);
                cmd.Parameters.AddWithValue("@UcusID", comboBox2.SelectedValue);
                cmd.Parameters.AddWithValue("@KoltukNo", textBox1.Text.Trim());

                con.Open();
                cmd.ExecuteNonQuery();
                MessageBox.Show("Bilet başarıyla eklendi.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
            finally
            {
                con.Close();
                BiletleriListele();
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                comboBox1.Text = row.Cells["YolcuAdi"].Value.ToString();
                textBox1.Text = row.Cells["KoltukNo"].Value.ToString();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string arama = textBox5.Text.Trim();

            if (arama != "")
            {
                SqlDataAdapter da = new SqlDataAdapter(@"
            SELECT * FROM Vw_YolcuUcusBilgisi 
            WHERE YolcuAdi LIKE @Arama OR KalkisHavalimani LIKE @Arama OR VarisHavalimani LIKE @Arama", con);

                da.SelectCommand.Parameters.AddWithValue("@Arama", "%" + arama + "%");

                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;
            }
            else
            {
                BiletleriListele(); // boşsa tüm listeyi getircek
            }
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            string arama = textBox5.Text.Trim();

            //ad havalimanına göre anlık 

            SqlDataAdapter da = new SqlDataAdapter(@"
        SELECT * FROM Vw_YolcuUcusBilgisi 
        WHERE YolcuAdi LIKE @Arama 
           OR KalkisHavalimani LIKE @Arama 
           OR VarisHavalimani LIKE @Arama", con);

            da.SelectCommand.Parameters.AddWithValue("@Arama", "%" + arama + "%");

            DataTable dt = new DataTable();
            da.Fill(dt);
            dataGridView1.DataSource = dt;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form6 form6 = new Form6(girisYapanKullanici, kullaniciGorev);
            form6.Show();
        }
    }
}

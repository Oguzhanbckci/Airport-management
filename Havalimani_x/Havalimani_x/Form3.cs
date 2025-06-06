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
    public partial class Form3 : Form
    {
        string girisYapanKullanici;
        string kullaniciGorev;
        SqlConnection con = new SqlConnection(@"Server=.;Database=HavalimaniX2;Trusted_Connection=True;");

        public Form3(string kullanıcı,string gorev)
        {
            InitializeComponent();
            girisYapanKullanici =kullanıcı;
            kullaniciGorev=gorev;
        }

        private void VerileriListele()
        {
            try
            {
                if (con.State != ConnectionState.Open)
                    con.Open();

                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Yolcular", con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veri çekme hatası: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }



        private void Form3_Load(object sender, EventArgs e)
        {
            VerileriListele();
        }
      
        private void dataGridView1_CellClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)   // Başlık satırı tıklanmadıysa
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];    // Tıklanan satırı al
                textBox1.Text = row.Cells["Ad"].Value.ToString();        // Hücre içeriğini textbox'a aktar
                textBox2.Text = row.Cells["Soyad"].Value.ToString();
                textBox3.Text = row.Cells["TCNo"].Value.ToString();
                textBox4.Text = row.Cells["Telefon"].Value.ToString();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (textBox1.Text != "" && textBox2.Text != "" && textBox3.Text != "" && textBox4.Text != "")
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("INSERT INTO Yolcular (Ad, Soyad, TCNo, Telefon) VALUES (@Ad, @Soyad, @TC, @Tel)", con);
                    cmd.Parameters.AddWithValue("@Ad", textBox1.Text.Trim());
                    cmd.Parameters.AddWithValue("@Soyad", textBox2.Text.Trim());
                    cmd.Parameters.AddWithValue("@TC", textBox3.Text.Trim());
                    cmd.Parameters.AddWithValue("@Tel", textBox4.Text.Trim());

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Yolcu başarıyla eklendi.");
                    VerileriListele();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ekleme hatası: " + ex.Message);
                }
                finally
                {
                    if (con.State == ConnectionState.Open)
                        con.Close();
                }
            }
            else
            {
                MessageBox.Show("Lütfen tüm alanları doldurun.");
            }

            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();


        }

        private void button2_Click(object sender, EventArgs e)
        {

            if (dataGridView1.CurrentRow != null)
            {
                int secilenID = Convert.ToInt32(dataGridView1.CurrentRow.Cells["YolcuID"].Value);
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(@"
                UPDATE Yolcular SET
                    Ad = @Ad,
                    Soyad = @Soyad,
                    TCNo = @TC,
                    Telefon = @Tel
                WHERE YolcuID = @ID", con);

                    cmd.Parameters.AddWithValue("@Ad", textBox1.Text.Trim());
                    cmd.Parameters.AddWithValue("@Soyad", textBox2.Text.Trim());
                    cmd.Parameters.AddWithValue("@TC", textBox3.Text.Trim());
                    cmd.Parameters.AddWithValue("@Tel", textBox4.Text.Trim());
                    cmd.Parameters.AddWithValue("@ID", secilenID);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Yolcu bilgisi güncellendi.");
                    VerileriListele();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Güncelleme hatası: " + ex.Message);
                }
                finally
                {
                    con.Close();
                }
            }
            else
            {
                MessageBox.Show("Güncellemek için listeden bir yolcu seçin.");
            }

            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                int secilenID = Convert.ToInt32(dataGridView1.CurrentRow.Cells["YolcuID"].Value);
                DialogResult dr = MessageBox.Show("Yolcu silinsin mi?", "Onay", MessageBoxButtons.YesNo);
                if (dr == DialogResult.Yes)
                {
                    try
                    {
                        con.Open();
                        SqlCommand cmd = new SqlCommand("DELETE FROM Yolcular WHERE YolcuID = @ID", con);
                        cmd.Parameters.AddWithValue("@ID", secilenID);

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Yolcu silindi.");
                        VerileriListele(); // Yeniden yükle
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Silme hatası: " + ex.Message);
                    }
                    finally
                    {
                        con.Close();
                    }
                }
            }
            else
            {
                MessageBox.Show("Lütfen silmek için bir yolcu seçin.");
            }

            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();

        }

        private void button4_Click(object sender, EventArgs e)
        {
            string aranan = textBox5.Text.Trim();

            try
            {
                if (con.State != ConnectionState.Open)
                    con.Open();

                SqlCommand cmd = new SqlCommand(@"
            SELECT * FROM Yolcular 
            WHERE Ad LIKE @aranan OR Soyad LIKE @aranan OR TCNo LIKE @aranan", con);
                cmd.Parameters.AddWithValue("@aranan", "%" + aranan + "%");

                // oğluum çok iyi oldu la ad soyad tcye göre adamın verisini getiroomm

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Arama hatası: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
            textBox5.Clear();

            //BUTON SAYIM AZ DİYE TEK TEK YAZDIM CLEARLARIMI YOKSA FONX YAPABİLİRDİM 

        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                if (con.State != ConnectionState.Open)
                    con.Open();

                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM YolcuSilmeLog ORDER BY SilmeTarihi DESC", con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;  
            }
            catch (Exception ex)
            {
                MessageBox.Show("Log verisi çekilemedi: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form6 form6 = new Form6(girisYapanKullanici, kullaniciGorev);
            form6.Show();
        }
    }
}



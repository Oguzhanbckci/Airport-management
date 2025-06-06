using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Havalimani_x
{
    public partial class Form2 : Form
    {
        string girisYapanKullanici;
        string kullaniciGorev;
        SqlConnection con = new SqlConnection(@"Server=.;Database=HavalimaniX2;Trusted_Connection=True;");

        public Form2(string kullanici, string gorev)
        {
            InitializeComponent();
            // form1 imden gelen veriyi kullanma 

            girisYapanKullanici = kullanici;
            kullaniciGorev = gorev;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect; // veri tablomun seçme özelliği
            dataGridView1.MultiSelect = false;

            comboBox1.Items.AddRange(new string[] { "Tümü", "Aktif", "Pasif" });
            comboBox1.SelectedIndex = 0;

            VerileriListele();

            switch (kullaniciGorev)
            {
                case "Yönetici":
                    button2.Enabled = true;
                    button3.Enabled = true;
                    button4.Enabled = true;

                    // Butonlarımın textleri görünür olsun

                    button2.Text = "Ekle";
                    button3.Text = "Güncelle";
                    button4.Text = "Sil";
                    break;

                case "Uçuş Personeli":
                    button2.Enabled = false;
                    button3.Enabled = true;
                    button4.Enabled = false;

                    button2.Text = "";
                    button4.Text = "";
                    break;

                default:
                    button2.Enabled = false;
                    button3.Enabled = false;
                    button4.Enabled = false;

                    button2.Text = "";
                    button3.Text = "";
                    button4.Text = "";
                    break;
            }

            DoldurComboBoxlar();
        }

        private void DoldurComboBoxlar()
        {
            try
            {
                con.Open();

                // Havalimanları comboBox2 ve comboBox3 için

                string havalimaniSorgu = "SELECT HavalimaniID, Ad FROM Havalimanlari";
                SqlDataAdapter daHavalimani = new SqlDataAdapter(havalimaniSorgu, con);
                DataTable dtHavalimani = new DataTable();
                daHavalimani.Fill(dtHavalimani);

                comboBox2.DataSource = dtHavalimani.Copy();
                comboBox2.DisplayMember = "Ad";
                comboBox2.ValueMember = "HavalimaniID";

                comboBox3.DataSource = dtHavalimani.Copy();
                comboBox3.DisplayMember = "Ad";
                comboBox3.ValueMember = "HavalimaniID";

                // Uçaklar comboBox4 için

                string ucaklarSorgu = "SELECT UcakID, Model FROM Ucaklar";
                SqlDataAdapter daUcaklar = new SqlDataAdapter(ucaklarSorgu, con);
                DataTable dtUcaklar = new DataTable();
                daUcaklar.Fill(dtUcaklar);

                comboBox4.DataSource = dtUcaklar;
                comboBox4.DisplayMember = "Model";
                comboBox4.ValueMember = "UcakID";

                // Durum comboBox5 için

                comboBox5.Items.Clear();
                comboBox5.Items.Add("Aktif");
                comboBox5.Items.Add("Pasif");
                comboBox5.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("ComboBox doldurma hatası: " + ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        private void VerileriListele(string ucusNo = "", string durum = "")
        {
            try
            {
                string sorgu = @"
            SELECT 
                u.UcusID, u.UcusNo, u.KalkisZamani, u.VarisZamani,
                h1.Ad AS KalkisHavalimani, 
                h2.Ad AS VarisHavalimani,
                uc.Model, uc.Kapasite,
                CASE WHEN u.Durum = 1 THEN 'Aktif' ELSE 'Pasif' END AS Durum
            FROM Ucuslar u
            INNER JOIN Havalimanlari h1 ON u.KalkisHavalimaniID = h1.HavalimaniID
            INNER JOIN Havalimanlari h2 ON u.VarisHavalimaniID = h2.HavalimaniID
            INNER JOIN Ucaklar uc ON u.UcakID = uc.UcakID
            WHERE (@ucusNo = '' OR u.UcusNo LIKE '%' + @ucusNo + '%')
            AND (@durum = '' OR (@durum = 'Aktif' AND u.Durum = 1) OR (@durum = 'Pasif' AND u.Durum = 0))
            ";

                using (SqlCommand cmd = new SqlCommand(sorgu, con))
                {
                    cmd.Parameters.AddWithValue("@ucusNo", ucusNo);
                    cmd.Parameters.AddWithValue("@durum", durum);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dataGridView1.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Listeleme Hatası: " + ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string ucusNoAra = textBox1.Text.Trim();
            string durumFiltre = comboBox1.SelectedItem?.ToString() ?? "";
            VerileriListele(ucusNoAra, durumFiltre == "Tümü" ? "" : durumFiltre);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
            textBox1.Clear();
            VerileriListele();
        }


        private void button2_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(textBox2.Text))
                {
                    MessageBox.Show("Uçuş No boş olamaz.");
                    return;
                }
                if (comboBox2.SelectedValue == null || comboBox3.SelectedValue == null || comboBox4.SelectedValue == null)
                {
                    MessageBox.Show("Lütfen tüm comboboxları seçiniz.");
                    return;
                }

                con.Open();

                // Aynı uçuş numarası var mı kontrol edicez

                string kontrolSorgu = "SELECT COUNT(*) FROM Ucuslar WHERE UcusNo = @UcusNo";
                SqlCommand kontrolCmd = new SqlCommand(kontrolSorgu, con);
                kontrolCmd.Parameters.AddWithValue("@UcusNo", textBox2.Text.Trim());
                int mevcut = (int)kontrolCmd.ExecuteScalar();

                if (mevcut > 0)
                {
                    MessageBox.Show("Bu uçuş numarası zaten kayıtlı. Lütfen farklı bir uçuş numarası girin.");
                    return;
                }

                string sorgu = @"
             INSERT INTO Ucuslar (UcusNo, KalkisHavalimaniID, VarisHavalimaniID, KalkisZamani, VarisZamani, UcakID, Durum)
             VALUES (@UcusNo, @KalkisID, @VarisID, @KalkisZamani, @VarisZamani, @UcakID, @Durum)";

                SqlCommand cmd = new SqlCommand(sorgu, con);
                cmd.Parameters.AddWithValue("@UcusNo", textBox2.Text.Trim());
                cmd.Parameters.AddWithValue("@KalkisID", comboBox2.SelectedValue);
                cmd.Parameters.AddWithValue("@VarisID", comboBox3.SelectedValue);
                cmd.Parameters.AddWithValue("@KalkisZamani", dateTimePicker1.Value);
                cmd.Parameters.AddWithValue("@VarisZamani", dateTimePicker2.Value);
                cmd.Parameters.AddWithValue("@UcakID", comboBox4.SelectedValue);
                cmd.Parameters.AddWithValue("@Durum", comboBox5.SelectedItem.ToString() == "Aktif" ? 1 : 0);

                int etkilenen = cmd.ExecuteNonQuery();

                if (etkilenen > 0)
                {
                    MessageBox.Show("Uçuş başarıyla eklendi.");
                    VerileriListele();
                }
                else
                {
                    MessageBox.Show("Ekleme işlemi başarısız oldu.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ekleme Hatası: " + ex.ToString());
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }

            textBox2.Clear();

        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            // Seçili satır yoksa işlem yapma

            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen silmek için listeden bir uçuş seçin.");
                return;
            }

            int silinecekUcusNo = -1;

            var ucusIdCell = dataGridView1.SelectedRows[0].Cells["UcusID"];
            if (ucusIdCell != null && ucusIdCell.Value != null)
            {
                silinecekUcusNo = Convert.ToInt32(ucusIdCell.Value);
            }
            else
            {
                MessageBox.Show("Seçili satırda geçerli bir uçuş numarası bulunamadı.");
                return;
            }

            DialogResult dr = MessageBox.Show($"Uçuş No {silinecekUcusNo} silinecek. Onaylıyor musunuz?", "Onay", MessageBoxButtons.YesNo);
            if (dr == DialogResult.Yes)
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("DELETE FROM Ucuslar WHERE UcusID = @ID", con);
                    cmd.Parameters.AddWithValue("@ID", silinecekUcusNo);
                    int etkilenen = cmd.ExecuteNonQuery();

                    if (etkilenen > 0)
                    {
                        MessageBox.Show("Uçuş silindi.");
                        VerileriListele();
                    }
                    else
                    {
                        MessageBox.Show("Silme işlemi başarısız oldu. Girilen uçuş numarası bulunamadı.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Silme Hatası: " + ex.Message);
                }
                finally
                {
                    con.Close();
                }
            }
        }




        private void button3_Click_1(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                int secilenID = Convert.ToInt32(dataGridView1.CurrentRow.Cells["UcusID"].Value);
                string yeniUcusNo = textBox2.Text.Trim();

                try
                {
                    con.Open();

                    // Önce veritabanından mevcut UcusNo'yu çekiyoruz

                    string mevcutSorgu = "SELECT UcusNo FROM Ucuslar WHERE UcusID = @ID";
                    SqlCommand mevcutCmd = new SqlCommand(mevcutSorgu, con);
                    mevcutCmd.Parameters.AddWithValue("@ID", secilenID);
                    string mevcutUcusNo = Convert.ToString(mevcutCmd.ExecuteScalar()).Trim();

                    // Eğer girilen uçuş no, mevcut uçuş no'dan farklıysa kontrol et

                    if (!string.Equals(mevcutUcusNo, yeniUcusNo, StringComparison.OrdinalIgnoreCase))
                    {
                        string kontrolSorgu = "SELECT COUNT(*) FROM Ucuslar WHERE UcusNo = @UcusNo AND UcusID != @UcusID";
                        SqlCommand kontrolCmd = new SqlCommand(kontrolSorgu, con);
                        kontrolCmd.Parameters.AddWithValue("@UcusNo", yeniUcusNo);
                        kontrolCmd.Parameters.AddWithValue("@UcusID", secilenID);

                        int sayi = (int)kontrolCmd.ExecuteScalar();
                        if (sayi > 0)
                        {
                            MessageBox.Show("Bu uçuş numarası zaten başka bir kayıt için kullanılıyor. Lütfen farklı bir uçuş numarası girin.");
                            return;
                        }
                    }

                    // Güncelleme update tarafı 

                    string sorgu = @"
                UPDATE Ucuslar SET
                    UcusNo = @UcusNo,
                    KalkisHavalimaniID = @KalkisID,
                    VarisHavalimaniID = @VarisID,
                    KalkisZamani = @KalkisZamani,
                    VarisZamani = @VarisZamani,
                    UcakID = @UcakID,
                    Durum = @Durum
                WHERE UcusID = @UcusID";

                    SqlCommand cmd = new SqlCommand(sorgu, con);

                    cmd.Parameters.AddWithValue("@UcusNo", yeniUcusNo);
                    cmd.Parameters.AddWithValue("@KalkisID", comboBox2.SelectedValue);
                    cmd.Parameters.AddWithValue("@VarisID", comboBox3.SelectedValue);
                    cmd.Parameters.AddWithValue("@KalkisZamani", dateTimePicker1.Value);
                    cmd.Parameters.AddWithValue("@VarisZamani", dateTimePicker2.Value);
                    cmd.Parameters.AddWithValue("@UcakID", comboBox4.SelectedValue);
                    cmd.Parameters.AddWithValue("@Durum", comboBox5.SelectedItem.ToString() == "Aktif" ? 1 : 0);
                    cmd.Parameters.AddWithValue("@UcusID", secilenID);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Uçuş başarıyla güncellendi.");
                    VerileriListele();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Güncelleme Hatası: " + ex.Message);
                }
                finally
                {
                    if (con.State == ConnectionState.Open)
                        con.Close();
                }
            }
            else
            {
                MessageBox.Show("Lütfen güncellemek istediğiniz uçuşu listeden seçin.");
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

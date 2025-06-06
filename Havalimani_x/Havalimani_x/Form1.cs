using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Havalimani_x
{
    public partial class Form1 : Form
    {
        // Veritabanı ve kullanıcı girişi + diğer formlar içşn

        SqlConnection con = new SqlConnection(@"Server=.;Database=HavalimaniX2;Trusted_Connection=True;");

       
        string girisYapanKullanici = "";
        string kullaniciGorev = "";

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            string kullaniciAdi = textBox1.Text.Trim();
            string parola = textBox2.Text.Trim();

            con.Open();
            SqlCommand cmd = new SqlCommand("SELECT * FROM Personel WHERE KullaniciAdi=@ad AND Parola=@parola", con);
            cmd.Parameters.AddWithValue("@ad", kullaniciAdi);
            cmd.Parameters.AddWithValue("@parola", parola);

            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                girisYapanKullanici = dr["KullaniciAdi"].ToString();
                kullaniciGorev = dr["Gorev"].ToString();

                MessageBox.Show("Giriş başarılı. Hoş geldiniz " + girisYapanKullanici);

                this.Hide();
                Form6 form6 = new Form6(girisYapanKullanici,kullaniciGorev);
                form6.Show();
                // ARA YÖNLENDİRME YAPAYIM DEDİM 
            }
            else
            {
                MessageBox.Show("Kullanıcı adı veya parola hatalı!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            // temizlesin ya garip duruyor 
            textBox1.Clear();
            textBox2.Clear();

            con.Close();
            
        }
    }
}

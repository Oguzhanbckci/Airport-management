using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Havalimani_x
{
    public partial class Form5 : Form
    {
        string girisYapanKullanici;
        string kullaniciGorev;
        SqlConnection baglanti = new SqlConnection("Server=.;Database=HavalimaniX2;Integrated Security=true;");

        public Form5(string kullanıcı, string gorev)
        {
            InitializeComponent();
            girisYapanKullanici = kullanıcı;
            kullaniciGorev = gorev;
        }

        private void Form5_Load(object sender, EventArgs e)
        {
            HavalimanlariGetir();
            ViewYolcuUcusBilgisiGoster();
            UcusSayisiChart();
        }

        // *== Yardımcı Fonksiyonlarım  aç kapa ==*
        void BaglantiyiAc()
        {
            if (baglanti.State != ConnectionState.Open)
                baglanti.Open();
        }

        void BaglantiyiKapat()
        {
            if (baglanti.State != ConnectionState.Closed)
                baglanti.Close();
        }

        DataTable VeriGetir(string sorgu)
        {
            DataTable dt = new DataTable();
            BaglantiyiAc();
            SqlDataAdapter da = new SqlDataAdapter(sorgu, baglanti);
            da.Fill(dt);
            BaglantiyiKapat();
            return dt;
        }

        DataTable ProcedureGetir(string spAdi, SqlParameter[] parametreler = null)
        {
            DataTable dt = new DataTable();
            BaglantiyiAc();
            SqlCommand komut = new SqlCommand(spAdi, baglanti);
            komut.CommandType = CommandType.StoredProcedure;
            if (parametreler != null)
                komut.Parameters.AddRange(parametreler);
            SqlDataAdapter da = new SqlDataAdapter(komut);
            da.Fill(dt);
            BaglantiyiKapat();
            return dt;
        }

        // *==  Verileri Getiren Fonksiyonlar ==*

        void HavalimanlariGetir()
        {
            string sorgu = "SELECT HavalimaniID, Ad FROM Havalimanlari";
            DataTable dt = VeriGetir(sorgu);
            comboBox1.DataSource = dt;
            comboBox1.DisplayMember = "Ad";
            comboBox1.ValueMember = "HavalimaniID";
        }

        void ViewYolcuUcusBilgisiGoster()
        {
            dataGridView1.DataSource = VeriGetir("SELECT * FROM Vw_YolcuUcusBilgisi");
        }

        void UcusSayisiChart()
        {
            string sorgu = @"
                SELECT h.Ad, COUNT(u.UcusID) AS UcusSayisi
                FROM Havalimanlari h
                LEFT JOIN Ucuslar u ON h.HavalimaniID = u.KalkisHavalimaniID
                GROUP BY h.Ad";

            BaglantiyiAc();
            SqlCommand komut = new SqlCommand(sorgu, baglanti);
            SqlDataReader dr = komut.ExecuteReader();

            chart1.Series.Clear();
            Series seri = chart1.Series.Add("Uçuş Sayısı");
            seri.ChartType = SeriesChartType.Column;

            while (dr.Read())
            {
                seri.Points.AddXY(dr["Ad"].ToString(), Convert.ToInt32(dr["UcusSayisi"]));
            }

            dr.Close(); // DataReader kapatmak önemli
            BaglantiyiKapat();
        }

        // === Olaylar ===

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedValue != null && int.TryParse(comboBox1.SelectedValue.ToString(), out int secilenID))
            {
                SqlParameter[] parametreler = {
            new SqlParameter("@KalkisHavalimaniID", secilenID)
        };
                dataGridView1.DataSource = ProcedureGetir("sp_UcuslariListele", parametreler);
            }
        }

        private void button1_Click(object sender, EventArgs e) // Uçuşlar
        {
            string sorgu = @"
                SELECT 
                    u.UcusNo,
                    hu.Ad AS KalkisHavalimani,
                    hv.Ad AS VarisHavalimani,
                    u.KalkisZamani,
                    u.VarisZamani,
                    u.Durum
                FROM Ucuslar u
                JOIN Havalimanlari hu ON u.KalkisHavalimaniID = hu.HavalimaniID
                JOIN Havalimanlari hv ON u.VarisHavalimaniID = hv.HavalimaniID";

            dataGridView1.DataSource = VeriGetir(sorgu);
        }

        private void button2_Click(object sender, EventArgs e) // Yolcu
        {
            ViewYolcuUcusBilgisiGoster();
        }

        private void button3_Click(object sender, EventArgs e) // Yolcu Log
        {
            dataGridView1.DataSource = VeriGetir("SELECT * FROM YolcuSilmeLog");
        }

        private void button4_Click(object sender, EventArgs e) // Toplam Bilet
        {
            string sorgu = "SELECT COUNT(*) FROM Biletler";
            BaglantiyiAc();
            SqlCommand komut = new SqlCommand(sorgu, baglanti);
            int toplamBilet = (int)komut.ExecuteScalar();
            BaglantiyiKapat();

            chart1.Series.Clear();
            Series seri = chart1.Series.Add("Toplam Bilet");
            seri.Points.AddXY("Toplam Bilet", toplamBilet);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form6 form6 = new Form6(girisYapanKullanici, kullaniciGorev);
            form6.Show();
        }
    }
}

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
    public partial class Form6 : Form
    {
        // ara yönlendirme sayfam 

        string girisYapanKullanici;
        string kullaniciGorev;
        SqlConnection con = new SqlConnection(@"Server=.;Database=HavalimaniX2;Trusted_Connection=True;");
        public Form6(string kullanıcı,string gorev)
        {
            InitializeComponent();
            girisYapanKullanici=kullanıcı;
            kullaniciGorev=gorev;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form2 form2 = new Form2(girisYapanKullanici, kullaniciGorev);
            form2.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form3 form3 = new Form3(girisYapanKullanici, kullaniciGorev);
            form3.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form4 form4 = new Form4(girisYapanKullanici, kullaniciGorev);
            form4.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form5 form5 = new Form5(girisYapanKullanici, kullaniciGorev);
            form5.Show();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form1 form1 = new Form1();
            form1.Show();
        }

        private void Form6_Load(object sender, EventArgs e)
        {
            if (kullaniciGorev == "Yönetici")
            {
                // tüm butonlara erişim açık
            }
            else if (kullaniciGorev == "Uçuş Personeli")
            {
                button5.Enabled = true;   
                button2.Enabled = false;  
                button3.Enabled = false; 
                button4.Enabled = false;  
            }
            else if (kullaniciGorev == "Destek")
            {
                button5.Enabled = false;
                button2.Enabled = true;
                button3.Enabled = false;
                button4.Enabled = true;
            }
            else
            {
                // Bilinmeyen görevse her şeyi kapatsın 

                button5.Enabled = false;
                button2.Enabled = false;
                button3.Enabled = false;
                button4.Enabled = false;
            }
        }
    }
}


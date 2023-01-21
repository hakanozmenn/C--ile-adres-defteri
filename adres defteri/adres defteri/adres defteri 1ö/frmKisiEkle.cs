using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;

namespace adres_defteri_1ö
{
    public partial class frmKisiEkle : Form
    {
        public frmKisiEkle()
        {
            InitializeComponent();
        }

        OleDbConnection baglanti;
        OleDbCommand komut;
        OleDbDataAdapter daGrup;
        DataSet dsGrup;


        void grupDoldur()
        {
            string sqlGrup = "SELECT * FROM Grup";
            daGrup = new OleDbDataAdapter(sqlGrup, baglanti);
            dsGrup = new DataSet();
            baglanti.Open();
            daGrup.Fill(dsGrup, "Grup");
            baglanti.Close();
            //ComboBox doluyor
            cmbGruplar.DataSource = dsGrup.Tables["Grup"];
            cmbGruplar.DisplayMember = "GrupAdi";
            cmbGruplar.ValueMember = "IDGrup";
            cmbGruplar.SelectedIndex = -1;
            //ListBox doluyor
            lstbGruplar.DataSource = dsGrup.Tables["Grup"];
            lstbGruplar.DisplayMember = "GrupAdi";
            lstbGruplar.ValueMember = "IDGrup";
            lstbGruplar.SelectedIndex = -1;
        }

        private void frmKisiEkle_Load(object sender, EventArgs e)
        {
            tabYeniKayitGrup.TabPages.Remove(tab2YeniGrup);
            baglanti = new OleDbConnection(Form1.yol);
            grupDoldur();
        }

        private void txtGrupEkleCikar_Click(object sender, EventArgs e)
        {
            tabYeniKayitGrup.TabPages.Add(tab2YeniGrup);
            tabYeniKayitGrup.TabPages.Remove(tab1YeniKayit);
        }

        private void btnKisiEkleCikar_Click(object sender, EventArgs e)
        {
            tabYeniKayitGrup.TabPages.Add(tab1YeniKayit);
            tabYeniKayitGrup.TabPages.Remove(tab2YeniGrup);
        }

        private void btnGeriDon_Click(object sender, EventArgs e)
        {
            Form1 frm1 = new Form1();
            frm1.Show();
            Close();
        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            if (txtAd.Text=="" || txtAdres.Text==""||txtemail.Text==""||txtSoyad.Text==""||txtTelefon.Text==""|| cmbGruplar.Text=="")
            {
                txtAd.Focus();
            }
            else
            {
                int IDGrup = Convert.ToInt32(cmbGruplar.SelectedValue);
                string sqlYeniKayit = "INSERT INTO Kisi(IDGrup, ad, soyad, telefon, email, adres) VALUES (@IDGrup, @ad1, @soyad1, @telefon1, @email1, @adres1)";
                komut = new OleDbCommand(sqlYeniKayit, baglanti);
                baglanti.Open();
                komut.Parameters.Add("@IDGrup", OleDbType.Integer).Value = IDGrup;
                komut.Parameters.Add("@ad1", OleDbType.Char, 50).Value = txtAd.Text;
                komut.Parameters.Add("@soyad1", OleDbType.Char, 50).Value = txtSoyad.Text;
                komut.Parameters.Add("@telefon1", OleDbType.Char, 50).Value = txtTelefon.Text;
                komut.Parameters.Add("@email1", OleDbType.Char, 50).Value = txtemail.Text;
                komut.Parameters.Add("@adres1", OleDbType.Char, 255).Value = txtAdres.Text;
                komut.ExecuteNonQuery();
                baglanti.Close();
                timer1.Start();
                MessageBox.Show("KAYIT YAPILDI", "DİKKAT!");
                temizle();
            } 
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            //timer1.Enabled = false;
            SendKeys.Send("{ESC}");
        }

        void temizle()
        {
            cmbGruplar.Text = "";
            txtAd.Text = "";
            txtAdres.Clear();
            txtemail.Text = null;
            txtSoyad.Text = "";
            txtTelefon.Text = "";
        }

        private void btnGrupKaydet_Click(object sender, EventArgs e)
        {
            if (txtYeniGrupAdi.Text=="")
            {
                txtYeniGrupAdi.Focus();
            }
            else
            {
                string sqlYeniGrup = "INSERT INTO Grup (GrupAdi) VALUES (@GrupAdi1)";
                komut = new OleDbCommand(sqlYeniGrup, baglanti);
                baglanti.Open();
                komut.Parameters.Add("@GrupAdi1", OleDbType.Char, 50).Value = txtYeniGrupAdi.Text;
                komut.ExecuteNonQuery();
                baglanti.Close();
                timer2.Enabled = true;
                MessageBox.Show("KAYIT YAPILDI", "DİKKAT!");
                grupDoldur();
                txtYeniGrupAdi.Text = "";
                txtYeniGrupAdi.Focus();
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            timer2.Stop();
            SendKeys.Send("{ESC}");
        }

        private void btnGrupSil_Click(object sender, EventArgs e)
        {
            try
            {
                string sqlGrupSil = "DELETE FROM Grup WHERE IDGrup="+lstbGruplar.SelectedValue;
                komut = new OleDbCommand(sqlGrupSil, baglanti);
                DialogResult cevap = MessageBox.Show("Bir kaydı silmek üzeresiniz...", "DİKKAT!",MessageBoxButtons.OKCancel);
                if (cevap==DialogResult.OK)
                {
                    baglanti.Open();
                    komut.ExecuteNonQuery();
                    baglanti.Close();
                    grupDoldur();
                }
            }
            catch 
            {
                baglanti.Close();
            }
        }

        private void lstbGruplar_DoubleClick(object sender, EventArgs e)
        {
            btnGrupSil_Click(sender, e);
        }
    }
}

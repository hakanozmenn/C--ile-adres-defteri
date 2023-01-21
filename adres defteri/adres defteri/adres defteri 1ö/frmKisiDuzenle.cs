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
    public partial class frmKisiDuzenle : Form
    {
        public frmKisiDuzenle()
        {
            InitializeComponent();
        }

        OleDbConnection baglanti;
        OleDbCommand komut;
        OleDbDataAdapter daKisi, daGrup;
        DataSet dsKisi, dsGrup;

        int Kull_ID, grupID;

        private void frmKisiDuzenle_Load(object sender, EventArgs e)
        {
            baglanti = new OleDbConnection(Form1.yol);
            grupDoldur();
            Kull_ID = Form1.kullaniciID;
            kisiBilgileriniGetir();
        }

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
        }

        void kisiBilgileriniGetir()
        {
            string sqlKisiBilgileri = "SELECT * FROM Kisi WHERE IDKisi=" + Kull_ID;
            daKisi = new OleDbDataAdapter(sqlKisiBilgileri, baglanti);
            dsKisi = new DataSet();
            baglanti.Open();
            daKisi.Fill(dsKisi, "Kisi");
            baglanti.Close();

            lblID.Text = Kull_ID.ToString();
            txtAd.Text = dsKisi.Tables[0].Rows[0]["ad"].ToString();
            txtSoyad.Text = dsKisi.Tables[0].Rows[0]["soyad"].ToString();
            txtTelefon.Text = dsKisi.Tables[0].Rows[0]["telefon"].ToString();
            txtemail.Text = dsKisi.Tables[0].Rows[0]["email"].ToString();
            txtAdres.Text = dsKisi.Tables[0].Rows[0]["adres"].ToString();
            grupID = Convert.ToInt32(dsKisi.Tables[0].Rows[0]["IDGrup"].ToString());
            cmbGruplar.SelectedValue = grupID;
        }

        private void btnGeriDon_Click(object sender, EventArgs e)
        {
            Form1 frm1 = new Form1();
            frm1.Show();
            this.Close();
        }

        void temizle()
        {
            cmbGruplar.Text = "";
            txtAd.Text = "";
            txtAdres.Clear();
            txtemail.Text = null;
            txtSoyad.Text = "";
            txtTelefon.Text = "";
            lblID.Text = "";
        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            if (txtAd.Text == "" || txtAdres.Text == "" || txtemail.Text == "" || txtSoyad.Text == "" || txtTelefon.Text == "" || cmbGruplar.Text == "")
            {
                txtAd.Focus();
            }
            else
            {
                int IDGrup = Convert.ToInt32(cmbGruplar.SelectedValue);
                string sqlGuncelle = "UPDATE Kisi SET IDGrup=@IDGrup, ad=@ad1, soyad=@soyad1, telefon=@telefon1, email=@email1, adres=@adres1 WHERE IDKisi=" + Kull_ID;
                komut = new OleDbCommand(sqlGuncelle, baglanti);
                baglanti.Open();
                komut.Parameters.Add("@IDGrup", OleDbType.Integer).Value = IDGrup;
                komut.Parameters.Add("@ad1", OleDbType.Char, 50).Value = txtAd.Text;
                komut.Parameters.Add("@soyad1", OleDbType.Char, 50).Value = txtSoyad.Text;
                komut.Parameters.Add("@telefon1", OleDbType.Char, 50).Value = txtTelefon.Text;
                komut.Parameters.Add("@email1", OleDbType.Char, 50).Value = txtemail.Text;
                komut.Parameters.Add("@adres1", OleDbType.Char, 255).Value = txtAdres.Text;
                DialogResult cevap = MessageBox.Show("KAYIT GÜNCELLENİYOR...", "DİKKAT!", MessageBoxButtons.OKCancel);
                if (cevap==DialogResult.OK)
                {
                    komut.ExecuteNonQuery();
                    baglanti.Close();
                    temizle();
                }
                else
                {
                    baglanti.Close();
                }
            }
        }
    }
}

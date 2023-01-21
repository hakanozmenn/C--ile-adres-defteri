using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;//Access için ekle
using System.Drawing.Printing;//yazdırma işlemi için eklendi

namespace adres_defteri_1ö
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        OleDbConnection baglanti;
        OleDbCommand komut;
        OleDbDataAdapter daKisi, daGrup;
        DataSet dsKisi, dsGrup;

        public static string yol;
        public static int kullaniciID;

        int grupID;//Combobox'ta görüntülenen üyenin index numarası için

        void gridDoldur(string sqlMetni)
        {
            daKisi = new OleDbDataAdapter(sqlMetni, baglanti);
            dsKisi = new DataSet();
            baglanti.Open();
            daKisi.Fill(dsKisi, "Kisi");
            baglanti.Close();
            gridKisiler.DataSource = dsKisi.Tables["Kisi"];
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            yol = "Provider=Microsoft.jet.oledb.4.0; data source=adresdefteri.mdb";
            baglanti = new OleDbConnection(yol);
            grupDoldur();
            //string sqlKayitlar = "SELECT * FROM Kisi";
            //gridDoldur(sqlKayitlar);

            gridDoldur("SELECT * FROM Kisi");
        }

        void grupDoldur()
        {
            string sqlGrup = "SELECT * FROM Grup";
            daGrup = new OleDbDataAdapter(sqlGrup, baglanti);
            dsGrup = new DataSet();
            baglanti.Open();
            daGrup.Fill(dsGrup, "Grup");
            baglanti.Close();

            cmbGruplar.DataSource = dsGrup.Tables["Grup"];
            cmbGruplar.DisplayMember = "GrupAdi";
            cmbGruplar.ValueMember = "IDGrup";
            cmbGruplar.SelectedIndex = -1;
        }

        int seciliSatir;
        private void gridKisiler_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            seciliSatir = e.RowIndex;
            lblID.Text = dsKisi.Tables[0].Rows[seciliSatir]["IDKisi"].ToString();
            lblSoyad.Text = dsKisi.Tables[0].Rows[seciliSatir]["soyad"].ToString();
            lblAd.Text = dsKisi.Tables[0].Rows[seciliSatir]["ad"].ToString();
            lblAdres.Text = dsKisi.Tables[0].Rows[seciliSatir]["adres"].ToString();
            lbleMail.Text = dsKisi.Tables[0].Rows[seciliSatir]["email"].ToString();
            lblTelefon.Text = dsKisi.Tables[0].Rows[seciliSatir]["telefon"].ToString();

            int geciciGrupID = Convert.ToInt32(dsKisi.Tables[0].Rows[seciliSatir]["IDGrup"]);
            lblGrup.Text = grupAdi(geciciGrupID);
            kullaniciID = Convert.ToInt32(lblID.Text);//kişiye ait kayıt silinirken bu kullanılacak
        }

        public string grupAdi(int geciciGrupID)
        {
            string sqlGrupAdi = "SELECT GrupAdi FROM Grup WHERE IDGrup=" + geciciGrupID;
            daGrup = new OleDbDataAdapter(sqlGrupAdi, baglanti);
            dsGrup = new DataSet();
            baglanti.Open();
            daGrup.Fill(dsGrup, "Grup");
            baglanti.Close();
            string grupAdi = dsGrup.Tables[0].Rows[0]["GrupAdi"].ToString();
            return grupAdi;
            //return dsGrup.Tables[0].Rows[0]["GrupAdi"].ToString();
        }

        private void txtAranan_TextChanged(object sender, EventArgs e)
        {
            string sqlAranan = "SELECT * FROM Kisi WHERE ad LIKE '" + txtAranan.Text + "%'";
            gridDoldur(sqlAranan);
        }

        private void cmbGruplar_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                grupID = Convert.ToInt32(cmbGruplar.SelectedValue);
                string sqlGrupNo = "SELECT * FROM Kisi WHERE IDGrup=" + grupID;
                gridDoldur(sqlGrupNo);
            }
            catch
            {
                //bir şey yapma 
            }
        }

        private void btnSil_Click(object sender, EventArgs e)
        {
            DialogResult cevap = MessageBox.Show(kullaniciID + " Nolu Kayıt Silinecek!", "DİKKAT!", MessageBoxButtons.OKCancel);
            if (cevap == DialogResult.OK)
            {
                string sqlSilinicekKayit = "DELETE FROM Kisi WHERE IDKisi=" + kullaniciID;
                komut = new OleDbCommand(sqlSilinicekKayit, baglanti);
                baglanti.Open();
                komut.ExecuteNonQuery();
                baglanti.Close();

                gridDoldur("SELECT * FROM Kisi");
            }
        }

        private void btnCikis_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            frmKisiEkle frmEkle = new frmKisiEkle();
            frmEkle.Show();
            this.Visible = false;
        }

        private void btnGuncelle_Click(object sender, EventArgs e)
        {
            frmKisiDuzenle frmDuzenle = new frmKisiDuzenle();
            frmDuzenle.Show();
            this.Visible = false;
        }

        private void seçiliKaydıSilToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnSil_Click(sender, e);
        }

        private void seçiliKaydıDüzenleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnGuncelle_Click(sender, e);
        }

        private void gridKisiler_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var hti = gridKisiler.HitTest(e.X, e.Y);
                gridKisiler.ClearSelection();
                gridKisiler.Rows[hti.RowIndex].Selected = true;
                kullaniciID = Convert.ToInt32(gridKisiler.Rows[hti.RowIndex].Cells[0].Value);
            }
        }

        #region YAZDIRMA İŞLEMLERİ

        int i = 0;//satırları yazdırıkem gerekli
        private void printDocument1_PrintPage(object sender, PrintPageEventArgs e)
        {
            PageSettings sayfa = printDocument1.DefaultPageSettings;
            e.Graphics.DrawLine(new Pen(Color.Black, 1), sayfa.Margins.Left - 75, 100, sayfa.PaperSize.Width - sayfa.Margins.Right + 75, 100);
            Font baslik = new Font("Arial", 10, FontStyle.Bold);
            int y = 135, satir = gridKisiler.Rows.Count;
            e.Graphics.DrawString("S.NO", baslik, Brushes.Black, 25, 100);
            e.Graphics.DrawString("AD", baslik, Brushes.Black, 60, 100);
            e.Graphics.DrawString("SOYAD", baslik, Brushes.Black, 160, 100);
            e.Graphics.DrawString("TELEFON", baslik, Brushes.Black, 260, 100);
            e.Graphics.DrawString("EMAİL", baslik, Brushes.Black, 335, 100);
            e.Graphics.DrawString("ADRES", baslik, Brushes.Black, 485, 100);
            e.Graphics.DrawLine(new Pen(Color.Black, 1), sayfa.Margins.Left - 75, 120, sayfa.PaperSize.Width - sayfa.Margins.Right + 75, 120);
            Font altBaslik = new Font("Arial", 8, FontStyle.Bold);
           
            while (i<satir)
            {
                e.Graphics.DrawString((i+1).ToString(), altBaslik, Brushes.Black, 25, y);
                e.Graphics.DrawString(gridKisiler.Rows[i].Cells[2].Value.ToString(), altBaslik, Brushes.Black, 60, y);
                e.Graphics.DrawString(gridKisiler.Rows[i].Cells[3].Value.ToString(), altBaslik, Brushes.Black, 160, y);
                e.Graphics.DrawString(gridKisiler.Rows[i].Cells[4].Value.ToString(), altBaslik, Brushes.Black, 260, y);
                e.Graphics.DrawString(gridKisiler.Rows[i].Cells[5].Value.ToString(), altBaslik, Brushes.Black, 335, y);
                e.Graphics.DrawString(gridKisiler.Rows[i].Cells[6].Value.ToString(), altBaslik, Brushes.Black, 485, y);
                i++; y += 25;
                //Eğer yazdırılacak satırlar birden fazla sayfaya yazılacaksa aşağıdaki kod ile bu sağlanır
                if (y + 135 > sayfa.PaperSize.Height + 80 - sayfa.Margins.Bottom + 80)
                {
                    e.HasMorePages = true;
                    break;
                }
            }
            if (i>=satir)
            {
                e.HasMorePages = false;
                i = 0;
            }
        }

        private void btnBaskiOnizleme_Click(object sender, EventArgs e)
        {
            printPreviewDialog1.Document = printDocument1;
            printPreviewDialog1.ShowDialog();
        }

        private void btnYaz_Click(object sender, EventArgs e)
        {
            printDocument1.Print();
        }        
        
        private void btnSayfaYapisi_Click(object sender, EventArgs e)
        {
            pageSetupDialog1.PageSettings = printDocument1.DefaultPageSettings;
            if (pageSetupDialog1.ShowDialog()==DialogResult.OK)
            {
                printDocument1.DefaultPageSettings = pageSetupDialog1.PageSettings;
            }
        }

        #endregion








    }
}

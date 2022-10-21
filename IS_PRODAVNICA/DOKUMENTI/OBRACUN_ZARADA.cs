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

namespace IS_PRODAVNICA.DOKUMENTI
{
    public partial class OBRACUN_ZARADA : Form
    {
        SqlConnection cn;
        SqlCommand cm = new SqlCommand();
        DBConection dbcon = new DBConection();
        SqlDataReader dr;

        //Javne promenljive
        int radnik_id, zarada_vrsta_id;
        string ugovorene_mesecna_zarada_radnika;
        double stimulacija, destimulacija, pom_ugovorena_mesecna_zarada, ukupna_mesecna_zarada;
        
        public OBRACUN_ZARADA()
        {
            InitializeComponent();
            //Povezivanje sa Bazom u trenutku kreiranja forme.
            cn = new SqlConnection(dbcon.MyConection());
            Ucitaj_Radnike();
            Ucitaj_Vrsta_Zarade();
        }

        //Ucitavanje zarada radnika

        public void Ucitaj_Zarade_Radnika()
        {
            int i = 0;
            dataGrid_Obracun_Zarada_Radnika.Rows.Clear();
            cn.Open();
            cm = new SqlCommand(" select r.id, r.ime_prezime, r.JMBG, ob.period_od, ob.period_do, vz.vrsta_zarade, " +
                                " ob.ukupna_zarada, ob.napomena " +
                                " from OBRACUN_ZARADA as ob " +
                                " left outer join RADNIK as r on ob.id_radnika = r.id " +
                                " left outer join VRSTA_ZARADE as vz on ob.id_vrsta_zarade = vz.id ", cn);
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                i += 1;
                dataGrid_Obracun_Zarada_Radnika.Rows.Add(i, dr["id"].ToString(), dr["ime_prezime"].ToString(), dr["JMBG"].ToString(),
                                             dr["vrsta_zarade"].ToString(), dr["period_od"].ToString(), dr["period_do"].ToString(),
                                             dr["ukupna_zarada"].ToString(), dr["napomena"].ToString());
            }
            dr.Close();
            cn.Close();

            if (dataGrid_Obracun_Zarada_Radnika.Rows.Count != 0 && dataGrid_Obracun_Zarada_Radnika.Rows.Count != 1)
            {
                //Selekcija posledljeg reda i celije po redu, u za u gridu
                int Indeks_Reda = dataGrid_Obracun_Zarada_Radnika.Rows.Count - 1;
                int Indeks_Kolone = dataGrid_Obracun_Zarada_Radnika.Columns.Count - 1;

                dataGrid_Obracun_Zarada_Radnika.Rows[Indeks_Reda].Selected = true;
                dataGrid_Obracun_Zarada_Radnika.Rows[Indeks_Reda].Cells[Indeks_Kolone].Selected = true;

                //In case if you want to scroll down as well.
                dataGrid_Obracun_Zarada_Radnika.FirstDisplayedScrollingRowIndex = Indeks_Reda;
            }
        }
        private void textDestimulacija_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Enter) || (e.KeyCode == Keys.Tab))
            {
                textUkupna_Zarada.Focus(); //Kontrola sa fokusom

                stimulacija = Double.Parse(textStmulacija.Text.ToString());

                destimulacija = Double.Parse(textDestimulacija.Text.ToString());
                ukupna_mesecna_zarada = pom_ugovorena_mesecna_zarada + stimulacija - destimulacija;
                textUkupna_Zarada.Text = ukupna_mesecna_zarada.ToString();

                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }
        //Ucitavanje zarada radnika
        public void Ucitaj_Sve_Zarade_Radnika()
        {

        }

        public void Unesi_Zaradu_Radnika()
        {
            try
            {
                
                if (textUkupna_Zarada.Text != "")
                {
                    // Treba uneti ogranicenja vezano za unos po gradu i mestu
                    cn.Open();
                    cm = new SqlCommand(" INSERT INTO OBRACUN_ZARADA ( id_radnika, id_vrsta_zarade, period_od, period_do, ukupna_zarada, napomena ) " +
                                        " VALUES (@id_radnika, @id_vrsta_zarade, @period_od, @period_do, @ukupna_zarada, @napomena) ", cn);
                    cm.Parameters.AddWithValue("@id_radnika", radnik_id);
                    cm.Parameters.AddWithValue("@id_vrsta_zarade", zarada_vrsta_id);
                    cm.Parameters.AddWithValue("@period_od", datePeriodDo.Value);
                    cm.Parameters.AddWithValue("@period_do", datePeriodOd.Value);
                    cm.Parameters.AddWithValue("@ukupna_zarada", ukupna_mesecna_zarada);
                    cm.Parameters.AddWithValue("@napomena", textNapomena_Zarada.Text);
                    cm.ExecuteNonQuery();

                    MessageBox.Show("Uspešno ste sačuvali zaradu radnika", "Unos Radnika", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    textUgovorena_Mesecna_Zarada.Clear();
                    textStmulacija.Clear();
                    textDestimulacija.Clear();
                    textUkupna_Zarada.Clear();
                    cmbIme_Prezime_Radnika.SelectedIndex = 0;
                    cmbVrsta_Zarade.SelectedIndex = 0;
                    textNapomena_Zarada.Clear();
                    cn.Close();
                }
                else
                {
                    MessageBox.Show("Niste uneli sve propratne podatke", " Upozorenje !!!",
                                         MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textUkupna_Zarada.Focus();

                }
                //Ucitavanje artikala da bi se odmah video efekat unosa
                Ucitaj_Sve_Zarade_Radnika();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void btnNovi_Zarada_Radnika_Click(object sender, EventArgs e)
        {
            Unesi_Zaradu_Radnika();
        }

        private void btnUcitaj_Zaradu_Radnika_Click(object sender, EventArgs e)
        {
            Ucitaj_Zarade_Radnika();
        }

        private void cmbVrsta_Zarade_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var id_vrste_zarade = cmbVrsta_Zarade.SelectedValue;
            zarada_vrsta_id = (int)id_vrste_zarade;
            
            // Proba selekcija Id_Vrste_Zarade
            // textStmulacija.Text= zarada_vrsta_id.ToString();
        }

        //Ucitavanje vrste zarade
        public void Ucitaj_Vrsta_Zarade()
        {
            //Selekcija Vrste Zarade
            cn.Open();

            cm = new SqlCommand(" SELECT id, vrsta_zarade " +
                                " FROM VRSTA_ZARADE ", cn);
            cm.ExecuteNonQuery();
            dr = null;
            dr = cm.ExecuteReader();
            DataTable data_pp = new DataTable();
            data_pp.Load(dr);

            //Unos reda -Select PP- prije pozivanja ostalih redova
            
            DataRow Selelct_Osnovni_PP = data_pp.NewRow();
            Selelct_Osnovni_PP[0] = 0;
            Selelct_Osnovni_PP[1] = "- Select Vrsta zarade - ";
            data_pp.Rows.InsertAt(Selelct_Osnovni_PP, 0);
            

            cmbVrsta_Zarade.DataSource = data_pp;
            
            //Poziv ostalih redova u DataSourse;
            cmbVrsta_Zarade.DisplayMember = "vrsta_zarade";
            cmbVrsta_Zarade.ValueMember = "id";

            cmbVrsta_Zarade.AutoCompleteMode = AutoCompleteMode.Suggest;
            cmbVrsta_Zarade.AutoCompleteSource = AutoCompleteSource.ListItems;

            dr.Close();
            cn.Close();

        }

        //Ucitavanje zaposljenih radnika
        public void Ucitaj_Radnike()
        {
            //Selekcija Radnika
            cn.Open();

            cm = new SqlCommand(" SELECT id, ime_prezime " +
                                " FROM RADNIK ", cn);
            cm.ExecuteNonQuery();
            dr = null;
            dr = cm.ExecuteReader();
            DataTable data_pp = new DataTable();
            data_pp.Load(dr);

            //Unos reda -Select PP- prije pozivanja ostalih redova
            
            DataRow Selelct_Osnovni_PP = data_pp.NewRow();
            Selelct_Osnovni_PP[0] = 0;
            Selelct_Osnovni_PP[1] = "- Select Radnika - ";
            data_pp.Rows.InsertAt(Selelct_Osnovni_PP, 0);
            
            cmbIme_Prezime_Radnika.DataSource = data_pp;
            

            //Poziv ostalih redova u DataSourse;
            cmbIme_Prezime_Radnika.DisplayMember = "ime_prezime";
            cmbIme_Prezime_Radnika.ValueMember = "id";

            cmbIme_Prezime_Radnika.AutoCompleteMode = AutoCompleteMode.Suggest;
            cmbIme_Prezime_Radnika.AutoCompleteSource = AutoCompleteSource.ListItems;

            dr.Close();
            cn.Close();
        }

        private void cmbIme_Prezime_Radnika_SelectionChangeCommitted(object sender, EventArgs e)
        {
            cn.Open();

            var id_radnika = cmbIme_Prezime_Radnika.SelectedValue;
            radnik_id = (int)id_radnika;

            cm = new SqlCommand(" SELECT id, mesecna_zarada  FROM RADNIK " +
                                " WHERE id=" + radnik_id, cn);

            cm.ExecuteNonQuery();
            dr = null;
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                //Dodeljivanje poreske stope oznacenom artiklu

                ugovorene_mesecna_zarada_radnika = dr["mesecna_zarada"].ToString();
                textUgovorena_Mesecna_Zarada.Text = ugovorene_mesecna_zarada_radnika;
                pom_ugovorena_mesecna_zarada =Double.Parse(ugovorene_mesecna_zarada_radnika);
            }
            dr.Close();
            cn.Close();
        }
    }
}

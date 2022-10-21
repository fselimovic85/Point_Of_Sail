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
    public partial class OSTALI_TROSKOVI : Form
    {

        SqlConnection cn;
        SqlCommand cm = new SqlCommand();
        DBConection dbcon = new DBConection();
        SqlDataReader dr;

        //Javne promenljive
        string vrsta_dokumenta = "Ostali-Troskovi";
        int id_zaglavlja_dokumenta;
        double vrednost_troska_sa_PDV;

        public OSTALI_TROSKOVI()
        {
            InitializeComponent();

            //Povezivanje sa Bazom u trenutku kreiranja forme.
            cn = new SqlConnection(dbcon.MyConection());

            //Dodavanje vrsta PP-a u ComboBox()
            cmbNacin_Placanja_Trosak.Items.Add("VIRMAN");
            cmbNacin_Placanja_Trosak.Items.Add("PLATNA KARTICA");
            cmbNacin_Placanja_Trosak.Items.Add("GOTOVINA");

            //Selektovanje prve prednosti
            cmbNacin_Placanja_Trosak.SelectedIndex = 0;


            //Dodavanje Vrste troška u ComboBox()
            cmbVrsta_Troska.Items.Add("TELEFON - FIKSNI");
            cmbVrsta_Troska.Items.Add("TELEFON - MOB");
            cmbVrsta_Troska.Items.Add("OSTALE PTT- USLUGE");
            cmbVrsta_Troska.Items.Add("ELEKTRIČNA ENERGIJA");
            cmbVrsta_Troska.Items.Add("USLUGE PREVOZA");
            cmbVrsta_Troska.Items.Add("USLUGE IZNOSENJA KOMUNALNOG OTPADA");
            cmbVrsta_Troska.Items.Add("ZAKUP POSLOVNOG PROSTORA - KIRIJA");
            cmbVrsta_Troska.Items.Add("TROSKOVI ZA ODRZAVANJE VOZILA");

            //Selektovanje prve prednosti
            cmbVrsta_Troska.SelectedIndex = 0;

            //Ucitavanje inicijalnih vrednosti za Poslovne Partnere
            Ucitaj_Poslovne_Partnere();
        }

        public void Ucitaj_Zaglavlje()
        {
            int i = 0;
            dataGrid_Zaglavlje_Trosak.Rows.Clear();
            cn.Open();
            cm = new SqlCommand(" Select zd.id as id, zd.broj_dokumenta as broj_dokumenta, zd.nacin_placanja as nacin_placanja, " +
                                         " pp.naziv_pp as naziv_pp_a, zd.datum_dokumenta as datum_dokumenta, zd.napomena as napomena   " +
                                " from ZAGLAVLJE_DOKUMENTA as zd" +
                                " Left outer join POSLOVNI_PARTNER as pp ON zd.id_poslovnog_partnera=pp.id " +
                                " Where zd.vrsta_dokumenta='Ostali-Troskovi' " +
                                " order by zd.id", cn);
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                i += 1;
                dataGrid_Zaglavlje_Trosak.Rows.Add(i, dr["id"].ToString(), dr["broj_dokumenta"].ToString(),
                                             dr["nacin_placanja"].ToString(), dr["naziv_pp_a"].ToString(),
                                             dr["datum_dokumenta"].ToString(), dr["napomena"].ToString());
            }
            dr.Close();
            cn.Close();

            if (dataGrid_Zaglavlje_Trosak.Rows.Count != 0 && dataGrid_Zaglavlje_Trosak.Rows.Count != 1)
            {
                //Selekcija posledljeg reda i celije po redu, u za u gridu
                int Indeks_Reda = dataGrid_Zaglavlje_Trosak.Rows.Count - 1;
                int Indeks_Kolone = dataGrid_Zaglavlje_Trosak.Columns.Count - 1;

                dataGrid_Zaglavlje_Trosak.Rows[Indeks_Reda].Selected = true;
                dataGrid_Zaglavlje_Trosak.Rows[Indeks_Reda].Cells[Indeks_Kolone].Selected = true;

                //In case if you want to scroll down as well.
                dataGrid_Zaglavlje_Trosak.FirstDisplayedScrollingRowIndex = Indeks_Reda;
            }

        }

        //Ucitavanje Poslovnih Partnera
        public void Ucitaj_Poslovne_Partnere()
        {
            //Selekcija Psolovnog Partnera
            cn.Open();

            cm = new SqlCommand(" SELECT id, naziv_pp " +
                                " FROM POSLOVNI_PARTNER ", cn);
            cm.ExecuteNonQuery();
            dr = null;
            dr = cm.ExecuteReader();
            DataTable data_pp = new DataTable();
            data_pp.Load(dr);

            //Unos reda -Select PP- prije pozivanja ostalih redova
            /*
            DataRow Selelct_Osnovni_PP = data_pp.NewRow();
            Selelct_Osnovni_PP[0] = 0;
            Selelct_Osnovni_PP[1] = "- Select PP - ";
            data_pp.Rows.InsertAt(Selelct_Osnovni_PP, 0);
            */
            cmbNazivPP_Trosak.DataSource = data_pp;

            //Poziv ostalih redova u DataSourse;
            cmbNazivPP_Trosak.DisplayMember = "naziv_pp";
            cmbNazivPP_Trosak.ValueMember = "id";

            cmbNazivPP_Trosak.AutoCompleteMode = AutoCompleteMode.Suggest;
            cmbNazivPP_Trosak.AutoCompleteSource = AutoCompleteSource.ListItems;

            dr.Close();
            cn.Close();
        }
        // Unos podataka zaglavlja za Troškove
        public void Unesi_Zaglavlje()
        {
            try
            {
                cn.Open();
                cm = new SqlCommand(" SELECT * " +
                                    " FROM ZAGLAVLJE_DOKUMENTA" +
                                    " WHERE  broj_dokumenta='" + textBrojDokumenta_Trosak.Text + "' AND vrsta_dokumenta=" +
                                    "'Ostali-Troskovi'", cn);
                cm.ExecuteNonQuery();
                dr = null;
                dr = cm.ExecuteReader();
                while (dr.Read())
                {
                    if (textBrojDokumenta_Trosak.Text == dr["broj_dokumenta"].ToString())
                    {
                        MessageBox.Show(" Postoji već Vrsta Troška sa datim brojem dokumenta ", " Upozorenje !!!",
                                          MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        dr.Close();
                        cn.Close();
                        textBrojDokumenta_Trosak.Clear();
                        textBrojDokumenta_Trosak.Focus();
                        break;
                    }
                }
                dr.Close();
                cn.Close();

                if (textBrojDokumenta_Trosak.Text != "")
                {
                    // Treba uneti ogranicenja vezano za unos po gradu i mestu
                    cn.Open();
                    cm = new SqlCommand(" INSERT INTO ZAGLAVLJE_DOKUMENTA ( broj_dokumenta, nacin_placanja, id_poslovnog_partnera, datum_dokumenta, napomena, vrsta_dokumenta ) " +
                                        " VALUES (@broj_dokumenta, @nacin_placanja, @id_poslovnog_partnera, @datum_dokumenta, @napomena, @vrsta_dokumenta) ", cn);
                    cm.Parameters.AddWithValue("@broj_dokumenta", textBrojDokumenta_Trosak.Text);
                    cm.Parameters.AddWithValue("@nacin_placanja", cmbNacin_Placanja_Trosak.SelectedItem);
                    cm.Parameters.AddWithValue("@id_poslovnog_partnera", cmbNazivPP_Trosak.SelectedValue);
                    cm.Parameters.AddWithValue("@datum_dokumenta", dtDatumDokumenta_Trosak.Value);
                    cm.Parameters.AddWithValue("@napomena", textNapomena_Trosak.Text);
                    cm.Parameters.AddWithValue("@vrsta_dokumenta", vrsta_dokumenta);
                    cm.ExecuteNonQuery();

                    MessageBox.Show("Uspešno ste sačuvali zaglavlje dokumenta", "Unos Zaglavlja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    textBrojDokumenta_Trosak.Clear();
                    cmbNacin_Placanja_Trosak.SelectedIndex = 0;
                    cmbNazivPP_Trosak.SelectedIndex = 0;
                    textNapomena_Trosak.Clear();
                    cn.Close();
                }
                else
                {
                    MessageBox.Show("Niste uneli sve propratne podatke", " Upozorenje !!!",
                                         MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBrojDokumenta_Trosak.Focus();

                }
                //Ucitavanje artikala da bi se odmah video efekat unosa
                Ucitaj_Zaglavlje();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        //Unos detalja Dokumenta Vrste troska
        private void Unesi_Detalje_Dokumenta()
        {
            try
            {
                if (textVrednost_Troska_sa_PDV_om.Text != "")
                {

                    vrednost_troska_sa_PDV = Math.Round(Double.Parse(textVrednost_Troska_sa_PDV_om.Text.ToString()), 2);

                    cn.Open();
                    cm = new SqlCommand(" INSERT INTO DETALJI_DOKUMENTA ( id_zaglavlja, vstra_troska, vrednost_troska_pdv ) " +
                                        " VALUES ( @id_zaglavlja, @vstra_troska, @vrednost_troska_pdv ) ", cn);
                    cm.Parameters.AddWithValue("@id_zaglavlja", id_zaglavlja_dokumenta);                   
                    cm.Parameters.AddWithValue("@vstra_troska", cmbVrsta_Troska.SelectedItem);
                    cm.Parameters.AddWithValue("@vrednost_troska_pdv", vrednost_troska_sa_PDV);

                    cm.ExecuteNonQuery();

                    MessageBox.Show("Uspešno ste sačuvali red detalja dokumenta", "Unos Detalja", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    cmbVrsta_Troska.SelectedIndex = 0;
                    textVrednost_Troska_sa_PDV_om.Clear();
                   

                    cn.Close();
                }
                else
                {
                    MessageBox.Show("Niste uneli sve propratne podatke", " Upozorenje !!!",
                                         MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cmbVrsta_Troska.Focus();

                }
                //Ucitavanje detalja da bi se odmah video efekat unosa
                Ucitaj_Detalje_Dokumenta();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Ucitaj_Detalje_Dokumenta()
        {
            int i = 0;
            dataGrid_Detalji_Trosak.Rows.Clear();
            cn.Open();
            cm = new SqlCommand(" SELECT det_dok.id, det_dok.vstra_troska, det_dok.vrednost_troska_pdv " +
                                " FROM DETALJI_DOKUMENTA det_dok" +
                                " LEFT OUTER JOIN ZAGLAVLJE_DOKUMENTA zag_dok ON det_dok.id_zaglavlja=zag_dok.id " +
                                " WHERE det_dok.id_zaglavlja=" + id_zaglavlja_dokumenta + "" +
                                " ORDER BY det_dok.id ", cn);
            cm.ExecuteNonQuery();
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                i += 1;
                dataGrid_Detalji_Trosak.Rows.Add(i, dr["id"].ToString(), dr["vstra_troska"].ToString(),
                                             dr["vrednost_troska_pdv"].ToString());
            }
            dr.Close();
            cn.Close();

            if (dataGrid_Detalji_Trosak.Rows.Count != 0 && dataGrid_Detalji_Trosak.Rows.Count != 1)
            {
                //Selekcija posledljeg reda i celije po redu, u za u gridu
                int Indeks_Reda = dataGrid_Detalji_Trosak.Rows.Count - 1;
                int Indeks_Kolone = dataGrid_Detalji_Trosak.Columns.Count - 1;

                dataGrid_Detalji_Trosak.Rows[Indeks_Reda].Selected = true;
                dataGrid_Detalji_Trosak.Rows[Indeks_Reda].Cells[Indeks_Kolone].Selected = true;

                //In case if you want to scroll down as well.
                dataGrid_Detalji_Trosak.FirstDisplayedScrollingRowIndex = Indeks_Reda;
            }
        }

        private void btnUcitaj_Zaglavnje_Trosak_Click(object sender, EventArgs e)
        {
            Ucitaj_Zaglavlje();
        }

        private void btnDodaj_Zaglavlje_Trosak_Click(object sender, EventArgs e)
        {
            Unesi_Zaglavlje();
        }

        private void btnDodaj_Detalje_Trosak_Click(object sender, EventArgs e)
        {
            Unesi_Detalje_Dokumenta();
            cmbVrsta_Troska.Focus();
        }

        private void dataGrid_Zaglavlje_Trosak_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            id_zaglavlja_dokumenta = Int32.Parse(dataGrid_Zaglavlje_Trosak.Rows[e.RowIndex].Cells[1].Value.ToString());
            Ucitaj_Detalje_Dokumenta();
        }
    }
}

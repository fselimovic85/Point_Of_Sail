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

namespace IS_PRODAVNICA.IZVESTAJI
{
    public partial class EVIDENCIJA_PRODAJE_TROSKOVA : Form
    {
        SqlConnection cn;
        SqlCommand cm = new SqlCommand();
        DBConection dbcon = new DBConection();
        SqlDataReader dr;

        // Globalne promenljive
        double ukupno_zarade_od_prodaje, ukupno_ostalih_troskova, 
               ukupno_troskova_od_zarade, bilans_uspeha;
        public EVIDENCIJA_PRODAJE_TROSKOVA()
        {
            InitializeComponent();
            cn = new SqlConnection(dbcon.MyConection());
        }

        /* Evidencija zarade od prodaje */
        public void Evidencija_Zarade_od_Prodaje()
        {

            int i = 0; ukupno_zarade_od_prodaje = 0;
            dataGrid_Ukupna_Zarada_od_Prodaje.Rows.Clear();
            cn.Open();
            cm = new SqlCommand(" Select DISTINCT d.id_zaglavlja,  z.broj_dokumenta, z.datum_dokumenta," +
                                " ROUND(SUM(d.vred_marze_sa_rabatom) over(partition by d.id_zaglavlja),2) as zarada, z.napomena " +
                                " From ZAGLAVLJE_DOKUMENTA z " +
                                " Left outer join DETALJI_DOKUMENTA d on z.ID = d.id_zaglavlja "+
                                " Where z.vrsta_dokumenta = 'Izlaz-Racun' and z.datum_dokumenta >='"+dtDatumOd.Value.ToString("yyyy-MM-dd")+"' and z.datum_dokumenta <='"+ dtDatumDo.Value.ToString("yyyy-MM-dd")+"'" +
                                " group by d.id_zaglavlja, z.broj_dokumenta, z.datum_dokumenta, d.vred_marze_sa_rabatom, z.napomena ", cn);
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                i += 1;
               
                dataGrid_Ukupna_Zarada_od_Prodaje.Rows.Add(i, dr["id_zaglavlja"].ToString(), dr["broj_dokumenta"].ToString(),
                                             dr["datum_dokumenta"].ToString(), (dr["zarada"].ToString()),
                                             dr["napomena"].ToString());
                ukupno_zarade_od_prodaje +=Double.Parse(dr["zarada"].ToString());
            }
            dr.Close();
            cn.Close();

            texSuma_Zarade.Text = ukupno_zarade_od_prodaje.ToString();

            if (dataGrid_Ukupna_Zarada_od_Prodaje.Rows.Count != 0 && dataGrid_Ukupna_Zarada_od_Prodaje.Rows.Count != 1)
            {
                //Selekcija posledljeg reda i celije po redu, u za u gridu
                int Indeks_Reda = dataGrid_Ukupna_Zarada_od_Prodaje.Rows.Count - 1;
                int Indeks_Kolone = dataGrid_Ukupna_Zarada_od_Prodaje.Columns.Count - 1;

                dataGrid_Ukupna_Zarada_od_Prodaje.Rows[Indeks_Reda].Selected = true;
                dataGrid_Ukupna_Zarada_od_Prodaje.Rows[Indeks_Reda].Cells[Indeks_Kolone].Selected = true;

                //In case if you want to scroll down as well.
                dataGrid_Ukupna_Zarada_od_Prodaje.FirstDisplayedScrollingRowIndex = Indeks_Reda;
            }


        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        /* Evidencija ukupnuh ostalih troskova */
        public void Evidencija_Ostalih_Troskova()
        {

            int i = 0; ukupno_ostalih_troskova= 0;
            dataGrid_Ukupno_Ostali_Troskovi.Rows.Clear();
            cn.Open();
            cm = new SqlCommand(" Select DISTINCT d.id_zaglavlja,  z.broj_dokumenta, z.datum_dokumenta," +
                                " ROUND(SUM(d.vrednost_troska_pdv) over(partition by d.id_zaglavlja),2) as ostali_troskovi, z.napomena " +
                                " From ZAGLAVLJE_DOKUMENTA z " +
                                " Left outer join DETALJI_DOKUMENTA d on z.ID = d.id_zaglavlja " +
                                " Where z.vrsta_dokumenta = 'Ostali-Troskovi' and z.datum_dokumenta >='" + dtDatumOd.Value.ToString("yyyy-MM-dd") + "' and z.datum_dokumenta <='" + dtDatumDo.Value.ToString("yyyy-MM-dd") + "'" +
                                " group by d.id_zaglavlja, z.broj_dokumenta, z.datum_dokumenta, d.vrednost_troska_pdv, z.napomena ", cn);
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                i += 1;

                dataGrid_Ukupno_Ostali_Troskovi.Rows.Add(i, dr["id_zaglavlja"].ToString(), dr["broj_dokumenta"].ToString(),
                                             dr["datum_dokumenta"].ToString(), dr["ostali_troskovi"].ToString(),
                                             dr["napomena"].ToString());
                ukupno_ostalih_troskova += Double.Parse(dr["ostali_troskovi"].ToString());
            }
            dr.Close();
            cn.Close();

            txtSuma_Ostalih_Troskova.Text = ukupno_ostalih_troskova.ToString();

            if (dataGrid_Ukupno_Ostali_Troskovi.Rows.Count != 0 && dataGrid_Ukupno_Ostali_Troskovi.Rows.Count != 1)
            {
                //Selekcija posledljeg reda i celije po redu, u za u gridu
                int Indeks_Reda = dataGrid_Ukupno_Ostali_Troskovi.Rows.Count - 1;
                int Indeks_Kolone = dataGrid_Ukupno_Ostali_Troskovi.Columns.Count - 1;

                dataGrid_Ukupno_Ostali_Troskovi.Rows[Indeks_Reda].Selected = true;
                dataGrid_Ukupno_Ostali_Troskovi.Rows[Indeks_Reda].Cells[Indeks_Kolone].Selected = true;

                //In case if you want to scroll down as well.
                dataGrid_Ukupno_Ostali_Troskovi.FirstDisplayedScrollingRowIndex = Indeks_Reda;
            }

        }

        /* Evidencija zarada radnika za posmtarani period */

        public void Evidencija_Troskova_Zarada_Radnika()
        {

            int i = 0; ukupno_troskova_od_zarade = 0;
            dataGrid_Ukupno_Zarade_Radnika.Rows.Clear();
            cn.Open();
            cm = new SqlCommand(" select oz.id, r.ime_prezime, oz.period_od, oz.period_do, oz.ukupna_zarada, oz.napomena "+
                                " from OBRACUN_ZARADA as oz "+
                                " left outer join RADNIK as r on oz.id_radnika = r.id" +
                                " Where oz.period_od >='" + dtDatumOd.Value.ToString("yyyy-MM-dd") + "' and oz.period_do <='" + dtDatumDo.Value.ToString("yyyy-MM-dd") + "'", cn);
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                i += 1;

                dataGrid_Ukupno_Zarade_Radnika.Rows.Add(i, dr["id"].ToString(), dr["ime_prezime"].ToString(), dr["period_od"].ToString(),
                                             dr["period_do"].ToString(), dr["ukupna_zarada"].ToString(),
                                             dr["napomena"].ToString());
                ukupno_troskova_od_zarade += Double.Parse(dr["ukupna_zarada"].ToString());
            }
            dr.Close();
            cn.Close();

            txtSuma_Troskova_Zarada.Text = ukupno_troskova_od_zarade.ToString();

            if (dataGrid_Ukupno_Zarade_Radnika.Rows.Count != 0 && dataGrid_Ukupno_Zarade_Radnika.Rows.Count != 1)
            {
                //Selekcija posledljeg reda i celije po redu, u za u gridu
                int Indeks_Reda = dataGrid_Ukupno_Zarade_Radnika.Rows.Count - 1;
                int Indeks_Kolone = dataGrid_Ukupno_Zarade_Radnika.Columns.Count - 1;

                dataGrid_Ukupno_Zarade_Radnika.Rows[Indeks_Reda].Selected = true;
                dataGrid_Ukupno_Zarade_Radnika.Rows[Indeks_Reda].Cells[Indeks_Kolone].Selected = true;

                //In case if you want to scroll down as well.
                dataGrid_Ukupno_Zarade_Radnika.FirstDisplayedScrollingRowIndex = Indeks_Reda;
            }

        }

        private void btnBilans_Uspeha_Click(object sender, EventArgs e)
        {
            /* Brisanje svih prethodno definisanih troskova*/
            txtSuma_Ostalih_Troskova.Clear();
            texSuma_Zarade.Clear();
            txtSuma_Troskova_Zarada.Clear();
            textBilans_Uspeha.Clear();
           
            // Poziv metoda za pregled i racunanje podatka
            Evidencija_Zarade_od_Prodaje();
            Evidencija_Ostalih_Troskova();
            Evidencija_Troskova_Zarada_Radnika();

            bilans_uspeha = Math.Round(ukupno_zarade_od_prodaje - (ukupno_ostalih_troskova + ukupno_troskova_od_zarade),2);
            
            
            if(bilans_uspeha<0)
            {
                Ispis_Poruke.ForeColor = Color.Red;
                Ispis_Poruke.Text = "Bilans uspeha je negativan";
            }
            else
            {
                Ispis_Poruke.ForeColor = Color.Green;
                Ispis_Poruke.Text = "Bilans uspeha je pozitivan";
            }
            textBilans_Uspeha.Text = bilans_uspeha.ToString();
            
        }
    }
}

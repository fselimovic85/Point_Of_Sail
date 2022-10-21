using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IS_PRODAVNICA.DOKUMENTI;
using IS_PRODAVNICA.MATICNI_PODACI;
using IS_PRODAVNICA.IZVESTAJI;

namespace IS_PRODAVNICA
{
    public partial class GLAVNA_FORMA : Form
    {
        public GLAVNA_FORMA()
        {
            InitializeComponent();
        }

        private void isplateRadnikaToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void radniciToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RADNICI frmRadnik = new RADNICI();
            frmRadnik.Show();
        }

        private void geografskiPodaciToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GEOGRAFSKI_PODACI frmGeo_Podaci = new GEOGRAFSKI_PODACI();
            frmGeo_Podaci.Show();
        }

        private void poslovniPartneriToolStripMenuItem_Click(object sender, EventArgs e)
        {
            POSLOVNI_PARTNERI frmPP = new POSLOVNI_PARTNERI();
            frmPP.Show();
        }

        private void artikliToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ARTIKLI frmArtikli = new ARTIKLI();
            frmArtikli.Show();
        }

        private void prijemnicaRobeKalkulacijaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ULAZ_KALKULACIJA frmKalkulcija = new ULAZ_KALKULACIJA();
            frmKalkulcija.Show();
        }

        private void računOtpremnicaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IZLAZNI_RACUN frmIzlazni_Racun = new IZLAZNI_RACUN();
            frmIzlazni_Racun.Show();
        }

        private void karticaArtikalaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            KARTICA_ARTIKA frmKartica_Artikla = new KARTICA_ARTIKA();
            frmKartica_Artikla.Show();
        }

        private void ostaliTroškoviToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OSTALI_TROSKOVI frmTrosak = new OSTALI_TROSKOVI();
            frmTrosak.Show();
        }

        private void isplateRadnicimaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OBRACUN_ZARADA frmZarade = new OBRACUN_ZARADA();
            frmZarade.Show();
        }

        private void evidencijaProdajeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EVIDENCIJA_PRODAJE_TROSKOVA frmEvidencija = new EVIDENCIJA_PRODAJE_TROSKOVA();
            frmEvidencija.Show();
        }
    }
}

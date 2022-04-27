using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Projekt2_Naumenko51447
{
    public partial class AnalizatorAlgorytmówSortowania : Form
    {
        const int vnMargines = 20;

        int vnPróbaBadawcza = 100;
        int vnMaxRozmiarTabl = 50;
        double vnDolnaGranicaWartości = 20.0;
        double vnGórnaGranicaWartości = 300000.0;

        double[] vnTabl;
        double[] vnWynikiZpomiaru;
        double[] vnWynikiAnalityczne;
        double[] vnKosztPamięczowy;
        int[] vnTablicaLOD;

        struct vnStyl
        {
            public int vnGrubośćLiniiWykresu;
            public Color vnKolorLinii;
            public Color vnKolorTłaDlaObszaruWykresu;
            public ChartDashStyle vnChartDashStyle;

            public vnStyl(Color vnLinia, Color vnTło, int vnGrubość, ChartDashStyle vnChartDashStyle)
            {
                vnGrubośćLiniiWykresu = vnGrubość;
                vnKolorLinii = vnLinia;
                vnKolorTłaDlaObszaruWykresu = vnTło;
                this.vnChartDashStyle = vnChartDashStyle;
            }
        }

        static vnStyl vnStylStandart = new vnStyl(SystemColors.ControlLightLight, SystemColors.ControlLightLight, 1, ChartDashStyle.Dash);
        vnStyl vnStylUżytkownik = vnStylStandart;

        public AnalizatorAlgorytmówSortowania()
        {
            InitializeComponent();

            vnResetuj();
        }

        // Class
        class vnSortowanie<vnT> where vnT : IComparable
        {
            // SelectSort
            public int vnSelectSort(ref vnT[] vnTabl, int n)
            {
                int vnK;
                vnT vnKopiaElementuDoWyniany;
                int vnLicznikOD = 0;
                for (int vnI = 0; vnI < n - 1; vnI++)
                {
                    vnK = vnI;
                    for (int vnJ = vnI + 1; vnJ < n; vnJ++)
                    {
                        vnLicznikOD++;
                        if (vnTabl[vnK].CompareTo(vnTabl[vnJ]) > 0)
                        {
                            vnK = vnJ;
                        }
                    }
                    vnKopiaElementuDoWyniany = vnTabl[vnI];
                    vnTabl[vnI] = vnTabl[vnK];
                    vnTabl[vnK] = vnKopiaElementuDoWyniany;
                }
                return vnLicznikOD;
            }

            // InsertionSort
            public int vnInsertionSort(ref vnT[] vnTabl, int vnN)
            {
                vnT vnKopiaElementuDoWyniany;
                int vnLicznikOD = 0;
                int vnK;
                for (int vnI = 1; vnI < vnN; vnI++)
                {
                    vnKopiaElementuDoWyniany = vnTabl[vnI];
                    vnK = vnI;

                    while ((vnK > 0) && (vnKopiaElementuDoWyniany.CompareTo(vnTabl[vnK - 1]) < 0))
                    {
                        vnLicznikOD++;
                        vnTabl[vnK] = vnTabl[vnK - 1];
                        vnK--;
                    }
                    if (vnK != 0) { vnLicznikOD++; }
                    vnTabl[vnK] = vnKopiaElementuDoWyniany;
                }
                return vnLicznikOD;
            }

            // ShellSort
            public int vnShellSort(ref vnT[] vnTabl, int vnN)
            {
                int vnLicznikOD = 0;
                for (int h = vnN / 2; h > 0; h /= 2)
                {
                    for (int i = h; i < vnN; i += 1)
                    {
                        vnLicznikOD++;
                        vnT vnTemp = vnTabl[i];
                        int j;
                        for (j = i; j >= h && (vnTabl[j - h].CompareTo(vnTemp) > 0); j -= h)
                        {
                            vnTabl[j] = vnTabl[j - h];
                        }
                        vnTabl[j] = vnTemp;
                    }
                }
                return vnLicznikOD;
            }

            // CombSort
            static int vnGetNextGap(int vnGap)
            {
                vnGap = (vnGap * 10) / 13;
                if (vnGap < 1)
                {
                    return 1;
                }
                return vnGap;
            }

            public int vnCombSort(ref vnT[] vnTabl, int vnN)
            {
                int vnGap = vnN;
                vnT vnKopiaElementuDoWyniany;
                bool vnSwapped = true;
                int vnLicznikOD = 0;
                while (vnGap != 1 || vnSwapped == true)
                {
                    vnGap = vnGetNextGap(vnGap);

                    vnSwapped = false;

                    for (int i = 0; i < vnN - vnGap; i++)
                    {
                        vnLicznikOD++;
                        if (vnTabl[i].CompareTo(vnTabl[i + vnGap]) > 0)
                        {
                            vnKopiaElementuDoWyniany = vnTabl[i];
                            vnTabl[i] = vnTabl[i + vnGap];
                            vnTabl[i + vnGap] = vnKopiaElementuDoWyniany;
                            vnSwapped = true;
                        }
                    }
                }
                return vnLicznikOD;
            }
        }

        // Metod

        private void vnResetuj()
        {
            // Styl
            vnLblWziernikWzorcaLinii.Text = "Wziernik wzorca linii";
            vnPnlWziernikWzorcaLinii.BackColor = SystemColors.ActiveCaption;
            vnLblUstalStylLiniiWykresu.Text = "Ustal styl linii wykresu";
            vnCmbUstalStylLiniiWykresu.SelectedIndex = 0;
            vnLblGrubośćLiniiWykresu.Text = "Grubość linii wykresu";
            vnTkbGrubośćLiniiWykresu.Value = vnStylStandart.vnGrubośćLiniiWykresu;
            vnBtnKolorLinii.Text = "Kolor linii";
            vnBtnKolorLinii.BackColor = vnStylStandart.vnKolorLinii;
            vnBtnKolorLinii.FlatStyle = FlatStyle.Flat;
            vnBtnKolorTłaDlaObszaruWykresu.Text = "Kolor tła dla obszaru wykresu";
            vnBtnKolorTłaDlaObszaruWykresu.BackColor = vnStylStandart.vnKolorTłaDlaObszaruWykresu;
            vnBtnKolorTłaDlaObszaruWykresu.FlatStyle = FlatStyle.Flat;
            vnBtnZaakceptujZmianęStylu.Text = "Zaakceptuj zmianę stylu";
            vnLblStanZmianyStylu.Text = "W porządku.";

            vnCmbWybierzAlgorytmuDoAnalizu.Items.Clear();
            vnCmbWybierzAlgorytmuDoAnalizu.Items.Add("SelectSort");
            vnCmbWybierzAlgorytmuDoAnalizu.Items.Add("InsertSort");
            vnCmbWybierzAlgorytmuDoAnalizu.Items.Add("ShellSort");
            vnCmbWybierzAlgorytmuDoAnalizu.Items.Add("CombSort");

            // Zmienne
            vnTabl = new double[vnMaxRozmiarTabl];
            vnKosztPamięczowy = new double[vnMaxRozmiarTabl];

            // Graficzna prezentacja złożoności algorytmów
            vnCrtWykresCzasawego.Titles.Clear();
            vnCrtWykresCzasawego.Legends.Clear();
            vnCrtWykresCzasawego.Series.Clear();

            // Tabeliczna prezentacja złożoności algorytów
            vnDGVTablica.Columns.Clear();
            vnDGVTablica.ReadOnly = true;

            // Panel
            vnPnlStyl.Visible = false;

            // Wartości
            vnTxbMinimalnaPróbaBadawcza.Text = vnPróbaBadawcza.ToString();
            vnTxbMaksymalnyRozmiarSortowanychTablicy.Text = vnMaxRozmiarTabl.ToString();
            vnTxbDolnaGranicaPrzedziałuWartościElementówSortowanychTablicy.Text = vnDolnaGranicaWartości.ToString();
            vnTxbGórnaGranicaPrzedziałuWartościElementówSortowanychTablicy.Text = vnGórnaGranicaWartości.ToString();
            vnCmbWybierzAlgorytmuDoAnalizu.SelectedIndex = 0;

            // Enables
            vnPnlWartości.Enabled = true;
            vnBtnResetuj.Enabled = false;
            vnBtnAkceptacjaDanychDlaBadaniaEksperymentalnego.Enabled = true;
            vnBtnWizualizacjaTablicyPrzedSortowaniem.Enabled = false;
            vnBtnWizualizacjaTablicyPoSortowaniu.Enabled = false;
            vnBtnTabelarycznaPrezentacjaZłożoności.Enabled = false;
            vnBtnGraficznaPrezentacjaZłożoności.Enabled = false;
            vnBtnStyl.Enabled = false;
            vnBtnDemo.Enabled = false;
            vnBtnResetuj.Enabled = false;
        }
        private void vnNarysowaćTablicyDwa()
        {
            vnDGVTablica.Columns.Clear();
            vnDGVTablica.ReadOnly = true;
            vnDGVTablica.Columns.Add("vnCmnIndex", "Index");
            vnDGVTablica.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            vnDGVTablica.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            vnDGVTablica.Columns.Add("vnCmnElement", "Element");
            vnDGVTablica.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            vnDGVTablica.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;

            for (int vnI = 0; vnI < vnMaxRozmiarTabl; vnI++)
            {
                vnDGVTablica.Rows.Add();
                vnDGVTablica.Rows[vnI].Cells[0].Value = vnI;
                vnDGVTablica.Rows[vnI].Cells[1].Value = String.Format("{0, 8:F3}", vnTabl[vnI]);
                if ((vnI % 2) == 0)
                {
                    vnDGVTablica.Rows[vnI].Cells[0].Style.BackColor = Color.LightGray;
                    vnDGVTablica.Rows[vnI].Cells[1].Style.BackColor = Color.LightGray;
                }
                else
                {
                    vnDGVTablica.Rows[vnI].Cells[0].Style.BackColor = Color.White;
                    vnDGVTablica.Rows[vnI].Cells[1].Style.BackColor = Color.White;
                }
                vnDGVTablica.Rows[vnI].Cells[0].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                vnDGVTablica.Rows[vnI].Cells[1].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }
        private void vnNarysowaćTablicyCztery()
        {
            vnDGVTablica.Columns.Clear();
            vnDGVTablica.ReadOnly = true;
            vnDGVTablica.Columns.Add("vnCmnRozmiarSortowanejTabeli", "Rozmiar sortowanej tabeli");
            vnDGVTablica.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            vnDGVTablica.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            vnDGVTablica.Columns.Add("vnCmnKosztCzasowyZpomiaru", "Koszt czasowy zpomiaru");
            vnDGVTablica.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            vnDGVTablica.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            vnDGVTablica.Columns.Add("vnCmnAnalitycznyKosztCzasowy", "Analityczny koszt czasowy");
            vnDGVTablica.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            vnDGVTablica.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            vnDGVTablica.Columns.Add("vnCmnKosztPamięciowy", "Koszt pamięciowy");
            vnDGVTablica.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            vnDGVTablica.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;

            for (int vnI = 0; vnI < vnMaxRozmiarTabl; vnI++)
            {
                vnDGVTablica.Rows.Add();
                vnDGVTablica.Rows[vnI].Cells[0].Value = vnI;
                vnDGVTablica.Rows[vnI].Cells[1].Value = String.Format("{0:F2}", vnWynikiZpomiaru[vnI]);
                vnDGVTablica.Rows[vnI].Cells[2].Value = String.Format("{0:F2}", vnWynikiAnalityczne[vnI]);
                vnDGVTablica.Rows[vnI].Cells[3].Value = String.Format("{0:F2}", vnKosztPamięczowy[vnI]);
                if (vnI % 2 == 0)
                {
                    vnDGVTablica.Rows[vnI].Cells[0].Style.BackColor = Color.LightGray;
                    vnDGVTablica.Rows[vnI].Cells[1].Style.BackColor = Color.LightGray;
                    vnDGVTablica.Rows[vnI].Cells[2].Style.BackColor = Color.LightGray;
                    vnDGVTablica.Rows[vnI].Cells[3].Style.BackColor = Color.LightGray;
                }
                else
                {
                    vnDGVTablica.Rows[vnI].Cells[0].Style.BackColor = Color.White;
                    vnDGVTablica.Rows[vnI].Cells[1].Style.BackColor = Color.White;
                    vnDGVTablica.Rows[vnI].Cells[2].Style.BackColor = Color.White;
                    vnDGVTablica.Rows[vnI].Cells[3].Style.BackColor = Color.White;
                }
                vnDGVTablica.Rows[vnI].Cells[0].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                vnDGVTablica.Rows[vnI].Cells[1].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                vnDGVTablica.Rows[vnI].Cells[2].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                vnDGVTablica.Rows[vnI].Cells[3].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }

        private bool vnZmianaDanych()
        {
            {
                // Próba badawcza
                if (!int.TryParse(vnTxbMinimalnaPróbaBadawcza.Text, out vnPróbaBadawcza) || vnPróbaBadawcza <= 0)
                {
                    if (vnPróbaBadawcza <= 0)
                        vnErrorProvider.SetError(vnTxbMinimalnaPróbaBadawcza, "UWAGA: wartość musi być większa od zera!");
                    else
                        vnErrorProvider.SetError(vnTxbMinimalnaPróbaBadawcza, "UWAGA: nie poprawnie wpisana wartość!");
                    return false;
                }
                else vnErrorProvider.Dispose();

                // Maksymalny Rozmiar Tablicy
                if (!int.TryParse(vnTxbMaksymalnyRozmiarSortowanychTablicy.Text, out vnMaxRozmiarTabl) || vnMaxRozmiarTabl <= 1)
                {
                    if (vnMaxRozmiarTabl <= 1)
                        vnErrorProvider.SetError(vnTxbMaksymalnyRozmiarSortowanychTablicy, "UWAGA: wartość musi być większa od jeden!");
                    else
                        vnErrorProvider.SetError(vnTxbMaksymalnyRozmiarSortowanychTablicy, "UWAGA: nie poprawnie wpisana wartość!");
                    return false;
                }
                else vnErrorProvider.Dispose();

                // Dolna granica przedziału wartości elementów sortowanych tablicy
                if (!double.TryParse(vnTxbDolnaGranicaPrzedziałuWartościElementówSortowanychTablicy.Text, out vnDolnaGranicaWartości))
                {
                    vnErrorProvider.SetError(vnTxbDolnaGranicaPrzedziałuWartościElementówSortowanychTablicy, "UWAGA: nie poprawnie wpisana wartość!");
                    return false;
                }
                else vnErrorProvider.Dispose();

                // Górna granica przedziału wartości elementów sortowanych tablicy
                if (!double.TryParse(vnTxbGórnaGranicaPrzedziałuWartościElementówSortowanychTablicy.Text, out vnGórnaGranicaWartości) || vnGórnaGranicaWartości <= vnDolnaGranicaWartości)
                {
                    if (vnGórnaGranicaWartości <= vnDolnaGranicaWartości)
                        vnErrorProvider.SetError(vnTxbGórnaGranicaPrzedziałuWartościElementówSortowanychTablicy, "UWAGA: górna granica musi być wieksza od dolnej!");
                    else
                        vnErrorProvider.SetError(vnTxbGórnaGranicaPrzedziałuWartościElementówSortowanychTablicy, "UWAGA: nie poprawnie wpisana wartość!");
                    return false;
                }
                else vnErrorProvider.Dispose();

                return true;
            }
        }

        // Event

        private void vnBtnWyjść_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void vnBtnResetuj_Click(object sender, EventArgs e)
        {
            vnResetuj();
        }

        private void vnBtnAkceptacjaDanychDlaBadaniaEksperymentalnego_Click(object sender, EventArgs e)
        {
            if (!vnZmianaDanych()) return;
            vnTabl = new double[vnMaxRozmiarTabl];
            vnWynikiZpomiaru = new double[vnMaxRozmiarTabl];
            vnWynikiAnalityczne = new double[vnMaxRozmiarTabl];
            vnTablicaLOD = new int[vnPróbaBadawcza];

            Random vnRnd = new Random();
            for (int i = 0; i < vnMaxRozmiarTabl; i++) { vnTabl[i] = (vnRnd.NextDouble() * (vnGórnaGranicaWartości - vnDolnaGranicaWartości) + vnDolnaGranicaWartości); }

            vnPnlWartości.Enabled = false;
            vnBtnAkceptacjaDanychDlaBadaniaEksperymentalnego.Enabled = false;
            vnBtnWizualizacjaTablicyPoSortowaniu.Enabled = true;
            vnBtnWizualizacjaTablicyPrzedSortowaniem.Enabled = true;
            vnBtnResetuj.Enabled = true;
        }

        private void vnBtnKolorLinii_Click(object sender, EventArgs e)
        {
            vnColorDialog.ShowDialog();
            vnStylUżytkownik.vnKolorLinii = vnColorDialog.Color;
            vnBtnKolorLinii.BackColor = vnStylUżytkownik.vnKolorLinii;
        }

        private void vnBtnKolorTłaDlaObszaruWykresu_Click(object sender, EventArgs e)
        {
            vnColorDialog.ShowDialog();
            vnStylUżytkownik.vnKolorTłaDlaObszaruWykresu = vnColorDialog.Color;
            vnBtnKolorTłaDlaObszaruWykresu.BackColor = vnStylUżytkownik.vnKolorTłaDlaObszaruWykresu;
        }

        private void vnBtnStyl_Changed(object sender, EventArgs e)
        {
            vnLblStanZmianyStylu.Text = "Styl został zmieniony. Kliknij \"Akceptacja\", aby zapisać zmiany.";
        }

        private void vnBtnWizualizacjaTablicyPoSortowaniu_Click(object sender, EventArgs e)
        {
            int vnLicznikOD;
            float vnSumaOD, vnŚredniaOD;

            vnSortowanie<double> vnAlgorytmySortowania = new vnSortowanie<double>();

            for (int vnL = 0; vnL < vnMaxRozmiarTabl; vnL++)
            {

                double[] vnTemp = vnTabl;
                vnTabl = new double[vnL];
                vnTabl = vnTemp;

                double[] vnTemp1 = vnKosztPamięczowy;
                vnKosztPamięczowy = new double[vnL];
                vnKosztPamięczowy = vnTemp1;
                for (int vnK = 0; vnK < vnPróbaBadawcza; vnK++)
                {
                    switch (vnCmbWybierzAlgorytmuDoAnalizu.SelectedIndex)
                    {
                        case 0: vnLicznikOD = vnAlgorytmySortowania.vnSelectSort(ref vnTabl, vnL); break;
                        case 1: vnLicznikOD = vnAlgorytmySortowania.vnInsertionSort(ref vnTabl, vnL); break;
                        case 2: vnLicznikOD = vnAlgorytmySortowania.vnShellSort(ref vnTabl, vnL); break;
                        case 3: vnLicznikOD = vnAlgorytmySortowania.vnCombSort(ref vnTabl, vnL); break;
                        default:
                            vnErrorProvider.SetError(vnBtnTabelarycznaPrezentacjaZłożoności,
                       "UWAGA: jeszcze tej metody nie opracowałem!");
                            return;
                    }
                    vnTablicaLOD[vnK] = vnLicznikOD;
                }

                vnSumaOD = 0.0F;
                for (int vnJ = 0; vnJ < vnPróbaBadawcza; vnJ++) { vnSumaOD = vnSumaOD + vnTablicaLOD[vnJ]; }
                vnŚredniaOD = vnSumaOD / vnPróbaBadawcza;
                vnWynikiZpomiaru[vnL] = vnŚredniaOD;
                switch (vnCmbWybierzAlgorytmuDoAnalizu.SelectedIndex)
                {
                    case 0: vnWynikiAnalityczne[vnL] = (vnL * (vnL - 1) / 2); break;
                    case 1: vnWynikiAnalityczne[vnL] = (double)Math.Pow(vnL, 2); break;
                    case 2: vnWynikiAnalityczne[vnL] = (double)vnL * Math.Log(vnL,2 ); break;
                    case 3: vnWynikiAnalityczne[vnL] = (double)vnL * Math.Log(vnL); break;
                    default: vnErrorProvider.SetError(vnBtnTabelarycznaPrezentacjaZłożoności,
                        "Uwaga: prace nad tym algorytmem, który został wybrany jeszcze trwają!");
                        return;
                }
                switch (vnCmbWybierzAlgorytmuDoAnalizu.SelectedIndex)
                {
                    case 0: vnKosztPamięczowy[vnL] = vnL; break;
                    case 1: vnKosztPamięczowy[vnL] = vnL; break;
                    case 2: vnKosztPamięczowy[vnL] = vnL; break;
                    case 3: vnKosztPamięczowy[vnL] = vnL; break;
                    default:
                        vnErrorProvider.SetError(vnBtnTabelarycznaPrezentacjaZłożoności,
                   "Uwaga: prace nad tym algorytmem, który został wybrany jeszcze trwają!");
                        return;
                }
            }
            vnNarysowaćTablicyDwa();
            MessageBox.Show("Przeprowadzono sortowanie algorytmem: " + vnCmbWybierzAlgorytmuDoAnalizu.SelectedItem);

            vnBtnWizualizacjaTablicyPoSortowaniu.Enabled = false;
            vnBtnWizualizacjaTablicyPrzedSortowaniem.Enabled = false;
            vnBtnGraficznaPrezentacjaZłożoności.Enabled = true;
            vnBtnTabelarycznaPrezentacjaZłożoności.Enabled = true;
        }

        private void vnBtnTabelarycznaPrezentacjaZłożoności_Click(object sender, EventArgs e)
        {
            vnNarysowaćTablicyCztery();
        }

        private void vnBtnWizualizacjaTablicyPrzedSortowaniem_Click(object sender, EventArgs e)
        {
            vnNarysowaćTablicyDwa();
            MessageBox.Show("Tablica przed sortowaniem");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            vnPnlStyl.Location = new Point(48, 45);
            vnPnlStyl.Visible = vnPnlStyl.Visible == true ? false : true;
        }

        private void vnBtnGraficznaPrezentacjaZłożoności_Click(object sender, EventArgs e)
        {
            vnCrtWykresCzasawego.Titles.Clear();
            vnCrtWykresCzasawego.Legends.Clear();
            vnCrtWykresCzasawego.Series.Clear();
            vnCrtWykresCzasawego.Titles.Add("Algorytm " + vnCmbWybierzAlgorytmuDoAnalizu.SelectedItem);
            vnCrtWykresCzasawego.BackColor = vnStylUżytkownik.vnKolorTłaDlaObszaruWykresu;
            vnCrtWykresCzasawego.Legends.Add("Legend1");
            vnCrtWykresCzasawego.Legends["Legend1"].Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Bottom;

            int[] vnRozmiarTabeli = new int[vnMaxRozmiarTabl];
            for (int vnI = 0; vnI < vnMaxRozmiarTabl; vnI++)
                vnRozmiarTabeli[vnI] = vnI;

            vnCrtWykresCzasawego.Series.Add("Koszt czasowy z pomiaru");
            vnCrtWykresCzasawego.Series[0].ChartType = SeriesChartType.Line;
            vnCrtWykresCzasawego.Series[0].Color = Color.Blue;
            vnCrtWykresCzasawego.Series[0].BorderDashStyle = ChartDashStyle.DashDot;
            vnCrtWykresCzasawego.Series[0].BorderWidth = 3;
            vnCrtWykresCzasawego.Series[0].Points.DataBindXY(vnRozmiarTabeli, vnWynikiZpomiaru);
            vnCrtWykresCzasawego.Series.Add("Koszt czasowy wg. wzoru");
            vnCrtWykresCzasawego.Series[1].ChartType = SeriesChartType.Line;
            vnCrtWykresCzasawego.Series[1].Color = Color.Black;
            vnCrtWykresCzasawego.Series[1].BorderDashStyle = ChartDashStyle.DashDot;
            vnCrtWykresCzasawego.Series[1].BorderWidth = 1;
            vnCrtWykresCzasawego.Series[1].Points.DataBindXY(vnRozmiarTabeli, vnWynikiAnalityczne);
            vnCrtWykresCzasawego.Series.Add("Koszt pamięci");
            vnCrtWykresCzasawego.Series[2].ChartType = SeriesChartType.Line;
            vnCrtWykresCzasawego.Series[2].Color = Color.Green;
            vnCrtWykresCzasawego.Series[2].BorderDashStyle = ChartDashStyle.DashDot;
            vnCrtWykresCzasawego.Series[2].BorderWidth = 1;
            vnCrtWykresCzasawego.Series[2].Points.DataBindXY(vnRozmiarTabeli, vnKosztPamięczowy);

            vnBtnStyl.Enabled = true;
        }

        private void vnBtnDemo_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Nistety, ale nie małem pomysłu...", "DEMO (Nie działa)", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void vnBtnZaakceptujZmianęStylu_Click(object sender, EventArgs e)
        {
            if (vnCmbSeriesStyl.SelectedIndex < 0)
            {
                vnErrorProvider.SetError(vnCmbSeriesStyl, "UWAGA: wybierz linii");
                return;
            }
            else vnErrorProvider.Dispose();

            vnLblStanZmianyStylu.Text = "W porządku.";
            vnCrtWykresCzasawego.BackColor = vnStylUżytkownik.vnKolorTłaDlaObszaruWykresu;
            switch (vnCmbSeriesStyl.SelectedIndex)
            {
                case 0:
                    vnCrtWykresCzasawego.Series[0].Color = vnStylUżytkownik.vnKolorLinii;
                    vnCrtWykresCzasawego.Series[0].BorderWidth = vnStylUżytkownik.vnGrubośćLiniiWykresu;
                    vnCrtWykresCzasawego.Series[0].BorderDashStyle = vnStylUżytkownik.vnChartDashStyle;
                    break;
                case 1:
                    vnCrtWykresCzasawego.Series[1].Color = vnStylUżytkownik.vnKolorLinii;
                    vnCrtWykresCzasawego.Series[1].BorderWidth = vnStylUżytkownik.vnGrubośćLiniiWykresu;
                    vnCrtWykresCzasawego.Series[1].BorderDashStyle = vnStylUżytkownik.vnChartDashStyle;
                    break;
                case 2:
                    vnCrtWykresCzasawego.Series[2].Color = vnStylUżytkownik.vnKolorLinii;
                    vnCrtWykresCzasawego.Series[2].BorderWidth = vnStylUżytkownik.vnGrubośćLiniiWykresu;
                    vnCrtWykresCzasawego.Series[2].BorderDashStyle = vnStylUżytkownik.vnChartDashStyle;
                    break;
            }
        }

        private void vnTkbGrubośćLiniiWykresu_Scroll(object sender, EventArgs e)
        {
            vnStylUżytkownik.vnGrubośćLiniiWykresu = vnTkbGrubośćLiniiWykresu.Value;
        }

        private void vnCmbUstalStylLiniiWykresu_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (vnCmbUstalStylLiniiWykresu.SelectedIndex)
            {
                case 0: vnStylUżytkownik.vnChartDashStyle = ChartDashStyle.Dash; break;
                case 1: vnStylUżytkownik.vnChartDashStyle = ChartDashStyle.DashDot; break;
                case 2: vnStylUżytkownik.vnChartDashStyle = ChartDashStyle.DashDotDot; break;
                case 3: vnStylUżytkownik.vnChartDashStyle = ChartDashStyle.Dot; break;
                case 4: vnStylUżytkownik.vnChartDashStyle = ChartDashStyle.Solid; break;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}

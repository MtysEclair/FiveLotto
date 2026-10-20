
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FiveLottoDesktop
{
    public partial class Form1 : Form
    {
        private CheckBox[] szamok = new CheckBox[90];
        private FlowLayoutPanel panelSzamok;


        public Form1()
        {
            InitializeComponent();
            AblakBeallitasa();
            SzamokLetrehozasa();
        }

        private void AblakBeallitasa()
        {
            this.Text = "Ötöslottó";
            this.ClientSize = new Size(780, 580);
            this.MinimumSize = new Size(800, 620);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(245, 247, 250);
            this.Font = new Font("Segoe UI", 10);

            btnSorsol.Text = "SORSOLÁS";
            btnSorsol.Size = new Size(150, 42);
            btnSorsol.Location = new Point(220, 505);
            btnSorsol.BackColor = Color.FromArgb(35, 145, 95);
            btnSorsol.ForeColor = Color.White;
            btnSorsol.FlatStyle = FlatStyle.Flat;
            btnSorsol.FlatAppearance.BorderSize = 0;
            btnSorsol.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnSorsol.Cursor = Cursors.Hand;
            btnSorsol.Enabled = false;
            btnSorsol.Anchor = AnchorStyles.Bottom;

            Button bezarGomb = this.Controls
                .OfType<Button>()
                .FirstOrDefault(b => b.Text == "Bezár");

            if (bezarGomb != null)
            {
                bezarGomb.Text = "BEZÁRÁS";
                bezarGomb.Size = new Size(150, 42);
                bezarGomb.Location = new Point(390, 505);
                bezarGomb.BackColor = Color.FromArgb(75, 85, 99);
                bezarGomb.ForeColor = Color.White;
                bezarGomb.FlatStyle = FlatStyle.Flat;
                bezarGomb.FlatAppearance.BorderSize = 0;
                bezarGomb.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                bezarGomb.Cursor = Cursors.Hand;
                bezarGomb.Anchor = AnchorStyles.Bottom;
            }

            lblEredmeny.Location = new Point(35, 445);
            lblEredmeny.Size = new Size(700, 45);
            lblEredmeny.AutoSize = false;
            lblEredmeny.TextAlign = ContentAlignment.MiddleCenter;
            lblEredmeny.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblEredmeny.ForeColor = Color.FromArgb(45, 55, 72);
            lblEredmeny.Anchor = AnchorStyles.Bottom
    | AnchorStyles.Left | AnchorStyles.Right;
        }

        private void SzamokLetrehozasa()
        {

            panelSzamok = new FlowLayoutPanel();
            panelSzamok.Location = new Point(35, 30);
            panelSzamok.Size = new Size(710, 400);
            panelSzamok.Anchor = AnchorStyles.Top | AnchorStyles.Bottom
                | AnchorStyles.Left | AnchorStyles.Right;
            panelSzamok.AutoScroll = true;
            panelSzamok.WrapContents = true;
            panelSzamok.FlowDirection = FlowDirection.LeftToRight;
            panelSzamok.Padding = new Padding(5);

            this.Controls.Add(panelSzamok);


            for (int i = 0; i < 90; i++)
            {
                CheckBox cb = new CheckBox();

                cb.Text = (i + 1).ToString();
                cb.Name = "chk" + (i + 1);

                cb.Width = 48;
                cb.Height = 36;
                cb.Appearance = Appearance.Button;
                cb.TextAlign = ContentAlignment.MiddleCenter;
                cb.FlatStyle = FlatStyle.Flat;
                cb.FlatAppearance.BorderColor = Color.FromArgb(210, 218, 228);
                cb.FlatAppearance.BorderSize = 1;
                cb.BackColor = Color.White;
                cb.ForeColor = Color.FromArgb(45, 55, 72);
                cb.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                cb.Cursor = Cursors.Hand;
                cb.CheckedChanged += SzamValtozott;

                szamok[i] = cb;
                panelSzamok.Controls.Add(cb);
            }
        }

        private void SzamValtozott(object sender, EventArgs e)
        {
            int kijelolt = szamok.Count(cb => cb.Checked);


            CheckBox aktualis = (CheckBox)sender;

            if (aktualis.Checked)
            {
                aktualis.BackColor = Color.FromArgb(35, 145, 95);
                aktualis.ForeColor = Color.White;
            }
            else
            {
                aktualis.BackColor = Color.White;
                aktualis.ForeColor = Color.FromArgb(45, 55, 72);
            }


            if (kijelolt > 5)
            {
                ((CheckBox)sender).Checked = false;
                MessageBox.Show("Legfeljebb 5 számot választhatsz!");
                return;
            }

            btnSorsol.Enabled = kijelolt == 5;

            lblEredmeny.Text = kijelolt == 5
                ? "Öt szám kiválasztva. Sorsolhatsz!"
                : $"Kiválasztott számok: {kijelolt}/5";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private void btnSorsol_Click(object sender, EventArgs e)
        {
            if (btnSorsol.Text == "ÚJ JÁTÉK")
            {
                foreach (CheckBox cb in szamok)
                {
                    cb.Checked = false;
                    cb.Enabled = true;
                }

                btnSorsol.Text = "SORSOLÁS";
                btnSorsol.Enabled = false;

                lblEredmeny.Text = "Kiválasztott számok: 0/5";
                return;
            }

            Random veletlen = new Random();

            List<int> huzottSzamok = Enumerable
                .Range(1, 90)
                .OrderBy(x => veletlen.Next())
                .Take(5)
                .OrderBy(x => x)
                .ToList();

            List<int> tippeltSzamok = szamok
                .Where(cb => cb.Checked)
                .Select(cb => int.Parse(cb.Text))
                .ToList();

            
            int talalatok = tippeltSzamok
                .Intersect(huzottSzamok)
                .Count();

  
            lblEredmeny.Text =
                "A te számaid: " + string.Join(", ", tippeltSzamok) +
                Environment.NewLine +
                "Kihúzott számok: " + string.Join(", ", huzottSzamok) +
                Environment.NewLine +
                "Találatok száma: " + talalatok;

            if (talalatok <= 2)
            {
                MessageBox.Show(
                    "Most nem nyertél, próbáld újra!",
                    "Ötöslottó",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            else if (talalatok < 5)
            {
                MessageBox.Show(
                    "Közel volt, legközelebb biztos sikerül!",
                    "Ötöslottó",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(
                    "Senkinek ne mondd el, de lesznek jelei! Gratulálok, nyertél!",
                    "Ötöslottó",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }

            foreach (CheckBox cb in szamok)
            {
                cb.Enabled = false;
            }

            btnSorsol.Text = "ÚJ JÁTÉK";
            btnSorsol.Enabled = true;
        }
    }
}

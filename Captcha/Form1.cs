using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Captcha
{
    public partial class Form1 : Form
    {
        private PictureBox[] pictureBoxes;
        private TextBox[] textBoxes;
        private Dictionary<PictureBox, TextBox> pictureBoxTextBoxPairs;
        private Point startingPoint;
        private Random random = new Random();
        private Dictionary<PictureBox, Point> lastPositions = new Dictionary<PictureBox, Point>();

        public Form1()
        {
            InitializeComponent();
            pictureBoxes = new PictureBox[] { pictureBox1, pictureBox2, pictureBox3, pictureBox4, pictureBox5, pictureBox6, pictureBox7, pictureBox8, pictureBox9 };
            textBoxes = new TextBox[] { textBox1, textBox2, textBox3, textBox4, textBox5, textBox6, textBox7, textBox8, textBox9 };

            pictureBoxTextBoxPairs = new Dictionary<PictureBox, TextBox>();
            for (int i = 0; i < pictureBoxes.Length; i++)
            {
                pictureBoxTextBoxPairs.Add(pictureBoxes[i], textBoxes[i]);
                pictureBoxes[i].AllowDrop = true;
                pictureBoxes[i].MouseDown += PictureBox_MouseDown;
                pictureBoxes[i].MouseMove += PictureBox_MouseMove;
                pictureBoxes[i].MouseUp += PictureBox_MouseUp;
                pictureBoxes[i].Tag = pictureBoxes[i].Left; // PictureBox'ın sol konumunu Tag olarak ayarlayın
            }
        }

        private void PictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            PictureBox pictureBox = (PictureBox)sender;
            startingPoint = e.Location;
            pictureBox.BringToFront();
        }

        private void PictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            PictureBox pictureBox = (PictureBox)sender;
            if (e.Button == MouseButtons.Left)
            {
                int deltaX = e.X - startingPoint.X;
                int deltaY = e.Y - startingPoint.Y;

                // Kaydırmadan önceki konumu kaydet
                if (!lastPositions.ContainsKey(pictureBox))
                {
                    lastPositions[pictureBox] = pictureBox.Location;
                }

                pictureBox.Left += deltaX;
                pictureBox.Top += deltaY;
            }
        }

        private void PictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            // Son sürüklenen PictureBox'ın konumunu kaydet
            PictureBox pictureBox = (PictureBox)sender;
            pictureBox.Tag = pictureBox.Left;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool correctOrder = true;

            foreach (var pictureBox in pictureBoxes)
            {
                int expectedOrder = Convert.ToInt32(pictureBox.Tag);

                // PictureBox'un konumu ve TextBox'ın konumu arasındaki farkı kontrol et
                int differenceX = Math.Abs(pictureBox.Left - pictureBoxTextBoxPairs[pictureBox].Left);
                int differenceY = Math.Abs(pictureBox.Top - pictureBoxTextBoxPairs[pictureBox].Top);

                // Eğer PictureBox'un konumu TextBox'un konumuyla çok yakınsa, bu durumu doğru sıralama olarak kabul et
                if (pictureBox.Image == null || differenceX > 5 || differenceY > 5 || Convert.ToInt32(pictureBox.Tag) != expectedOrder)
                {
                    correctOrder = false;
                    break;
                }
            }

            if (correctOrder)
            {
                MessageBox.Show("Doğru sıralama! Captcha onaylandı.");
            }
            else
            {
                MessageBox.Show("Yanlış sıralama! Lütfen tekrar deneyin.");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Her PictureBox'ı başlangıç konumuna döndür
            foreach (var pictureBox in pictureBoxes)
            {
                pictureBox.Left = (int)pictureBox.Tag;
                pictureBox.Top = 0; // Varsayılan olarak en üstte başlasın
            }

            // PictureBox'ları rastgele sırayla karıştır
            ShufflePictureBoxes();
        }

        private void ShufflePictureBoxes()
        {
            // PictureBox'ları rastgele bir sırayla karıştırmak için Fisher-Yates shuffle algoritmasını kullanın
            int n = pictureBoxes.Length;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                // Swap işlemi
                var tempPictureBox = pictureBoxes[k];
                pictureBoxes[k] = pictureBoxes[n];
                pictureBoxes[n] = tempPictureBox;
            }

            // PictureBox'ların yerlerini güncelle
            for (int i = 0; i < pictureBoxes.Length; i++)
            {
                pictureBoxes[i].Left = startingPoint.X + (i % 3) * pictureBoxes[i].Width;
                pictureBoxes[i].Top = startingPoint.Y + (i / 3) * pictureBoxes[i].Height;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Her PictureBox'ı önceki konumuna geri al
            foreach (var pictureBox in pictureBoxes)
            {
                if (lastPositions.ContainsKey(pictureBox))
                {
                    pictureBox.Location = lastPositions[pictureBox];
                }
            }

            // Geri alma tamamlandıktan sonra önceki konumları temizle
            lastPositions.Clear();
        }
    }
}

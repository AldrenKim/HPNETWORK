using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HopfieldNeuralNetwork;
using ImageProcess2;

namespace HPNet
{
    public partial class Form1 : Form
    {
        List<PictureBox> pic_box = new List<PictureBox>(101);
        BitmapFilter bp = new BitmapFilter();
        List<PictureBox> pattern_list = new List<PictureBox>();
        NeuralNetwork nn;

        int[,] matrix;
        int neurons;
        double energy;
        int noPattern;


        public Form1()
        {
            InitializeComponent();
        }
        private void AddPicBoxes()
        {
            foreach(var pb in groupBox2.Controls)
            {
                var a = pb as PictureBox;
                pic_box.Add(a);
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            AddPicBoxes();
            pic_box.Sort(CompareByPicBoxName);
        }
        
        private static int CompareByPicBoxName(PictureBox x, PictureBox y)
        {

            if (x == null)
            {
                if (y == null)
                {
                    // If x is null and y is null, they're
                    // equal.
                    return 0;
                }
                else
                {
                    // If x is null and y is not null, y
                    // is greater.
                    return -1;
                }
            }
            else
            {
                if (y == null)
                    return 1;
                else
                {
                    string[] sName1 = x.Name.Split(new string[] { "pictureBox" }, StringSplitOptions.None);
                    string[] sName2 = y.Name.Split(new string[] { "pictureBox" }, StringSplitOptions.None);

                    int x1 = Convert.ToInt32(sName1[1]);
                    int x2 = Convert.ToInt32(sName2[1]);

                    if (x1 > x2)
                        return 1;
                    else
                        return -1;

                }
            }

            

            

        }
        private void UpdateValue()
        {
            noNeurons.Text = neurons.ToString();
            energyVal.Text = energy.ToString();
            numPatterns.Text = noPattern.ToString();
        }

        private void NN_EnergyChanged(object sender, EnergyEventArgs e)
        {
            //... 
        }

        private void CreatePattern()
        {
            int offset = 0;
            int r = 0;
            Random rnd = new Random();
            for (int i = 0; i < nn.N; i++)
            {
                for(int j = 0; j < nn.N; j++)
                {
                    r = rnd.Next(2);
                    if(r==0) pic_box[offset+j].BackColor = Color.Black;
                    else if(r==1) pic_box[offset+j].BackColor = Color.White;

                }
                offset += 10;
            }
        }
        private void displayNeurons(List<Neuron> a)
        {
            foreach(var b in a)
            {
                Console.WriteLine(b.State);
            }
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            string picName = "Pattern"+ noPattern;

            Bitmap image = new Bitmap(openFileDialog1.FileName);
            Bitmap image2= new Bitmap(10,10);
            //BitmapFilter.RandomJitter(image, 30);
            BitmapFilter.Scale(ref image, ref image2, 150, 150);

            CreatePictureBox(picName, image2);
            noPattern++;
            UpdateValue();

            //addPatternOnRemoveMenu(picName);
        }

        

        private void CreatePictureBox(string name, Bitmap image) {

            PictureBox temp = new PictureBox();
            panel1.Controls.Add(temp);
            temp.Image = image;
            temp.Location = new Point(9,panel1.Location.Y + (noPattern * 180));
            temp.Name = name;
            temp.Size = new System.Drawing.Size(150, 150);
            pattern_list.Add(temp);

        }

        private void createNeurons100ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            nn = new NeuralNetwork(10);
            neurons = nn.N * nn.N;

            nn.EnergyChanged += new EnergyChangedHandler(NN_EnergyChanged);

            CreatePattern();
            UpdateValue();

            matrix = nn.Matrix;
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (var a in pic_box)
                a.BackColor = Color.Transparent;
            nn = new NeuralNetwork(0);
            neurons = 0;
            energy = 0;
            noPattern = 0;
            removeAll();
            UpdateValue();
        }

        private void addPaternToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void removeAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            removeAll();
        }

        private void removeAll()
        {
            panel1.Controls.Clear();
            pattern_list.Clear();
            noPattern = 0;
            UpdateValue();
        }




        /*private void MethodEventWrapper(object source, EventArgs e)
        {
            ToolStripMenuItem d = (ToolStripMenuItem)source;

            int index = pattern_list.FindIndex(r => r.Name == d.Name);
            pattern_list[index].Image = null;
            panel1.Controls.Remove(pattern_list[index]);
            pattern_list.RemoveAll(r => r.Name == d.Name);

            removePatternToolStripMenuItem.DropDownItems.Remove(d);

            noPattern--;
            UpdateValue();
        }
        
         private void addPatternOnRemoveMenu(string name)
        {
            ToolStripMenuItem tool = new ToolStripMenuItem();
            tool.Name = name;
            tool.Size = new System.Drawing.Size(182, 22);
            tool.Text = name;
            tool.Click += new System.EventHandler(MethodEventWrapper);
            removePatternToolStripMenuItem.DropDownItems.Add(tool);
        } 
        */
    }
}

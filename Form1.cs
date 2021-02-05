using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using HopfieldNeuralNetwork;
using ImageProcess2;

namespace HPNet
{
    public partial class Form1 : Form
    {
        List<PictureBox> pic_box = new List<PictureBox>(101);
        List<PictureBox> pattern_list = new List<PictureBox>();

        Bitmap clicked_pattern;
        
        NeuralNetwork nn;


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
            addPaternToolStripMenuItem.Enabled = false;
            runNetworkDynamicToolStripMenuItem.Enabled = false;
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

        private void createNeurons100ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            nn = new NeuralNetwork(100);
            neurons = nn.N;
            nn.EnergyChanged += new EnergyChangedHandler(NN_EnergyChanged);
            

            addPaternToolStripMenuItem.Enabled = true;

            CreatePattern();
            
            UpdateValue();

        }

        private void NN_EnergyChanged(object sender, EnergyEventArgs e)
        {
            int index = e.NeuronIndex;
            energyVal.Text = e.Energy.ToString();
            if (pic_box[index].BackColor == Color.Black)
            {
                pic_box[index].BackColor = Color.White;
            }
            else if (pic_box[index].BackColor == Color.White)
            {
                pic_box[index].BackColor = Color.Black;
            }
            pic_box[index].Refresh();
            energyVal.Refresh();
            Thread.Sleep(200);
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
            addPaternToolStripMenuItem.Enabled = false;
            runNetworkDynamicToolStripMenuItem.Enabled = false;
        }


        private void CreatePattern()
        {
            int offset = 0;
            int r = 0;
            Random rnd = new Random();
            for (int i = 0; i < 10; i++)
            {
                for(int j = 0; j < 10; j++)
                {
                    r = rnd.Next(2);
                    if(r==0) pic_box[offset+j].BackColor = Color.Black;
                    else if(r==1) pic_box[offset+j].BackColor = Color.White;

                }
                offset += 10;
            }
        }
        private void CreatePattern(Bitmap img)
        {
            int row = img.Height;
            int col = img.Width;
            int offset = 0;

            List<Neuron> pattern = new List<Neuron>();

            for(int i = 0; i < row; i++)
            {
                for(int j = 0; j < col; j++)
                {
                    Neuron n = new Neuron();
                    Color pixel = img.GetPixel(j,i);
                    if (pixel.B == 255 && pixel.G == 255 && pixel.R == 255)
                    {
                        n.State = NeuronStates.AlongField;
                        pic_box[offset+j].BackColor = Color.White;
                    }
                    else
                    {
                        n.State = NeuronStates.AgainstField;
                        pic_box[offset+j].BackColor = Color.Black;
                    }
                    pattern.Add(n);
                }
                offset += 10;   
            }
            nn.AddPattern(pattern);
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            string picName = "Pattern"+ noPattern;

            Bitmap originalImage = new Bitmap(openFileDialog1.FileName);
            Bitmap image2= new Bitmap(10,10);
            Bitmap temp = new Bitmap(originalImage);

            BitmapFilter.Scale(ref temp, ref image2, 150, 150);

            CreatePictureBox(picName, image2, originalImage);
            noPattern++;


            runNetworkDynamicToolStripMenuItem.Enabled = true;

            UpdateValue();


            //addPatternOnRemoveMenu(picName);
        }
        private void CreatePictureBox(string name, Bitmap image, Bitmap originalImage) {

            PictureBox temp = new PictureBox();
            panel1.Controls.Add(temp);
            temp.Image = image;
            temp.Location = new Point(9,panel1.Location.Y + (noPattern * 180));
            temp.Name = name;
            temp.Tag = originalImage;
            temp.Size = new System.Drawing.Size(150, 150);
            temp.Click += new EventHandler(pattern_click);
            pattern_list.Add(temp);

        }

        private void pattern_click(object sender, EventArgs e)
        {
            PictureBox p = (PictureBox)sender;
            Bitmap b = new Bitmap((Bitmap)p.Tag);
            clicked_pattern = new Bitmap((Bitmap)p.Tag);
            int disVal=0;


            foreach(var a in panel2.Controls.OfType<RadioButton>())
            {
                if (a.Checked == true)
                {
                    string[] s = a.Text.Split('%');
                    disVal = Convert.ToInt16(s[0]);
                    switch (disVal)
                    {
                        case 25: disVal = 80;
                            break;
                        case 100: disVal = 20;
                            break;
                        default: ;
                            break;
                    }
                }
            }

            foreach (var a in panel2.Controls.OfType<RadioButton>())
            {
                a.Checked = false;
            }

            BitmapFilter.RandomJitter(b, (Int16)disVal);
            CreatePattern(b);
        }

        private void runNetworkDynamicToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (clicked_pattern != null)
            {
                List<Neuron> initial = new List<Neuron>(nn.N);
                int size = nn.N / 10;
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        Neuron neuron = new Neuron();
                        Color pixel = clicked_pattern.GetPixel(j, i);
                        if (pixel.R == 0 && pixel.G == 0 && pixel.B == 0)
                        {
                            neuron.State = NeuronStates.AgainstField;
                        }
                        else if (pixel.R == 255 && pixel.G == 255 && pixel.B == 255)
                        {
                            neuron.State = NeuronStates.AlongField;
                        }
                        initial.Add(neuron);
                    }
                }
                nn.Run(initial);
                clicked_pattern = null;
                energyVal.Text = nn.Energy.ToString();
            }
            else
            {
                MessageBox.Show( "No pattern selected.","Error404", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Choose a level and click on a pattern.", "How to set distort level?", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

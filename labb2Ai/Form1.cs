using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace labb2Ai
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void button_browse_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            labelThumb.Text = "";
            //show the dialog and get the result
            DialogResult result = ofd.ShowDialog();
            if (result == DialogResult.OK)//test result
            {
                 string file = ofd.FileName;
                try
                {
                    FunctionsAI functionsAI = new FunctionsAI();
                    Controls.Add(pictureBox1);
                    pictureBox1.Image = new Bitmap(file);

                    textBoxAnswer.Text = string.Join("\n",await functionsAI.GetAiRespons(file));
                    pictureBox2.Image = await functionsAI.GetThumbnail(file);
                    labelThumb.Text = "Generated thumbnail";


                }
                catch(Exception ex)
                {
                    textBoxAnswer.Text = ex.Message + " Try another picture..";
                }
            }
            

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}

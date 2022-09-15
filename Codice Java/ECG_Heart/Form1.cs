using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;

namespace ECG_Heart
{
    public partial class Form1 : Form
    {
        delegate void serialCalback(string val);
        double x;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            btndiscconnect.Enabled = false;
            btnconnect.Enabled = true;
            timer1.Tick += timer1_Tick;
            chart1.Series[0].IsVisibleInLegend = false; 
        }

        private void cmbporte_DropDown(object sender, EventArgs e)
        {
            string[] portList = SerialPort.GetPortNames();
            cmbporte.Items.Clear();
            cmbporte.Items.AddRange(portList);

        }

        private void btnconnect_Click(object sender, EventArgs e)
        {
            if (!serialPort.IsOpen)
            {
                try
                {
                    serialPort.PortName = cmbporte.Text;
                    serialPort.BaudRate = Convert.ToInt32(cbmbound.Text);
                    serialPort.Open();

                    MessageBox.Show("Connessione Arduino");
                    btndiscconnect.Enabled = true;
                    btnconnect.Enabled = false;
                    chart1.Series[0].Points.Clear();
                    timer1.Start();
                }
                catch (Exception error)
                {
                    MessageBox.Show(error.Message);
                }
            }
        }

        private void btndiscconnect_Click(object sender, EventArgs e)
        {
            if (serialPort.IsOpen)
            {
                try
                {
                    timer1.Stop();
                    serialPort.Close();

                    MessageBox.Show("Disconnessione Arduino");
                    btndiscconnect.Enabled = false;
                    btnconnect.Enabled = true;
                    chart1.Series[0].Points.Clear();
                    
                }
                catch (Exception error)
                {
                    MessageBox.Show(error.Message);
                }               
            }
        }

        private void chartsetText(string val)
        {
            if (this.chart1.InvokeRequired)
            {
                serialCalback scb = new serialCalback(chartsetText);
                this.BeginInvoke(scb, new object[] { val });
            }
            else
            {
                if(!String.Equals("!",val))
                {
                    chart1.Series[0].Points.AddXY(x, val);
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string incomString = serialPort.ReadLine();
            chartsetText(incomString);

            //if(String.Equals(incomString,"0"))
            //{
            //    serialPort.Write("0");
            //}

            if (chart1.Series[0].Points.Count > 100) chart1.Series[0].Points.RemoveAt(0);

            chart1.ChartAreas[0].AxisX.Minimum = chart1.Series[0].Points[0].XValue;
            chart1.ChartAreas[0].AxisX.Maximum = x;

            x += 1;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.DirectX.DirectInput;


namespace directinputtest
{



    public partial class Form1 : Form
    {
        Timer timer = new Timer();
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            timer.Tick += new EventHandler(timer_Tick); // Everytime timer ticks, timer_Tick will be called
            timer.Interval = (1000) * (1);              // Timer will tick evert second
            timer.Enabled = true;                       // Enable the timer
            timer.Start();                              // Start the timer

            
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        
        void timer_Tick(object sender, EventArgs e)
        {
            int a = 1;
            SIClass.SendKeyAsInput(Microsoft.DirectX.DirectInput.Key.Return);
           // SIClass.SendKeyAsInput(Keys.A);    
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ShowLib
{
    public partial class FrmNotification : Form
    {

        int timerIntervalBackup;
        /// <summary>
        ///  
        ///  Exemplo: FrmNotification.Show("versão armazenada");
        ///          
        /// </summary>
        /// <param name="Mensagem"></param>
        /// 
        public FrmNotification(string Mensagem)
        {
            InitializeComponent();
            lblMsg.Text = Mensagem;
        }

        public static void Show(string message)
        {
            FrmNotification f = new FrmNotification(message);
            f.ShowDialog();
        }

        public static void Show(string message, double timeInSeconds)
        {
            FrmNotification f = new FrmNotification(message, timeInSeconds);
            f.ShowDialog();
        }


        public FrmNotification(string Mensagem, double timeInSeconds)
        {
            InitializeComponent();
            lblMsg.Text = Mensagem;
            timer.Interval = (int) (1000 * timeInSeconds);
        }

        private void lblMsg_Click(object sender, EventArgs e)
        {
            timer.Interval = timerIntervalBackup;
            Close();
        }

        private void NeoNotification_Load(object sender, EventArgs e)
        {
            timerIntervalBackup = timer.Interval;
            timer.Enabled = true;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            timer.Interval = timerIntervalBackup;
            Close();
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace SLT_safe_adsl
{
    public partial class frmMain : Form
    {
        String dataservice = "https://www.internetvas.slt.lk/SLTVasPortal-war/application/GetProfile";
        String passwordwrongurl = "https://www.internetvas.slt.lk/SLTVasPortal-war/application/j_security_check";
        decimal remainingPeak = -1;
        decimal remainingOPeak = -1;
        decimal lastPeack = -1;
        decimal lastOPeack = -1;
        public frmMain()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            showBaloon();
            this.Hide();
        }

        private void frmMain_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == this.WindowState)
            {
                showBaloon();
                this.Hide();
            }

            else if (FormWindowState.Normal == this.WindowState)
            {
                mynotifyIcon.Visible = false;
            }
        }

        private void portalAccountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmPortalLogin pl = new frmPortalLogin();
            pl.ShowDialog();
            bgsltworker.RunWorkerAsync();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 abt = new AboutBox1();
            abt.ShowDialog();
        }

        private void bgsltworker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                String adslusername = "";
                String adslpassword = "";

                if ((Application.UserAppDataRegistry.GetValue("slt_adsl_username") != null) && (Application.UserAppDataRegistry.GetValue("slt_adsl_password") != null))
                {
                    adslusername =
                      Application.UserAppDataRegistry.GetValue(
                      "slt_adsl_username").ToString();
                    adslpassword =
                      Application.UserAppDataRegistry.GetValue(
                      "slt_adsl_password").ToString();
                }
                else
                {
                    return;
                }
                String Parameters =  String.Format("j_username={0}&j_password={1}",adslusername,adslpassword);
                CookieContainer cookies = new CookieContainer();
                HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(passwordwrongurl);
                req.CookieContainer = cookies;
                req.ContentType = "application/x-www-form-urlencoded";
                req.Method = "POST";
                req.ContentLength = Parameters.Length;
                StreamWriter writer = new StreamWriter(req.GetRequestStream());
                writer.Write(Parameters);
                writer.Close();
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                if (resp == null)
                {
                    String str = "";
                }
                else
                {
                    System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
                    String str = sr.ReadToEnd().Trim();
                    if (str.Contains("Redirection Page"))
                    {
                        HttpWebRequest dataRequest = (HttpWebRequest)WebRequest.Create(dataservice);
                        dataRequest.CookieContainer = cookies;
                        HttpWebResponse dataresponse = (HttpWebResponse)dataRequest.GetResponse();

                        System.IO.StreamReader sr2 = new System.IO.StreamReader(dataresponse.GetResponseStream());
                        String str2 = sr2.ReadToEnd().Trim();

                        dynamic sltdata = JsonConvert.DeserializeObject(str2);
                        remainingPeak = Math.Round((Convert.ToDecimal(sltdata.peakrem) / 1073741824), 2);
                        remainingOPeak = Math.Round(((Convert.ToDecimal(sltdata.totalrem) - Convert.ToDecimal(sltdata.peakrem)) / 1073741824), 2);
                        //sltdata.totalrem 1073741824
                    }
                }
            }
            catch (Exception ex)
            {
                mynotifyIcon.BalloonTipText = "Sorry Internet connection issue";
                mynotifyIcon.BalloonTipText = "SLT Safe ADSL";
                mynotifyIcon.ShowBalloonTip(500);
                //MessageBox.Show("Internet connection issue","SLT Safe ADSL");
            }
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            bgsltworker.RunWorkerAsync();
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            bgsltworker.RunWorkerAsync();
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            showBaloon();
            this.Hide();
        }

        private void showBaloon()
        {
            mynotifyIcon.Visible = true;
            if (remainingPeak == -1)
            {
                mynotifyIcon.BalloonTipText = "Sorry Still contacting slt...";
                mynotifyIcon.BalloonTipTitle = "SLT Safe ADSL";
            }
            else
            {
                mynotifyIcon.BalloonTipText = String.Format("You have Peak {0} GB and Off Peak {1} GB remains", remainingPeak,remainingOPeak);
                mynotifyIcon.BalloonTipTitle = "SLT Safe ADSL";
            }
            mynotifyIcon.ShowBalloonTip(500);
        }

        private void bgsltworker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            lblpe.Text = remainingPeak.ToString() + " GB";
            lbloffp.Text = remainingOPeak.ToString() + " GB";
            showBaloon();
        }

        private void mynotifyIcon_BalloonTipClicked(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
            }
        }

        private void mynotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //if (this.WindowState == FormWindowState.Minimized)
            //{
                this.Show();
                this.WindowState = FormWindowState.Normal;
            //}
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            bgsltworker.RunWorkerAsync();
        }

    }
}

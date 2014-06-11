using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;

namespace SLT_safe_adsl
{
    public partial class frmPortalLogin : Form
    {
        public frmPortalLogin()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                Application.UserAppDataRegistry.SetValue("slt_adsl_username",
               txtUsername.Text);
                Application.UserAppDataRegistry.SetValue("slt_adsl_password",
    txtPassword.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            this.Close();
        }

        private void frmPortalLogin_Load(object sender, EventArgs e)
        {
            try
            {
                // Get the connection string from the registry.
                if ((Application.UserAppDataRegistry.GetValue("slt_adsl_username") != null) && (Application.UserAppDataRegistry.GetValue("slt_adsl_password") != null))
                {
                    txtUsername.Text =
                      Application.UserAppDataRegistry.GetValue(
                      "slt_adsl_username").ToString();
                    txtPassword.Text =
                      Application.UserAppDataRegistry.GetValue(
                      "slt_adsl_password").ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}

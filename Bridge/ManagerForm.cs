using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bridge
{
    public partial class ManagerForm : Form
    {
        private static Manager manager = null;
        public ManagerForm()
        {
            InitializeComponent();

            manager = new Manager();
            manager.updateTextEvent += new EventHandler(updateText);
            manager.init();
        }
        
        public void updateText(object sender, EventArgs s)
        {
            try
            {
                ErrorMsgLabel.Text = manager.getErrorMsg();
                CHAI3D_data.Text = manager.getHIPposition();
            }
            catch
            {
                ErrorMsgLabel.Text = "update text error";
            }
        }

        private void ManagerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            manager.stopCHAI3D();
        }
    }
}

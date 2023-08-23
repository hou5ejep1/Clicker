using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utilities;

namespace Click
{
    public partial class Form1 : Form
    {

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, IntPtr lParam);
        #region dllUsing
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        #endregion
        #region tray
        private readonly static string path = @"..\..\img\Icon.ico";
        private readonly Icon icon = new Icon(path);
        private void ЗакрытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void NotifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            notifyIcon1.Visible = false;
            WindowState = FormWindowState.Normal;
        }
        private void Form1_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                notifyIcon1.Visible = true;
            }
            else if (FormWindowState.Normal == this.WindowState)
            { notifyIcon1.Visible = false; }
        }
        #endregion
        private globalKeyboardHook gkh = new globalKeyboardHook();
        private static bool _intercept = true;
        private static Process pr = new Process();

        public Form1()
        {
            InitializeComponent();
            this.Icon = icon;
            this.Text = "Кликер";
            notifyIcon1.Icon = icon;
            notifyIcon1.Text = "Кликер";
            GetProc();
            gkh.HookedKeys.Add(Keys.PageDown);
            gkh.HookedKeys.Add(Keys.PageUp);
            gkh.KeyDown += new KeyEventHandler(Gkh_KeyDown);

        }
        private void GetProc()
        {
            comboBox1.Items.Clear();
            foreach (Process process in Process.GetProcesses().
                                            Where(p => !string.IsNullOrEmpty(p.MainWindowTitle)).ToList())
            {
                comboBox1.Items.Add(process.MainWindowTitle + "/" + process.Id);
            }
        }
        private void ComboBox1_Click(object sender, EventArgs e)
        {
            GetProc();
        }
        void Gkh_KeyDown(object sender, KeyEventArgs e)
        {
            IntPtr nowP = GetForegroundWindow();
            Keys key = e.KeyCode;
            if (_intercept)
            {
                try
                {
                    _intercept = false; // Stop listening keystroke for the next key
                    SetForegroundWindow(pr.MainWindowHandle);
                    
                    SendKeys.Send(key.ToString());
                    SetForegroundWindow(nowP);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    _intercept = true; // Reactivate listening keystroke for the next key
                }
            }
            e.Handled = true;
        }

        private void ComboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            string text = comboBox1.SelectedItem.ToString();
            string[] word = text.Split('/');
            int procId = int.Parse(word[word.Length - 1]);
            pr = Process.GetProcessById(procId);
        }

        
    }
}

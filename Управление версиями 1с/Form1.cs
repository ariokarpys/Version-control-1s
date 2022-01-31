using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using System.IO;
using System.ServiceProcess;

namespace Управление_версиями_1с
{
    public partial class Form1 : Form
    {
        string path;

        public Form1()
        {
            InitializeComponent();


            path = Directory.GetCurrentDirectory() + "\\Versions.txt";
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text != "")
            {
                WriteRegistry(comboBox1.Text);
            }

            UpdateLabe();
        }

        void WriteRegistry(string version)
        {
            string paramvalue = @"""C:\Program Files\1cv8\" + version + @"\bin\ragent.exe"" -srvc -agent -regport 1541 -port 1540 -range 1560:1591 -d ""C:\Program Files\1cv8\srvinfo"" -debug";

            try
            {
                RegistryKey currentUserKey = Registry.LocalMachine;
                RegistryKey currentKey = currentUserKey.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\1C:Enterprise 8.3 Server Agent (x86-64)", true);

                currentKey.SetValue("ImagePath", paramvalue, RegistryValueKind.ExpandString);

                currentUserKey.Close();
                currentKey.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(DateTime.Now + " Ошибка: [" + ex.Message + "]\r\n");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            OpenFileList();

            UpdateLabe();
        }

        private void OpenFileList()
        {
            if (File.Exists(path))
            {
                try
                {
                    using (StreamReader sr = new StreamReader(path))
                    {
                        string line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            comboBox1.Items.Add(line);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(DateTime.Now + " Ошибка: [" + ex.Message + "]\r\n");
                }
            }
            else
            {
                File.Create(path).Close();
            }
        }

        private void UpdateLabe()
        {
            ServiceController sc = new ServiceController("1C:Enterprise 8.3 Server Agent (x86-64)");

            label1.Text = "Служба: " + sc.Status.ToString();

            RegistryKey currentUserKey = Registry.LocalMachine;
            RegistryKey currentKey = currentUserKey.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\1C:Enterprise 8.3 Server Agent (x86-64)");

            string paramvalue = currentKey.GetValue("ImagePath").ToString();

            currentUserKey.Close();
            currentKey.Close();

            label2.Text = "Текущая версия: " + paramvalue.Split('\\')[3];
        }

        private void button2_Click(object sender, EventArgs e)
        {
            UpdateLabe();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ServiceController sc = new ServiceController("1C:Enterprise 8.3 Server Agent (x86-64)");
            if ((sc.Status.Equals(ServiceControllerStatus.Stopped)) || (sc.Status.Equals(ServiceControllerStatus.StopPending)))
            {
                try
                {
                    sc.Start();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(DateTime.Now + " Ошибка: [" + ex.Message + "]\r\n");
                }
            }

            UpdateLabe();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ServiceController sc = new ServiceController("1C:Enterprise 8.3 Server Agent (x86-64)");

            if ((sc.Status.Equals(ServiceControllerStatus.Running)) || (sc.Status.Equals(ServiceControllerStatus.StartPending)))
            {
                try
                {
                    sc.Stop();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(DateTime.Now + " Ошибка: [" + ex.Message + "]\r\n");
                }
            }

            UpdateLabe();
        }
    }
}

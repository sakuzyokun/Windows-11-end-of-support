using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Windows_11_end_of_support
{
    public partial class Form1 : Form
    {
        const string REG_PATH = @"Software\Microsoft\Windows\CurrentVersion\Run";
        const string REG_NAME = "Win11SupportNotice";
        public Form1()
        {
            InitializeComponent();

            string productName;

            using (RegistryKey key =
                Registry.LocalMachine.OpenSubKey(
                    @"SOFTWARE\Microsoft\Windows NT\CurrentVersion"))
            {
                productName = key.GetValue("ProductName")?.ToString();
            }

            if (productName.Contains("Windows Vista"))
            {
                label1.Text =
                    $"Windows Vista のサポートは、2012 年 4 月 10 日に終了します。詳細について\r\n" +
                    "は、こちらをクリックしてください。";
            }
            else if (productName.Contains("Windows 7"))
            {
                label1.Text =
                    $"Windows 7 のサポートは、2014 年 1 月 14 日に終了します。詳細について\r\n" +
                    "は、こちらをクリックしてください。";
            }
            else if (productName.Contains("Windows 8"))
            {
                label1.Text =
                    $"Windows 8 のサポートは、2016 年 1 月 13 日に終了します。詳細について\r\n" +
                    "は、こちらをクリックしてください。";
            }
            else if (productName.Contains("Windows 8.1"))
            {
                label1.Text =
                    $"Windows 8.1 のサポートは、2018 年 1 月 10 日に終了します。詳細について\r\n" +
                    "は、こちらをクリックしてください。";
            }
            /*else if (productName.Contains("Windows 10"))
            {
                label1.Text =
                    $"Windows 10 のサポートは、2025 年 10 月 14 日に終了します。詳細について\r\n" +
                    "は、こちらをクリックしてください。";
            }*/
            else
            {
                DateTime now = DateTime.Now;

                // 今月の最終日を取得（30 or 31 / 2月なら28 or 29）
                int lastDay = DateTime.DaysInMonth(now.Year, now.Month);

                string dateText =
                    $"{now.Year} 年 {now.Month} 月 {lastDay} 日";

                label1.Text =
                    $"Windows 11 のサポートは、{now.Year} 年 {now.Month} 月 {lastDay} 日に終了します。詳細について\r\n" +
                    "は、こちらをクリックしてください。";
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            using (RegistryKey rk = Registry.CurrentUser.OpenSubKey(REG_PATH, false))
            {
                if (rk == null || rk.GetValue(REG_NAME) == null)
                {
                    // レジストリが無い → False（＝自動起動ON）
                    checkBox1.Checked = false;
                    EnableStartup();
                }
                else
                {
                    // レジストリがある → 自動起動ON状態
                    checkBox1.Checked = false;
                }
            }

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                // True → 自動起動OFF
                DisableStartup();
            }
            else
            {
                // False → 自動起動ON
                EnableStartup();
            }
        }


        void EnableStartup()
        {
            using (RegistryKey rk = Registry.CurrentUser.OpenSubKey(REG_PATH, true))
            {
                rk.SetValue(REG_NAME, Application.ExecutablePath);
            }
        }

        void DisableStartup()
        {
            using (RegistryKey rk = Registry.CurrentUser.OpenSubKey(REG_PATH, true))
            {
                if (rk.GetValue(REG_NAME) != null)
                {
                    rk.DeleteValue(REG_NAME);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            System.Media.SystemSounds.Hand.Play();
            throw new Exception("Windows 11 End of Support.");
            Application.Exit();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string htmlPath = Path.Combine(Application.StartupPath, "11_eos.html");

            if (!File.Exists(htmlPath))
            {
                // 無かったら静かに無視（仕様です）
                return;
            }

            Process.Start(new ProcessStartInfo
            {
                FileName = htmlPath,
                UseShellExecute = true
            });
        }
    }
}

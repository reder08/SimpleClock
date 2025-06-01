using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.Wave;           // 音效檔播放器函式庫
using System.IO;             // 檔案讀取的IO函式庫

namespace SimpleClock
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            comboboxInitialzation();  // 下拉選單初始化

            timerClcok.Interval = 1000;
            timerClcok.Tick += timerClcok_Tick;
            timerClcok.Start();

            timerAlert.Interval = 1000;
            timerAlert.Tick += timerAlert_Tick;
        }

        List<string> hours = new List<string>();
        List<string> minutes = new List<string>();

        private string strSelectTime;
        private WaveOutEvent waveOut;
        private AudioFileReader audioFileReader;

        private string audioFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "alert.wav");

        // 初始化下拉選單
        private void comboboxInitialzation()
        {
            for (int i = 0; i <= 23; i++)
                cmbHour.Items.Add(string.Format("{0:00}", i));
            cmbHour.SelectedIndex = 0;

            for (int i = 0; i <= 59; i++)
                cmbMin.Items.Add(string.Format("{0:00}", i));
            cmbMin.SelectedIndex = 0;
        }

        private void timerClcok_Tick(object sender, EventArgs e)
        {
            txtTime.Text = DateTime.Now.ToString("HH:mm:ss");
            txtDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            txtWeekDay.Text = DateTime.Now.ToString("dddd");
        }

        private void timerAlert_Tick(object sender, EventArgs e)
        {
            if (strSelectTime == DateTime.Now.ToString("HH:mm"))
            {
                try
                {
                    stopWaveOut();

                    audioFileReader = new AudioFileReader(audioFilePath);
                    waveOut = new WaveOutEvent();
                    waveOut.Init(audioFileReader);
                    waveOut.Volume = 1.0f;
                    waveOut.Play();

                    MessageBox.Show("鬧鐘時間到了！");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("無法播放聲音檔，錯誤資訊: " + ex.Message);
                }
                finally
                {
                    timerAlert.Stop();
                }
            }
        }

        private void stopWaveOut()
        {
            if (waveOut != null)
            {
                waveOut.Stop();
                waveOut.Dispose();
                waveOut = null;
            }

            if (audioFileReader != null)
            {
                audioFileReader.Dispose();
                audioFileReader = null;
            }
        }

        private void btnSetAlert_Click(object sender, EventArgs e)
        {
            strSelectTime = cmbHour.SelectedItem.ToString() + ":" + cmbMin.SelectedItem.ToString();
            timerAlert.Start();
            btnSetAlert.Enabled = false;
            btnCancelAlert.Enabled = true;
        }

        private void btnCancelAlert_Click(object sender, EventArgs e)
        {
            stopWaveOut();
            timerAlert.Stop();
            btnSetAlert.Enabled = true;
            btnCancelAlert.Enabled = false;
        }
    }
}


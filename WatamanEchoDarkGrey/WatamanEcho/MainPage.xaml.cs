﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.SpeechSynthesis;
using Windows.Media.SpeechRecognition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.ViewManagement;
using Windows.UI;
using System.Globalization;
using Windows.UI.Core;
using System.Threading.Tasks;
using Windows.Devices.Power;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace WatamanEcho
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        DispatcherTimer timer = new DispatcherTimer();
        int tempMinute = 99;
        int tempHour = 99;
        int tempDate = 99;
        int m_iInterval = 15;

        int m_iSecond = 0;
        int m_iMinute = 0;
        int m_iHour = 0;
        int m_iHour12 = 0;
        int m_iDate = 0;

        String m_sPeriod = WEConstants.AM;

        int widthw = 500;

        public MainPage()
        {
            this.InitializeComponent();
            Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
            Windows.Devices.Power.Battery.AggregateBattery.ReportUpdated += AggregateBatteryOnReportUpdated;

            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
            //txtInfoX.Visibility = Visibility.Collapsed;
            //txtInfoY.Visibility = Visibility.Collapsed;
            //txtInterval.Visibility = Visibility.Collapsed;

        }
        private void Timer_Tick(object sender, object e)
        {
            DateTime dd = DateTime.Now;
            m_iSecond = dd.Second;
            m_iMinute = dd.Minute;
            if (m_iSecond % 2 == 0)
            {
                secondHand.Foreground = new SolidColorBrush(Colors.DarkGray);
            }
            else
            {
                secondHand.Foreground = new SolidColorBrush(Colors.Black);
            }

            if (m_iMinute != tempMinute)
            {
                minuteHand.Text = m_iMinute.ToString(WEConstants.TIME_FORMAT);
                tempMinute = m_iMinute;
                m_iHour = dd.Hour;
                tempDisplay();
                if (m_iHour != tempHour)
                {

                    tempHour = m_iHour;

                    if (m_iHour >= 12)
                    {
                        m_sPeriod = WEConstants.PM;
                    }
                    else
                    {
                        m_sPeriod = WEConstants.AM;
                    }
                    lblPeriod.Text = m_sPeriod;
                    if (m_iHour > 12)
                    {
                        m_iHour12 = m_iHour - 12;
                    }
                    else
                    {
                        m_iHour12 = m_iHour;
                    }
                    hourHand.Text = m_iHour12.ToString(WEConstants.TIME_FORMAT);

                    m_iDate = dd.Day;
                    if (m_iDate != tempDate)
                    {
                        tempDate = m_iDate;
                        lblDate.Text = tempDate.ToString(WEConstants.DATE_FORMAT);
                        lblDay.Text = dd.DayOfWeek.ToString();
                        String l_sYear = dd.Year.ToString();
                        lblYear1.Text = l_sYear[0].ToString();
                        lblYear2.Text = l_sYear[1].ToString();
                        lblYear3.Text = l_sYear[2].ToString();
                        lblYear4.Text = l_sYear[3].ToString();
                    }

                }

                if (m_iMinute % m_iInterval == 0 && tempHour > 5 && tempHour < 23)
                {
                    String l_sSayTime = "The time is ";
                    if (m_iHour == 0)
                    {
                        l_sSayTime += ": 12";
                    }
                    else
                    {
                        l_sSayTime += ":" + m_iHour12.ToString();
                    }

                    if (m_iMinute != 0)
                    {
                        l_sSayTime += ":" + m_iMinute.ToString();
                    }
                    l_sSayTime += ":" + m_sPeriod;
                    speak(l_sSayTime);
                }
            }
        }

        private void tempDisplay()
        {
            //txtPanel1_1.Text = panelHeader.ActualHeight.ToString();
            //txtPanel1_2.Text = panelHeader.ActualWidth.ToString();

            //txtPanel2_1.Text = panelClock.ActualHeight.ToString();
            //txtPanel2_2.Text = panelClock.ActualWidth.ToString();

            //txtPanel3_1.Text = panelInformation.ActualHeight.ToString();
            //txtPanel3_2.Text = panelInformation.ActualWidth.ToString();

            //var view = ApplicationView.GetForCurrentView();
            //txtPanel4_2.Text = view.VisibleBounds.Height.ToString() + "," + view.VisibleBounds.Width.ToString();

        }

        private async void speak(String p_sText)
        {
            var synth = new SpeechSynthesizer();
            synth.Voice = SpeechSynthesizer.AllVoices.First(gender => gender.Gender == VoiceGender.Female);

            var speechStream = await synth.SynthesizeTextToStreamAsync(p_sText);

            this.mediaElement.AutoPlay = true;
            this.mediaElement.SetSource(speechStream, speechStream.ContentType);
            this.mediaElement.Play();

        }

        private async void AggregateBatteryOnReportUpdated(Windows.Devices.Power.Battery sender, object args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                // await UpdatePercentage(sender);    
                var details = sender.GetReport();
                double getPercentage = (details.RemainingCapacityInMilliwattHours.Value / (double)details.FullChargeCapacityInMilliwattHours.Value);
                var getStatus = details.Status;
              
                int l_dPercentage = (int)(getPercentage * 100);
             //   txtPanel4_1.Text = l_dPercentage.ToString();
                if (l_dPercentage < 10)
                {
                    lblBatteryStatus.Foreground = GetSolidColorBrush("#AA0000");
                }
                else if (l_dPercentage < 20)
                {
                    lblBatteryStatus.Foreground = GetSolidColorBrush("#FF0000");
                }
                else if (l_dPercentage < 30)
                {
                    lblBatteryStatus.Foreground = GetSolidColorBrush("#FF4400");
                }
                else if (l_dPercentage < 40)
                {
                    lblBatteryStatus.Foreground = GetSolidColorBrush("#FF8800");
                }
                else if(l_dPercentage < 50)
                {
                    lblBatteryStatus.Foreground = GetSolidColorBrush("#FFCC00");
                }
                else if (l_dPercentage < 60)
                {
                    lblBatteryStatus.Foreground = GetSolidColorBrush("#FFFF00");
                }
                else if (l_dPercentage < 70)
                {
                    lblBatteryStatus.Foreground = GetSolidColorBrush("#CCFF00");
                }
                else if (l_dPercentage < 80)
                {
                    lblBatteryStatus.Foreground = GetSolidColorBrush("#88FF00");
                }
                else if (l_dPercentage < 90)
                {
                    lblBatteryStatus.Foreground = GetSolidColorBrush("#44FF00");
                }
                else if (l_dPercentage < 100)
                {
                    lblBatteryStatus.Foreground = GetSolidColorBrush("#00CC00");
                }
                else
                {
                    lblBatteryStatus.Foreground = GetSolidColorBrush("#00AA00");
                }

                if (getStatus.ToString().Equals("Charging"))
                {
                    lblBattery.Foreground = new SolidColorBrush(Colors.Aqua);
                } else
                {
                    lblBattery.Foreground = new SolidColorBrush(Colors.White);
                }
                lblBatteryStatus.Text = getStatus.ToString();
                lblBattery.Text = getPercentage.ToString("##%");
            });
        }

        public SolidColorBrush GetSolidColorBrush(string hex)
        {
            hex = hex.Replace("#", string.Empty);
            byte r = (byte)(Convert.ToUInt32(hex.Substring(0, 2), 16));
            byte g = (byte)(Convert.ToUInt32(hex.Substring(2, 2), 16));
            byte b = (byte)(Convert.ToUInt32(hex.Substring(4, 2), 16));
            SolidColorBrush myBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(Byte.Parse("255"), r, g, b));
            return myBrush;
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            
            Application.Current.Exit();
        }
    }
}

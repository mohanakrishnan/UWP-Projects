using System;
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
using Windows.UI.Xaml.Media.Imaging;
using SpeechReco;
using Windows.Networking.Connectivity;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace WatamanEcho
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        DispatcherTimer timer = new DispatcherTimer();
        SpeechRecognizer _recognizer;
        int tempMinute = 99;
        int tempHour = 99;
        int tempDate = 99;
        int m_iInterval = 15;

        int m_iSecond = 0;
        int m_iMinute = 0;
        int m_iHour = 0;
        int m_iHour12 = 0;
        int m_iDate = 0;

        bool m_bCharging = false;
        bool m_bDayFlag = true;
        String m_sPeriod = WEConstants.AM;

        int imgCounter = 1;
        int gifCounter = 1;
        private SolidColorBrush colorDarkgrey;
        private SolidColorBrush colorOrange;
        private SolidColorBrush colorWhite;

        public MainPage()
        {
            this.InitializeComponent();

            Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
            Windows.Devices.Power.Battery.AggregateBattery.ReportUpdated += AggregateBatteryOnReportUpdated;
         //   NetworkInformation.NetworkStatusChanged += NetworkInformation_NetworkStatusChanged;


            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
            lblResult.Visibility = Visibility.Collapsed;

            colorOrange = GetSolidColorBrush(WEConstants.Orange);
            colorDarkgrey = GetSolidColorBrush(WEConstants.DarkGrey);
            colorWhite = GetSolidColorBrush(WEConstants.White);

            changeTemp();
        }

        

        private void Timer_Tick(object sender, object e)
        {
            DateTime dd = DateTime.Now;
            m_iSecond = dd.Second;
            m_iMinute = dd.Minute;
            changeSecond();

            if (m_iMinute != tempMinute)
            {
                tempMinute = m_iMinute;
                m_iHour = dd.Hour;

                if (m_iHour != tempHour)
                {
                    tempHour = m_iHour;
                    m_iDate = dd.Day;
                    setDayFlag();
                    changeHour();
                    changeDate(dd);
                    changeAppearance();
                }
                changeMinute();
            }
        }

        private async void changeTemp()
        {
            double lat = 10.45;
            double lon = 77.51;
            Root myWeather = await OpenWeatherMap.GetWeather(lat, lon);
            if(myWeather.main != null)
            {
                lblTemp.Text = ((int)myWeather.main.temp).ToString();
                if (m_bDayFlag)
                {
                    lblTemp.Foreground = colorWhite;
                }else
                {
                    lblTemp.Foreground = colorDarkgrey;
                }
            }
            else
            {
                lblTemp.Foreground = colorOrange;
            }
        }

        private void setDayFlag()
        {
            if (tempHour > 5 && tempHour < 23)
            {
                m_bDayFlag = true;
            }
            else
            {
                m_bDayFlag = false;
            }
        }

        private void changeAppearance()
        {
            if (m_bDayFlag)
            {
                grdGrid.Background = GetSolidColorBrush(WEConstants.Transparent);

                brdr.Opacity = 60;

                hourHand.Foreground = colorWhite;
                minuteHand.Foreground = colorWhite;
                secondHand.Foreground = colorWhite;
                lblDay.Foreground = colorWhite;
                //lblTemp.Foreground = colorWhite;
                lblCelcius.Foreground = colorWhite;
                lblTitle.Foreground = colorWhite;
                HamburgerButton.Foreground = colorWhite;
            }
            else
            {
                grdGrid.Background = GetSolidColorBrush(WEConstants.Black);

                brdr.Opacity = 100;
                hourHand.Foreground = colorDarkgrey;
                minuteHand.Foreground = colorDarkgrey;
                secondHand.Foreground = colorDarkgrey;
                lblDay.Foreground = colorDarkgrey;
               // lblTemp.Foreground = colorDarkgrey;
                lblCelcius.Foreground = colorDarkgrey;
                lblTitle.Foreground = colorDarkgrey;
                HamburgerButton.Foreground = colorDarkgrey;

            }
        }

        private void changeDate(DateTime dd)
        {
            if (m_iDate != tempDate)
            {
                tempDate = m_iDate;
                lblDay.Text = dd.DayOfWeek.ToString() + ", " + dd.ToString("MMMM") + " " + tempDate.ToString(WEConstants.DATE_FORMAT);
            }
        }

        private void sayTime()
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

        private void changeMinute()
        {
            minuteHand.Text = m_iMinute.ToString(WEConstants.TIME_FORMAT);
            if (m_bDayFlag)
            {
              //  changeGif();
                changeImage();
            }
            if (m_iMinute % m_iInterval == 0)
            {
                if (m_bDayFlag)
                {
                    sayTime();
                }
                changeTemp();
            }
        }

        private void changeHour()
        {
            if (m_iHour >= 12)
            {
                m_sPeriod = WEConstants.PM;
            }
            else
            {
                m_sPeriod = WEConstants.AM;
            }

            if (m_iHour > 12)
            {
                m_iHour12 = m_iHour - 12;
            }
            else
            {
                m_iHour12 = m_iHour;
            }
            hourHand.Text = m_iHour12.ToString(WEConstants.TIME_FORMAT);
        }

        private void changeSecond()
        {
            if (m_iSecond % 2 == 0)
            {
                secondHand.Opacity = 0;
                if (m_bCharging)
                {
                    lblBattery.Opacity = 100;
                }
            }
            else
            {
                secondHand.Opacity = 100;
                if (m_bCharging)
                {
                    lblBattery.Opacity = 0;
                }
            }
        }

        private void changeGif()
        {
            if (gifCounter < 2)
            {
                gifCounter++;
            }
            else
            {
                gifCounter = 1;
            }

            grdGrid.Background = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri(this.BaseUri, "Assets/Images/H" + gifCounter + ".gif")),
                Stretch = Stretch.UniformToFill
            };
        }
        private void changeImage()
        {
            if (imgCounter < 7)
            {
                imgCounter++;
            }
            else
            {
                imgCounter = 1;
            }
            //ImageBrush brush = new ImageBrush();
            //brush.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/Image/M"+imgCounter+".jpg"));
            //brush.Stretch = Stretch.UniformToFill;
            //grdGrid.Background = brush;

            brdr.Background = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri(this.BaseUri, "Assets/Images/M" + imgCounter + ".jpg")),
                Stretch = Stretch.UniformToFill, Opacity = 60
            };
        }

        private async void speak(String p_sText)
        {
            SpeechSynthesizer synth = new SpeechSynthesizer();

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
                if (m_bDayFlag)
                {
                    int l_dPercentage = (int)(getPercentage * 100);
                    //   txtPanel4_1.Text = l_dPercentage.ToString();
                    if (l_dPercentage < 10)
                    {
                        lblBattery.Foreground = GetSolidColorBrush("#FFAA0000");
                    }
                    else if (l_dPercentage < 20)
                    {
                        lblBattery.Foreground = GetSolidColorBrush("#FFFF0000");
                    }
                    else if (l_dPercentage < 30)
                    {
                        lblBattery.Foreground = GetSolidColorBrush("#FFFF4400");
                    }
                    else if (l_dPercentage < 40)
                    {
                        lblBattery.Foreground = GetSolidColorBrush("#FFFF8800");
                    }
                    else if (l_dPercentage < 50)
                    {
                        lblBattery.Foreground = GetSolidColorBrush("#FFFFCC00");
                    }
                    else if (l_dPercentage < 60)
                    {
                        lblBattery.Foreground = GetSolidColorBrush("#FFFFFF00");
                    }
                    else if (l_dPercentage < 70)
                    {
                        lblBattery.Foreground = GetSolidColorBrush("#FFCCFF00");
                    }
                    else if (l_dPercentage < 80)
                    {
                        lblBattery.Foreground = GetSolidColorBrush("#FF88FF00");
                    }
                    else if (l_dPercentage < 90)
                    {
                        lblBattery.Foreground = GetSolidColorBrush("#FF44FF00");
                    }
                    else if (l_dPercentage < 100)
                    {
                        lblBattery.Foreground = GetSolidColorBrush("#FF00CC00");
                    }
                    else
                    {
                        lblBattery.Foreground = GetSolidColorBrush("#FF00AA00");
                    }
                }else
                {
                    lblBattery.Foreground = colorOrange;
                }
                if (getStatus.ToString().Equals("Charging"))
                {
                    m_bCharging = true;
                }
                else
                {
                    m_bCharging = false;
                    lblBattery.Opacity = 100;
                }

                lblBattery.Text = getPercentage.ToString("##%");
            });
        }

        public SolidColorBrush GetSolidColorBrush(string hex)
        {
            hex = hex.Replace("#", string.Empty);
            byte a = (byte)(Convert.ToUInt32(hex.Substring(0, 2), 16));
            byte r = (byte)(Convert.ToUInt32(hex.Substring(2, 2), 16));
            byte g = (byte)(Convert.ToUInt32(hex.Substring(4, 2), 16));
            byte b = (byte)(Convert.ToUInt32(hex.Substring(6, 2), 16));
            SolidColorBrush myBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(a, r, g, b));
            return myBrush;
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {

            Application.Current.Exit();
        }
    }
}

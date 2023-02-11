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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace WatamanEcho
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        DispatcherTimer timer = new DispatcherTimer();
        int pMinute = 0;
        int pHour = 5;

        public MainPage()
        {
            this.InitializeComponent();
            Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();

        }
        private void Timer_Tick(object sender, object e)
        {
            string fmt = "00";
            DateTime dd = DateTime.Now;
          
            secondHand.Text = dd.Second.ToString(fmt);
            minuteHand.Text = dd.Minute.ToString(fmt);
            hourHand.Text = dd.Hour.ToString(fmt);

            if (dd.Minute != pMinute)
            {
                pMinute = dd.Minute;
                if (dd.Hour != pHour)
                {
                    pHour = dd.Hour;
                }
                int ff = pMinute % 5;
                if (ff == 0 && pHour > 5 && pHour < 23 )
                {
                   
                    speak(dd.Hour.ToString() + ":" + pMinute);
                 //   speak("i Minute :" + ff.ToString() + "i Hour :" + pHour);
                }
            }

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
    }
}

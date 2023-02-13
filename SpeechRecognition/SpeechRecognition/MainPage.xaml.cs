using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.sp
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.SpeechRecognition;
using Windows.Media.SpeechSynthesis;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SpeechRecognition
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        SpeechRecognizer _recognizer = new SpeechRecognizer();
        SpeechSynthesizer _synth = new SpeechSynthesizer();
        SpeechRecognizer _startListening = new SpeechRecognizer();
        Random rnd = new Random();
        int RecTimeOut = 0;

        public MainPage()
        {
            this.InitializeComponent();
           
        }
        private async void spec()
        {
            if (_recognizer.State != SpeechRecognizerState.Idle)
            {
                await _recognizer.ContinuousRecognitionSession.CancelAsync();
                _recognizer.
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.SpeechRecognition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SpeechReco
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            if (true)
            {
                this.recognizer = new SpeechRecognizer();
            }
            speak();
        }
        SpeechRecognizer recognizer;
        private async void button_Click(object sender, RoutedEventArgs e)
        {
            bool permissionGained = await AudioCapturePermissions.RequestMicrophonePermission();
          

        }

        private async void speak()
        {
           
            try
            {

                await this.recognizer.CompileConstraintsAsync();

             //   this.recognizer.Timeouts.InitialSilenceTimeout = TimeSpan.FromSeconds(5);
                this.recognizer.Timeouts.EndSilenceTimeout = TimeSpan.FromSeconds(20);

                this.recognizer.UIOptions.AudiblePrompt = "Say whatever you like, I'm listening";
                this.recognizer.UIOptions.ExampleText = "The quick brown fox jumps over the lazy dog";
                this.recognizer.UIOptions.ShowConfirmation = true;
                this.recognizer.UIOptions.IsReadBackEnabled = true;
             //   this.recognizer.Timeouts.BabbleTimeout = TimeSpan.FromSeconds(5);

                var result = await this.recognizer.RecognizeWithUIAsync();

                if (result != null)
                {
                    StringBuilder builder = new StringBuilder();

                    builder.AppendLine(
                      $"I have {result.Confidence} confidence that you said [{result.Text}] " +
                      $"and it took {result.PhraseDuration.TotalSeconds} seconds to say it " +
                      $"starting at {result.PhraseStartTime:g}");

                    var alternates = result.GetAlternates(10);

                    builder.AppendLine(
                      $"There were {alternates?.Count} alternates - listed below (if any)");

                    if (alternates != null)
                    {
                        foreach (var alternate in alternates)
                        {
                            builder.AppendLine(
                              $"Alternate {alternate.Confidence} confident you said [{alternate.Text}]");
                        }
                    }
                    this.txtResults.Text = builder.ToString();
                }
            }
            catch (Exception exception)
            {

                var messageDialog = new Windows.UI.Popups.MessageDialog(exception.StackTrace, "Exception");
                await messageDialog.ShowAsync();

            }
        }
    }
}

using NationalInstruments.Controls;
using NationalInstruments.DAQmx;
using NationalInstruments.NetworkVariable;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace SL_DeskTop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Lab : Window
    {
       
      

        private NetworkVariableBufferedWriter<string[]> fGenParamsWriter;
        private string fgenparamslocation = @"\\localhost\system\fgenparams";

        private NetworkVariableSubscriber<double[]> waveSubscriber;
        private string wavearraylocation = @"\\localhost\system\wavearray";



        private string wavetype;
        public Lab()
        {
            InitializeComponent();


            GetChannels();

            fGenParamsWriter = new NetworkVariableBufferedWriter<string[]>(fgenparamslocation);
            fGenParamsWriter.PropertyChanged += FGenParamsWriter_PropertyChanged;
            fGenParamsWriter.Connect();


            waveSubscriber = new NetworkVariableSubscriber<double[]>(wavearraylocation);           
            waveSubscriber.ConnectionBehavior = SubscriberConnectionBehavior.UpdateOnConnect;
            waveSubscriber.PropertyChanged += WaveSubscriber_PropertyChanged;
            waveSubscriber.DataUpdated += WaveSubscriber_DataUpdated;
         

          

         


        }

        private void WaveSubscriber_DataUpdated(object sender, DataUpdatedEventArgs<double[]> e)
        {
            if (e.Data.HasValue)
            {
                double[] data = e.Data.GetValue();
               
                waveGraph.DataSource = data;

               
            }
           
        }

        private void WaveSubscriber_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ConnectionStatus")
                lblo.Content = waveSubscriber.ConnectionStatus;
        }

        private void FGenParamsWriter_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ConnectionStatus")
                lblstatus.Content = fGenParamsWriter.ConnectionStatus;
        }

        void GetChannels()
        {
            int devices = DaqSystem.Local.Devices.Length;
            if (devices > 0)
            {
                btnStart.IsEnabled = true;
                knobHorizontal.IsEnabled = true;
                knobVertical.IsEnabled = true;


            
            }


            int count = DaqSystem.Local.GetPhysicalChannels(PhysicalChannelTypes.AI, PhysicalChannelAccess.External).Length;

            for (int i = 0; i < count; i++)
                cmbChannels.Items.Add(DaqSystem.Local.GetPhysicalChannels(PhysicalChannelTypes.AI, PhysicalChannelAccess.External).GetValue(i));

            if (cmbChannels.Items.Count > 0)
                cmbChannels.SelectedIndex = 0;
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {


            btnStart.IsEnabled = false;
            cmbChannels.IsEnabled = false;
            btnStop.IsEnabled = true;

           waveSubscriber.Connect();



        }
        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            btnStart.IsEnabled = true;
            cmbChannels.IsEnabled = true;
            btnStop.IsEnabled = false;
            waveSubscriber.Disconnect();
           
        }




      

        private void knobHorizontal_ValueChanged(object sender, ValueChangedEventArgs<double> e)
        {
          
       
            hAxis.Range = new Range<double>(0, e.NewValue);
          
          
         
        }

        private void knobVertical_ValueChanged(object sender, ValueChangedEventArgs<double> e)
        {
          
              
           vAxis.Range = new Range<double>(-e.NewValue, e.NewValue);
            
      
            
        }

        private void KnobFrequency_ValueChanged(object sender, ValueChangedEventArgs<double> e)
        {
            nmbFrequency.Value = e.NewValue;
        }

        private void nmbFrequency_ValueChanged(object sender, ValueChangedEventArgs<double> e)
        {
            KnobFrequency.Value = e.NewValue;
        }

        private void KnobAmplitude_ValueChanged(object sender, ValueChangedEventArgs<double> e)
        {
            nmbAmplitude.Value = e.NewValue;
        }

        private void nmbAmplitude_ValueChanged(object sender, ValueChangedEventArgs<double> e)
        {
            KnobAmplitude.Value = e.NewValue;

        }

        private void btnStartGen_Click(object sender, RoutedEventArgs e)
        {
            string[] data = new string[] {
                KnobFrequency.Value.ToString(),
                wavetype,
                (KnobAmplitude.Value/2).ToString()};

            fGenParamsWriter.WriteData(new NetworkVariableData<string[]>(data));

            btnStopGen.IsEnabled = true;
            btnStartGen.IsEnabled = false;
        }

        private void WaveType_Checked(object sender, RoutedEventArgs e)
        {
            
                RadioButton radioButton = sender as RadioButton;
                wavetype = radioButton.Content.ToString().Trim() + " Wave";
            
        }

        private void btnStopGen_Click(object sender, RoutedEventArgs e)
        {

            btnStartGen.IsEnabled = true;
            btnStopGen.IsEnabled = false;

            //if (fGenTask != null)
            //{
            //    try
            //    {
            //        fGenTask.Stop();
            //    }
            //    catch (Exception x)
            //    {
            //        MessageBox.Show(x.Message);
            //    }

            //    fGenTask.Dispose();
            //    fGenTask = null;
            //    btnStartGen.IsEnabled = true;
            //    btnStopGen.IsEnabled = false;
            //}
        }

         }
}

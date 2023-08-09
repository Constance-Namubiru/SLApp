using NationalInstruments.DAQmx;
using System;
using System.Windows;
using System.Windows.Threading;

namespace SL_DeskTop.Classes
{
    class AOReader
    {
        AnalogSingleChannelReader reader;
        private Task waveTask;
        private int samples = 1000;
        private int samplingRate = 3000;

        private DispatcherTimer dispatcherTimer;


        public AOReader()
        {
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(1);
            dispatcherTimer.Tick += DispatcherTimer_Tick;



        }
        public void Read()
        {

            try
            {

                waveTask = new Task();

                // Create a virtual channel
                waveTask.AIChannels.CreateVoltageChannel("channel name", "",
                     (AITerminalConfiguration)(-1), Convert.ToDouble(-5),
                         Convert.ToDouble(5), AIVoltageUnits.Volts);

                waveTask.Timing.ConfigureSampleClock(string.Empty, samplingRate, SampleClockActiveEdge.Rising,
                                                         SampleQuantityMode.ContinuousSamples, samples);

                // Verify the Task
                waveTask.Control(TaskAction.Verify);

                reader = new AnalogSingleChannelReader(waveTask.Stream);

                dispatcherTimer.Start();
            }

            catch (DaqException exception)
            {

                MessageBox.Show(exception.Message);


            }

        }


        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            try
            {

                double[] data = reader.ReadMultiSample(samples);

               
            }
            catch (DaqException exception)
            {
                MessageBox.Show(exception.Message);
            }

        }


        public void StopRead()
        {

            dispatcherTimer.Stop();
            waveTask.Dispose();
        }
    }
}

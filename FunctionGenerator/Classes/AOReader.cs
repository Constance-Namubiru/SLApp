using NationalInstruments.DAQmx;
using NationalInstruments.NetworkVariable;
using System;
using System.Threading;
using System.Windows.Threading;

namespace FGWriter.Classes
{
    class AOReader
    {
        AnalogSingleChannelReader reader;
        private Task waveTask;
        private int samples = 12000;
        private int samplingRate = 15000;
        private double[] data = new double[1000];
      
        Timer t;

        NetworkVariableBufferedWriter<double[]> bufferedWriter;
        string variablelacation = @"\\localhost\system\wavearray";

        public AOReader()
        {
            bufferedWriter = new NetworkVariableBufferedWriter<double[]>(variablelacation);
            bufferedWriter.Connect();
           
     
        }


        public double[] Data
        {
            get { return data; }

        }
        public void StartWaveTask()
        {

            try
            {

                waveTask = new Task();

                // Create a virtual channel
                waveTask.AIChannels.CreateVoltageChannel("Dev1/ai0", "",
                     (AITerminalConfiguration)(-1), Convert.ToDouble(-5),
                         Convert.ToDouble(5), AIVoltageUnits.Volts);

                waveTask.Timing.ConfigureSampleClock(string.Empty, samplingRate, SampleClockActiveEdge.Rising,
                                                         SampleQuantityMode.ContinuousSamples, samples);

                // Verify the Task
                waveTask.Control(TaskAction.Verify);

                reader = new AnalogSingleChannelReader(waveTask.Stream);

                t = new Timer(DisplayTimeEvent, null, 0, 100);
               


            }

            catch (DaqException exception)
            {

                Console.WriteLine(exception.Message);


            }

        }


        private void DisplayTimeEvent(object o)
        {
            
            data = reader.ReadMultiSample(samples);
            bufferedWriter.WriteValue(data);
          

        }



        public void StopRead()
        {

          
            waveTask.Dispose();
        }


    }
}

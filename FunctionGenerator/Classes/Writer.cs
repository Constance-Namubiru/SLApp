using NationalInstruments.DAQmx;
using NationalInstruments.NetworkVariable;
using System;
using System.Threading;

namespace FGWriter.Classes
{
    class Writer
    {
        Task fGenTask;

        AOReader reader;
     

        public Writer()
        {
            reader = new AOReader();
            
        }

       
        public void Write(string frequency, string wavetype, string amplitude)
        {


            try
            {

                // create the task and channel
                fGenTask = new Task();
                fGenTask.AOChannels.CreateVoltageChannel("Dev1/ao0",
                    "",
                    Convert.ToDouble(-10),
                    Convert.ToDouble(10),
                    AOVoltageUnits.Volts);

                // verify the task before doing the waveform calculations
                fGenTask.Control(TaskAction.Verify);

                // calculate some waveform parameters and generate data
                FunctionGenerator fGen = new FunctionGenerator(
                    fGenTask.Timing,
                    frequency,
                    "250",
                    "5",
                    wavetype,
                    amplitude);

                // configure the sample clock with the calculated rate
                fGenTask.Timing.ConfigureSampleClock("",
                    fGen.ResultingSampleClockRate,
                    SampleClockActiveEdge.Rising,
                    SampleQuantityMode.ContinuousSamples, 1000);



                AnalogSingleChannelWriter writer =
                    new AnalogSingleChannelWriter(fGenTask.Stream);

                //write data to buffer
                writer.WriteMultiSample(false, fGen.Data);

                //start writing out data
                fGenTask.Start();
                reader.StartWaveTask();

            }
            catch (DaqException err)
            {
                //statusCheckTimer.Enabled = false;
                Console.WriteLine(err.Message);
                fGenTask.Dispose();
            }

           

        }



    


      

    }
}

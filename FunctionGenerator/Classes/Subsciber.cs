using NationalInstruments.NetworkVariable;
using System;
using System.ComponentModel;

namespace FGWriter.Classes
{
    
    class Subscriber
    {
        private NetworkVariableSubscriber<string[]> subscriber;
        private string variablelocation = @"\\localhost\system\fgenparams";

        Writer writer;
        public Subscriber()
        {
            writer = new Writer();

            subscriber = new NetworkVariableSubscriber<string[]>(variablelocation);
            subscriber.PropertyChanged += subscriber_PropertyChanged;
            subscriber.DataUpdated += Subscriber_DataUpdated;          
        
        }
        public void Connect()
        {
            subscriber.Connect();

        }
      

        private void subscriber_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ConnectionStatus")
                Console.WriteLine(subscriber.ConnectionStatus);
        }

        private void Subscriber_DataUpdated(object sender, DataUpdatedEventArgs<string[]> e)
        {
            if(e.Data.HasValue)
            {
                string[] data = e.Data.GetValue();
                string frequency = data[0];
                string wavetype = data[1];
                string amplitude = data[2];
                writer.Write(frequency, wavetype,amplitude);
            }
                
        }
    }
}

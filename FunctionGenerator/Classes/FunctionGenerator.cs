using NationalInstruments.DAQmx;
using NationalInstruments.NetworkVariable;
using System;
using System.Diagnostics;

namespace FGWriter.Classes
{

    public enum WaveformType
        {
            SineWave = 0,
            SquareWave = 1,
            TriangleWave = 2

        }

        public class FunctionGenerator
        {



        public FunctionGenerator(
                Timing timingSubobject,
                string desiredFrequency,
                string samplesPerBuffer,
                string cyclesPerBuffer,
                string type,
                string amplitude,
                int phase = 0)
            {

           
             WaveformType t = new WaveformType();
                t = WaveformType.SineWave;
                if (type == "Sine Wave")
                    t = WaveformType.SineWave;
                else if (type == "Square Wave")
                    t = WaveformType.SquareWave;
                else if (type == "Triangle Wave")
                    t = WaveformType.TriangleWave;

                else
                    Debug.Assert(false, "Invalid Waveform Type");

                Init(
                    timingSubobject,
                    Double.Parse(desiredFrequency),
                    Double.Parse(samplesPerBuffer),
                    Double.Parse(cyclesPerBuffer),
                    t,
                    Double.Parse(amplitude),
                    phase);
            }

            public FunctionGenerator(
                Timing timingSubobject,
                double desiredFrequency,
                double samplesPerBuffer,
                double cyclesPerBuffer,
                WaveformType type,
                double amplitude,
                int phase = 0)
            {
                Init(
                    timingSubobject,
                    desiredFrequency,
                    samplesPerBuffer,
                    cyclesPerBuffer,
                    type,
                    amplitude,
                    phase);
            }

            private void Init(
                Timing timingSubobject,
                double desiredFrequency,
                double samplesPerBuffer,
                double cyclesPerBuffer,
                WaveformType type,
                double amplitude,
                int phase)
            {
                if (desiredFrequency <= 0)
                    throw new ArgumentOutOfRangeException("desiredFrequency", desiredFrequency, "This parameter must be a positive number");
                if (samplesPerBuffer <= 0)
                    throw new ArgumentOutOfRangeException("samplesPerBuffer", samplesPerBuffer, "This parameter must be a positive number");
                if (cyclesPerBuffer <= 0)
                    throw new ArgumentOutOfRangeException("cyclesPerBuffer", cyclesPerBuffer, "This parameter must be a positive number");

                // First configure the Task timing parameters
                if (timingSubobject.SampleTimingType == SampleTimingType.OnDemand)
                    timingSubobject.SampleTimingType = SampleTimingType.SampleClock;

                _desiredSampleClockRate = (desiredFrequency * samplesPerBuffer) / cyclesPerBuffer;
                _samplesPerCycle = samplesPerBuffer / cyclesPerBuffer;

                // Determine the actual sample clock rate
                timingSubobject.SampleClockRate = _desiredSampleClockRate;
                _resultingSampleClockRate = timingSubobject.SampleClockRate;

                _resultingFrequency = _resultingSampleClockRate / (samplesPerBuffer / cyclesPerBuffer);

                switch (type)
                {
                    case WaveformType.SineWave:
                        _data = GenerateSineWave(_resultingFrequency, amplitude, _resultingSampleClockRate, samplesPerBuffer,phase);
                        break;
                    case WaveformType.SquareWave:
                        _data = GenerateSquareWave(_resultingFrequency, amplitude, _resultingSampleClockRate, samplesPerBuffer);
                        break;
                    case WaveformType.TriangleWave:
                        _data = GenerateTriangleWave(_resultingFrequency, amplitude, _resultingSampleClockRate, samplesPerBuffer);
                        break;
                    default:
                        // Invalid type value
                        Debug.Assert(false);
                        break;
                }
            }

            public double[] Data
            {
                get
                {
                    return _data;
                }
            }

            public double ResultingSampleClockRate
            {
                get
                {
                    return _resultingSampleClockRate;
                }
            }

            public static double[] GenerateSineWave(
                double frequency,
                double amplitude,
                double sampleClockRate,
                double samplesPerBuffer,
                int phase
                )
            {
                double deltaT = 1 / sampleClockRate; // sec./samp
                int intSamplesPerBuffer = (int)samplesPerBuffer;

                double[] rVal = new double[intSamplesPerBuffer];

                for (int i = 0; i < intSamplesPerBuffer; i++)
                    rVal[i] = amplitude * Math.Sin((2.0 * Math.PI * frequency * (i * deltaT)) + phase);


                return rVal;
            }

            public static double[] GenerateSquareWave(
                 double frequency,
                 double amplitude,
                 double sampleClockRate,
                 double samplesPerBuffer)
            {
                double deltaT = 1 / sampleClockRate; // sec./samp
                int intSamplesPerBuffer = (int)samplesPerBuffer;

                double[] rVal = new double[intSamplesPerBuffer];

                for (int i = 0; i < intSamplesPerBuffer; i++)
                    rVal[i] = amplitude * Math.Sign(Math.Sin(2.0 * Math.PI * frequency * (i * deltaT)));


                return rVal;
            }

            public static double[] GenerateTriangleWave(
             double frequency,
             double amplitude,
             double sampleClockRate,
             double samplesPerBuffer)
            {
                double deltaT = 1 / sampleClockRate; // sec./samp
                int intSamplesPerBuffer = (int)samplesPerBuffer;

                double[] rVal = new double[intSamplesPerBuffer];

                for (int i = 0; i < intSamplesPerBuffer; i++)
                    rVal[i] = amplitude / 2 - 4 * (float)Math.Abs
                        (Math.Round(deltaT * i * frequency - (0.25))
                        - (deltaT * i * frequency - 0.25));


                return rVal;
            }

            private double[] _data;
            private double _resultingSampleClockRate;
            private double _resultingFrequency;
            private double _desiredSampleClockRate;
            private double _samplesPerCycle;
        }
    }


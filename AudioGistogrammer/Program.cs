﻿using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics.PerformanceData;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AudioGistogrammer
{
    //class Program
    //{
    //    static void Main(string[] args)
    //    {
    //        float[] averageBuffer = new float[702];
    //        string path = @"C:\Users\Alexander\Desktop\IGOR - 2019\5. RUNNING OUT OF TIME.mp3";
    //        Gistogrammer gistogrammer = new Gistogrammer(path);
    //        gistogrammer.calculateValues(1000);
    //        float[] map = gistogrammer.getValuesInPersent();
    //        Console.WriteLine("______________________________");
    //        Array.ForEach(map, Console.WriteLine);
    //        for (int i = 0; i < map.Length; i++)
    //        {
    //            Console.WriteLine(i + ") " + map[i]);
    //        }
    //    }
    //}

    public class Gistogrammer
    {
        private WaveBuffer buffer;
        private float[] valuesBuffer;
        private int bufferLength;
        private float minAverage;
        private float maxAverage;

        public Gistogrammer(string path)
        {
            using (MediaFoundationReader media = new MediaFoundationReader(path))
            {
                int _byteBuffer32_length = (int)media.Length * 2;
                this.bufferLength = _byteBuffer32_length / sizeof(float);
                IWaveProvider stream32 = new Wave16ToFloatProvider(media);
                this.buffer = new WaveBuffer(_byteBuffer32_length);
                stream32.Read(this.buffer, 0, (int)_byteBuffer32_length);
            }
        }
        public float[] getValues()
        {
            return this.valuesBuffer;
        }

        public float[] getValuesInPersent(int roundValue = 2)
        {
            normalizeBuffer();
            float[] newBuffer = new float[this.valuesBuffer.Length];
            float range = Math.Abs(maxAverage) + Math.Abs(minAverage);
            for (int i = 0; i < this.valuesBuffer.Length; i++)
            {
                newBuffer[i] = (float)Math.Round((double)this.valuesBuffer[i] * 100 / maxAverage, roundValue, MidpointRounding.AwayFromZero);
            }
            return newBuffer;
        }
        private void normalizeBuffer()
        {
            for (int i = 0; i < this.valuesBuffer.Length; i++)
            {
                this.valuesBuffer[i] = this.valuesBuffer[i] + Math.Abs(minAverage);
            }
            maxAverage = Math.Abs(minAverage) + Math.Abs(maxAverage);
            minAverage = 0;
        }
        public void calculateValues(int pointsCount)
        {
            this.valuesBuffer = new float[pointsCount];
            int startRange = 0;
            int range = bufferLength / pointsCount;
            float averageValue = 0.0f;
            int number = 0;
            for (int i = 0; i < this.bufferLength; i++)
            {
                averageValue += buffer.FloatBuffer[i];
                startRange++;
                if (startRange == range)
                {
                    averageValue = averageValue / range;
                    valuesBuffer[number] = averageValue;
                    if (averageValue > maxAverage)
                    {
                        maxAverage = averageValue;
                    }
                    if (averageValue < minAverage)
                    {
                        minAverage = averageValue;
                    }
                    number++;
                    averageValue = 0.0f;
                    startRange = 0;
                }

            }

        }
    }
}

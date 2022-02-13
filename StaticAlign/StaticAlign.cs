using Trsfile;
using static Trsfile.Enums.TRSTag;


class StaticAlign
{
    string path = @"C:\Users\sucha\Desktop\DetectorV2\trs\Train_des（非接触式）.trs";
    string resPath = @"C:\Users\sucha\Desktop\DetectorV2\trs\res.trs";
    TraceSet traceSet;
    static object obj = new();
    const int shiftMax = 100;
    const int referenceFirst = 23062;
    const int referenceSampleNum = 1092;
    double[] reTrace;
    const int referenceTraceIndex = 2;

    double max;
    int threshold = 95;


    static void Main(string[] args)
    {
        var align = new StaticAlign();
        align.TimeTest();
    }

    StaticAlign()
    {
        traceSet = TraceSet.Open(path);
        reTrace = new double[referenceSampleNum];
        float[] vs = traceSet.Get(referenceTraceIndex).Sample;
        Array.Copy(vs, referenceFirst, reTrace, 0, referenceSampleNum);
    }

    float[]? DoAlign(float[] f)
    {
        double[] offsetTrace = new double[referenceSampleNum];
        //寻找最佳对齐点
        int shift = 0;
        for (int j = -shiftMax / 2; j < shiftMax / 2; j++)
        {
            Array.Copy(f, referenceFirst + j, offsetTrace, 0, referenceSampleNum);
            double cor = MathNet.Numerics.Statistics.Correlation.Pearson(reTrace, offsetTrace);
            if (max == 0)
                max = cor;
            if (cor > max)
            {
                max = cor;
                shift = j;
            }

        }

        if (max < threshold / 100f)
        {
            return null;
        }
        if (shift == 0)
            return f;
        if (shift < 0)
        {
            float[] temp = new float[f.Length];
            Array.Copy(f, f.Length + shift, temp, 0, -shift);
            Array.Copy(f, 0, temp, -shift, f.Length + shift);
            return temp;
        }
        else
        {
            float[] temp = new float[f.Length];
            Array.Copy(f, shift, temp, 0, f.Length - shift);
            Array.Copy(f, 0, temp, f.Length - shift, shift);
            return temp;
        }
    }

    Trace? Process(Trace t)
    {
        max = 0;

        float[]? d = DoAlign(t.Sample);
        if (d == null)
        {
            return null;
        }
        return new Trace(t.Title ?? string.Empty, d, t.Parameters);

    }

    void TimeTest()
    {
        var res = TraceSet.Create(resPath);
        int numTraces = traceSet.MetaData.GetInt(NUMBER_OF_TRACES);
        var sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        Parallel.For(0, numTraces, (i, ParallelLoopState) =>
        {
            Trace? trace = Process(traceSet.Get(i));
            if (trace is not null)
                lock (obj)
                {
                    res.Add(trace);
                }

        });
        //for (int i = 0; i < numTraces; i++)
        //{
        //    Trace? trace = Process(traceSet.Get(i));
        //    if (trace is null)
        //    {
        //        continue;
        //    }
        //    else
        //    {
        //        res.Add(trace);
        //    }

        //}
        res.Close();
        sw.Stop();
        Console.WriteLine(sw.ElapsedMilliseconds);
    }

}










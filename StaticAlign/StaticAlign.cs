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


    Trace? Process(Trace t)
    {
        float[] d = t.Sample;
        double max = 0;
        double[] offsetTrace = new double[referenceSampleNum];
        //寻找最佳对齐点
        int shift = 0;
        for (int j = -shiftMax / 2; j < shiftMax / 2; j++)
        {
            Array.Copy(d, referenceFirst + j, offsetTrace, 0, referenceSampleNum);
            double cor = MathNet.Numerics.Statistics.Correlation.Pearson(reTrace, offsetTrace);
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
            return t;
        if (shift < 0)
        {
            float[] temp = new float[d.Length];
            Array.Copy(d, d.Length + shift, temp, 0, -shift);
            Array.Copy(d, 0, temp, -shift, d.Length + shift);
        }
        else
        {
            float[] temp = new float[d.Length];
            Array.Copy(d, shift, temp, 0, d.Length - shift);
            Array.Copy(d, 0, temp, d.Length - shift, shift);
        }
        return new Trace(t.Title ?? string.Empty, d, t.Parameters);
    }

    void TimeTest()
    {
        var res = TraceSet.Create(resPath);
        int numTraces = traceSet.MetaData.GetInt(NUMBER_OF_TRACES);
        var sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        Parallel.For(0, numTraces, i =>
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










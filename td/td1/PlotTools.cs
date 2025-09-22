using OxyPlot;
public class PlotTools
{
    public static void MakePlot(string title, string file, LowpassRC filter)
    {
        // On calcule la valeur pour 400 points entre 10^1 et 10^5 Hz
        const int POINT_COUNT = 400;

        double[] x = Enumerable.Range(0, POINT_COUNT).Select(x => Math.Pow(10, (double)x / POINT_COUNT * 4 + 1)).ToArray();
        var impedance = x.Select(filter.H).ToArray();
        double[] yMag = impedance.Select(h => 20 * Math.Log10(h.Magnitude)).ToArray();
        double[] yPhase = impedance.Select(h => h.Phase * 180 / Math.PI).ToArray();

        var phaseSeries = new OxyPlot.Series.LineSeries()
        {
            YAxisKey = "Phase",
        };
        phaseSeries.Points.AddRange(x.Zip(yPhase, (xf, yf) => new OxyPlot.DataPoint(xf, yf)));
        var magSeries = new OxyPlot.Series.LineSeries()
        {
            YAxisKey = "Magnitude"
        };
        magSeries.Points.AddRange(x.Zip(yMag, (xf, yf) => new OxyPlot.DataPoint(xf, yf)));
        double f0 = filter.GetCutoffFrequency();
        var cutoffLine = new OxyPlot.Series.LineSeries()
        {
            Title = "Cutoff Frequency",
            Color = OxyColors.Red,
            LineStyle = LineStyle.Dash,
            YAxisKey = "Magnitude",
            Points =
            {
                new DataPoint(f0, -3), new DataPoint(f0, -40)
            }
        };

        var pm = new PlotModel
        {
            Title = title,
            Axes =
            {
                new OxyPlot.Axes.LogarithmicAxis
                {
                    Position = OxyPlot.Axes.AxisPosition.Bottom,
                    Title = "Frequency (Hz)",
                    MajorGridlineStyle = LineStyle.Solid,
                    MinorGridlineStyle = LineStyle.Dot,
                },
                new OxyPlot.Axes.LinearAxis
                {
                    Title = "Phase (°)",
                    MajorGridlineStyle = LineStyle.Solid,
                    MinorGridlineStyle = LineStyle.Dot,
                    Key = "Phase",
                    StartPosition = 0.0,
                    EndPosition = 0.49
                },
                new OxyPlot.Axes.LinearAxis
                {
                    Title = "Magnitude (dB)",
                    MajorGridlineStyle = LineStyle.Solid,
                    MinorGridlineStyle = LineStyle.Dot,
                    Key = "Magnitude",
                    StartPosition = 0.51,
                    EndPosition = 1.0,
                }
            },
            Series =
            {
                phaseSeries,
                magSeries,
                cutoffLine
            },
            Background = OxyColors.White
        };

        var pngExporter = new OxyPlot.SkiaSharp.PngExporter { Width = 800, Height = 600 };
        using var fs = File.Create(file);
        pngExporter.Export(pm, fs);
    }
}
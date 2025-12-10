using System.Numerics;

// --- correctif pour que les symbols Ohm et infini s'affichent correctement sous Windows
Console.OutputEncoding = System.Text.Encoding.UTF8;
// ---

{
    RCFilter f1 = new RCFilter(1e3, 0.01e-6, RCFilterKind.Lowpass);
    RCFilter f2 = new RCFilter(100e3, 0.0001e-6, RCFilterKind.Lowpass);
    SeriesFilter ff = new SeriesFilter([f1, f2]);
    PlotTools.MakePlot("Filtre RC 2nd ordre", "rc_2order.png", ff);
}

{
    RCFilter r1c1 = new RCFilter(10, 34e-6, RCFilterKind.Lowpass);
    Divider l2c8 = new Divider("L2C8", new Inductor("L2", 1.7e-3), new Capacitor("C8", 48e-6));
    Divider l5c9 = new Divider("L5C9", new Inductor("L5", 1.7e-3), new Capacitor("C9", 34e-6));
    SeriesFilter cheb = new SeriesFilter([r1c1, l2c8, l5c9]);
    PlotTools.MakePlot("Filtre Chebyshev", "chebyshev.png", cheb);
}

public abstract class Dipole
{
    private string nom;
    public Dipole(string nom)
    {
        if (string.IsNullOrWhiteSpace(nom))
        {
            throw new ArgumentException("nom invalide !");
        }
        this.nom = nom;
    }

    public string GetName()
    {
        return nom;
    }

    public abstract double GetResistance();
    public abstract Complex GetImpedance(double fHz);
}


public class Resistance : Dipole
{
    private double valeur_ohms;

    public Resistance(string nom, double valeur_ohms) : base(nom)
    {
        this.valeur_ohms = valeur_ohms;
    }

    // On rajoute override car GetResistance est désormais créée dans la classe de base
    // Ici on ne fait que lui donner un corps
    public override double GetResistance()
    {
        return valeur_ohms;
    }

    public override Complex GetImpedance(double fHz)
    {
        return new Complex(valeur_ohms, 0);
    }

    public override string ToString()
    {
        return $"Res({GetName()}, {valeur_ohms} Ω)";
    }
}

public class Capacitor : Dipole
{
    private double valeur_farads;

    public Capacitor(string nom, double valeur_farads) : base(nom)
    {
        this.valeur_farads = valeur_farads;
    }

    public override double GetResistance()
    {
        // On travaille pour l'instant en régime continu
        // Dans un tel régime, un condensateur se comporte comme un circuit ouvert
        // Donc sa "résistance" est infinie
        return double.PositiveInfinity;
    }

    public double GetCapacitance()
    {
        return valeur_farads;
    }

    public override Complex GetImpedance(double fHz)
    {
        return new Complex(0, -1 / (2 * Math.PI * fHz * valeur_farads));
    }

    public override string ToString()
    {
        return $"Cap({GetName()}, {valeur_farads} F)";
    }
}

public class Inductor : Dipole
{
    private double valeur_henrys;

    public Inductor(string nom, double valeur_henrys) : base(nom)
    {
        this.valeur_henrys = valeur_henrys;
    }

    public double GetInductance()
    {
        return this.valeur_henrys;
    }

    public override double GetResistance()
    {
        // On travaille pour l'instant en régime continu
        // Dans un tel régime, une inductance se comporte comme un fil
        // Donc sa "résistance" est nulle
        return 0;
    }

    public override Complex GetImpedance(double fHz)
    {
        return new Complex(0, 2 * Math.PI * fHz * valeur_henrys);
    }

    public override string ToString()
    {
        return $"Ind({GetName()}, {valeur_henrys} H)";
    }
}

public class Series : Dipole
{
    private Dipole[] dipoles;

    public Series(string nom, Dipole[] dipoles) : base(nom)
    {
        this.dipoles = dipoles;
    }

    public override double GetResistance()
    {
        double resistance = 0;
        foreach (Dipole d in dipoles)
        {
            resistance += d.GetResistance();
        }
        return resistance;
    }

    public override Complex GetImpedance(double fHz)
    {
        Complex impedance = new Complex(0, 0);
        foreach (Dipole d in dipoles)
        {
            impedance += d.GetImpedance(fHz);
        }
        return impedance;
    }

    public override string ToString()
    {
        return $"Ser({GetName()}, {string.Join(", ", (object[])dipoles)})";
    }
}

public class Parallel : Dipole
{
    private Dipole[] dipoles;

    public Parallel(string nom, Dipole[] dipoles) : base(nom)
    {
        this.dipoles = dipoles;
    }

    public override double GetResistance()
    {
        double inverseResistance = 0;
        foreach (Dipole d in this.dipoles)
        {
            inverseResistance += 1 / d.GetResistance();
        }
        return 1 / inverseResistance;
    }

    public override Complex GetImpedance(double fHz)
    {
        Complex inverseImpedance = new Complex(0, 0);
        foreach (Dipole d in this.dipoles)
        {
            inverseImpedance += 1 / d.GetImpedance(fHz);
        }
        return 1 / inverseImpedance;
    }

    public override string ToString()
    {
        return $"Par({this.GetName()}, {string.Join(", ", (object[])this.dipoles)})";
    }
}

public enum RCFilterKind
{
    Lowpass,
    Highpass
}

public class RCFilter : IFilter
{
    private Resistance R;
    private Capacitor C;
    private RCFilterKind kind;
    private Divider divider;

    public RCFilter(double R_ohms, double C_farads, RCFilterKind kind)
    {
        this.R = new Resistance("R", R_ohms);
        this.C = new Capacitor("C", C_farads);
        this.kind = kind;
        if (kind == RCFilterKind.Lowpass)
        {
            this.divider = new Divider("RC Divider", R, C);
        }
        else
        {
            this.divider = new Divider("RC Divider", C, R);
        }
    }

    public static RCFilter CreateForCutoffFrequency(double fHz, double cF, RCFilterKind kind)
    {
        double R_ohms = 1 / (2 * Math.PI * fHz * cF);
        return new RCFilter(R_ohms, cF, kind);
    }

    public double GetCutoffFrequency()
    {
        return 1 / (2 * Math.PI * R.GetResistance() * C.GetCapacitance());
    }

    public override double[] GetCharacteristicFrequencies()
    {
        return [GetCutoffFrequency()];
    }

    public override Complex H(double fHz)
    {
        return divider.H(fHz);
    }

    public override string ToString()
    {
        return $"RCFilter(R={R.GetResistance()} Ω, C={C.GetCapacitance()} F, f0={GetCutoffFrequency()} Hz, kind={kind})";
    }
}

public enum RLCFilterKind
{
    Lowpass,
    Highpass,
    Bandpass,
    Bandcut
}

public class RLCFilter : IFilter
{
    private Resistance R;
    private Capacitor C;
    private Inductor L;
    private RLCFilterKind kind;
    private Divider divider;

    public RLCFilter(double R_ohms, double C_farads, double L_henrys, RLCFilterKind kind)
    {
        this.R = new Resistance("R", R_ohms);
        this.C = new Capacitor("C", C_farads);
        this.L = new Inductor("L", L_henrys);
        this.kind = kind;
        if (kind == RLCFilterKind.Lowpass)
        {
            this.divider = new Divider("RLC Divider", 
                new Series("Series", new Dipole[] { R, L }), C);
        }
        else if (kind == RLCFilterKind.Highpass)
        {
            this.divider = new Divider("RLC Divider", 
                new Series("Series", new Dipole[] { R, C }), L);
        }
        else if (kind == RLCFilterKind.Bandpass)
        {
            this.divider = new Divider("RLC Divider", 
                new Series("Series", new Dipole[] { L, C }), R);
        }
        else if (kind == RLCFilterKind.Bandcut)
        {
            this.divider = new Divider("RLC Divider", R, 
                new Series("Series", new Dipole[] { L, C }));
        }
    }

    public override double[] GetCharacteristicFrequencies()
    {   
        double f0 = 1 / (2 * Math.PI * Math.Sqrt(L.GetInductance() * C.GetCapacitance()));
        switch (this.kind)
        {
            case RLCFilterKind.Lowpass or RLCFilterKind.Highpass:
                return [f0];
            case RLCFilterKind.Bandpass:
                return [
                    f0,
                    f0 + R.GetResistance() / (2 * Math.PI * L.GetInductance()),
                    f0 - R.GetResistance() / (2 * Math.PI * L.GetInductance()),
                ];

            case RLCFilterKind.Bandcut:
                return [
                    f0,
                    f0 + 1 / (2 * Math.PI * (R.GetResistance() * C.GetCapacitance())),
                    f0 - 1 / (2 * Math.PI * (R.GetResistance() * C.GetCapacitance())),
                ];
            default:
                throw new Exception("Invalid type");
        }
    }

    public override Complex H(double fHz)
    {
        return divider.H(fHz);
    }

    public override string ToString()
    {
        return $"RLCFilter(R={R.GetResistance()} Ω, L={L.GetInductance()} H, C={C.GetCapacitance()} F, kind={kind})";
    }
}

public class Divider : Dipole, IFilter
{
    private Dipole d1;
    private Dipole d2;

    public Divider(string nom, Dipole d1, Dipole d2) : base(nom)
    {
        this.d1 = d1;
        this.d2 = d2;
    }

    public override double GetResistance()
    {
        return d1.GetResistance() + d2.GetResistance();
    }

    public override Complex GetImpedance(double fHz)
    {
        return d1.GetImpedance(fHz) + d2.GetImpedance(fHz);
    }

    public override string ToString()
    {
        return $"Div({GetName()}, {d1}, {d2})";
    }

    public double[] GetCharacteristicFrequencies()
    {
        return [];
    }

    public Complex H(double fHz)
    {
        Complex Z1 = d1.GetImpedance(fHz);
        Complex Z2 = d2.GetImpedance(fHz);
        return Z2 / (Z1 + Z2);
    }
}

public interface IFilter
{
    Complex H(double fHz);
    double[] GetCharacteristicFrequencies();
}

public class SeriesFilter : IFilter
{
    private IFilter[] filters;

    public SeriesFilter(IFilter[] filters)
    {
        this.filters = filters;
    }

    public double[] GetCharacteristicFrequencies()
    {
        return [];
    }

    public Complex H(double fHz)
    {
        Complex hTotal = new Complex(1, 0);
        foreach (IFilter filter in filters)
        {
            hTotal *= filter.H(fHz);
        }
        return hTotal;
    }
}

public class ParallelFilter : IFilter
{
    private IFilter[] filters;

    public ParallelFilter(IFilter[] filters)
    {
        this.filters = filters;
    }

    public double[] GetCharacteristicFrequencies()
    {
        return [];
    }

    public Complex H(double fHz)
    {
        Complex hTotal = new Complex(0, 0);
        foreach (IFilter filter in filters)
        {
            hTotal += filter.H(fHz);
        }
        return hTotal;
    }
}
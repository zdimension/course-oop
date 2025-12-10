using System.Numerics;

// --- correctif pour que les symbols Ohm et infini s'affichent correctement sous Windows
Console.OutputEncoding = System.Text.Encoding.UTF8;
// ---

RCFilter filter4560 = RCFilter.CreateForCutoffFrequency(4560, 330e-9, RCFilterKind.Lowpass);
Console.WriteLine(filter4560); // affiche par exemple LowpassRC(R=105,7648478813765 Ω, C=3,3E-07 F, f0=4560 Hz)

RCFilter filter7000 = RCFilter.CreateForCutoffFrequency(7000, 470e-9, RCFilterKind.Highpass);
Console.WriteLine(filter7000); // affiche par exemple HighpassRC(R=48,3096153846154 Ω, C=4,7E-07 F, f0=7000 Hz)

 new RCFilter(1, 2, RCFilterKind.Lowpass)

public abstract class Dipole
{
    protected string nom;

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

    public Resistance(string nom, double valeur_ohms)
    {
        this.nom = nom;
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

    public Capacitor(string nom, double valeur_farads)
    {
        this.nom = nom;
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

    public Inductor(string nom, double valeur_henrys)
    {
        this.nom = nom;
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

    public Series(string nom, Dipole[] dipoles)
    {
        this.nom = nom;
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

    public Parallel(string nom, Dipole[] dipoles)
    {
        this.nom = nom;
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

public abstract class Filter
{
    public abstract Complex H(double fHz);
    public abstract double[] GetCharacteristicFrequencies();
}

public enum RCFilterKind
{
    Lowpass,
    Highpass
}

public class RCFilter : Filter
{
    private Resistance R;
    private Capacitor C;
    private RCFilterKind kind;

    public RCFilter(double R_ohms, double C_farads, RCFilterKind kind)
    {
        this.R = new Resistance("R", R_ohms);
        this.C = new Capacitor("C", C_farads);
        this.kind = kind;
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
        Complex Z_R = R.GetImpedance(fHz);
        Complex Z_C = C.GetImpedance(fHz);
        // if (kind == RCFilterKind.Lowpass)
        // {
        //     return Z_C / (Z_R + Z_C);
        // } 
        // else
        // {
        //     return Z_R + (Z_R + Z_C);
        // }

        return kind switch
        {
            RCFilterKind.Lowpass => Z_C / (Z_R + Z_C),
            RCFilterKind.Highpass => Z_R / (Z_R + Z_C),
            _ => throw new ArgumentException("Valeur incorrecte")
        };
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

public class RLCFilter : Filter
{
    private Resistance R;
    private Capacitor C;
    private Inductor L;
    private RLCFilterKind kind;

    public RLCFilter(double R_ohms, double C_farads, double L_henrys, RLCFilterKind kind)
    {
        this.R = new Resistance("R", R_ohms);
        this.C = new Capacitor("C", C_farads);
        this.L = new Inductor("L", L_henrys);
        this.kind = kind;
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
        Complex Z_R = R.GetImpedance(fHz);
        Complex Z_L = L.GetImpedance(fHz);
        Complex Z_C = C.GetImpedance(fHz);

        return kind switch
        {
            RLCFilterKind.Lowpass => Z_C / (Z_R + Z_L + Z_C),
            RLCFilterKind.Highpass => Z_L / (Z_R + Z_L + Z_C),
            RLCFilterKind.Bandpass => Z_R / (Z_R + Z_L + Z_C),
            RLCFilterKind.Bandcut => (Z_L + Z_C) / (Z_R + Z_L + Z_C),
            _ => throw new ArgumentException("Valeur incorrecte")
        };
    }

    public override string ToString()
    {
        return $"RLCFilter(R={R.GetResistance()} Ω, L={L.GetInductance()} H, C={C.GetCapacitance()} F, kind={kind})";
    }
}
using System.Numerics;

// --- correctif pour que les symbols Ohm et infini s'affichent correctement sous Windows
Console.OutputEncoding = System.Text.Encoding.UTF8;
// ---

LowpassRC filter4560 = LowpassRC.CreateForCutoffFrequency(4560, 330e-9);
Console.WriteLine(filter4560); // affiche par exemple LowpassRC(R=105,7648478813765 Ω, C=3,3E-07 F, f0=4560 Hz)

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
        return $"Res({nom}, {valeur_ohms} Ω)";
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
        return $"Cap({nom}, {valeur_farads} F)";
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
        return $"Ind({nom}, {valeur_henrys} H)";
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
        return $"Ser({nom}, {string.Join(", ", (object[])dipoles)})";
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
        return $"Par({this.nom}, {string.Join(", ", (object[])this.dipoles)})";
    }
}

public class LowpassRC
{
    private Resistance R;
    private Capacitor C;

    public LowpassRC(double R_ohms, double C_farads)
    {
        R = new Resistance("R", R_ohms);
        C = new Capacitor("C", C_farads);
    }

    public double GetCutoffFrequency()
    {
        return 1 / (2 * Math.PI * R.GetResistance() * C.GetCapacitance());
    }

    public static LowpassRC CreateForCutoffFrequency(double fHz, double cF)
    {
        double R_ohms = 1 / (2 * Math.PI * fHz * cF);
        return new LowpassRC(R_ohms, cF);
    }

    public Complex H(double fHz)
    {
        Complex Z_R = R.GetImpedance(fHz);
        Complex Z_C = C.GetImpedance(fHz);
        return Z_C / (Z_R + Z_C);
    }

    public override string ToString()
    {
        return $"LowpassRC(R={R.GetResistance()} Ω, C={C.GetCapacitance()} F, f0={GetCutoffFrequency()} Hz)";
    }
}
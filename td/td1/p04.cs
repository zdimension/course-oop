// --- correctif pour que les symbols Ohm et infini s'affichent correctement sous Windows
Console.OutputEncoding = System.Text.Encoding.UTF8;
// ---

void DisplayComponent(Dipole d, double i)
{
    Console.WriteLine($"{d}, U={d.GetResistance() * i} V");
}

Resistance R1 = new Resistance("R1", 1e3);
Resistance R2 = new Resistance("R2", 2e3);
Series S1 = new Series("S1", R1, R2);
DisplayComponent(S1, 2e-3); // affiche "Ser(S1, Res(R1, 1000 Ω), Res(R2, 2000 Ω)), U=6 V"

Parallel P1 = new Parallel("P1", R1, R2);
DisplayComponent(P1, 2e-3); // affiche "Par(P1, Res(R1, 1000 Ω), Res(R2, 2000 Ω)), U=1,3333333333333333 V"

public abstract class Dipole
{
    protected string nom;

    public string GetName()
    {
        return nom;
    }

    public abstract double GetResistance();
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

    public override string ToString()
    {
        return $"Cap({nom}, {valeur_farads} F)";
    }
}

public class Series : Dipole
{
    private Dipole d1, d2;

    public Series(string nom, Dipole d1, Dipole d2)
    {
        this.nom = nom;
        this.d1 = d1;
        this.d2 = d2;
    }

    public override double GetResistance()
    {
        return d1.GetResistance() + d2.GetResistance();
    }

    public override string ToString()
    {
        return $"Ser({d1}, {d2})";
    }
}

public class Parallel : Dipole
{
    private Dipole d1, d2;

    public Parallel(string nom, Dipole d1, Dipole d2)
    {
        this.nom = nom;
        this.d1 = d1;
        this.d2 = d2;
    }

    public override double GetResistance()
    {
        return 1 / (1 / d1.GetResistance() + 1 / d2.GetResistance());
    }

    public override string ToString()
    {
        return $"Par({d1}, {d2})";
    }
}
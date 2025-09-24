// --- correctif pour que les symbols Ohm et infini s'affichent correctement sous Windows
Console.OutputEncoding = System.Text.Encoding.UTF8;
// ---

Resistance R1 = new Resistance("R1", 1e3);
double I = 2e-3;

DisplayComponent(R1, I);

Resistance R2 = new Resistance("R2", 2e3);
DisplayComponent(R2, I);

Resistance R3 = new Resistance("R3", 3e3);
DisplayComponent(R3, I);

// On fait une fonction qui prend un Dipole en argument
// Ainsi on pourra réutiliser cette fonction pour d'autres types de dipôles
void DisplayComponent(Dipole d, double i)
{
    Console.WriteLine($"{d}, U={d.GetResistance() * i} V");
}

public abstract class Dipole
{
    // nom doit être protected pour qu'il soit accessible par les classes filles
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
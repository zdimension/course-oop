Resistance r = new Resistance(1e3);
double I = 2e-3;
double U = r.GetResistance() * I;
Console.WriteLine($"Pour la résistance R, à {I} A, la tension aux bornes est {U} V");


public class Resistance
{
    private double valeur_ohms;

    public Resistance(double valeur_ohms)
    {
        this.valeur_ohms = valeur_ohms;
    }

    // L'attribut est privé donc on crée une fonction publique permettant
    // de récupérer la valeur
    public double GetResistance()
    {
        return valeur_ohms;
    }
}
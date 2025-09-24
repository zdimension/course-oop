// --- correctif pour que les symbols Ohm et infini s'affichent correctement sous Windows
Console.OutputEncoding = System.Text.Encoding.UTF8;
// ---

Resistance R1 = new Resistance("R1", 1e3);
double I = 2e-3;
Console.WriteLine($"Pour {R1.GetName()}, à {I} A, la tension est {R1.GetResistance() * I} V");

Resistance R2 = new Resistance("R2", 2e3);
Console.WriteLine($"Pour {R2.GetName()}, à {I} A, la tension est {R2.GetResistance() * I} V");

Resistance R3 = new Resistance("R3", 3e3);
Console.WriteLine($"Pour {R3.GetName()}, à {I} A, la tension est {R3.GetResistance() * I} V");

Console.WriteLine(R1);
Console.WriteLine(R2);
Console.WriteLine(R3);


public class Resistance
{
    private string nom;
    private double valeur_ohms;

    public Resistance(string nom, double valeur_ohms)
    {
        this.nom = nom;
        this.valeur_ohms = valeur_ohms;
    }

    public string GetName()
    {
        return nom;
    }

    // L'attribut est privé donc on crée une fonction publique permettant
    // de récupérer la valeur
    public double GetResistance()
    {
        return valeur_ohms;
    }

    // Resistance, comme toutes les classes, hérite de la classe Object
    // Cette dernière contient une méthode ToString() qui est appelée
    // quand on veut afficher un objet (par exemple avec Console.WriteLine)
    // Ici on redéfinit cette méthode pour afficher quelque chose de plus
    // parlant
    public override string ToString()
    {
        return $"Res({nom}, {valeur_ohms} Ω)";
    }
}
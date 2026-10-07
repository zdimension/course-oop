# TD 2 — Des composants polymorphes

On veut ajouter des condensateurs et des inductances, puis manipuler tous les
composants de la même manière. Reprenez votre code du TD 1 ou la correction
`checkpoints/s01`.

## 1. Une classe commune

Créez une classe `Component` et faites-en hériter `Resistor`. Déplacez le nom
et la méthode `name()` dans cette classe : tous les composants auront un nom.

Le champ reste privé. Ajoutez un constructeur `protected` prenant le nom
en argument, ainsi que les méthodes publiques suivantes :

```cpp
virtual ~Component() = default;
virtual std::complex<double> impedance(double frequency_hz) const = 0;
virtual std::string description() const = 0;
```

> [!TIP]
> L'héritage public s'écrit `class Resistor : public Component`.
> Appelez le constructeur de la classe mère dans la liste d'initialisation :
> ```cpp
> Resistor::Resistor(std::string name, double resistance_ohm)
>     : Component(name), resistance_ohm_(resistance_ohm) {}
> ```
> `= 0` déclare une méthode virtuelle pure : les classes filles devront la
> redéfinir pour pouvoir être instanciées. Ajoutez `override` à ces redéfinitions.

Essayez de créer directement un `Component`. Pourquoi le compilateur le refuse-t-il ?

## 2. Impédance de la résistance

Implémentez `impedance` dans `Resistor`. L'impédance d'une résistance vaut
simplement $Z_R(f) = R$, quelle que soit la fréquence.

> [!TIP]
> [`std::complex<double>`](https://en.cppreference.com/cpp/numeric/complex)
> (`<complex>`) représente un nombre complexe.
> Par exemple, `{1000.0, 0.0}` représente une impédance réelle de 1 kΩ.
> Les opérateurs `+`, `*` et `/` fonctionnent sur les complexes.

## 3. Condensateur et inductance

Créez les classes `Capacitor` et `Inductor`, elles aussi dérivées de `Component`.
Leurs constructeurs prennent un nom et, respectivement, une capacité en farads
ou une inductance en henrys. Redéfinissez `description` et `impedance` :

$$
Z_C(f)=\frac{1}{j2\pi fC}, \qquad Z_L(f)=j2\pi fL
$$

Utilisez `constexpr double pi = 3.14159265358979323846;` et un complexe
`{0.0, 1.0}` pour $j$. Pour l'instant, vous pouvez supposer que les valeurs passées
en paramètres (capacité, inductance, fréquence) sont strictement positives.

## 4. Parcourir les composants

Créez trois objets et un tableau de pointeurs :

```cpp
Resistor resistor("R1", 1000.0);
Capacitor capacitor("C1", 100e-9);
Inductor inductor("L1", 10e-3);

const Component* components[] = {&resistor, &capacitor, &inductor};
```

Avec une boucle, affichez la description et l'impédance à 1 kHz de chaque
composant. Vous devriez obtenir, en ohms : $1000$, environ $-1591{,}55j$
et $62{,}83j$.

> [!TIP]
> Vous pouvez parcourir le tableau avec
> `for (const Component* component : components)`.
> Utilisez `->` pour appeler une méthode à travers un pointeur.
>
> Mais une boucle C classique fonctionne aussi, l'avantage de celle ci-dessus
> est qu'elle gère automatiquement le parcours.

Faudrait-il appeler `delete` sur les éléments du tableau ? Qui détruit les
trois composants ?

Ajoutez un tableau de fréquences `{10.0, 100.0, 1000.0, 10000.0}` et affichez
les trois impédances à chacune de ces fréquences.

## 5. Passer un composant à une fonction

Extrayez l'affichage dans une fonction `display_at` prenant un composant et
une fréquence. Passez le composant par référence constante pour éviter une copie.

Appelez la fonction sur une résistance, puis sur un condensateur. Vérifiez que
chacun utilise sa propre formule d'impédance.

Essayez ensuite de remplacer la référence par un passage par valeur. Lisez
l'erreur : `Component` est abstraite et ne peut pas être copiée ainsi.

> [!NOTE]
> Avec une classe mère concrète et copiable, un passage par valeur serait possible,
> mais ne conserverait que la partie mère de l'objet. C'est le *slicing*.

Écrivez enfin une fonction qui affiche tout le tableau à une fréquence donnée :

```cpp
void display_all(const Component* components[], std::size_t count, double frequency_hz);
```

Appelez-la avec `components` et la taille 3. Supposez les pointeurs non nuls
et les objets encore vivants pendant l'appel.

> [!TIP]
> Comme en C, un tableau passé à une fonction ne transmet pas sa taille :
> il faut la fournir séparément. `std::size_t` (`<cstddef>`) est un type
> entier utilisé pour les tailles et les indices.

## Pour aller plus loin

### Module et phase

Affichez le module et la phase des impédances avec
[`std::abs`](https://en.cppreference.com/cpp/numeric/complex/abs) et
[`std::arg`](https://en.cppreference.com/cpp/numeric/complex/arg).
Retrouvez dans la documentation l'unité de la phase, puis affichez-la en degrés.

### Chercher dans un tableau

Écrivez une fonction qui reçoit un tableau de pointeurs vers des composants,
sa taille et une fréquence, puis renvoie le composant dont le module de
l'impédance est le plus grand. Renvoyez `nullptr` pour un tableau vide.
En cas d'égalité, conservez le premier.

Testez à 10 Hz, 1 kHz et 100 kHz. Est-ce toujours le même composant ?
La fonction doit-elle connaître les classes `Resistor`, `Capacitor` et
`Inductor` pour faire cette recherche ?

### Deux pointeurs, combien d'objets ?

Placez deux fois l'adresse de la même résistance dans le tableau.
Combien d'objets `Resistor` avez-vous créés ? Combien de descriptions sont
affichées ? Vérifiez en ajoutant temporairement un affichage au constructeur.

### Ajouter un composant

Créez `ResistiveSensor`, dérivée de `Component`, dont la résistance dépend
d'une température : $R(T) = R_0(1 + \alpha(T-T_0))$.
Le constructeur reçoit le nom, $R_0$, $\alpha$, $T_0$ et $T$ ; l'impédance est réelle.
Prenez $R_0 = 100\,\Omega$, $\alpha = 0{,}004\,°C^{-1}$ et $T_0 = 0\,°C$.

Ajoutez des capteurs à 0 °C et 25 °C au tableau. Avez-vous dû modifier
`display_all` ? Et votre fonction de recherche ?

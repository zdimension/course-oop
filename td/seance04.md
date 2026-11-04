# TD 4 — Vérifier les valeurs et signaler les erreurs

Nos classes acceptent encore n'importe quelle valeur. On va leur faire refuser
les valeurs absurdes, puis réagir à ce refus dans le programme qui les utilise.
Reprenez votre code du TD 3 ou `checkpoints/s03`.

> [!TIP]
> Comme au TD 3, placez chaque exercice dans sa propre fonction et appelez
> depuis `main` celle qui vous intéresse. Le programme de l'exercice 1 ne
> pourra plus aller jusqu'au bout une fois l'exercice 2 fait.

## 1. Des valeurs absurdes

Construisez une résistance de −1000 Ω, un condensateur de 0 F et
une résistance au nom vide. Affichez la description de chacun et son impédance
à 1 kHz.

Le programme s'exécute jusqu'au bout sans rien signaler. L'impédance du
condensateur s'affiche `(inf,-nan)` : placé dans un montage, il fausserait le
résultat de tout le montage, sans indiquer d'où vient le problème.

## 2. Refuser une valeur invalide

Dans le constructeur de `Resistor`, lancez une exception
[`std::invalid_argument`](https://en.cppreference.com/cpp/error/invalid_argument)
(`<stdexcept>`) si la résistance n'est pas strictement positive :

```cpp
throw std::invalid_argument("resistance must be positive");
```

Relancez le programme de l'exercice 1 et lisez le message affiché. Quelles
lignes ont été exécutées ?

Faites de même pour la capacité et l'inductance.

Il reste le nom. Où placer ce contrôle pour qu'il s'applique à tous les
composants, `Series` et `Parallel` compris, en ne l'écrivant qu'une fois ?

> [!TIP]
> [`name.empty()`](https://en.cppreference.com/cpp/string/basic_string/empty)
> indique si une chaîne est vide.

> [!WARNING]
> Si le constructeur initialise le champ avec `name_(std::move(name))`, testez
> `name_` et non `name` : après le déplacement, le paramètre ne doit plus être
> utilisé (TD 3). Avec GCC, il est vide, et le test refuserait tous les noms.

Reste la fréquence. Une fréquence négative n'a pas de sens ici. Une fréquence
nulle en a un : en continu, un condensateur se comporte comme un circuit ouvert
et une inductance comme un fil. Mais `std::complex<double>` représente mal une
impédance infinie : le condensateur donne `(inf,-nan)` à 0 Hz, et le NaN se
propage dans les calculs suivants. Notre modèle se limite donc au régime
sinusoïdal : dans chaque `impedance`, lancez `std::invalid_argument` si la
fréquence n'est pas strictement positive.

Faites-le aussi pour la résistance, même si elle saurait calculer son impédance
à 0 Hz : appeler `impedance` avec une fréquence nulle est une erreur de
l'appelant, qui doit être signalée même si elle ne tombe que sur des
résistances.

On suppose ici que les nombres reçus sont finis (ni NaN ni infini).

## 3. Intercepter l'exception

Testez la création d'une résistance négative :

```cpp
bool rejected = false;
try {
    Resistor resistor("R1", -1000.0);
} catch (const std::invalid_argument& error) {
    rejected = true;
    std::cout << error.what() << '\n';
}
assert(rejected); // <cassert>
```

On veut tester plusieurs cas, chacun dans son propre `try` : le premier
`throw` quitte le bloc, et les instructions suivantes du bloc ne seraient pas
exécutées. Plutôt que de recopier ce bloc, écrivez une fonction :

```cpp
bool resistor_rejected(const std::string& name, double resistance_ohm);
```

Elle renvoie `true` si la construction lance `std::invalid_argument`.
Vérifiez qu'une résistance négative, une résistance nulle et un nom vide sont
refusés, et qu'une résistance R1 de 1 kΩ est acceptée.

Testez de même une capacité et une inductance nulles. Pour la fréquence nulle,
une seule fonction suffit pour les trois composants : quel type de paramètre
doit-elle recevoir ?

Relancez enfin vos tests du TD 3 : les calculs valides doivent donner les
mêmes résultats qu'avant.

## 4. Montages vides et pointeurs nuls

Calculez l'impédance d'un `Series` vide, puis d'un `Parallel` vide. On pourrait
y voir un fil et un circuit ouvert, mais dans notre programme, un montage vide
vient presque toujours d'un `add` oublié.

Le constructeur ne peut pas refuser un montage vide : un montage est toujours
vide juste après sa construction, et on le remplit ensuite. Le contrôle se fait
donc au moment du calcul. Dans `impedance`, lancez
[`std::logic_error`](https://en.cppreference.com/cpp/error/logic_error) si le
montage est vide. Ce n'est pas l'argument reçu qui est en cause, mais l'état de
l'objet : on n'utilise donc pas `std::invalid_argument`.

Au TD 3, on supposait aussi que `add` ne recevait jamais de pointeur nul. Ce
cas arrive pourtant facilement :

```cpp
auto r3 = std::make_unique<Resistor>("R3", 3000.0);
series.add(std::move(r3));
series.add(std::move(r3)); // r3 vaut déjà nullptr
```

Essayez : affichez un message après le second `add`, puis calculez l'impédance
de la série. À quel moment le programme s'arrête-t-il ? Est-ce à l'appel qui
contient l'erreur ?

Dans `add`, lancez `std::invalid_argument` si le pointeur est nul : l'erreur
sera signalée par l'appel fautif. Testez les deux contrôles.

Un montage vide peut toujours être créé, rempli et décrit ; seul le calcul de
son impédance est refusé.

Parmi les contrôles ajoutés depuis le début du TD, lesquels protègent un
invariant de l'objet ? Lesquels vérifient une précondition d'une méthode ?

## 5. Choisir un type de filtre

Aux TD 5 et 6, on construira des filtres passe-bas et passe-haut. Pour
représenter ce choix, créez `include/filter_kind.hpp` :

```cpp
enum class RcFilterKind { LowPass, HighPass };
```

Déclarez une variable `RcFilterKind kind = RcFilterKind::LowPass;` et essayez
de l'afficher avec `std::cout`. Lisez la première erreur du compilateur.

Déclarez dans `filter_kind.hpp` une fonction
`std::string to_string(RcFilterKind kind)`, et définissez-la dans
`src/filter_kind.cpp` avec un `switch` : elle renvoie `"low-pass"` pour
`LowPass` et `"high-pass"` pour `HighPass`.

> [!TIP]
> Ajoutez `src/filter_kind.cpp` aux sources dans `CMakeLists.txt`. Vérifiez
> aussi que les avertissements sont activés (`-Wall -Wextra -Wpedantic` dans
> `target_compile_options`, comme dans
> [l'installation](setup.md#4-vérifier-addresssanitizer)).

Compilez et lisez l'avertissement de GCC sur la fin de la fonction. Une variable `RcFilterKind` peut en effet contenir une autre valeur
que les deux prévues : `static_cast<RcFilterKind>(42)` compile. Après le
`switch`, lancez `std::invalid_argument`, puis vérifiez que
`to_string(static_cast<RcFilterKind>(42))` lance bien cette exception.

Ajoutez temporairement une valeur `BandPass` à l'énumération et recompilez :
le compilateur signale le `switch` à compléter. Il ne le ferait pas si le
`switch` avait un `default`, qui traiterait la nouvelle valeur sans rien dire.
Retirez ensuite `BandPass`.

Testez les deux valeurs de l'énumération.

## Pour aller plus loin

### Des valeurs moins ordinaires

Refusez les noms composés uniquement d'espaces. Refusez aussi NaN et les infinis
avec [`std::isfinite`](https://en.cppreference.com/cpp/numeric/math/isfinite).
Dans [`std::numeric_limits<double>`](https://en.cppreference.com/cpp/types/numeric_limits),
trouvez comment obtenir un infini et un NaN pour vos tests.

Testez `value <= 0.0` avec un NaN. Ce test suffit-il à rejeter toutes les
valeurs qui ne sont pas des résistances valides ?

### Lire une résistance

Lisez une résistance au clavier dans `main`, puis construisez l'objet et
interceptez son éventuelle exception. Testez avec `1000`, `-1000` et `bonjour`.

> [!TIP]
> `std::cin >> value` lit une valeur au clavier.
> `if (!(std::cin >> value))` détecte un échec de lecture, par exemple du texte
> à la place d'un nombre. C'est une erreur différente d'une résistance négative.

### Quel `catch` est appelé ?

Interceptez une erreur de résistance négative avec deux `catch`, d'abord
`std::invalid_argument`, puis `std::logic_error`. Inversez leur ordre et
recompilez : que signale le compilateur ? Quel `catch` est exécuté ?
Cherchez dans la documentation de quelle classe dérive `std::invalid_argument`.

### Retour sur `child`

Si vous avez écrit `child` avec `at` au TD 3, appelez `child(2)` sur une série
de deux composants, dans un `try`. Quel type d'exception `at` lance-t-il ?
Un `catch` de `std::logic_error` l'intercepte-t-il ?

### Une exception au milieu du travail

Dans un `try`, créez une résistance valide avec `make_unique`, puis essayez
d'en construire une seconde avec une valeur négative. La première est-elle
libérée lorsque l'exception quitte le bloc ? Prédisez, puis vérifiez avec
un affichage temporaire du destructeur et AddressSanitizer.

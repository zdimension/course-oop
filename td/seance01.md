# TD 1 — Premiers objets et durée de vie

On va commencer par modéliser une résistance. Si vous n'avez pas encore compilé
le Hello World, suivez d'abord la [page d'installation](setup.md).

## 1. Pointeurs et références

Sans lancer le programme, prédisez son affichage :

```cpp
#include <iostream>

void add_one(int value) {
    ++value;
}

void add_one_pointer(int* value) {
    ++(*value);
}

int main() {
    int value = 41;
    int* pointer = &value;
    add_one(value);
    std::cout << value << '\n';
    add_one_pointer(pointer);
    std::cout << value << '\n';
}
```

> [!TIP]
> `std::cout` (`<iostream>`) affiche dans le terminal. L'opérateur `<<` lui
> envoie les valeurs à afficher ; `'\n'` ajoute un saut de ligne.

Compilez et comparez avec votre réponse. Pourquoi les deux appels n'ont-ils pas
le même effet sur `value` ?

Ajoutez une troisième fonction, `void add_one_reference(int& value)`, et
appelez-la avec `add_one_reference(value)`. Que constatez-vous ?

> [!TIP]
> Une référence (`int&`) est un autre nom pour un objet existant. Contrairement
> au passage par valeur, elle ne crée pas de copie.

## 2. Une première résistance

Créez une classe `Resistor` avec deux champs privés : un nom (`std::string`)
et une valeur en ohms (`double`). Ajoutez les méthodes suivantes :

```cpp
Resistor(std::string name, double resistance_ohm);
const std::string& name() const;
double resistance_ohm() const;
// U = RI
double voltage(double current_ampere) const;
std::string description() const;
```

Le constructeur initialise les deux champs. `name()` et `resistance_ohm()`
renvoient leurs valeurs ; `description()` renvoie une chaîne telle que
`R1 (1000 ohm)`.

> [!TIP]
> [`std::string`](https://en.cppreference.com/cpp/string/basic_string) est déclaré dans `<string>`.
> Vous pouvez assembler des chaînes avec `+` et convertir un nombre avec
> [`std::to_string`](https://en.cppreference.com/cpp/string/basic_string/to_string). Le nombre de décimales
> affichées n'a pas d'importance ici.
>
> Le `const` après les parenthèses indique que la méthode ne modifie pas l'objet.
> `name()` renvoie une référence au nom : elle reste valide tant que la résistance
> existe. `description()` renvoie une nouvelle chaîne par valeur.

Placez la déclaration de la classe dans `include/resistor.hpp` et les définitions
des méthodes dans `src/resistor.cpp`. Ajoutez `#pragma once` en tête du `.hpp`
pour éviter les inclusions multiples.

Dans le `CMakeLists.txt` du Hello World, gardez `cmake_minimum_required` et
`project`, puis remplacez la déclaration de l'exécutable par :

```cmake
add_executable(td01 main.cpp src/resistor.cpp)
target_include_directories(td01 PRIVATE include)
target_compile_features(td01 PRIVATE cxx_std_20)
```

Si vous avez gardé les options d'AddressSanitizer, remplacez aussi leur cible
`cpp_hello` par `td01`.

Dans `main.cpp`, créez une résistance « R1 » de 1 kΩ. Affichez sa description
et la tension pour un courant de 2 mA : vous devriez trouver 2 V.

```bash
cmake -S . -B build -G "Unix Makefiles" -DCMAKE_BUILD_TYPE=Debug
cmake --build build
./build/td01
```

> [!TIP]
> Gardez vos vérifications dans le programme avec
> [`assert`](https://en.cppreference.com/cpp/error/assert) (`<cassert>`) :
> ```cpp
> assert(std::abs(resistor.voltage(0.002) - 2.0) < 1e-9); // <cmath>
> ```
> On utilise une tolérance pour comparer des nombres flottants. Un `assert`
> réussi n'affiche rien ; un échec arrête le programme. Compilez en mode Debug
> pour garder ces vérifications actives.

## 3. Durée de vie des objets

Ajoutez un affichage dans le constructeur et dans le destructeur de `Resistor`
(`~Resistor()`). Écrivez une fonction contenant :

```cpp
Resistor r1("R1", 1000.0);
{
    Resistor r2("R2", 2000.0);
}
Resistor r3("R3", 3000.0);
```

Dans quel ordre les résistances seront-elles détruites ? Appelez la fonction
pour vérifier.

Retirez ensuite les affichages et le destructeur : les champs de cette classe
se détruisent déjà automatiquement.

## 4. Allocation manuelle

Dans un programme séparé, allouez une résistance avec `new`, utilisez-la, puis
libérez-la avec `delete`. Compilez avec AddressSanitizer
(voir [l'installation](setup.md#4-vérifier-addresssanitizer)).

Essayez de lire la résistance après le `delete`. Repérez dans le diagnostic
la ligne de l'accès invalide et celle de la libération.

Si vous avez le temps, essayez aussi d'oublier `delete`, puis de l'appeler deux
fois. Faites ces essais séparément.

> [!NOTE]
> Un pointeur vers un objet détruit est un *pointeur pendant* (*dangling pointer*).
> Une allocation jamais libérée est une *fuite mémoire*. La détection des fuites
> dépend de la plateforme ; une absence de diagnostic ne garantit pas un code correct.

## 5. Gestion automatique avec RAII

Créez maintenant une résistance avec
[`std::make_unique`](https://en.cppreference.com/cpp/memory/unique_ptr/make_unique) :

```cpp
auto resistor = std::make_unique<Resistor>("R4", 4700.0); // <memory>
std::cout << resistor->description() << '\n';
```

Placez ce code dans un bloc et observez la destruction de la résistance à sa
sortie, en remettant temporairement l'affichage de l'exercice 3. Qui appelle
le destructeur cette fois-ci ?

Essayez de copier le `unique_ptr`. Lisez l'erreur de compilation, puis commentez
la ligne. Nous verrons au TD 3 comment transférer l'objet à un autre propriétaire.

## Pour aller plus loin

### Un tableau de tensions

Écrivez une fonction `void display_voltage(const Resistor&, double)` qui affiche
la tension pour un courant donné. Que changerait un passage de la résistance
par valeur ?

Utilisez-la dans une fonction `void display_voltages(const Resistor&)` pour
afficher les tensions de 0 à 10 mA, par pas de 0,5 mA, avec une boucle `for`.
Prenez un compteur entier et calculez le courant à chaque tour.

Créez ensuite un tableau de résistances de 1 kΩ, 2,2 kΩ et 4,7 kΩ, puis
affichez ce tableau de tensions pour chacune.

### Soigner l'affichage

Alignez les colonnes « I (mA) » et « U (V) » avec
[`std::setw`](https://en.cppreference.com/cpp/io/manip/setw).
Dans la documentation, trouvez l'en-tête à inclure. Faut-il appeler `setw`
une seule fois ou avant chaque valeur ? Vérifiez avec un petit essai.

### Un retour anticipé

Créez une résistance locale et une autre avec `make_unique` dans une fonction.
Ajoutez un `return` avant la fin et prédisez les destructions, puis vérifiez
avec les affichages du destructeur. Retirez-les après l'essai.

À votre avis, faudrait-il ajouter un `delete` avant ce `return` si la seconde
résistance avait été créée avec un `new` seul ?

### Posséder ou observer ?

Consultez [`std::unique_ptr`](https://en.cppreference.com/cpp/memory/unique_ptr).
Quelle méthode permet d'obtenir un pointeur vers l'objet sans le libérer ni
transférer sa possession ? Utilisez-la pour afficher la résistance.
Ce pointeur resterait-il utilisable après la destruction du `unique_ptr` ?

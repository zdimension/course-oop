# Préinstallation — C++ sous Linux avec VS Code

Installez les outils et compilez un premier programme avant de commencer le TD 1.

Les environnements acceptés sont : Linux natif, la VM Linux Polytech ou WSL sous
Windows. Les commandes ci-dessous supposent une distribution Ubuntu ou Debian récente
et un compte autorisé à utiliser `sudo`.

### Si vous souhaitez utiliser WSL sous Windows

Assurez-vous d'avoir WSL d'installé, si ce n'est pas le cas lancez un terminal en admin (Win+X puis A) et tapez 
```powershell
wsl --install
```

Suivez les instructions, choisissez un mot de passe, etc.

Dans VS Code, installez [l'extension WSL](https://marketplace.visualstudio.com/items?itemName=ms-vscode-remote.remote-wsl). Vous pourrez désormais soit
- ouvrir VS Code depuis un terminal WSL, dans le dossier courant, en tapant `code .`
- ouvrir VS Code dans WSL en lançant VS Code, puis en faisant F1 (ou Fn+F1) pour ouvrir la palette de commandes et en tapant WSL. Différentes commandes sont proposées, notamment "WSL in New Window" qui ouvre un VS Code tournant dans WSL.

## 1. Installer les outils de base

```bash
sudo apt update
sudo apt install build-essential cmake git gdb gnuplot curl zip unzip tar pkg-config
```

Vérifiez chaque outil :

```bash
g++ --version
cmake --version
make --version
git --version
gdb --version
gnuplot --version
```

Il faut un compilateur compatible C++20 et CMake 3.25 ou plus pour les presets
de la séance 5. CMake 3.20 suffit pour les premiers TD.

Make est installé avec `build-essential`. CMake générera les Makefiles :
vous n'aurez pas à les écrire à la main.

## 2. Préparer VS Code

Installez VS Code dans l'environnement depuis lequel vous ouvrirez les fichiers. Sous
WSL, installez aussi l'extension **WSL** côté Windows puis ouvrez le dossier avec
`code .` depuis le terminal WSL.

Extensions recommandées :

- **C/C++** — Microsoft ;
- **CMake Tools** — Microsoft.

## 3. Un premier programme

Créez un dossier :

```bash
mkdir -p ~/cpp-hello
cd ~/cpp-hello
```

Créez `main.cpp` :

```cpp
#include <iostream>

int main() {
    std::cout << "Hello C++!\n";
    return 0;
}
```

Créez `CMakeLists.txt` :

```cmake
cmake_minimum_required(VERSION 3.20)
project(cpp_hello LANGUAGES CXX)

add_executable(cpp_hello main.cpp) # construire l'exécutable à partir de main.cpp
target_compile_features(cpp_hello PRIVATE cxx_std_20) # compiler en C++20
```

Configurez, compilez et lancez :

```bash
cmake -S . -B build -G "Unix Makefiles" -DCMAKE_BUILD_TYPE=Debug
cmake --build build
./build/cpp_hello
```

> [!TIP]
> La première commande génère les Makefiles du projet dans `build`.
> La seconde lance Make, comme `make -C build` : relancez-la après chaque
> modification des sources.
> CMake régénère la configuration si nécessaire. Pour changer une option, comme
> le mode Debug, relancez la première commande.

Le terminal doit afficher `Hello C++!`. Sinon, lisez les messages d'erreur et
demandez de l'aide si vous restez bloqué.

## 4. Vérifier AddressSanitizer

Remplacez temporairement `main.cpp` par :

```cpp
int main() {
    int* values = new int[3];
    values[3] = 42; // indice en dehors du tableau !
    delete[] values;
}
```

Ce programme a un comportement indéfini : il peut sembler fonctionner, planter ou
faire autre chose. Une exécution sans message d'erreur ne permet pas de le valider.

Ajoutez à `CMakeLists.txt` :

```cmake
target_compile_options(cpp_hello PRIVATE -Wall -Wextra -Wpedantic
    -g -fno-omit-frame-pointer -fsanitize=address,undefined)
target_link_options(cpp_hello PRIVATE -fsanitize=address,undefined)
```

Recompilez et exécutez. Le programme doit produire un diagnostic contenant notamment
`AddressSanitizer`. Remettez ensuite le Hello World initial.

Ces options conviennent à GCC/Clang sous Linux. Exécutez les essais dans le
terminal, sans débogueur. Pour les réutiliser dans le TD, remplacez `cpp_hello`
par le nom de votre exécutable dans les deux lignes.

## 5. Préparer vcpkg — requis seulement à partir de la séance 5

Vous pouvez garder cette installation pour le TD 5.

```bash
git clone https://github.com/microsoft/vcpkg.git ~/vcpkg
~/vcpkg/bootstrap-vcpkg.sh
echo 'export VCPKG_ROOT="$HOME/vcpkg"' >> ~/.bashrc
echo 'export PATH="$VCPKG_ROOT:$PATH"' >> ~/.bashrc
source ~/.bashrc
vcpkg version
```

Le fichier `vcpkg.json` fourni au TD 5 demandera l'installation de Matplot++.

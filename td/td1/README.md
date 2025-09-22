# TD 1 : circuits électriques

On cherche à modéliser des composants électriques simples : résistances, condensateurs, inductances, et des circuits composés de ces composants.

Ces choses devraient a priori vous êtes familières, mais on rappellera les notions essentielles au fur et à mesure.

Le TD servira de fil rouge pour introduire progressivement les concepts de la POO mais aussi les fonctionnalités du langage C#.

À la fin du TD, votre code permettra de calculer des circuits simples tels que des ponts diviseurs ou des filtres RC.

> [!WARNING]
> ### Utilisation de LLMs (ChatGPT, ...)
>
> Les TD étant faits sur machine, je ne peux pas surveiller chaque personne. Vous interdire d'utiliser ChatGPT ou d'autres LLMs ne servirait pas à grand chose. Ils sont parfois utiles ! Je suis convaincu que la totalité des exercices de ce TD peuvent être faits par GPT, et si vous souhaitez le faire, libre à vous, mais *vous n'apprendrez rien*.
>
> Le but de ces TDs est de vous faire pratiquer la POO et le C#. Écrire du code vous-même, rester bloquer sur un souci 10 minutes, vous fera apprendre bien plus que de lire les réponses de GPT.
>
> Au demeurant, l'usage de GPT sera formellement interdit pendant les examens, et pour le coup ce sera surveillé. Si vous arrivez au DS après avoir utilisé GPT tout le semestre, tant pis pour vous...
>
> **À l'inverse,** chercher sur Google, StackOverflow, ou consulter la documentation officielle de C# pendant ces TD peut vous être très utile (mais je suis là aussi pour répondre à vos questions) car les réponses ne seront pas pré-mâchées.

> [!IMPORTANT]
> Par défaut, l'IDE affiche beaucoup d'avertissements qui ne sont pas très utile pour ce cours. À la racine de ce dépôt GitHub se trouve le fichier [.editorconfig](../../.editorconfig). Dès que vous créez un nouveau projet dans Rider, copiez ce fichier à la racine (à côté du fichier `Program.cs`) dans Rider (ou glissez-le dans la liste des fichiers).

> [!TIP]
> Chaque page correspond grosso modo à un exercice. Je vous recommande de créer des fonctions `void Exo1()`, `void Exo2()`, etc. dans `Program.cs` et d'appeler la fonction que vous voulez au moment voulu. Comme ça, **vous pouvez conserver** le code des exercices précédents.
>
> Si, plus loin dans le TD, un changement casse le code d'un exercice précédent, vous pouvez simplement le mettre en commentaires avec `/* ... */`.

Utilisez la barre de navigation à gauche pour naviguer dans le TD, en commençant par `p1.md`.
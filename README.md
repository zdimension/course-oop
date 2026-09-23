# EIESE781 — Programmation orientée objet en C++

Ce dossier contient les ressources du cours de programmation orientée objet en C++.
Le semestre suit un même projet : construire progressivement une petite bibliothèque
capable de modéliser des circuits R/L/C et de tracer leurs diagrammes de Bode.

## Organisation

- [`cm/`](cm/) : notes pour les présentations au tableau ;
- [`td/`](td/) : sujets de TD + [configuration logicielle](td/setup.md) ;
- [`checkpoints/`](checkpoints/) : corrections de TD, distribuées au fil des séances.

Chaque TD repart du TD précédent. Les corrections seront mises en ligne au fur et à mesure.

## Calendrier

| Séance | Durée | Thème | Objectifs |
|---|---:|---|---|
| 1 | 3 h 30 | Du C au C++, notion de durée de vie | classe `Resistor`, RAII |
| 2 | 3 h 30 | Héritage et polymorphisme | composants R/L/C |
| 3 | 3 h 30 | Collections et possession | `Series` et `Parallel` |
| 4 | 2 h + DS 1 | Enums, exceptions, invariants | modèle robuste |
| 5 | 3 h 30 | Interfaces et fonctions de transfert | pont diviseur, Bode RC |
| 6 | 3 h 30 | Composition et filtres RLC | quatre familles de filtres + les diagrammes qui vont avec |
| 7 | 3 h 30 | Intégration et architecture | simulateur final |
| 8 | 2 h + DS 2 | Durée de vie et contraintes embarquées | variante à taille fixe en C++11 |

Les 2 h des séances 4 et 8 comprennent un CM court, le TD, un temps de reprise et une
pause avant le DS de 1 h 30. Le **DS 1 porte sur les séances 1 à 3** ; le **DS 2 porte
sur les séances 1 à 7**. Les notions de séance 8 sont donc hors programme des DS.

## Habitudes à prendre

- Commencez par un objet local ou un membre stocké par valeur. Une valeur de type classe n'impose
  pas d'allocation dynamique.
- Utilisez de préférence les références plutôt que les pointeurs.
- Quand une allocation dynamique est utile, confiez-la à `std::unique_ptr`.
  Réservez `new` et `delete` aux exercices qui les étudient explicitement.
- Gardez les données privées lorsqu'une classe doit en garantir la cohérence.
  N'ajoutez pas automatiquement un setter pour chaque champ. Il ne doit y en avoir un que si cela a du sens dans le cas concerné.
- Compilez souvent.
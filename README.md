# Doliprane

`Doliprane` est une application personnelle conçue pour suivre facilement les prises de médicaments, éviter les erreurs de dosage et gérer les interactions entre traitements.

## Sommaire

- [Pourquoi faire ?](#pourquoi-faire-)
- [Spécification du langage `.med`](#spécification-du-langage-med)

## Pourquoi faire ?
Pendant près d'un an, les épisodes de fièvre et les prises de Doliprane sont devenus réguliers. Nous devions noter systématiquement l'heure de chaque prise pour savoir quand la suivante serait autorisée, et vérifier les éventuelles interactions entre plusieurs médicaments (par exemple Doliprane et Ibuprofène).

J'étais celui qui centralisait toutes ces informations et faisait les calculs à chaque fois. Lorsque je n'étais pas disponible, cela devenait vite compliqué.

L'idée de cette application est née de ce constat : pouvoir enregistrer facilement les horaires des prises et connaître automatiquement le moment où la prochaine dose est possible.

L'application a ensuite évolué :
- une jauge visuelle permet de savoir d'un coup d'œil si une nouvelle prise est autorisée,
- les interactions entre médicaments sont gérées,
- la posologie a été adaptée à nos cas personnels (par exemple, pas d'obligation d'attendre 8 heures entre deux prises de Doliprane en l'absence de problèmes rénaux).

Pour simplifier la gestion des posologies et des interactions, j'ai même créé un petit langage permettant de décrire les règles propres à chaque médicament et leurs liens entre eux.

# Spécification du langage `.med`

Les fichiers `.med` définissent les règles de posologie et d'interactions d'un médicament.
Ils sont utilisés par l'application pour déterminer automatiquement si une prise est possible ou non.

Voici un exemple d'utilisation avec le médicament "Doliprane" :

```
MEDICAMENT Doliprane

  POSOLOGIE Adulte SUR 24h
    DOSAGE Paracetamol
      0-3000 : Oui
      3000-4000 : Avertissement
      4000+ : Non
    ATTENDRE APRES Diosmectite
      0-2 : Non
      2+  : Oui
    ATTENDRE APRES Paracetamol
      0-4 : Non
      4-6 : Possible
      6+  : Oui
    ATTENDRE APRES Ibuprofene
      0-3 : Non
      3-4 : Avertissement
      4+  : Oui
      
  POSOLOGIE Enfant SUR 24h
    PRISE Paracetamol
      0-3 : Oui
      4 : Avertissement
      4+ : Non
    ATTENDRE APRES Diosmectite
      0-2 : Non
      2+ : Oui
    ATTENDRE APRES Paracetamol
      0-6 : Non
      6+ : Oui
    ATTENDRE APRES Ibuprofene
      0-3 : Non
      3-4 : Avertissement
      4+ : Oui
      
FIN
```

Cet exemple illustre bien toutes les règles du langage.\
La documentation ci-dessous explique chaque élément en détail.

**<ins>Structure générale d'un fichier `.med`</ins>**

Un fichier peut contenir un ou plusieurs blocs `MEDICAMENT` :

```
MEDICAMENT NomDuMédicament
  POSOLOGIE ...
    (règles)
FIN
```

Règles importantes :
- Un bloc commence par `MEDICAMENT Nom`.
- Il se termine obligatoirement par `FIN`.
- Le nom du médicament doit correspondre à un médicament connu de l'application.
- L'indentation et les sauts de lignes sont libres.
- Une ligne qui commence par `#` est un commentaire.

**<ins>Structure d'un bloc `MEDICAMENT`</ins>**

Chaque médicament doit contenir au moins une section `POSOLOGIE`, et toutes ces règles associés doivent être sous cette section.

Structure :

```
MEDICAMENT Nom
  POSOLOGIE Type SUR Durée
    (règles ATTENDRE APRES / PRISE / DOSAGE)
FIN
```

**<ins>Structure d'un bloc `POSOLOGIE`</ins>**

La posologie indique sur combien d'heures les règles s'appliquent.

Exemple de format :

```
POSOLOGIE Adulte SUR 24h
POSOLOGIE Enfant SUR 20h
```

Contraintes :
- Types possibles : Adulte, Enfant
- Durée possible : 24h ou 20h

_La durée de 20h est utilisée pour les médicaments prescrits en prises réparties par périodes — matin, midi, soir — plutôt qu’à intervalles stricts._

**<ins>Règles à l'intérieur d'une `POSOLOGIE`</ins>**

Trois types de règles sont possibles :

|Type de règle|Signification|
|-|-|
|ATTENDRE APRES X|Temps à attendre après le médicament X|
|PRISE X|Peut-on prendre X avant/avec celui-ci ?|
|DOSAGE X|Autorisation selon le dosage de X|

_Chaque règle porte le nom d'un médicament ou d'un composé (ex : Diosmectite, paracétamol) connu de l'application._

**<ins>Format des plages</ins>**

Les règles utilisent des plages de la forme :

```
borneMin-borneMax : Valeur
borne             : Valeur
borne+            : Valeur
```

Exemples de format :

```
0-2 : Non
2-4 : Avertissement
5   : Possible
5+  : Oui
```

Règles obligatoires :
- La première plage doit commencer par 0.
- La dernière plage doit se terminer par un "+" (exemple 4+).
- Chaque plage suivante doit commencer par la valeur de fin de la précédente.
  - Exemple : 0-2, puis 2-4.
  - Exception s'il y a qu'une seule valeur, Exemple : 5 qui est équivalent à la plage 4-5.
- Il faut au minimum 2 lignes de plages pour `PRISE`et `DOSAGE`, mais au moins 1 ligne pour `ATTENDRE APRES`.
- Espaces optionnels autour du `:`.
- Les valeurs autorisées sont `Non`, `Avertissement`, `Possible` ou `Oui`

|Règle|Ordre|Borne|
|-|-|-|
|`ATTENDRE APRES`|1. Non <br> 2. Avertissement <br> 3. Possible <br> 4. Oui|min <= `valeur` < max|
|`PRISE` <br> `DOSAGE`|1. Oui <br> 2.Avertissement <br> 3.Non|min < `valeur` <= max|

Contraintes :
- La première valeur pour `ATTENDRE APRES` est `Non`.
- La première valeur pour `PRISE` ou `DOSAGE` est `Oui`.

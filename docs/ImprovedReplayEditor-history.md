# Timeline FX v8 : keyframes d'effets sur leur propre timeline

Installation : copie `IRE.dll` dans `<jeu>\JibbersMods\cs\` (remplace l'ancienne) puis relance le jeu.

- Une 2e timeline (fine barre sombre avec des points cyan) apparait SOUS la timeline du jeu : ce sont les keyframes d'effets,
  separes des keyframes camera du jeu. Le point blanc est celui proche de la tete de lecture. Distance reglable (Options > Mods > Timeline).
- Edition : section "Timeline FX" tout en bas du menu Camera Effects (et du panneau "Keyframe #N").
  Place la tete de lecture, bouge un curseur : une keyframe d'effets est creee a cette frame. "Delete the effect keyframe here" la supprime.
  Contenu : heure du jour, camera shake (force, vitesse, douceur, rotation, position, roulis), et tous les curseurs d'effets d'image.
- Valeurs interpolees entre keyframes d'effets. Sauvegarde par clip dans JibbersMods\timeline_fx\ (anciens fichiers v5-v7 importes).

v13 : plus de section Timeline FX dans le menu Camera Effects. Dans le panneau "Keyframe #N" : les reglages de base du jeu sont ranges en onglets
(Timing, Path & Transitions, Position & Look, Lens) et les reglages Timeline FX en onglets comme le menu Camera Effects
(Time of day, Camera Shake, Lens & Focus, Exposure & Tone, Color, Stylize).

v15 : l icone d une keyframe d effets est un rond divise en parts egales (une part par onglet utilise) : orange = Time of day, rouge = Camera Shake, bleu = Lens & Focus,
jaune = Exposure & Tone, violet = Color, vert = Stylize. Gris = keyframe vide. Fond blanc = keyframe sous la tete de lecture.

v16 : en quittant le replay, tout est reinitialise (effets d'image restaures, caches et menus oublies) : ouvrir un autre replay ne reutilise plus d'objets du precedent.
Les boutons "+ Effect keyframe" et "Delete" sont retires : bouge un curseur pour creer une keyframe d'effets ; F10 (modifiable) supprime celle sous la tete de lecture.
Les reglages de base du panneau Keyframe sont ranges en onglets (detection par nom, sinon par position).

## v17
- Un bouton « Delete <onglet> keyframe here » par onglet (Time of day, Camera Shake, Lens & Focus, Exposure & Tone, Color, Stylize) : supprime uniquement les options de cet onglet sur la keyframe sélectionnée. La keyframe disparaît quand il ne lui reste plus rien. F10 supprime toujours toute la keyframe.
- Regroupement des options de base du jeu : le nom des lignes est maintenant lu depuis le texte affiché. Si ça ne regroupe toujours pas, envoyer jibbers_hotload.log (ligne « TLFX: keyframe panel has N top-level rows »).

## Renommage
Le DLL s'appelle maintenant IRE.dll ; nom affiché en jeu : Improved Replay Editor. Supprime l'ancien TimelineFx.dll de JibbersMods\cs.

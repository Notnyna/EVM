# Disclaimer
This mod is written incompatible with [Predators Swallow](https://steamcommunity.com/workshop/filedetails/?id=3163169357) as it now includes it, but having both should cause no issue.


## Changes from original
*Do not have the time or expertise to make a new mod extension, so I did some modifications to the library instead, gomen :p
- Added [gene Predator] with abilities Devour and Stun.
- [Stun] allows to incapacitate or paralyze pawns for a chance to strip them or eliminate them without killing. Success depends on melee skill, psychic sensitivity and damagefactor.
- [Devour] allows to swallow nearby pawn after a prey dodge check
- Added [mass check] - prey must not weigh more than the predator mass x mawsize *disabled by default in settings. Do you wanna swallow a pawn in marine armour?
- Added [contents ignore check] - predator cannot endlessly stuff prey inside him beyond what his bodysizeXmawsize allows
- You can vomit out prey by right clicking the predator pawn.


- Created a tale about swallowing - now you can create a sculpture about it.
- Added thoughts for others about swallowing other pawns and being swallowed, normal pawns are terrified of maneaters.
*Has a few references to Anomaly due to lazyness (rulepackdef, taledef, stun ability icon), which can be easily edited out. Why would you play without anomaly tho?
- Digestion and devour now has sounds.
*Sounds taken from EchidnaWarsDX, drug texture from Food Poisoning Cures, devour icon from Big and Small framework.
- Devouring now takes time and stuns initially and then staggers prey depending on bodysize difference.
* If you're lucky there is a window where you can still escape!

- Digestion 'Fatal'
*Nutrition from tick now works. Now also takes into account digestion efficiency and metabolism
- Reduce your metabolism by ingesting a new [drug] called [gut neutralizer] other name [heartburn]. Taking it too much will make you puke out the prey so be warned.
- By default the damage from digestion kills a normal pawn way too fast
*xml reduced efficiency to 0.2
- Struggling prey no longer yeet themselves to a devour dimension. They eject themselves before container (usually stomach) is destroyed. Temporary solution just makes the container def undestroyable while it has content. 
*Possible side effect - every pawn suddenly have indestructible stomachs...

- Prey now increases mass of the predator. The display is a bit bugged with multiple prey.
*A whole bunch of minor changes made to make abilities work I did not bother to remember.

# Patches:
- Patched animal thinktree to randomly use devour ability
- [Vanilla expanded animals] - Anaconda buffed because why not. Optionally commented out section to also make them draftable.
*Because I like my pet anaconda.
- Manhunters patched to allow for devour ability... Enjoy the manhunter anaconda raids.
*Animals will have devour ability only if you increase their mawsize higher than default! Game restart needed.

# To fix:
- Do not *under any circumstances* devour a pawn who has devourd a pawn!
- Reset ability cooldown on devour fail
- Make a reset button for menu settings
- Thoughts on pawns for eating their pets
- Placeholder text can still be seen in detailed screen
- Weird float behavior in settings screen, no clue why it doesn't behave properly, copy paste works though
- Fix Harmony patch of predator hunts, something bugs me - they do not actually devour!
- A safe surgery option to extract contents of stomach. Because my pet anaconda sometimes swallows things I'd like to keep alive.
- Harmony patch for normal cases where devouring is applicable
- Try something hacky or harmony patch to ensure contained prey are not deleted by accident and to avoid invulnerability changes to bodypart defs!
- MeleeDodgeChance does not take into account Verb supriseattack? How to access it? MVCF
- Deaths while being digested does not change the reference prey, what even are corpses and where do they appear from?
- Most likely made devouring rocks and items broken

# Needs testing
- Damage values for fatal stomach need tweaking, especially if playing with combat extended
- Hediff changes on longer digestive tracks, actually, should test ALL functionality of bigger digestive tracks, likely broken
- Surgery and external damage on digestive organ while pawns still inside
- Animal Pawn Pawn animal tales and thoughts, are animals even pawns? are pawns animals?
- Does death by digestion registers as killing for pawns?

# Overview
This is mostly a library but it also includes Predators Swallow now, which makes so if they can, predators will swallow their prey whole. Maw size and if this feature is active are configurable. 

I was tired of adding RedMattis' [Lamia mod](https://steamcommunity.com/workshop/filedetails/?id=2908225858) as a dependency to mods that should not have any (except Harmony maybe). RedMattis' mods are amazing, but his Engulf gene is a gene, locking it to Biotech and it currently only allows Fatal and Healing. Mine should allow custom logic, although only Fatal was tested yet. 

This mod is still in beta. 

# How does this mod work?
Jaw
- ~~Added field mawSize. This is used to see if you can swallow this creature whole. (there is an option to ignore this part)~~
This was removed to confugure like from [Predators Swallow](https://steamcommunity.com/workshop/filedetails/?id=3163169357)

Stomach<br/>
Added the following fields.
- baseDamage: damage dealt per digestion tick = baseDamage * digestionEfficiancy * metabolism level
- digestionEfficiancy: damage dealt per digestion tick = baseDamage * digestionEfficiancy * metabolism level
- comfort: for those who are not being digested, how good of a bed is this place?
- deadline: time before food move to the next stage of your digestive track
- grantsNutrition: does this stomach feed you?
- nutritionCost: does this stomach takes nutrition rather than feeding you?
- digestionWorker: logic used
- digestionDamageType: type of damage your digestion applies
Added the name tag "DefaultStomach" to the stomach, hoping to help crossmod compatibility

Body<br/>
Added a default digestive track to the human body.

# Digestive Track?
A body can have multiple digestive tracks. Each must have a Purpose and a list of the body parts a prey must go through before being freed. All those body parts must have the fields mentioned in Stomach or they will default. 

If you have the devour options activated, when you right click on a pawn while having one of your pawns selected, you have the option of in which digestive track you want to send them, while if you right click your colonist after, you can regurgitate. 

# Digestion Worker
I made the following digestion worker so far.
- DigestionWorker_Fatal
- DigestionWorker_Heal
- DigestionWorker_HealScars
- DigestionWorker_JoyPred
- DigestionWorker_JoyPrey
- DigestionWorker_Mend
- DigestionWorker_Regenerate
- DigestionWorker_Safe
- DigestionWorker_Tend

If you want to make your own, add this mod as a library and implement DigestionWorker in your class. 

# Plans for the future
- Biotech features
- A tutorial

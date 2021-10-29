using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PC_ShapeShifter : MonoBehaviour
{
    public DynamicMap Sshifter_DynM;
    public PlayableCharacter Sshifter_PC;
    public PC_ShapeShifter Sshifter;
    public Button TransformButton;
    public bool Predator = false;
    public int PredatorCountdown = 0;
    int predatorBoost_potential;
    int predatorBoost_armor;
    int goliathBoost;
    int armorMemory;
    int damageMemory;
    public int shield;
    public int potential;


    public void initializeShapeShifter(PlayableCharacter playable, PC_ShapeShifter shapeShifter, DynamicMap M)
    {
        Sshifter_PC = playable;
        Sshifter = shapeShifter;
        Sshifter_DynM = M;
        //Character Start Attribiutes
        Sshifter_PC.MAX_ActionPoints = 6;
        Sshifter_PC.ActionPoints = 6;
        Sshifter_PC.MovementCost = 2;
        Sshifter_PC.AttackRange = 1;
        Sshifter_PC.MAX_Health = 50;
        Sshifter_PC.Health = 50;
        Sshifter_PC.Armor = 2;
        Sshifter_PC.HealthUpgradeCost = 150;
        Sshifter_PC.DamageUpgradeCost = 250;
        Sshifter_PC.ArmorUpgradeCost = 250;
    }

    public void disableTransformButton()
    {
        TransformButton.gameObject.SetActive(false);
    }
    public void enableTransfromButton()
    {
        if (((!Sshifter_PC.playerActionMenuMode) || (!Sshifter_PC.attackMenuMode) || (!Sshifter_PC.moveMenuMode)) && (!Predator) && (Sshifter_PC.ActionPoints == Sshifter_PC.MAX_ActionPoints))
            TransformButton.gameObject.SetActive(true);
    }

    public void Ability_Transform()
    {
        /*if (Sshifter_PC.Cursed)
        {

            Sshifter_PC.curseTick();

        }*/
        Predator = true;
        TransformButton.gameObject.SetActive(false);
        PredatorCountdown = 3;
        Debug.Log("OH SHIT, RUN!!!");
        //shield
        shield = potential;
        //damage boost and armor loss
        Sshifter_PC.Armor = Sshifter_PC.Armor - goliathBoost;
        goliathBoost = 0;

        predatorBoost_armor = (Sshifter_PC.Armor / 2);
        armorMemory = Sshifter_PC.Armor; //storing armor to return later
        Sshifter_PC.Armor = 0;

        predatorBoost_potential = (potential / 3);
        potential = 0;

        Sshifter_PC.DamageBonus = Sshifter_PC.DamageBonus + predatorBoost_armor + predatorBoost_potential;

        //Speed boost
        Sshifter_PC.MovementCost = 1;
    }

    public void Revert()
    {
        Predator = false;
        TransformButton.gameObject.SetActive(true);
        Debug.Log("Oh, ok, we're not all dead.");
        //shield expiration
        shield = 0;
        //damage boost removal and and armor restoration
        Sshifter_PC.DamageBonus = Sshifter_PC.DamageBonus - predatorBoost_potential;
        Sshifter_PC.DamageBonus = Sshifter_PC.DamageBonus - predatorBoost_armor;
        predatorBoost_potential = 0;
        predatorBoost_armor = 0;

        goliathBoost = (Sshifter_PC.DamageBonus / 2);
        damageMemory = Sshifter_PC.DamageBonus;
        Sshifter_PC.DamageBonus = 0;

        Sshifter_PC.Armor = armorMemory;
        armorMemory = 0;

        Sshifter_PC.Armor = Sshifter_PC.Armor + goliathBoost;

        //Removal of speed boost
        Sshifter_PC.MovementCost = 2;
    }

    public void DamageBuyUpdate() //recalculating damage bonus when buying damage in Goliath form
    {
        damageMemory++;
        Sshifter_PC.DamageBonus = 0;
        Sshifter_PC.Armor = Sshifter_PC.Armor - goliathBoost;
        goliathBoost = (damageMemory / 2);
        Sshifter_PC.Armor = Sshifter_PC.Armor + goliathBoost;
    }

    public void ArmorBuyUpdate() //recalculating damage bonus when buying armor in Predator form
    {
        armorMemory++;
        Sshifter_PC.Armor = 0;
        Sshifter_PC.DamageBonus = Sshifter_PC.DamageBonus - predatorBoost_armor;
        predatorBoost_armor = (armorMemory / 2);
        Sshifter_PC.DamageBonus = Sshifter_PC.DamageBonus + predatorBoost_armor;
    }

    public void Shapeshifter_StartTurnConditions()
    {
        if (Predator)
        {
            PredatorCountdown--;
            if (PredatorCountdown == 0)
            {
                Revert();
            }
        }
        else
        {
            TransformButton.gameObject.SetActive(true);
        }
    }

    public void Shapeshifter_EndTurnConditions()
    {
        TransformButton.gameObject.SetActive(false);
        Sshifter_PC.Health = Sshifter_PC.Health + (potential / 2);
        potential = 0;
    }
    void LateUpdate()
    {
        if (Sshifter_PC.ActionPoints < Sshifter_PC.MAX_ActionPoints)
        {
            TransformButton.gameObject.SetActive(false);
        }
    }

    public void Predator_Attack(PlayableCharacter Victim) //in case generic attack is changed
    {
        if (Sshifter_PC.ActionPoints >= Sshifter_PC.AttackActionCost)
        {
            Sshifter_PC.DamageBonus = Sshifter_PC.DamageBonus + Sshifter_PC.specialDamageBonus;
            //RNG Set
            Sshifter_PC.RNGeezus.numberOfDice = 1;
            Sshifter_PC.RNGeezus.diceMax = 8;
            //RNG call
            Sshifter_PC.RNGeezus.Damage_Roll();
            //Set attack properties
            Sshifter_PC.RNGeezus.missOn = 2;
            Sshifter_PC.RNGeezus.armorPierce = false;
            //Damage calculation
            Sshifter_PC.Target_ArchetypeSwitch(Victim, Sshifter_PC);
            Sshifter_PC.DamageBonus = Sshifter_PC.DamageBonus - Sshifter_PC.specialDamageBonus;
            if (Sshifter_PC.ActionPoints == 0)
            {
                Sshifter_PC.Deselect();
            }
        }
        else
        {
            Debug.Log("Not enough Action Points");
        }
    }

    public void Goliath_Attack(PlayableCharacter Victim)
    {
        if (Sshifter_PC.ActionPoints >= Sshifter_PC.AttackActionCost)
        {
            Sshifter_PC.DamageBonus = Sshifter_PC.Armor + Sshifter_PC.specialDamageBonus;
            //RNG Set
            Sshifter_PC.RNGeezus.numberOfDice = 2;
            Sshifter_PC.RNGeezus.diceMax = 4;
            //RNG call
            Sshifter_PC.RNGeezus.Damage_Roll();
            //Set attack properties
            Sshifter_PC.RNGeezus.missOn = 2;
            Sshifter_PC.RNGeezus.armorPierce = false;
            //Damage calculation
            Sshifter_PC.Target_ArchetypeSwitch(Victim, Sshifter_PC);

            Sshifter_PC.DamageBonus = Sshifter_PC.specialDamageBonus;
            if (Sshifter_PC.ActionPoints == 0)
            {
                Sshifter_PC.Deselect();
            }
        }
        else
        {
            Debug.Log("Not enough Action Points");
        }
    }

    public void Goliath_TakeDamage(PlayableCharacter Agressor) //This was created to simplify damage calculations and character specific cases...
    {
        int rawDamage = Agressor.RNGeezus.Result + Agressor.DamageBonus;
        //check armor piercing
        int effectiveArmor = Sshifter_PC.Armor + Sshifter_PC.specialArmor;
        if (Agressor.RNGeezus.armorPierce && !Sshifter_PC.trueArmor)
        {
            effectiveArmor = 0;
        }
        else if (Agressor.RNGeezus.trueDamage)
        {
            effectiveArmor = 0;
        }
        //this is used later
        int effectiveDamage = 0;

        //check to see if the enemy missed
        int hitCheck = 0;
        for (int i = 0; i < Agressor.RNGeezus.numberOfDice; i++)
        {
            if (Agressor.RNGeezus.diceResult[i] > Agressor.RNGeezus.missOn)
            {
                hitCheck++;
            }
        }

        if (hitCheck == 0)
        {
            Debug.Log("You missed!");
            Agressor.RNGeezus.isHit = false;
        }
        else
        {
            Agressor.RNGeezus.isHit = true;
            effectiveDamage = rawDamage - effectiveArmor;
            //Chip damage
            if (effectiveDamage <= 0)
            {
                Agressor.RNGeezus.isChip = true;
                effectiveDamage = 1;
                Sshifter_PC.Health = Sshifter_PC.Health - 1;
                Debug.Log("You chip for 1 damage...");
            }
            //Kill damage
            else if (effectiveDamage >= Sshifter_PC.Health)
            {
                Agressor.RNGeezus.isChip = false;
                
                Agressor.VictoryPoints = Agressor.VictoryPoints + 2;
                if (Agressor.GetComponent<PC_Headhuntress>() != null)
                {
                    Agressor.VictoryPoints = Agressor.VictoryPoints + 3;
                }
                effectiveDamage = Sshifter_PC.Health;
                Sshifter_PC.Health = 0;
                Debug.Log("You have killed your target");
            }
            else if (effectiveDamage < Sshifter_PC.Health)
            {
                Agressor.RNGeezus.isChip = false;
                Sshifter_PC.Health = Sshifter_PC.Health - effectiveDamage;
                
                Debug.Log("You did " + effectiveDamage + " damage!");
            }
            potential = potential + effectiveDamage;

            if (Sshifter_PC.Health == 0)
            {
                Sshifter_PC.currentTile.whoIsOnTile = isOnTile.empty;
                //Agressor.FindValidMovement();
                this.gameObject.SetActive(false); //[PRONE TO EXPLODING]
            }
            else if (Sshifter_PC.Health < 0)
            {
                Debug.Log("WTF, negative health?");
            }
            Debug.Log("That tickles");
        }
    }

    public void Predator_TakeDamage(PlayableCharacter Agressor) //This was created to simplify damage calculations and character specific cases...
    {
        int rawDamage = Agressor.RNGeezus.Result + Agressor.DamageBonus;
        //check armor piercing
        int effectiveArmor = Sshifter_PC.specialArmor;
        if (Agressor.RNGeezus.armorPierce && !Sshifter_PC.trueArmor)
        {
            effectiveArmor = 0;
        }
        else if (Agressor.RNGeezus.trueDamage)
        {
            effectiveArmor = 0;
        }
        //this is used later
        int effectiveDamage = 0;

        //check to see if the enemy missed
        int hitCheck = 0;
        for (int i = 0; i < Agressor.RNGeezus.numberOfDice; i++)
        {
            if (Agressor.RNGeezus.diceResult[i] > Agressor.RNGeezus.missOn)
            {
                hitCheck++;
            }
        }

        if (hitCheck == 0)
        {
            Debug.Log("You missed!");
            Agressor.RNGeezus.isHit = false;
        }
        else
        {
            Agressor.RNGeezus.isHit = true;
            effectiveDamage = rawDamage - effectiveArmor; //[IMPORTANT] change shield damage calculations from effectieDamage to rawDamage
            //Chip damage
            if (effectiveDamage <= 0)
            {
                effectiveDamage = 1;
                if (shield >= 1)
                {
                    shield = shield - 1;
                }
                else
                {
                    Sshifter_PC.Health = Sshifter_PC.Health - 1;
                }
                Agressor.RNGeezus.isChip = true;
                Debug.Log("You chip for 1 damage...");
            }
            //Kill damage
            else if (effectiveDamage >= (Sshifter_PC.Health + shield))
            {
                Agressor.RNGeezus.isChip = false;
                Agressor.VictoryPoints = Agressor.VictoryPoints + 2;
                
                if (Agressor.GetComponent<PC_Headhuntress>() != null)
                {
                    Agressor.VictoryPoints = Agressor.VictoryPoints + 3;
                }
                effectiveDamage = Sshifter_PC.Health;
                Sshifter_PC.Health = 0;
                shield = 0;
                Debug.Log("You have killed your target");

            }
            else if (effectiveDamage < (Sshifter_PC.Health + shield))
            {
                if (shield >= effectiveDamage)
                {
                    Agressor.RNGeezus.isChip = true;
                    Debug.Log("You did " + effectiveDamage + " damage!");
                    shield = shield - effectiveDamage;
                    effectiveDamage = 0;
                }
                else
                {
                    Agressor.RNGeezus.isChip = false;
                    Debug.Log("You did " + effectiveDamage + " damage!");
                   
                    Sshifter_PC.Health = Sshifter_PC.Health - (effectiveDamage - shield);
                    effectiveDamage = effectiveDamage - shield;
                    shield = 0;
                }
            }

            if (Sshifter_PC.Health == 0)
            {
                Sshifter_PC.currentTile.whoIsOnTile = isOnTile.empty;
                //Agressor.FindValidMovement();
                this.gameObject.SetActive(false); //[PRONE TO EXPLODING]
            }
            else if (Sshifter_PC.Health < 0)
            {
                Debug.Log("WTF, negative health?");
            }
        }
    }
}

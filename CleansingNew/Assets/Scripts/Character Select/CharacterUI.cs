using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUI : MonoBehaviour                                     //This script changes the character pictures
{
    [Header("Images")]
    [SerializeField] private Image TankImage = null;
    [SerializeField] private Image SoliderImage = null;
    [SerializeField] private Image HealerImage = null;

    [Header("Characters")]
    [SerializeField] private Sprite TankCharacter = null;
    [SerializeField] private Sprite SoliderCharacter = null;
    [SerializeField] private Sprite HealerCharacter = null;

    [Header("Icons")]
    [SerializeField] private Sprite TankIcon = null;
    [SerializeField] private Sprite SoliderIcon = null;
    [SerializeField] private Sprite HealerIcon = null;

    public void ChangeSoliderToCharacter()
    {
        SoliderImage.sprite = SoliderCharacter;
    }
    public void ChangeSoliderToIcon()
    {
        SoliderImage.sprite = SoliderIcon;
    }

    public void ChangeTankToCharacter()
    {
        TankImage.sprite = TankCharacter;
    }

    public void ChangeTankToIcon()
    {
        TankImage.sprite = TankIcon;
    }

    public void ChangeHealerToCharacter()
    {
        HealerImage.sprite = HealerCharacter; 
    }

    public void ChangeHealerToIcon()
    {
        HealerImage.sprite = HealerIcon;
    }
}

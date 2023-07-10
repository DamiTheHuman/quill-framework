using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ActBGM", order = 4)]
public class ActBGMScritpableObject : ScriptableObject
{
    [Tooltip("A pool of all the main sounds that can be played")]
    public List<SoundData> BGMSounds = new List<SoundData> {
        new SoundData
        {
            soundTag = BGMToPlay.MainBGM ,
            name = General.TransformSpacesToUpperCaseCharacters(BGMToPlay.MainBGM.ToString())
        },
        new SoundData
        {
            soundTag = BGMToPlay.BossTheme ,
            name = General.TransformSpacesToUpperCaseCharacters(BGMToPlay.BossTheme.ToString())
        },
        new SoundData
        {
            soundTag = BGMToPlay.ActClearJingle ,
            name = General.TransformSpacesToUpperCaseCharacters(BGMToPlay.ActClearJingle.ToString())
        },
        new SoundData
        {
            soundTag = BGMToPlay.InvincibilityTheme ,
            name = General.TransformSpacesToUpperCaseCharacters(BGMToPlay.InvincibilityTheme.ToString())
        },
        new SoundData
        {
            soundTag = BGMToPlay.DrowningTheme ,
            name = General.TransformSpacesToUpperCaseCharacters(BGMToPlay.DrowningTheme.ToString())
        },
        new SoundData
        {
            soundTag = BGMToPlay.PowerSneakers ,
            name = General.TransformSpacesToUpperCaseCharacters(BGMToPlay.PowerSneakers.ToString())
        },
        new SoundData
        {
            soundTag = BGMToPlay.ExtraLifeJingle ,
            name = General.TransformSpacesToUpperCaseCharacters(BGMToPlay.ExtraLifeJingle.ToString())
        },
        new SoundData
        {
            soundTag = BGMToPlay.SuperFormTheme ,
            name = General.TransformSpacesToUpperCaseCharacters(BGMToPlay.SuperFormTheme.ToString())
        }
    };

    private void OnValidate()
    {
        foreach (SoundData sound in this.BGMSounds)
        {
            sound.name = General.TransformSpacesToUpperCaseCharacters(sound.soundTag.ToString());
        }
    }

}

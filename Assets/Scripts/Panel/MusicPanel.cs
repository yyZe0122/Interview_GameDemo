using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicPanel : BasePanel
{
    private Slider sliderMusic;
    private Slider sliderSound;
    MusicData musicData = GameDataMgr.Instance.musicData;
    public override void ShowMe()
    {
        sliderMusic = GetControl<Slider>("sliderMusic");
        sliderSound = GetControl<Slider>("sliderSound");
        sliderMusic.value = musicData.MusicValue;
        sliderSound.value = musicData.SoundValue;
    }

    public override void HideMe()
    {
        
    }

    protected override void ClickButton(string buttonName)
    {
        switch (buttonName)
        {
            case "btnMusicExit":
                UIMagr.Instance.HidePanel<MusicPanel>();
                GameDataMgr.Instance.SaveMusicData();
                break;
        }
    }

    protected override void ClickSlider(string sliderName, float value)
    {
        switch (sliderName)
        {
            case "sliderMusic":
                MusicMgr.Instance.ChangeMusicValue(value);
                musicData.MusicValue = value;
                break;
            case "sliderSound":
                MusicMgr.Instance.ChangeSoundValue(value);
                musicData.SoundValue = value;
                break;
        }
    }
}

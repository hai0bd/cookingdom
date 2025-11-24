using HuyThanh.Cooking.Burrito;
using Link;
using System.Collections;
using UnityEngine;

public class Pan : ItemIdleBase
{
    public enum State
    {
        Normal,
        HeatOn, // bat lua len roi
        HaveOil,
        HaveOnion,
        HaveGarlic,
        Mixing1,
        Meat,
        Pepper,
        Chili,
        Salt,
        Turmeric,
        Tomato,
        Oregano,
        Mixing2,
        DoneMix2,
        WaitForTurnOff,
        Done
    }

    [SerializeField] State state;

    [SerializeField] Animation anim;
    [SerializeField] string animOil, animMeat, animTomato;

    [Header("Drop2D")]
    [SerializeField] Drop2D onionDrop2D;
    [SerializeField] Drop2D garlicDrop2D;
    [SerializeField] Drop2D pepperDrop2D;
    [SerializeField] Drop2D chiliDrop2D;
    [SerializeField] Drop2D saltDrop2D;
    [SerializeField] Drop2D turmericDrop2D;
    [SerializeField] Drop2D oreganoDrop2D;

    [Header("Item Alpha")]
    [SerializeField] ItemAlpha rawMeatAlpha;
    [SerializeField] ItemAlpha tomatoSauceAlpha;
    [SerializeField] ItemAlpha mixMeatAlpha;
    [SerializeField] ItemAlpha doneMeatAlpha;

    [Header("Spice Item Fry Control")]
    [SerializeField] SpiceItemFryControl onionFryControl;
    [SerializeField] SpiceItemFryControl garlicFryControl;
    [SerializeField] SpiceItemFryControl pepperFryControl;
    [SerializeField] SpiceItemFryControl chiliFryControl;
    [SerializeField] SpiceItemFryControl saltFryControl;
    [SerializeField] SpiceItemFryControl turmericFryControl;
    [SerializeField] SpiceItemFryControl oreganoFryControl;
    [Header("VFX")]
    [SerializeField] ParticleSystem oilVFX;
    [SerializeField] ParticleSystem smokeVFX;
    [SerializeField] ParticleSystem blinkVFX;

    [Header("Meat Sprite")]
    [SerializeField] Transform meatGo;

    [SerializeField] HintText hintText_garlic, hintText_addMeat, hintText_addSpice;


    float[] sizeFloat = { 0.75f, 0.5f, 0f };
    int takeMeatCount = 0;

    float cookingRate = 0;
    public override bool IsState<T>(T t)
    {
        return state == (State)(object)t;
    }

    public override void ChangeState<T>(T t)
    {
        state = (State)(object)t;

        switch (state)
        {
            case State.HaveOil:
                anim.Play(animOil);
                oilVFX.Play();
                break;
            case State.HaveOnion:
                onionDrop2D.OnActive();
                break;
            case State.HaveGarlic:
                garlicDrop2D.OnActive();
                ChangeState(State.Mixing1); // Change to Mixing state after adding garlic
                break;

            case State.Meat:
                hintText_garlic.OnActiveHint();
                LevelControl.Ins.NextHint();
                blinkVFX.Play();
                break;
            case State.Pepper:
                hintText_addMeat.OnActiveHint();
                break;
            case State.Mixing2:
                hintText_addSpice.OnActiveHint();
                break;
            case State.DoneMix2:
                LevelControl.Ins.NextHint();
                blinkVFX.Play();
                SoundControl.Ins.PlayFX(Fx.DoneSomething);
                smokeVFX.Play();
                break;
            case State.WaitForTurnOff:

                smokeVFX.Stop();
                break;
            case State.Done:
                LevelControl.Ins.NextHint();
                LevelControl.Ins.CheckStep(1f); // Check step completion
                oilVFX.Stop();
                break;
        }
    }

    public override bool OnTake(IItemMoving item)
    {
        if (item is BowlPur && item.IsState(BowlPur.State.Normal) && this.IsState(State.HeatOn))
        {
            item.OnMove(TF.position + Vector3.up * 0.4f + Vector3.left * 0.15f, Quaternion.identity, 0.2f);
            item.ChangeState(BowlPur.State.Pouring);

            this.ChangeState(State.HaveOil);
            return true; // Allow the item to be taken
        }

        if (item is SpiceItem && item.IsState(SpiceItem.State.Normal))
        {
            return TakeSpiceItem(item as SpiceItem);
        }
        return base.OnTake(item);
    }

    public bool TakeSpiceItem(SpiceItem spiceItem)
    {
        if (this.IsState(State.HaveOil) && spiceItem.IsSpiceType(SpiceType.Onion))
        {
            spiceItem.OnMove(TF.position + Vector3.up * 0.4f + Vector3.left * 0.15f, Quaternion.identity, 0.2f);
            spiceItem.ChangeState(SpiceItem.State.Pouring);

            StartCoroutine(WaitToPourOnion());
            return true;
        }

        if (this.IsState(State.HaveOnion) && spiceItem.IsSpiceType(SpiceType.Garlic))
        {
            spiceItem.OnMove(TF.position + Vector3.up * 0.4f + Vector3.left * 0.15f, Quaternion.identity, 0.2f);
            spiceItem.ChangeState(SpiceItem.State.Pouring);

            StartCoroutine(WaitToPourGarlic());
            return true;
        }

        if (this.IsState(State.Meat) && spiceItem.IsSpiceType(SpiceType.Meat))
        {
            spiceItem.OnMove(TF.position + Vector3.up * 0.4f + Vector3.left * 0.15f, Quaternion.identity, 0.2f);
            spiceItem.ChangeState(SpiceItem.State.Pouring);

            StartCoroutine(WaitToPourMeat());
            return true;
        }

        if (this.IsState(State.Pepper) && spiceItem.IsSpiceType(SpiceType.Pepper))
        {
            spiceItem.OnMove(TF.position + Vector3.up * 0.4f + Vector3.left * 0.15f, Quaternion.identity, 0.2f);
            spiceItem.ChangeState(SpiceItem.State.Pouring);

            StartCoroutine(WaitToPourPepper());
            return true;
        }

        if (this.IsState(State.Chili) && spiceItem.IsSpiceType(SpiceType.Chili))
        {
            spiceItem.OnMove(TF.position + Vector3.up * 0.4f + Vector3.left * 0.15f, Quaternion.identity, 0.2f);
            spiceItem.ChangeState(SpiceItem.State.Pouring);

            StartCoroutine(WaitToPourChili());
            return true;
        }

        if (this.IsState(State.Salt) && spiceItem.IsSpiceType(SpiceType.Salt))
        {
            spiceItem.OnMove(TF.position + Vector3.up * 0.4f + Vector3.left * 0.15f, Quaternion.identity, 0.2f);
            spiceItem.ChangeState(SpiceItem.State.Pouring);

            StartCoroutine(WaitToPourSalt());
            return true;
        }

        if (this.IsState(State.Turmeric) && spiceItem.IsSpiceType(SpiceType.Tumeric))
        {
            spiceItem.OnMove(TF.position + Vector3.up * 0.4f + Vector3.left * 0.15f, Quaternion.identity, 0.2f);
            spiceItem.ChangeState(SpiceItem.State.Pouring);

            StartCoroutine(WaitToPourTurmeric());
            return true;
        }

        if (this.IsState(State.Tomato) && spiceItem.IsSpiceType(SpiceType.Tomato))
        {
            spiceItem.OnMove(TF.position + Vector3.up * 0.4f + Vector3.left * 0.15f, Quaternion.identity, 0.2f);
            spiceItem.ChangeState(SpiceItem.State.Pouring);

            StartCoroutine(WaitToPourTomato());
            return true;
        }

        if (this.IsState(State.Oregano) && spiceItem.IsSpiceType(SpiceType.Oregano))
        {
            spiceItem.OnMove(TF.position + Vector3.up * 0.4f + Vector3.left * 0.15f, Quaternion.identity, 0.2f);
            spiceItem.ChangeState(SpiceItem.State.Pouring);

            StartCoroutine(WaitToPourOregano());
            return true;
        }
        return false;
    }

    public void TakeMeat()
    {
        anim.Play(animMeat);
        meatGo.localScale = Vector3.one * sizeFloat[takeMeatCount];
        takeMeatCount++;

        if (takeMeatCount >= 3)
        {
            ChangeState(State.WaitForTurnOff);
        }
    }

    public void OnClickButton(bool isOn)
    {
        if (isOn == true && IsState(Pan.State.Normal))// neu la bat thi phai o state haveitem
        {
            ChangeState(Pan.State.HeatOn);
        }

        if (isOn == false && IsState(Pan.State.WaitForTurnOff))
        {
            ChangeState(Pan.State.Done);
        }
    }

    public void SetCookingRate(Vector3 point)
    {
        if (IsState(State.Mixing1) || IsState(State.Mixing2))
        {
            cookingRate += Time.deltaTime * 0.5f;
        }

        if (IsState(State.Mixing1))
        {
            onionFryControl.SetRipeRate(cookingRate);
            garlicFryControl.SetRipeRate(cookingRate);

            if (cookingRate >= 1)
            {
                ChangeState(State.Meat);
            }
        }

        if (IsState(State.Mixing2))
        {
            anim.Play(animMeat);
            onionFryControl.SetRipeRate(cookingRate);
            garlicFryControl.SetRipeRate(cookingRate);
            pepperFryControl.SetRipeRate(cookingRate);
            chiliFryControl.SetRipeRate(cookingRate);
            saltFryControl.SetRipeRate(cookingRate);
            turmericFryControl.SetRipeRate(cookingRate);
            oreganoFryControl.SetRipeRate(cookingRate);

            if (cookingRate >= 2f && cookingRate < 3f)
            {
                rawMeatAlpha.SetAlpha(3 - cookingRate);
                tomatoSauceAlpha.SetAlpha(3 - cookingRate);
                mixMeatAlpha.SetAlpha(cookingRate - 2f);
            }

            if (cookingRate >= 3f && cookingRate < 4f)
            {
                mixMeatAlpha.SetAlpha(4f - cookingRate);
                doneMeatAlpha.SetAlpha(cookingRate - 3f);
            }

            if (cookingRate >= 4f)
            {
                ChangeState(State.DoneMix2); // Change to DoneMix2 state when cooking is complete
            }
        }
    }


    #region Delay Time To Change Anim
    IEnumerator WaitToPourOnion()
    {
        yield return WaitForSecondCache.Get(0.9f);
        ChangeState(State.HaveOnion);
    }

    IEnumerator WaitToPourGarlic()
    {
        yield return WaitForSecondCache.Get(0.9f);
        ChangeState(State.HaveGarlic);
    }

    IEnumerator WaitToPourMeat()
    {
        yield return WaitForSecondCache.Get(0.7f);
        rawMeatAlpha.DoAlpha(1f, 0.5f);
        anim.Play(animMeat);
        ChangeState(State.Pepper);
    }

    IEnumerator WaitToPourPepper()
    {
        yield return WaitForSecondCache.Get(0.9f);
        pepperDrop2D.OnActive();
        ChangeState(State.Chili);
    }

    IEnumerator WaitToPourChili()
    {
        yield return WaitForSecondCache.Get(0.9f);
        chiliDrop2D.OnActive();
        ChangeState(State.Salt);
    }

    IEnumerator WaitToPourSalt()
    {
        yield return WaitForSecondCache.Get(0.9f);
        saltDrop2D.OnActive();
        ChangeState(State.Turmeric);
    }

    IEnumerator WaitToPourTurmeric()
    {
        yield return WaitForSecondCache.Get(0.9f);
        turmericDrop2D.OnActive();
        ChangeState(State.Tomato);
    }

    IEnumerator WaitToPourTomato()
    {
        yield return WaitForSecondCache.Get(0.3f);
        anim.Play(animTomato);
        ChangeState(State.Oregano);
    }

    IEnumerator WaitToPourOregano()
    {
        yield return WaitForSecondCache.Get(0.9f);
        oreganoDrop2D.OnActive();
        ChangeState(State.Mixing2);

    }
    #endregion
}

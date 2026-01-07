using UnityEngine;

public class UpgradeNotification : MonoBehaviour
{
    public ParticleSystem Magnete;
    public ParticleSystem Weight;
    public ParticleSystem Speed;
    public ParticleSystem Fuel;
    public ParticleSystem Refil;
    public ParticleSystem UnderZone;
    public void MagneteUpgrade()
    {
        if (Magnete != null)
            Magnete.Play();
        if (UnderZone != null)
            UnderZone.Play();
    }
    public void WeightUpgrade()
    {
        if (Weight != null)
            Weight.Play();
        if (UnderZone != null)
            UnderZone.Play();
    }
    public void SpeedUpgrade()
    {
        if (Speed != null)
            Speed.Play();
        if (UnderZone != null)
            UnderZone.Play();
    }
    public void FuelUpgrade()
    {
        if (Fuel != null)
            Fuel.Play();
        if (UnderZone != null)
            UnderZone.Play();
    }
    public void RefilUpgrade()
    {
        if (Refil != null)
            Refil.Play();
        if (UnderZone != null)
            UnderZone.Play();
    }

}

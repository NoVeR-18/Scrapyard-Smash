using UnityEngine;

public class UpgradeNotification : MonoBehaviour
{
    public ParticleSystem Magnete;
    public ParticleSystem Weight;
    public ParticleSystem Speed;
    public ParticleSystem Fuel;
    public ParticleSystem Refil;

    public void MagneteUpgrade()
    {
        if (Magnete != null)
            Magnete.Play();
    }
    public void WeightUpgrade()
    {
        if (Weight != null)
            Weight.Play();
    }
    public void SpeedUpgrade()
    {
        if (Speed != null)
            Speed.Play();
    }
    public void FuelUpgrade()
    {
        if (Fuel != null)
            Fuel.Play();
    }
    public void RefilUpgrade()
    {
        if (Refil != null)
            Refil.Play();
    }

}

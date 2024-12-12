using System.Linq;
using UnityEngine;

public enum VehicleDetail
{
    Helicopter,
    Magnetic
}

public class Details : MonoBehaviour
{
    public VehicleDetail type;


    private void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Player")
        {
            if (type == VehicleDetail.Helicopter)
            {
                var helicopter = LevelManager.Instance.vechicles.OfType<Helicopter>().FirstOrDefault();

                if (helicopter != null)
                {
                    helicopter.ColectDetails();
                    Destroy(gameObject);
                }
            }
            else if (type == VehicleDetail.Magnetic)
            {

                var magnete = LevelManager.Instance.vechicles.OfType<Magnete>().FirstOrDefault();

                if (magnete != null)
                {
                    magnete.ColectDetails();
                    Destroy(gameObject);
                }
            }
        }
    }
}

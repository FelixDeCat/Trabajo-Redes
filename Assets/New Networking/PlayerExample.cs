using UnityEngine;

public class PlayerExample : MonoBehaviour
{
	void Update () {
       
        if (Input.GetKeyDown(KeyCode.A))
            new PacketBase(PacketIDs.Move_Command).Add("14,7,8,9,25,4,77").Add(new Vector3(75, 12, 14)).Send();

        if (Input.GetKeyDown(KeyCode.S))
            new PacketBase(PacketIDs.Select_Command).Add("14,7,8,9,25,4,77").Send();

        if (Input.GetKeyDown(KeyCode.D))
            new PacketBase(PacketIDs.Attack_Command).Add("14,7,8,9,25,4,77").Add(123f).Send();

        if (Input.GetKeyDown(KeyCode.F))
            new PacketBase(PacketIDs.Old_School_Command).Add("Old_School_Move/14,7,8,9,25,4,77-12,24,32").Send();
    }
}

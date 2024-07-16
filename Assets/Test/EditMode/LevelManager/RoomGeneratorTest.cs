using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomGeneratorTest
{
    //MethodName_StateUnderTest_ExpectedBehavior
    [Test]
    public void GenerateRooms_MethodResultContainsNullReferences_False()
    {
        try
        {
            //Arange

            GameObject gameObject = new GameObject();
            RoomGenerator roomGenerator = gameObject.AddComponent<RoomGenerator>();

            //Act

            List<List<PathUnit>[]> roomStructure = roomGenerator.GenerateRooms();

            //Assert


            for (int i = 0; i < roomStructure.Count; i++)
            {
                for (int j = 0;j < roomStructure[i].Length; j++)
                {
                    for (int k = 0; k < roomStructure[i][j].Count; k++)
                    {
                        Assert.IsFalse(roomStructure[i][j][k] == null);
                    }
                }
            }
        }
        catch (System.Exception ex)
        {
            Assert.Fail(ex.Message);
        }
    }
}

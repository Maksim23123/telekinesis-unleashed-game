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

            foreach (var pathUnit in roomStructure)
            {
                Assert.IsFalse(pathUnit == null);
            }
        }
        catch (System.Exception ex)
        {
            Assert.Fail(ex.Message);
        }
    }
}

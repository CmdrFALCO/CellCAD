using Xunit;
using CellCAD.models;

namespace CellCAD.Tests;

public class UnitTest1
{
    [Fact]
    public void Smoke_Test_Project_Wires_Up()
    {
        var info = new CellInfo();
        Assert.Equal("Prototype", info.Name);
    }
}
